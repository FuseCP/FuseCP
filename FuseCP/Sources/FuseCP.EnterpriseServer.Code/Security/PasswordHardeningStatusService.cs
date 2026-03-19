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

namespace FuseCP.EnterpriseServer.Security
{
    public class PasswordHardeningStatusService : ControllerBase
    {
        private const int DefaultBruteForceMaxFailedAttempts = 5;
        private const int DefaultBruteForceWindowMinutes = 30;
        private const int DefaultBruteForceLockoutMinutes = 15;
        private const int DefaultBruteForceCriticalAttempts = 20;

        public PasswordHardeningStatusService(ControllerBase provider) : base(provider) { }

        public PasswordHardeningStatusService() : base() { }

        public PasswordHardeningStatusInfo GetStatus()
        {
            var status = new PasswordHardeningStatusInfo
            {
                GeneratedUtc = DateTime.UtcNow,
                SetupModeEnabled = SystemController.GetSystemSetupMode(),
                UserLazyMigrationEnabled = true,
                ApiBruteForceProtectionEnabled = true,
                ServerRequestAuthenticationEnabled = true
            };

            PopulatePortalSettings(status);
            PopulateUserStatus(status);
            PopulateServerStatus(status);

            return status;
        }

        public List<LegacyPasswordUserInfo> GetLegacyUsers(int maxResults, string usernameFilter = null)
        {
            int take = maxResults <= 0 ? 200 : Math.Min(maxResults, 2000);
            string filter = string.IsNullOrWhiteSpace(usernameFilter)
                ? null
                : usernameFilter.Trim().ToLowerInvariant();

            var users = Database.Users
                .Select(u => new { u.UserId, u.Username, u.Email, u.Password, u.IsDemo, u.RoleId, u.Changed })
                .ToList();

            var results = new List<LegacyPasswordUserInfo>();

            foreach (var user in users.OrderBy(u => u.Username))
            {
                if (!string.IsNullOrEmpty(filter) && (user.Username == null || user.Username.ToLowerInvariant().Contains(filter) == false))
                    continue;

                string storedPassword = NormalizeStoredPassword(user.Password);
                if (string.IsNullOrWhiteSpace(storedPassword))
                {
                    results.Add(new LegacyPasswordUserInfo
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        PasswordStatus = "Empty",
                        CanAutoHarden = false,
                        IsDemo = user.IsDemo,
                        RoleId = user.RoleId,
                        Changed = user.Changed
                    });
                }
                else if (!PasswordHardeningService.IsHardenedHash(storedPassword))
                {
                    bool canAutoHarden = CanAutoHardenStoredPassword(storedPassword);
                    results.Add(new LegacyPasswordUserInfo
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        PasswordStatus = canAutoHarden ? "Legacy SHA256 proof" : "Legacy reset required",
                        CanAutoHarden = canAutoHarden,
                        IsDemo = user.IsDemo,
                        RoleId = user.RoleId,
                        Changed = user.Changed
                    });
                }

                if (results.Count >= take)
                    break;
            }

            return results;
        }

        public AutoHardenEligibleUsersResultInfo AutoHardenEligibleUsers()
        {
            var users = Database.Users
                .Select(u => new { u.UserId, u.Password })
                .ToList();

            int eligibleUserCount = 0;
            int convertedUserCount = 0;

            foreach (var user in users)
            {
                string storedPassword = NormalizeStoredPassword(user.Password);
                if (!CanAutoHardenStoredPassword(storedPassword))
                    continue;

                eligibleUserCount++;
                string hardenedPassword = PasswordHardeningService.HashClientPasswordProof(storedPassword);
                Database.ChangeUserPassword(-1, user.UserId, CryptoUtils.Encrypt(hardenedPassword));
                convertedUserCount++;
            }

            int remainingLegacyUserCount = Database.Users
                .Select(u => u.Password)
                .ToList()
                .Select(NormalizeStoredPassword)
                .Count(storedPassword => string.IsNullOrWhiteSpace(storedPassword) || !PasswordHardeningService.IsHardenedHash(storedPassword));

            return new AutoHardenEligibleUsersResultInfo
            {
                ConvertedUserCount = convertedUserCount,
                EligibleUserCountBeforeRun = eligibleUserCount,
                RemainingLegacyUserCount = remainingLegacyUserCount
            };
        }

        private void PopulatePortalSettings(PasswordHardeningStatusInfo status)
        {
            var authSettings = SystemController.GetSystemSettingsInternal(SystemSettings.AUTHENTICATION_SETTINGS, false);
            var portalSettings = SystemController.GetSystemSettingsInternal(SystemSettings.WEBDAV_PORTAL_SETTINGS, false);

            status.PortalPasswordResetEnabled = ConvertToBool(portalSettings?[SystemSettings.WEBDAV_PASSWORD_RESET_ENABLED_KEY], false);
            status.PortalPasswordResetLinkLifeSpan = portalSettings?[SystemSettings.WEBDAV_PASSWORD_RESET_LINK_LIFE_SPAN] ?? string.Empty;
            status.MfaDisplayNameConfigured = !string.IsNullOrWhiteSpace(authSettings?[SystemSettings.MFA_TOKEN_APP_DISPLAY_NAME]);
            status.CanPeerChangeMfa = ConvertToBool(authSettings?[SystemSettings.MFA_CAN_PEER_CHANGE_MFA], true);

            status.BruteForceMaxFailedAttempts = ParseSetting(authSettings?[SystemSettings.AUTH_BRUTEFORCE_MAX_FAILED_ATTEMPTS], DefaultBruteForceMaxFailedAttempts);
            status.BruteForceWindowMinutes = ParseSetting(authSettings?[SystemSettings.AUTH_BRUTEFORCE_WINDOW_MINUTES], DefaultBruteForceWindowMinutes);
            status.BruteForceLockoutMinutes = ParseSetting(authSettings?[SystemSettings.AUTH_BRUTEFORCE_LOCKOUT_MINUTES], DefaultBruteForceLockoutMinutes);
            status.BruteForceCriticalAttemptThreshold = ParseSetting(authSettings?[SystemSettings.AUTH_BRUTEFORCE_CRITICAL_ATTEMPTS], DefaultBruteForceCriticalAttempts);
        }

        private void PopulateUserStatus(PasswordHardeningStatusInfo status)
        {
            var users = Database.Users
                .Select(u => new { u.UserId, u.Password })
                .ToList();

            status.TotalUserCount = users.Count;

            foreach (var user in users)
            {
                string storedPassword = NormalizeStoredPassword(user.Password);
                if (string.IsNullOrWhiteSpace(storedPassword))
                {
                    status.EmptyUserPasswordCount++;
                    continue;
                }

                if (PasswordHardeningService.IsHardenedHash(storedPassword))
                {
                    status.HardenedUserCount++;
                }
                else
                {
                    status.LegacyUserCount++;
					if (CanAutoHardenStoredPassword(storedPassword))
						status.AutoHardenEligibleUserCount++;
                }
            }
        }

        private void PopulateServerStatus(PasswordHardeningStatusInfo status)
        {
            var servers = ServerController.GetAllServers() ?? new List<ServerInfo>();
            var serverStatuses = new List<ServerPasswordHardeningStatusInfo>();

            status.TotalServerCount = servers.Count;

            foreach (var server in servers.OrderBy(s => s.ServerName))
            {
                var serverStatus = new ServerPasswordHardeningStatusInfo
                {
                    ServerId = server.ServerId,
                    ServerName = server.ServerName,
                    PasswordIsSha256 = server.PasswordIsSHA256,
                    ProbeSucceeded = false,
                    SupportsHmacAuthentication = false,
                    SupportsLegacyPasswordAuthentication = false,
                    KeyId = string.Empty,
                    Status = "Probe failed"
                };

                try
                {
                    var authenticationInfo = ServerController.GetServerAuthenticationInfo(CryptoUtils.EncryptServerUrl(server.ServerUrl));
                    if (authenticationInfo != null)
                    {
                        serverStatus.ProbeSucceeded = true;
                        serverStatus.PasswordIsSha256 = authenticationInfo.PasswordIsSha256;
                        serverStatus.SupportsHmacAuthentication = authenticationInfo.SupportsHmacAuthentication;
                        serverStatus.SupportsLegacyPasswordAuthentication = authenticationInfo.SupportsLegacyPasswordAuthentication;
                        serverStatus.KeyId = authenticationInfo.KeyId ?? string.Empty;
						serverStatus.Status = authenticationInfo.SupportsHmacAuthentication && authenticationInfo.PasswordIsSha256 && !authenticationInfo.SupportsLegacyPasswordAuthentication
							? "Secure"
							: (authenticationInfo.SupportsLegacyPasswordAuthentication ? "Legacy fallback enabled" : "Legacy compatibility");
                    }
                }
                catch
                {
                }

                if (serverStatus.ProbeSucceeded)
                {
                    if (serverStatus.PasswordIsSha256)
                        status.Sha256ServerCount++;

                    if (serverStatus.SupportsHmacAuthentication)
                        status.HmacCapableServerCount++;

					if (!serverStatus.SupportsHmacAuthentication || !serverStatus.PasswordIsSha256 || serverStatus.SupportsLegacyPasswordAuthentication)
                        status.LegacyServerCount++;
                }
                else
                {
                    status.ServerProbeFailureCount++;
                }

                serverStatuses.Add(serverStatus);
            }

            status.Servers = serverStatuses.ToArray();
        }

        private static string NormalizeStoredPassword(string encryptedOrStoredPassword)
        {
            if (string.IsNullOrWhiteSpace(encryptedOrStoredPassword))
                return string.Empty;

            try
            {
                return CryptoUtils.Decrypt(encryptedOrStoredPassword);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                return encryptedOrStoredPassword;
            }
            catch (FormatException)
            {
                return encryptedOrStoredPassword;
            }
        }

        private static bool CanAutoHardenStoredPassword(string storedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword) || PasswordHardeningService.IsHardenedHash(storedPassword))
                return false;

            return CryptoUtils.IsSHA256(storedPassword);
        }

        private static int ParseSetting(string value, int defaultValue)
        {
            return int.TryParse(value, out int parsedValue) ? parsedValue : defaultValue;
        }

        private static bool ConvertToBool(string value, bool defaultValue)
        {
            return bool.TryParse(value, out bool parsedValue) ? parsedValue : defaultValue;
        }
    }
}