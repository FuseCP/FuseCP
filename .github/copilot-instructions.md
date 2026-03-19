# Copilot Instructions for FuseCP

These instructions guide AI coding assistants working in this repository.

## Core Rules

* Keep changes minimal and task-focused.
* Preserve existing behavior unless the issue explicitly requests a change.
* Match existing architecture and coding style in the touched project.
* Do not modify unrelated files.

## Tech Context

* FuseCP is the follow-up to SolidCP and is migrating to ASP.NET Core.
* Solutions and projects are split across multiple subfolders under `FuseCP/Sources`.
* Build and deployment scripts exist in `FuseCP/` and `tools/`.
* Many solutions have ordering/dependency relationships; prefer orchestrated build entrypoints over ad-hoc independent `.sln` builds for end-to-end validation.
* For quick repository orientation and safe-first workflows, use `.github/AI_FUSECP_PLAYBOOK.md`.

## Implementation Expectations

* Prefer root-cause fixes over cosmetic patches.
* Validate null handling, error paths, and permission checks.
* Keep backward compatibility in shared contracts unless explicitly approved.
* Update docs when behavior, configuration, or deployment steps change.
* For files that carry copyright headers/metadata, use exact text `Copyright (C) 2026 FuseCP` and keep generator-driven files in sync (for example `FuseCP/build.xml` and generated `VersionInfo.*` files).
* **UI/CSS changes**: Always edit the LESS source files (`main.less`, `Menus.less`) — never `main.css` directly. Recompile with `npm run build:css` from the `App_Themes/Default/Styles/` directory and commit both the `.less` and the recompiled `.css`.
* **Database schema changes**: Edit Entity classes under `FuseCP.EnterpriseServer.Data/Entities/`, create corresponding Configuration class in `Configuration/`, then:
  1. Add `ApplyConfiguration(model, new MyEntityConfiguration());` in `DbContextBase.OnModelCreating()`
  2. Add DbSet property in `DbContext.Sets.cs`
  3. Run `FuseCP/Sources/FuseCP.EnterpriseServer.Data/MigrationAdd.bat` to generate migrations for all 4 providers and regenerate `install.*.sql`
  4. Commit Entity, Configuration, migration files, and regenerated `install.*.sql` files
  5. **Database workflow verification is FULLY AUTOMATED**: Never manually run verification scripts - they execute automatically in CI, local validation, and pre-commit hooks. Treat `install.*.sql` as generated artifacts, not the source of truth. SQLite upgrades run through EF migrations. Never edit EF model snapshot files or migration files by hand. See `DATABASE_WORKFLOW_COMPLETE.md` for complete reference.

## Exchange Provider Patterns

* **Provider parity**: Exchange providers for 2013, 2016, and 2019 (`FuseCP.Providers.HostedSolution.Exchange2013/2016/2019`) share identical method structure. Any change to `GetMailbox*`, `SetMailbox*`, or shared helper methods must be applied to **all three providers in the same commit** and all three must be built to confirm no compile regressions.
* **Remoted PSObject type variance**: Exchange PowerShell remoting returns PSObjects whose properties can have unexpected runtime shapes — e.g. `SmtpAddress` may arrive as a plain string, size properties may arrive as `Unlimited<ByteQuantifiedSize>` or as a formatted string, and boolean properties may arrive as non-bool objects. Never use direct casts (`(bool)`, `(Unlimited<int>)`, `(Unlimited<ByteQuantifiedSize>)`) on `GetPSObjectProperty()` results. Use the existing safe helpers: `ObjToBoolean`, `ConvertByteSizePropertyToKB`, `ConvertByteSizePropertyToMB`, `ConvertUnlimitedIntPropertyToInt32`.
* **No-language runspace restrictions**: Exchange remoting runs in constrained/no-language mode. Setting `ConfirmPreference` and calling `Get-MailboxSearch` can throw "Script invocation is not supported in this session configuration" — always guard such calls with try-catch and provide a fallback code path.
* **PSObject property access**: Prefer `PSObject.Properties["name"]` over `PSObject.Members["name"]` when reading remoted Exchange objects; `Members` can hit script-backed properties that fail in constrained sessions.

## Security and Data Handling

* Never expose secrets, credentials, tokens, or private tenant data.
* Avoid introducing insecure defaults.
* Flag security-sensitive changes for maintainer review.
* **Web.config policy**: Commit only required structural/runtime fixes (for example ANCM wiring, handler registration, section definitions, non-secret defaults). Never commit environment-specific secrets in `Web.config` (including real connection strings, usernames/passwords, machine keys, and private endpoints).
* **When `Web.config` needs functional fixes**: create a sanitized commit-safe variant for git, then restore local secret-bearing values after commit and keep those local-only values out of source control (for example via local git index flags such as `skip-worktree` where appropriate).

## Testing and Verification

* **Automated Database Verification** (NO MANUAL LAUNCHES): Database schema compliance is fully automated:
  - Single entry point: `FuseCP/Tools/Orchestrate-Database-Workflow.ps1` (modes: Quick, Full, Verify, Fix, Report)
  - Enforced at: CI (every PR/commit), local builds (before validation), pre-commit (if hook enabled)
  - Automatically regenerates MySQL artifacts when migrations change
  - Blocks builds that violate EF workflow (misaligned entities/configs, stale install scripts)
  - Reference: `DATABASE_WORKFLOW_COMPLETE.md` for complete guide, developer workflows, troubleshooting

* At the start of each new development day/session, run `FuseCP/Tools/Start-Of-Day.ps1` before making code changes.
* If the task is docs-only or this check would be redundant in the same session, at minimum run `FuseCP/Tools/check-sln-scope-sync.ps1`.
* Run the narrowest relevant build/tests first, then broaden if needed.
* For broad validation, use repository orchestrators (`build.xml`, `build-debug.bat`, `build-release.bat`, `deploy-*.bat`) because independent solution order may be insufficient.
* Prefer the scripted validation entrypoint `FuseCP/Tools/run-local-validation.ps1` to keep local verification consistent and efficient.
* Prefer `run-local-validation.ps1 -ChangedOnly` for fast iteration when a path-based scope can be inferred safely.
* Prefer `-JsonOutputPath` when validation output should be consumed by PR tooling or CI helpers.
* Use `-DisableNuGetAudit` only to reduce local warning noise during iteration; keep full audit in regular validation.
* Use `-SkipIfNoChanges` with `-ChangedOnly` to avoid unnecessary local builds when no files are touched.
* Use `-ScopeMapPath` to extend path-to-scope routing from JSON without editing script logic.
* If work affects legacy installer `.vdproj` packaging, verify prerequisites with `check-test-environment.ps1 -Profile Package -RequireLegacyMsi`.
* Report what was validated and what could not be validated locally.

## Pull Request Hygiene

* Keep PR scope cohesive.
* Include a concise summary, risk notes, and verification steps.
* When asked to create an upstream PR, use `FuseCP/Tools/Create-Upstream-PR.ps1` so `PR_DRAFT.md` is cleared only after successful PR creation.
* If AI materially assisted implementation, disclose usage in the PR body.
* For GitHub Actions artifact uploads, sanitize any dynamic artifact names (for example from commit/PR text) so invalid filesystem characters are removed/replaced before `actions/upload-artifact` runs (`"`, `:`, `<`, `>`, `|`, `*`, `?`, `\\`, `/`, CR, LF).
