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

﻿using System;
using System.IO;
//using System.Configuration;
using FuseCP.Providers;
using FuseCP.Providers.Common;
using FuseCP.Providers.OS;
using FuseCP.Server.Utils;
using System.Reflection;

namespace FuseCP.Server.Code
{
    public class AutoDiscoveryHelper
    {
        public const string DisableAutoDiscovery = "DisableAutoDiscovery";
        public static BoolResult IsInstalled(string name)
        {
            
            Log.WriteStart("IsInstalled started. Name:{0}", name);
            BoolResult res = new BoolResult {IsSuccess = true};

            try
            {
                bool disableAutoDiscovery;

                if (!bool.TryParse(System.Configuration.ConfigurationManager.AppSettings[DisableAutoDiscovery], out disableAutoDiscovery))
                    disableAutoDiscovery = false;
                
                if (disableAutoDiscovery)
                {
                    res.Value = true;
                    
                }
                else
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        res.IsSuccess = false;
                        res.ErrorCodes.Add(ErrorCodes.PROVIDER_NAME_IS_NOT_SPECIFIED);
                        return res;
                    }

                    Type providerType = Type.GetType(name);
                    IHostingServiceProvider provider = (IHostingServiceProvider)Activator.CreateInstance(providerType);
                    res.Value = provider.IsInstalled();
                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ErrorCodes.Add(ErrorCodes.CANNOT_CREATE_PROVIDER_INSTANCE);
                Log.WriteError(ex);
            }
            finally
            {
                Log.WriteEnd("IsInstalled ended. Name:{0}", name);
            }

            return res;
        }

        public static string GetServerFilePath() {
#if NETFRAMEWORK
            return System.Web.HttpContext.Current.Server.MapPath("~/");
#else
            return new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..")).FullName;
#endif
        }

        public static string GetServerVersion()
        {
            object[] attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
            if (attrs.Length > 0)
                return ((AssemblyFileVersionAttribute)attrs[0]).Version;
            else
			    return typeof(AutoDiscoveryHelper).Assembly.GetName().Version.ToString(3);
        }
        public static string OS => OSInfo.IsWindows ? "Windows" :
			(OSInfo.IsMac ? "Mac" :
			(OSInfo.IsLinux ? "Linux" : "Unix"));
		public static bool IsServerPasswordSHA256 => Cryptor.IsSHA256(Settings.Password);

        public static ServerAuthenticationInfo GetServerAuthenticationInfo()
        {
            return new ServerAuthenticationInfo
            {
                Version = 2,
                SupportsHmacAuthentication = true,
                SupportsLegacyPasswordAuthentication = Settings.AllowLegacyPasswordAuthentication,
                PasswordIsSha256 = IsServerPasswordSHA256,
                AllowedClockSkewSeconds = ServerRequestAuthentication.DefaultAllowedClockSkewSeconds,
                KeyId = ServerRequestAuthentication.BuildKeyId(Settings.Password),
                ClusterId = string.Empty
            };
        }
	}
}
