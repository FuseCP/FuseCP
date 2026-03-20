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
using FuseCP.Providers;
using FuseCP.Web.Services;

namespace FuseCP.Server
{
    public class Settings
    {

#if NETFRAMEWORK
	    public static string Password => ServerConfiguration.Security.Password;
#else
	    public static string Password => Configuration.Password;
        public static bool AllowLegacyPasswordAuthentication => Configuration.AllowLegacyPasswordAuthentication;
#endif

        static string cryptoKey = null;
        public static string CryptoKey => cryptoKey ?? (cryptoKey = Cryptor.SHA256($"{Password}{DateTime.Now}"));

        public static void ApplyAuthenticationSettings(string password, bool allowLegacyPasswordAuthentication)
        {
            Configuration.Password = password ?? string.Empty;
            Configuration.AllowLegacyPasswordAuthentication = allowLegacyPasswordAuthentication;
            cryptoKey = null;
        }
    }
}
