#!/usr/bin/env pwsh
#Requires -RunAsAdministrator

param(
    [switch]$SkipVS = $false,
    [switch]$SkipNodeTools = $false
)

$ErrorActionPreference = "Stop"
$configPath = Join-Path $PSScriptRoot "winget-config.json"

function header($msg) {
    Write-Host "`n================================================" -ForegroundColor Cyan
    Write-Host $msg -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
}

function success($msg) {
    Write-Host "✓ $msg" -ForegroundColor Green
}

function info($msg) {
    Write-Host "ℹ $msg" -ForegroundColor Yellow
}

function error_msg($msg) {
    Write-Host "✗ $msg" -ForegroundColor Red
}

function cmd_exists($cmd) {
    $null = Get-Command $cmd -ErrorAction SilentlyContinue
    return $?
}

header "Checking Prerequisites"

if (-not (cmd_exists winget)) {
    error_msg "winget not found. Please install Windows Package Manager."
    info "Download from: https://www.microsoft.com/p/app-installer/9nblggh4nns1"
    exit 1
}
success "winget is installed"

header "Installing Development Tools via winget"

if (Test-Path $configPath) {
    info "Using configuration from: $configPath"
    try {
        winget import -i $configPath --accept-package-agreements --accept-source-agreements
        success "Winget packages installed successfully"
    }
    catch {
        error_msg "Failed to install winget packages: $_"
        exit 1
    }
}
else {
    error_msg "Configuration file not found: $configPath"
    exit 1
}

if (-not $SkipVS) {
    header "Configuring Visual Studio 2022 Workloads"
    
    $vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products "*" -requires Microsoft.VisualStudio.Product.Professional -property installationPath 2>$null
    
    if (-not $vsPath) {
        info "Visual Studio 2022 Professional not found."
        info "Once installed, run this command:"
        info "  vsconfig.exe --add Microsoft.VisualStudio.Workload.NetWeb --add Microsoft.VisualStudio.Workload.Azure --add Microsoft.VisualStudio.Workload.ManagedDesktop --quiet"
    }
    else {
        info "Found Visual Studio at: $vsPath"
        $vsConfigPath = Join-Path $vsPath "Common7\Tools\vsconfig.exe"
        
        if (Test-Path $vsConfigPath) {
            info "Installing required workloads..."
            $workloads = @(
                "Microsoft.VisualStudio.Workload.NetWeb",
                "Microsoft.VisualStudio.Workload.Azure",
                "Microsoft.VisualStudio.Workload.ManagedDesktop"
            )
            
            $workloadArgs = ($workloads | ForEach-Object { "--add $_" }) -join " "
            $command = "& `"$vsConfigPath`" $workloadArgs --quiet --norestart"
            
            try {
                Invoke-Expression $command
                success "Visual Studio workloads installed successfully"
            }
            catch {
                error_msg "Failed to install VS workloads: $_"
                info "Install them manually through Visual Studio Installer"
            }
        }
        else {
            error_msg "vsconfig.exe not found"
        }
    }
}

if (-not $SkipNodeTools) {
    header "Verifying Node.js Setup via Volta"
    
    if (cmd_exists volta) {
        success "Volta is installed"
        $package = Join-Path (Split-Path $PSScriptRoot -Parent) "ui\package.json"
        if (Test-Path $package) {
            info "Volta configuration found: $package"
            info "Node.js version will be managed automatically"
        }
    }
    else {
        error_msg "Volta not found, but should have been installed"
    }
}

header "Setup Complete!"
success "Development environment setup completed"

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Restart Visual Studio if open during workload installation"
Write-Host "2. Clone repo: git clone https://bitbucket.org/demoulas/smart-profit-sharing"
Write-Host "3. Get secrets.json from team member - place in src/services/configuration/"
Write-Host "4. Add ArtifactoryCloud NuGet source (see README.md)"
Write-Host "5. Open Demoulas.ProfitSharing.slnx in Visual Studio"
Write-Host "6. Set Demoulas.ProfitSharing.AppHost as startup project"
Write-Host "7. Press F5 to start"
Write-Host ""

Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  Services: src/services/README.md"
Write-Host "  Setup: developer_setup/README.md"
Write-Host ""
