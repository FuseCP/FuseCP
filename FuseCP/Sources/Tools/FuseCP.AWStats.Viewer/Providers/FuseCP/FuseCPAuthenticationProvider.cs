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
using System.Configuration;

using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Client;
using FuseCP.Providers.Statistics;

namespace FuseCP.AWStats.Viewer
{
	/// <summary>
	/// Summary description for FuseCPAuthenticationProvider.
	/// </summary>
	public class FuseCPAuthenticationProvider : AuthenticationProvider
	{
		public override AuthenticationResult AuthenticateUser(string domain, string username, string password)
		{
            try
            {
                // authentication
                esAuthentication auth = new esAuthentication();
                SetupProxy(auth);

                int result = auth.AuthenticateUser(username, password, "");

                if (result == -109)
                    return AuthenticationResult.WrongUsername;
                else if (result == -110)
                    return AuthenticationResult.WrongPassword;

                // load user account
                UserInfo user = auth.GetUserByUsernamePassword(username, password, "");
                if (user == null)
                    return AuthenticationResult.WrongUsername;

                // get all packages
                esPackages packagesProxy = new esPackages();
                SetupProxy(packagesProxy, username, password);
                esStatisticsServers statsServers = new esStatisticsServers();
                SetupProxy(statsServers, username, password);
                PackageInfo[] packages = packagesProxy.GetMyPackages(user.UserId);

                // load all statistics sites from all packages
                foreach (PackageInfo package in packages)
                {
                    StatsSite[] sites = statsServers.GetStatisticsSites(package.PackageId, false);

                    foreach (StatsSite site in sites)
                    {
                        if (String.Compare(site.Name, domain, true) == 0)
                            return AuthenticationResult.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            return AuthenticationResult.DomainNotFound;
		}

        private void SetupProxy(Web.Clients.ClientBase proxy)
        {
            SetupProxy(proxy, null, null);
        }

        private void SetupProxy(Web.Clients.ClientBase proxy,
            string username, string password)
        {
            // create ES configurator
            string serverUrl = ConfigurationManager.AppSettings["AWStats.FuseCPAuthenticationProvider.EnterpriseServer"];
            if (String.IsNullOrEmpty(serverUrl))
                throw new Exception("Enterprise Server URL could not be empty");

            EnterpriseServerProxyConfigurator cnfg = new EnterpriseServerProxyConfigurator();
            cnfg.EnterpriseServerUrl = serverUrl;
            cnfg.Username = username;
            cnfg.Password = password;
            cnfg.Configure(proxy);
        }
	}
}
