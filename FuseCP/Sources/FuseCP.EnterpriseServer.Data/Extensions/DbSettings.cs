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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Win32;
using FuseCP.Providers.OS;
using System.Data;

namespace FuseCP.EnterpriseServer.Data
{
	public class DbSettings
    {

        const string EnterpriseServerRegistryPath = "SOFTWARE\\FuseCP\\EnterpriseServer";

        private static string GetKeyFromRegistry(string Key)
        {
            string value = string.Empty;

			if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
				return value;

            if (!string.IsNullOrEmpty(Key))
            {
                RegistryKey root = Registry.LocalMachine;
                RegistryKey rk = root.OpenSubKey(EnterpriseServerRegistryPath);
                if (rk != null)
                {
                    value = (string)rk.GetValue(Key, null);
                    rk.Close();
                }
            }
            return value;
        }


        static string connectionString = null;

		static string ConnectionStringNetCore
		{
			get
			{
				if (string.IsNullOrEmpty(connectionString))
				{

					string connectionKey = null;
					if (OSInfo.IsNetFX)
					{
						//connectionKey = ConfigurationManager.AppSettings["FuseCP.AltConnectionString"];
					}
					else
					{
						connectionKey = ConfigurationManager.AppSettings["FuseCP.AltConnectionString"];
					}

					string value = string.Empty;

					if (!string.IsNullOrEmpty(connectionKey) && OSInfo.IsWindows)
					{
						value = GetKeyFromRegistry(connectionKey);
					}

					if (!string.IsNullOrEmpty(value))
					{
						connectionString = value;
					}
					else
					{
						if (OSInfo.IsNetFX)
						{
							//connectionString = ConfigurationManager.ConnectionStrings["EnterpriseServer"].ConnectionString;
						}
						else
						{
							connectionString = ConfigurationManager.ConnectionStrings["EnterpriseServer"]?.ConnectionString;

						}
					}
				}
				return connectionString;
			}
		}
		static string ConnectionStringNetFX
		{
			get
			{
				if (string.IsNullOrEmpty(connectionString))
				{

					string connectionKey = null;
					if (OSInfo.IsNetFX)
					{
						connectionKey = ConfigurationManager.AppSettings["FuseCP.AltConnectionString"];
					}
					else
					{
						//connectionKey = Web.Services.Configuration.AltConnectionString;
					}

					string value = string.Empty;

					if (!string.IsNullOrEmpty(connectionKey) && OSInfo.IsWindows)
					{
						value = GetKeyFromRegistry(connectionKey);
					}

					if (!string.IsNullOrEmpty(value))
					{
						connectionString = value;
					}
					else
					{
						if (OSInfo.IsNetFX)
						{
							connectionString = ConfigurationManager.ConnectionStrings["EnterpriseServer"].ConnectionString;
						}
						else
						{
							//connectionString = Web.Services.Configuration.ConnectionString;

						}
					}
				}
				return connectionString;
			}
		}
	
		public static string ConnectionString =>
			Environment.GetEnvironmentVariable("FUSECP_CONNECTIONSTRING") ??
			(OSInfo.IsNetFX ? ConnectionStringNetFX : ConnectionStringNetCore);
		public static string NativeConnectionString => GetNativeConnectionString(ConnectionString);

        public static string GetNativeConnectionString(string connectionString)
        {
			if (string.IsNullOrWhiteSpace(connectionString))
				return string.Empty;

            return Regex.Replace(connectionString, @$"^\s*{nameof(DbType)}\s*=[^;$]*;|;\s*{nameof(DbType)}\s*=[^;$]*", "", RegexOptions.IgnoreCase);
		}
		public static DbType GetDbType(string connectionString)
        {
			if (string.IsNullOrWhiteSpace(connectionString))
				return DbType.Unknown;

            DbType dbType = DbType.Unknown;
            var dbTypeName = Regex.Match(connectionString, @$"(?<=(?:;|^)\s*{nameof(DbType)}\s*=\s*)[^;$]*", RegexOptions.IgnoreCase)?.Value.Trim();
			if (!string.IsNullOrEmpty(dbTypeName) && !Enum.TryParse<DbType>(dbTypeName, true, out dbType)) dbType = DbType.Other;
            return dbType;
		}

        static DbType dbType = DbType.Unknown;
        public static DbType DbType => dbType != DbType.Unknown ? dbType : (dbType = GetDbType(ConnectionString));

		public static bool AlwaysUseEntityFrameworkNetFX =>
			Environment.GetEnvironmentVariable("FUSECP_ALWAYS_USE_ENTITY_FRAMEWORK") == "true" ||
			string.Equals(ConfigurationManager.AppSettings["FuseCP.AlwaysUseEntityFramework"], "true", StringComparison.OrdinalIgnoreCase);
	}
}
