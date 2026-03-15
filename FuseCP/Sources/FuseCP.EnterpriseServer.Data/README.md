# FuseCP.EnterpriseServer.Data
This project contains the EntityFramework database layer of FuseCP.EnterpriseServer. It uses EntityFramework Core 8
when running in .NET 10 or EntityFramework 6 when running in .NET Framework.

# Programmer's Introduction Video
Here's a [programmer's introduction video of the FuseCP EntityFramework port](https://youtu.be/fRJz-iDz4_s).

# Database Support
This implementation supports the following database flavors:

- Microsoft SQL Server
- MySQL
- MariaDB
- SQLite

PostgreSQL support is implemented, but is limited to .NET 10, because the EntityFramework 6 PostgreSQL driver for
.NET Framework is buggy and not maintained anymore. But PostgreSQL should work when running EnterpriseServer
on .NET 10 using EntityFramework Core.
Probably one could also support Oracle, adding support for it should be rather trivial, but we have chosen not to
support it at the moment.
Even we support SQLite, we do not recommend using SQLite on a production server.

# Folder Structure
The entity classes for the entities are contained in the Entities folder. The entity model Fluent API configuration classes are
contained in the Configuration folder. Migrations are contained in the Migrations folder and in the subfolder
corresponding to the database flavor. The folder CodeTemplates contains the T4 templates used for database scaffolding.
The Extensions folder contains extension classes.

The `Configuration` folder is also the source of truth for EF FluentAPI relationship mapping and seeded data (`HasData`).
If seeded providers, quotas, resource groups, schedule task parameters, or other bootstrap rows change, update the
Configuration classes and regenerate migrations/scripts instead of hand-editing generated `install.*.sql` output.

# Scaffolding of the Database
The folder CodeTemplates contains the T4 templates used by the "dotnet ef dbcontext scaffold" command to create the
entity, dbcontext and configuration classes from an existing SQL Server database. You can run the scaffolding of the
database by executing Scaffold.bat. The connection string for scaffolding must be set in the
FuseCP.EnterpriseServer.Data.csproj in the ScaffoldConnectionString property. Be sure to run the scaffolding against a
fresh database, since it will also scaffold seed data.
You can also apply changes to the database model done through the old way with update_db.sql, in that you apply
update_db.sql to the database, and then scaffold the database. You will see all the changes to the model in the
Entities\Sources\.., Configuraton\Sources\.. and DbContextBase.Source.cs files. You can then use the diff viewer
to track all changes in those classes and can then apply the changes to the classes in the Entities and Configuration
folders and to DbContextBase.cs.

This raw-SQL path is legacy compatibility guidance, not the primary workflow for current releases. `update_db.sql`
is kept for older upgrade scenarios up to the v2.0.0 migration boundary. Post-v2.0.0 schema evolution should be done
through Entity/Configuration changes plus EF migrations.
For portability accross different database flavors, you'll have to tweak the classes in the Entities\Sources folder a
bit. In particular, you'll have to comment out all manual assignmens of a database type by the
[Column(TypeName="...")]. If you want to assign a specific database type, you'll have to do it in the Configuration
classes through the Fluent API, and you need to distinguish the different database flavors, so for a
```
[Colum(TypeName="ntext")]
public string ExecutionLog { get; set; }
```
for example you would write in the Fluent API:
```
if (IsSqlServer) {
    Property(e => e.ExecutionLog).HasColumnType("ntext");
} else if (IsCore && (IsMySql || IsMariaDb || IsSqlite || IsPostgreSql)) {
    Property(e => e.ExecutionLog).HasColumnType("TEXT");
}
```
and comment out the Column attribute like so:
```
// [Colum(TypeName="ntext")]
public string ExecutionLog { get; set; }
```

In the above code, the `HasColumnType("TEXT")` command is only run when running on .NET 10, by using `if (IsCore && ...)`
because some EntityFramework 6 drivers cannot handle custom column types, so `HasColumnType` is only called when using
EF Core. As migrations are always handled by EF Core, the migration creates the correct column type also for EntityFramework 6.

# Migrations
Migrations are only managed with EF Core, not with EF 6. They are always executed with .NET 10 or with SQL scripts by the
installer, not with .NET Framework.

`MigrationAdd.bat` generates both migration classes and the `install.*.sql` scripts copied into `FuseCP/Database/`.
Treat those `install.*.sql` files as generated artifacts for fresh installs. For SQL Server, the installer can use the
generated install script for fresh installs and migration-chain-based upgrades from v2 onwards. For SQLite, upgrades do
not go through `install.sqlite.sql`; they must run through EF migration execution on .NET 10.

`update_db.sql` must not be used as the normal post-v2.0.0 migration mechanism. It remains a legacy bridge for older
databases that must first be brought forward to the v2.0.0 migration baseline. `Migrate_msSQL.sql` is likewise a legacy
upgrade/helper script used for supported SQL Server cleanup/upgrade flows, not the primary source of schema truth.

`LegacyScripts/master.update_db.sql` is an archival baseline copy of the original 1.5.1 update script. Do not modify it.
If upgrade behavior needs to be fixed, implement the fix through Entity/Configuration + EF migrations first, and only then
update supported legacy upgrade scripts (`Database/update_db.sql`, `LegacyScripts/update_db.sql`,
`Migrations/SqlServer/v1.5.1/update_db.sql`) when the old upgrade path itself must be repaired.

To create a new migration, you can run MigrationAdd.bat, or if you just want a migration for the database flavor
you're developing with, copy the individual lines in AddMigration.bat to a command line shell.

When you create FuseCP release with deploy-release.bat, deploy-release.bat creates backups of the Model Snapshots
..DbContextModelSnapshot.cs files, so you can always create migrations based on the last FuseCP release. When
creating a FuseCP release, one can combine all new migrations into one by using the DbContext in the snapshot
backup .cs file for the `dotnet ef migrations add` command or by reverting the Model Snapshot to that of the last
release and creating a new migration. During development, when you have to change the database model often
you might want to revert the last migration and then calculate a new migration with the MigrationRemove.bat
script or the command `dotnet ef migrations remove`.

Backup snapshot files (for example `*DbContextModelSnapshot_*` backups used for release/migration workflows) are
reference points and should not be hand-edited. Let EF tooling regenerate active snapshots; use backup snapshots only for
revert/rebase migration workflows.

# Usage of FuseCP.EnterpriseServer.Data
FuseCP.EnterpriseServer.Data provides a class DbContext, that can be used as EF DbContext to access the database,
It has properties to access the DbSet's of the Entities, and the usual SaveChanges etc. commands. In order to
consume SoldiCP.EnterpriseServer.Data, you don't have to import the assemblies for EF Core 8 or EF 6, just use the
FuseCP.EnterpriseServer.Data.DbContext class. It will use either EF Core 8 or EF 6 for accessing the database
depending on wether you run on NET 10 or on NET Framework.

# Connection Strings
FuseCP.EnterpriseServer.Data uses an additional setting in the connection strings, `DbType`. You can set the
`DbType` token in the connection string to either `DbType=SqlServer`, `DbType=MySql`, `DbType=MariaDb`,
`DbType=Sqlite` or `DbType=PostgreSql`. You don't have to specify a providerName with your connection string
as FuseCP will determine the correct database type according to the `DbType` token in your connection string
automatically. So for example correct connection strings would be:

- SQL Server: `DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;uid=sa;pwd=Password12`
- MySQL: `DbType=MySql;server=localhost;port=3306;database=FuseCP;uid=root;password=Password12`
- MariaDB: `DbType=MariaDb;server=localhost;port=3306;database=FuseCP;uid=root;password=Password12`
- SQLite: `DbType=Sqlite;data source=App_Data\FuseCP.sqlite`
