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
/// Records each authentication attempt for brute force detection.
/// Tracks attempts per IP address, username, and authentication layer.
/// </summary>
[Table("BruteForceLogs")]
public partial class BruteForceLog
{
    /// <summary>Gets or sets the primary key.</summary>
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the IP address that made the attempt.</summary>
    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; }

    /// <summary>Gets or sets the username attempted (may be null for API layer).</summary>
    [StringLength(255)]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the authentication layer.
    /// Values: "Portal", "API", "Server", "Module"
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Layer { get; set; }

    /// <summary>Gets or sets the UTC timestamp of this attempt.</summary>
    public DateTime AttemptTime { get; set; }

    /// <summary>Gets or sets whether this attempt succeeded.</summary>
    public bool Succeeded { get; set; }

    /// <summary>Gets or sets the User-Agent header value (optional).</summary>
    [StringLength(500)]
    public string UserAgent { get; set; }
}
#endif
