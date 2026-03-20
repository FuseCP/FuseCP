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

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
	[DbContext(typeof(SqliteDbContext))]
	[Migration("20260320130000_RemovedLegacyStorefrontArtifacts")]
	public class RemovedLegacyStorefrontArtifacts : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(LegacyStorefrontCleanupSql.BuildSqliteDropTablesScript());
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
		}
	}
}