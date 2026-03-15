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
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace FuseCP.WebDav.Core.Security.Authentication.Principals
{
    public class ScpPrincipal : IPrincipal
    {
        public int AccountId { get; set; }
        public string OrganizationId { get; set; }
        public int ItemId { get; set; }

        public string Login { get; set; }

        public string DisplayName { get; set; }
        public string AccountName { get; set; }

        public string UserName
        {
            get
            {
                return !string.IsNullOrEmpty(Login) ? Login.Split('@')[0] : string.Empty;
            }
        }

        [XmlIgnore, JsonIgnore]
        public IIdentity Identity { get; private set; }

        public string EncryptedPassword { get; set; }

        public ScpPrincipal(string username)
        {
            Identity = new GenericIdentity(username);//new WindowsIdentity(username, "WindowsAuthentication");
            Login = username;
	    }

        public ScpPrincipal()
        {
        }

        public bool IsInRole(string role)
        {
            return Identity.IsAuthenticated 
                && !string.IsNullOrWhiteSpace(role) 
                && Identity is ClaimsIdentity claimsIdentity
                && claimsIdentity.HasClaim(claim =>
                    string.Equals(claim.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(claim.Value, role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
