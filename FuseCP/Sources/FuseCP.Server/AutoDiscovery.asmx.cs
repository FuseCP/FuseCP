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
using System.ComponentModel;
using FuseCP.Providers.Common;
using FuseCP.Server.Code;
using FuseCP.Web.Services;
using FuseCP.Providers.OS;

namespace FuseCP.Server
{
	/// <summary>
	/// Summary description for AutoDiscovery
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class AutoDiscovery : WebService {

        [WebMethod]
        public BoolResult IsInstalled(string providerName) {
            return AutoDiscoveryHelper.IsInstalled(providerName);
        }

        [WebMethod]
        public string GetServerFilePath() {
            return AutoDiscoveryHelper.GetServerFilePath();
        }

        [WebMethod]
        public string GetServerVersion()
        {
            return AutoDiscoveryHelper.GetServerVersion();
        }


        [WebMethod]
		public OSPlatformInfo GetOSPlatform()
		{
            return OSInfo.Current.GetOSPlatform();
        }

        [WebMethod]
        public bool GetServerPasswordIsSHA256() => AutoDiscoveryHelper.IsServerPasswordSHA256;

        [WebMethod]
        public FuseCP.Providers.ServerAuthenticationInfo GetServerAuthenticationInfo() => AutoDiscoveryHelper.GetServerAuthenticationInfo();

	}
}
