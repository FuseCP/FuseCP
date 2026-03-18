param(
    [ValidateSet("Portal", "Enterprise", "Server", "Shared")]
    [string[]]$Scope = @("Shared"),
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    [switch]$IncludeTests,
    [switch]$UseOrchestratedBuild,
    [switch]$ChangedOnly,
    [switch]$SkipIfNoChanges,
    [string]$BaseRef = "HEAD",
    [switch]$DisableNuGetAudit,
    [switch]$NoRestore,
    [switch]$SkipEfWorkflowGuards,
    [switch]$SkipDatabaseWorkflow,
    [string]$JsonOutputPath,
    [string]$ScopeMapPath
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Invoke-Step {
    param(
        [string]$Name,
        [string]$DisplayCommand,
        [string[]]$CommandArgs,
        [scriptblock]$Action,
        [switch]$ContinueOnError
    )

    Write-Host "Running: $Name" -ForegroundColor Yellow
    if (-not [string]::IsNullOrWhiteSpace($DisplayCommand)) {
        $script:ExecutedCommands += $DisplayCommand
    }

    if ($null -ne $CommandArgs -and $CommandArgs.Count -gt 0) {
        $script:ExecutedArgs += ,$CommandArgs
    }

    & $Action
    if ($LASTEXITCODE -ne 0) {
        if ($ContinueOnError) {
            Write-Host "  (continuing despite error)" -ForegroundColor Yellow
        } else {
            throw "Step failed: $Name"
        }
    }
}

function Build-DotnetCommandDisplay {
    param([string[]]$CommandParts)

    return "dotnet " + ($CommandParts -join " ")
}

function Normalize-RelativePath {
    param([string]$Path)

    return ($Path -replace "\\", "/").ToLowerInvariant()
}

function Get-ChangedFilesFromGit {
    param(
        [string]$RepoRoot,
        [string]$BaseRef
    )

    if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
        throw "git is required for -ChangedOnly mode"
    }

    $trackedChanged = @(& git -C $RepoRoot diff --name-only $BaseRef -- . 2>$null)
    if ($LASTEXITCODE -ne 0) {
        throw "Unable to resolve changed files from git diff against '$BaseRef'."
    }

    $untracked = @(& git -C $RepoRoot ls-files --others --exclude-standard 2>$null)
    if ($LASTEXITCODE -ne 0) {
        $untracked = @()
    }

    $all = @($trackedChanged + $untracked) |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { Normalize-RelativePath $_ } |
        Sort-Object -Unique

    return @($all)
}

function Resolve-ScopesFromPaths {
    param(
        [string[]]$ChangedPaths,
        [hashtable]$ScopeMap
    )

    $scopes = New-Object System.Collections.Generic.HashSet[string]

    if ($null -eq $ScopeMap) {
        throw "Scope map is required for scope resolution."
    }

    foreach ($path in $ChangedPaths) {
        $matched = $false
        foreach ($scopeName in $ScopeMap.Keys) {
            $patterns = @($ScopeMap[$scopeName])
            foreach ($pattern in $patterns) {
                if ($path.StartsWith($pattern)) {
                    [void]$scopes.Add($scopeName)
                    $matched = $true
                    break
                }
            }

            if ($matched) {
                break
            }
        }

        if (-not $matched) {
            [void]$scopes.Add("Shared")
        }
    }

    return @($scopes)
}

function Get-DefaultScopeMap {
    return @{
        Portal = @(
            "fusecp/sources/fusecp.webportal/",
            "fusecp/sources/fusecp.webdavportal/",
            "languages/"
        )
        Enterprise = @(
            "fusecp/sources/fusecp.enterpriseserver",
            "fusecp/sources/fusecp.web.services/",
            "fusecp/sources/fusecp.common.utils/"
        )
        Server = @(
            "fusecp/sources/fusecp.server/",
            "fusecp/sources/fusecp.providers.",
            "fusecp.hyperv.utils/",
            "fusecp.vmconfig/"
        )
    }
}

function Merge-ScopeMaps {
    param(
        [hashtable]$DefaultMap,
        [hashtable]$OverrideMap
    )

    $merged = @{}

    foreach ($key in $DefaultMap.Keys) {
        $merged[$key] = @($DefaultMap[$key])
    }

    if ($null -ne $OverrideMap) {
        foreach ($key in $OverrideMap.Keys) {
            if ($merged.ContainsKey($key)) {
                $merged[$key] = @($merged[$key] + @($OverrideMap[$key]) | Sort-Object -Unique)
            }
            else {
                $merged[$key] = @($OverrideMap[$key] | Sort-Object -Unique)
            }
        }
    }

    return $merged
}

function Get-ScopeMapFromConfig {
    param([string]$ConfigPath)

    if ([string]::IsNullOrWhiteSpace($ConfigPath)) {
        return $null
    }

    $resolvedPath = $ConfigPath
    if (-not [System.IO.Path]::IsPathRooted($resolvedPath)) {
        $resolvedPath = Join-Path $repoRoot $resolvedPath
    }

    if (-not (Test-Path $resolvedPath)) {
        throw "Scope map config not found: $resolvedPath"
    }

    $raw = Get-Content -Path $resolvedPath -Raw
    if ([string]::IsNullOrWhiteSpace($raw)) {
        return $null
    }

    $parsed = $raw | ConvertFrom-Json -AsHashtable
    if ($null -eq $parsed) {
        return $null
    }

    $allowedScopes = @("Portal", "Enterprise", "Server", "Shared")
    $map = @{}
    foreach ($scopeName in $allowedScopes) {
        if ($parsed.ContainsKey($scopeName) -and $null -ne $parsed[$scopeName]) {
            $map[$scopeName] = @($parsed[$scopeName] | ForEach-Object { Normalize-RelativePath $_ })
        }
    }

    return $map
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$fuseCpDir = Resolve-Path (Join-Path $scriptDir "..")
$repoRoot = Resolve-Path (Join-Path $fuseCpDir "..")
$sourcesDir = Join-Path $fuseCpDir "Sources"
$toolsDir = $scriptDir
$script:ExecutedCommands = @()
$script:ExecutedArgs = @()
$scopeExplicitlyProvided = $PSBoundParameters.ContainsKey("Scope")
$runStart = Get-Date
$changedFiles = @()
$defaultScopeMap = Get-DefaultScopeMap
$configScopeMap = Get-ScopeMapFromConfig -ConfigPath $ScopeMapPath
$scopeMap = Merge-ScopeMaps -DefaultMap $defaultScopeMap -OverrideMap $configScopeMap
$skipBuild = $false
$noChangesDetected = $false

if ($ChangedOnly) {
    Write-Step "Detecting changed files"
    $changedFiles = Get-ChangedFilesFromGit -RepoRoot $repoRoot -BaseRef $BaseRef
    Write-Host "Changed files detected: $($changedFiles.Count) (base: $BaseRef)" -ForegroundColor Gray

    if ($changedFiles.Count -gt 0) {
        $derivedScope = Resolve-ScopesFromPaths -ChangedPaths $changedFiles -ScopeMap $scopeMap

        if ($scopeExplicitlyProvided) {
            $Scope = @($Scope + $derivedScope | Sort-Object -Unique)
            Write-Host "Scope merged from explicit + changed files." -ForegroundColor Gray
        }
        else {
            $Scope = @($derivedScope | Sort-Object -Unique)
            Write-Host "Scope derived from changed files." -ForegroundColor Gray
        }
    }
    elseif (-not $scopeExplicitlyProvided) {
        $noChangesDetected = $true
        if ($SkipIfNoChanges) {
            $Scope = @()
            $skipBuild = $true
            Write-Host "No changed files detected; skipping build due to -SkipIfNoChanges." -ForegroundColor Gray
        }
        else {
            $Scope = @("Shared")
            Write-Host "No changed files detected; falling back to Shared scope." -ForegroundColor Gray
        }
    }
}

Write-Step "Local validation"
if ($Scope.Count -gt 0) {
    Write-Host "Scope: $($Scope -join ', ')" -ForegroundColor Gray
}
else {
    Write-Host "Scope: <none>" -ForegroundColor Gray
}
Write-Host "Configuration: $Configuration" -ForegroundColor Gray
if ($DisableNuGetAudit) {
    Write-Host "NuGet audit warnings: disabled for this run (-DisableNuGetAudit)" -ForegroundColor Gray
}
if ($NoRestore) {
    Write-Host "Restore mode: disabled for scoped dotnet commands (-NoRestore)" -ForegroundColor Gray
}

$runOrchestrated = -not $skipBuild -and ($UseOrchestratedBuild -or ($Scope -contains "Shared"))

$resultState = "Succeeded"
$resultError = $null
$requestedScope = @()
if ($PSBoundParameters.ContainsKey("Scope")) {
    $requestedScope = @($PSBoundParameters["Scope"])
}

try {
    if (-not $SkipDatabaseWorkflow) {
        $dbOrchestratorScript = Join-Path $toolsDir "Orchestrate-Database-Workflow.ps1"
        if (Test-Path $dbOrchestratorScript) {
            Invoke-Step -Name "Database workflow automation" -DisplayCommand "pwsh -NoProfile -File FuseCP/Tools/Orchestrate-Database-Workflow.ps1 -Mode Full" -CommandArgs @("pwsh", "-NoProfile", "-File", $dbOrchestratorScript, "-Mode", "Full") -Action {
                & pwsh -NoProfile -File $dbOrchestratorScript -Mode Full
            } -ContinueOnError
        }
    }

    if ($skipBuild) {
        Write-Host "Skipping build execution." -ForegroundColor Gray
    }
    elseif ($runOrchestrated) {
        if ($NoRestore) {
            Write-Host "-NoRestore is ignored for orchestrated build.xml mode." -ForegroundColor DarkYellow
        }

        Push-Location $fuseCpDir
        try {
            $msbuildArgs = @("msbuild", "build.xml", "/target:Build", "/p:BuildConfiguration=$Configuration", "/v:m", "/m:1")
            if ($DisableNuGetAudit) {
                $msbuildArgs += "/p:NuGetAudit=false"
            }

            Invoke-Step -Name "Ordered build (build.xml)" -DisplayCommand (Build-DotnetCommandDisplay -CommandParts $msbuildArgs) -CommandArgs $msbuildArgs -Action {
                dotnet @msbuildArgs
            }
        }
        finally {
            Pop-Location
        }
    }
    else {
        Push-Location $sourcesDir
        try {
            $scopeSet = New-Object System.Collections.Generic.HashSet[string] ([System.StringComparer]::OrdinalIgnoreCase)
            foreach ($scopeName in $Scope) {
                if (-not [string]::IsNullOrWhiteSpace($scopeName)) {
                    [void]$scopeSet.Add($scopeName)
                }
            }

            if ($scopeSet.Contains("Portal") -and $scopeSet.Contains("Enterprise")) {
                Write-Host "Portal scope already includes Enterprise build; skipping redundant Enterprise-only solution build." -ForegroundColor DarkYellow
                [void]$scopeSet.Remove("Enterprise")
            }

            if ($scopeSet.Contains("Portal")) {
                $portalArgs = @("build", "FuseCP.WebPortalAndEnterpriseServer.sln", "--configuration", $Configuration)
                if ($NoRestore) {
                    $portalArgs += "--no-restore"
                }
                if ($DisableNuGetAudit) {
                    $portalArgs += "-p:NuGetAudit=false"
                }

                Invoke-Step -Name "Build Portal + Enterprise solution" -DisplayCommand (Build-DotnetCommandDisplay -CommandParts $portalArgs) -CommandArgs $portalArgs -Action {
                    dotnet @portalArgs
                }
            }

            if ($scopeSet.Contains("Enterprise")) {
                $enterpriseArgs = @("build", "FuseCP.EnterpriseServer.sln", "--configuration", $Configuration)
                if ($NoRestore) {
                    $enterpriseArgs += "--no-restore"
                }
                if ($DisableNuGetAudit) {
                    $enterpriseArgs += "-p:NuGetAudit=false"
                }

                Invoke-Step -Name "Build Enterprise solution" -DisplayCommand (Build-DotnetCommandDisplay -CommandParts $enterpriseArgs) -CommandArgs $enterpriseArgs -Action {
                    dotnet @enterpriseArgs
                }
            }

            if ($scopeSet.Contains("Server")) {
                $serverArgs = @("build", "FuseCP.Server.sln", "--configuration", $Configuration)
                if ($NoRestore) {
                    $serverArgs += "--no-restore"
                }
                if ($DisableNuGetAudit) {
                    $serverArgs += "-p:NuGetAudit=false"
                }

                Invoke-Step -Name "Build Server solution" -DisplayCommand (Build-DotnetCommandDisplay -CommandParts $serverArgs) -CommandArgs $serverArgs -Action {
                    dotnet @serverArgs
                }
            }
        }
        finally {
            Pop-Location
        }
    }

    if ($IncludeTests) {
        Push-Location $sourcesDir
        try {
            $testBuildArgs = @("build", "FuseCP.Tests.sln", "--configuration", $Configuration)
            if ($NoRestore) {
                $testBuildArgs += "--no-restore"
            }
            if ($DisableNuGetAudit) {
                $testBuildArgs += "-p:NuGetAudit=false"
            }

            Invoke-Step -Name "Build tests" -DisplayCommand (Build-DotnetCommandDisplay -CommandParts $testBuildArgs) -CommandArgs $testBuildArgs -Action {
                dotnet @testBuildArgs
            }

            $testRunArgs = @("test", "FuseCP.Tests.sln", "--configuration", $Configuration, "--no-build", "-v", "n")
            if ($NoRestore) {
                $testRunArgs += "--no-restore"
            }
            if ($DisableNuGetAudit) {
                $testRunArgs += "-p:NuGetAudit=false"
            }

            Invoke-Step -Name "Run tests" -DisplayCommand (Build-DotnetCommandDisplay -CommandParts $testRunArgs) -CommandArgs $testRunArgs -Action {
                dotnet @testRunArgs
            }
        }
        finally {
            Pop-Location
        }
    }
}
catch {
    $resultState = "Failed"
    $resultError = $_.Exception.Message
    throw
}
finally {
    if ($script:ExecutedCommands.Count -gt 0) {
        Write-Step "Executed command summary"
        foreach ($command in $script:ExecutedCommands) {
            Write-Host "- $command" -ForegroundColor Gray
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($JsonOutputPath)) {
        $resolvedJsonPath = $JsonOutputPath
        if (-not [System.IO.Path]::IsPathRooted($resolvedJsonPath)) {
            $resolvedJsonPath = Join-Path $repoRoot $resolvedJsonPath
        }

        $jsonDirectory = Split-Path -Parent $resolvedJsonPath
        if (-not [string]::IsNullOrWhiteSpace($jsonDirectory) -and -not (Test-Path $jsonDirectory)) {
            New-Item -Path $jsonDirectory -ItemType Directory -Force | Out-Null
        }

        $summary = [pscustomobject]@{
            status = $resultState
            error = $resultError
            startedAt = $runStart.ToString("o")
            finishedAt = (Get-Date).ToString("o")
            configuration = $Configuration
            changedOnly = [bool]$ChangedOnly
            skipIfNoChanges = [bool]$SkipIfNoChanges
            noChangesDetected = [bool]$noChangesDetected
            skippedBuild = [bool]$skipBuild
            baseRef = $BaseRef
            includeTests = [bool]$IncludeTests
            useOrchestratedBuild = [bool]$UseOrchestratedBuild
            resolvedOrchestratedBuild = [bool]$runOrchestrated
            disableNuGetAudit = [bool]$DisableNuGetAudit
            noRestore = [bool]$NoRestore
            requestedScope = @($requestedScope)
            resolvedScope = @($Scope)
            changedFiles = @($changedFiles)
            executedCommands = @($script:ExecutedCommands)
        }

        $summary | ConvertTo-Json -Depth 8 | Set-Content -Path $resolvedJsonPath -Encoding UTF8
        Write-Host "JSON summary written: $resolvedJsonPath" -ForegroundColor Gray
    }
}

Write-Host "`nValidation completed successfully." -ForegroundColor Green
exit 0




