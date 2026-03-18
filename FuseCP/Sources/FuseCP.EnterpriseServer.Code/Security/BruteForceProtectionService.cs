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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer.Data;
using FuseCP.EnterpriseServer.Data.Entities;

namespace FuseCP.EnterpriseServer.Security
{
    /// <summary>
    /// Provides brute force protection by tracking authentication attempts per IP address
    /// and enforcing rate limits across all authentication layers.
    /// </summary>
    public class BruteForceProtectionService : ControllerBase
    {
        /// <summary>Authentication layers tracked by this service.</summary>
        public static class Layers
        {
            /// <summary>Portal web login.</summary>
            public const string Portal = "Portal";
            /// <summary>Enterprise Server API (SOAP/REST).</summary>
            public const string Api = "API";
            /// <summary>Physical server communication.</summary>
            public const string Server = "Server";
            /// <summary>Third-party module calls (e.g. WHMCS).</summary>
            public const string Module = "Module";
        }

        // Default thresholds. These can be overridden via system settings in the future.
        private const int DefaultMaxFailedAttempts = 5;
        private static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan DefaultIpLockoutDuration = TimeSpan.FromMinutes(15);
        private const int CriticalAttemptThreshold = 20;

        /// <inheritdoc/>
        public BruteForceProtectionService(ControllerBase provider) : base(provider) { }

        /// <inheritdoc/>
        public BruteForceProtectionService() : base() { }

        /// <summary>
        /// Records an authentication attempt and returns the updated block status.
        /// If recording causes the IP to exceed thresholds the IP is automatically blocked.
        /// </summary>
        /// <param name="ipAddress">The remote IP address.</param>
        /// <param name="username">The username attempted (may be null for non-portal layers).</param>
        /// <param name="layer">The authentication layer (use <see cref="Layers"/> constants).</param>
        /// <param name="succeeded">Whether this attempt succeeded.</param>
        /// <param name="userAgent">Optional User-Agent string.</param>
        /// <returns>
        /// <c>true</c> if the IP is now blocked (either it was already blocked or the threshold
        /// was just exceeded); <c>false</c> if the IP is allowed to continue.
        /// </returns>
        public bool RecordAttempt(string ipAddress, string username, string layer, bool succeeded,
            string userAgent = null)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            // Always allow whitelisted IPs.
            if (IsIpWhitelisted(ipAddress))
                return false;

            // Persist the log entry.
            var log = new BruteForceLog
            {
                IpAddress = ipAddress,
                Username = username,
                Layer = layer ?? Layers.Portal,
                AttemptTime = DateTime.UtcNow,
                Succeeded = succeeded,
                UserAgent = userAgent
            };
            Database.BruteForceLogs.Add(log);
            Database.SaveChanges();

            if (succeeded)
                return false;

            // Count recent failures from this IP on this layer.
            var windowStart = DateTime.UtcNow.Subtract(DefaultWindow);
            var recentFailures = Database.BruteForceLogs
                .Count(l => l.IpAddress == ipAddress
                         && l.Layer == layer
                         && !l.Succeeded
                         && l.AttemptTime >= windowStart);

            // Critical threshold → permanent (high severity) blacklist.
            if (recentFailures >= CriticalAttemptThreshold)
            {
                EnsureBlacklisted(ipAddress, "Auto-blacklisted: critical attempt threshold exceeded",
                    severityLevel: 3, expiry: null);
                return true;
            }

            // Standard threshold → timed lockout.
            if (recentFailures >= DefaultMaxFailedAttempts)
            {
                EnsureBlacklisted(ipAddress, $"Auto-blocked after {recentFailures} failed {layer} attempts",
                    severityLevel: 1, expiry: DateTime.UtcNow.Add(DefaultIpLockoutDuration));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the given IP address is currently blocked.
        /// </summary>
        /// <param name="ipAddress">The remote IP address.</param>
        /// <returns><c>true</c> if the IP is blocked; otherwise <c>false</c>.</returns>
        public bool IsIpBlocked(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            if (IsIpWhitelisted(ipAddress))
                return false;

            var now = DateTime.UtcNow;

            return Database.IpSecurityPolicies.Any(p =>
                p.IpRange == ipAddress
                && !p.IsWhitelist
                && p.IsActive
                && (p.ExpiresDate == null || p.ExpiresDate > now));
        }

        /// <summary>
        /// Determines whether the given IP address is whitelisted (safe path).
        /// Supports both exact matches and CIDR ranges.
        /// </summary>
        /// <param name="ipAddress">The remote IP address to test.</param>
        /// <returns><c>true</c> if the IP is whitelisted; otherwise <c>false</c>.</returns>
        public bool IsIpWhitelisted(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return false;

            var now = DateTime.UtcNow;

            var whitelistEntries = Database.IpSecurityPolicies
                .Where(p => p.IsWhitelist && p.IsActive && (p.ExpiresDate == null || p.ExpiresDate > now))
                .Select(p => p.IpRange)
                .ToList();

            return whitelistEntries.Any(entry => IpMatchesEntry(ipAddress, entry));
        }

        /// <summary>
        /// Manually blacklists an IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to block.</param>
        /// <param name="reason">Human-readable reason for the block.</param>
        /// <param name="createdBy">Username of the administrator performing the block.</param>
        /// <param name="expiry">Optional UTC expiration time; null means permanent.</param>
        public void BlockIp(string ipAddress, string reason, string createdBy = "admin",
            DateTime? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return;

            EnsureBlacklisted(ipAddress, reason, severityLevel: 2, expiry: expiry,
                createdBy: createdBy);
        }

        /// <summary>
        /// Removes all active blacklist entries for the given IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address to unblock.</param>
        public void UnblockIp(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return;

            var entries = Database.IpSecurityPolicies
                .Where(p => p.IpRange == ipAddress && !p.IsWhitelist && p.IsActive)
                .ToList();

            foreach (var e in entries)
                e.IsActive = false;

            Database.SaveChanges();
        }

        /// <summary>
        /// Adds an IP address or CIDR range to the whitelist (safe path).
        /// </summary>
        /// <param name="ipRange">An exact IP address or CIDR range (e.g. "10.0.0.0/8").</param>
        /// <param name="reason">Human-readable reason for whitelisting.</param>
        /// <param name="createdBy">Username of the administrator performing the action.</param>
        /// <param name="expiry">Optional UTC expiration time; null means permanent.</param>
        public void WhitelistIp(string ipRange, string reason, string createdBy = "admin",
            DateTime? expiry = null)
        {
            if (string.IsNullOrWhiteSpace(ipRange))
                return;

            // Remove any active blacklist entries for this range first.
            var blocked = Database.IpSecurityPolicies
                .Where(p => p.IpRange == ipRange && !p.IsWhitelist && p.IsActive)
                .ToList();
            foreach (var b in blocked)
                b.IsActive = false;

            // Add whitelist entry if one doesn't already exist.
            var existing = Database.IpSecurityPolicies
                .FirstOrDefault(p => p.IpRange == ipRange && p.IsWhitelist && p.IsActive);

            if (existing == null)
            {
                Database.IpSecurityPolicies.Add(new IpSecurityPolicy
                {
                    IpRange = ipRange,
                    IsWhitelist = true,
                    CreatedDate = DateTime.UtcNow,
                    ExpiresDate = expiry,
                    Reason = reason,
                    IsActive = true,
                    SeverityLevel = 0,
                    CreatedBy = createdBy
                });
            }

            Database.SaveChanges();
        }

        /// <summary>
        /// Returns paged brute force log entries ordered by most recent first.
        /// </summary>
        /// <param name="ipAddress">Optional filter by IP address.</param>
        /// <param name="layer">Optional filter by authentication layer.</param>
        /// <param name="failedOnly">When true, only failed attempts are returned.</param>
        /// <param name="skip">Number of records to skip for pagination.</param>
        /// <param name="take">Number of records to take per page.</param>
        public List<BruteForceLog> GetAttempts(string ipAddress = null, string layer = null,
            bool failedOnly = false, int skip = 0, int take = 50)
        {
            IQueryable<BruteForceLog> query = Database.BruteForceLogs;

            if (!string.IsNullOrEmpty(ipAddress))
                query = query.Where(l => l.IpAddress == ipAddress);

            if (!string.IsNullOrEmpty(layer))
                query = query.Where(l => l.Layer == layer);

            if (failedOnly)
                query = query.Where(l => !l.Succeeded);

            return query.OrderByDescending(l => l.AttemptTime)
                        .Skip(skip)
                        .Take(take)
                        .ToList();
        }

        /// <summary>
        /// Returns all currently active IP security policies.
        /// </summary>
        /// <param name="whitelistOnly">When true, returns only whitelist entries.</param>
        /// <param name="blacklistOnly">When true, returns only blacklist entries.</param>
        public List<IpSecurityPolicy> GetPolicies(bool whitelistOnly = false, bool blacklistOnly = false)
        {
            var now = DateTime.UtcNow;

            IQueryable<IpSecurityPolicy> query = Database.IpSecurityPolicies
                .Where(p => p.IsActive && (p.ExpiresDate == null || p.ExpiresDate > now));

            if (whitelistOnly) query = query.Where(p => p.IsWhitelist);
            if (blacklistOnly) query = query.Where(p => !p.IsWhitelist);

            return query.OrderByDescending(p => p.CreatedDate).ToList();
        }

        // ------------------------------------------------------------------ //
        // Private helpers
        // ------------------------------------------------------------------ //

        private void EnsureBlacklisted(string ipAddress, string reason,
            int severityLevel, DateTime? expiry, string createdBy = "system")
        {
            // Update the existing entry if there is one; otherwise insert.
            var existing = Database.IpSecurityPolicies
                .FirstOrDefault(p => p.IpRange == ipAddress && !p.IsWhitelist && p.IsActive);

            if (existing != null)
            {
                // Escalate severity if needed; extend expiry.
                if (severityLevel > existing.SeverityLevel)
                    existing.SeverityLevel = severityLevel;
                if (expiry == null || (existing.ExpiresDate.HasValue && expiry > existing.ExpiresDate))
                    existing.ExpiresDate = expiry;
            }
            else
            {
                Database.IpSecurityPolicies.Add(new IpSecurityPolicy
                {
                    IpRange = ipAddress,
                    IsWhitelist = false,
                    CreatedDate = DateTime.UtcNow,
                    ExpiresDate = expiry,
                    Reason = reason,
                    IsActive = true,
                    SeverityLevel = severityLevel,
                    CreatedBy = createdBy
                });
            }

            Database.SaveChanges();
        }

        /// <summary>
        /// Matches an IP address against an entry that may be an exact IP or a CIDR range.
        /// </summary>
        private static bool IpMatchesEntry(string ipAddress, string entry)
        {
            if (entry.Contains('/'))
                return IsIpInCidr(ipAddress, entry);

            return string.Equals(ipAddress, entry, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether <paramref name="ipAddress"/> falls within the CIDR range.
        /// Supports both IPv4 and IPv6.
        /// </summary>
        public static bool IsIpInCidr(string ipAddress, string cidr)
        {
            try
            {
                var parts = cidr.Split('/');
                if (parts.Length != 2)
                    return false;

                if (!System.Net.IPAddress.TryParse(ipAddress, out var ip))
                    return false;

                if (!System.Net.IPAddress.TryParse(parts[0], out var networkAddress))
                    return false;

                if (!int.TryParse(parts[1], out var prefixLength))
                    return false;

                var ipBytes = ip.GetAddressBytes();
                var networkBytes = networkAddress.GetAddressBytes();

                if (ipBytes.Length != networkBytes.Length)
                    return false;

                int fullBytes = prefixLength / 8;
                int remainingBits = prefixLength % 8;

                for (int i = 0; i < fullBytes; i++)
                {
                    if (ipBytes[i] != networkBytes[i])
                        return false;
                }

                if (remainingBits > 0 && fullBytes < ipBytes.Length)
                {
                    var mask = (byte)(0xFF << (8 - remainingBits));
                    if ((ipBytes[fullBytes] & mask) != (networkBytes[fullBytes] & mask))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
