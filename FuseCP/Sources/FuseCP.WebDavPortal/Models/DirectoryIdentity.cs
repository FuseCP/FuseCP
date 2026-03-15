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
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Principal;

namespace FuseCP.WebDavPortal.Models
{
    [SupportedOSPlatform("windows")]
    public class DirectoryIdentity : IIdentity
    {
        private readonly bool _auth;
        private readonly string _path;

        public DirectoryIdentity(string userName, string password) : this(null, userName, password)
        {
        }

        public DirectoryIdentity(string path, string userName, string password) : this(new DirectoryEntry(path, userName, password))
        {
        }

        public DirectoryIdentity(DirectoryEntry directoryEntry)
        {
            try
            {
                var userName = directoryEntry.Username;
                var ds = new DirectorySearcher(directoryEntry);
                if (userName.Contains("\\"))
                    userName = userName.Substring(userName.IndexOf("\\") + 1);
                ds.Filter = "samaccountname=" + userName;
                ds.PropertiesToLoad.Add("cn");
                SearchResult sr = ds.FindOne();
                if (sr == null) throw new Exception();
                _path = sr.Path;
                _auth = true;
            }
            catch (AmbiguousMatchException)
            {
                _auth = false;
            }
        }

        public string AuthenticationType
        {
            get { return null; }
        }

        public bool IsAuthenticated
        {
            get { return _auth; }
        }

        public string Name
        {
            get
            {
                int i = _path.IndexOf('=') + 1, j = _path.IndexOf(',');
                return _path.Substring(i, j - i);
            }
        }
    }
}
