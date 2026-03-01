# Multi-Day Work Tracker

Last updated: 2026-03-01 (session continuity enabled)

## Status Legend

- TODO = not started
- IN-PROGRESS = actively being worked
- PAUSED = intentionally paused
- BLOCKED = waiting on dependency/decision
- DONE = completed

## How We Use This

1. Move only one high-priority item to IN-PROGRESS at a time.
2. If we stop for the day, set it to PAUSED and add a short resume note.
3. At next session start, pick from IN-PROGRESS first, then highest-priority TODO.
4. Add a one-line entry to Daily Log after each meaningful session.

## Active Backlog

| ID | Priority | Status | Area | Task | Next Resume Step | Updated |
|---|---|---|---|---|---|---|
| P1-01 | P1 | IN-PROGRESS | Net10 Compatibility | Upgrade WebFormsForCore stack from 1.4.2 -> 1.4.6 in Portal and Portal.Modules | Update package refs in WebPortal + Portal.Modules, then run `run-local-validation.ps1 -Scope Portal -Configuration Debug -DisableNuGetAudit` | 2026-03-01 |
| P1-02 | P1 | TODO | Compatibility | Remove SSL3/WebRequest-era TLS behavior in CheckWebSiteTask | Replace with modern HttpClient flow, preserve behavior | 2026-03-01 |
| P1-03 | P1 | TODO | MailCleaner | Modernize APIMailCleanerHelper from HttpWebRequest/ServicePointManager to HttpClientHandler | Keep IgnoreCheckSSL behavior, add compatibility smoke check | 2026-03-01 |
| P2-01 | P2 | TODO | Packages | Patch/minor update wave for MailKit, MimeKit, Microsoft.Data.SqlClient, BouncyCastle | Update in smallest safe batch, run ChangedOnly validation | 2026-03-01 |
| P2-02 | P2 | TODO | Packages | Align IdentityModel package family versions | Choose one aligned version band and rebuild Enterprise/Server | 2026-03-01 |
| P2-03 | P2 | TODO | Packages | Evaluate Swashbuckle 9.x -> 10.x for net10 paths | Upgrade Web.Services path and verify startup/smoke | 2026-03-01 |
| P2-04 | P2 | TODO | Build Hygiene | Reduce highest-noise warning clusters (CA2200, SYSLIB, NU1701) | Triage top 10 warning sources and create focused fixes | 2026-03-01 |
| P3-01 | P3 | TODO | Legacy Frameworks | Inventory remaining v2.0/v3.5/v4.0/netcoreapp2.1 projects and define lifecycle | Draft keep/migrate/retire matrix | 2026-03-01 |
| P3-02 | P3 | TODO | Build Process | Decide policy for optional legacy MSI build dependency | Confirm maintainer stance on BuildInstallerMsi default path | 2026-03-01 |

## Session Checklist (Start / Pause / Resume)

### Start of Session

- Review top IN-PROGRESS or highest-priority TODO.
- Confirm environment quickly:
  - `pwsh -File .\FuseCP\Tools\check-test-environment.ps1 -Profile Unit`
- Run narrow validation before broad build.

### Pause of Session

- Update row Status (IN-PROGRESS -> PAUSED if unfinished).
- Fill Next Resume Step with exact next action.
- Add Daily Log entry with what changed and validation done.

### Resume of Session

- Read last Daily Log entry.
- Continue from Next Resume Step.
- Keep scope narrow and revalidate.

## Daily Log

### 2026-03-01

- Established baseline: core projects show no vulnerable packages in targeted scan.
- Scoped validation (Portal/Enterprise/Server) succeeded with high warning volume.
- Identified primary next focus: net10 compatibility warnings (NU1701/WebFormsForCore path) and legacy TLS/WebRequest code paths.
- Session continuity enabled: set P1-01 to IN-PROGRESS as the default resume target.

## Parking Lot (Ideas, Not Committed)

- Add CI job to publish warning delta by solution scope.
- Add automated markdown report output from run-local-validation JSON mode.
- Separate package update lanes: security lane vs modernization lane.