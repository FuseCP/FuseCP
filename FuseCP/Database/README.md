# Database Scripts Folder

The database is now managed with EntityFramework Core migrations. The primary source of truth is in `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Entities`, `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Configuration`, and `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Migrations`.

The `install.*.sql` files in this folder are generated artifacts copied from the EF migration project. Treat them as installer output for fresh installs, not as the place to author schema or seed-data changes.

Upgrade rules:

- `update_db.sql` is a legacy bridge for upgrading older installations to the v2.0.0 migration baseline. Do not use it as the normal post-v2.0.0 upgrade mechanism.
- `Migrate_msSQL.sql` is a legacy SQL Server upgrade/helper script for supported cleanup and migration scenarios.
- SQL Server upgrades from v2 onwards follow the EF migration chain and generated SQL.
- SQLite upgrades do not use `install.sqlite.sql`; they must run through EF migration execution on .NET 10.

If you need the old scripts, the legacy SQL scripts are located in `FuseCP/Sources/FuseCP.EnterpriseServer.Data/LegacyScripts`.

Here is a [doumentation video](https://www.youtube.com/watch?v=4994pCWvb-M) on YouTube.
