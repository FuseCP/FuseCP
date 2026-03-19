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
using System.DirectoryServices;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using FuseCP.EnterpriseServer.Security;

using FuseCP.Server.Client;

namespace FuseCP.EnterpriseServer
{
	public class ServiceProviderProxy: ControllerBase
	{
		const int ServerAuthenticationCacheSeconds = 300;

		class CachedServerAuthenticationMode
		{
			public DateTime CachedUtc { get; set; }
			public bool UseServerRequestAuthentication { get; set; }
			public bool PasswordIsSHA256 { get; set; }
		}

		static readonly ConcurrentDictionary<int, CachedServerAuthenticationMode> ServerAuthenticationCache =
			new ConcurrentDictionary<int, CachedServerAuthenticationMode>();

		public static void ClearServerAuthenticationCache(int serverId)
		{
			ServerAuthenticationCache.TryRemove(serverId, out _);
		}

		public ServiceProviderProxy(ControllerBase provider) : base(provider) { }

		public FuseCP.Web.Clients.ClientBase Init(FuseCP.Web.Clients.ClientBase proxy, int serviceId, StringDictionary additionalSettings = null)
		{
			ServerProxyConfigurator cnfg = new ServerProxyConfigurator();

			// get service
			ServiceInfo service = ServerController.GetServiceInfo(serviceId);

			if (service == null)
				throw new Exception($"Service with ID {serviceId} was not found");

			// set service settings
			StringDictionary serviceSettings = ServerController.GetServiceSettings(serviceId);
			foreach (string key in serviceSettings.Keys)
				cnfg.ProviderSettings.Settings[key] = serviceSettings[key];
			// RB ADDED EMAIL SECURITY SE
			if (additionalSettings != null)
			{
				foreach (string str in additionalSettings.Keys)
				{
					cnfg.ProviderSettings.Settings[str] = additionalSettings[str];
				}
			}
			// get provider
			ProviderInfo provider = ServerController.GetProvider(service.ProviderId);
			cnfg.ProviderSettings.ProviderGroupID = provider.GroupId;
			cnfg.ProviderSettings.ProviderCode = provider.ProviderName;
			cnfg.ProviderSettings.ProviderName = provider.DisplayName;
			cnfg.ProviderSettings.ProviderType = provider.ProviderType;

			// init service on the server level
			return ServerInit(proxy, cnfg, service.ServerId);
		}

		public FuseCP.Web.Clients.ClientBase ServerInit(FuseCP.Web.Clients.ClientBase proxy, ServerProxyConfigurator cnfg, int serverId)
		{
			// get server info
			ServerInfo server = ServerController.GetServerByIdInternal(serverId);

			if (server == null)
				throw new Exception(String.Format("Server with ID {0} was not found", serverId));

			// set AD integration settings
			cnfg.ServerSettings.ADEnabled = server.ADEnabled;
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
			{
				cnfg.ServerSettings.ADAuthenticationType = AuthenticationTypes.Secure;

				AuthenticationTypes adAuthenticationType;
				if (Enum.TryParse<AuthenticationTypes>(server.ADAuthenticationType, true, out adAuthenticationType))
					cnfg.ServerSettings.ADAuthenticationType = adAuthenticationType;
			}

			cnfg.ServerSettings.ADRootDomain = server.ADRootDomain;
			cnfg.ServerSettings.ADUsername = server.ADUsername;
			cnfg.ServerSettings.ADPassword = server.ADPassword;
			cnfg.ServerSettings.ADParentDomain = server.ADParentDomain;
			cnfg.ServerSettings.ADParentDomainController = server.ADParentDomainController;

			// set timeout
			cnfg.Timeout = ConfigSettings.ServerRequestTimeout;
			
			// set if server is running on net core
			cnfg.IsCore = server.IsCore;

			var authenticationMode = ResolveServerAuthenticationMode(serverId, server.ServerUrl, server.PasswordIsSHA256);
			cnfg.PasswordIsSHA256 = authenticationMode.PasswordIsSHA256;
			cnfg.UseServerRequestAuthentication = authenticationMode.UseServerRequestAuthentication;

			return ServerInit(proxy, cnfg, server.ServerUrl, server.Password);
		}

		CachedServerAuthenticationMode ResolveServerAuthenticationMode(int serverId, string encryptedServerUrl, bool defaultPasswordIsSHA256)
		{
			if (ServerAuthenticationCache.TryGetValue(serverId, out var cached))
			{
				if ((DateTime.UtcNow - cached.CachedUtc).TotalSeconds <= ServerAuthenticationCacheSeconds)
					return cached;
			}

			var resolved = new CachedServerAuthenticationMode
			{
				CachedUtc = DateTime.UtcNow,
				UseServerRequestAuthentication = false,
				PasswordIsSHA256 = defaultPasswordIsSHA256
			};

			try
			{
				var autoDiscovery = new AutoDiscovery();
				ServerInit(autoDiscovery, encryptedServerUrl, string.Empty, false);

				var info = autoDiscovery.GetServerAuthenticationInfo();
				if (info != null)
				{
					resolved.UseServerRequestAuthentication = info.SupportsHmacAuthentication;
					resolved.PasswordIsSHA256 = info.PasswordIsSha256;
					TryRecordServerAuthAttempt(encryptedServerUrl, serverId, succeeded: true);
				}
			}
			catch
			{
				try
				{
					var autoDiscovery = new AutoDiscovery();
					ServerInit(autoDiscovery, encryptedServerUrl, string.Empty, false);
					resolved.PasswordIsSHA256 = autoDiscovery.GetServerPasswordIsSHA256();
					resolved.UseServerRequestAuthentication = false;
					TryRecordServerAuthAttempt(encryptedServerUrl, serverId, succeeded: true);
				}
				catch
				{
					resolved.UseServerRequestAuthentication = false;
					resolved.PasswordIsSHA256 = defaultPasswordIsSHA256;
					TryRecordServerAuthAttempt(encryptedServerUrl, serverId, succeeded: false);
				}
			}

			ServerAuthenticationCache[serverId] = resolved;
			return resolved;
		}

		void TryRecordServerAuthAttempt(string encryptedServerUrl, int serverId, bool succeeded)
		{
			try
			{
				var serverUrl = CryptoUtils.DecryptServerUrl(encryptedServerUrl);
				if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out var uri))
					return;

				string remoteAddress = uri.Host;
				if (string.IsNullOrWhiteSpace(remoteAddress))
					return;

				var bruteForce = new BruteForceProtectionService(this);
				bruteForce.RecordAttempt(remoteAddress, $"server:{serverId}", BruteForceProtectionService.Layers.Server, succeeded);
			}
			catch
			{
				// Best-effort logging only.
			}
		}

		private static FuseCP.Web.Clients.ClientBase ServerInit(FuseCP.Web.Clients.ClientBase proxy,
			 ServerProxyConfigurator cnfg, string serverUrl, string serverPassword)
		{			
			// set URL & password
			cnfg.ServerUrl = CryptoUtils.DecryptServerUrl(serverUrl);
			if (proxy.IsAuthenticated)
			{
				cnfg.ServerPassword = cnfg.PasswordIsSHA256 ? CryptoUtils.SHA256(serverPassword) : CryptoUtils.SHA1(serverPassword);
			}

			// configure proxy!
			cnfg.Configure(proxy);

			return proxy;
		}

		public FuseCP.Web.Clients.ClientBase ServerInit(FuseCP.Web.Clients.ClientBase proxy,
			 string serverUrl, string serverPassword, bool sha256Password)
		{
			return ServerInit(proxy, new ServerProxyConfigurator() { PasswordIsSHA256 = sha256Password }, serverUrl, serverPassword);
		}

		public FuseCP.Web.Clients.ClientBase ServerInit(FuseCP.Web.Clients.ClientBase proxy, int serverId)
		{
			return ServerInit(proxy, new ServerProxyConfigurator(), serverId);
		}
	}
}
