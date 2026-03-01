param(
    [ValidateSet("IN-PROGRESS", "PAUSED", "BLOCKED", "DONE")]
    [string]$Status = "PAUSED",
    [string]$Objective,
    [string]$WorkItemId,
    [string[]]$ResumeCommands,
    [switch]$OpenInEditor
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..\..")
$personalProgressDir = Join-Path $repoRoot ".fusecp-local\progress"
New-Item -Path $personalProgressDir -ItemType Directory -Force | Out-Null

$handoffPath = Join-Path $personalProgressDir "CONTINUATION_HANDOFF.md"
$trackerPath = Join-Path $personalProgressDir "TODO_MULTI_DAY_TRACKER.md"
$trackerReference = ".fusecp-local/progress/TODO_MULTI_DAY_TRACKER.md"
$manualReference = ".fusecp-local/progress/MANUAL_TESTING_CURRENT_PROGRESS.md"
$handoffReference = ".fusecp-local/progress/CONTINUATION_HANDOFF.md"

function Get-CurrentBranch {
    param([string]$RepoRoot)

    if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
        return "<unknown>"
    }

    $branch = (& git -C $RepoRoot rev-parse --abbrev-ref HEAD 2>$null | Out-String).Trim()
    if ([string]::IsNullOrWhiteSpace($branch)) {
        return "<unknown>"
    }

    return $branch
}

function Get-InProgressWorkItemId {
    param([string]$TrackerFile)

    if (-not (Test-Path $TrackerFile)) {
        return "<set-work-item-id>"
    }

    $lines = Get-Content -Path $TrackerFile -ErrorAction SilentlyContinue
    foreach ($line in $lines) {
        $match = [regex]::Match($line, '^\|\s*([^|]+?)\s*\|\s*[^|]+\|\s*IN-PROGRESS\s*\|', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        if ($match.Success) {
            return $match.Groups[1].Value.Trim()
        }
    }

    return "<set-work-item-id>"
}

function Get-ResumeCommandsFromCurrentHandoff {
    param([string]$HandoffFile)

    if (-not (Test-Path $HandoffFile)) {
        return @()
    }

    $commands = @()
    $lines = Get-Content -Path $HandoffFile -ErrorAction SilentlyContinue
    $inSection = $false

    foreach ($line in $lines) {
        if ($line -match '^##\s+4\)\s+Exact Resume Sequence') {
            $inSection = $true
            continue
        }

        if ($inSection -and $line -match '^##\s+') {
            break
        }

        if ($inSection) {
            $cmdMatch = [regex]::Match($line, '^\s*\d+\.\s+(.+?)\s*$')
            if ($cmdMatch.Success) {
                $commands += $cmdMatch.Groups[1].Value.Trim()
            }
        }
    }

    return @($commands)
}

$date = Get-Date -Format "yyyy-MM-dd"
$branch = Get-CurrentBranch -RepoRoot $repoRoot

if ([string]::IsNullOrWhiteSpace($WorkItemId)) {
    $WorkItemId = Get-InProgressWorkItemId -TrackerFile $trackerPath
}

if ([string]::IsNullOrWhiteSpace($Objective)) {
    $Objective = "Continue current IN-PROGRESS work item with smallest safe validated batch."
}

$effectiveCommands = @()
if ($ResumeCommands -and $ResumeCommands.Count -gt 0) {
    $effectiveCommands = @($ResumeCommands)
}
else {
    $effectiveCommands = Get-ResumeCommandsFromCurrentHandoff -HandoffFile $handoffPath
}

if ($effectiveCommands.Count -eq 0) {
    $effectiveCommands = @(
        "pwsh -File .\\FuseCP\\Tools\\check-test-environment.ps1 -Profile Unit",
        "pwsh -File .\\FuseCP\\Tools\\run-local-validation.ps1 -ChangedOnly -SkipIfNoChanges",
        "pwsh -File .\\FuseCP\\Tools\\analyze-build-warnings.ps1 -LogPath `"FuseCP/msbuild.log`" -Top 40 -JsonOutputPath `"artifacts/validation/warning-summary.json`""
    )
}

$resumeLines = @()
for ($i = 0; $i -lt $effectiveCommands.Count; $i++) {
    $resumeLines += "$($i + 1). $($effectiveCommands[$i])"
}

$content = @"
# Continuation Handoff ($date)

## 1) Session Snapshot

- Date: $date
- Branch: $branch
- Primary work item ID (from .fusecp-local/progress/TODO_MULTI_DAY_TRACKER.md): $WorkItemId
- Current status: $Status
- Primary objective (one sentence): $Objective

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

$($resumeLines -join "`r`n")

## 5) First Safe Next Batch

- Next smallest safe change to apply:
- Scope to validate after change:
- Expected success criteria:

## 6) Risks / Open Questions

- Risk 1:
- Risk 2:
- Decision needed:

## 7) Canonical References

- Tracker: $trackerReference
- Manual evidence: $manualReference
- Current handoff file: $handoffReference
"@

Set-Content -Path $handoffPath -Value $content -Encoding UTF8

Write-Host "Continuation handoff saved: $handoffPath" -ForegroundColor Green
Write-Host "Status: $Status | Work item: $WorkItemId | Branch: $branch | Mode: personal"

if ($OpenInEditor) {
    & code $handoffPath
}

exit 0
