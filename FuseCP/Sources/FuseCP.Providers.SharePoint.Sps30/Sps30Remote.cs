// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Win32;

using FuseCP.Providers.OS;
using FuseCP.Providers.Utils;
using FuseCP.Server.Utils;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;

namespace FuseCP.Providers.SharePoint
{
	public class Sps30Remote : MarshalByRefObject
	{
		private static void RunAsCurrentUser(Action action)
		{
			using WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsIdentity.RunImpersonated(identity.AccessToken, action);
		}

		private static T RunAsCurrentUser<T>(Func<T> action)
		{
			using WindowsIdentity identity = WindowsIdentity.GetCurrent();
			return WindowsIdentity.RunImpersonated(identity.AccessToken, action);
		}

        public void ExtendVirtualServer(SharePointSite site, bool exclusiveNTLM)
		{
			try
			{
				RunAsCurrentUser(() =>
				{
					string siteUrl = "http://" + site.Name;

					// check input parameters
					if (String.IsNullOrEmpty(site.RootFolder)
						|| !Directory.Exists(site.RootFolder))
						throw new Exception("Could not create SharePoint site, because web site root folder does not exist. Open web site properties and click \"Update\" button to re-create site folder.");

					SPWebApplication app = SPWebApplication.Lookup(new Uri(siteUrl));
					if (app != null)
						throw new Exception("SharePoint is already installed on this web site.");

					SPFarm farm = SPFarm.Local;
					SPWebApplicationBuilder builder = new SPWebApplicationBuilder(farm);
					builder.ApplicationPoolId = site.ApplicationPool;
					builder.DatabaseServer = site.DatabaseServer;
					builder.DatabaseName = site.DatabaseName;
					builder.DatabaseUsername = site.DatabaseUser;
					builder.DatabasePassword = site.DatabasePassword;

					builder.ServerComment = site.Name;
					builder.HostHeader = site.Name;
					builder.Port = 80;

					builder.RootDirectory = new DirectoryInfo(site.RootFolder);
					builder.DefaultZoneUri = new Uri(siteUrl);
                    builder.UseNTLMExclusively = exclusiveNTLM;

					app = builder.Create();
					app.Name = site.Name;

					app.Sites.Add(siteUrl, null, null, (uint)site.LocaleID, null, site.OwnerLogin, null, site.OwnerEmail);

					app.Update();
					app.Provision();
				});
			}
			catch (Exception ex)
			{
				try
				{
					// try to delete app if it was created
					SPWebApplication app = SPWebApplication.Lookup(new Uri("http://" + site.Name));
					if (app != null)
						app.Delete();
				}
				catch { /* nop */ }

				throw new Exception("Error creating SharePoint site", ex);
			}
		}

		public void UnextendVirtualServer(string url, bool deleteContent)
		{
			try
			{
				RunAsCurrentUser(() =>
				{
					Uri uri = new Uri("http://" + url);
					SPWebApplication app = SPWebApplication.Lookup(uri);
					if (app == null)
						return;

					SPGlobalAdmin adm = new SPGlobalAdmin();
					adm.UnextendVirtualServer(uri, false);

					app.Delete();
				});
			}
			catch (Exception ex)
			{
				throw new Exception("Could not uninstall SharePoint from the web site", ex);
			}
		}

		public string BackupVirtualServer(string url, string fileName, bool zipBackup)
		{
			try
			{
				return RunAsCurrentUser(() =>
				{
					string tempPath = Path.GetTempPath();
					string bakFile = Path.Combine(tempPath, (zipBackup
						? StringUtils.CleanIdentifier(url) + ".bsh"
						: StringUtils.CleanIdentifier(fileName)));

					SPWebApplication app = SPWebApplication.Lookup(new Uri("http://" + url));
					if (app == null)
						throw new Exception("SharePoint is not installed on the web site");

					app.Sites.Backup("http://" + url, bakFile, true);

					if (zipBackup)
					{
						string zipFile = Path.Combine(tempPath, fileName);
						string zipRoot = Path.GetDirectoryName(bakFile);

						FileUtils.ZipFiles(zipFile, zipRoot, new string[] { Path.GetFileName(bakFile) });
						FileUtils.DeleteFile(bakFile);

						bakFile = zipFile;
					}

					return bakFile;
				});
			}
			catch (Exception ex)
			{
				throw new Exception("Could not backup SharePoint site", ex);
			}
		}

		public void RestoreVirtualServer(string url, string fileName)
		{
			try
			{
				RunAsCurrentUser(() =>
				{
					SPWebApplication app = SPWebApplication.Lookup(new Uri("http://" + url));
					if (app == null)
						throw new Exception("SharePoint is not installed on the web site");

					string tempPath = Path.GetTempPath();

					string expandedFile = fileName;
					if (Path.GetExtension(fileName).ToLower() == ".zip")
					{
						expandedFile = FileUtils.UnzipFiles(fileName, tempPath)[0];
						FileUtils.DeleteFile(fileName);
					}

					SPSiteAdministration site = new SPSiteAdministration("http://" + url);
					site.Delete(false);

					app.Sites.Restore("http://" + url, expandedFile, true);
					FileUtils.DeleteFile(expandedFile);
				});
			}
			catch (Exception ex)
			{
				throw new Exception("Could not restore SharePoint site", ex);
			}
		}

		public string[] GetInstalledWebParts(string url)
		{
			try
			{
				return RunAsCurrentUser(() =>
				{
					SPGlobalAdmin adm = new SPGlobalAdmin();
					string lines = adm.EnumWPPacks(null, "http://" + url, false);

					List<string> list = new List<string>();

					if (!String.IsNullOrEmpty(lines))
					{
						string line = null;
						StringReader reader = new StringReader(lines);
						while ((line = reader.ReadLine()) != null)
						{
							line = line.Trim();
							int commaIdx = line.IndexOf(",");
							if (!String.IsNullOrEmpty(line) && commaIdx != -1)
								list.Add(line.Substring(0, commaIdx));
						}
					}

					return list.ToArray();
				});
			}
			catch (Exception ex)
			{
				throw new Exception("Error reading web parts packages", ex);
			}
		}

		public void InstallWebPartsPackage(string url, string fileName)
		{
			try
			{
				RunAsCurrentUser(() =>
				{
					string tempPath = Path.GetTempPath();

					string expandedFile = fileName;
					if (Path.GetExtension(fileName).ToLower() == ".zip")
					{
						expandedFile = FileUtils.UnzipFiles(fileName, tempPath)[0];
						FileUtils.DeleteFile(fileName);
					}

					StringWriter errors = new StringWriter();

					SPGlobalAdmin adm = new SPGlobalAdmin();
					int result = adm.AddWPPack(expandedFile, null, 0, "http://" + url, false, true, errors);
					if (result > 1)
						throw new Exception("Error installing web parts package: " + errors.ToString());

					FileUtils.DeleteFile(expandedFile);
				});

			}
			catch (Exception ex)
			{
				throw new Exception("Could not install web parts package", ex);
			}
		}

		public void DeleteWebPartsPackage(string url, string packageName)
		{
			try
			{
				RunAsCurrentUser(() =>
				{
					StringWriter errors = new StringWriter();

					SPGlobalAdmin adm = new SPGlobalAdmin();
					int result = adm.RemoveWPPack(packageName, 0, "http://" + url, errors);
					if (result > 1)
						throw new Exception("Error uninstalling web parts package: " + errors.ToString());
				});
			}
			catch (Exception ex)
			{
				throw new Exception("Could not uninstall web parts package", ex);
			}
		}
	}
}
