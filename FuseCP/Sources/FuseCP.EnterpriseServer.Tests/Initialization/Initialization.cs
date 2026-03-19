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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer.Data;

namespace FuseCP.Tests
{
	[TestClass]
	public class Initialization
	{

		[AssemblyInitialize]
		public static void Init(TestContext context)
		{
			// Init the assembly loader
			EnterpriseServer.InitAssemblyLoader();

			// Init all db flavors DbConfiguration
#if NETFRAMEWORK
			DbConfiguration.InitAllDatabaseProviders();
#else
			FuseCP.Web.Services.Configuration.CryptoKey = "1234567890";
			FuseCP.Web.Services.Configuration.EncryptionEnabled = true;
#endif
			// install the certificate
			Certificate.Install();
			Certificate.TrustAll();
			
			// Setup EnterpriseServer website & database
			EnterpriseServer.Clone();
			var sqliteConnectionString = EnterpriseServer.SetupSqliteDb();
			if (EnterpriseServer.TrySetupLocalDb(out var connStr, out var localDbError))
			{
				EnterpriseServer.ConfigureDatabase(connStr);
			}
			else
			{
				Console.WriteLine($"Skipping LocalDB-backed EnterpriseServer tests: {localDbError.Message}");
				EnterpriseServer.ConfigureDatabase(sqliteConnectionString);
			}

			Servers.Init(Component.EnterpriseServer);
		}

		[AssemblyCleanup]
		public static void Cleanup()
		{
			// remove the certificate
			Certificate.Remove();

			Servers.StopAll();

			EnterpriseServer.DeleteDatabases();
			EnterpriseServer.Delete();
		}
	}
}
