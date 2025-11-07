#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Automated setup script for Demoulas Profit Sharing developer environment.
.DESCRIPTION
    This script installs all required development tools and Visual Studio workloads
    using winget and Visual Studio installer automation.
.EXAMPLE
    .\Setup-DeveloperEnvironment.ps1
.NOTES
    Requires admin privileges to install packages and VS workloads.
#>

#Requires -RunAsAdministrator

param(
    [switch]$SkipVS = $false,
    [switch]$SkipNodeTools = $false,
    [switch]$Verbose = $false
)

$ErrorActionPreference = "Stop"
$configPath = Join-Path $PSScriptRoot "winget-config.json"

# Define functions first (before they're called)
function Write-Header {
    param([string]$Message)
    Write-Host "`n================================================" -ForegroundColor Cyan
    Write-Host $Message -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Yellow
}

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Test-CommandExists {
    param([string]$Command)
    $null = Get-Command $Command -ErrorAction SilentlyContinue
    return $?
}

# Check prerequisites
Write-Header "Checking Prerequisites"

if (-not (Test-CommandExists winget)) {
    Write-ErrorMsg "winget not found. Please install Windows Package Manager first."
    Write-Info "Download from: https://www.microsoft.com/p/app-installer/9nblggh4nns1"
    exit 1
}
Write-Success "winget is installed"

# Install winget packages
Write-Header "Installing Development Tools via winget"

if (Test-Path $configPath) {
    Write-Info "Using configuration from: $configPath"
    try {
        winget import -i $configPath --accept-package-agreements --accept-source-agreements
        Write-Success "Winget packages installed successfully"
    }
    catch {
        Write-ErrorMsg "Failed to install winget packages: $_"
        exit 1
    }
}
else {
    Write-ErrorMsg "Configuration file not found: $configPath"
    exit 1
}

# Visual Studio Workload Configuration
if (-not $SkipVS) {
    Write-Header "Configuring Visual Studio 2022 Workloads"
    
    # Locate Visual Studio installer
    $vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products "*" -requires Microsoft.VisualStudio.Product.Professional -property installationPath 2>$null
    
    if (-not $vsPath) {
        Write-Info "Visual Studio 2022 Professional not found. It may still be installing..."
        Write-Info "Once installation completes, run this command manually:"
        Write-Info "  vsconfig.exe --add Microsoft.VisualStudio.Workload.NetWeb --add Microsoft.VisualStudio.Workload.Azure --add Microsoft.VisualStudio.Workload.ManagedDesktop --quiet"
    }
    else {
        Write-Info "Found Visual Studio at: $vsPath"
        $vsConfigPath = Join-Path $vsPath "Common7\Tools\vsconfig.exe"
        
        if (Test-Path $vsConfigPath) {
            Write-Info "Installing required workloads..."
            $workloads = @(
                "Microsoft.VisualStudio.Workload.NetWeb",      # ASP.NET and web development
                "Microsoft.VisualStudio.Workload.Azure",       # Azure development
                "Microsoft.VisualStudio.Workload.ManagedDesktop" # .NET desktop development
            )
            
            $workloadArgs = ($workloads | ForEach-Object { "--add $_" }) -join " "
            $command = "& `"$vsConfigPath`" $workloadArgs --quiet --norestart"
            
            try {
                Invoke-Expression $command
                Write-Success "Visual Studio workloads installed successfully"
            }
            catch {
                Write-ErrorMsg "Failed to install VS workloads: $_"
                Write-Info "You may need to install them manually through Visual Studio Installer"
            }
        }
        else {
            Write-ErrorMsg "vsconfig.exe not found at expected location"
        }
    }
}

# Node.js via Volta
if (-not $SkipNodeTools) {
    Write-Header "Verifying Node.js Setup via Volta"
    
    if (Test-CommandExists volta) {
        Write-Success "Volta is installed"
        
        # Check if Node.js is pinned in volta
        $package = Join-Path (Split-Path $PSScriptRoot -Parent) "ui\package.json"
        if (Test-Path $package) {
            Write-Info "Volta configuration found in: $package"
            Write-Info "Node.js version will be managed automatically by Volta"
        }
    }
    else {
        Write-ErrorMsg "Volta not found, but should have been installed by winget"
    }
}

# Summary
Write-Header "Setup Complete!"
Write-Success "Development environment setup completed"

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Restart Visual Studio if it was open during workload installation"
Write-Host "2. Clone the repository: git clone https://bitbucket.org/demoulas/smart-profit-sharing"
Write-Host "3. Obtain secrets.json from a team member and place in: src/services/configuration/"
Write-Host "4. Add ArtifactoryCloud NuGet source (see README.md)"
Write-Host "5. Open Demoulas.ProfitSharing.slnx in Visual Studio"
Write-Host "6. Set Demoulas.ProfitSharing.AppHost as startup project"
Write-Host "7. Press F5 to start debugging`n"

Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  Services README: src/services/README.md"
Write-Host "  Setup Guide: developer_setup/README.md`n"
