<#
.SYNOPSIS
    Validates bitbucket-pipelines.yml syntax before committing changes.

.DESCRIPTION
    This script validates the Bitbucket pipelines configuration file using js-yaml.
    It should be run before committing any changes to bitbucket-pipelines.yml.
    
    The script will:
    1. Check if npx is available (Node.js required)
    2. Validate the YAML syntax using js-yaml
    3. Report any syntax errors found
    
    Note: Some pre-existing YAML patterns may trigger warnings in js-yaml even though
    they work in Bitbucket Pipelines. Focus on errors in sections you've modified.

.PARAMETER PipelineFile
    Path to the bitbucket-pipelines.yml file. Defaults to repository root.

.PARAMETER InstallIfMissing
    If true, attempts to install Node.js/npm if not found. Defaults to false.

.EXAMPLE
    .\scripts\Validate-Pipeline.ps1
    Validates the pipeline file in the repository root.

.EXAMPLE
    .\scripts\Validate-Pipeline.ps1 -InstallIfMissing
    Validates the pipeline and offers to install Node.js if missing.

.NOTES
    Author: Bitbucket Pipeline Automation
    Date: 2025-01-24
    Ticket: PS-2069
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$PipelineFile = "bitbucket-pipelines.yml",
    
    [Parameter()]
    [switch]$InstallIfMissing
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "  Bitbucket Pipeline YAML Validator" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check if pipeline file exists
if (-not (Test-Path $PipelineFile)) {
    Write-Error "Pipeline file not found: $PipelineFile"
    exit 1
}

Write-Host "[✓] Found pipeline file: $PipelineFile" -ForegroundColor Green

# Step 2: Check if Node.js is installed
Write-Host ""
Write-Host "Checking for Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version 2>$null
    Write-Host "[✓] Node.js is installed: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "[✗] Node.js is not installed" -ForegroundColor Red
    
    if ($InstallIfMissing) {
        Write-Host ""
        Write-Host "Attempting to install Node.js via winget..." -ForegroundColor Yellow
        
        try {
            winget install OpenJS.NodeJS.LTS --silent --accept-package-agreements --accept-source-agreements
            Write-Host "[✓] Node.js installation completed" -ForegroundColor Green
            Write-Host "Please restart your terminal and run this script again." -ForegroundColor Yellow
            exit 0
        } catch {
            Write-Error "Failed to install Node.js. Please install manually from: https://nodejs.org/"
            exit 1
        }
    } else {
        Write-Host ""
        Write-Host "Node.js is required for YAML validation." -ForegroundColor Red
        Write-Host "Options:" -ForegroundColor Yellow
        Write-Host "  1. Run with -InstallIfMissing flag to auto-install" -ForegroundColor White
        Write-Host "  2. Install manually from: https://nodejs.org/" -ForegroundColor White
        Write-Host "  3. Use winget: winget install OpenJS.NodeJS.LTS" -ForegroundColor White
        exit 1
    }
}

# Step 3: Check if npx is available
Write-Host ""
Write-Host "Checking for npx..." -ForegroundColor Yellow
try {
    $npxVersion = npx --version 2>$null
    Write-Host "[✓] npx is available: $npxVersion" -ForegroundColor Green
} catch {
    Write-Error "npx is not available. Please reinstall Node.js."
    exit 1
}

# Step 4: Validate YAML syntax
Write-Host ""
Write-Host "Validating YAML syntax..." -ForegroundColor Yellow
Write-Host "File: $PipelineFile" -ForegroundColor White
Write-Host ""

try {
    $output = npx js-yaml $PipelineFile 2>&1
    $exitCode = $LASTEXITCODE
    
    if ($exitCode -eq 0) {
        Write-Host "================================================================" -ForegroundColor Green
        Write-Host "  ✓ YAML SYNTAX VALIDATION PASSED" -ForegroundColor Green
        Write-Host "================================================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "The pipeline file has valid YAML syntax." -ForegroundColor Green
        Write-Host "You can safely commit your changes." -ForegroundColor Green
        Write-Host ""
        exit 0
    } else {
        Write-Host "================================================================" -ForegroundColor Red
        Write-Host "  ✗ YAML SYNTAX VALIDATION FAILED" -ForegroundColor Red
        Write-Host "================================================================" -ForegroundColor Red
        Write-Host ""
        Write-Host "Validation errors:" -ForegroundColor Red
        Write-Host $output -ForegroundColor Yellow
        Write-Host ""
        Write-Host "IMPORTANT: If the error is in a section you did NOT modify," -ForegroundColor Yellow
        Write-Host "it may be a pre-existing issue. js-yaml is sometimes stricter" -ForegroundColor Yellow
        Write-Host "than Bitbucket Pipelines' YAML parser." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Focus on errors in sections you changed:" -ForegroundColor White
        Write-Host "  1. Check the line number mentioned in the error" -ForegroundColor White
        Write-Host "  2. Compare against your git diff to see if it's your change" -ForegroundColor White
        Write-Host "  3. If it's pre-existing, you can proceed with your changes" -ForegroundColor White
        Write-Host ""
        exit 1
    }
} catch {
    Write-Error "Failed to validate YAML: $($_.Exception.Message)"
    exit 1
}
