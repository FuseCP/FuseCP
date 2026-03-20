# Contributing to FuseCP

This guide describes the expected workflow to develop, test, and commit changes
in FuseCP.

## Architecture Overview

FuseCP is organized into three primary layers:

* **Portal**: Front-end GUI pages and user-facing web experience
* **Enterprise**: Business logic and database communication
* **Server**: Execution layer for server-side operations on managed hosts

For code changes, keep the scope inside the relevant layer(s) and avoid cross-
module refactors unless required by the issue.

## Prerequisites

1. Clone repository and initialize submodules:

   `git submodule update --init --recursive`

2. Install current .NET SDK (CI uses .NET 10).
3. Install Visual Studio/MSBuild tooling on Windows for full build scripts.

## Development Workflow

From repository root, use the module-focused workflow below.

### Apply pending EF migrations (Enterprise DB)

Before testing Portal/Enterprise changes that add or alter schema, apply pending migrations to your local DB.

Run from repository root:

* `Set-Location "FuseCP/Sources/FuseCP.EnterpriseServer.Data"`
* `dotnet ef database update --framework net10.0 --context SqlServerDbContext -- "DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;Integrated Security=True;TrustServerCertificate=true"`

If your SQL instance uses SQL logins instead of integrated auth:

* `Set-Location "FuseCP/Sources/FuseCP.EnterpriseServer.Data"`
* `dotnet ef database update --framework net10.0 --context SqlServerDbContext -- "DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;Uid=YOUR_USER;Pwd=YOUR_PASSWORD;TrustServerCertificate=true"`

Optional verification:

* `dotnet ef migrations list --framework net10.0 --context SqlServerDbContext -- "DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;Integrated Security=True;TrustServerCertificate=true"`

Important: many FuseCP solutions depend on shared projects/artifacts in a
specific order. Independent `.sln` builds are useful for focused iteration, but
for reliable integration validation use orchestrated repository build entrypoints.

### Portal + Enterprise development

Run from `FuseCP/Sources`:

* `dotnet build FuseCP.WebPortalAndEnterpriseServer.sln`
* `dotnet build FuseCP.EnterpriseServer.sln`

### Server development

Run from `FuseCP/Sources`:

* `dotnet build FuseCP.Server.sln`

### Full local build scripts

Run from `FuseCP`:

* Debug build: `build-debug.bat`
* Release build: `build-release.bat`
* Ordered MSBuild orchestration: `dotnet msbuild build.xml /target:Build /p:BuildConfiguration=Debug`

## Testing Workflow

Run from `FuseCP/Sources`:

* Build tests: `dotnet build FuseCP.Tests.sln`
* Execute tests: `dotnet test FuseCP.Tests.sln --configuration Release --no-build -v n`

CI also runs `dotnet test` against `FuseCP.Tests.sln` on pull requests to
`release`.

## Commit Workflow

1. Keep one logical change per commit.
2. Use clear commit messages with module scope when possible, for example:
   * `Portal: fix package creation validation`
   * `Enterprise: prevent null customer contact mapping`
   * `Server: harden IIS app pool restart handling`
3. Before pushing, run the narrowest relevant build/tests for your touched
   module(s).
4. Open a PR and complete `.github/PULL_REQUEST_TEMPLATE.md`, including AI
   disclosure when applicable.

## Branch Naming Convention

Use short, module-scoped branch names:

* `portal/<issue-or-topic>`
* `enterprise/<issue-or-topic>`
* `server/<issue-or-topic>`
* `shared/<issue-or-topic>` for approved cross-module work

Examples:

* `portal/1421-fix-login-redirect`
* `enterprise/1508-null-safe-plan-sync`
* `server/1513-harden-iis-site-delete`

## Commit Message Convention

Use module prefix and imperative summary:

* `Portal: fix login redirect loop`
* `Enterprise: handle null quota rows`
* `Server: retry app pool recycle`

When one commit spans multiple modules, use `Shared:`.

## PR-Ready Command Checklist

Run the narrowest command set that matches your change:

Fast path (single command):

* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Portal,Enterprise`
* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Server -IncludeTests`
* `pwsh -File FuseCP/Tools/run-local-validation.ps1` (shared/integration-safe)
* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly` (auto scope from changed files)
* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -JsonOutputPath artifacts/validation/summary.json`
* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -DisableNuGetAudit`
* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges`
* `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -ScopeMapPath FuseCP/Tools/validation-scope-map.json`

Preferred integration-safe path (ordered dependencies):

* `cd FuseCP`
* `build-debug.bat`
   * or `dotnet msbuild build.xml /target:Build /p:BuildConfiguration=Debug`

* Portal + Enterprise changes:
   * `cd FuseCP/Sources`
   * `dotnet build FuseCP.WebPortalAndEnterpriseServer.sln`
* Enterprise-only changes:
   * `cd FuseCP/Sources`
   * `dotnet build FuseCP.EnterpriseServer.sln`
* Server changes:
   * `cd FuseCP/Sources`
   * `dotnet build FuseCP.Server.sln`
* Tests for PR confidence:
   * `cd FuseCP/Sources`
   * `dotnet build FuseCP.Tests.sln`
   * `dotnet test FuseCP.Tests.sln --configuration Release --no-build -v n`

## Testing Environment Prerequisites

See [TESTING_ENVIRONMENT.md](TESTING_ENVIRONMENT.md) for required tools and
validation commands (WiX, SQL tooling, IIS/WebAdministration, WSL/rpmbuild,
and packaging dependencies).

If your change requires legacy `.vdproj` MSI output, verify that specifically:

* `pwsh -File FuseCP/Tools/check-test-environment.ps1 -Profile Package -RequireLegacyMsi`

## Pull Request Expectations

* Include change summary, risk notes, and validation steps.
* Note whether behavior/configuration changed and update docs if needed.
* Keep backward compatibility unless breaking change is explicitly approved.
* For dependency/CVE updates, include explicit compatibility evidence for affected TFMs and solution scopes, and update related scripts/docs when requirements or commands change.
