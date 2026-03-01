# Continuation Handoff Template

Use this form whenever work continues across days, reboots, or paused sessions.

Quick helper command:

- Personal/local (git-ignored):
	- `pwsh -File .\FuseCP\Tools\Save-Session-Handoff.ps1 -Status PAUSED`
	- `pwsh -File .\FuseCP\Tools\Save-Session-Handoff.ps1 -Status IN-PROGRESS`

## 1) Session Snapshot

- Date:
- Branch:
- Primary work item ID (from TODO_MULTI_DAY_TRACKER.md):
- Current status: IN-PROGRESS / PAUSED / BLOCKED
- Primary objective (one sentence):

## 2) What Changed In This Session

- Files changed:
- Scripts/tools run:
- Key implementation outcomes:

## 3) Validation Evidence

- Validation commands executed:
- Result summary (pass/fail/blocked):
- Known blockers and exact error signal:

## 4) Exact Resume Sequence

Run from workspace root unless stated otherwise.

1.
2.
3.
4.
5.

## 5) First Safe Next Batch

- Next smallest safe change to apply:
- Scope to validate after change:
- Expected success criteria:

## 6) Risks / Open Questions

- Risk 1:
- Risk 2:
- Decision needed:

## 7) Canonical References

- Personal tracker: .fusecp-local/progress/TODO_MULTI_DAY_TRACKER.md
- Personal manual evidence: .fusecp-local/progress/MANUAL_TESTING_CURRENT_PROGRESS.md
- Personal handoff file: .fusecp-local/progress/CONTINUATION_HANDOFF.md
