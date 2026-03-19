param(
    [switch]$SkipEnvironmentCheck,
    [switch]$SkipSolutionSyncCheck,
    [switch]$StartWebsites
)

$ErrorActionPreference = "Stop"

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Ensure-ElevatedSession {
    param(
        [string]$Reason = "Start-of-day requires administrator permissions."
    )

    if (Test-IsAdministrator) {
        return
    }

    if ($env:FUSECP_START_OF_DAY_ELEVATED -eq "1") {
        return
    }

    $relaunchArgs = @(
        "-NoProfile",
        "-ExecutionPolicy", "Bypass",
        "-File", $PSCommandPath
    )

    if ($SkipEnvironmentCheck) { $relaunchArgs += "-SkipEnvironmentCheck" }
    if ($SkipSolutionSyncCheck) { $relaunchArgs += "-SkipSolutionSyncCheck" }
    if ($StartWebsites) { $relaunchArgs += "-StartWebsites" }

    Write-Host $Reason -ForegroundColor Yellow
    Write-Host "Requesting Administrator permissions for start-of-day tasks..." -ForegroundColor Yellow

    try {
        $childProcess = Start-Process -FilePath "pwsh" -ArgumentList $relaunchArgs -Verb RunAs -Wait -PassThru -WorkingDirectory (Get-Location) -Environment @{ FUSECP_START_OF_DAY_ELEVATED = "1" }
        exit $childProcess.ExitCode
    }
    catch {
        throw "Unable to start elevated shell. Please approve UAC prompt or run an Administrator shell."
    }
}

function Get-SqlExpressService {
    return Get-Service -Name "MSSQL`$SQLEXPRESS" -ErrorAction SilentlyContinue
}

function Ensure-AdminIfRequired {
    if (Test-IsAdministrator) {
        return
    }

    if ($StartWebsites) {
        Ensure-ElevatedSession -Reason "Current shell is not elevated and website startup may require IIS administrator permissions."
    }

    if ($SkipEnvironmentCheck) {
        return
    }

    $sqlService = Get-SqlExpressService
    if ($null -eq $sqlService) {
        return
    }

    if ($sqlService.Status -ne "Running") {
        Ensure-ElevatedSession -Reason "Current shell is not elevated and SQLExpress is stopped. Elevation is required to start MSSQL`$SQLEXPRESS."
    }
}

function Ensure-SqlExpressRunning {
    $sqlService = Get-SqlExpressService
    if ($null -eq $sqlService) {
        Write-Host "SQLExpress service not found (MSSQL`$SQLEXPRESS)." -ForegroundColor DarkYellow
        return
    }

    if ($sqlService.Status -eq "Running") {
        return
    }

    Write-Host "Starting SQLExpress service (MSSQL`$SQLEXPRESS)..." -ForegroundColor Cyan
    Start-Service -Name "MSSQL`$SQLEXPRESS"
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$checkEnvironmentScript = Join-Path $PSScriptRoot "check-test-environment.ps1"
$checkSolutionSyncScript = Join-Path $PSScriptRoot "check-sln-scope-sync.ps1"
$databaseWorkflowScript = Join-Path $PSScriptRoot "Orchestrate-Database-Workflow.ps1"
$startWebsiteScript = Join-Path $PSScriptRoot "StartWebsite.ps1"

if (-not (Test-Path $checkEnvironmentScript)) {
    throw "Required script not found: $checkEnvironmentScript"
}

if (-not (Test-Path $checkSolutionSyncScript)) {
    throw "Required script not found: $checkSolutionSyncScript"
}

if (-not (Test-Path $databaseWorkflowScript)) {
    throw "Required script not found: $databaseWorkflowScript"
}

if ($StartWebsites -and -not (Test-Path $startWebsiteScript)) {
    throw "Required script not found: $startWebsiteScript"
}

Set-Location $repoRoot

Ensure-AdminIfRequired

if (-not $SkipEnvironmentCheck) {
    Ensure-SqlExpressRunning
}

if (-not $SkipEnvironmentCheck) {
    Write-Host "Running environment check (Profile: Full)..." -ForegroundColor Cyan
    & pwsh -NoProfile -ExecutionPolicy Bypass -File $checkEnvironmentScript -Profile Full
    if ($LASTEXITCODE -ne 0) {
        throw "Environment check failed. Resolve prerequisites before continuing."
    }
}
else {
    Write-Host "Skipping environment check." -ForegroundColor DarkYellow
}

if (-not $SkipSolutionSyncCheck) {
    Write-Host "Running solution sync check..." -ForegroundColor Cyan
    & pwsh -NoProfile -ExecutionPolicy Bypass -File $checkSolutionSyncScript
    if ($LASTEXITCODE -ne 0) {
        throw "Solution sync check failed. Align FuseCP.sln with scope solutions before continuing."
    }
}
else {
    Write-Host "Skipping solution sync check." -ForegroundColor DarkYellow
}

Write-Host "Running database workflow quick check..." -ForegroundColor Cyan
& pwsh -NoProfile -ExecutionPolicy Bypass -File $databaseWorkflowScript -Mode Quick
if ($LASTEXITCODE -ne 0) {
    throw "Database workflow quick check failed. Resolve workflow issues before continuing."
}

if ($StartWebsites) {
    Write-Host "Starting local websites..." -ForegroundColor Cyan
    & pwsh -NoProfile -ExecutionPolicy Bypass -File $startWebsiteScript
}

Write-Host "Start-of-day routine completed." -ForegroundColor Green
