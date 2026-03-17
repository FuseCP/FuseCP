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
using System.Data;
using System.Web;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using FuseCP.Web.Services;
using System.ComponentModel;
using FuseCP.Providers;
using FuseCP.Providers.OS;
using FuseCP.Server.Utils;
using FuseCP.Providers.DNS;
using FuseCP.Providers.DomainLookup;
using System.Collections.Generic;

namespace FuseCP.Server
{
	/// <summary>
	/// Summary description for OperatingSystem
	/// </summary>
	[WebService(Namespace = "http://smbsaas/fusecp/server/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[Policy("ServerPolicy")]
	[ToolboxItem(false)]
	public class OperatingSystem : HostingServiceProviderWebService, IWindowsOperatingSystem, IUnixOperatingSystem
	{
		protected override IHostingServiceProvider Provider
		{
			get
			{
				try
				{
					return base.Provider;
				}
				catch { }
				return (IHostingServiceProvider)OSInfo.Current;
			}
		}
		private IOperatingSystem OSProvider
		{
			get
			{
				var p = Provider as IOperatingSystem;
				if (p == null)
					throw new InvalidOperationException(
						$"No OS provider is available for this server. " +
						$"Ensure a compatible OS provider is deployed and the provider type '{ProviderSettings.ProviderType}' can be resolved.");
				return p;
			}
		}
		private IWindowsOperatingSystem WinProvider
		{
			get
			{
				if (Provider is IWindowsOperatingSystem win) return win;
				else throw new NotSupportedException("This command is only supported on a Windows Server.");
			}
		}
		private IUnixOperatingSystem UnixProvider
		{
			get
			{
				if (Provider is IUnixOperatingSystem unix) return unix;
				else throw new NotSupportedException("This command is only supported on a Unix Server.");
			}
		}

		public Installer WinGet => WinProvider.WinGet;

		public Installer Chocolatey => WinProvider.Chocolatey;

		public Shell Cmd => WinProvider.Cmd;

		public Shell PowerShell => WinProvider.PowerShell;

		public Shell DefaultShell => OSProvider.DefaultShell;

		public Installer DefaultInstaller => OSProvider.DefaultInstaller;

		public Shell Sh => UnixProvider.Sh;

		public Shell Bash => UnixProvider.Bash;

		public Installer Apt => UnixProvider.Apt;

		public Installer Yum => UnixProvider.Yum;

		public Installer Brew => UnixProvider.Brew;

		#region Files
		[WebMethod, SoapHeader("settings")]
		public string PathCombine(params string[] segments)
		{
			try
			{
				Log.WriteStart("'{0}' PathCombine", ProviderSettings.ProviderName);
				string result = OSProvider.PathCombine(segments);
				Log.WriteEnd("'{0}' PathCombine", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' PathCombine", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public string CreatePackageFolder(string initialPath)
		{
			try
			{
				Log.WriteStart("'{0}' CreatePackageFolder", ProviderSettings.ProviderName);
				string result = OSProvider.CreatePackageFolder(initialPath);
				Log.WriteEnd("'{0}' CreatePackageFolder", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CreatePackageFolder", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public bool FileExists(string path)
		{
			try
			{
				Log.WriteStart("'{0}' FileExists", ProviderSettings.ProviderName);
				bool result = OSProvider.FileExists(path);
				Log.WriteEnd("'{0}' FileExists", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' FileExists", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public bool DirectoryExists(string path)
		{
			try
			{
				Log.WriteStart("'{0}' DirectoryExists", ProviderSettings.ProviderName);
				bool result = OSProvider.DirectoryExists(path);
				Log.WriteEnd("'{0}' DirectoryExists", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' DirectoryExists", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemFile GetFile(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetFile", ProviderSettings.ProviderName);
				SystemFile result = OSProvider.GetFile(path);
				Log.WriteEnd("'{0}' GetFile", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFile", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemFile[] GetFiles(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetFiles", ProviderSettings.ProviderName);
				SystemFile[] result = OSProvider.GetFiles(path);
				Log.WriteEnd("'{0}' GetFiles", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFiles", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemFile[] GetDirectoriesRecursive(string rootFolder, string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetDirectoriesRecursive", ProviderSettings.ProviderName);
				SystemFile[] result = OSProvider.GetDirectoriesRecursive(rootFolder, path);
				Log.WriteEnd("'{0}' GetDirectoriesRecursive", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetDirectoriesRecursive", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemFile[] GetFilesRecursive(string rootFolder, string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetFilesRecursive", ProviderSettings.ProviderName);
				SystemFile[] result = OSProvider.GetFilesRecursive(rootFolder, path);
				Log.WriteEnd("'{0}' GetFilesRecursive", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFilesRecursive", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemFile[] GetFilesRecursiveByPattern(string rootFolder, string path, string pattern)
		{
			try
			{
				Log.WriteStart("'{0}' GetFilesRecursiveByPattern", ProviderSettings.ProviderName);
				SystemFile[] result = OSProvider.GetFilesRecursiveByPattern(rootFolder, path, pattern);
				Log.WriteEnd("'{0}' GetFilesRecursiveByPattern", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFilesRecursiveByPattern", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public byte[] GetFileBinaryContent(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetFileBinaryContent", ProviderSettings.ProviderName);
				byte[] result = OSProvider.GetFileBinaryContent(path);
				Log.WriteEnd("'{0}' GetFileBinaryContent", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFileBinaryContent", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public byte[] GetFileBinaryContentUsingEncoding(string path, string encoding)
		{
			try
			{
				Log.WriteStart("'{0}' GetFileBinaryContentUsingEncoding", ProviderSettings.ProviderName);
				byte[] result = OSProvider.GetFileBinaryContentUsingEncoding(path, encoding);
				Log.WriteEnd("'{0}' GetFileBinaryContentUsingEncoding", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFileBinaryContentUsingEncoding", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public byte[] GetFileBinaryChunk(string path, int offset, int length)
		{
			try
			{
				Log.WriteStart("'{0}' GetFileBinaryChunk", ProviderSettings.ProviderName);
				byte[] result = OSProvider.GetFileBinaryChunk(path, offset, length);
				Log.WriteEnd("'{0}' GetFileBinaryChunk", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFileBinaryContent", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public string GetFileTextContent(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetFileTextContent", ProviderSettings.ProviderName);
				string result = OSProvider.GetFileTextContent(path);
				Log.WriteEnd("'{0}' GetFileTextContent", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFileTextContent", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CreateFile(string path)
		{
			try
			{
				Log.WriteStart("'{0}' CreateFile", ProviderSettings.ProviderName);
				OSProvider.CreateFile(path);
				Log.WriteEnd("'{0}' CreateFile", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CreateFile", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CreateDirectory(string path)
		{
			try
			{
				Log.WriteStart("'{0}' CreateDirectory", ProviderSettings.ProviderName);
				OSProvider.CreateDirectory(path);
				Log.WriteEnd("'{0}' CreateDirectory", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CreateDirectory", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void ChangeFileAttributes(string path, DateTime createdTime, DateTime changedTime)
		{
			try
			{
				Log.WriteStart("'{0}' ChangeFileAttributes", ProviderSettings.ProviderName);
				OSProvider.ChangeFileAttributes(path, createdTime, changedTime);
				Log.WriteEnd("'{0}' ChangeFileAttributes", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ChangeFileAttributes", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void DeleteFile(string path)
		{
			try
			{
				Log.WriteStart("'{0}' DeleteFile", ProviderSettings.ProviderName);
				OSProvider.DeleteFile(path);
				Log.WriteEnd("'{0}' DeleteFile", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' DeleteFile", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void DeleteFiles(string[] files)
		{
			try
			{
				Log.WriteStart("'{0}' DeleteFiles", ProviderSettings.ProviderName);
				OSProvider.DeleteFiles(files);
				Log.WriteEnd("'{0}' DeleteFiles", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' DeleteFiles", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void DeleteEmptyDirectories(string[] directories)
		{
			try
			{
				Log.WriteStart("'{0}' DeleteEmptyDirectories", ProviderSettings.ProviderName);
				OSProvider.DeleteEmptyDirectories(directories);
				Log.WriteEnd("'{0}' DeleteEmptyDirectories", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' DeleteEmptyDirectories", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void UpdateFileBinaryContent(string path, byte[] content)
		{
			try
			{
				Log.WriteStart("'{0}' UpdateFileBinaryContent", ProviderSettings.ProviderName);
				OSProvider.UpdateFileBinaryContent(path, content);
				Log.WriteEnd("'{0}' UpdateFileBinaryContent", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' UpdateFileBinaryContent", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void UpdateFileBinaryContentUsingEncoding(string path, byte[] content, string encoding)
		{
			try
			{
				Log.WriteStart("'{0}' UpdateFileBinaryContentUsingEncoding", ProviderSettings.ProviderName);
				OSProvider.UpdateFileBinaryContentUsingEncoding(path, content, encoding);
				Log.WriteEnd("'{0}' UpdateFileBinaryContentUsingEncoding", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' UpdateFileBinaryContentUsingEncoding", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void AppendFileBinaryContent(string path, byte[] chunk)
		{
			try
			{
				Log.WriteStart("'{0}' AppendFileBinaryContent", ProviderSettings.ProviderName);
				OSProvider.AppendFileBinaryContent(path, chunk);
				Log.WriteEnd("'{0}' AppendFileBinaryContent", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' AppendFileBinaryContent", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void UpdateFileTextContent(string path, string content)
		{
			try
			{
				Log.WriteStart("'{0}' UpdateFileTextContent", ProviderSettings.ProviderName);
				OSProvider.UpdateFileTextContent(path, content);
				Log.WriteEnd("'{0}' UpdateFileTextContent", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' UpdateFileTextContent", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void MoveFile(string sourcePath, string destinationPath)
		{
			try
			{
				Log.WriteStart("'{0}' MoveFile", ProviderSettings.ProviderName);
				OSProvider.MoveFile(sourcePath, destinationPath);
				Log.WriteEnd("'{0}' MoveFile", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' MoveFile", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CopyFile(string sourcePath, string destinationPath)
		{
			try
			{
				Log.WriteStart("'{0}' CopyFile", ProviderSettings.ProviderName);
				OSProvider.CopyFile(sourcePath, destinationPath);
				Log.WriteEnd("'{0}' CopyFile", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CopyFile", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public OSPlatformInfo GetOSPlatform()
		{
			try
			{
				Log.WriteStart("'{0}' OSPlatform", ProviderSettings.ProviderName);
				var res = OSProvider.GetOSPlatform();
				Log.WriteEnd("'{0}' OSPlatform", ProviderSettings.ProviderName);
				return res;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' OSPlatform", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void ZipFiles(string zipFile, string rootPath, string[] files)
		{
			try
			{
				Log.WriteStart("'{0}' ZipFiles", ProviderSettings.ProviderName);
				OSProvider.ZipFiles(zipFile, rootPath, files);
				Log.WriteEnd("'{0}' ZipFiles", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ZipFiles", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public string[] UnzipFiles(string zipFile, string destFolder)
		{
			try
			{
				Log.WriteStart("'{0}' UnzipFiles", ProviderSettings.ProviderName);
				string[] result = OSProvider.UnzipFiles(zipFile, destFolder);
				Log.WriteEnd("'{0}' UnzipFiles", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' UnzipFiles", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CreateBackupZip(string zipFile, string rootPath)
		{
			try
			{
				Log.WriteStart("'{0}' ZipFiles", ProviderSettings.ProviderName);
				OSProvider.CreateBackupZip(zipFile, rootPath);
				Log.WriteEnd("'{0}' ZipFiles", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ZipFiles", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CreateAccessDatabase(string databasePath)
		{
			try
			{
				Log.WriteStart("'{0}' CreateAccessDatabase", ProviderSettings.ProviderName);
				WinProvider.CreateAccessDatabase(databasePath);
				Log.WriteEnd("'{0}' CreateAccessDatabase", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CreateAccessDatabase", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public UserPermission[] GetGroupNtfsPermissions(string path, UserPermission[] users, string usersOU)
		{
			try
			{
				Log.WriteStart("'{0}' GetGroupNtfsPermissions", ProviderSettings.ProviderName);
				UserPermission[] result = WinProvider.GetGroupNtfsPermissions(path, users, usersOU);
				Log.WriteEnd("'{0}' GetGroupNtfsPermissions", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetGroupNtfsPermissions", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void GrantGroupNtfsPermissions(string path, UserPermission[] users, string usersOU, bool resetChildPermissions)
		{
			try
			{
				Log.WriteStart("'{0}' GrantGroupNtfsPermissions", ProviderSettings.ProviderName);
				WinProvider.GrantGroupNtfsPermissions(path, users, usersOU, resetChildPermissions);
				Log.WriteEnd("'{0}' GrantGroupNtfsPermissions", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GrantGroupNtfsPermissions", ProviderSettings.ProviderName), ex);
				throw;
			}
		}


		[WebMethod, SoapHeader("settings")]
		public void SetQuotaLimitOnFolder(string folderPath, string shareNameDrive, QuotaType quotaType, string quotaLimit, int mode, string wmiUserName, string wmiPassword)
		{
			try
			{
				Log.WriteStart("'{0}' SetQuotaLimitOnFolder", ProviderSettings.ProviderName);
				OSProvider.SetQuotaLimitOnFolder(folderPath, shareNameDrive, quotaType, quotaLimit, mode, wmiUserName, wmiPassword);
				Log.WriteEnd("'{0}' SetQuotaLimitOnFolder", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' SetQuotaLimitOnFolder", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public Quota GetQuotaOnFolder(string folderPath, string wmiUserName, string wmiPassword)
		{
			try
			{
				Log.WriteStart("'{0}' GetQuotaOnFolder", ProviderSettings.ProviderName);
				var result = OSProvider.GetQuotaOnFolder(folderPath, wmiUserName, wmiPassword);
				Log.WriteEnd("'{0}' GetQuotaOnFolder", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetQuotaOnFolder", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void DeleteDirectoryRecursive(string rootPath)
		{
			try
			{
				Log.WriteStart("'{0}' DeleteDirectoryRecursive", ProviderSettings.ProviderName);
				OSProvider.DeleteDirectoryRecursive(rootPath);
				Log.WriteEnd("'{0}' DeleteDirectoryRecursive", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' DeleteDirectoryRecursive", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public bool CheckFileServicesInstallation()
		{
			try
			{
				Log.WriteStart("'{0}' CheckFileServicesInstallation", ProviderSettings.ProviderName);
				bool bResult = WinProvider.CheckFileServicesInstallation();
				Log.WriteEnd("'{0}' CheckFileServicesInstallation", ProviderSettings.ProviderName);
				return bResult;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CheckFileServicesInstallation", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public bool InstallFsrmService()
		{
			try
			{
				Log.WriteStart("'{0}' InstallFsrmService", ProviderSettings.ProviderName);
				bool bResult = WinProvider.InstallFsrmService();
				Log.WriteEnd("'{0}' InstallFsrmService", ProviderSettings.ProviderName);
				return bResult;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' InstallFsrmService", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion

		#region Synchronizing
		[WebMethod, SoapHeader("settings")]
		public FolderGraph GetFolderGraph(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetFolderGraph", ProviderSettings.ProviderName);
				FolderGraph result = OSProvider.GetFolderGraph(path);
				Log.WriteEnd("'{0}' GetFolderGraph", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetFolderGraph", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void ExecuteSyncActions(FileSyncAction[] actions)
		{
			try
			{
				Log.WriteStart("'{0}' ExecuteSyncActions", ProviderSettings.ProviderName);
				OSProvider.ExecuteSyncActions(actions);
				Log.WriteEnd("'{0}' ExecuteSyncActions", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ExecuteSyncActions", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion

		#region ODBC DSNs
		[WebMethod, SoapHeader("settings")]
		public string[] GetInstalledOdbcDrivers()
		{
			try
			{
				Log.WriteStart("'{0}' GetInstalledOdbcDrivers", ProviderSettings.ProviderName);
				string[] result = WinProvider.GetInstalledOdbcDrivers();
				Log.WriteEnd("'{0}' GetInstalledOdbcDrivers", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetInstalledOdbcDrivers", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public string[] GetDSNNames()
		{
			try
			{
				Log.WriteStart("'{0}' GetDSNNames", ProviderSettings.ProviderName);
				string[] result = WinProvider.GetDSNNames();
				Log.WriteEnd("'{0}' GetDSNNames", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetDSNNames", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemDSN GetDSN(string dsnName)
		{
			try
			{
				Log.WriteStart("'{0}' GetDSN", ProviderSettings.ProviderName);
				SystemDSN result = WinProvider.GetDSN(dsnName);
				Log.WriteEnd("'{0}' GetDSN", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetDSN", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CreateDSN(SystemDSN dsn)
		{
			try
			{
				Log.WriteStart("'{0}' CreateDSN", ProviderSettings.ProviderName);
				WinProvider.CreateDSN(dsn);
				Log.WriteEnd("'{0}' CreateDSN", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CreateDSN", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void UpdateDSN(SystemDSN dsn)
		{
			try
			{
				Log.WriteStart("'{0}' UpdateDSN", ProviderSettings.ProviderName);
				WinProvider.UpdateDSN(dsn);
				Log.WriteEnd("'{0}' UpdateDSN", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' UpdateDSN", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void DeleteDSN(string dsnName)
		{
			try
			{
				Log.WriteStart("'{0}' DeleteDSN", ProviderSettings.ProviderName);
				WinProvider.DeleteDSN(dsnName);
				Log.WriteEnd("'{0}' DeleteDSN", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' DeleteDSN", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion ODBC DSNs

		#region Unix Permissions
		[WebMethod, SoapHeader("settings")]
		public Providers.OS.UnixFileMode GetUnixPermissions(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetUnixPermissions", ProviderSettings.ProviderName);
				var result = UnixProvider.GetUnixPermissions(path);
				Log.WriteEnd("'{0}' GetUnixPermissions", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetUnixPermissions", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void GrantUnixPermissions(string path, Providers.OS.UnixFileMode mode, bool resetChildPermissions = false)
		{
			try
			{
				Log.WriteStart("'{0}' GrantUnixPermissions", ProviderSettings.ProviderName);
				UnixProvider.GrantUnixPermissions(path, mode, resetChildPermissions);
				Log.WriteEnd("'{0}' GrantUnixPermissions", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GrantUnixPermissions", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void ChangeUnixFileOwner(string path, string owner, string group, bool applyToChildren = false)
		{
			try
			{
				Log.WriteStart("'{0}' ChangeUnixFileOwner", ProviderSettings.ProviderName);
				UnixProvider.ChangeUnixFileOwner(path, owner, group, applyToChildren);
				Log.WriteEnd("'{0}' ChangeUnixFileOwner", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ChangeUnixFileOwner", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		[WebMethod, SoapHeader("settings")]
		public UnixFileOwner GetUnixFileOwner(string path)
		{
			try
			{
				Log.WriteStart("'{0}' GetUnixFileOwner", ProviderSettings.ProviderName);
				var result = UnixProvider.GetUnixFileOwner(path);
				Log.WriteEnd("'{0}' GetUnixFileOwner", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetUnixFileOwner", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion Unix Permissions

		#region Terminal Services
		[WebMethod, SoapHeader("settings")]
		public TerminalSession[] GetTerminalServicesSessions()
		{
			try
			{
				Log.WriteStart("'{0}' GetTerminalServicesSessions", ProviderSettings.ProviderName);
				var result = WinProvider.GetTerminalServicesSessions();
				Log.WriteEnd("'{0}' GetTerminalServicesSessions", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetTerminalServicesSessions", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void CloseTerminalServicesSession(int sessionId)
		{
			try
			{
				Log.WriteStart("'{0}' CloseTerminalServicesSession", ProviderSettings.ProviderName);
				WinProvider.CloseTerminalServicesSession(sessionId);
				Log.WriteEnd("'{0}' CloseTerminalServicesSession", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' CloseTerminalServicesSession", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion Terminal Services

		#region Event Log
		[WebMethod, SoapHeader("settings")]
		public List<string> GetLogNames()
		{
			try
			{
				Log.WriteStart("'{0}' GetLogNames", ProviderSettings.ProviderName);
				var result = OSProvider.GetLogNames();
				Log.WriteEnd("'{0}' GetLogNames", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetLogNames", ProviderSettings.ProviderName), ex);
				throw;
			}

		}

		[WebMethod, SoapHeader("settings")]
		public List<SystemLogEntry> GetLogEntries(string logName)
		{
			try
			{
				Log.WriteStart("'{0}' GetLogEntries", ProviderSettings.ProviderName);
				var result = OSProvider.GetLogEntries(logName);
				Log.WriteEnd("'{0}' GetLogEntries", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetLogEntries", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public SystemLogEntriesPaged GetLogEntriesPaged(string logName, int startRow, int maximumRows)
		{
			try
			{
				Log.WriteStart("'{0}' GetLogEntriesPaged", ProviderSettings.ProviderName);
				var result = OSProvider.GetLogEntriesPaged(logName, startRow, maximumRows);
				Log.WriteEnd("'{0}' GetLogEntriesPaged", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetLogEntriesPaged", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void ClearLog(string logName)
		{
			try
			{
				Log.WriteStart("'{0}' ClearLog", ProviderSettings.ProviderName);
				OSProvider.ClearLog(logName);
				Log.WriteEnd("'{0}' ClearLog", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ClearLog", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion Event Log

		#region OS Processes & Services
		[WebMethod, SoapHeader("settings")]
		public OSProcess[] GetOSProcesses()
		{
			try
			{
				Log.WriteStart("'{0}' GetOSProcesses", ProviderSettings.ProviderName);
				var result = OSProvider.GetOSProcesses();
				Log.WriteEnd("'{0}' GetOSProcesses", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetOSProcesses", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void TerminateOSProcess(int pid)
		{
			try
			{
				Log.WriteStart("'{0}' TerminateOSProcess", ProviderSettings.ProviderName);
				OSProvider.TerminateOSProcess(pid);
				Log.WriteEnd("'{0}' TerminateOSProcess", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' TerminateOSProcess", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public OSService[] GetOSServices()
		{
			try
			{
				Log.WriteStart("'{0}' GetOSServices", ProviderSettings.ProviderName);
				var result = OSProvider.GetOSServices();
				Log.WriteEnd("'{0}' GetOSServices", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetOSServices", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void ChangeOSServiceStatus(string id, OSServiceStatus status)
		{
			try
			{
				Log.WriteStart("'{0}' ChangeOSServiceStatus", ProviderSettings.ProviderName);
				OSProvider.ChangeOSServiceStatus(id, status);
				Log.WriteEnd("'{0}' ChangeOSServiceStatus", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ChangeOSServiceStatus", ProviderSettings.ProviderName), ex);
				throw;
			}
		}

		[WebMethod, SoapHeader("settings")]
		public void RebootSystem()
		{
			try
			{
				Log.WriteStart("'{0}' RebootSystem", ProviderSettings.ProviderName);
				OSProvider.RebootSystem();
				Log.WriteEnd("'{0}' RebootSystem", ProviderSettings.ProviderName);
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' RebootSystem", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion

		#region OS Info
		[WebMethod, SoapHeader("settings")]
		public SystemMemoryInfo GetSystemMemoryInfo()
		{
			try
			{
				Log.WriteStart("'{0}' GetSystemMemoryInfo", ProviderSettings.ProviderName);
				var result = OSProvider.GetSystemMemoryInfo();
				Log.WriteEnd("'{0}' GetSystemMemoryInfo", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetSystemMemoryInfo", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		[WebMethod, SoapHeader("settings")]
		public SystemResourceUsageInfo GetSystemResourceUsageInfo()
		{
			try
			{
				Log.WriteStart("'{0}' GetSystemResourceUsageInfo", ProviderSettings.ProviderName);
				var result = OSProvider.GetSystemResourceUsageInfo();
				Log.WriteEnd("'{0}' GetSystemResourceUsageInfo", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' GetSystemResourceUsageInfo", ProviderSettings.ProviderName), ex);
				throw;
			}
		}
		#endregion

		[WebMethod, SoapHeader("settings")]
		public string ExecuteSystemCommand(string user, string password, string path, string args)
		{
			try
			{
				Log.WriteStart("'{0}' ExecuteSystemCommand", ProviderSettings.ProviderName);
				var result = OSProvider.ExecuteSystemCommand(user, password, path, args);
				Log.WriteEnd("'{0}' ExecuteSystemCommand", ProviderSettings.ProviderName);
				return result;
			}
			catch (Exception ex)
			{
				Log.WriteError(String.Format("'{0}' ExecuteSystemCommand", ProviderSettings.ProviderName), ex);
				throw;
			}
		}


		[WebMethod, SoapHeader("settings")]
		public List<DnsRecordInfo> GetDomainDnsRecords(string domain, string dnsServer, DnsRecordType recordType, int pause)
			=> OSProvider.GetDomainDnsRecords(domain, dnsServer, recordType, pause);

		public Providers.Web.IWebServer WebServer => OSProvider.WebServer;
		public ServiceController ServiceController => OSProvider.ServiceController;
		public WSLShell WSL => WinProvider.WSL;
		public bool IsSystemd => Provider is IUnixOperatingSystem && UnixProvider.IsSystemd;
		public bool IsOpenRC => Provider is IUnixOperatingSystem && UnixProvider.IsOpenRC;
		public TraceListener DefaultTraceListener => OSProvider.DefaultTraceListener;
		public bool IsUnix() => OSProvider.IsUnix();

	}
}
