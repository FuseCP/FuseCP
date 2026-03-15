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
using System.Collections.Generic;
using System.Threading;
using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Client;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace FuseCP.WebDav.Core.Scp.Framework
{
    // FCP.Services

    public class FCP
    {
        private const string WebServicesKey = "WebServices";
        private static readonly AsyncLocal<Dictionary<string, object>> CurrentItems = new AsyncLocal<Dictionary<string, object>>();
        private readonly ICryptography _cryptography;

        public static IServiceProvider ServiceProvider { get; set; }

        private static Dictionary<string, object> Items
        {
            get
            {
                if (CurrentItems.Value == null)
                {
                    CurrentItems.Value = new Dictionary<string, object>();
                }

                return CurrentItems.Value;
            }
        }

        protected FCP()
        {
            _cryptography = ServiceProvider?.GetService<ICryptography>() ?? new CryptoUtils();
        }

        public static FCP Services
        {
            get
            {
                FCP services = null;

                if (Items.ContainsKey(WebServicesKey))
                {
                    services = Items[WebServicesKey] as FCP;
                }

                if (services == null)
                {
                    services = new FCP();
                    Items[WebServicesKey] = services;
                }

                return services;
            }
        }

        public esCRM CRM
        {
            get
            {
                return GetCachedProxy<esCRM>();
            }
        }
        

        public esVirtualizationServer VPS
        {
            get { return GetCachedProxy<esVirtualizationServer>(); }
        }

        public esVirtualizationServerForPrivateCloud VPSPC
        {
            get { return GetCachedProxy<esVirtualizationServerForPrivateCloud>(); }
        }

        public esBlackBerry BlackBerry
        {
            get { return GetCachedProxy<esBlackBerry>(); }
        }
        
        public esOCS OCS
        {
            get { return GetCachedProxy<esOCS>(); }
        }


        public esLync Lync
        {
            get { return GetCachedProxy<esLync>(); }
        }

        public esSfB SfB
        {
            get { return GetCachedProxy<esSfB>(); }
        }


        public esOrganizations Organizations
        {
            get
            {
                return GetCachedProxy<esOrganizations>();
            }
        }

		public esSystem System
		{
			get { return GetCachedProxy<esSystem>(); }
		}

        public esAuditLog AuditLog
        {
            get { return GetCachedProxy<esAuditLog>(); }
        }

        public esAuthentication Authentication
        {
            get { return GetCachedProxy<esAuthentication>(false); }
        }

        public esComments Comments
        {
            get { return GetCachedProxy<esComments>(); }
        }

        public esDatabaseServers DatabaseServers
        {
            get { return GetCachedProxy<esDatabaseServers>(); }
        }

        public esFiles Files
        {
            get { return GetCachedProxy<esFiles>(); }
        }

        public esFtpServers FtpServers
        {
            get { return GetCachedProxy<esFtpServers>(); }
        }

        public esMailServers MailServers
        {
            get { return GetCachedProxy<esMailServers>(); }
        }

        public esOperatingSystems OperatingSystems
        {
            get { return GetCachedProxy<esOperatingSystems>(); }
        }

        public esPackages Packages
        {
            get { return GetCachedProxy<esPackages>(); }
        }

        public esScheduler Scheduler
        {
            get { return GetCachedProxy<esScheduler>(); }
        }

        public esTasks Tasks
        {
            get { return GetCachedProxy<esTasks>(); }
        }

        public esServers Servers
        {
            get { return GetCachedProxy<esServers>(); }
        }

        public esStatisticsServers StatisticsServers
        {
            get { return GetCachedProxy<esStatisticsServers>(); }
        }

        public esUsers Users
        {
            get { return GetCachedProxy<esUsers>(); }
        }

        public esWebServers WebServers
        {
            get { return GetCachedProxy<esWebServers>(); }
        }

        public esSharePointServers SharePointServers
        {
            get { return GetCachedProxy<esSharePointServers>(); }
        }

		public esHostedSharePointServers HostedSharePointServers
		{
			get { return GetCachedProxy<esHostedSharePointServers>(); }
		}

        public esImport Import
        {
            get { return GetCachedProxy<esImport>(); }
        }

        public esBackup Backup
        {
            get { return GetCachedProxy<esBackup>(); }
        }

		public esExchangeServer ExchangeServer
		{
			get { return GetCachedProxy<esExchangeServer>(); }
		}


        public esEnterpriseStorage EnterpriseStorage
        {
            get { return GetCachedProxy<esEnterpriseStorage>(); }
        }

        public esRemoteDesktopServices RDS
        {
            get { return GetCachedProxy<esRemoteDesktopServices>(); }
        }
        
        protected virtual T GetCachedProxy<T>()
        {
            return GetCachedProxy<T>(true);
        }

        protected virtual T GetCachedProxy<T>(bool secureCalls)
        {
            Type t = typeof(T);
            string key = t.FullName + ".ServiceProxy";
            T proxy = default(T);

            if (Items.ContainsKey(key))
            {
                proxy = (T)Items[key];
            }

            if (proxy == null)
            {
                proxy = (T)Activator.CreateInstance(t);
                Items[key] = proxy;
            }

            object p = proxy;

            // configure proxy
			ConfigureEnterpriseServerProxy((FuseCP.Web.Clients.ClientBase)p, secureCalls);

            return proxy;
        }

        public void ConfigureEnterpriseServerProxy(FuseCP.Web.Clients.ClientBase proxy, bool applyPolicy)
        {
            // load ES properties
            string serverUrl = WebDavAppConfigManager.Instance.EnterpriseServerUrl;

            EnterpriseServerProxyConfigurator cnfg = new EnterpriseServerProxyConfigurator();
            cnfg.EnterpriseServerUrl = serverUrl;

            // create assertion
            if (applyPolicy)
            {

                cnfg.Username = WebDavAppConfigManager.Instance.FuseCPConstantUserParameters.Login;
                cnfg.Password = _cryptography.Decrypt(WebDavAppConfigManager.Instance.FuseCPConstantUserParameters.Password);
            }

            cnfg.Configure(proxy);
        }
    }
}
