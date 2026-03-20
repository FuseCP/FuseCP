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

#if Client
// Copyright (c) 2016, FuseCP
// FuseCP is distributed under the Creative Commons Share-alike license
// 
// FuseCP is a fork of WebsitePanel:
// Copyright (c) 2015, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Microsoft.Web.Services3;
//using Microsoft.Web.Services3.Design;

using FuseCP.Providers;

namespace FuseCP.Server.Client
{
	public class ServerProxyConfigurator
	{
		public const bool UseNetHttpAsDefaultProtocol = true;
		public const bool UseMessageSecurityOverHttp = true;
		public const bool UseNetHttpOnCore = true;
		public const bool UseMessageSecurityOnCore = false;

		private int timeout = -1;
		private string serverUrl = string.Empty;
		private string serverPassword = string.Empty;
		RemoteServerSettings serverSettings = new RemoteServerSettings();
		ServiceProviderSettings providerSettings = new ServiceProviderSettings();

		public FuseCP.Providers.RemoteServerSettings ServerSettings
		{
			get { return this.serverSettings; }
			set { this.serverSettings = value; }
		}

		public FuseCP.Providers.ServiceProviderSettings ProviderSettings
		{
			get { return this.providerSettings; }
			set { this.providerSettings = value; }
		}

		public string ServerUrl
		{
			get { return this.serverUrl; }
			set { this.serverUrl = value; }
		}

		public string ServerPassword
		{
			get { return this.serverPassword; }
			set { this.serverPassword = value; }
		}

		public bool PasswordIsSHA256 { get; set; } = false;
		public bool UseServerRequestAuthentication { get; set; } = true;
		public int Timeout
		{
			get { return this.timeout; }
			set { this.timeout = value; }
		}

		public bool? IsCore { get; set; } = null;

		public void Configure(FuseCP.Web.Clients.ClientBase proxy)
		{
			// configure proxy URL
			if (!String.IsNullOrEmpty(serverUrl))
			{
				if (serverUrl.EndsWith("/"))
					serverUrl = serverUrl.Substring(0, serverUrl.Length - 1);

				proxy.Url = serverUrl; // + proxy.Url.Substring(proxy.Url.LastIndexOf('/'));
			}

			// set proxy timeout
			proxy.Timeout = (timeout == -1) ? null : TimeSpan.FromMilliseconds(timeout * 1000);

			// setup security assertion
			if (!String.IsNullOrEmpty(serverPassword) && proxy.IsAuthenticated)
			{
				//ServerUsernameAssertion assert
				//     = new ServerUsernameAssertion(ServerSettings.ServerId, serverPassword);

				// create policy
				//Policy policy = new Policy();
				//policy.Assertions.Add(assert);

				//proxy.SetPolicy(policy);
				proxy.Credentials.Password = serverPassword;
				proxy.Credentials.UserName = UseServerRequestAuthentication ? string.Empty : "legacy";
			}

			// provider settings
			ServiceProviderSettingsSoapHeader settingsHeader = new ServiceProviderSettingsSoapHeader();
			List<string> settings = new List<string>();

			// AD Settings
			settings.Add("AD:Enabled=" + ServerSettings.ADEnabled.ToString());
			settings.Add("AD:AuthenticationType=" + ServerSettings.ADAuthenticationType.ToString());
			settings.Add("AD:ParentDomain=" + ServerSettings.ADParentDomain);
			settings.Add("AD:ParentDomainController=" + ServerSettings.ADParentDomainController);
			settings.Add("AD:RootDomain=" + ServerSettings.ADRootDomain);
			settings.Add("AD:Username=" + ServerSettings.ADUsername);
			settings.Add("AD:Password=" + ServerSettings.ADPassword);

			// Server Settings
			settings.Add("Server:ServerId=" + ServerSettings.ServerId);
			settings.Add("Server:ServerName=" + ServerSettings.ServerName);

			// Provider Settings
			settings.Add("Provider:ProviderGroupID=" + ProviderSettings.ProviderGroupID.ToString());
			settings.Add("Provider:ProviderCode=" + ProviderSettings.ProviderCode);
			settings.Add("Provider:ProviderName=" + ProviderSettings.ProviderName);
			settings.Add("Provider:ProviderType=" + ProviderSettings.ProviderType);

			// Custom Provider Settings
			foreach (string settingName in ProviderSettings.Settings.Keys)
			{
				settings.Add(settingName + "=" + ProviderSettings.Settings[settingName]);
			}

			// set header
			settingsHeader.Settings = settings.ToArray();

			if (proxy.HasSoapHeaders && (proxy.IsEncrypted || proxy.IsLocal))
			{
				proxy.SoapHeader = settingsHeader;
			}
			//FieldInfo field = proxy.GetType().GetField("ServiceProviderSettingsSoapHeaderValue");
			//if (field != null)
			//    field.SetValue(proxy, settingsHeader);

			// Use NetHttp as default protocol or WSHttp if UseMessageSecurityOverHttp is set
			if (proxy.IsDefaultApi)
			{
				if (UseMessageSecurityOverHttp && proxy.IsHttp && proxy.IsEncrypted && !proxy.IsLocal &&
					(UseMessageSecurityOnCore || IsCore.HasValue && IsCore.Value == false))
				{
					proxy.Protocol = Web.Clients.Protocols.WSHttp;
				}
				else if (UseNetHttpAsDefaultProtocol &&
					(UseNetHttpOnCore || IsCore.HasValue && IsCore.Value == false))
				{
					if (proxy.IsHttp) proxy.Protocol = Web.Clients.Protocols.NetHttp;
					else if (proxy.IsHttps) proxy.Protocol = Web.Clients.Protocols.NetHttps;
					else if (proxy.IsSsh) proxy.Protocol = Web.Clients.Protocols.NetHttp;
				} 
			} else if (proxy.IsSsh && proxy.Protocol == Web.Clients.Protocols.NetTcpSsl)
			{
				proxy.Protocol = Web.Clients.Protocols.NetTcp;
			}
		}
	}
}
#endif
