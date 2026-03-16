param(
    [ValidateSet("Unit", "Integration", "Package", "Full")]
    [string]$Profile = "Unit",
    [switch]$RequireLegacyMsi
)

$ErrorActionPreference = "Stop"

function Add-Result {
    param(
        [string]$Name,
        [bool]$Passed,
        [string]$Details,
        [string]$Level = "error"
    )

    [pscustomobject]@{
        Check   = $Name
        Status  = if ($Passed) { "PASS" } else { "FAIL" }
        Details = $Details
        Level   = $Level
    }
}

function Test-CommandExists {
    param([string]$CommandName)
    return $null -ne (Get-Command $CommandName -ErrorAction SilentlyContinue)
}

function Test-WebAdministrationModule {
    try {
        $moduleName = & powershell -NoProfile -Command "(Get-Module -ListAvailable -Name WebAdministration | Select-Object -First 1).Name" 2>$null
        return ($moduleName | Out-String).Trim() -eq "WebAdministration"
    }
    catch {
        return $false
    }
}

function Find-FirstExistingPath {
    param([string[]]$CandidatePaths)

    foreach ($path in $CandidatePaths) {
        if (Test-Path $path) {
            return $path
        }
    }

    return $null
}

function Test-WixCaTargetsAvailable {
    $candidatePaths = @(
        "C:\Program Files (x86)\WiX Toolset v3.14\SDK\wix.ca.targets",
        "C:\Program Files (x86)\WiX Toolset v3.11\SDK\wix.ca.targets",
        "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Microsoft\WiX\v3.x\Wix.CA.targets"
    )

    foreach ($path in $candidatePaths) {
        if (Test-Path $path) {
            return $path
        }
    }

    return $null
}

function Test-LegacyMsiExtensionAvailable {
    param([string]$DevEnvPath)

    if ([string]::IsNullOrWhiteSpace($DevEnvPath) -or -not (Test-Path $DevEnvPath)) {
        return $null
    }

    $ideRoot = Split-Path -Parent $DevEnvPath
    $vsRoot = Split-Path -Parent (Split-Path -Parent $ideRoot)

    $candidatePaths = @(
        (Join-Path $vsRoot "Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe"),
        (Join-Path $vsRoot "Common7\IDE\CommonExtensions\Microsoft\VSI\Microsoft.VisualStudio.Deployment.dll"),
        (Join-Path $vsRoot "Common7\IDE\Extensions\Microsoft\Visual Studio Installer Projects\DisableOutOfProcBuild.exe"),
        (Join-Path $vsRoot "Common7\IDE\Extensions\Microsoft\Visual Studio Installer Projects\Microsoft.VisualStudio.Deployment.dll")
    )

    foreach ($path in $candidatePaths) {
        if (Test-Path $path) {
            return $path
        }
    }

    $userExtensionRoot = Join-Path $env:LOCALAPPDATA "Microsoft\VisualStudio"
    if (Test-Path $userExtensionRoot) {
        $userMarker = Get-ChildItem $userExtensionRoot -Recurse -ErrorAction SilentlyContinue |
            Where-Object {
                $_.Name -in @("DisableOutOfProcBuild.exe", "Microsoft.VisualStudio.Deployment.dll")
            } |
            Select-Object -First 1 -ExpandProperty FullName

        if ($userMarker) {
            return $userMarker
        }
    }

    return $null
}

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

$results = @()

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..\..")

$wixHeat = Join-Path $repoRoot "tools\WIX\heat.exe"
$webDeploy = Join-Path $repoRoot "tools\WebDeploy\msdeploy.exe"
$zipExe = Join-Path $repoRoot "tools\7-Zip\7z.exe"
$sqlCmdExe = Join-Path $repoRoot "tools\sqlcmd\SQLCMD.EXE"

$needIntegration = $Profile -in @("Integration", "Full")
$needPackage = $Profile -in @("Package", "Full")

# Unit profile checks
$pwshCommand = Get-Command pwsh -ErrorAction SilentlyContinue
$pwshKnownPath = Find-FirstExistingPath -CandidatePaths @(
    "C:\Program Files\PowerShell\7\pwsh.exe"
)
$hasPwsh = ($null -ne $pwshCommand) -or ($null -ne $pwshKnownPath)

$pwshDetails = "Install PowerShell 7 (pwsh)"
if ($null -ne $pwshCommand) {
    $pwshDetails = "pwsh is available at $($pwshCommand.Source)"
}
elseif ($null -ne $pwshKnownPath) {
    $pwshDetails = "pwsh found at $pwshKnownPath (restart terminal to refresh PATH)"
}
$results += Add-Result -Name "PowerShell 7 (pwsh)" -Passed $hasPwsh -Details $pwshDetails

$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
$dotnetKnownPath = Find-FirstExistingPath -CandidatePaths @(
    "C:\Program Files\dotnet\dotnet.exe"
)
$hasDotnet = ($null -ne $dotnetCommand) -or ($null -ne $dotnetKnownPath)

$dotnetDetails = "Install .NET SDK (CI uses .NET 10)"
if ($null -ne $dotnetCommand) {
    $dotnetDetails = "dotnet is available at $($dotnetCommand.Source)"
}
elseif ($null -ne $dotnetKnownPath) {
    $dotnetDetails = "dotnet found at $dotnetKnownPath (restart terminal to refresh PATH)"
}
$results += Add-Result -Name "dotnet CLI" -Passed $hasDotnet -Details $dotnetDetails

$msbuildCommand = Get-Command msbuild -ErrorAction SilentlyContinue
$msbuildKnownPath = Find-FirstExistingPath -CandidatePaths @(
    "C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)
$hasMsbuild = ($null -ne $msbuildCommand) -or ($null -ne $msbuildKnownPath)

$msbuildDetails = "Install Visual Studio Build Tools / Visual Studio"
if ($null -ne $msbuildCommand) {
    $msbuildDetails = "msbuild is available at $($msbuildCommand.Source)"
}
elseif ($null -ne $msbuildKnownPath) {
    $msbuildDetails = "msbuild found at $msbuildKnownPath (restart terminal to refresh PATH)"
}
$results += Add-Result -Name "MSBuild" -Passed $hasMsbuild -Details $msbuildDetails

if ($needIntegration) {
    $isAdmin = Test-IsAdministrator
    $adminDetails = "Run PowerShell as Administrator for IIS website creation"
    if ($isAdmin) { $adminDetails = "Elevated shell detected" }
    $results += Add-Result -Name "Administrator shell" -Passed $isAdmin -Details $adminDetails -Level "warning"

    $hasWebAdminModule = Test-WebAdministrationModule
    $webAdminDetails = "Enable IIS Management Scripts and Tools"
    if ($hasWebAdminModule) { $webAdminDetails = "Module available (Windows PowerShell)" }
    $results += Add-Result -Name "IIS WebAdministration module" -Passed $hasWebAdminModule -Details $webAdminDetails

    $hasW3Svc = $null -ne (Get-Service -Name "W3SVC" -ErrorAction SilentlyContinue)
    $w3svcDetails = "Install IIS Web Server role"
    if ($hasW3Svc) { $w3svcDetails = "IIS service found" }
    $results += Add-Result -Name "IIS service (W3SVC)" -Passed $hasW3Svc -Details $w3svcDetails

    if (Test-Path $sqlCmdExe) {
        $sqlInstances = @("(local)\SQLExpress", "(local)")
        $connectionErrors = New-Object System.Collections.Generic.List[string]
        $connectedInstance = $null

        foreach ($instance in $sqlInstances) {
            try {
                $sqlOutput = & $sqlCmdExe -S $instance -E -Q "SELECT @@VERSION" 2>&1 | Out-String
                if ($LASTEXITCODE -eq 0) {
                    $connectedInstance = $instance
                    break
                }

                $trimmedOutput = $sqlOutput.Trim()
                if ([string]::IsNullOrWhiteSpace($trimmedOutput)) {
                    $trimmedOutput = "Cannot connect to $instance using integrated auth"
                }

                if ($trimmedOutput.Length -gt 120) {
                    $trimmedOutput = $trimmedOutput.Substring(0, 120) + "..."
                }

                $connectionErrors.Add("[$instance] $trimmedOutput")
            }
            catch {
                $details = $_.Exception.Message
                if ([string]::IsNullOrWhiteSpace($details)) {
                    $details = "Cannot connect to $instance using integrated auth"
                }

                if ($details.Length -gt 120) {
                    $details = $details.Substring(0, 120) + "..."
                }

                $connectionErrors.Add("[$instance] $details")
            }
        }

        if ($null -ne $connectedInstance) {
            $results += Add-Result -Name "SQL Server connectivity" -Passed $true -Details "Connected to $connectedInstance"
        }
        else {
            $errorDetails = ($connectionErrors -join " | ")
            if ([string]::IsNullOrWhiteSpace($errorDetails)) {
                $errorDetails = "Cannot connect to SQL Server instances (local)\\SQLExpress or (local) using integrated auth"
            }

            if ($errorDetails.Length -gt 180) {
                $errorDetails = $errorDetails.Substring(0, 180) + "..."
            }

            $results += Add-Result -Name "SQL Server connectivity" -Passed $false -Details $errorDetails
        }
    }
    else {
        $results += Add-Result -Name "SQL Server connectivity" -Passed $false -Details "Missing tools/sqlcmd/SQLCMD.EXE"
    }
}

if ($needPackage) {
    $hasWixHeat = Test-Path $wixHeat
    $wixDetails = "Missing tools/WIX/heat.exe"
    if ($hasWixHeat) { $wixDetails = $wixHeat }
    $results += Add-Result -Name "Bundled WiX (heat.exe)" -Passed $hasWixHeat -Details $wixDetails

    $hasWebDeploy = Test-Path $webDeploy
    $webDeployDetails = "Missing tools/WebDeploy/msdeploy.exe"
    if ($hasWebDeploy) { $webDeployDetails = $webDeploy }
    $results += Add-Result -Name "Bundled WebDeploy" -Passed $hasWebDeploy -Details $webDeployDetails

    $hasZip = Test-Path $zipExe
    $zipDetails = "Missing tools/7-Zip/7z.exe"
    if ($hasZip) { $zipDetails = $zipExe }
    $results += Add-Result -Name "Bundled 7-Zip" -Passed $hasZip -Details $zipDetails

    $hasSqlCmd = Test-Path $sqlCmdExe
    $sqlCmdDetails = "Missing tools/sqlcmd/SQLCMD.EXE"
    if ($hasSqlCmd) { $sqlCmdDetails = $sqlCmdExe }
    $results += Add-Result -Name "Bundled SQLCMD" -Passed $hasSqlCmd -Details $sqlCmdDetails

    $hasWsl = Test-CommandExists -CommandName "wsl"
    if ($hasWsl) {
        try {
            $wslList = (& cmd /c "wsl --list --quiet 2>nul" | Out-String).Trim()
            $hasDistro = -not [string]::IsNullOrWhiteSpace($wslList)
            $wslDetails = "WSL installed but no distro configured"
            if ($hasDistro) { $wslDetails = "WSL distro(s) detected" }
            $results += Add-Result -Name "WSL availability" -Passed $hasDistro -Details $wslDetails -Level "warning"
        }
        catch {
            $results += Add-Result -Name "WSL availability" -Passed $false -Details "WSL is present but failed to query distro list" -Level "warning"
        }
    }
    else {
        $results += Add-Result -Name "WSL availability" -Passed $false -Details "Install WSL2 if building Linux packages" -Level "warning"
    }

    $wixCaTargetsPath = Test-WixCaTargetsAvailable
    $hasWixCaTargets = $null -ne $wixCaTargetsPath
    $wixCaDetails = "Install WiX v3 MSBuild integration (Wix.CA.targets) for legacy installer projects"
    if ($hasWixCaTargets) {
        $wixCaDetails = "Found at $wixCaTargetsPath"
    }

    $wixCaLevel = if ($RequireLegacyMsi) { "error" } else { "warning" }
    $results += Add-Result -Name "WiX MSBuild targets (Wix.CA.targets)" -Passed $hasWixCaTargets -Details $wixCaDetails -Level $wixCaLevel

    $devEnvPath = Find-FirstExistingPath -CandidatePaths @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\18\Professional\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.com",
        "C:\Program Files\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.com"
    )
    $hasDevEnv = $null -ne $devEnvPath
    $legacyMsiExtensionPath = Test-LegacyMsiExtensionAvailable -DevEnvPath $devEnvPath
    $hasLegacyMsiExtension = $null -ne $legacyMsiExtensionPath
    $hasLegacyMsiToolchain = $hasDevEnv -and $hasLegacyMsiExtension

    if ($RequireLegacyMsi) {
        $details = "Install full Visual Studio + Installer Projects extension for .vdproj MSI build"
        if ($hasLegacyMsiToolchain) {
            $details = "devenv.com found at $devEnvPath; Installer Projects marker found at $legacyMsiExtensionPath"
        }
        elseif ($hasDevEnv) {
            $details = "devenv.com found at $devEnvPath, but no Installer Projects extension marker was found. .vdproj builds may silently no-op until that extension is installed."
        }
        $results += Add-Result -Name "Legacy MSI toolchain (.vdproj)" -Passed $hasLegacyMsiToolchain -Details $details
    }
    else {
        $details = "Optional: full Visual Studio + Installer Projects extension needed only when forcing /p:BuildInstallerMsi=true"
        if ($hasLegacyMsiToolchain) {
            $details = "Optional tooling present at $devEnvPath; Installer Projects marker found at $legacyMsiExtensionPath"
        }
        elseif ($hasDevEnv) {
            $details = "devenv.com found at $devEnvPath, but no Installer Projects extension marker was found"
        }
        $results += Add-Result -Name "Legacy MSI toolchain (.vdproj, optional)" -Passed $hasLegacyMsiToolchain -Details $details -Level "warning"
    }
}

$results | Format-Table -AutoSize

$failed = ($results | Where-Object { $_.Status -eq "FAIL" -and $_.Level -ne "warning" }).Count
if ($failed -gt 0) {
    Write-Host ""
    Write-Host "Profile '$Profile' has $failed failing prerequisite(s)." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Profile '$Profile' prerequisites look good." -ForegroundColor Green
exit 0
