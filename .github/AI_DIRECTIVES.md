# AI Directives for FuseCP

This document defines minimum standards for AI-assisted work in the FuseCP
repository. It applies to maintainers, contributors, and any automation
submitting content.

## 1. Accountability

All AI-assisted output must be reviewed and approved by a human contributor
before merge. The person opening the PR is responsible for correctness,
security, licensing, and production impact.

## 2. Transparency

If a change is materially AI-assisted, disclose it in the PR description with:

* Which AI tool(s) were used
* Which parts were generated, summarized, or transformed
* What manual validation was performed

## 3. Security and Privacy

Do not share secrets or sensitive operational data with third-party AI systems,
including:

* Credentials, keys, connection strings, and tokens
* Customer or tenant data
* Internal infrastructure details not required for the task

Use sanitized examples whenever possible.

## 4. Code Quality and Testing

AI-generated changes must meet the same quality standard as human-authored code:

* Compile and pass relevant tests
* Prefer repository build orchestrators (`build.xml`, `build-*.bat`, `deploy-*.bat`) over independently building isolated solution files when validating broad changes
* Prefer streamlined local validation entrypoint `FuseCP/Tools/run-local-validation.ps1` for repeatable local checks
* Use `run-local-validation.ps1 -ChangedOnly` for fast local loops when scope can be derived from changed files
* Prefer `-JsonOutputPath` for machine-readable validation evidence in PR workflows
* Use `-DisableNuGetAudit` only for local iteration noise reduction; do not treat it as a security fix
* Use `-SkipIfNoChanges` with `-ChangedOnly` when you want no-op runs to complete without full builds
* Use `-ScopeMapPath` to extend path-to-scope routing without editing the script
* Before adding/removing/changing any `ProjectReference`, check all relevant solution files under `FuseCP/Sources/*.sln` (at minimum Portal/Enterprise/Server paths) and validate the affected solution builds to avoid cross-solution breakage
* Treat FuseCP as a migrated codebase: if implementation intent is unclear, consult project origins in branch `origin/SolidCPv1` (typically under `SolidCP/Sources/...`) to recover legacy behavior and architecture context before changing contracts or build wiring
* For package/dependency/CVE updates, require compatibility validation across affected target frameworks (`net48`, `net10.0`, `netstandard2.0` where applicable) and affected solution scopes (Portal/Enterprise/Server)
* For package/dependency/CVE updates, update related validation scripts/docs/PR notes whenever requirements, tooling assumptions, or recommended commands change
* Follow existing project patterns and style
* Include documentation updates when behavior changes
* Avoid introducing unused dependencies or broad refactors unrelated to the task
* If changes touch legacy installer `.vdproj` packaging, explicitly validate legacy MSI prerequisites (`check-test-environment.ps1 -Profile Package -RequireLegacyMsi`)

## 5. Legal and Licensing

Contributions must comply with repository licensing and third-party license
requirements. Do not submit generated code or content if you cannot verify legal
use or attribution obligations.

## 6. Disallowed Uses

AI tools must not be used to:

* Generate abusive, discriminatory, or harassing project content
* Fabricate test results, benchmarks, incident data, or release notes
* Bypass review, approval, or security controls

## 7. Maintainer Enforcement

Maintainers may request edits, additional testing, provenance details, or reject
changes that do not comply with these directives.

## 8. Session Continuity (Required)

When AI performs multi-session work, it must maintain the repository tracker:

* Use `TODO_MULTI_DAY_TRACKER.md` as the single source of truth for ongoing tasks.
* At session start, resume from any `IN-PROGRESS` item first; otherwise pick the highest-priority `TODO`.
* Keep exactly one item in `IN-PROGRESS` unless maintainers explicitly request parallel tracks.
* On pause/stop, update the item status (`PAUSED` or `DONE`) and set a concrete `Next Resume Step`.
* Append a one-line Daily Log entry summarizing progress and validation performed.

If this tracker is missing or out of date, AI should create/update it before continuing substantial implementation.

These directives supplement (and do not replace) the project Code of Conduct.