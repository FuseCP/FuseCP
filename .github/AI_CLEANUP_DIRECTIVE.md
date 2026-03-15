# AI Code Cleanup Directive

## Requirement: COMPLETE & COMPREHENSIVE CLEANUP

When removing a feature, technology, provider, or component from FuseCP:

**DO NOT perform partial removals.** Incomplete cleanup creates technical debt and confusion.

### Cleanup Checklist (Must Satisfy ALL)

1. **Solution Files & Project References** (ALL Solution Files, Not Just Primary)
   - [ ] Search for ALL `.sln` files in repo (not just `FuseCP.sln`)
   - [ ] Remove project declarations from **each** solution file found
   - [ ] Remove corresponding ProjectConfigurationPlatforms entries (Debug/Release/Platform variants)
   - [ ] Verify with `grep_search` on `**/*.sln` for any remaining project name references
   - [ ] Example: CRM appeared in both `FuseCP.sln` AND `FuseCP/Sources/FuseCP.Server.sln`

2. **Source Code & Project Directories**
   - [ ] Delete all source files related to the component (*.cs, *.vb, etc.)
   - [ ] Delete entire project directories if removing a provider/plugin
   - [ ] Verify deletion with `file_search` to ensure no stray references remain

3. **Database & Data**
   - [ ] Remove EF seed data (Configuration Fluent API files)
   - [ ] Remove or update database enums/constants
   - [ ] Add cleanup in the real source of truth first: Entity classes, `Configuration/*.cs` Fluent API seed/config, and EF migrations
   - [ ] Regenerate `install.*.sql` from migrations; do not hand-edit generated install scripts as the primary fix
   - [ ] Update legacy upgrade scripts (`update_db.sql`, `Migrate_msSQL.sql`, `LegacyScripts/`) only when the supported pre-v2.0.0 or module-cleanup upgrade path requires it

4. **Dependencies & References**
   - [ ] Remove NuGet package references from *.csproj files
   - [ ] Remove assembly references from *.csproj files  
   - [ ] Remove using/import statements that become unused
   
5. **Language & Localization**
   - [ ] Search `Languages/Resources.xml` and language subdirectories for component strings
   - [ ] Remove or comment out resource entries for the component
   - [ ] Example: CRM cleanup required removing "Hosted CRM", "CRM Organization", quota keys, error messages

6. **Configuration & Documentation**
   - [ ] Remove from web.config, app.config templates
   - [ ] Update README or documentation files
   - [ ] Update CHANGELOG if component was user-facing
   - [ ] Check `.github/upgrades/scenarios/` for assessment/upgrade docs referencing the component

7. **Build Orchestration & Tooling**
   - [ ] Remove from `FuseCP/build.xml` (MSBuild orchestration)
   - [ ] Remove from `FuseCP/test.xml` if present
   - [ ] Remove from build/deploy scripts in `FuseCP/` root if referenced
   - [ ] Example: CRM required removing `<ServerBuildExclude>` entry for `Microsoft.Crm.*` assemblies

8. **Tests & Examples**
   - [ ] Delete test fixtures or test projects
   - [ ] Remove example code referencing the component
   - [ ] Remove from unit test suites

9. **Artifacts & Reports** (Documentation/Auto-Generated)
   - [ ] Regenerate solution scope reports if they exist (e.g., `artifacts/scope-sln-inclusion-report.json`)
   - [ ] These are auto-generated but should be validated after cleanup to confirm component removal
   - [ ] Note: Some reports may be stale and should be regenerated or safely ignored per project docs

10. **Server Module Provider Verification** (REQUIRED for provider logic changes)
   - [ ] If a server/provider module is changed, run a targeted build for the provider project (`dotnet build ...Providers...csproj`)
   - [ ] Verify no provider contract drift: interface and externally consumed method signatures must remain intact unless explicitly requested
   - [ ] Verify EnterpriseServer service entry points still map correctly to the provider
   - [ ] Verify WebPortal call paths still target the same EnterpriseServer methods/contracts
   - [ ] If execution strategy changes (for example AppDomain removal), confirm behavior parity for normal and error paths
   - [ ] Record validation evidence in commit/PR notes (what was built, what was checked, known warnings)

### Example: CRM Cleanup (Completed 2026-03-13)

**Commits involved (or pending):**
- `2345b83ac` - Removed EF seed data + added DB migration guards
- `efbceab06` - Excluded CRM code from net10.0 build, removed references
- `b8c689777` - Deleted CRM source files (CRMProxy.cs, CRMProvider.cs)  
- `LATEST` - Complete CRM removal: solution files, project directories, build config, directive update

**Files/Areas touched (comprehensive):**

**Solution Files (Multiple):**
- `FuseCP.sln` (removed 4 projects + config entries)
- `FuseCP/Sources/FuseCP.Server.sln` (removed 4 projects with different GUIDs)

**Source Code & Directories:**
- `FuseCP/Sources/FuseCP.Providers.HostedSolution/` (2 source files deleted: CRMProxy.cs, CRMProvider.cs; csproj updated)
- `FuseCP/Sources/FuseCP.Providers.HostedSolution.Crm2011/` - **DIRECTORY DELETED**
- `FuseCP/Sources/FuseCP.Providers.HostedSolution.Crm2013/` - **DIRECTORY DELETED**
- `FuseCP/Sources/FuseCP.Providers.HostedSolution.Crm2015/` - **DIRECTORY DELETED**
- `FuseCP/Sources/FuseCP.Providers.HostedSolution.Crm2016/` - **DIRECTORY DELETED**

**Database & Data:**
- `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Configuration/` (3 Fluent API files modified: ProviderConfiguration, QuotaConfiguration, ResourceGroupConfiguration)
- `FuseCP/Database/install.sqlserver.sql` (migration guard added)
- `FuseCP/Database/install.mysql.sql` (migration guard added)
- `FuseCP/Database/install.postgresql.sql` (migration guard added)
- `FuseCP/Database/install.sqlite.sql` (migration guard added)
- `FuseCP/Database/update_db.sql` (migration guard added)
- `FuseCP/Sources/FuseCP.EnterpriseServer.Data/LegacyScripts/` (1 legacy script updated)

**Language & Localization:**
- `Languages/Resources.xml` (11+ resource entries for CRM messages, quotas, warnings - pending removal review)

**Build & Orchestration:**
- `FuseCP/build.xml` (removed `<ServerBuildExclude Include="$(ServerSrc)\bin\Microsoft.Crm.*"/>`)

**Documentation & Assessments:**
- `.github/upgrades/scenarios/new-dotnet-version_642b12/assessment.md` (CRM project references in tables - pending cleanup or regeneration)
- `artifacts/scope-sln-inclusion-report.json` (CRM entries - auto-generated, will be stale until regenerated)
- `artifacts/scope-sln-inclusion-report.md` (CRM entries - auto-generated, will be stale until regenerated)

**Lessons Learned:**
- CRM removal was NOT simply a "delete source code" task - required touching 15+ distinct files/locations
- Multiple solution files exist in repo; grep across **all** .sln files, not just primary
- Build orchestration (build.xml) contains component-specific exclusions that must also be cleaned
- Language resources contain user-facing strings that still reference removed components
- Generated installer SQL can hide the real root cause; provider/seed cleanup usually starts in `Configuration/*.cs` and migrations, not in `install.*.sql`

### Enforcement

- **For AI assistants**: Use this checklist before marking cleanup work as "complete."
- **For reviewers**: Reject cleanup PRs that are incomplete, with reference to this directive.
- **For maintainers**: If partial cleanup is discovered, return to AI/developer for full cleanup before merge.

---

**Last Updated:** 2026-03-13  
**Related Work:** CRM Decommission (TASK-016 Phase 3)
