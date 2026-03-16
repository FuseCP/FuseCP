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

using Microsoft.Identity.Client;
using Microsoft.Web.Administration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FuseCP.EnterpriseServer.Data;
using FuseCP.Providers;
using FuseCP.Providers.Common;
using FuseCP.Providers.OS;
using FuseCP.Providers.Utils;
using FuseCP.Providers.Web;
using FuseCP.UniversalInstaller.Core;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FuseCP.UniversalInstaller;

public abstract partial class Installer
{
	public const bool RunAsAdmin = true;
	public virtual string FuseCP => "FuseCP";
	public virtual string ServerFolder => "Server";
	public virtual string EnterpriseServerFolder => "EnterpriseServer";
	public virtual string EnterpriseServerAltFolder => "Enterprise Server";
	public virtual string WebPortalFolder => "Portal";
	public virtual string WebDavPortalFolder => "WebDavPortal";
	public virtual string ServerUser => $"{FuseCP}Server";
	public virtual string EnterpriseServerUser => $"{FuseCP}EnterpriseServer";
	public virtual string WebPortalUser => $"{FuseCP}Portal";
	public virtual string FuseCPWebUsersGroup => "FCP_IUSRS";
	public virtual string UnixAppRootPath => "/usr/share";
	public virtual string NewLine => Environment.NewLine;
	public virtual bool CanInstallServer => true;
	public virtual bool CanInstallEnterpriseServer => true;
	public virtual bool CanInstallPortal => true;
	public virtual string InstallerSettingsFile => "installer.settings.json";
	public virtual string CertificateFolder => "Certificates";
	public virtual string TempPath => Settings.Installer.TempPath;
	public virtual string BackupPath => Path.Combine(TempPath, "Backup");
	public virtual string ComponentTempPath => Path.Combine(TempPath, "Component");
	public virtual bool IsWindows => OSInfo.IsWindows;
	public virtual bool IsUnix => OSInfo.IsUnix;
	public virtual bool IsMac => OSInfo.IsMac;
	public virtual bool IsSystemd => OSInfo.IsSystemd;
	public virtual bool IsOpenRC => OSInfo.IsOpenRC;
	public virtual bool IsInstallAction => Settings.Installer.Action == SetupActions.Install;
	public virtual bool IsUpdateAction => Settings.Installer.Action == SetupActions.Update;
	public virtual bool IsSetupAction => Settings.Installer.Action == SetupActions.Setup;
	public virtual bool IsUninstallAction => Settings.Installer.Action == SetupActions.Uninstall;
	public virtual string FuseCPGroup => FuseCP.ToLower();
	public virtual string FuseCPUnixGroup => FuseCPGroup;

	public const bool UseHttpsOnWindows = true;
	public const bool UseLettuceEncrypt = false;
	public Action OnExit { get; set; }
	public Action<Exception> OnError { get; set; }

	static bool? hasDotnet = null;
	public virtual bool HasDotnet
	{
		get
		{
			if (hasDotnet == null)
			{
				var find = Shell.Find("dotnet");
				hasDotnet = find != null && !Regex.IsMatch(find, "^/mnt/[a-zA-Z]/");
			}
			return hasDotnet.Value;
		}
		set => hasDotnet = value;
	}
	public void ResetHasDotnet() => hasDotnet = null;
	public virtual bool IsRemoteConsole =>
		!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PSRemotingProtocolVersion")) ||
		!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PSSenderInfo")) ||
		!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WSManStackVersion")) ||
		!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SSH_CLIENT")) ||
		!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SSH_TTY"));

	public bool NeedRemoveNet10Runtime = false;
	public bool NeedRemoveNet10AspRuntime = false;
	public virtual string InstallWebRootPath { get; set; } = null;
	public virtual string InstallExeRootPath { get; set; } = null;
	public abstract string WebsiteLogsPath { get; }

	int? estimatedOutputLines = null;
	public virtual Func<int> CalculateEstimateOutputLines { get; set; } = null;
	public virtual int EstimatedOutputLines
	{
		get => estimatedOutputLines ??= CalculateEstimateOutputLines != null ?
			CalculateEstimateOutputLines() : Files + DatabaseStatements + 50;
		set => estimatedOutputLines = value;
	}
	public int Files
	{
		get => Settings.Installer.Files;
		set => Settings.Installer.Files = value;
	}
	public int DatabaseStatements { get; set; } = 0;

	public InstallerSettings Settings { get; set; } = new InstallerSettings()
	{
		Server = new ServerSettings(),
		EnterpriseServer = new EnterpriseServerSettings(),
		WebPortal = new WebPortalSettings(),
		Installer = new InstallerSpecificSettings(),
		WebDavPortal = new WebDavPortalSettings(),
		Standalone = new StandaloneSettings()
	};

	public Shell Shell { get; set; } = Shell.Standard.Clone;
	public Providers.OS.Installer OSInstaller => OSInfo.Current.DefaultInstaller;
	public IWebServer WebServer => OSInfo.Current.WebServer;
	public ServiceController ServiceController => OSInfo.Current.ServiceController;
	public UI UI => UI.Current;
	LogWriter log = null;
	public virtual LogWriter Log => log ??= new LogWriter();

	public List<string> InstallLogs { get; private set; } = new List<string>();
	public void InstallLog(string msg)
	{
		InstallLogs.Add(msg);
		Log.WriteLine(msg);
	}
	public Action<string> OnInfo { get; set; }
	public void Info(string message)
	{
		OnInfo?.Invoke(message);
		Log.WriteLine(message);
	}
	public Assembly GetEntryAssembly()
	{
		var asm = Assembly.GetEntryAssembly();
		if (asm == null) asm = AppDomain.CurrentDomain.GetAssemblies()
			.FirstOrDefault(a => a.GetName().Name == "Setup2");
		return asm;
	}
	public virtual bool IsSetup => !GetEntryAssembly().GetName().Name.Contains("Installer");
	public virtual Version Version
	{
		get
		{
			if (!IsSetup) return GetEntryAssembly().GetName().Version;
			else return Settings.Installer.Version;
		}
	}

	public void ImportLegacySettings()
	{
		var exe = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
		var configFile = exe + ".config";
		if (Settings.Installer.OldConfigImported || !File.Exists(configFile)) return;

		Settings.Installer.OldConfigImported = true;
		
		var doc = XDocument.Load(configFile);
		var installer = doc.Element("installer");
		var instSettings = installer?.Element("settings");
		bool useProxy = false;
		Settings.Installer.Proxy = new ProxySettings();

		foreach (var setting in instSettings?.Elements("add"))
		{
			var key = setting.Attribute("key")?.Value;
			var value = setting.Attribute("value")?.Value;
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
			{
				switch (key)
				{
					case "Web.AutoCheck":
						Settings.Installer.CheckForUpdate = bool.Parse(value);
						break;
					case "Web.Proxy.UseProxy":
						useProxy = bool.Parse(value);
						break;
					case "Web.Proxy.Server":
						Settings.Installer.Proxy.Address = value;
						break;
					case "Web.Proxy.UserName":
						Settings.Installer.Proxy.Username = value;
						break;
					case "Web.Proxy.Password":
						Settings.Installer.Proxy.Password = value;
						break;
				}
			}
		}

		if (!useProxy) Settings.Installer.Proxy = null;
		
		var components = installer?.Element("components")?.Elements("component");
		if (components != null)
		{
			foreach (var component in components)
			{
				var info = new ComponentInfo();
				info.ApplicationName = FuseCP;

				var settingsFromCode = new Dictionary<string, ComponentSettings>()
				{
					{ Global.Server.ComponentCode, Settings.Server },
					{ Global.EntServer.ComponentCode, Settings.EnterpriseServer },
					{ Global.WebPortal.ComponentCode, Settings.WebPortal },
					{ Global.WebDavPortal.ComponentCode, Settings.WebDavPortal },
					{ Global.StandaloneServer.ComponentCode, Settings.Standalone }
				};

				var settings = component.Element("settings")?.Elements("add");
				if (settings != null)
				{
					CommonSettings commonSettings = null;
					ComponentSettings componentSettings = null;

					foreach (var setting in settings.OrderBy(set => set.Attribute("key").Value))
					{
						var key = setting.Attribute("key")?.Value;
						var value = setting.Attribute("value")?.Value;
						if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
						{
							switch (key)
							{
								case "ComponentCode":
									info.ComponentCode = value;
									componentSettings = settingsFromCode[value];
									commonSettings = componentSettings as CommonSettings;
									break;
								case "ComponentName":
									info.ComponentName = value;
									break;
								case "ComponentDescription":
									info.ComponentDescription = value;
									break;
								case "Release":
									info.Version = Version.Parse(value);
									info.VersionName = value;
									break;
								case "InstallFolder":
									componentSettings.InstallPath = value;
									break;
								case "InstallerType":
									info.InstallerType = value;
									break;
								case "InstallerPath":
									info.InstallerPath = value;
									break;
								case "WebSiteIP":
									commonSettings.WebSiteIp = value;
									break;
								case "WebSitePort":
									commonSettings.WebSitePort = int.Parse(value);
									break;
								case "WebSiteDomain":
									commonSettings.WebSiteDomain = value;
									break;
								case "UserAccount":
									commonSettings.Username = value;
									break;
								case "UserPassword":
									commonSettings.Password = value;
									break;
								case "Database":
									Settings.EnterpriseServer.DatabaseName = value;
									break;
								case "DatabaseUser":
									Settings.EnterpriseServer.DatabaseUser = value;
									break;
								case "DatabaseServer":
									Settings.EnterpriseServer.DatabaseServer = value;
									break;
								case "InstallConnectionString":
									Settings.EnterpriseServer.DbInstallConnectionString = value;
									Settings.EnterpriseServer.DatabaseType = EnterpriseServer.Data.DbType.SqlServer;
									break;
								case "ConnectionString":
									var csb = new ConnectionStringBuilder(value);
									var pwd = (csb["Password"] ?? csb["Pwd"]) as string;
									int port = 1433;
									var server = csb["Server"] as string;
									if (server.Contains(','))
									{
										var tokens = server.Split(',');
										int.TryParse(tokens.LastOrDefault() ?? "", out port);
									}
									Settings.EnterpriseServer.DatabaseWindowsAuthentication =
										(csb["Integrated Security"] as bool?) == true;
									Settings.EnterpriseServer.DatabasePort = port;
									Settings.EnterpriseServer.DatabasePassword = pwd;
									Settings.EnterpriseServer.DatabaseType = EnterpriseServer.Data.DbType.SqlServer;
									break;
								case "CryptoKey":
									Settings.EnterpriseServer.CryptoKey = value;
									break;
								default: break;
							}
						}
					}
				}
				var fileInfo = Installer.Current.InstallerWebService.GetReleaseFileInfo(info.ComponentCode, info.VersionName);
				info.FullFilePath = fileInfo.FullFilePath;
				info.UpgradeFilePath = fileInfo.UpgradeFilePath;
				info.ReleaseFileId = fileInfo.ReleaseFileId;
				info.Platforms = fileInfo.Platforms;

				Settings.Installer.InstalledComponents.Add(info);
			}
		}

		SaveSettings();
	}

	public void LoadSettings()
	{
		var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, InstallerSettingsFile);
		if (File.Exists(path))
		{
			var settings = JsonConvert.DeserializeObject<InstallerSettings>(
				File.ReadAllText(path), new VersionConverter(), new StringEnumConverter());
			settings.Installer.Version = Version;
			settings.Installer.TempPath = FileUtils.GetTempDirectory();
			settings.Installer.UI = UI.Current.GetType().Name;
			var unattendedInstallPackages = Settings.Installer.UnattendedInstallPackages;
			var action = Settings.Installer.Action;
			Settings = settings;
			if (!string.IsNullOrEmpty(unattendedInstallPackages))
			{
				Settings.Installer.UnattendedInstallPackages = unattendedInstallPackages;
				Settings.Installer.Action = action;
			}
		}
		else SaveSettings();
	}
	public void SaveSettings()
	{
		var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, InstallerSettingsFile);
		var tempPath = TempPath;
		Settings.Installer.TempPath = null;
		Settings.Installer.Version = null;
		Settings.Installer.UI = null;
		var json = JsonConvert.SerializeObject(Settings, Formatting.Indented,
			new VersionConverter(), new StringEnumConverter());
		Settings.Installer.TempPath = tempPath;
		Settings.Installer.UI = UI.Current.GetType().Name;
		Settings.Installer.Version = Version;
		File.WriteAllText(path, json);
	}
	public void UpdateSettings()
	{
		if (HasError) return;

		if (Settings.Installer.Component != null)
		{
			ComponentInfo component;
			switch (Settings.Installer.Action)
			{
				case SetupActions.Install:
					Settings.Installer.InstalledComponents.Add(Settings.Installer.Component);
					break;
				case SetupActions.Uninstall:
					component = Settings.Installer.InstalledComponents
						.FirstOrDefault(c => c.ComponentCode == Settings.Installer.Component.ComponentCode);
					if (component != null) Settings.Installer.InstalledComponents.Remove(component);
					break;
				case SetupActions.Setup:
					break;
				case SetupActions.Update:
					component = Settings.Installer.InstalledComponents
						.FirstOrDefault(c => c.ComponentCode == Settings.Installer.Component.ComponentCode);
					if (component != null) Settings.Installer.InstalledComponents.Remove(component);
					Settings.Installer.InstalledComponents.Add(Settings.Installer.Component);
					break;
			}
			Settings.Installer.Component = null;
			SaveSettings();
		}
	}

	public virtual void Cleanup()
	{
		if (Directory.Exists(BackupPath)) Task.Run(() => Directory.Delete(BackupPath, true));
		Settings.Installer.Component = null;
		Error = null;
		InstallLogs.Clear();
		Cancel = new CancellationTokenSource();
	}

	public void PassLegacyArguments(Hashtable args, ComponentInfo info)
	{
		if (!info.UsesNewInstaller)
		{
			var fileName = info.FullFilePath.Replace('\\', Path.DirectorySeparatorChar);

			args[Global.Parameters.ComponentName] = info.ComponentName;
			args[Global.Parameters.ApplicationName] = FuseCP;
			args[Global.Parameters.ComponentCode] = info.ComponentCode;
			args[Global.Parameters.ComponentDescription] = info.ComponentDescription;
			args[Global.Parameters.Version] = info.Version.ToString();
			args[Global.Parameters.InstallerFolder] = TempPath;
			args[Global.Parameters.InstallerPath] = info.InstallerPath.Replace('\\', Path.DirectorySeparatorChar);
			args[Global.Parameters.InstallerType] = info.InstallerType;
			args[Global.Parameters.Installer] = Path.GetFileName(fileName);
			args[Global.Parameters.ShellVersion] = Installer.Current.LoadContext.GetShellVersion();
			args[Global.Parameters.BaseDirectory] = FileUtils.GetCurrentDirectory();
			args[Global.Parameters.ShellMode] = Global.VisualInstallerShell;
			args[Global.Parameters.IISVersion] = Global.IISVersion;
			args[Global.Parameters.SetupXml] = null; //TODO Pass SetupXml
			args[Global.Parameters.UIType] = UI.Current.GetType().Name;
		}
	}
	public bool RunSetup(ComponentInfo info, SetupActions action)
	{
		Settings.Installer.Component = info;
		Settings.Installer.Action = action;

		info.FullFilePath = info.FullFilePath?.Replace('\\', Path.DirectorySeparatorChar);
		info.UpgradeFilePath = info.UpgradeFilePath?.Replace('\\', Path.DirectorySeparatorChar);
		info.InstallerPath = info.InstallerPath?.Replace('\\', Path.DirectorySeparatorChar);

		RemoteFile file = new RemoteFile(info, action != SetupActions.Update);
		string installerPath = info.InstallerPath;
		string installerType = info.InstallerType;

		if (action == SetupActions.Install && info.IsInstalled)
		{
			UI.Current.ShowWarning(Global.Messages.ComponentIsAlreadyInstalled);
			return false;
		}
		else if (action != SetupActions.Install && !info.IsInstalled)
		{
			UI.Current.ShowWarning(Global.Messages.ComponentIsNotInstalled);
			return false;
		}
		try
		{
			// download installer
			var tmpFolder = ComponentTempPath;

            if (UI.Current.DownloadSetup(file, action == SetupActions.Uninstall || action == SetupActions.Setup))
            {
                UI.Current.ShowWaitCursor();
				string path = Path.Combine(tmpFolder, installerPath);
				string method;
				switch (Settings.Installer.Action)
				{
					default:
					case SetupActions.Install: method = "Install"; break;
					case SetupActions.Uninstall: method = "Uninstall"; break;
					case SetupActions.Setup: method = "Setup"; break;
					case SetupActions.Update: method = "Update"; break;
				}
				Log.WriteStart(string.Format("Running installer {0}.{1} from {2}", installerType, method, path));

				var json = JsonConvert.SerializeObject(Settings, new VersionConverter(), new StringEnumConverter());

				var hashtable = new Hashtable();
				hashtable["ParametersJson"] = json;
				UI.PassArguments(hashtable);
				PassLegacyArguments(hashtable, info);

				//run installer
				var res = (Result)LoadContext.Execute(path, installerType, method, new object[] { hashtable }) == Result.OK;

				Task.Run(() =>
				{
					FileUtils.DeleteTempDirectory();
				});

				LoadSettings();

				UI.EndWaitCursor();

				Log.WriteInfo(string.Format("Installer returned {0}", res));
				Log.WriteEnd("Installer finished");

				return res;
			}
		}
		catch (Exception ex)
		{
			Log.WriteError("Installer error", ex);
			UI.Current.ShowError(ex);
		}

		return false;
	}
	public bool Install(ComponentInfo info) => RunSetup(info, SetupActions.Install);
	public bool Uninstall(ComponentInfo info) => RunSetup(info, SetupActions.Uninstall);
	public bool Setup(ComponentInfo info) => RunSetup(info, SetupActions.Setup);
	public bool Update(ComponentInfo info) => RunSetup(info, SetupActions.Update);
	public bool Equals(string a, string b)
	{
		return a.Equals(b, StringComparison.OrdinalIgnoreCase) ||
			a.Replace(" ", "").Equals(b.Replace(" ", ""), StringComparison.OrdinalIgnoreCase);
	}
	public async Task RunUnattended()
	{
		UI.ShowWaitCursor();

		var releases = Installer.Current.Releases;

		var components = await releases.GetAvailableComponentsAsync();

		var componentsToInstall = Settings.Installer.UnattendedInstallPackages.Split(',', ';')
			.Select(name => name.Trim())
			.Where(name => !string.IsNullOrEmpty(name))
			.ToArray();

		components = components
			.Where(c => componentsToInstall.Any(ci => Equals(c.ComponentName, ci) ||
				Equals(c.ComponentCode, ci) || Equals(c.Component, ci)))
			.Where(c => OSInfo.IsWindows && c.Platforms.HasFlag(Platforms.Windows) ||
				!OSInfo.IsWindows && c.Platforms.HasFlag(Platforms.Unix))
			.ToList();

		var success = true;
		foreach (var component in components)
		{
			Log.WriteInfo($"Unattended ${Settings.Installer.Action} ${component.ComponentName}.");
			success &= RunSetup(component, Settings.Installer.Action);
		}

        Settings.Installer.UnattendedInstallPackages = null;
        SaveSettings();

        UI.EndWaitCursor();

		Exit(success ? 0 : 1);
	}

	protected bool? Net10RuntimeInstalled { get; set; }
	public bool CheckNet10RuntimeInstalled()
	{
		if (Net10RuntimeInstalled != null) return Net10RuntimeInstalled.Value;

		if (HasDotnet)
		{
			var output = Shell.Exec("dotnet --info").Output().Result ?? "";
			NeedRemoveNet10AspRuntime = !output.Contains("Microsoft.AspNetCore.App 10.");
			NeedRemoveNet10Runtime = !output.Contains("Microsoft.NETCore.App 10.");
			var installed = !NeedRemoveNet10Runtime && !NeedRemoveNet10AspRuntime;
			Net10RuntimeInstalled = installed;
			return installed;
		}
		else
		{
			NeedRemoveNet10Runtime = NeedRemoveNet10AspRuntime = true;
			Net10RuntimeInstalled = false;
			return false;
		}
	}
	public abstract void InstallNet10Runtime();
	public abstract void RemoveNet10AspRuntime();
	public abstract void RemoveNet10NetRuntime();
	public virtual void RemoveNet10Runtime()
	{
		if (NeedRemoveNet10Runtime) RemoveNet10NetRuntime();
		if (NeedRemoveNet10AspRuntime) RemoveNet10AspRuntime();

		if (NeedRemoveNet10Runtime || NeedRemoveNet10AspRuntime)
			InstallLog("Removed .NET 10 Runtime");
	}
	public virtual string[] UserIsMemeberOf(CommonSettings settings) => new string[0];
	public virtual void SetFilePermissions(string folder, string user = null)
	{
		if (!Path.IsPathRooted(folder)) folder = Path.Combine(InstallWebRootPath, folder);

		if (!OSInfo.IsWindows)
		{
			Info("Grant file permissions...");
			OSInfo.Unix.GrantUnixPermissions(folder,
				UnixFileMode.UserWrite | UnixFileMode.GroupWrite |
				UnixFileMode.UserRead | UnixFileMode.GroupRead | UnixFileMode.OtherRead |
				UnixFileMode.UserExecute | UnixFileMode.GroupExecute, true);
			InstallLog($"Granted file permissions on {folder}.");
		}
		else
		{
			if (!string.IsNullOrEmpty(user))
			{
				((WindowsInstaller)this).SetFolderPermission(folder, user, NtfsPermission.FullControl);
			}
		}
	}
	public virtual void SetFileOwner(string folder, string owner, string group)
	{
		if (!Path.IsPathRooted(folder)) folder = Path.Combine(InstallWebRootPath, folder);

		if (!OSInfo.IsWindows)
		{
			if (string.IsNullOrEmpty(owner)) owner = "root";
			Info("Set file owner...");
			OSInfo.Unix.ChangeUnixFileOwner(folder, owner, group, true);
			InstallLog($"Changed file owner in {folder}.");
		}
	}

	public IEnumerable<int> ExternalPortsFromUrls(string urls)
	{
		return (urls ?? "").Split(',', ';')
			.Select(url => new Uri(url))
			.Where(uri => uri.Host != "localhost" && uri.Host != "127.0.0.1" && uri.Host != "::1")
			.Select(uri => uri.Port);
	}
	public virtual void OpenFirewall(string urls) => OpenFirewall(ExternalPortsFromUrls(urls));
	public virtual void RemoveFirewallRule(string urls) => RemoveFirewallRule(ExternalPortsFromUrls(urls));
	public virtual void OpenFirewall(int port) { }
	public virtual void RemoveFirewallRule(int port) { }
	public virtual void OpenFirewall(params IEnumerable<int> ports)
	{
		if (ports.Any())
		{
			Info("Configure firewall...");
			foreach (int port in ports) OpenFirewall(port);
			InstallLog($"Opened firewall on {string.Join(",", ports.OfType<object>().ToArray())}");
		}
	}
	public virtual void RemoveFirewallRule(params IEnumerable<int> ports)
	{
		if (ports.Any())
		{
			Info("Configure firewall...");
			foreach (int port in ports) RemoveFirewallRule(port);
			InstallLog($"Closed firewall on {string.Join(",", ports.OfType<object>().ToArray())}");
		}
	}

	protected bool IsIis7 => OSInfo.IsWindows && OSInfo.Windows.WebServer.Version.Major >= 7;
	protected bool IsHttps(CommonSettings settings)
	{
		if (!string.IsNullOrEmpty(settings.Urls))
		{
			return settings.Urls.Split(';', ',')
				.Select(url => new Uri(url))
				.Any(uri => uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase));
		}
		return (IsIis7 || !OSInfo.IsWindows) && Utils.IsHttps(UseHttpsOnWindows, settings.WebSiteIp, settings.WebSiteDomain);
	}
	public virtual string GetUrls(CommonSettings settings)
	{
		if (!string.IsNullOrEmpty(settings.Urls)) return settings.Urls;
		else
		{
			var isHttps = IsHttps(settings);
			return $"http{(isHttps ? "s" : "")}://{(!string.IsNullOrEmpty(settings.WebSiteDomain) ? settings.WebSiteDomain : settings.WebSiteIp)}:{settings.WebSitePort}";
		}
	}

	public virtual void CreateUser(CommonSettings settings)
	{
		var username = settings.Username;
		var password = settings.Password;
		string domain = "";
		if (settings.UseActiveDirectory)
		{
			var tokens = username.Split('\\');
			if (tokens.Length == 2)
			{
				domain = tokens[0];
				username = tokens[1];
			}
		}
	}
	public virtual void InstallWebsite(string name, string path, CommonSettings settings,
		string group, string dll, string description, string serviceId)
	{
		/* var remoteServerSettings = new RemoteServerSettings() { ADEnabled = false };
		// Create web users group
		if (!SecurityUtils.GroupExists(FuseCPWebUsersGroup, remoteServerSettings, ""))
		{
			var group = new SystemGroup() { GroupName = FuseCPWebUsersGroup, Name = FuseCPWebUsersGroup, Description = "FuseCP Website User Group" };
			SecurityUtils.CreateGroup(group, remoteServerSettings, "", "");
		} */

		var site = new WebSite()
		{
			ContentPath = path,
			GroupName = FuseCPWebUsersGroup,
			AspNetInstalled = "4I",
			AnonymousUsername = settings.Username ?? "",
			AnonymousUserPassword = settings.Password ?? "",
			ApplicationPool = name,
			DedicatedApplicationPool = true,
			EnableAnonymousAccess = true,
			EnableBasicAuthentication = true,
			EnableDynamicCompression = false,
			EnableWritePermissions = true,
			Name = name,
			LogsPath = WebsiteLogsPath,
		};
		if (!string.IsNullOrEmpty(settings.Urls))
		{
			site.Bindings = (settings.Urls ?? "")
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
					return new ServerBinding(uri.Scheme, ip?.ToString(), uri.Port.ToString(), host);
				})
				.ToArray();
		}
		else
		{
			bool isHttps = IsHttps(settings);

			site.Bindings = new[] {
					new ServerBinding(isHttps ? "https" : "http", settings.WebSiteIp, settings.WebSitePort.ToString(), settings.WebSiteDomain)
				};
		}
		Transaction(() =>
		{
			Info($"Install Website {name}");

			((HostingServiceProviderBase)WebServer).ProviderSettings.Settings.Add("WebGroupName", FuseCPWebUsersGroup);

			WebServer.CreateSite(site);
			InstallLog($"Installed {name} website, listening on the url(s):" +
				$"{string.Join(NewLine, (GetUrls(settings) ?? "").Split(',', ';')
					.Select(url => "  " + url))}");
		})
		.WithRollback(() => WebServer.DeleteSite(site.SiteId));

		int p = 0;
		var ports = site.Bindings
			.Where(binding => binding.IP != "127.0.0.1" && binding.IP != "::1" && binding.Host != "localhost")
			.Select(binding => int.TryParse(binding.Port, out p) ? p : 0);
		OpenFirewall(ports);
	}

	public virtual void RemoveUser(string username)
	{
		if (!string.IsNullOrEmpty(username) && OSInfo.IsWindows)
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
	public virtual void RemoveWebsite(string siteId, CommonSettings setting)
	{
		RemoveFirewallRule(GetUrls(setting));

		Info($"Delete website {siteId}");
		WebServer.DeleteSite(siteId);
		InstallLog($"Removed website {siteId}");
	}

	public void InstallService(ServiceDescription description)
	{
		Transaction(() =>
		{
			Info($"Install service {description.ServiceId}");
			var service = ServiceController.Install(description);
			service.Enable();
			var status = service.Info;
			if (status != null && status.Status == OSServiceStatus.Running) service.Stop();
			service.Start();
			InstallLog($"Installed service {description.ServiceId}");
		})
		.WithRollback(() => RemoveService(description.ServiceId));
	}
	public void RemoveService(string serviceId)
	{
		Info($"Remove service {serviceId}");
		var service = ServiceController[serviceId];
		service.Stop();
		service.Disable();
		service.Remove();
		InstallLog($"Removed service {serviceId}.");
	}
	public virtual Func<string, string> UnzipFilter => null;

	public async Task<string> DownloadFileAsync(string url)
	{
		Info($"Download {url}.");
		var web = new HttpClient();
		var tmp = Path.GetTempFileName();
		tmp = Path.ChangeExtension(tmp, Path.GetExtension(url));
		using (HttpResponseMessage response = await web.GetAsync(url))
		{
			if (response.StatusCode != System.Net.HttpStatusCode.OK) throw new Exception($"Could not download file {url}. Status code: {response.StatusCode}");

			using (var file = new FileStream(tmp, FileMode.Create, FileAccess.Write))
			{
				await response.Content.CopyToAsync(file);
			}
		}
		var name = Regex.Replace(url, @"(.*?/)|(?:\?.*$)", "", RegexOptions.Singleline);
		InstallLog($"Downloaded file {name}");
		return tmp;
	}
	public string DownloadFile(string url) => DownloadFileAsync(url).Result;
	public string SetupFilter(string file) => file != null && !Regex.IsMatch(file, @"^(?:[a-zA-Z ]+/)?Setup(?:/|$)") ? file : null;
	public string Net48Filter(string file)
	{
		file = SetupFilter(file);
		return (file != null && !Regex.IsMatch(file, "(?:^|/)bin_dotnet(?:/|$)") && !Regex.IsMatch(file, "(?:^|/)appsettings.json$", RegexOptions.IgnoreCase)) ? file : null;
	}
	public virtual string Net10Filter(string file)
	{
		file = SetupFilter(file);
		return (file != null && !Regex.IsMatch(file, "(?:^|/)bin/(?!netstandard(?:/|$))")) ? file : null;
	}
	public virtual string ConfigAndSetupFilter(string file)
	{
		file = OSInfo.IsWindows ? SetupFilter(file) : Net10Filter(file);
		return (file != null && !file.EndsWith("/web.config", StringComparison.OrdinalIgnoreCase) &&
			!Regex.IsMatch(file, "(?:^|/)appsettings.json$", RegexOptions.IgnoreCase)) ? file : null;
	}
	public virtual string CorrectEnterpriseServer(string file)
	{
		if (OSInfo.IsWindows) return file;
		if (file == null) return null;
		if (file == "Enterprise Server") return "EnterpriseServer";
		if (file.EndsWith("/Enterprise Server")) return file.Substring(0, file.Length - "/Enterprise Server".Length) + "/EnterpriseServer";
		if (file.StartsWith("Enterprise Server/")) return "EnterpriseServer/" + file.Substring("Enterprise Server/".Length);
		return file.Replace("/Enterprise Server/", "/EnterpriseServer/");
		//return Regex.Replace(file, "(?<=/|^)Enterprise Server(?=/|$)", "EnterpriseServer");
	}
	public virtual string StandaloneInstallFilter(string file) => CorrectEnterpriseServer(SetupFilter(file));
	public virtual string StandaloneUpdateFiler(string file) => CorrectEnterpriseServer(ConfigAndSetupFilter(file));
	public virtual string StandardUpdateFilter(string file) => ConfigAndSetupFilter(file);
	public virtual string StandardInstallFilter(string file) => OSInfo.IsWindows ? SetupFilter(file) : Net10Filter(file);
	public virtual void CopyFile(string source, string destination, bool settings = false)
	{
		if (File.Exists(source))
		{
			File.Copy(source, destination, true);
			if (settings)
			{
				if (IsUnix)
				{
					var owner = OSInfo.Unix.GetUnixFileOwner(source);
					OSInfo.Unix.ChangeUnixFileOwner(destination, owner.Owner, owner.Group);
					var mode = OSInfo.Unix.GetUnixPermissions(source);
					OSInfo.Unix.GrantUnixPermissions(destination, mode);
				}
				else if (IsWindows)
				{
					var sourceInfo = new FileInfo(source);
					var acl = sourceInfo.GetAccessControl();
					acl.SetAccessRuleProtection(true, true);
					var destInfo = new FileInfo(destination);
					destInfo.SetAccessControl(acl);
				}
			}
			Log.ProgressOne();
		}
	}
	public string GetFiltered(string file, string root, string destroot, Func<string, string> filter)
	{
		if (filter != null && file.StartsWith(root))
		{
			int len = root.Length;
			if (!root.EndsWith(Path.DirectorySeparatorChar.ToString()) && len < file.Length) len++;
			file = file.Substring(len)
				.Replace(Path.DirectorySeparatorChar, '/');
			var dest = filter(file);
			if (dest == null) return null;
			return Path.Combine(destroot, dest.Replace('/', Path.DirectorySeparatorChar));
		}
		else return file;
	}
	public void CopySettings(string source, string destination, bool settings)
	{
		if (settings)
		{ // Copy security settings
			if (IsUnix)
			{
				/* var owner = OSInfo.Unix.GetUnixFileOwner(source);
				OSInfo.Unix.ChangeUnixFileOwner(destination, owner.Owner, owner.Group);
				var mode = OSInfo.Unix.GetUnixPermissions(source);
				OSInfo.Unix.GrantUnixPermissions(destination, mode);*/
				var srcInfo = Mono.Unix.UnixFileSystemInfo.GetFileSystemEntry(source);
				var destInfo = Mono.Unix.UnixFileSystemInfo.GetFileSystemEntry(destination);
				destInfo.FileAccessPermissions = srcInfo.FileAccessPermissions;
				destInfo.SetOwner(srcInfo.OwnerUserId, srcInfo.OwnerGroupId);
				destInfo.Refresh();
			}
			else if (IsWindows)
			{
				var sourceInfo = new DirectoryInfo(source);
				var acl = sourceInfo.GetAccessControl();
				acl.SetAccessRuleProtection(true, true);
				var destInfo = new DirectoryInfo(destination);
				destInfo.SetAccessControl(acl);
			}
		}
	}
	private void DeleteRootDirectory(string destination)
	{
		if (destination != Settings.Standalone.InstallPath) Directory.Delete(destination, true);
		else
		{
			if (Directory.Exists(Settings.EnterpriseServer.InstallPath)) Directory.Delete(Settings.EnterpriseServer.InstallPath, true);
			if (Directory.Exists(Settings.Server.InstallPath)) Directory.Delete(Settings.Server.InstallPath, true);
			if (Directory.Exists(Settings.WebPortal.InstallPath)) Directory.Delete(Settings.WebPortal.InstallPath, true);
			if (Directory.Exists(Settings.WebDavPortal.InstallPath)) Directory.Delete(Settings.WebDavPortal.InstallPath, true);
		}
	}
	public void CopyFiles(string source, string destination, bool clearDestination = false,
		Func<string, string> filter = null, string root = null, string destroot = null, bool settings = false)
	{
		if (root == null && destroot == null)
		{
			Info("Copy files...");
			if (IsUpdateAction)
			{
				// Create backup
				if (Directory.Exists(BackupPath)) Directory.Delete(BackupPath, true);
				CopyFiles(destination, BackupPath, clearDestination, null, destination, BackupPath, true);
			}
			Transaction(() =>
			{

				if (clearDestination && Directory.Exists(destination)) DeleteRootDirectory(destination);
				CopyFiles(source, destination, clearDestination, filter, source, destination);
				// delete source temporary directory after successful copying
				Directory.Delete(source, true);
				InstallLog("Unzipped & installed distribution files");
			})
				.WithRollback(() =>
				{
					DeleteRootDirectory(destination);
					// delete source temporary directory
					Directory.Delete(source, true);
					if (IsUpdateAction)
					{
						// Restore backup
						CopyFiles(BackupPath, destination, clearDestination, null, source, BackupPath, true);
					}
				});
		}
		else
		{
			if (root == null) root = source;
			if (destroot == null) destroot = destination;
			destination = GetFiltered(source, root, destroot, filter);
			if (destination == null) return;
			if (Directory.Exists(source))
			{
				if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
				CopySettings(source, destination, settings);
				foreach (var file in Directory.GetFiles(source))
				{
					var dest = GetFiltered(file, root, destroot, filter);
					if (dest != null)
					{
						File.Copy(file, dest, true);
						CopySettings(file, dest, settings);
						Log.ProgressOne();
					}
				}
				foreach (var dir in Directory.GetDirectories(source))
				{
					var dest = GetFiltered(dir, root, destroot, filter);
					if (dest != null) CopyFiles(dir, dest, clearDestination, filter, root, destroot);
				}
			}
			else if (File.Exists(source) && destination != null)
			{
				File.Copy(source, destination, true);
				CopySettings(source, destination, settings);
				Log.ProgressOne();
			}
		}
		Log.WriteLine();
	}

	public void UnzipFromStream(Stream resource, string destinationPath, Func<string, string> filter = null)
	{
		Transaction(() =>
		{
			Info($"Unizp {Path.GetFileName(destinationPath)}");

			Directory.CreateDirectory(destinationPath);

			if (filter == null) filter = name => name;

			var dirs = new HashSet<string>();
			using (var zip = new ZipArchive(resource))
			{
				foreach (var zipEntry in zip.Entries)
				{
					var name = filter?.Invoke(zipEntry.FullName);

					if (name != null)
					{
						var fullName = Path.GetFullPath(Path.Combine(destinationPath, name.Replace('/', Path.DirectorySeparatorChar)));

						if (string.IsNullOrEmpty(zipEntry.Name))
							Directory.CreateDirectory(fullName);
						else
							zipEntry.ExtractToFile(fullName);

						Shell.Log?.Invoke($"Extracted {name}{NewLine}");
					}
				}
			}
		})
		.WithRollback(() => Directory.Delete(destinationPath, true));
	}

	public virtual void ConfigureAppsettings(CommonSettings settings)
	{
		AppSettings appsettings = null;
		string folder;
		WebPortalSettings webSettings = null;
		bool embedEnterpriseServer = false;
		if (settings is ServerSettings) folder = ServerFolder;
		else if (settings is EnterpriseServerSettings) folder = EnterpriseServerFolder;
		else if (settings is WebDavPortalSettings) folder = WebDavPortalFolder;
		else if (settings is WebPortalSettings)
		{
			folder = WebPortalFolder;
			webSettings = (WebPortalSettings)settings;
			embedEnterpriseServer = webSettings.EmbedEnterpriseServer;
		}
		else throw new NotSupportedException();

		var appsettingsfile = Path.Combine(settings.InstallPath, "bin_dotnet", "appsettings.json");
		if (File.Exists(appsettingsfile))
		{
			appsettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(appsettingsfile),
				new VersionConverter(), new StringEnumConverter()) ?? new AppSettings();
		}
		else
		{
			appsettings = new AppSettings();
		}

		var urls = settings.Urls;
		if (string.IsNullOrEmpty(urls)) urls = GetUrls(settings);

		appsettings.applicationUrls = urls;
		var allowedHosts = (urls ?? "").Split(',', ';')
			.Select(url => new Uri(url.Trim()).Host)
			.ToList();
		if (allowedHosts.Any(host => host == "localhost"))
		{
			allowedHosts.Add("127.0.0.1");
			allowedHosts.Add("::1");
		}
		if (allowedHosts.Any(host => host == "*")) appsettings.AllowedHosts = null;
		else appsettings.AllowedHosts = string.Join(";", allowedHosts.Distinct());

		if (!string.IsNullOrEmpty(settings.LetsEncryptCertificateEmail) && !string.IsNullOrEmpty(settings.LetsEncryptCertificateDomains))
		{
			appsettings.LettuceEncrypt = new AppSettings.LettuceEncryptSetting()
			{
				AcceptTermOfService = true,
				EmailAddress = settings.LetsEncryptCertificateEmail,
				DomainNames = settings.LetsEncryptCertificateDomains
					?.Split(',', ';')
					.Select(domain => domain.Trim())
					.ToArray() ?? new string[0]
			};
		}
		else if (!string.IsNullOrEmpty(settings.CertificateFile) && !string.IsNullOrEmpty(settings.CertificatePassword))
		{
			// create a local copy of the certificate file
			var certFile = settings.CertificateFile;
			var certFolder = Path.Combine(Path.GetDirectoryName(settings.InstallPath), CertificateFolder);
			if (!Directory.Exists(certFolder)) Directory.CreateDirectory(certFolder);
			var shadowFileName = $"{Guid.NewGuid()}.{Path.GetFileName(certFile)}";
			var shadowFile = Path.Combine(certFolder, shadowFileName);
			File.Copy(certFile, shadowFile);
			OSInfo.Unix.GrantUnixPermissions(shadowFile, UnixFileMode.UserRead | UnixFileMode.UserWrite);

			appsettings.Certificate = new AppSettings.CertificateSetting()
			{
				File = shadowFile,
				Password = settings.CertificatePassword
			};
		}
		else if (!string.IsNullOrEmpty(settings.CertificateStoreLocation) && !string.IsNullOrEmpty(settings.CertificateStoreName))
		{
			appsettings.Certificate = new AppSettings.CertificateSetting()
			{
				FindValue = settings.CertificateFindValue
			};
			Enum.TryParse<StoreLocation>(settings.CertificateStoreLocation, out appsettings.Certificate.StoreLocation);
			Enum.TryParse<StoreName>(settings.CertificateStoreName, out appsettings.Certificate.StoreName);
			Enum.TryParse<X509FindType>(settings.CertificateFindType, out appsettings.Certificate.FindType);
		}

		if (string.IsNullOrEmpty(appsettings.probingPaths)) appsettings.probingPaths = Path.Combine("..", "bin", "netstandard");

		if (settings is WebPortalSettings wsettings)
		{
			if (webSettings.EmbedEnterpriseServer)
			{
				appsettings.probingPaths = @$"..\bin\netstandard;..\{webSettings.EnterpriseServerPath}\bin_dotnet;..\{webSettings.EnterpriseServerPath}\bin\netstandard";
				appsettings.probingPaths = appsettings.probingPaths.Replace('\\', Path.DirectorySeparatorChar);

				var esJsonFile = Path.GetFullPath(Path.Combine(settings.InstallPath, wsettings.EnterpriseServerPath, "bin_dotnet", "appsettings.json"));
				if (File.Exists(esJsonFile))
				{
					var esSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(esJsonFile),
						new VersionConverter(), new StringEnumConverter()) ?? new AppSettings();
					appsettings.EnterpriseServer = esSettings.EnterpriseServer;

					var esCstring = appsettings.EnterpriseServer?.ConnectionString;
					var csb = new ConnectionStringBuilder(esCstring);
					if ((csb["DbType"] as string)?.Contains("Sqlite") == true)
					{
						if (csb["Data Source"] != null)
						{
							var cspath = (string)csb["Data Source"];
							cspath = Regex.Replace(cspath, $@"^\.\.{Regex.Escape(Path.DirectorySeparatorChar.ToString())}", "");
							csb["Data Source"] = Path.Combine("..", Settings.WebPortal.EnterpriseServerPath, cspath);
						}
						appsettings.EnterpriseServer.ConnectionString = csb.ConnectionString;
					}
				}
				else throw new NotSupportedException("EnterpriseServer settings file not found. You must install EnterpriseServer prior to install WebPortal with embedded EnterpriseServer.");
			}
		}
		else if (settings is EnterpriseServerSettings esettings)
		{
			var connectionString = DatabaseUtils.BuildConnectionString(esettings.DatabaseType, esettings.DatabaseServer,
				esettings.DatabasePort, esettings.DatabaseName, esettings.DatabaseUser, esettings.DatabasePassword, null, false);
			var csb = new ConnectionStringBuilder(connectionString);
			if ((csb["DbType"] as string)?.Contains("Sqlite") == true)
			{
				if (csb["Data Source"] != null)
				{
					csb["Data Source"] = Path.Combine("..", (string)csb["Data Source"]);
				}
				connectionString = csb.ConnectionString;
			}

			appsettings.EnterpriseServer = new AppSettings.EnterpriseServerSetting()
			{
				CryptoKey = esettings.CryptoKey ?? CryptoUtils.GetRandomString(20),
				ConnectionString = connectionString,
				EncryptionEnabled = true,
			};
		}
		else if (settings is ServerSettings srvSettings &&
			(!string.IsNullOrEmpty(srvSettings.ServerPassword) || !string.IsNullOrEmpty(srvSettings.ServerPasswordSHA)))
		{
			string pwsha;
			if (!string.IsNullOrEmpty(srvSettings.ServerPassword))
			{
				pwsha = CryptoUtils.ComputeSHAServerPassword(srvSettings.ServerPassword);
			}
			else
			{
				pwsha = srvSettings.ServerPasswordSHA;
			}
			appsettings.Server = new AppSettings.ServerSetting() { Password = pwsha };
		}

		var path = Path.GetDirectoryName(appsettingsfile);
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		File.WriteAllText(appsettingsfile, JsonConvert.SerializeObject(appsettings, Formatting.Indented, new JsonSerializerSettings()
		{
			ContractResolver = new AppSettings.IgnoreAllowedHostsResolver(),
			Converters = new List<JsonConverter>() { new VersionConverter(), new StringEnumConverter() }
		}));
	}

	public void ConfigureCertificateNetFX(CommonSettings settings, XElement configuration)
	{
		// server certificate
		if (!settings.ConfigureCertificateManually)
		{
			if (!string.IsNullOrEmpty(settings.CertificateStoreLocation) &&
				!string.IsNullOrEmpty(settings.CertificateStoreName) &&
				!string.IsNullOrEmpty(settings.CertificateFindType) &&
				!string.IsNullOrEmpty(settings.CertificateFindValue))
			{
				var serviceModel = configuration.Element("system.serviceModel");
				if (serviceModel == null)
				{
					serviceModel = new XElement("system.serviceModel");
					configuration.Add(serviceModel);
				}
				var behaviors = serviceModel.Element("behaviors");
				if (behaviors == null)
				{
					behaviors = new XElement("behaviors");
					serviceModel.Add(behaviors);
				}
				var serviceBehaiors = behaviors.Element("serviceBehaviors");
				if (serviceBehaiors == null)
				{
					serviceBehaiors = new XElement("serviceBehaviors");
					behaviors.Add(serviceBehaiors);
				}
				var behavior = serviceBehaiors.Element("behavior");
				if (behavior == null)
				{
					behavior = new XElement("behavior");
					serviceBehaiors.Add(behavior);
				}
				var serviceCredentials = behavior.Element("serviceCredentials");
				if (serviceCredentials == null)
				{
					serviceCredentials = new XElement("serviceCredentials");
					behavior.Add(serviceCredentials);
				}
				var cert = serviceCredentials.Element("serviceCertificate");
				if (cert != null) cert.Remove();
				cert = new XElement("serviceCertificate", new XAttribute("storeName", settings.CertificateStoreName),
					new XAttribute("storeLocation", settings.CertificateStoreLocation),
					new XAttribute("x509FindType", settings.CertificateFindType),
					new XAttribute("findValue", settings.CertificateFindValue));
				serviceCredentials.Add(cert);
			}
			else throw new PlatformNotSupportedException("Certificate file or Let's Encrypt not supported on Windows.");
		}
	}
	public virtual string GetUninstallLog(ComponentSettings settings) => "";
	public void UnzipFromResource(string resourcePath, string destinationPath, Func<string, string> filter = null)
	{
		Transaction(() =>
		{
			var assembly = GetEntryAssembly();
			var resourceName = assembly.GetManifestResourceNames()
				.FirstOrDefault(res => res.EndsWith(resourcePath, StringComparison.OrdinalIgnoreCase));

			if (resourceName == null) throw new NotSupportedException($"Cannot find {resourcePath} in resources.");

			using (var resource = assembly.GetManifestResourceStream(resourceName))
			{
				UnzipFromStream(resource, destinationPath, filter);
			}
		})
		.WithRollback(() => Directory.Delete(destinationPath));
	}
	public virtual void InstallWinAcme() => Shell.Exec("dotnet tool install win-acme --global");
	public virtual bool IsRunningAsAdmin => true;
	public virtual void RestartAsAdmin() { }

	bool exitCalled = false;
	public virtual void Exit(int errorCode = 0)
	{
		if (!exitCalled)
		{
			exitCalled = true;
			SaveSettings();
			OnExit?.Invoke();
			UI.Exit();
			Environment.Exit(errorCode);
		}
	}
	protected void ParseConnectionString(string connectionString, out EnterpriseServer.Data.DbType dbType, out string server, out string user, out string password,
		out bool windowsAuthentication, out bool trustCertificate)
	{
		server = user = password = "";
		windowsAuthentication = false;
		dbType = EnterpriseServer.Data.DbType.Unknown;

		var csb = new System.Data.Common.DbConnectionStringBuilder();
		csb.ConnectionString = connectionString;

		string get(string key) => csb.ContainsKey(key) ? csb[key] as string : null;

		string type = get("dbtype");
		Enum.TryParse<EnterpriseServer.Data.DbType>(type, out dbType);
		server = get("server");
		user = get("uid") ?? get("user id") ?? get("username") ?? get("user");
		password = get("pwd") ?? get("password");
		var security = get("integrated sceurity");
		windowsAuthentication = security.Equals("true", StringComparison.OrdinalIgnoreCase) ||
			security.Equals("sspi");
		var tc = get("trustservercertificate");
		trustCertificate = tc != null && tc.Equals("true", StringComparison.OrdinalIgnoreCase);
	}
	public abstract bool CheckOSSupported();
	public abstract bool CheckInitSystemSupported();
	public abstract bool CheckIISVersionSupported();
	public abstract bool CheckNetVersionSupported();

	public virtual void WaitForDownloadToComplete()
	{
		Info("Download & Unzip Component...");

		var progressFile = Path.Combine(TempPath, SetupLoader.DownloadProgressFile);
		var nofFilesFile = Path.Combine(TempPath, SetupLoader.NofFilesFile);
		int n = 0;
		var info = new FileInfo(progressFile);
		while (info.Exists)
		{
			while (info.Exists && n < info.Length)
			{
				Log.ProgressOne();
				n++;
			}
			Thread.Sleep(50);
			info = new FileInfo(progressFile);
		}
		while (n++ < 100) Log.ProgressOne();

		if (File.Exists(nofFilesFile))
		{
			var fileTxt = File.ReadAllText(nofFilesFile);
			Installer.Current.Files = int.Parse(fileTxt);
			File.Delete(nofFilesFile);
		}
		Log.WriteLine();
	}
	public virtual ILoadContext LoadContext
	{
		get
		{
			if (OSInfo.IsCore) return Activator.CreateInstance(GetType("FuseCP.UniversalInstaller.LoadContextImplementation, FuseCP.UniversalInstaller.Runtime.NetCore")) as ILoadContext;
			else return Activator.CreateInstance(GetType("FuseCP.UniversalInstaller.LoadContextImplementation, FuseCP.UniversalInstaller.Runtime.NetFX")) as ILoadContext;
		}
	}

	public virtual SecurityUtils SecurityUtils
	{
		get
		{
			if (OSInfo.IsCore) return Activator.CreateInstance(GetType("FuseCP.UniversalInstaller.Runtime.SecurityUtils, FuseCP.UniversalInstaller.Runtime.NetCore")) as SecurityUtils;
			else return Activator.CreateInstance(GetType("FuseCP.UniversalInstaller.Runtime.SecurityUtils, FuseCP.UniversalInstaller.Runtime.NetFX")) as SecurityUtils;
		}
	}

	public virtual WebUtils WebUtils
	{
		get
		{
			if (OSInfo.IsCore) return Activator.CreateInstance(GetType("FuseCP.UniversalInstaller.Runtime.WebUtils, FuseCP.UniversalInstaller.Runtime.NetCore")) as WebUtils;
			else return Activator.CreateInstance(GetType("FuseCP.UniversalInstaller.Runtime.WebUtils, FuseCP.UniversalInstaller.Runtime.NetFX")) as WebUtils;
		}
	}

	// Gets a Type by name. On NET Core it uses the custom AssemblyLoadContext.
	public virtual Type GetType(string name)
	{
		if (OSInfo.IsCore)
		{
			var ctxType = Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader");
			var ctx = ctxType.GetMethod("GetLoadContext").Invoke(null, new object[] { Assembly.GetExecutingAssembly() });
			if (ctx.GetType().Name == "SetupAssemblyLoadContext")
			{
				var defaultCtx = ctxType.GetProperty("Default").GetValue(null);
				var loadCustom = ctx.GetType().GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);
				var loadDefault = defaultCtx.GetType().GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic);
				return Type.GetType(name, asmName =>
					(loadCustom.Invoke(ctx, new object[] { asmName }) ??
						loadDefault.Invoke(defaultCtx, new object[] { asmName })) as Assembly,
					(Func<Assembly, string, bool, Type>)((asm, type, ignoreCase) =>
						asm.GetType(type, true, ignoreCase)));
			}
		}
		return Type.GetType(name);
	}

	static Installer current;
	public static Installer Current
	{
		get
		{
			if (current == null)
			{
				var flavor = OSInfo.OSFlavor;
				switch (flavor)
				{
					// TODO support for Ubuntu variants
					case OSFlavor.Windows: current = new WindowsInstaller(); break;
					case OSFlavor.Mac: current = new MacInstaller(); break;
					case OSFlavor.Debian: current = new DebianInstaller(); break;
					case OSFlavor.Ubuntu: current = new UbuntuInstaller(); break;
					case OSFlavor.Mint: current = new MintInstaller(); break;
					case OSFlavor.Kali: current = new KaliInstaller(); break;
					case OSFlavor.Fedora: current = new FedoraInstaller(); break;
					case OSFlavor.RedHat: current = new RedHatInstaller(); break;
					case OSFlavor.CentOS: current = new CentOSInstaller(); break;
					case OSFlavor.Alma: current = new AlmaInstaller(); break;
					case OSFlavor.Rocky: current = new RockyInstaller(); break;
					case OSFlavor.Oracle: current = new OracleInstaller(); break;
					case OSFlavor.SUSE: current = new SuseInstaller(); break;
					case OSFlavor.Arch: current = new ArchInstaller(); break;
					case OSFlavor.Alpine: current = new AlpineInstaller(); break;
					default:
						var assembly = Assembly.GetExecutingAssembly();
						var type = assembly.GetType($"FuseCP.UniversalInstaller.{flavor}");
						if (type != null) current = Activator.CreateInstance(type) as Installer;
						else throw new PlatformNotSupportedException("This OS is not supported by the installer.");
						break;
				}
				current.Shell.LogCommand += msg => current.Log.WriteInfo($"> {msg}");
			}
			return current;
		}
	}

	public bool CheckForInstallerUpdate(out ComponentUpdateInfo component)
	{
		bool ret = false;
		Log.WriteStart("Checking for a new version");
		//
		UI.ShowWaitCursor();
		var webService = Installer.Current.InstallerWebService;
		component = webService.GetLatestComponentUpdate(OSInfo.IsWindows ? Global.InstallerProductCode : Global.InstallerProductUnixCode);
		UI.EndWaitCursor();
		//
		Log.WriteEnd("Checked for a new version");
		if (component != null)
		{
			Version currentVersion = GetType().Assembly.GetName().Version;
			Version newVersion = null;
			if (component.Version != default) newVersion = component.Version;
			else
			{
				Log.WriteError("Version error");
				return false;
			}
			if (newVersion > currentVersion)
			{
				ret = true;
				Log.WriteInfo(string.Format("Version {0} is available for download", newVersion));
			}
		}
		return ret;
	}

	public bool DownloadInstallerUpdate(ComponentUpdateInfo component)
	{
		Log.WriteStart("Starting updater");
		string entry = Installer.Current.GetEntryAssembly().Location;
		string tmpFile = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(entry));
		
		File.Copy(entry, tmpFile);

		//
		string url = Releases.GetDownloadUrlAsync(new RemoteFile(component, false)).Result;
		//
		string proxyServer = string.Empty;
		string user = string.Empty;
		string password = string.Empty;

		// check if we need to add a proxy to access Internet
		bool useProxy = Installer.Current.Settings.Installer.Proxy != null;
		if (useProxy)
		{
			proxyServer = Installer.Current.Settings.Installer.Proxy.Address;
			user = Installer.Current.Settings.Installer.Proxy.Username;
			password = Installer.Current.Settings.Installer.Proxy.Password;
		}

		//prepare command line args
		StringBuilder sb = new StringBuilder();

		ProcessStartInfo info = new ProcessStartInfo();
		var isExe = Path.GetExtension(tmpFile).Equals(".exe", StringComparison.OrdinalIgnoreCase);
		var winconsole = UI.IsConsole && OSInfo.IsWindows;
		if (isExe) info.FileName = tmpFile;
		else
		{
			var runtime = Path.ChangeExtension(entry, ".runtimeconfig.json");
			var runtimedest = Path.ChangeExtension(tmpFile, ".runtimeconfig.json");
			File.Copy(runtime, runtimedest);
			info.FileName = Shell.Find(OSInfo.IsWindows ? "dotnet.exe" : "dotnet");
			sb.Append($"\"{tmpFile}\" ");
		}
		sb.Append("-update ");
		sb.Append($"-ui={UI.Current.GetType().Name.Replace("UI", "").ToLower()} ");
		sb.AppendFormat("-url:\"{0}\" ", url);
		sb.AppendFormat("-target:\"{0}\" ", entry);
		//sb.AppendFormat("-file:\"{0}\" ", component.UpgradeFilePath);
		sb.AppendFormat("-proxy:\"{0}\" ", proxyServer);
		sb.AppendFormat("-user:\"{0}\" ", user);
		sb.AppendFormat("-password:\"{0}\" ", password);
		info.Arguments = sb.ToString();
		info.UseShellExecute = winconsole;
		info.CreateNoWindow = !winconsole;

		Process process = Process.Start(info);
		if (process.Handle != IntPtr.Zero)
		{
			User32.SetForegroundWindow(process.Handle);
		}
		Log.WriteEnd("Updater started");

		if (UI.IsConsole && !Providers.OS.OSInfo.IsWindows)
		{
			UI.ShowWaitCursor();
			process.WaitForExit();
			UI.EndWaitCursor();

			Console.WriteLine("FuseCP Installer has been updated. Please restart it...");
			Installer.Current.Exit();
		}

		return (process.Handle != IntPtr.Zero);
	}
}
