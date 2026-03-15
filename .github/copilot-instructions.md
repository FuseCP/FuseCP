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
* **Database schema changes**: Edit Entity classes under `FuseCP.EnterpriseServer.Data/Entities/`, update Configuration Fluent API if needed, then generate a migration with `MigrationAdd.bat`. Treat `install.*.sql` as generated artifacts, not the source of truth; SQLite upgrades run through EF migrations, not `install.sqlite.sql`. Never edit EF model snapshot files by hand. See `FuseCP/Sources/FuseCP.EnterpriseServer.Data/README.md` and `AI_DIRECTIVES.md §6` for the full workflow.

## Security and Data Handling

* Never expose secrets, credentials, tokens, or private tenant data.
* Avoid introducing insecure defaults.
* Flag security-sensitive changes for maintainer review.

## Testing and Verification

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