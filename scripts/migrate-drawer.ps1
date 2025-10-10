#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Migrates PSDrawer from hardcoded implementation to 100% data-driven MVVM version

.DESCRIPTION
    This script:
    1. Backs up current PSDrawer.tsx (if not already done)
    2. Replaces it with PSDrawer.refactored.tsx (100% data-driven)
    3. Provides rollback instructions
    
    After running, you should:
    - Test the application manually
    - Run E2E tests
    - Remove old menuLevels() function from MenuData.ts

.EXAMPLE
    .\migrate-drawer.ps1
    
.EXAMPLE
    # Rollback if needed
    .\migrate-drawer.ps1 -Rollback
#>

param(
    [switch]$Rollback,
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"

# Paths
$repoRoot = Split-Path -Parent $PSScriptRoot
$drawerPath = Join-Path $repoRoot "src\ui\src\components\Drawer"
$currentDrawer = Join-Path $drawerPath "PSDrawer.tsx"
$refactoredDrawer = Join-Path $drawerPath "PSDrawer.refactored.tsx"
$backupDrawer = Join-Path $drawerPath "PSDrawer.original.tsx"

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Test-Prerequisites {
    Write-ColorOutput "`n=== Checking Prerequisites ===" "Cyan"
    
    if (-not (Test-Path $currentDrawer)) {
        Write-ColorOutput "❌ Current PSDrawer.tsx not found at: $currentDrawer" "Red"
        exit 1
    }
    Write-ColorOutput "✅ Current PSDrawer.tsx found" "Green"
    
    if (-not (Test-Path $refactoredDrawer)) {
        Write-ColorOutput "❌ Refactored PSDrawer.refactored.tsx not found at: $refactoredDrawer" "Red"
        exit 1
    }
    Write-ColorOutput "✅ Refactored PSDrawer.refactored.tsx found" "Green"
    
    # Check if hooks, models, utils folders exist
    $hooksPath = Join-Path $drawerPath "hooks"
    $modelsPath = Join-Path $drawerPath "models"
    $utilsPath = Join-Path $drawerPath "utils"
    
    if (-not (Test-Path $hooksPath)) {
        Write-ColorOutput "❌ hooks folder not found at: $hooksPath" "Red"
        exit 1
    }
    Write-ColorOutput "✅ hooks folder found" "Green"
    
    if (-not (Test-Path $modelsPath)) {
        Write-ColorOutput "❌ models folder not found at: $modelsPath" "Red"
        exit 1
    }
    Write-ColorOutput "✅ models folder found" "Green"
    
    if (-not (Test-Path $utilsPath)) {
        Write-ColorOutput "❌ utils folder not found at: $utilsPath" "Red"
        exit 1
    }
    Write-ColorOutput "✅ utils folder found" "Green"
    
    Write-ColorOutput "`n✅ All prerequisites met!" "Green"
}

function Invoke-Rollback {
    Write-ColorOutput "`n=== Rolling Back to Original Implementation ===" "Yellow"
    
    if (-not (Test-Path $backupDrawer)) {
        Write-ColorOutput "❌ Backup file not found at: $backupDrawer" "Red"
        Write-ColorOutput "Cannot rollback without backup!" "Red"
        exit 1
    }
    
    if ($WhatIf) {
        Write-ColorOutput "`n[WHAT-IF] Would copy:" "Cyan"
        Write-ColorOutput "  From: $backupDrawer" "Cyan"
        Write-ColorOutput "  To:   $currentDrawer" "Cyan"
        return
    }
    
    Copy-Item $backupDrawer $currentDrawer -Force
    Write-ColorOutput "✅ Rolled back to original PSDrawer.tsx" "Green"
    Write-ColorOutput "`nOriginal implementation restored from backup." "Yellow"
}

function Invoke-Migration {
    Write-ColorOutput "`n=== Starting Migration to 100% Data-Driven Drawer ===" "Cyan"
    
    # Step 1: Backup if not already done
    if (-not (Test-Path $backupDrawer)) {
        Write-ColorOutput "`nStep 1: Creating backup..." "Yellow"
        if ($WhatIf) {
            Write-ColorOutput "[WHAT-IF] Would copy:" "Cyan"
            Write-ColorOutput "  From: $currentDrawer" "Cyan"
            Write-ColorOutput "  To:   $backupDrawer" "Cyan"
        } else {
            Copy-Item $currentDrawer $backupDrawer -Force
            Write-ColorOutput "✅ Backup created: PSDrawer.original.tsx" "Green"
        }
    } else {
        Write-ColorOutput "`nStep 1: Backup already exists (skipping)" "Gray"
    }
    
    # Step 2: Replace with refactored version
    Write-ColorOutput "`nStep 2: Replacing PSDrawer.tsx with refactored version..." "Yellow"
    if ($WhatIf) {
        Write-ColorOutput "[WHAT-IF] Would copy:" "Cyan"
        Write-ColorOutput "  From: $refactoredDrawer" "Cyan"
        Write-ColorOutput "  To:   $currentDrawer" "Cyan"
    } else {
        Copy-Item $refactoredDrawer $currentDrawer -Force
        Write-ColorOutput "✅ PSDrawer.tsx replaced with 100% data-driven version" "Green"
    }
    
    # Step 3: Instructions
    Write-ColorOutput "`n=== Migration Complete! ===" "Green"
    Write-ColorOutput "`nNext Steps:" "Cyan"
    Write-ColorOutput "1. Remove hardcoded menuLevels() function from src\ui\src\MenuData.ts" "White"
    Write-ColorOutput "2. Test the application:" "White"
    Write-ColorOutput "   cd src\ui" "Gray"
    Write-ColorOutput "   npm run dev" "Gray"
    Write-ColorOutput "3. Run E2E tests:" "White"
    Write-ColorOutput "   cd src\ui" "Gray"
    Write-ColorOutput "   npx playwright test" "Gray"
    Write-ColorOutput "4. If issues arise, rollback with:" "White"
    Write-ColorOutput "   .\scripts\migrate-drawer.ps1 -Rollback" "Gray"
    
    Write-ColorOutput "`nKey Changes:" "Cyan"
    Write-ColorOutput "✅ No hardcoded 'YEAR END' search" "Green"
    Write-ColorOutput "✅ 100% data-driven from API (parentId === null for L0)" "Green"
    Write-ColorOutput "✅ Supports unlimited nesting depth" "Green"
    Write-ColorOutput "✅ MVVM pattern with testable ViewModel" "Green"
    Write-ColorOutput "✅ Works with ANY L0 navigation section" "Green"
    
    Write-ColorOutput "`nDocumentation:" "Cyan"
    Write-ColorOutput "- MIGRATION_STEPS.md - Detailed migration guide" "White"
    Write-ColorOutput "- FULLY_DYNAMIC.md - How 100% dynamic system works" "White"
    Write-ColorOutput "- README.md - Implementation overview" "White"
}

# Main execution
try {
    Write-ColorOutput "`n╔════════════════════════════════════════════════════════════╗" "Cyan"
    Write-ColorOutput "║     PSDrawer Migration to 100% Data-Driven MVVM           ║" "Cyan"
    Write-ColorOutput "╚════════════════════════════════════════════════════════════╝" "Cyan"
    
    Test-Prerequisites
    
    if ($Rollback) {
        Invoke-Rollback
    } else {
        Invoke-Migration
    }
    
    Write-ColorOutput "`n✅ Script completed successfully!" "Green"
    
} catch {
    Write-ColorOutput "`n❌ Error: $_" "Red"
    Write-ColorOutput $_.ScriptStackTrace "Red"
    exit 1
}
