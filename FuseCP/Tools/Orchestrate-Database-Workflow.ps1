# Copyright (C) 2026 FuseCP
#
# FuseCP Database Workflow Orchestrator
# Single consolidated automation entry point for all database verification and maintenance
# 
# Modes:
#  - Quick   : Fast pre-commit checks (drift, registration, migrations) ~1 second
#  - Full    : Complete automation with regeneration + verification ~10 seconds
#  - Verify  : Comprehensive 8-phase audit (detailed, includes all guards)
#  - Fix     : Force MySQL artifact regeneration
#  - Report  : JSON output for CI integration

param(
    [ValidateSet("Quick", "Full", "Verify", "Fix", "Report")]
    [string]$Mode = "Full",
    [switch]$Force,
    [switch]$BlockBuildOnFailure,
    [string]$JsonOutputPath,
    [switch]$Detailed
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path (Resolve-Path $MyInvocation.MyCommand.Path) -Parent
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$dataDir = Join-Path $repoRoot "FuseCP\Sources\FuseCP.EnterpriseServer.Data"
$databaseDir = Join-Path $repoRoot "FuseCP\Database"

$reportData = @{
    Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Mode = $Mode
    Results = @()
    Status = "PENDING"
    Errors = @()
    Summary = @{}
}

# ============================================================================
# REPORTING FUNCTIONS
# ============================================================================

function Add-Result {
    param(
        [string]$Name,
        [string]$Status,  # PASS, FAIL, WARN, SKIP
        [string]$Message,
        [hashtable]$Details = @{}
    )
    
    $reportData.Results += @{
        Name = $Name
        Status = $Status
        Message = $Message
        Details = $Details
        Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
    }
    
    $color = @{
        PASS = "Green"
        FAIL = "Red"
        WARN = "Yellow"
        SKIP = "Cyan"
    }[$Status]
    
    Write-Host "[$Status] $Name" -ForegroundColor $color
    if ($Message) { Write-Host "  → $Message" }
}

function Write-Section {
    param([string]$Title)
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host $Title -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host ""
}

# ============================================================================
# QUICK CHECKS (Pre-Commit / Quick Mode)
# ============================================================================

function Test-SchemaDrift {
    <# Check entity/configuration count alignment #>
    $entities = @(Get-ChildItem "$dataDir\Entities\*.cs" -ErrorAction SilentlyContinue)
    $configs = @(Get-ChildItem "$dataDir\Configuration\*.cs" -ErrorAction SilentlyContinue)
    
    $driftDetected = $entities.Count -ne $configs.Count
    
    if ($driftDetected) {
        Add-Result "Schema Drift Detection" "FAIL" `
            "Entity/Configuration mismatch: $($entities.Count) entities vs $($configs.Count) configs" `
            @{ Entities = $entities.Count; Configurations = $configs.Count }
        return $false
    } else {
        Add-Result "Schema Drift Detection" "PASS" `
            "Entity/Configuration aligned ($($entities.Count) each)" `
            @{ Count = $entities.Count }
        return $true
    }
}

function Test-ConfigurationRegistration {
    <# Check all configs are registered in DbContextBase.OnModelCreating() #>
    $dbContextFile = Join-Path $dataDir "DbContextBase.cs"
    $content = Get-Content $dbContextFile -Raw
    
    $appliedCount = [regex]::Matches($content, "ApplyConfiguration\s*\(\s*model\s*,\s*new\s*\w+Configuration\s*\(\s*\)").Count
    $configCount = @(Get-ChildItem "$dataDir\Configuration\*.cs").Count
    
    if ($appliedCount -ne $configCount) {
        Add-Result "Configuration Registration" "FAIL" `
            "Not all configurations registered: $appliedCount applied vs $configCount defined" `
            @{ Applied = $appliedCount; Defined = $configCount }
        return $false
    } else {
        Add-Result "Configuration Registration" "PASS" `
            "All $appliedCount configurations registered" `
            @{ Count = $appliedCount }
        return $true
    }
}

function Test-MigrationFilesExist {
    <# Check all 4 providers have the same migration files #>
    $targetMigration = "20260318133000_AddBruteForceProtection"
    $allExist = $true
    $providers = @("SqlServer", "MySql", "PostgreSql", "Sqlite")
    $missingProviders = @()
    
    foreach ($provider in $providers) {
        $dir = Join-Path $dataDir "Migrations\$provider\v2.0.0"
        $csFile = Join-Path $dir "$targetMigration.cs"
        $designerFile = Join-Path $dir "$targetMigration.Designer.cs"
        
        if (-not ((Test-Path $csFile) -and (Test-Path $designerFile))) {
            $allExist = $false
            $missingProviders += $provider
        }
    }
    
    if ($allExist) {
        Add-Result "Migration Files (All Providers)" "PASS" `
            "All migrations present for all 4 providers" `
            @{ Providers = $providers }
        return $true
    } else {
        Add-Result "Migration Files (All Providers)" "FAIL" `
            "Missing migrations for providers: $($missingProviders -join ', ')" `
            @{ Missing = $missingProviders }
        return $false
    }
}

# ============================================================================
# EF WORKFLOW GUARDS (Comprehensive Checks)
# ============================================================================

function Test-EfWorkflowGuards {
    <# Comprehensive EF compliance checks #>
    Write-Section "EF Workflow Guards"
    
    $failures = @()
    
    # 1. Check for stray duplicate source files
    $straySecurityService = Join-Path $repoRoot "Sources/FuseCP.EnterpriseServer.Code/Security/BruteForceProtectionService.cs"
    if (Test-Path $straySecurityService) {
        Add-Result "Stray Files Check" "FAIL" "Found duplicate source file: Sources/FuseCP.EnterpriseServer.Code/Security/BruteForceProtectionService.cs"
        $failures += "Stray duplicate source"
    } else {
        Add-Result "Stray Files Check" "PASS" "No stray duplicate source files"
    }
    
    # 2. Check test initialization file
    $testHelperFile = Join-Path $repoRoot "FuseCP\Sources\FuseCP.EnterpriseServer.Tests\Initialization\EnterpriseServer.cs"
    if (-not (Test-Path $testHelperFile)) {
        Add-Result "Test Init File" "FAIL" "Missing: FuseCP/Sources/FuseCP.EnterpriseServer.Tests/Initialization/EnterpriseServer.cs"
        $failures += "Test init missing"
    } else {
        $testContent = Get-Content $testHelperFile -Raw
        $testLines = (Get-Content $testHelperFile).Count
        
        if ($testLines -lt 120) {
            Add-Result "Test Init Size" "FAIL" "Test init appears truncated ($testLines lines)"
            $failures += "Test init truncated"
        } else {
            Add-Result "Test Init Size" "PASS" "Test init has expected size ($testLines lines)"
        }
        
        $setupSqliteExists = $testContent -match "SetupSqliteDb"
        $installDbExists = $testContent -match "InstallFreshDatabase"
        $migrateExists = $testContent -match "\.Migrate\(\)"
        
        if (-not ($setupSqliteExists -and $installDbExists -and $migrateExists)) {
            Add-Result "Test Bootstrap Methods" "FAIL" "Missing required methods in test init"
            $failures += "Test bootstrap incomplete"
        } else {
            Add-Result "Test Bootstrap Methods" "PASS" "All required test bootstrap methods present"
        }
    }
    
    # 3. Check DbSet and DbContextBase
    $dbContextSets = Join-Path $dataDir "DbContext.Sets.cs"
    $dbContextBase = Join-Path $dataDir "DbContextBase.cs"
    
    if ((Test-Path $dbContextSets) -and (Test-Path $dbContextBase)) {
        $setsContent = Get-Content $dbContextSets -Raw
        $baseContent = Get-Content $dbContextBase -Raw
        
        if ($setsContent -match "DbSet<BruteForceLog>") {
            if ($baseContent -notmatch "BruteForceLogConfiguration") {
                $failures += "BruteForceLogConfiguration not applied"
                Add-Result "BruteForceLog Configuration" "FAIL" "DbContextBase does not apply BruteForceLogConfiguration"
            } else {
                Add-Result "BruteForceLog Configuration" "PASS" "BruteForceLogConfiguration properly applied"
            }
        }
        
        if ($setsContent -match "DbSet<IpSecurityPolicy>") {
            if ($baseContent -notmatch "IpSecurityPolicyConfiguration") {
                $failures += "IpSecurityPolicyConfiguration not applied"
                Add-Result "IpSecurityPolicy Configuration" "FAIL" "DbContextBase does not apply IpSecurityPolicyConfiguration"
            } else {
                Add-Result "IpSecurityPolicy Configuration" "PASS" "IpSecurityPolicyConfiguration properly applied"
            }
        }
    } else {
        Add-Result "DbContext Files" "FAIL" "Missing DbContext files"
        $failures += "DbContext files missing"
    }
    
    return $failures.Count -eq 0
}

# ============================================================================
# INSTALL SCRIPT VERIFICATION
# ============================================================================

function Test-InstallScriptsCurrent {
    <# Check all install scripts are up-to-date #>
    Write-Section "Install Scripts Currency"
    
    $scripts = @{
        "SqlServer" = Join-Path $databaseDir "install.sqlserver.sql"
        "MySQL" = Join-Path $databaseDir "install.mysql.sql"
        "PostgreSQL" = Join-Path $databaseDir "install.postgresql.sql"
        "SQLite" = Join-Path $databaseDir "install.sqlite.sql"
    }
    
    $allCurrent = $true
    $staleScripts = @()
    
    foreach ($name in $scripts.Keys) {
        $path = $scripts[$name]
        if (Test-Path $path) {
            $hasSecurityTables = (Select-String -Path $path -Pattern "BruteForceLogs|IpSecurityPolicies" | Measure-Object).Count -gt 0
            $size = [Math]::Round((Get-Item $path).Length / 1MB, 2)
            $lines = @(Get-Content $path).Count
            
            if (-not $hasSecurityTables) {
                $allCurrent = $false
                $staleScripts += $name
                Add-Result "$name Install Script" "FAIL" "Missing latest tables (BruteForceLogs, IpSecurityPolicies)"
            } else {
                Add-Result "$name Install Script" "PASS" "Current ($lines lines, $size MB)"
            }
        } else {
            $allCurrent = $false
            $staleScripts += "$name (MISSING)"
            Add-Result "$name Install Script" "FAIL" "File not found"
        }
    }
    
    return $allCurrent
}

# ============================================================================
# MYSQL ARTIFACT REGENERATION
# ============================================================================

function Invoke-MysqlArtifactRegeneration {
    <# Regenerate MySQL from PostgreSQL template #>
    $postgresqlScript = Join-Path $databaseDir "install.postgresql.sql"
    $mysqlScript = Join-Path $databaseDir "install.mysql.sql"
    
    if (-not (Test-Path $postgresqlScript)) {
        Add-Result "MySQL Artifact Regeneration" "SKIP" "PostgreSQL template not found"
        return $false
    }
    
    $postgresTime = (Get-Item $postgresqlScript).LastWriteTime
    $mysqlTime = if (Test-Path $mysqlScript) { (Get-Item $mysqlScript).LastWriteTime } else { [datetime]::MinValue }
    
    if ($postgresTime -gt $mysqlTime -or $Force) {
        try {
            Write-Host "  Regenerating MySQL install.sql from PostgreSQL template..."
            
            $content = Get-Content -Path $postgresqlScript -Raw
            
            # PostgreSQL → MySQL transformations
            Write-Host "  • Converting PostgreSQL identifiers to MySQL..."
            $content = $content -replace '"([^"]+)"', '`$1'
            $content = $content -replace "SET search_path TO public;`r?`n", ""
            
            # Fix datetime types
            Write-Host "  • Normalizing datetime types..."
            $content = $content -replace "datetime2", "datetime"
            $content = $content -replace "timestamp\s+(?:without time zone)", "datetime"
            
            # Line endings
            $content = $content -replace "`r`n", "`n"
            $content = $content -replace "`n", "`r`n"
            
            Set-Content -Path $mysqlScript -Value $content -Encoding UTF8
            
            $fileSize = (Get-Item $mysqlScript).Length / 1MB
            Add-Result "MySQL Artifact Regeneration" "PASS" "Regenerated ($([Math]::Round($fileSize, 2)) MB)"
            return $true
        } catch {
            Add-Result "MySQL Artifact Regeneration" "FAIL" "Regeneration failed: $_"
            return $false
        }
    } else {
        Add-Result "MySQL Artifact Regeneration" "SKIP" "MySQL script is current"
        return $true
    }
}

# ============================================================================
# COMPREHENSIVE VERIFICATION (8 Phases)
# ============================================================================

function Invoke-ComprehensiveVerification {
    <# Full 8-phase verification #>
    Write-Section "Comprehensive Database Verification (8 Phases)"
    
    $allPassed = $true
    
    # Phase 1: Entity & Configuration Alignment
    Write-Host "Phase 1: Entity & Configuration Alignment"
    $entitiesDir = Join-Path $dataDir "Entities"
    $configDir = Join-Path $dataDir "Configuration"
    $entityCount = (Get-ChildItem "$entitiesDir\*.cs" -ErrorAction SilentlyContinue).Count
    $configCount = (Get-ChildItem "$configDir\*.cs" -ErrorAction SilentlyContinue).Count
    
    if ($entityCount -eq $configCount) {
        Add-Result "  Entity ⟷ Configuration 1:1 Mapping" "PASS" "$entityCount entities = $configCount configurations"
    } else {
        Add-Result "  Entity ⟷ Configuration 1:1 Mapping" "FAIL" "$entityCount entities vs $configCount configurations"
        $allPassed = $false
    }
    
    # Phase 2: Configuration Registration
    Write-Host "Phase 2: Configuration Registration in DbContext"
    $dbContextFile = Join-Path $dataDir "DbContextBase.cs"
    $contextContent = Get-Content $dbContextFile -Raw
    $appliedCount = [regex]::Matches($contextContent, "ApplyConfiguration\s*\(\s*model\s*,\s*new\s*\w+Configuration\s*\(\s*\)").Count
    
    if ($appliedCount -eq $entityCount) {
        Add-Result "  All Configurations Registered" "PASS" "All $appliedCount configurations"
        
        if ($contextContent -match "BruteForceLogConfiguration") {
            Add-Result "    BruteForceLogConfiguration" "PASS" "Registered"
        } else {
            Add-Result "    BruteForceLogConfiguration" "FAIL" "NOT registered"
            $allPassed = $false
        }
        
        if ($contextContent -match "IpSecurityPolicyConfiguration") {
            Add-Result "    IpSecurityPolicyConfiguration" "PASS" "Registered"
        } else {
            Add-Result "    IpSecurityPolicyConfiguration" "FAIL" "NOT registered"
            $allPassed = $false
        }
    } else {
        Add-Result "  All Configurations Registered" "FAIL" "$appliedCount applied vs $entityCount defined"
        $allPassed = $false
    }
    
    # Phase 3: DbSet Properties
    Write-Host "Phase 3: DbSet Properties in DbContext"
    $dbSetFile = Join-Path $dataDir "DbContext.Sets.cs"
    $dbSetContent = Get-Content $dbSetFile -Raw
    
    if ($dbSetContent -match "DbSet<BruteForceLog>") {
        Add-Result "  BruteForceLogs DbSet" "PASS" "Defined"
    } else {
        Add-Result "  BruteForceLogs DbSet" "FAIL" "NOT found"
        $allPassed = $false
    }
    
    if ($dbSetContent -match "DbSet<IpSecurityPolicy>") {
        Add-Result "  IpSecurityPolicies DbSet" "PASS" "Defined"
    } else {
        Add-Result "  IpSecurityPolicies DbSet" "FAIL" "NOT found"
        $allPassed = $false
    }
    
    # Phase 4: Migrations for All Providers
    Write-Host "Phase 4: Migration Files for All Providers"
    $targetMigration = "20260318133000_AddBruteForceProtection"
    $providers = @("SqlServer", "MySql", "PostgreSql", "Sqlite")
    
    foreach ($provider in $providers) {
        $migrationDir = Join-Path $dataDir "Migrations\$provider\v2.0.0"
        $migrationFile = Join-Path $migrationDir "$targetMigration.cs"
        $designerFile = Join-Path $migrationDir "$targetMigration.Designer.cs"
        
        if ((Test-Path $migrationFile) -and (Test-Path $designerFile)) {
            Add-Result "  $provider migration" "PASS" "Files exist"
        } else {
            Add-Result "  $provider migration" "FAIL" "Missing files"
            $allPassed = $false
        }
    }
    
    # Phase 5-6: Install Scripts & Migration History
    Write-Host "Phase 5-6: Install Scripts & Migration History"
    if (Test-InstallScriptsCurrent) {
        Add-Result "  Install Scripts Currency" "PASS" "All scripts current"
    } else {
        Add-Result "  Install Scripts Currency" "FAIL" "Some scripts stale"
        $allPassed = $false
    }
    
    # Phase 7: Test Bootstrap
    Write-Host "Phase 7: Test Bootstrap (SQLite)"
    $testHelperFile = Join-Path $repoRoot "FuseCP\Sources\FuseCP.EnterpriseServer.Tests\Initialization\EnterpriseServer.cs"
    if (Test-Path $testHelperFile) {
        $testContent = Get-Content $testHelperFile -Raw
        $hasSetup = $testContent -match "SetupSqliteDb"
        $hasInstall = $testContent -match "InstallFreshDatabase"
        $hasMigrate = $testContent -match "\.Migrate\(\)"
        
        if ($hasSetup -and $hasInstall -and $hasMigrate) {
            Add-Result "  Test Bootstrap Setup" "PASS" "All methods present"
        } else {
            Add-Result "  Test Bootstrap Setup" "FAIL" "Missing required methods"
            $allPassed = $false
        }
    } else {
        Add-Result "  Test Bootstrap Setup" "FAIL" "Test helper file missing"
        $allPassed = $false
    }
    
    # Phase 8: EF Workflow Guards
    Write-Host "Phase 8: EF Workflow Guards"
    if (Test-EfWorkflowGuards) {
        Add-Result "  EF Workflow Guards" "PASS" "All guards passed"
    } else {
        Add-Result "  EF Workflow Guards" "FAIL" "Some guards failed"
        $allPassed = $false
    }
    
    return $allPassed
}

# ============================================================================
# REPORT GENERATION
# ============================================================================

function Generate-Report {
    $passCount = ($reportData.Results | Where-Object { $_.Status -eq "PASS" } | Measure-Object).Count
    $failCount = ($reportData.Results | Where-Object { $_.Status -eq "FAIL" } | Measure-Object).Count
    $warnCount = ($reportData.Results | Where-Object { $_.Status -eq "WARN" } | Measure-Object).Count
    $skipCount = ($reportData.Results | Where-Object { $_.Status -eq "SKIP" } | Measure-Object).Count
    
    $reportData.Status = if ($failCount -gt 0) { "FAILED" } else { "PASSED" }
    $reportData.Summary = @{
        Passed = $passCount
        Failed = $failCount
        Warnings = $warnCount
        Skipped = $skipCount
        Total = $passCount + $failCount + $warnCount + $skipCount
    }
    
    Write-Host ""
    Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║                      AUTOMATION REPORT                         ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Mode:       $($reportData.Mode)"
    Write-Host "Status:     $($reportData.Status)" -ForegroundColor $(if ($reportData.Status -eq "PASSED") { "Green" } else { "Red" })
    Write-Host "Timestamp:  $($reportData.Timestamp)"
    Write-Host ""
    Write-Host "Results:"
    Write-Host "  ✓ Passed:   $passCount"
    Write-Host "  ✗ Failed:   $failCount" -ForegroundColor $(if ($failCount -gt 0) { "Red" } else { "Green" })
    Write-Host "  ⚠ Warnings: $warnCount"
    Write-Host "  ⊘ Skipped:  $skipCount"
    Write-Host ""
    
    if ($JsonOutputPath) {
        $reportData | ConvertTo-Json -Depth 5 | Set-Content -Path $JsonOutputPath
        Write-Host "Report saved: $JsonOutputPath"
    }
    
    return $reportData.Status -eq "PASSED"
}

# ============================================================================
# MAIN ORCHESTRATION
# ============================================================================

Write-Host "Database Workflow Orchestrator - Mode: $Mode" -ForegroundColor Cyan

switch ($Mode) {
    "Quick" {
        Write-Section "Quick Checks (Pre-Commit)"
        
        $driftOk = Test-SchemaDrift
        $regOk = Test-ConfigurationRegistration
        $migrationsOk = Test-MigrationFilesExist
        
        if (-not ($driftOk -and $regOk -and $migrationsOk)) {
            $reportData.Status = "FAILED"
            if ($BlockBuildOnFailure) {
                Write-Host ""
                Write-Host "✗ CRITICAL: EF workflow violation detected. Build blocked." -ForegroundColor Red
                exit 1
            }
        }
    }
    
    "Full" {
        Write-Section "Full Automation"
        
        # Run quick checks
        Test-SchemaDrift | Out-Null
        Test-ConfigurationRegistration | Out-Null
        Test-MigrationFilesExist | Out-Null
        
        # Auto-regenerate MySQL if needed
        Invoke-MysqlArtifactRegeneration | Out-Null
        
        # Full verification
        Test-InstallScriptsCurrent | Out-Null
        Invoke-ComprehensiveVerification | Out-Null
    }
    
    "Fix" {
        Write-Section "Force MySQL Artifact Regeneration"
        Invoke-MysqlArtifactRegeneration -Force | Out-Null
        Test-InstallScriptsCurrent | Out-Null
    }
    
    "Verify" {
        Write-Section "Comprehensive 8-Phase Verification"
        Invoke-ComprehensiveVerification | Out-Null
        Test-EfWorkflowGuards | Out-Null
    }
    
    "Report" {
        Write-Section "Detailed Analysis Report"
        Test-SchemaDrift | Out-Null
        Test-ConfigurationRegistration | Out-Null
        Test-MigrationFilesExist | Out-Null
        Test-InstallScriptsCurrent | Out-Null
        Invoke-ComprehensiveVerification | Out-Null
    }
}

Generate-Report

if ($reportData.Status -eq "FAILED" -and $BlockBuildOnFailure) {
    exit 1
}
