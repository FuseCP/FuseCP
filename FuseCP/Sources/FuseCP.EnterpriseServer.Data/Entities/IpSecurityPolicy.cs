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

#if ScaffoldedEntities
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuseCP.EnterpriseServer.Data.Entities;

/// <summary>
/// Stores IP whitelist and blacklist entries for security enforcement.
/// Supports individual IP addresses and CIDR ranges.
/// </summary>
[Table("IpSecurityPolicies")]
public partial class IpSecurityPolicy
{
    /// <summary>Gets or sets the primary key.</summary>
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the IP address or CIDR range (e.g. "192.168.1.1" or "10.0.0.0/8").
    /// </summary>
    [Required]
    [StringLength(50)]
    public string IpRange { get; set; }

    /// <summary>
    /// Gets or sets whether this is a whitelist entry.
    /// true = whitelist (always allow), false = blacklist (always deny).
    /// </summary>
    public bool IsWhitelist { get; set; }

    /// <summary>Gets or sets the UTC creation timestamp.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Gets or sets the optional UTC expiration timestamp.</summary>
    public DateTime? ExpiresDate { get; set; }

    /// <summary>Gets or sets the human-readable reason for this policy entry.</summary>
    [StringLength(500)]
    public string Reason { get; set; }

    /// <summary>Gets or sets whether this policy entry is currently active.</summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the severity level for blacklisted IPs.
    /// 0 = low, 1 = medium, 2 = high, 3 = critical (auto-blacklisted).
    /// </summary>
    public int SeverityLevel { get; set; }

    /// <summary>Gets or sets the username or system that created this entry.</summary>
    [StringLength(255)]
    public string CreatedBy { get; set; }
}
#endif
