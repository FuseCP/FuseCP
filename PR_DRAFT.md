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

## Tests

- Commit: 1d5f88f11
- Message: fix(tests): degrade gracefully when LocalDB unavailable on CI runner
- Impact:
	- Wraps `SetupLocalDb()` in `TrySetupLocalDb()` with fallback to SQLite when infrastructure unavailable.
	- Adds `IsLocalDbAvailable` probe and `RequireSqlServer()` guards in test data rows.
	- Allows 30 SQLite tests to run green on CI runners where LocalDB is not present; SqlServer rows report `Inconclusive`.
	- Root cause: `EnterpriseServer.Tests` was added to CI in PR #7 (.NET 10 upgrade) but hardcoded LocalDB dependency; when LocalDB fails on runner, entire assembly marked failed.
- Validation:
	- Local: 131/131 tests pass with LocalDB present
	- CI: ~30 SQLite tests expected to pass, ~4 SqlServer rows Inconclusive (not Failed)

- Commit: 66052758a
- Message: fix(db): regenerate complete SqlServer migration Designer snapshot
- Impact:
	- Regenerates the broken SqlServer `AddBruteForceProtection.Designer.cs` which had an empty `BuildTargetModel` method (38 lines vs 920KB).
	- This incomplete snapshot caused `dotnet ef database update` to fail with "pending model changes" error.
	- Moves SqlServer migrations to root `Migrations\SqlServer\` for consistency.
- Validation:
	- `dotnet build --framework net10.0 FuseCP/Sources/FuseCP.EnterpriseServer.Data`
	- `dotnet ef migrations has-pending-model-changes --framework net10.0 --context SqlServerDbContext` returns no pending changes
	- `sqlcmd` idempotent install script applies migration to dev environment

## CI

- Commit: ca6be8ef5
- Message: fix(ci): build release configuration before no-build test stage
- Impact:
	- Aligns Build Server & EnterpriseServer step with the Release matrix configuration used by Run Unit Tests.
	- Prevents stale/missing Release test binaries when running `dotnet test --no-build --configuration Release FuseCP.Tests.sln`.
	- Removes a false check failure path that could mask the actual SQLite migration status.
- Validation:
	- `dotnet build --configuration Release FuseCP.Tests.sln`
	- `dotnet test --no-build -v n --logger trx -m:1 -p:BuildInParallel=false --configuration Release --results-directory ..\..\test-results FuseCP.Tests.sln`
