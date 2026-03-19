// Copyright (C) 2026 FuseCP
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

namespace FuseCP.EnterpriseServer
{
    [Serializable]
    public class IpSecurityPolicyInfo
    {
        public int Id { get; set; }
        public string IpRange { get; set; }
        public bool IsWhitelist { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public int SeverityLevel { get; set; }
        public string CreatedBy { get; set; }
    }
}
