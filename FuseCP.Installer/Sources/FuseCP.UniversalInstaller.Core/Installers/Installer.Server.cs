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
using System.Reflection;
using FuseCP.Providers;
using FuseCP.Providers.Web;
using FuseCP.Providers.OS;
using FuseCP.Providers.Utils;
using System.Globalization;
using System.Security.Policy;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Converters;
using static Azure.Core.HttpHeader;
using static System.Collections.Specialized.BitVector32;

namespace FuseCP.UniversalInstaller;

public abstract partial class Installer
{
	public virtual string ServerSiteId => $"{FuseCP}Server";
	public virtual string UnixServerServiceId => "fusecp-server";

	public virtual void InstallServerPrerequisites() { }
	public virtual void RemoveServerPrerequisites() { }
	public virtual void CreateServerUser() => CreateUser(Settings.Server);
	public virtual void RemoveServerUser() => RemoveUser(Settings.Server.Username);
	public virtual void SetServerFilePermissions() => SetFilePermissions(Settings.Server.InstallPath, Settings.Server.Username);
	public virtual void SetServerFileOwner() => SetFileOwner(Settings.Server.InstallPath, Settings.Server.Username, FuseCPGroup);
	/// <summary>
	/// Pre-creates appsettings.hardened.json so the IIS app pool can write to it (file-level access)
	/// without needing broader directory-create permission. The file is loaded at startup as an optional
	/// config overlay (higher priority than appsettings.json) and is written by HardenServerAuthentication.
	/// </summary>
	public virtual void PrepareServerHardenedConfig()
	{
		string hardenedFile = Path.Combine(Settings.Server.InstallPath, "appsettings.hardened.json");
		if (!File.Exists(hardenedFile))
		{
			File.WriteAllText(hardenedFile, "{}");
			InstallLog($"Created {hardenedFile}");
		}
	}

	public virtual void InstallServer()
	{
		InstallServerPrerequisites();
		CopyServer(true, StandardInstallFilter);
		CreateServerUser();
		ConfigureServer();
		PrepareServerHardenedConfig();
		SetServerFilePermissions();
		SetServerFileOwner();
		InstallServerWebsite();
	}
	public virtual void UpdateServer()
	{
		InstallServerPrerequisites();
		CopyServer(true, StandardUpdateFilter);
		UpdateServerConfig();
		ConfigureServer();
		PrepareServerHardenedConfig();
		SetServerFilePermissions();
		SetServerFileOwner();
		InstallServerWebsite();
	}
	public virtual void SetupServer()
	{
		RemoveServerWebsite();
		ConfigureServer();
		InstallServerWebsite();
	}
	public virtual void RemoveServer()
	{
		RemoveServerWebsite();
		RemoveServerFolder();
		RemoveServerUser();
	}

	public virtual void InstallServerUser() { }
	public virtual void InstallServerApplicationPool() { }
	public virtual void InstallServerWebsite()
	{
		var web = Settings.Server.InstallPath;
		var dll = Path.Combine(web, "bin_dotnet", "FuseCP.Server.dll");
		InstallWebsite(ServerSiteId,
			web,
			Settings.Server,
			FuseCPUnixGroup,
			dll,
			"FuseCP.Server service, the server management service for the FuseCP control panel.",
			UnixServerServiceId);
	}
	public virtual void RemoveServerWebsite()
	{
		RemoveWebsite(ServerSiteId, Settings.Server);
	}
	public virtual void RemoveServerFolder()
	{
		var dir = Settings.Server.InstallPath;
		if (Directory.Exists(dir)) Directory.Delete(dir, true);
		InstallLog("Removed Server files");
	}
	public virtual void RemoveServerApplicationPool() { }
	public virtual void ReadServerConfigurationNetFX()
	{
		// Read Server configuration from Server Web.config

		Settings.Server = new ServerSettings();

		var confFile = Path.Combine(Settings.Server.InstallPath, "Web.config");

		if (File.Exists(confFile))
		{
			var webconf = XElement.Load(confFile);
			var configuration = webconf.Element("configuration");

			// server certificate
			var cert = configuration?.Element("system.serviceModel/behaviors/serviceBehaviors/behavior/serviceCredentials/serviceCertificate");
			if (cert != null)
			{
				Settings.Server.CertificateStoreLocation = cert.Attribute("storeLocation")?.Value;
				Settings.Server.CertificateStoreName = cert.Attribute("storeName")?.Value;
				Settings.Server.CertificateFindType = cert.Attribute("x509FindType")?.Value;
				Settings.Server.CertificateFindValue = cert.Attribute("findValue")?.Value;
			}

			Settings.Server.ServerPasswordSHA = Settings.Server.ServerPassword = "";
			// server password
			var password = configuration?.Element("FuseCP.server/security/password");
			if (password != null)
			{
				Settings.Server.ServerPasswordSHA = password.Attribute("value")?.Value;
			}
		}
	}
	public virtual void ReadServerConfiguration() => ReadEnterpriseServerConfigurationNetFX();
	public virtual void UpdateServerConfig() {
		var configFile = Path.Combine(Settings.Server.InstallPath, "Web.config");
		var config = XElement.Load(configFile);
		var sections = config.Element("configSections");
		// Remove WSE3
		sections
			?.Elements("section")
			.FirstOrDefault(e => e.Attribute("name")?.Value == "microsoft.web.services3")
			?.Remove();
		config
			.Elements("microsoft.web.services3")
			.FirstOrDefault()
			?.Remove();
		// Add SwaggerWCF
		sections.Add(new XElement("section",
			new XAttribute("name", "swaggerwcf"),
			new XAttribute("type", "SwaggerWcf.Configuration.SwaggerWcfSection, SwaggerWcf")));
		var swaggerwcf = XElement.Parse(@"<swaggerwcf>
		<settings>
			<setting name=""InfoDescription"" value=""FuseCP Server Service"" />
			<setting name=""InfoVersion"" value=""2.0.0"" />
			<setting name=""InfoTermsOfService"" value=""Terms of Service"" />
			<setting name=""InfoTitle"" value=""FuseCP Server Service"" />
			<setting name=""InfoContactName"" value=""FuseCP"" />
			<setting name=""InfoContactUrl"" value=""http://fusecp.com/forum"" />
			<setting name=""InfoContactEmail"" value=""support@fusecp.com"" />
			<setting name=""InfoLicenseUrl"" value=""https://github.com/FuseCP/FuseCP/blob/main/LICENSE"" />
			<setting name=""InfoLicenseName"" value=""Creative Commons Share-alike"" />
		</settings>
	</swaggerwcf>");
		config.Add(swaggerwcf);

		// Adjust httpRuntime
		var systemWeb = config.Element("system.web");
		var httpRuntime = systemWeb?.Element("httpRuntime");
		if (httpRuntime == null) systemWeb.Add(httpRuntime = new XElement("httpRuntime"));
		var requestLength = httpRuntime.Attribute("maxRequestLength");
		if (requestLength == null)
		{
			httpRuntime.Add(requestLength = new XAttribute("maxRequestLength", 4194304));
		}
		else requestLength.SetValue(4194304);
		var targetFramework = httpRuntime.Attribute("targetFramework");
		if (targetFramework == null)
		{
			httpRuntime.Add(targetFramework = new XAttribute("targetFramework", "4.8"));
		}
		else targetFramework.SetValue("4.8");
		var allowDynamicModuleRegistration = httpRuntime.Attribute("allowDynamicModuleRegistration");
		if (allowDynamicModuleRegistration == null)
		{
			httpRuntime.Add(allowDynamicModuleRegistration = new XAttribute("allowDynamicModuleRegistration", "true"));
		}
		else allowDynamicModuleRegistration.SetValue("true");
		systemWeb?.Element("webServices")?.Remove();

		// Block bin_dotnet
		var location = XElement.Parse(@"<location path=""bin_dotnet"">
        <system.webServer>
            <handlers>
                <add name=""DisallowServe"" path=""*.*"" verb=""*"" type=""System.Web.HttpNotFoundHandler"" />
                <!-- Return 404 instead of 403 -->
            </handlers>
        </system.webServer>
    </location>");
		config.Add(location);

		// Add system.webServers
		var systemWebServer = XElement.Parse(@"<system.webServer>
		<modules runAllManagedModulesForAllRequests=""true"" />
        <!-- hide the bin_dotnet folder -->
        <security>
            <requestFiltering>
                <hiddenSegments>
                    <add segment=""bin_dotnet"" />
                </hiddenSegments>
                <requestLimits maxAllowedContentLength=""4194304"" />
            </requestFiltering>
        </security>
        <httpErrors errorMode=""Detailed"" />
		<!--
        To browse web app root directory during debugging, set the value below to true.
        Set to false before deployment to avoid disclosing web app folder information.
      -->
	</system.webServer>");
		if (config.Element("system.webServer") == null) config.Add(systemWebServer);
		else config.Element("system.webServer").ReplaceWith(systemWebServer);

		// Add WCF certificate
		var systemServiceModel = config.Element("system.serviceModel");
		var behaviors = systemServiceModel?.Element("behaviors");
		if (behaviors == null)
		{
			systemServiceModel.Add(behaviors = new XElement("behaviors"));
		}
		var serviceBehaviors = XElement.Parse(@"<serviceBehaviors>
				<behavior>
					<serviceCredentials>
						<serviceCertificate storeLocation=""LocalMachine"" storeName=""My"" x509FindType=""FindBySubjectName"" findValue=""localhost"" />
					</serviceCredentials>
				</behavior>
			</serviceBehaviors>");
		behaviors?.Add(serviceBehaviors);

		// Runtime
		var runtime = config.Element("runtime");
		if (runtime == null) config.Add(runtime = new XElement("runtime"));

		runtime.ReplaceWith(XElement.Parse(@"<runtime>
		<assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
			<probing privatePath=""bin/Crm2011;bin/Crm2013;bin/Exchange2013;bin/Exchange2016;bin/Exchange2019;bin/Sharepoint2013;bin/Sharepoint2016;bin/Sharepoint2019;bin/Lync2013;bin/SfB2015;bin/SfB2019;bin/Lync2013HP;bin/IceWarp;bin/IIs80;bin/IIs100;bin/HyperV2012R2;bin/HyperVvmm;bin/Crm2015;bin/Crm2016;bin/Filters;bin/Database;bin/DNS;bin/Providers;bin/Server;bin/EnterpriseServer;bin/netstandard"" />
            <dependentAssembly>
                <assemblyIdentity name=""System.Memory""
                    publicKeyToken=""cc7b13ffcd2ddd51""
                    culture=""neutral"" />
                <bindingRedirect oldVersion=""4.0.0.0-4.0.5.0"" newVersion=""4.0.5.0"" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name=""System.Buffers"" publicKeyToken=""cc7b13ffcd2ddd51"" culture=""neutral"" />
                <bindingRedirect oldVersion=""4.0.0.0-4.0.5.0"" newVersion=""4.0.5.0"" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name=""System.Collections.Immutable"" publicKeyToken=""b03f5f7f11d50a3a"" culture=""neutral""/>
                <bindingRedirect oldVersion=""0.0.0.0-10.0.0.0"" newVersion=""10.0.0.0""/>
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name=""System.Runtime.CompilerServices.Unsafe""
                    publicKeyToken=""b03f5f7f11d50a3a""
                    culture=""neutral"" />
                <bindingRedirect oldVersion=""4.0.0.0-6.0.3.0"" newVersion=""6.0.3.0"" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name=""System.Text.Encoding.CodePages""
                    publicKeyToken=""b03f5f7f11d50a3a""
                    culture=""neutral"" />
                <bindingRedirect oldVersion=""4.0.0.0-10.0.0.0"" newVersion=""10.0.0.0"" />
            </dependentAssembly>
            <dependentAssembly>
                <assemblyIdentity name=""System.Diagnostics.EventLog""
                    publicKeyToken=""cc7b13ffcd2ddd51""
                    culture=""neutral"" />
                <bindingRedirect oldVersion=""4.0.0.0-10.0.0.0"" newVersion=""10.0.0.0"" />
            </dependentAssembly>
        </assemblyBinding>
	</runtime>"));

		File.WriteAllText(configFile, config.ToString());
	}
	public virtual void ConfigureServerNetFX()
	{
		var settings = Settings.Server;
		var confFile = Path.Combine(Settings.Server.InstallPath, "Web.config");
		var configuration = XElement.Load(confFile);

		ConfigureCertificateNetFX(settings, configuration);

		// Server password
		var server = configuration.Element("FuseCP.server");
		var security = server?.Element("security");
		var password = security?.Element("password");
		var pwsha = string.IsNullOrEmpty(settings.ServerPassword) ? settings.ServerPasswordSHA : CryptoUtils.ComputeSHAServerPassword(settings.ServerPassword);
		password.Attribute("value").SetValue(pwsha);

		// Swagger Version
		var swaggerwcf = configuration.Element("swaggerwcf");
		var swagsetting = swaggerwcf?.Element("settings");
		var versionInfo = swagsetting?.Elements("setting").FirstOrDefault(e => e.Attribute("name")?.Value == "InfoVersion");
		if (versionInfo != null)
		{
			var version = Settings.Installer.Component.Version;
			versionInfo.Attribute("value").SetValue(version);
		}

		configuration.Save(confFile);
	}
	public virtual void ConfigureServerNetCore() => ConfigureAppsettings(Settings.Server);
	public virtual void ConfigureServer(bool standalone = false)
	{
		ConfigureServerNetFX();
		ConfigureServerNetCore();

		InstallLog("Configured Server.");
	}
	public virtual void CopyServer(bool clearDestination = false, Func<string, string> filter = null)
	{
		filter ??= SetupFilter;
		var websitePath = Settings.Server.InstallPath;
		InstallWebRootPath = Path.GetDirectoryName(websitePath);
		CopyFiles(ComponentTempPath, websitePath, clearDestination, filter);
	}
	public virtual int InstallServerMaxProgress => 100;
	public virtual int UninstallServerMaxProgress => 100;
	public virtual int SetupServerMaxProgress => 100;
	public virtual int UpdateServerMaxProgress => 100;
}
