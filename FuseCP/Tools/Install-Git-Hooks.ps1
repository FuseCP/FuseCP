# Copyright (C) 2026 FuseCP
#
# Git Hook Installer - Sets up automated database workflow verification
# Installs pre-commit hook for local enforcement

param(
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path (Resolve-Path $MyInvocation.MyCommand.Path) -Parent
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$gitHooksDir = Join-Path $repoRoot ".git\hooks"

Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║              FuseCP Git Hook Installer                       ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

if ($Uninstall) {
    Write-Host "Removing git hooks..."
    
    $preCommitHook = Join-Path $gitHooksDir "pre-commit"
    if (Test-Path $preCommitHook) {
        Remove-Item $preCommitHook
        Write-Host "✓ Removed pre-commit hook"
    } else {
        Write-Host "⊘ Pre-commit hook not found (already uninstalled)"
    }
    
    Write-Host ""
    Write-Host "✓ Git hooks uninstalled"
    Write-Host ""
    exit 0
}

if (-not (Test-Path $gitHooksDir)) {
    Write-Host "✗ Git hooks directory not found: $gitHooksDir"
    Write-Host "Are you in a git repository?"
    exit 1
}

$preCommitHook = Join-Path $gitHooksDir "pre-commit"

$hookContent = @"
#!/bin/bash
# FuseCP Database Workflow Automation - Pre-commit Hook
# Prevents commits that violate EF workflow compliance

REPO_ROOT="`$(cd "`$(dirname "`$0")"/../.. && pwd)"
TOOLS_DIR="`$REPO_ROOT/FuseCP/Tools"
ORCHESTRATOR="`$TOOLS_DIR/Orchestrate-Database-Workflow.ps1"

echo ""
echo "Running database workflow verification..."
echo ""

if [ -f "`$ORCHESTRATOR" ]; then
    pwsh -NoProfile -File "`$ORCHESTRATOR" -Mode Quick
    EXIT_CODE=`$?
    
    if [ `$EXIT_CODE -ne 0 ]; then
        echo ""
        echo "╔═══════════════════════════════════════════════════════════════╗"
        echo "║  COMMIT BLOCKED: Database workflow violation detected         ║"
        echo "╚═══════════════════════════════════════════════════════════════╝"
        echo ""
        echo "Your commit was blocked because:"
        echo "  • Entity/Configuration mismatch (schema drift)"
        echo "  • Missing configuration registration"
        echo "  • Missing migration files"
        echo ""
        echo "Fix these issues:"
        echo "  1. Create Entity class in Entities/ folder"
        echo "  2. Create corresponding Configuration class"
        echo "  3. Wire in DbContextBase.OnModelCreating()"
        echo "  4. Add DbSet property in DbContext.Sets.cs"
        echo "  5. Run: cd FuseCP\Sources\FuseCP.EnterpriseServer.Data && MigrationAdd.bat"
        echo ""
        echo "For more info: see DATABASE_WORKFLOW_COMPLETE.md"
        echo ""
        exit 1
    fi
else
    echo "⚠ Orchestrator script not found at: `$ORCHESTRATOR"
fi

exit 0
"@

Write-Host "Installing pre-commit hook..."
Write-Host ""

# Create the hook with Unix line endings
$hookContent -split "`n" | Join-String -Separator "`n" | Set-Content -Path $preCommitHook -Encoding UTF8

# Make it executable (if on Unix)
if ($PSVersionTable.Platform -eq "Unix" -or (-not $PSVersionTable.PSVersion.Major -ge 6)) {
    chmod +x $preCommitHook 2>$null
}

Write-Host "✓ Pre-commit hook installed at: $preCommitHook"
Write-Host ""
Write-Host "Hook will run on every commit and enforce:"
Write-Host "  ✓ Entity/Configuration 1:1 alignment"
Write-Host "  ✓ All configurations registered in DbContextBase"
Write-Host "  ✓ Migration files present for all 4 providers"
Write-Host ""
Write-Host "Commits that violate EF workflow will be blocked."
Write-Host ""
Write-Host "To disable for a specific commit:"
Write-Host "  git commit --no-verify -m 'message'"
Write-Host ""
Write-Host "To uninstall hooks:"
Write-Host "  pwsh -File FuseCP/Tools/Install-Git-Hooks.ps1 -Uninstall"
Write-Host ""
