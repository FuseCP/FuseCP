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
using System.Linq;
using FuseCP.EnterpriseServer.Security;

namespace FuseCP.Tests
{
    /// <summary>
    /// Unit tests for <see cref="BruteForceProtectionService"/>.
    /// Tests that do not touch the database exercise the CIDR matching logic directly.
    /// Tests that require database access use the Sqlite in-process database set up
    /// by the assembly initializer.
    /// </summary>
    [TestClass]
    public class BruteForceProtectionTests
    {
        // ------------------------------------------------------------------ //
        // CIDR matching – pure logic, no database
        // ------------------------------------------------------------------ //

        [TestMethod]
        public void IsIpInCidr_ExactMatchWithin_ReturnsTrue()
        {
            Assert.IsTrue(BruteForceProtectionService.IsIpInCidr("192.168.1.1", "192.168.1.0/24"));
        }

        [TestMethod]
        public void IsIpInCidr_HighestAddressInSubnet_ReturnsTrue()
        {
            Assert.IsTrue(BruteForceProtectionService.IsIpInCidr("192.168.1.255", "192.168.1.0/24"));
        }

        [TestMethod]
        public void IsIpInCidr_AddressOutsideSubnet_ReturnsFalse()
        {
            Assert.IsFalse(BruteForceProtectionService.IsIpInCidr("192.168.2.1", "192.168.1.0/24"));
        }

        [TestMethod]
        public void IsIpInCidr_LoopbackInSlash8_ReturnsTrue()
        {
            Assert.IsTrue(BruteForceProtectionService.IsIpInCidr("10.99.1.1", "10.0.0.0/8"));
        }

        [TestMethod]
        public void IsIpInCidr_LoopbackOutsideSlash8_ReturnsFalse()
        {
            Assert.IsFalse(BruteForceProtectionService.IsIpInCidr("11.0.0.1", "10.0.0.0/8"));
        }

        [TestMethod]
        public void IsIpInCidr_ExactHostRoute_ReturnsTrue()
        {
            Assert.IsTrue(BruteForceProtectionService.IsIpInCidr("192.168.1.5", "192.168.1.5/32"));
        }

        [TestMethod]
        public void IsIpInCidr_DifferentHostInSlash32_ReturnsFalse()
        {
            Assert.IsFalse(BruteForceProtectionService.IsIpInCidr("192.168.1.6", "192.168.1.5/32"));
        }

        [TestMethod]
        public void IsIpInCidr_InvalidCidr_ReturnsFalse()
        {
            Assert.IsFalse(BruteForceProtectionService.IsIpInCidr("192.168.1.1", "not-a-cidr"));
        }

        [TestMethod]
        public void IsIpInCidr_EmptyIp_ReturnsFalse()
        {
            Assert.IsFalse(BruteForceProtectionService.IsIpInCidr("", "192.168.1.0/24"));
        }

        [TestMethod]
        public void IsIpInCidr_IPv6_LoopbackInSlash64_ReturnsTrue()
        {
            Assert.IsTrue(BruteForceProtectionService.IsIpInCidr(
                "2001:db8::1", "2001:db8::/32"));
        }

        // ------------------------------------------------------------------ //
        // Database-backed tests (use the Sqlite in-process db)
        // ------------------------------------------------------------------ //

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void RecordAttempt_SingleFailure_NotBlocked(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.1.1.{Random.Shared.Next(10, 200)}";

            var blocked = svc.RecordAttempt(ip, "testuser", BruteForceProtectionService.Layers.Portal,
                succeeded: false);

            Assert.IsFalse(blocked, "Single failure should not block the IP.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void RecordAttempt_SuccessAfterFailure_NotBlocked(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.2.1.{Random.Shared.Next(10, 200)}";

            svc.RecordAttempt(ip, "testuser", BruteForceProtectionService.Layers.Portal, succeeded: false);
            var blocked = svc.RecordAttempt(ip, "testuser", BruteForceProtectionService.Layers.Portal,
                succeeded: true);

            Assert.IsFalse(blocked, "Successful login should not result in a block.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void BlockIp_IsIpBlocked_ReturnsTrue(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.3.1.{Random.Shared.Next(10, 200)}";

            svc.BlockIp(ip, "manual block test", "unit-test");

            Assert.IsTrue(svc.IsIpBlocked(ip), "Manually blocked IP should be reported as blocked.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void UnblockIp_AfterBlock_IsNotBlocked(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.4.1.{Random.Shared.Next(10, 200)}";

            svc.BlockIp(ip, "to be unblocked", "unit-test");
            svc.UnblockIp(ip);

            Assert.IsFalse(svc.IsIpBlocked(ip), "Unblocked IP should no longer be reported as blocked.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void WhitelistIp_IsIpWhitelisted_ReturnsTrue(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.5.1.{Random.Shared.Next(10, 200)}";

            svc.WhitelistIp(ip, "trusted server", "unit-test");

            Assert.IsTrue(svc.IsIpWhitelisted(ip), "Whitelisted IP should be reported as whitelisted.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void WhitelistIp_BlockIsIgnored_NotBlocked(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.6.1.{Random.Shared.Next(10, 200)}";
            svc.WhitelistIp(ip, "trusted server", "unit-test");

            // Even manually blocking should be overridden by whitelist check.
            svc.BlockIp(ip, "should be ignored", "unit-test");

            Assert.IsFalse(svc.IsIpBlocked(ip), "Whitelisted IP must not be considered blocked.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void GetAttempts_ReturnsRecordedEntries(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.7.1.{Random.Shared.Next(10, 200)}";
            svc.RecordAttempt(ip, "getuser", BruteForceProtectionService.Layers.Api, succeeded: false);
            svc.RecordAttempt(ip, "getuser", BruteForceProtectionService.Layers.Api, succeeded: false);

            var attempts = svc.GetAttempts(ipAddress: ip, layer: BruteForceProtectionService.Layers.Api,
                failedOnly: true);

            Assert.IsGreaterThanOrEqualTo(attempts.Count, 2, "Should return at least the 2 recorded failed attempts.");
        }

        [TestMethod]
        [DataRow(FuseCP.EnterpriseServer.Data.DbType.Sqlite)]
        public void GetPolicies_BlacklistOnly_ReturnsBlacklistEntries(FuseCP.EnterpriseServer.Data.DbType dbtype)
        {
            var connStr = EnterpriseServer.ConnectionString(dbtype);
            using var db = new FuseCP.EnterpriseServer.DataProvider(connStr);
            var svc = new BruteForceProtectionService(new FuseCP.EnterpriseServer.ControllerBase(db));

            var ip = $"10.8.1.{Random.Shared.Next(10, 200)}";
            svc.BlockIp(ip, "policy test", "unit-test");

            var policies = svc.GetPolicies(blacklistOnly: true);

            Assert.IsTrue(policies.Any(p => p.IpRange == ip),
                "Blacklist-only query should return the newly blocked IP.");
        }
    }
}
