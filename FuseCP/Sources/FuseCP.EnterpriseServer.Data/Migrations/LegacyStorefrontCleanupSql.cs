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

using System.Text;

namespace FuseCP.EnterpriseServer.Data.Migrations
{
	internal static class LegacyStorefrontCleanupSql
	{
		static readonly string[] LegacyStorefrontTables =
		{
			"ecTopLevelDomainsCycles",
			"ecTopLevelDomains",
			"ecTaxations",
			"ecSystemTriggers",
			"ecSvcsUsageLog",
			"ecSupportedPlugins",
			"ecSupportedPluginLog",
			"ecStoreSettings",
			"ecStoreDefaultSettings",
			"ecServiceHandlersResponses",
			"ecService",
			"ecProductTypeControls",
			"ecProductType",
			"ecProductsHighlights",
			"ecProductCategories",
			"ecProduct",
			"ecPluginsProperties",
			"ecPaymentProfiles",
			"ecPaymentMethods",
			"ecInvoiceItems",
			"ecInvoice",
			"ecHostingPlansBillingCycles",
			"ecHostingPlans",
			"ecHostingPackageSvcsCycles",
			"ecHostingPackageSvcs",
			"ecHostingAddonSvcsCycles",
			"ecHostingAddonSvcs",
			"ecHostingAddonsCycles",
			"ecHostingAddons",
			"ecDomainSvcsCycles",
			"ecDomainSvcs",
			"ecCustomersPayments",
			"ecContracts",
			"ecCategory",
			"ecBillingCycles",
			"ecAddonProducts"
		};

		public static string BuildSqliteDropTablesScript()
		{
			var builder = new StringBuilder();
			builder.AppendLine("PRAGMA foreign_keys = OFF;");

			foreach (string tableName in LegacyStorefrontTables)
				builder.Append("DROP TABLE IF EXISTS \"").Append(tableName).AppendLine("\";");

			builder.AppendLine("PRAGMA foreign_keys = ON;");
			return builder.ToString();
		}

		public static string BuildMySqlDropTablesScript()
		{
			var builder = new StringBuilder();
			builder.AppendLine("SET FOREIGN_KEY_CHECKS = 0;");

			foreach (string tableName in LegacyStorefrontTables)
				builder.Append("DROP TABLE IF EXISTS `").Append(tableName).AppendLine("`;");

			builder.AppendLine("SET FOREIGN_KEY_CHECKS = 1;");
			return builder.ToString();
		}

		public static string BuildPostgreSqlDropTablesScript()
		{
			var builder = new StringBuilder();

			foreach (string tableName in LegacyStorefrontTables)
				builder.Append("DROP TABLE IF EXISTS public.\"").Append(tableName).AppendLine("\" CASCADE;");

			return builder.ToString();
		}
	}
}