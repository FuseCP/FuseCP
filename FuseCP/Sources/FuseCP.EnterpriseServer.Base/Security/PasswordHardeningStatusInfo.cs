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
    public class PasswordHardeningStatusInfo
    {
        public DateTime GeneratedUtc { get; set; }
        public bool SetupModeEnabled { get; set; }

        public int TotalUserCount { get; set; }
        public int HardenedUserCount { get; set; }
        public int LegacyUserCount { get; set; }
        public int EmptyUserPasswordCount { get; set; }
        public int AutoHardenEligibleUserCount { get; set; }

        public bool PortalPasswordResetEnabled { get; set; }
        public string PortalPasswordResetLinkLifeSpan { get; set; }
        public bool MfaDisplayNameConfigured { get; set; }
        public bool CanPeerChangeMfa { get; set; }

        public int BruteForceMaxFailedAttempts { get; set; }
        public int BruteForceWindowMinutes { get; set; }
        public int BruteForceLockoutMinutes { get; set; }
        public int BruteForceCriticalAttemptThreshold { get; set; }

        public bool UserLazyMigrationEnabled { get; set; }
        public bool ApiBruteForceProtectionEnabled { get; set; }
        public bool ServerRequestAuthenticationEnabled { get; set; }

        public int TotalServerCount { get; set; }
        public int HmacCapableServerCount { get; set; }
        public int Sha256ServerCount { get; set; }
        public int LegacyServerCount { get; set; }
        public int ServerProbeFailureCount { get; set; }

        public ServerPasswordHardeningStatusInfo[] Servers { get; set; }
    }
}