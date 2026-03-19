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
using System.Reflection;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
#if NETFRAMEWORK
using System.Data.Entity;
#else
using Microsoft.EntityFrameworkCore;
#endif
using FuseCP.EnterpriseServer.Code;
using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Data;


namespace FuseCP.Tests
{
	[TestClass]
	public class Database
	{
		static void RequireSqlServer(DbType dbtype)
		{
			if (dbtype == DbType.SqlServer && !EnterpriseServer.IsLocalDbAvailable)
			{
				Assert.Inconclusive($"SqlServer test requires SqlLocalDB, which is unavailable: {EnterpriseServer.LocalDbUnavailableReason}");
			}
		}

		public string ConnectionString(DbType dbtype) => EnterpriseServer.ConnectionString(dbtype);

		[TestMethod]
		[DataRow(DbType.SqlServer)]
		[DataRow(DbType.Sqlite)]
		public void TestDbAccess(DbType dbtype)
		{
			RequireSqlServer(dbtype);
			using (var db = new FuseCP.EnterpriseServer.Data.DbContext(ConnectionString(dbtype)))
			{
				var providers = db.Providers.ToArray();
				Assert.IsNotEmpty(providers);
			}
		}

		[TestMethod]
		[DataRow(DbType.SqlServer)]
		[DataRow(DbType.Sqlite)]
		public void TestDynamicLike(DbType dbtype)
		{
			RequireSqlServer(dbtype);
			using (var db = new FuseCP.EnterpriseServer.Data.DbContext(ConnectionString(dbtype)))
			{
				var columnName = "ProviderName";
				var columnValue = "%S";
#if NETFRAMEWORK
				var providersStatic = db.Providers.Where(p => DbFunctions.Like(p.ProviderName, columnValue));
#else
				var providersStatic = db.Providers.Where(p => EF.Functions.Like(p.ProviderName, columnValue));
#endif
				var config = new ParsingConfig { ResolveTypesBySimpleName = true };
				var providersDynamic = db.Providers.Where(DynamicFunctions.ColumnLike(db.Providers, columnName, columnValue));
				var nstatic = providersStatic.Count();
				var ndynamic = providersDynamic.Count();
				Assert.AreEqual(nstatic, ndynamic);
			}
		}

		[TestMethod]
		[DataRow(DbType.SqlServer)]
		[DataRow(DbType.Sqlite)]
		public void TestGetSearchObjectEF(DbType dbtype)
		{
			RequireSqlServer(dbtype);
			using (var db = new DataProvider(ConnectionString(dbtype)))
			{
				db.AlwaysUseEntityFramework = true;
				var set = db.GetSearchObject(1, 1, null, "%test%", 0, 0, "", 0, 15, null, null, false, true);
			}
		}

		[TestMethod]
		[DataRow(DbType.SqlServer)]
		[DataRow(DbType.Sqlite)]
		public void TestGetSearchObjectStoredProcedure(DbType dbtype)
		{
			RequireSqlServer(dbtype);
			using (var db = new DataProvider(ConnectionString(dbtype)))
			{
				db.AlwaysUseEntityFramework = false;
				var set = db.GetSearchObject(1, 1, null, "%test%", 0, 0, "", 0, 15, null, null, false, true);
			}
		}

	}
}
