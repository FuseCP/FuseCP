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

using Microsoft.Win32;
using Newtonsoft.Json.Bson;
using FuseCP.Providers;
using FuseCP.Providers.OS;
using FuseCP.Providers.Web;
using FuseCP.UniversalInstaller.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Dism;
using System.Collections.Immutable;

namespace FuseCP.UniversalInstaller;

public class WindowsInstaller : Installer
{
	const bool Net10RuntimeNeededOnWindows = true;
	public override string EnterpriseServerFolder => "Enterprise Server";
	public override string InstallExeRootPath
	{
		get => base.InstallExeRootPath ??
			(base.InstallExeRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), FuseCP));
		set => base.InstallExeRootPath = value;
	}
	public override string InstallWebRootPath { get => base.InstallWebRootPath ?? InstallExeRootPath; set => base.InstallWebRootPath = value; }
	public override string WebsiteLogsPath => InstallExeRootPath ?? "";
	Providers.OS.Installer WinGet => ((IWindowsOperatingSystem)OSInfo.Current).WinGet;
	Shell PowerShellShell => ((IWindowsOperatingSystem)OSInfo.Current).PowerShell;

	public virtual void InstallWinGet()
	{
		var ver = OSInfo.WindowsVersion;
		if (!WinGet.IsInstallerInstalled &&
			(OSInfo.IsWindowsServer && ver >= WindowsVersion.WindowsServer2022 ||
			!OSInfo.IsWindowsServer && ver >= WindowsVersion.Windows10))
		{
			const string Version = "v1.12.350";
			var tmpFile = Path.GetTempFileName() + ".msixbundle";
			PowerShellShell.ExecScript($@"
Invoke-WebRequest -Uri ""https://github.com/microsoft/winget-cli/releases/download/{Version}/Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.msixbundle"" -OutFile ""{tmpFile}""
Add-AppxPackage ""{tmpFile}""");
			File.Delete(tmpFile);
		}
	}
	public override void InstallNet10Runtime()
	{
		if (Net10RuntimeNeededOnWindows)
		{
			if (CheckNet10RuntimeInstalled()) return;

			var ver = OSInfo.WindowsVersion;
			if (!(OSInfo.IsWindowsServer && ver >= WindowsVersion.WindowsServer2012 ||
				!OSInfo.IsWindowsServer && ver >= WindowsVersion.Windows10))
				throw new PlatformNotSupportedException("NET 10 is not supported on this OS.");

			Info("Installing .NET 10 Runtime...");

			InstallWinGet();

			if (WinGet.IsInstallerInstalled) WinGet.Install("Microsoft.DotNet.AspNetCore.8;Microsoft.DotNet.Runtime.8");
			else
			{
				if (OSInfo.Architecture == Architecture.X64)
				{
					var exe = DownloadFile("https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/10.0.0/aspnetcore-runtime-10.0.0-win-x64.exe");
					Shell.Exec($"\"{exe}\" /install /quiet /norestart");
					exe = DownloadFile("https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/10.0.0/windowsdesktop-runtime-10.0.0-win-x64.exe");
					Shell.Exec($"\"{exe}\" /install /quiet /norestart");
				}
				else if (OSInfo.Architecture == Architecture.Arm64)
				{
					var exe = DownloadFile("https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/10.0.0/aspnetcore-runtime-10.0.0-win-arm64.exe");
					Shell.Exec($"\"{exe}\" /install /quiet /norestart");
					exe = DownloadFile("https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/10.0.0/windowsdesktop-runtime-10.0.0-win-arm64.exe");
					Shell.Exec($"\"{exe}\" /install /quiet /norestart");
				}
				else throw new NotSupportedException($"CPU architecture {OSInfo.Architecture} not supported.");
			}

			InstallLog("Installed .NET 10 Runtime.");

			ResetHasDotnet();
		}
	}

	public override void RemoveNet10AspRuntime()
	{
		WinGet.Remove("Microsoft.DotNet.AspNetCore.8");

		ResetHasDotnet();
	}
	public override void RemoveNet10NetRuntime()
	{
		WinGet.Remove("Microsoft.DotNet.Runtime.8");

		ResetHasDotnet();
	}

	private static List<string> GetInstalledNetFX1To45VersionFromRegistry()
	{
		var list = new List<string>();
		// Opens the registry key for the .NET Framework entry.
		using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
		{
			foreach (string versionKeyName in ndpKey.GetSubKeyNames())
			{
				// Skip .NET Framework 4.5 version information.
				if (versionKeyName == "v4")
				{
					continue;
				}

				if (versionKeyName.StartsWith("v"))
				{

					RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
					// Get the .NET Framework version value.
					string name = (string)versionKey.GetValue("Version", "");
					// Get the service pack (SP) number.
					string sp = versionKey.GetValue("SP", "").ToString();

					// Get the installation flag, or an empty string if there is none.
					string install = versionKey.GetValue("Install", "").ToString();
					if (string.IsNullOrEmpty(install)) // No install info; it must be in a child subkey.
						list.Add(name);
					else
					{
						if (!(string.IsNullOrEmpty(sp)) && install == "1")
						{
							list.Add(name);
						}
					}
					if (!string.IsNullOrEmpty(name))
					{
						continue;
					}
					foreach (string subKeyName in versionKey.GetSubKeyNames())
					{
						RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
						name = (string)subKey.GetValue("Version", "");
						if (!string.IsNullOrEmpty(name))
							sp = subKey.GetValue("SP", "").ToString();

						install = subKey.GetValue("Install", "").ToString();
						if (string.IsNullOrEmpty(install)) //No install info; it must be later.
							list.Add(name);
						else
						{
							if (!(string.IsNullOrEmpty(sp)) && install == "1")
							{
								list.Add(name);
							}
							else if (install == "1")
							{
								list.Add(name);
							}
						}
					}
				}
			}
		}
		return list;
	}
	private static List<string> GetInstalledNetFX45PlusFromRegistry()
	{
		var list = new List<string>();

		const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

		using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(subkey))
		{
			if (ndpKey == null)
				return list;
			//First check if there's an specific version indicated
			if (ndpKey.GetValue("Version") != null)
			{
				list.Add(ndpKey.GetValue("Version").ToString());
			}
			else
			{
				if (ndpKey != null && ndpKey.GetValue("Release") != null)
				{
					list.Add(CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
				}
			}
			return list;
		}

		// Checking the version using >= enables forward compatibility.
		string CheckFor45PlusVersion(int releaseKey)
		{
			if (releaseKey >= 533320)
				return "4.8.1";
			if (releaseKey >= 528040)
				return "4.8";
			if (releaseKey >= 461808)
				return "4.7.2";
			if (releaseKey >= 461308)
				return "4.7.1";
			if (releaseKey >= 460798)
				return "4.7";
			if (releaseKey >= 394802)
				return "4.6.2";
			if (releaseKey >= 394254)
				return "4.6.1";
			if (releaseKey >= 393295)
				return "4.6";
			if (releaseKey >= 379893)
				return "4.5.2";
			if (releaseKey >= 378675)
				return "4.5.1";
			if (releaseKey >= 378389)
				return "4.5";
			// This code should never execute. A non-null release key should mean
			// that 4.5 or later is installed.
			return "";
		}
	}

	public virtual bool IsNet48Installed
	{
		get
		{
			if (OSInfo.IsWindows)
			{
				var versions = GetInstalledNetFX45PlusFromRegistry()
					.Select(ver =>
					{
						Version version = default;
						System.Version.TryParse(ver, out version);
						return version;
					});

				return versions.Any(ver => ver >= new System.Version(4, 8));
			}
			else return false;
		}
	}
	public void InstallNet48()
	{
		if (!IsNet48Installed)
		{
			Log.WriteLine("Installing NET Framework 4.8");

			try
			{
				var file = DownloadFile("https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer");
				if (file != null)
				{
					Shell.Exec($"\"{file}\" /q");
				}

				InstallLog("Installed .NET Framework 4.8.");
			}
			catch (Exception) { }
		}
	}

    public virtual void InstallWindowsFeature(params IEnumerable<string> features)
    {
        try
        {
            DismApi.Initialize(DismLogLevel.LogErrorsWarnings);
            using (var session = DismApi.OpenOnlineSession())
            {
				var installed = DismApi.GetFeatures(session)
					.Where(feat => feat.State == DismPackageFeatureState.Installed)
					.Select(feat => feat.FeatureName)
					.ToList();
				features = features.Except(installed);

                foreach (var feature in features)
                {
                    Log.WriteLine($"Enabling Windows feature {feature}...");

                    try
                    {
                        DismApi.EnableFeature(session, feature, false, false);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteError($"Error enabling Windows feature {feature}: {ex.Message}", ex);
                    }

                    Log.WriteLine($"{feature} enabled successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.WriteError($"Error: {ex.Message}", ex);
        }
        finally
        {
            // Always shutdown
            DismApi.Shutdown();
        }
    }

    public virtual void InstallWindowsFeatures()
    {
        Info("Install Windows Features...");

        InstallWindowsFeature("IIS-WebServerRole", "IIS-WebServer", "IIS-CommonHttpFeatures", "IIS-HttpErrors",
            "IIS-HttpRedirect", "IIS-Security", "IIS-NetFxExtensibility45",
            "IIS-HttpCompressionDynamic", "IIS-StaticContent", "IIS-DefaultDocument", "IIS-DirectoryBrowsing",
            "IIS-WebDAV", "IIS-WebSockets", "IIS-ApplicationInit", "IIS-ISAPIFilter", "IIS-ISAPIExtensions",
            "IIS-ASPNET45", "IIS-BasicAuthentication", "IIS-HttpCompressionStatic", "IIS-FTPSvc",
            "NetFx4Extended-ASPNET45", "WCF-TCP-PortSharing45", "WCF-Pipe-Activation45",
            "WCF-TCP-Activation45", "WCF-HTTP-Activation45", "WAS-ProcessModel", "WAS-ConfigurationAPI");
    }

    public override void InstallServerPrerequisites()
    {
        //InstallNet10Runtime();
        InstallNet48();
        ConfigureAspNetTempFolderPermissions();
        InstallWindowsFeatures();
    }

    public override void InstallStandaloneServerPrerequisites()
    {
        if (Settings.EnterpriseServer.RunOnNetCore || Settings.WebPortal.RunOnNetCore) InstallNet10Runtime();
        InstallNet48();
        ConfigureAspNetTempFolderPermissions();
        InstallWindowsFeatures();
    }
    public override void InstallEnterpriseServerPrerequisites()
    {
        if (Settings.EnterpriseServer.RunOnNetCore) InstallNet10Runtime();
        InstallNet48();
        ConfigureAspNetTempFolderPermissions();
        InstallWindowsFeatures();

    }
    public override void InstallWebDavPortalPrerequisites()
    {
        InstallNet48();
        ConfigureAspNetTempFolderPermissions();
        InstallWindowsFeatures();

    }
    public override void InstallWebPortalPrerequisites()
    {
        if (Settings.WebPortal.RunOnNetCore) InstallNet10Runtime();
        InstallNet48();
        ConfigureAspNetTempFolderPermissions();
        InstallWindowsFeatures();
    }
    public override bool IsRunningAsAdmin
		=> new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

	public override void ShowLogFile()
	{
		try
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Log.File);
			Shell.Standard.Exec($"notepad.exe \"{path}\"");
		}
		catch { }
	}

	public void UserAndDomain(string username, out string domain, out string user)
	{
		var bslash = username.IndexOf('\\');
		if (bslash >= 0)
		{
			user = username.Substring(bslash + 1);
			domain = username.Substring(0, bslash);
		}
		else
		{
			user = username;
			domain = "";
		}
	}

	public virtual string AppPoolName(CommonSettings setting) => $"FuseCP {setting.ComponentName} Pool";

	public void CreateApplicationPool(CommonSettings setting)
	{
		var appPoolName = AppPoolName(setting);
		string domain, user;
		UserAndDomain(setting.Username, out domain, out user);
		var password = setting.Password;
		var identity = WebUtils.GetWebIdentity(setting);
		var poolExists = WebUtils.ApplicationPoolExists(appPoolName);

		if (poolExists)
		{
			Log.WriteStart("Updating application pool");
			Log.WriteInfo($"Updating application pool \"{appPoolName}\"");

			WebUtils.UpdateApplicationPool(appPoolName, user, password);

			Log.WriteEnd("Updated application pool");

			InstallLog($"Updated application pool named \"{appPoolName}\"");
		}
		else
		{
			Log.WriteStart("Creating application pool");
			Log.WriteInfo($"Creating application pool \"{appPoolName}\"");

			WebUtils.CreateApplicationPool(appPoolName, user, password);

			Log.WriteEnd("Created application pool");

			InstallLog($"Created a new application pool named \"{appPoolName}\"");
		}
	}

	public void DeleteApplicationPool(CommonSettings setting)
	{
		var appPoolName = AppPoolName(setting);
		string domain, user;
		UserAndDomain(setting.Username, out domain, out user);
		var password = setting.Password;
		var identity = WebUtils.GetWebIdentity(setting);
		var poolExists = WebUtils.ApplicationPoolExists(appPoolName);
		if (!poolExists) Log.WriteInfo($"Application pool {appPoolName} not found.");
		else
		{
			Log.WriteStart($"Deleting Application Pool {appPoolName}");
			try
			{
				WebUtils.DeleteApplicationPool(appPoolName);

				Log.WriteEnd($"Application pool deleted");
			}
			catch (Exception ex)
			{
				Log.WriteError("Error deleting Application Pool", ex);
			}
		}
	}

	public Providers.ServerBinding[] Bindings(CommonSettings setting)
	{
		if (setting.WebSitePort != default)
		{
			return new[]
			{
				new Providers.ServerBinding()
				{
					IP = setting.WebSiteIp,
					Port = setting.WebSitePort.ToString(),
					Host = setting.WebSiteDomain,
					Protocol = IsHttps(setting) ? "https" : "http"
				}
			};
		}
		else if (!string.IsNullOrEmpty(setting.Urls))
		{
			var bindings = (setting.Urls ?? "")
				.Split(';')
				.Select(url =>
				{
					url = url.Trim();
					var uri = new Uri(url);
					IPAddress ip;
					string host;
					if (!IPAddress.TryParse(uri.Host, out ip))
					{
						ip = null;
						host = uri.Host;
					}
					else
					{
						host = "";
					}
					return new Providers.ServerBinding(uri.Scheme, ip?.ToString(), uri.Port.ToString(), host);
				})
				.ToArray();
			return bindings;
		}
		else return null;
	}

	private static SSLCertificate GetSSLCertificateFromX509Certificate2(X509Certificate2 cert)
	{
		var certificate = new SSLCertificate
		{
			Hostname = cert.GetNameInfo(X509NameType.SimpleName, false),
			FriendlyName = cert.FriendlyName,
			CSRLength = Convert.ToInt32(cert.PublicKey.Key.KeySize.ToString(CultureInfo.InvariantCulture)),
			Installed = true,
			DistinguishedName = cert.Subject,
			Hash = cert.GetCertHash(),
			SerialNumber = cert.SerialNumber,
			ExpiryDate = DateTime.Parse(cert.GetExpirationDateString()),
			ValidFrom = DateTime.Parse(cert.GetEffectiveDateString()),
			Success = true
		};

		return certificate;
	}

	public void CreateWebsite(string siteName, CommonSettings setting, string contentPath)
	{
		Info($"Creating Website {siteName}");
		var binding = Bindings(setting).FirstOrDefault();

		var ip = binding.IP;
		var port = binding.Port;
		var domain = binding.Host;
		var scheme = binding.Protocol;
		var userName = WebUtils.GetWebIdentity(setting);
		var userPassword = setting.Password;
		var appPool = AppPoolName(setting);
		var componentId = setting.ComponentCode;

		Log.WriteInfo(String.Format("Creating web site \"{0}\" ( IP: {1}, Port: {2}, Domain: {3} )", siteName, ip, port, domain));

		var oldSiteId = WebUtils.GetSiteIdByBinding(ip, port.ToString(), domain);
		if (oldSiteId != null)
		{
			// get site name
			string oldSiteName = IsIis7 ? oldSiteId : WebUtils.GetSite(oldSiteId).Name;
			throw new Exception($"'{oldSiteName}' web site already has server binding ( IP: {ip}, Port: {port}, Domain: {domain} )");
		}

		if (setting.RunOnNetCore) contentPath = Path.Combine(contentPath, "bin_dotnet");

		//TODO certificate

		// create site
		var site = new WebSiteItem
		{
			Name = siteName,
			SiteIPAddress = ip,
			ContentPath = contentPath,
			AllowExecuteAccess = false,
			AllowScriptAccess = true,
			AllowSourceAccess = false,
			AllowReadAccess = true,
			AllowWriteAccess = false,
			AnonymousUsername = userName,
			AnonymousUserPassword = userPassword,
			AllowDirectoryBrowsingAccess = false,
			AuthAnonymous = true,
			AuthWindows = true,
			DefaultDocs = null,
			HttpRedirect = "",
			InstalledDotNetFramework = AspNetVersion.AspNet40,
			ApplicationPool = appPool,
			//
			Bindings = new Web.ServerBinding[] {
					new Web.ServerBinding(ip, port.ToString(), domain, scheme, componentId)
				}
		};

		var newSiteId = WebUtils.CreateSite(site);
		
		if (scheme == "https")
		{
			var webServer = OSInfo.Windows.WebServer;
			var website = webServer.GetSite(siteName);
			// Install certificate
			if (!string.IsNullOrEmpty(setting.CertificateFile))
			{
				webServer.InstallPFX(File.ReadAllBytes(setting.CertificateFile), setting.CertificatePassword, website);
			} else
			{
				var store = new X509Store(setting.CertificateStoreName, (StoreLocation)Enum.Parse(typeof(StoreLocation),
					setting.CertificateStoreLocation));
				store.Open(OpenFlags.MaxAllowed);
				var cert = store.Certificates.Find((X509FindType)Enum.Parse(typeof(X509FindType), setting.CertificateFindType),
					setting.CertificateFindValue, true)[0];
				var certData = cert.Export(X509ContentType.Pfx);
				store.Close();
				var convertedCert = new X509Certificate2(certData, string.Empty, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
				var password = Guid.NewGuid().ToString();
				var certDataWithPassword = convertedCert.Export(X509ContentType.Pfx, password);
				webServer.InstallPFX(certDataWithPassword, password, website);
			}
		} 
		Log.WriteEnd("Created web site");
		InstallLog($"Installed {siteName} website, listening on the url(s):{NewLine}" +
			$"{string.Join(NewLine, (GetUrls(setting) ?? "").Split(',', ';')
			.Select(url => "  " + url))}");

	}

	public void SetFolderPermission(string path, string account, NtfsPermission permission)
	{
		try
		{
			if (!FileUtils.DirectoryExists(path))
			{
				FileUtils.CreateDirectory(path);
				Log.WriteInfo(string.Format("Created {0} folder", path));
			}

			Log.WriteStart(string.Format("Setting '{0}' permission for '{1}' folder for '{2}' account", permission, path, account));
			SecurityUtils.GrantNtfsPermissions(path, null, account, permission, true, true);
			Log.WriteEnd("Set security permissions");
		}
		catch (Exception ex)
		{
			if (Utils.IsThreadAbortException(ex))
				return;

			Log.WriteError("Security error", ex);
		}
	}

	private void SetFolderPermissionBySid(string path, string account, NtfsPermission permission)
	{
		try
		{
			if (!FileUtils.DirectoryExists(path))
			{
				FileUtils.CreateDirectory(path);
				Log.WriteInfo(string.Format("Created {0} folder", path));
			}

			Log.WriteStart(string.Format("Setting '{0}' permission for '{1}' folder for '{2}' account", permission, path, account));
			SecurityUtils.GrantNtfsPermissionsBySid(path, account, permission, true, true);
			Log.WriteEnd("Set security permissions");
		}
		catch (Exception ex)
		{
			if (Utils.IsThreadAbortException(ex))
				return;

			Log.WriteError("Security error", ex);
		}
	}

	public void ConfigureAspNetTempFolderPermissions()
	{
		string path;
		if (OSInfo.IsWindows && OSInfo.Windows.WebServer.Version.Major == 6)
		{
			// IIS_WPG -> C:\WINDOWS\Temp
			path = Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.Machine);
			SetFolderPermission(path, "IIS_WPG", NtfsPermission.Modify);

			// IIS_WPG - > C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\Temporary ASP.NET Files
			path = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
				"Temporary ASP.NET Files");
			if (Utils.IsWin64() && Utils.IIS32Enabled())
				path = path.Replace("Framework64", "Framework");
			SetFolderPermission(path, "IIS_WPG", NtfsPermission.Modify);
		}
		// NETWORK_SERVICE -> C:\WINDOWS\Temp
		path = Environment.GetEnvironmentVariable("TMP", EnvironmentVariableTarget.Machine);
		//
		SetFolderPermissionBySid(path, SystemSID.NETWORK_SERVICE, NtfsPermission.Modify);
	}
	public override string[] UserIsMemeberOf(CommonSettings settings)
	{
		if (settings is ServerSettings)
		{
			if (OSInfo.Windows.WebServer.Version.Major >= 7)
			{
				return new string[] { "AD:Domain Admins", "SID:" + SystemSID.ADMINISTRATORS, "IIS_IUSRS" };
			}
			else
			{
				return new string[] { "AD:Domain Admins", "SID:" + SystemSID.ADMINISTRATORS, "IIS_WPG" };
			}
		}
		else
		{
			if (OSInfo.Windows.WebServer.Version.Major >= 7) return new string[] { "IIS_IUSRS" };
			else return new string[] { "IIS_WPG" };
		}
	}
	public virtual void CreateWindowsAccount(CommonSettings setting)
	{
		Info("Create user...");
		const string UserAccountExists = "Account already exists";
		const string UserAccountDescription = "{0} account for anonymous access to Internet Information Services";
		const string LogInfoMessage = "Creating Windows user account \"{0}\"";
		const string LogEndMessage = "Created windows user account";
		const string InstallLogMessageLocal = "Created a new Windows user account \"{0}\"";
		const string InstallLogMessageDomain = "Created a new Windows user account \"{0}\" in \"{1}\" domain";

		string domain, userName;
		UserAndDomain(setting.Username, out domain, out userName);

		Log.WriteStart(String.Format(LogInfoMessage, userName));

		var description = String.Format(UserAccountDescription, setting.ComponentName);
		var memberOf = UserIsMemeberOf(setting);
		var password = setting.Password;

		// create account
		SystemUserItem user = new SystemUserItem
		{
			Domain = domain,
			Name = userName,
			FullName = userName,
			Description = description,
			MemberOf = memberOf,
			Password = password,
			PasswordCantChange = true,
			PasswordNeverExpires = true,
			AccountDisabled = false,
			System = true
		};

		Transaction(() =>
		{
			// Exit with an error if Windows account with the same name already exists
			if (SecurityUtils.UserExists(domain, userName))
				throw new Exception(UserAccountExists);
		})
			.WithRollback(() => { });

		Transaction(() => SecurityUtils.CreateUser(user))
			.WithRollback(() => SecurityUtils.DeleteUser(domain, userName));

		// update log
		Log.WriteEnd(LogEndMessage);

		// update install log
		if (string.IsNullOrEmpty(domain))
		{
			InstallLog(string.Format(InstallLogMessageLocal, userName));
		}
		else
		{
			InstallLog(string.Format(InstallLogMessageDomain, userName, domain));
		}
	}
	public const string UserAccountDescription = "{0} account for anonymous access to Internet Information Services";
	public void DeleteUserAccount(CommonSettings settings)
	{
		var username = settings.Username;
		var bslash = username.IndexOf('\\');
		string domain = "";
		if (bslash >= 0)
		{
			domain = username.Substring(0, bslash);
			username = username.Substring(bslash + 1);
		}
		else
		{
			domain = Environment.MachineName;
		}
		var password = settings.Password;

		if (SecurityUtils.UserExists(domain, username)) SecurityUtils.DeleteUser(domain, username);
	}
	public virtual void AddUnixGroup(string group) => Shell.Exec($"groupadd {group}");
	public virtual void AddUnixUser(string user, string group, string password)
	{
		Shell.Exec($"useradd --home /home/{user} --gid {group} -m --shell /bin/false {user}");

		var shell = Shell.ExecAsync($"passwd {user}");
		shell.Input.WriteLine(password);
		shell.Input.WriteLine(password);
		var output = shell.Output().Result;
		Log.WriteLine(output);

		InstallLog($"Added System User {user}.");
	}
	public override void InstallWebsite(string name, string path, CommonSettings settings,
		string group, string dll, string description, string serviceId)
	{
		Transaction(() => CreateApplicationPool(settings))
			.WithRollback(() => DeleteApplicationPool(settings));
		
		CreateWebsite(name, settings, path);
	}

	public override void RemoveWebsite(string siteId, CommonSettings setting)
	{
		RemoveFirewallRule(GetUrls(setting));

		Info($"Delete website {siteId}");
		Log.WriteStart($"Delete website {siteId}");
		WebUtils.DeleteSite(siteId);
		InstallLog($"Removed website {siteId}");
		Log.WriteEnd("Website deleted");
		DeleteApplicationPool(setting);
	}
	public override void ConfigureSchedulerService()
	{
		var binFolder = (Settings.EnterpriseServer.RunOnNetCore ||
			Settings.WebPortal.RunOnNetCore && Settings.WebPortal.EmbedEnterpriseServer) ?
				"bin_dotnet" : "bin\\Code";
		var exe = Path.Combine(Settings.EnterpriseServer.InstallPath, binFolder, "FuseCP.SchedulerService.exe");

		var config = exe + ".config";
		var xml = XElement.Load(config);

		var conStrings = xml.Element("connectionStrings");
		if (conStrings == null) xml.Add(conStrings = new XElement("connectionStrings"));
		var appSettings = xml.Element("appSettings");
		if (appSettings == null) xml.Add(appSettings = new XElement("appSettings"));

		var conStrElement = conStrings.Elements("add").FirstOrDefault(e => e.Attribute("name")?.Value == "EnterpriseServer");
		var esConfFile = Path.GetFullPath(Path.Combine(Settings.EnterpriseServer.InstallPath, "Web.config"));
		if (File.Exists(esConfFile))
		{
			var esConf = XElement.Load(esConfFile);
			var esAppSettings = esConf.Element("appSettings");

			foreach (var esSetting in esAppSettings.Elements("add"))
			{
				var key = esSetting.Attribute("key")?.Value;
				var setting = appSettings.Elements("add").FirstOrDefault(s => s.Attribute("key")?.Value == key);
				if (setting == null)
				{
					appSettings.Add(esSetting);
				}
				else
				{
					setting.Attribute("value").SetValue(esSetting.Attribute("value")?.Value);
				}
			}

			var esConStrings = esConf.Element("connectionStrings");
			conStrings.ReplaceWith(esConStrings);

			File.WriteAllText(config, xml.ToString());
		}
	}
	public override void InstallSchedulerService()
	{
		var services = OSInfo.Current.ServiceController;
		if (services.Info(SchedulerServiceId) != null) services.Remove(SchedulerServiceId);

		ConfigureSchedulerService();

		Transaction(() =>
		{
			var binFolder = (Settings.EnterpriseServer.RunOnNetCore ||
				Settings.WebPortal.RunOnNetCore && Settings.WebPortal.EmbedEnterpriseServer) ?
					"bin_dotnet" : "bin\\Code";
			var service = new WindowsServiceDescription()
			{
				ServiceId = SchedulerServiceId,
				DisplayName = "FuseCP Scheduler Service",
				Executable = Path.Combine(Settings.EnterpriseServer.InstallPath, binFolder, "FuseCP.SchedulerService.exe"),
				Start = WindowsServiceStartMode.DelayedAuto,
				Type = WindowsServiceType.Own,
				Error = WindowsServiceErrorHandling.Normal
			};

			InstallService(service);

		}).WithRollback(() =>
		{
			try
			{
				RemoveSchedulerService();
			}
			catch { }
		});
	}
	public override void RemoveSchedulerService() => RemoveService(SchedulerServiceId);
	public override bool CheckOSSupported() => OSInfo.WindowsVersion >= WindowsVersion.Windows7;
	public override bool CheckIISVersionSupported() => CheckOSSupported();
	public override bool CheckInitSystemSupported() => true;
	public override bool CheckNetVersionSupported()
	{
		if (OSInfo.IsNet48) return true;
		var fxver = new Version(OSInfo.NetFXVersion);
		return OSInfo.IsCore &&
			int.Parse(Regex.Match(OSInfo.FrameworkDescription, "[0-9]+").Value) >= 8 &&
			(fxver.Major > 4 || fxver.Major == 4 && fxver.Minor >= 8);
	}

	public override void RestartAsAdmin()
	{
		if (RunAsAdmin)
		{
			//var currentp = Process.GetCurrentProcess();
			ProcessStartInfo procInfo = new ProcessStartInfo();
			procInfo.UseShellExecute = true;
			var assemblyFile = GetEntryAssembly().Location;
			if (OSInfo.IsMono) procInfo.FileName = "mono";
			else if (assemblyFile.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) procInfo.FileName = assemblyFile;
			else if (OSInfo.IsCore) procInfo.FileName = "dotnet";
			procInfo.WorkingDirectory = Environment.CurrentDirectory;
			procInfo.Arguments = string.Join(" ", Environment.GetCommandLineArgs()
				.Select(arg => arg.Contains(' ') ? $"\"{arg}\"" : arg));
			procInfo.Verb = "runas";
			try
			{
				var p = Process.Start(procInfo);
				p.WaitForExit();
				Environment.Exit(p.ExitCode);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				Console.Read();
				Environment.Exit(-1);
			}
		}
	}

	public override string GetUninstallLog(ComponentSettings settings)
	{
		switch (settings.ComponentCode)
		{
			case Global.Server.ComponentCode:
				return
@"- Remove FuseCP Server website
- Delete FuseCP Server folder.
- Remove firewall rule.";
			case Global.EntServer.ComponentCode:
				return
@"- Remove FuseCP EnterpriseServer website.
- Delete FuseCP EnterpriseServer folder.
- Remove FuseCP Database.
- Remove firewall rule.";
			case Global.WebPortal.ComponentCode:
				return
@"- Remove FuseCP WebPortal website.
- Delete FuseCP WebPortal folder.
- Remove firewall rule.";
			case Global.WebDavPortal.ComponentCode:
				return
@"- Remove FuseCP EnterpriseServer website.
- Delete FuseCP EnterpriseServer folder.
- Remove firewall rule.";
			case Global.StandaloneServer.ComponentCode:
				return
@"- Remove FuseCP WebPortal website.
- Delete FuseCP WebPortal, EnterpriseServer & Server folder.
- Remove firewall rule.";
			default: throw new NotSupportedException();
		}
	}

	public override void CreateUser(CommonSettings settings) => CreateWindowsAccount(settings);
	public override void RemoveUser(string username)
	{
		if (!string.IsNullOrEmpty(username))
		{
			var bslash = username.IndexOf('\\');
			string machine, user;
			if (bslash >= 0)
			{
				machine = username.Substring(0, bslash);
				if (bslash < username.Length - 1) user = username.Substring(bslash, username.Length - bslash - 1);
				else user = "";
			}
			else
			{
				machine = "";
				user = username;
			}
			if (SecurityUtils.UserExists(machine, user))
			{
				SecurityUtils.DeleteUser(machine, user);
				InstallLog($"Removed user {username}");
			}
		}
	}
}
