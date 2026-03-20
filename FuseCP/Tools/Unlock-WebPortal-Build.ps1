param(
    [switch]$RunBuild,
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$KillAllW3wp
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$webPortalBin = Join-Path $repoRoot "FuseCP\Sources\FuseCP.WebPortal\bin_dotnet"
$lockedTargets = @(
    (Join-Path $webPortalBin "FuseCP.WebPortal.dll")
    (Join-Path $webPortalBin "FuseCP.Portal.Modules.dll")
)

Write-Step "Checking IIS worker processes"
$w3wpProcesses = @(Get-Process -Name "w3wp" -ErrorAction SilentlyContinue)
if ($w3wpProcesses.Count -eq 0) {
    Write-Host "No running w3wp processes found."
} else {
    Write-Host "Found w3wp processes: $($w3wpProcesses.Id -join ', ')"
}

if ($w3wpProcesses.Count -gt 0) {
    Write-Step "Stopping potential lock holders"
    if ($KillAllW3wp) {
        $w3wpProcesses | Stop-Process -Force
        Write-Host "Stopped all running w3wp processes."
    } else {
        # Default behavior still stops all w3wp because lock attribution is not reliable across environments.
        $w3wpProcesses | Stop-Process -Force
        Write-Host "Stopped all running w3wp processes (default lock-safe behavior)."
    }
}

Write-Step "Checking target output files"
foreach ($target in $lockedTargets) {
    if (Test-Path $target) {
        Write-Host "Target present: $target"
    } else {
        Write-Host "Target not present yet: $target"
    }
}

if ($RunBuild) {
    Write-Step "Running Portal Modules build"
    Push-Location $repoRoot
    try {
        dotnet build "FuseCP/Sources/FuseCP.WebPortal/DesktopModules/FuseCP/FuseCP.Portal.Modules.csproj" -c $Configuration -v minimal
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed with exit code $LASTEXITCODE"
        }
    }
    finally {
        Pop-Location
    }

    Write-Host "Build succeeded." -ForegroundColor Green
} else {
    Write-Host "Unlock completed. Use -RunBuild to build immediately after unlock."
}
