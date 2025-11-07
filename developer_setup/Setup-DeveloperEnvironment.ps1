#!/usr/bin/env pwsh
#Requires -RunAsAdministrator

param([switch]$SkipVS, [switch]$SkipNodeTools)

$ErrorActionPreference = "Stop"

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "Developer Setup" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

Write-Host "`nChecking Prerequisites..." -ForegroundColor Cyan
$wingetExists = $null -ne (Get-Command winget -ErrorAction SilentlyContinue)

if (-not $wingetExists) {
    Write-Host "`nX winget not found" -ForegroundColor Red
    Write-Host "I Download: https://www.microsoft.com/p/app-installer/9nblggh4nns1" -ForegroundColor Yellow
    exit 1
}

Write-Host "OK winget is installed" -ForegroundColor Green

Write-Host "`nInstalling Development Tools..." -ForegroundColor Cyan

$configPath = Join-Path $PSScriptRoot "winget-config.json"

if (-not (Test-Path $configPath)) {
    Write-Host "X winget-config.json not found at: $configPath" -ForegroundColor Red
    Write-Host "I Place winget-config.json in the same folder as this script" -ForegroundColor Yellow
    exit 1
}

try {
    Write-Host "I Installing packages via winget..." -ForegroundColor Yellow
    winget import -i $configPath --accept-package-agreements --accept-source-agreements
    Write-Host "OK Winget packages installed successfully" -ForegroundColor Green
}
catch {
    Write-Host "X Failed to install packages: $_" -ForegroundColor Red
    exit 1
}

if (-not $SkipVS) {
    Write-Host "`nConfiguring Visual Studio 2022..." -ForegroundColor Cyan
    
    $vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products "*" -requires Microsoft.VisualStudio.Product.Professional -property installationPath 2>$null
    
    if (-not $vsPath) {
        Write-Host "I Visual Studio 2022 Professional not found (may still be installing)" -ForegroundColor Yellow
    }
    else {
        Write-Host "OK Found Visual Studio at: $vsPath" -ForegroundColor Green
        
        $vsSetupPath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\setup.exe"
        Write-Host "I Checking for installer at: $vsSetupPath" -ForegroundColor Yellow
        
        if (Test-Path $vsSetupPath) {
            Write-Host "I Installing required workloads..." -ForegroundColor Yellow
            try {
                & $vsSetupPath modify --installPath $vsPath `
                    --add Microsoft.VisualStudio.Workload.NetWeb `
                    --add Microsoft.VisualStudio.Workload.Azure `
                    --add Microsoft.VisualStudio.Workload.ManagedDesktop `
                    --includeRecommended `
                    --passive --norestart
                Write-Host "OK Visual Studio workloads installed successfully" -ForegroundColor Green
            }
            catch {
                Write-Host "X Failed to install VS workloads: $_" -ForegroundColor Red
                Write-Host "I Manual installation:" -ForegroundColor Yellow
                Write-Host "  1. Open Visual Studio Installer" -ForegroundColor Yellow
                Write-Host "  2. Click 'Modify' on Visual Studio 2022 Professional" -ForegroundColor Yellow
                Write-Host "  3. Select workloads: ASP.NET, Azure, .NET desktop" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "X setup.exe not found at: $vsSetupPath" -ForegroundColor Red
            Write-Host "I To configure workloads, open Visual Studio Installer and:" -ForegroundColor Yellow
            Write-Host "  1. Click 'Modify' on Visual Studio 2022 Professional" -ForegroundColor Yellow
            Write-Host "  2. Select these workloads:" -ForegroundColor Yellow
            Write-Host "     - ASP.NET and web development" -ForegroundColor Yellow
            Write-Host "     - Azure development" -ForegroundColor Yellow
            Write-Host "     - .NET desktop development" -ForegroundColor Yellow
            Write-Host "  3. Click 'Modify' to install" -ForegroundColor Yellow
        }
    }
}

if (-not $SkipNodeTools) {
    Write-Host "`nVerifying Node.js Setup..." -ForegroundColor Cyan
    
    $voltaExists = $null -ne (Get-Command volta -ErrorAction SilentlyContinue)
    
    if ($voltaExists) {
        Write-Host "OK Volta is installed" -ForegroundColor Green
        Write-Host "I Node.js version will be managed automatically by Volta" -ForegroundColor Yellow
    }
    else {
        Write-Host "X Volta not found" -ForegroundColor Red
        Write-Host "I Install manually: https://docs.volta.sh/guide/getting-started" -ForegroundColor Yellow
        Write-Host "I Or run: winget install Volta.Volta --accept-package-agreements" -ForegroundColor Yellow
    }
}

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

Write-Host "`nNext Steps:" -ForegroundColor Green
Write-Host "1. Restart Visual Studio if it was open"
Write-Host "2. Clone: git clone https://github.com/dotnet/aspire-samples.git"
Write-Host "3. Get secrets.json from team member"
Write-Host "4. Place secrets.json in: src/services/configuration/"
Write-Host "5. Add ArtifactoryCloud NuGet source (see README.md)"
Write-Host "8. Press F5 to run"
Write-Host ""

Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  Services: src/services/README.md"
Write-Host "  Setup: developer_setup/README.md"
Write-Host ""
