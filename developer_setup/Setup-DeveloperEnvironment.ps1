#!/usr/bin/env pwsh
#Requires -RunAsAdministrator

param([switch]$SkipVS, [switch]$SkipNodeTools)

$ErrorActionPreference = "Stop"

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "Demoulas Profit Sharing - Developer Setup" -ForegroundColor Cyan
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
        Write-Host "I Once installed, run this command:" -ForegroundColor Yellow
        Write-Host "  vsconfig.exe --add Microsoft.VisualStudio.Workload.NetWeb --add Microsoft.VisualStudio.Workload.Azure --add Microsoft.VisualStudio.Workload.ManagedDesktop --quiet" -ForegroundColor Yellow
    }
    else {
        Write-Host "I Found Visual Studio at: $vsPath" -ForegroundColor Yellow
        $vsConfigPath = Join-Path $vsPath "Common7\Tools\vsconfig.exe"
        
        if (Test-Path $vsConfigPath) {
            Write-Host "I Installing required workloads..." -ForegroundColor Yellow
            $workloads = @(
                "Microsoft.VisualStudio.Workload.NetWeb",
                "Microsoft.VisualStudio.Workload.Azure",
                "Microsoft.VisualStudio.Workload.ManagedDesktop"
            )
            
            $workloadArgs = ($workloads | ForEach-Object { "--add $_" }) -join " "
            
            try {
                & $vsConfigPath $workloadArgs --quiet --norestart
                Write-Host "OK Visual Studio workloads installed successfully" -ForegroundColor Green
            }
            catch {
                Write-Host "X Failed to install VS workloads: $_" -ForegroundColor Red
                Write-Host "I Install them manually through Visual Studio Installer" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "X vsconfig.exe not found" -ForegroundColor Red
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
        Write-Host "X Volta not found (should have been installed by winget)" -ForegroundColor Red
    }
}

Write-Host "`n================================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan

Write-Host "`nNext Steps:" -ForegroundColor Green
Write-Host "1. Restart Visual Studio if it was open"
Write-Host "2. Clone: git clone https://bitbucket.org/demoulas/smart-profit-sharing"
Write-Host "3. Get secrets.json from team member"
Write-Host "4. Place secrets.json in: src/services/configuration/"
Write-Host "5. Add ArtifactoryCloud NuGet source (see README.md)"
Write-Host "6. Open src/services/Demoulas.ProfitSharing.slnx"
Write-Host "7. Set Demoulas.ProfitSharing.AppHost as startup project"
Write-Host "8. Press F5 to run"
Write-Host ""

Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  Services: src/services/README.md"
Write-Host "  Setup: developer_setup/README.md"
Write-Host ""
