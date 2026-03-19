## Database

- Commit: f0186ef22
- Message: fix(sqlite): align migration snapshot and runtime migrate path
- Impact:
	- Repairs the SQLite `AddBruteForceProtection` migration designer snapshot so EF no longer reports pending model changes.
	- Adds the active SQLite model snapshot file and excludes the stale historical snapshot from compilation.
	- Aligns runtime SQLite migration paths to use the seeded model and native SQLite connection string in both tests and `DatabaseUtils.UpdateDatabase`.
- Validation:
	- `dotnet test "FuseCP/Sources/FuseCP.EnterpriseServer.Tests/FuseCP.EnterpriseServer.Tests.csproj" -c Release --framework net10.0 --filter "FullyQualifiedName~FuseCP.Tests.Database"`
	- `dotnet ef migrations has-pending-model-changes --configuration Release --framework net10.0 --context SqliteDbContext -- "DbType=Sqlite;Data Source=tmp.sqlite"`
