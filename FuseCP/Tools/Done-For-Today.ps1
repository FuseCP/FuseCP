param(
    [string]$Objective = "Session paused for today; resume from the listed continuation sequence.",
    [switch]$OpenInEditor
)

$ErrorActionPreference = "Stop"

$saveScript = Join-Path $PSScriptRoot "Save-Session-Handoff.ps1"
if (-not (Test-Path $saveScript)) {
    throw "Missing required script: $saveScript"
}

$args = @(
    "-File", $saveScript,
    "-Status", "PAUSED",
    "-Objective", $Objective
)

if ($OpenInEditor) {
    $args += "-OpenInEditor"
}

& pwsh @args
exit $LASTEXITCODE
