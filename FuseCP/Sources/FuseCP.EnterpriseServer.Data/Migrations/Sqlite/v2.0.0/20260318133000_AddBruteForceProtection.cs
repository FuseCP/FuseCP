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
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddBruteForceProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BruteForceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Layer = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Succeeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BruteForceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IpSecurityPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IpRange = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsWhitelist = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeverityLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpSecurityPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BruteForceLogs_IpAddress_Layer_AttemptTime",
                table: "BruteForceLogs",
                columns: new[] { "IpAddress", "Layer", "AttemptTime" });

            migrationBuilder.CreateIndex(
                name: "IX_IpSecurityPolicies_IpRange_IsActive",
                table: "IpSecurityPolicies",
                columns: new[] { "IpRange", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BruteForceLogs");
            migrationBuilder.DropTable(name: "IpSecurityPolicies");
        }
    }
}
