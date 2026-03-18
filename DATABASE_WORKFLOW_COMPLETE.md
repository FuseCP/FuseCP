# FuseCP Database Workflow - Complete Reference

**Status:** ✅ Fully automated, zero manual launches required

This document covers the complete FuseCP database architecture, EF workflow, and automated verification system.

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Automation Overview](#automation-overview)
3. [Workflow Scenarios](#workflow-scenarios)
4. [Developer Guide](#developer-guide)
5. [Troubleshooting](#troubleshooting)

---

## Architecture Overview

### The EF Stack

FuseCP uses **Entity Framework as the source-of-truth** for database schema:
- **EF Core 8** (.NET 10) – Cloud/containerized deployments
- **EF 6** (.NET Framework 4.8) – Legacy on-premises installations

**Key principle:** Migrations are managed by EF Core; migration SQL is used by both EF Core and EF 6.

### Directory Structure

```
FuseCP/Sources/FuseCP.EnterpriseServer.Data/
├── Entities/                          # ~95 entity classes (source-of-truth)
├── Configuration/                    # ~95 Fluent API configurations
├── Migrations/                        # Provider-specific migration chains
│   ├── SqlServer/
│   ├── MySql/
│   ├── PostgreSql/
│   └── Sqlite/
├── DbContextBase.cs                  # OnModelCreating() wiring point
└── DbContext.cs                      # Provider routing & connection management

FuseCP/Database/                       # **GENERATED ARTIFACTS (not source-of-truth)**
├── install.sqlserver.sql             # Fresh install for SQL Server
├── install.mysql.sql                 # Fresh install for MySQL
├── install.postgresql.sql            # Fresh install for PostgreSQL
├── install.sqlite.sql                # Fresh install for SQLite
└── update_db.sql / Migrate_msSQL.sql # LEGACY - pre-v2.0.0 upgrades only
```

### Invariants (Automatically Enforced)

1. **Entity/Configuration Alignment**: Number of `Entities/*.cs` = Number of `Configuration/*.cs`
2. **Configuration Registration**: All configs wired in `DbContextBase.OnModelCreating()`
3. **Migration Synchronization**: All 4 providers have matching migration files
4. **Install Script Currency**: All `install.*.sql` contain latest schema
5. **Test Bootstrap**: SQLite setup calls `InstallFreshDatabase()` + `Migrate()`
6. **EF Workflow Guards**: No stray duplicate source files, all DbSet properties defined

---

## Automation Overview

### Where & When Automation Runs

| Level | Trigger | Mode | Frequency | Blocks? |
|-------|---------|------|-----------|---------|
| **CI Pipeline** | Every PR/push | Full | On every commit | ✅ Yes |
| **Local Builds** | Before build | Full | Per `run-local-validation.ps1` | ⚠️ Warns |
| **Pre-Commit** | Git commit | Quick | When hook installed | ✅ Yes |
| **Daily Routine** | Manual launch | Quick | Start-Of-Day.ps1 | ❌ No |

### Orchestrator Modes

Single entry point: `FuseCP/Tools/Orchestrate-Database-Workflow.ps1`

```powershell
# Quick: Fast pre-commit checks (<1 second)
pwsh -File Orchestrate-Database-Workflow.ps1 -Mode Quick

# Full: Complete automation with regeneration (~10 seconds) [DEFAULT]
pwsh -File Orchestrate-Database-Workflow.ps1 -Mode Full

# Verify: Comprehensive 8-phase audit (~15 seconds)
pwsh -File Orchestrate-Database-Workflow.ps1 -Mode Verify

# Fix: Force MySQL artifact regeneration (~5 seconds)
pwsh -File Orchestrate-Database-Workflow.ps1 -Mode Fix -Force

# Report: JSON output for CI integration
pwsh -File Orchestrate-Database-Workflow.ps1 -Mode Report -JsonOutputPath report.json
```

### What Gets Checked

✅ **Entity/Configuration Alignment**
- Drift detection: counts must match (95 = 95)
- Catches: new entity without configuration
- Time: <100ms

✅ **Configuration Registration**
- Verifies: all configs wired in DbContextBase.OnModelCreating()
- Catches: forgotten ApplyConfiguration() call
- Time: <100ms

✅ **Migration Files**
- Verifies: all 4 providers have matching migration files
- Catches: migrations pushed for some providers but not others
- Time: <100ms

✅ **Install Script Currency**
- Verifies: all install.*.sql contain latest tables (BruteForceLogs, IpSecurityPolicies)
- Auto-regenerates: MySQL from PostgreSQL template if stale
- Catches: stale install scripts preventing fresh deployments
- Time: <1sec (Quick/Full) or 5-10sec (Full with regen)

✅ **Test Bootstrap**
- Verifies: SQLite test setup calls InstallFreshDatabase() + Migrate()
- Catches: unit test database bootstrap failures
- Time: <100ms

✅ **EF Workflow Guards**
- Verifies: no stray duplicate source files
- Verifies: all DbSet properties defined
- Verifies: all required methods in test bootstrap

### Auto-Fix Capability

**MySQL Artifact Regeneration** (Automatic in Full mode):
- Detects: if PostgreSQL template is newer than MySQL install script
- Regenerates: MySQL from PostgreSQL template automatically
- Prevents: stale artifacts causing deployment failures
- User action: None required

---

## Workflow Scenarios

### Scenario 1: Fresh Installation (Green-Field Deployment)

**Trigger**: New customer, new database, no prior FuseCP data

**Implementation**:
```powershell
# Select appropriate install.*.sql for engine
sqlcmd -S (server) -U user -P pass -i FuseCP\Database\install.sqlserver.sql
```

**Automated checks**:
- ✅ CI ensures all install scripts are current before PR merges
- ✅ Local validation regenerates MySQL if PostgreSQL template newer
- ✅ Test bootstrap validates SQLite schema installation

---

### Scenario 2: EF Migrations (Schema Changes)

**Trigger**: Developer adds new entity and configuration

**Implementation**:

1. **Code changes**:
   ```
   FuseCP/Sources/FuseCP.EnterpriseServer.Data/
   ├── Entities/MyEntity.cs                    [NEW]
   ├── Configuration/MyEntityConfiguration.cs  [NEW]
   ```

2. **Wire in DbContext**:
   ```csharp
   // DbContextBase.cs
   modelBuilder.ApplyConfiguration(new MyEntityConfiguration());
   
   // DbContext.Sets.cs
   public DbSet<MyEntity> MyEntities => Set<MyEntity>();
   ```

3. **Generate migrations**:
   ```powershell
   cd FuseCP\Sources\FuseCP.EnterpriseServer.Data
   .\MigrationAdd.bat          # Generates for all 4 providers
   ```

4. **Commit & Push**:
   ```powershell
   git add .
   git commit -m "Add MyEntity and configuration"
   git push
   ```

**Automated checks**:
- ✅ Quick check (pre-commit): Entity/config counts match + all configs registered
- ✅ Full check (local build): All providers have migrations + install scripts current
- ✅ Full build (CI): Comprehensive 8-phase verification

---

### Scenario 3: Incremental Updates (Compatibility Layer)

**Trigger**: Maintaining compatibility with older client versions

**Implementation**: `update_db.sql` (for databases pre-v2.0.0 only)

**Note**: Modern updates use EF migrations instead of manual SQL.

---

### Scenario 4: Multi-Provider Support

**How it works**:
- One set of entities and configurations
- Four provider-specific migration chains (SqlServer, MySql, PostgreSql, Sqlite)
- Fresh install scripts generated from migrations
- All synchronized automatically

**Provider-specific handling**:

| Provider | Type System | Install Location |
|----------|------------|------------------|
| SQL Server | `datetime`, `nvarchar`, `bit` | `install.sqlserver.sql` |
| MySQL | `TIMESTAMP`, `varchar`, `tinyint(1)` | `install.mysql.sql` (auto-generated) |
| PostgreSQL | `timestamp`, `character varying`, `boolean` | `install.postgresql.sql` |
| SQLite | `TEXT` for dates, minimal constraints | `install.sqlite.sql` |

**CI ensures**: All 4 stay synchronized, no divergences.

---

## Developer Guide

### Task 1: Add a New Entity

**You do**:
1. Create `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Entities/MyEntity.cs`
2. Create `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Configuration/MyEntityConfiguration.cs`
3. Add `modelBuilder.ApplyConfiguration(new MyEntityConfiguration());` in DbContextBase.OnModelCreating()
4. Add `public DbSet<MyEntity> MyEntities => Set<MyEntity>();` in DbContext.Sets.cs
5. Run `MigrationAdd.bat` in `FuseCP/Sources/FuseCP.EnterpriseServer.Data/`
6. Commit & push

**Automation does**:
- 🤖 Pre-commit: Verifies entity/config counts match, all configs registered
- 🤖 Local build: Verifies all 4 providers have matching migrations
- 🤖 CI/PR: Full 8-phase verification before merge

**If you forget a step**:
- Forgot config file? → Quick check fails, pre-commit blocks commit
- Forgot ApplyConfiguration()? → Quick check fails in CI, PR blocked
- Forgot MigrationAdd.bat? → Migration files missing, CI blocks PR

---

### Task 2: Running Verification Manually

**Quick check** (pre-commit style):
```powershell
pwsh -File FuseCP\Tools\Orchestrate-Database-Workflow.ps1 -Mode Quick
```
Output: <1 second, 3 quick tests

**Full verification** (what CI runs):
```powershell
pwsh -File FuseCP\Tools\Orchestrate-Database-Workflow.ps1 -Mode Full
```
Output: ~10 seconds, all checks + auto-regen MySQL

**Comprehensive audit** (detailed debugging):
```powershell
pwsh -File FuseCP\Tools\Orchestrate-Database-Workflow.ps1 -Mode Verify
```
Output: ~15 seconds, 8-phase detailed verification

**JSON report** (for CI integration):
```powershell
pwsh -File FuseCP\Tools\Orchestrate-Database-Workflow.ps1 -Mode Report -JsonOutputPath report.json
```
Output: Structured data for downstream processing

---

### Task 3: Fixing MySQL Generated Script

**If MySQL install.sql is stale** (PostgreSQL template has new migrations):

Automatic: ✅ `run-local-validation.ps1` regenerates before every build

Manual (if needed):
```powershell
pwsh -File FuseCP\Tools\Orchestrate-Database-Workflow.ps1 -Mode Fix -Force
```

**This**:
1. Detects PostgreSQL is newer
2. Reads PostgreSQL install.mysql.sql
3. Transforms PostgreSQL syntax → MySQL syntax
4. Writes to `FuseCP\Database\install.mysql.sql`
5. Verifies result contains latest tables
6. If all checks pass, no action needed in commit

---

## Troubleshooting

### Error: "Entity/Configuration mismatch: 96 entities vs 95 configs"

**Cause**: Created new entity without configuration

**Fix**:
```powershell
# 1. Create the missing configuration
# FuseCP/Sources/FuseCP.EnterpriseServer.Data/Configuration/MyEntityConfiguration.cs

# 2. Wire it in DbContextBase
# modelBuilder.ApplyConfiguration(new MyEntityConfiguration());

# 3. Run migrations
cd FuseCP\Sources\FuseCP.EnterpriseServer.Data
.\MigrationAdd.bat

# 4. Re-run verification
pwsh -File ..\..\..\Tools\Orchestrate-Database-Workflow.ps1 -Mode Quick
```

---

### Error: "Not all configurations registered: 94 applied vs 95 defined"

**Cause**: Created configuration but didn't wire it in DbContextBase.OnModelCreating()

**Fix**:
```csharp
// DbContextBase.cs, in OnModelCreating method
modelBuilder.ApplyConfiguration(new MyMissingConfiguration());
```

Then commit and verify.

---

### Error: "Missing migrations for providers: MySql"

**Cause**: Ran `MigrationAdd.bat` but didn't include MySQL

**Fix**:
```powershell
# MigrationAdd.bat should generate for all 4 providers
# If it fails, check:
cd FuseCP\Sources\FuseCP.EnterpriseServer.Data
.\MigrationAdd.bat

# Verify all 4 providers present:
dir Migrations\SqlServer\v2.0.0\
dir Migrations\MySql\v2.0.0\
dir Migrations\PostgreSql\v2.0.0\
dir Migrations\Sqlite\v2.0.0\
```

---

### Error: "install.mysql.sql MISSING security tables"

**Cause**: MySQL artifact not regenerated after migrations added

**Fix** (automatic):
```powershell
# Local build automatically regenerates if stale
.\FuseCP\Tools\run-local-validation.ps1
```

**Fix** (manual):
```powershell
pwsh -File FuseCP\Tools\Orchestrate-Database-Workflow.ps1 -Mode Fix -Force
```

---

### Error: "Test bootstrap missing required components"

**Cause**: Test helper file missing methods or broken

**Fix**:
```csharp
// FuseCP/Sources/FuseCP.EnterpriseServer.Tests/Initialization/EnterpriseServer.cs
// Ensure it has:
// 1. SetupSqliteDb() method
// 2. Calls to InstallFreshDatabase()
// 3. Calls to context.Database.Migrate()
```

---

## Integration Points

### CI Pipeline (.github/workflows/test-release.yaml)

```yaml
- name: Automated Database Verification
  run: |
    pwsh -NoProfile -File .\FuseCP\Tools\Orchestrate-Database-Workflow.ps1 `
      -Mode Full `
      -BlockBuildOnFailure `
      -JsonOutputPath db-workflow-report.json
```

**Behavior**:
- Runs on every PR to main/release
- Full mode: all checks + MySQL regen
- Blocks PR if verification fails
- Uploads JSON report as artifact (7-day retention)

---

### Local Validation (FuseCP/Tools/run-local-validation.ps1)

```powershell
# Integrated automatically, no user action
Invoke-Step "Database workflow automation" {
    Orchestrate-Database-Workflow.ps1 -Mode Full -ContinueOnError
}
```

**Behavior**:
- Runs before every build
- Full mode: all checks + MySQL regen
- Warns but doesn't block (allows dev to iterate)
- Auto-regenerates MySQL if stale

---

### Pre-Commit Hook (Optional, .git/hooks/pre-commit)

**Setup**:
```powershell
pwsh -File FuseCP\Tools\Install-Git-Hooks.ps1
```

**Behavior**:
- Runs on every local commit
- Quick mode: drift, registration, migrations only
- Blocks on failure (bypass with `--no-verify`)
- Takes <1 second

**Uninstall**:
```powershell
pwsh -File FuseCP\Tools\Install-Git-Hooks.ps1 -Uninstall
```

---

### Daily Routine (FuseCP/Tools/Start-Of-Day.ps1)

**Optional manual check**:
```powershell
# Runs orchestrator in Quick mode
.\FuseCP\Tools\Start-Of-Day.ps1
```

Takes <5 seconds, optional but recommended before starting dev session.

---

## Compliance Guarantees

| Invariant | Enforced At | Blocks? | Auto-Fixed? |
|-----------|------------|---------|------------|
| Entity/Config mismatch | Pre-commit, CI | ✅ Yes | ❌ Manual |
| Config not registered | Pre-commit, CI | ✅ Yes | ❌ Manual |
| Migration missing | Pre-commit, CI | ✅ Yes | ❌ Manual |
| Install script stale | Local build, CI | ⚠️ Local: warns, CI: fixes | ✅ MySQL auto |
| Test bootstrap broken | CI | ✅ Yes | ❌ Manual |
| Stray duplicate files | CI | ✅ Yes | ❌ Manual |

---

## What You Don't Do Anymore

❌ Manually run `Verify-Database-Workflow.ps1` (removed, functions merged)  
❌ Manually run `Regenerate-MySQL-Artifacts.ps1` (automatic in Full mode)  
❌ Manually run `check-ef-workflow-guards.ps1` (removed, integrated)  
❌ Remember to check if MySQL is stale (automatic detection)  
❌ Hope CI catches schema violations (deterministic enforcement)  
❌ Receive reminders about database workflow (automatic blocking)

---

## Quick Reference

| Need | Command |
|------|---------|
| Quick sanity check | `Orchestrate-Database-Workflow.ps1 -Mode Quick` |
| Full verification | `Orchestrate-Database-Workflow.ps1 -Mode Full` |
| Detailed audit | `Orchestrate-Database-Workflow.ps1 -Mode Verify` |
| Fix MySQL artifact | `Orchestrate-Database-Workflow.ps1 -Mode Fix -Force` |
| JSON report | `Orchestrate-Database-Workflow.ps1 -Mode Report -JsonOutputPath out.json` |
| Install pre-commit hook | `Install-Git-Hooks.ps1` |
| Remove pre-commit hook | `Install-Git-Hooks.ps1 -Uninstall` |
| Daily routine check | `Start-Of-Day.ps1` |

---

## Files Structure

```
FuseCP/Tools/
├── Orchestrate-Database-Workflow.ps1   # Single consolidated entry point
├── Install-Git-Hooks.ps1               # Optional pre-commit setup
├── Start-Of-Day.ps1                    # Optional daily check
└── [Other tools...]

.github/workflows/
└── test-release.yaml                   # CI integration

FuseCP/Sources/FuseCP.EnterpriseServer.Data/
├── Entities/                           # Entity definitions (source-of-truth)
├── Configuration/                      # Fluent API configurations
├── Migrations/                         # Provider-specific migrations
├── DbContextBase.cs                    # Configuration wiring
└── DbContext.cs                        # Provider routing

FuseCP/Database/
├── install.*.sql                       # Generated fresh install scripts
└── [Upgrade artifacts...]
```

---

## Summary

✅ **Fully Automated**: No manual script launches, no reminders  
✅ **Deterministic**: Clear pass/fail outcomes, measurable compliance  
✅ **Multi-Level**: CI enforcement, local automation, optional pre-commit blocking  
✅ **Self-Healing**: MySQL regenerates automatically when needed  
✅ **Developer Friendly**: Clear error messages with resolution steps  
✅ **Consolidated**: Single entry point, unified documentation
