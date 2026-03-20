# FuseCP

[![Build](https://github.com/FuseCP/FuseCP/actions/workflows/deploy-release.yaml/badge.svg)](https://github.com/FuseCP/FuseCP/actions/workflows/deploy-release.yaml)
[![Test](https://github.com/FuseCP/FuseCP/actions/workflows/test-release.yaml/badge.svg)](https://github.com/FuseCP/FuseCP/actions/workflows/test-release.yaml)

FuseCP is a complete, open-source control panel for Cloud Computing Companies and IT Providers. It lets you provision and manage a full suite of hosted services across your infrastructure from a single web-based administration portal.

> FuseCP is the successor to SolidCP. It is free, open source, and actively developed.

**Website & downloads:** [www.fusecp.com](https://www.fusecp.com)

---

## What can FuseCP manage?

| Category | Services |
| -------- | -------- |
| **Web hosting** | IIS 10, FTP |
| **Databases** | SQL Server 2022, MySQL, MariaDB, SQLite, PostgreSQL |
| **Email & collaboration** | Microsoft Exchange SE & 2019, Mailenable, Smartermail |
| **Directory services** | Active Directory |
| **Remote access** | Microsoft RemoteApp / RDS, Guacamole (browser-based RDP/VNC/SSH) |
| **Virtualisation** | Hyper-V, Proxmox |
| **Billing integration** | WHMCS module included |

FuseCP uses a multi-tenant model, meaning one installation can serve multiple resellers and their end customers, each with their own isolated service quotas and hosting plans.

---

## Installing FuseCP

Download the latest release installer from [www.fusecp.com](https://www.fusecp.com) or the [Releases](../../releases) page.

The installer sets up three components on your Windows Server:

| Component | Role |
| --------- | ---- |
| **Portal** | The web-based administration interface (IIS) |
| **Enterprise Server** | The business logic and database service |
| **Server Agent** | Runs on each managed host to execute provisioning actions |

All three can run on the same machine for small deployments, or be split across servers for larger environments.

### Supported platforms

* **Operating system**: Windows Server 2016 / 2019 / 2022 / 2025
* **Database**: SQL Server (Express or full), MySQL, MariaDB
* **.NET**: .NET Framework 4.8 (current release) — .NET 10 support in progress

---

## Getting started

After installation, open the FuseCP Portal URL in a browser. Log in with the administrator account created during setup.

From the portal you can:

1. **Add servers** — register your managed hosts under *Server* → *Servers*
2. **Create hosting plans** — define resource quotas under *System* → *Hosting Plans*
3. **Add resellers and customers** — provision accounts under *Account Management*
4. **Provision services** — assign web, mail, database, and other services to customer packages

Full administration documentation is available at [www.fusecp.com](https://www.fusecp.com).

---

## Upgrading from SolidCP

FuseCP is a direct successor to SolidCP and maintains database and configuration compatibility. The upgrade path is covered in the installation documentation on [www.fusecp.com](https://www.fusecp.com).

---

## Support & community

* **Website**: [www.fusecp.com](https://www.fusecp.com)
* **Issues**: [GitHub Issues](../../issues)
* **Security vulnerabilities**: see [SECURITY.md](SECURITY.md) for responsible disclosure

---

## For developers and contributors

If you want to build FuseCP from source, contribute code, or understand the internal architecture, see:

| Document | Purpose |
| -------- | ------- |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Development workflow, build commands, and PR process |
| [TESTING_ENVIRONMENT.md](TESTING_ENVIRONMENT.md) | Local test environment setup |
| [AI_DIRECTIVES.md](.github/AI_DIRECTIVES.md) | Standards for AI-assisted contributions, CSS/LESS and DB schema rules |
| [AI_FUSECP_PLAYBOOK.md](.github/AI_FUSECP_PLAYBOOK.md) | Safe-first workflow for AI coding agents |
| [FuseCP.EnterpriseServer.Data/README.md](FuseCP/Sources/FuseCP.EnterpriseServer.Data/README.md) | EF database layer, migrations, and scaffolding |
| [PROCESS_STREAMLINING.md](PROCESS_STREAMLINING.md) | Tooling and process notes |

---

## License

FuseCP is released under the GNU General Public License v3. See [LICENSE](LICENSE) for details.

---

## Community standards

* [Code of Conduct](CODE_OF_CONDUCT.md)
* [Security Policy](SECURITY.md)
* [Contributing Guide](CONTRIBUTING.md)

---

## Architecture

FuseCP is organized into three primary layers:

| Layer | Description |
| ----- | ----------- |
| **Portal** | ASP.NET (WebForms, migrating to Core) front-end — user-facing web UI |
| **Enterprise Server** | Business logic, database access (EF Core 8 / EF 6), and service orchestration |
| **Server** | Execution agent that runs on managed hosts and carries out provisioning actions |

Source code is under `FuseCP/Sources/`. Build and deployment scripts are under `FuseCP/` and `tools/`.

---

## Prerequisites

| Requirement | Notes |
| ----------- | ----- |
| .NET 10 SDK | Required for Core-targeted builds and EF migrations |
| Visual Studio / MSBuild | Required for full orchestrated builds and `.vdproj` installer projects |
| Node.js / npm | Required for portal LESS → CSS compilation (`FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles/`) |
| PowerShell 7+ (`pwsh`) | Required for all tooling scripts |
| SQL Server Express (local) | Required for integration testing |
| IIS | Required for portal integration tests |
| WiX Toolset v3.14 | Required for legacy installer packaging only |

First-time full setup (elevated PowerShell):

```powershell
powershell -File FuseCP/Tools/bootstrap-test-environment.ps1 `
  -Install -RunAllProfiles -InstallSqlExpress -InstallIIS -InstallWsl -InstallWixToolset
```

Check prerequisites only:

```powershell
pwsh -File FuseCP/Tools/check-test-environment.ps1 -Profile Unit
```

---

## Quick Start

### 1. Clone

```bash
git clone https://github.com/FuseCP/FuseCP.git
git submodule update --init --recursive
```

### 2. Start of day

```powershell
pwsh -File FuseCP/Tools/Start-Of-Day.ps1
```

### 3. Build (debug)

```powershell
# Full orchestrated build
FuseCP/build-debug.bat

# Or via the scripted validation entrypoint (recommended for CI-like local checks)
pwsh -File FuseCP/Tools/run-local-validation.ps1
```

### 4. Validate changed files only (fast loop)

```powershell
pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges -DisableNuGetAudit
```

### 5. Deploy (debug, local)

```powershell
FuseCP/deploy-debug.bat
```

### 6. End of day

Run the VS Code task **Done for today** or:

```powershell
pwsh -File FuseCP/Tools/Done-For-Today.ps1
```

---

## Folder Structure

```
FuseCP/                         Build, deploy, and tooling scripts
  Sources/                      All C# / VB source code
    FuseCP.WebPortal/           Portal ASP.NET project
      App_Themes/Default/Styles/  LESS + compiled CSS theme files
    FuseCP.EnterpriseServer/    Business logic layer
    FuseCP.EnterpriseServer.Data/  EF database layer (Entities, Migrations, Config)
    FuseCP.Server/              Server agent project
  Database/                     SQL install and update scripts
  Tools/                        PowerShell dev and CI tooling scripts
  Bin/                          Shared compiled output
FuseCP.Installer/               Legacy MSI installer (.vdproj)
FuseCP.WebSite/                 Standalone website package
Languages/                      Localization resource files
tools/                          Third-party build tools (WiX, 7-Zip, etc.)
artifacts/                      Local build and session artifacts (gitignored)
```

---

## UI Theme Development (LESS/CSS)

All portal styling is authored in LESS source files — never edit `main.css` directly.

| File | Purpose |
| ---- | ------- |
| `main.less` | Main theme rules — colors, layout, component overrides |
| `Menus.less` | Navigation, menus, popup and grid rules |
| `defaultVariables.less` | Shared LESS variables (`@footer-bg`, Bootstrap overrides, etc.) |
| `defaultTheme.less` | Root entry point — imports all of the above |
| `main.css` | **Compiled output — do not edit** |

Recompile after editing any `.less` file:

```powershell
cd FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/Styles
npm run build:css
```

Commit both the `.less` source and the recompiled `main.css` together.

---

## Database Schema Changes (Entity Framework)

FuseCP uses EF Core 8 on .NET 10 and EF 6 on .NET Framework, with a single shared `DbContext` in `FuseCP.EnterpriseServer.Data`.

### Workflow

1. Edit Entity class(es) in `FuseCP/Sources/FuseCP.EnterpriseServer.Data/Entities/`.
2. Update Fluent API configuration in `Configuration/` for cross-DB type mapping if needed.
3. Create a migration:

   ```bat
   cd FuseCP/Sources/FuseCP.EnterpriseServer.Data
   MigrationAdd.bat
   ```

4. Review the generated files under `Migrations/`.
5. Also update `FuseCP/Database/update_db.sql` to keep the legacy SQL path in sync.

### Apply pending EF migrations to your local database

When code includes new entities/tables (for example `IpSecurityPolicies`), update your local DB before running the portal/server.

1. Open PowerShell in repository root.
2. Change directory to the EF data project.
3. Run `dotnet ef database update` with the provider/connection you use locally.

Windows Integrated Authentication (recommended for local dev):

```powershell
Set-Location "FuseCP/Sources/FuseCP.EnterpriseServer.Data"
dotnet ef database update --framework net10.0 --context SqlServerDbContext -- "DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;Integrated Security=True;TrustServerCertificate=true"
```

SQL Login authentication (if your local SQL instance uses SQL users):

```powershell
Set-Location "FuseCP/Sources/FuseCP.EnterpriseServer.Data"
dotnet ef database update --framework net10.0 --context SqlServerDbContext -- "DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;Uid=YOUR_USER;Pwd=YOUR_PASSWORD;TrustServerCertificate=true"
```

Optional verification (shows migration chain known to EF):

```powershell
Set-Location "FuseCP/Sources/FuseCP.EnterpriseServer.Data"
dotnet ef migrations list --framework net10.0 --context SqlServerDbContext -- "DbType=SqlServer;Server=(local);Initial Catalog=FuseCP;Integrated Security=True;TrustServerCertificate=true"
```

If update fails with a login error, switch authentication mode (Integrated Security vs SQL login) and retry with credentials that can modify schema in the target database.

For scaffolding from an existing database or porting raw SQL changes, see the full guide in [`FuseCP/Sources/FuseCP.EnterpriseServer.Data/README.md`](FuseCP/Sources/FuseCP.EnterpriseServer.Data/README.md).

**Never edit EF model snapshot (`.cs`) files by hand** — always let `dotnet ef` maintain them.

---

## Validation Reference

```powershell
# Broadest (full build, all scopes)
pwsh -File FuseCP/Tools/run-local-validation.ps1

# Changed files only
pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges

# Scoped (Portal, Enterprise, or Server)
pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Portal

# After initial restore (skip restore for speed)
pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Enterprise -NoRestore

# Machine-readable output for PR tooling
pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -JsonOutputPath artifacts/validation/summary.json
```

> In scoped mode, `Portal` already builds `FuseCP.WebPortalAndEnterpriseServer.sln`; selecting both `Portal` and `Enterprise` does not run a redundant extra build.

---

## Governance

| Document | Purpose |
| -------- | ------- |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Development workflow, architecture overview, PR process |
| [AI_DIRECTIVES.md](.github/AI_DIRECTIVES.md) | Minimum standards for AI-assisted contributions, including CSS/LESS and DB schema rules |
| [AI_FUSECP_PLAYBOOK.md](.github/AI_FUSECP_PLAYBOOK.md) | Safe-first workflow and scope tips for AI coding agents |
| [TESTING_ENVIRONMENT.md](TESTING_ENVIRONMENT.md) | Local test environment setup and profiles |
| [SECURITY.md](SECURITY.md) | Vulnerability disclosure policy |
| [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) | Community standards |
| [PROCESS_STREAMLINING.md](PROCESS_STREAMLINING.md) | Tooling and process optimization notes |
| [CHANGELOG](CHANGELOG) | Release history |
