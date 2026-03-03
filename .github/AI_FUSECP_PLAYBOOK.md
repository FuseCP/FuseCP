# AI FuseCP Playbook

This playbook gives AI coding agents a safe, fast starting workflow for FuseCP.

## 1) First 5 Minutes

1. Read:
   - `.github/AI_DIRECTIVES.md`
   - `.github/copilot-instructions.md`
   - `CONTRIBUTING.md`
   - `TESTING_ENVIRONMENT.md`
2. Confirm prerequisites:
   - `pwsh -File FuseCP/Tools/check-test-environment.ps1 -Profile Unit`
3. Run a fast local validation baseline:
   - `pwsh -File FuseCP/Tools/run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges -DisableNuGetAudit`

## 2) Build/Validation Strategy

- Use **smallest relevant scope first**:
  - Portal: `-Scope Portal`
  - Enterprise: `-Scope Enterprise`
  - Server: `-Scope Server`
- Scope note:
  - `Portal` already builds `FuseCP.WebPortalAndEnterpriseServer.sln`.
  - If both `Portal` and `Enterprise` are selected, validation now skips the redundant Enterprise-only build automatically.
- Use orchestrated build when touching shared/build/deployment flows:
  - `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Shared`
- For repeated local loops after restore:
  - `pwsh -File FuseCP/Tools/run-local-validation.ps1 -Scope Enterprise -NoRestore`

## 3) Safe Change Rules

- Keep edits minimal and task-focused.
- Preserve behavior unless explicitly requested.
- Prefer root-cause fixes.
- Avoid broad package churn in unrelated projects.
- Keep PR scope cohesive; separate docs/tooling/runtime concerns when possible.

## 4) Scope Mapping Tips

`run-local-validation.ps1 -ChangedOnly` derives scope from changed paths.
Use optional overrides with:

- `-ScopeMapPath FuseCP/Tools/validation-scope-map.example.json`

Choose `-Scope Shared` when uncertain.

## 5) Warning Reduction Policy

- Prioritize low-risk fixes first:
  - exact-version alignment for existing package constraints
  - removal of unnecessary explicit references
- Validate warning deltas in batches (not per-file loops) before broadening.
- Use `FuseCP/build-debug.bat` as the default validation gate for warning-remediation batches.
- Do not hide warnings globally; prefer explicit fixes.

Regression-prevention requirements during warning work:

- Do not remove or weaken project dependency edges/output-copy behavior to silence warnings.
- If any package/reference/project-reference/output-path change is made, verify critical runtime assemblies still resolve:
  - `FuseCP.EnterpriseServer.dll`
  - `FuseCP.Server.dll`
  - `FuseCP.EnterpriseServer.Client.dll`
- Treat Portal/Enterprise/Server as coupled scopes for compatibility checks.

For dependency/CVE updates:

- Validate compatibility across all affected TFMs (`net48`, `net10.0`, `netstandard2.0` as applicable).
- Validate all affected solution scopes (`Portal`, `Enterprise`, `Server`) before merge.
- Update related scripts/docs if package requirements or recommended commands change.

## 6) PR Hygiene Checklist

- Include:
  - concise summary
  - risk notes
  - exact validation commands run
  - AI usage disclosure when materially assisted
- Mention what was **not** validated locally if anything was skipped.

## 7) Escalation Triggers

Escalate to maintainers when changes involve:

- installer packaging (`.vdproj`, legacy MSI prerequisites)
- security-sensitive defaults
- multi-solution dependency graph changes
- broad framework/package major upgrades
