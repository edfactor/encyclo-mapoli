<#
.SYNOPSIS
Install .NET 10 Hosting Bundle for IIS on a remote or local machine

.DESCRIPTION
Downloads and installs the .NET 10 ASP.NET Core Hosting Bundle on Windows Server.
This enables IIS to host .NET 10 ASP.NET Core applications.

.PARAMETER ComputerName
The name or IP address of the target machine. Defaults to localhost.

.PARAMETER DownloadUrl
The URL to download the hosting bundle from. 
Default: https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/10.0.0/dotnet-hosting-10.0.0-win.exe

.PARAMETER InstallPath
Local path where the installer will be downloaded. 
Default: $env:TEMP\dotnet-hosting-10.0.0-win.exe

.PARAMETER Force
If $true, skips version check and forces reinstallation.

.PARAMETER Credential
PSCredential object for remote authentication. Required for remote machines.
Use: Get-Credential to prompt for credentials.

.EXAMPLE
# Install on local machine
.\Install-DotNetHostingBundle.ps1

# Install on remote machine
.\Install-DotNetHostingBundle.ps1 -ComputerName "web-server-01" -Credential (Get-Credential)

# Force reinstallation
.\Install-DotNetHostingBundle.ps1 -Force

.NOTES
- Requires Administrator privileges on the target machine
- IIS must be installed on the target machine
- May require IIS restart after installation
- Network access to download URL required
#>

param(
    [Parameter(Mandatory = $false)]
    [string]$ComputerName = "localhost",
    
    [Parameter(Mandatory = $false)]
    [string]$DownloadUrl = "https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/10.0.0/dotnet-hosting-10.0.0-win.exe",
    
    [Parameter(Mandatory = $false)]
    [string]$InstallPath = "$env:TEMP\dotnet-hosting-10.0.0-win.exe",
    
    [Parameter(Mandatory = $false)]
    [switch]$Force,
    
    [Parameter(Mandatory = $false)]
    [PSCredential]$Credential
)

$ErrorActionPreference = "Stop"

# ==========================================
# Helper Functions
# ==========================================

function Write-Header {
    [CmdletBinding()]
    param([string]$Title)
    
    Write-Host ""
    Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║  $($Title.PadRight(62))  ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Step {
    [CmdletBinding()]
    param([string]$Message)
    
    Write-Host "▶ $Message" -ForegroundColor Yellow
}

function Write-Success {
    [CmdletBinding()]
    param([string]$Message)
    
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Warning-Custom {
    [CmdletBinding()]
    param([string]$Message)
    
    Write-Host "⚠ $Message" -ForegroundColor Yellow
}

function Write-Error-Custom {
    [CmdletBinding()]
    param([string]$Message)
    
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Test-AdminPrivileges {
    [CmdletBinding()]
    param()
    
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
    return $isAdmin
}

function Get-DotNetHostingVersion {
    [CmdletBinding()]
    param(
        [string]$ComputerName,
        [PSCredential]$Credential
    )
    
    $scriptBlock = {
        # Check for ASP.NET Core Hosting Bundle in registry
        $hostingPath = "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall"
        $hostingKey = Get-ChildItem -Path $hostingPath -ErrorAction SilentlyContinue | 
            Where-Object { $_.GetValue("DisplayName") -like "*ASP.NET Core*Hosting*" } |
            Select-Object -First 1
        
        if ($hostingKey) {
            $version = $hostingKey.GetValue("DisplayVersion")
            return $version
        }
        
        return $null
    }
    
    if ($ComputerName -eq "localhost" -or $ComputerName -eq $env:COMPUTERNAME) {
        return & $scriptBlock
    } else {
        if ($null -eq $Credential) {
            throw "Credential is required for remote machine operations."
        }
        
        $result = Invoke-Command -ComputerName $ComputerName -ScriptBlock $scriptBlock -Credential $Credential -ErrorAction SilentlyContinue
        return $result
    }
}

function Test-UrlAccessibility {
    [CmdletBinding()]
    param([string]$Url)
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Head -TimeoutSec 10 -ErrorAction SilentlyContinue
        return $response.StatusCode -eq 200
    }
    catch {
        return $false
    }
}

function Download-HostingBundle {
    [CmdletBinding()]
    param(
        [string]$Url,
        [string]$SavePath
    )
    
    Write-Step "Downloading .NET 10 Hosting Bundle from $Url"
    
    try {
        # Create directory if it doesn't exist
        $directory = Split-Path -Parent $SavePath
        if (-not (Test-Path $directory)) {
            New-Item -ItemType Directory -Path $directory -Force | Out-Null
        }
        
        # Download with progress
        $ProgressPreference = 'Continue'
        Invoke-WebRequest -Uri $Url -OutFile $SavePath -ErrorAction Stop
        
        if (Test-Path $SavePath) {
            $fileSize = (Get-Item $SavePath).Length / 1MB
            Write-Success "Downloaded to $SavePath ($('{0:N2}' -f $fileSize) MB)"
            return $true
        } else {
            Write-Error-Custom "Download failed - file not created"
            return $false
        }
    }
    catch {
        Write-Error-Custom "Download failed: $_"
        return $false
    }
}

function Install-HostingBundleLocal {
    [CmdletBinding()]
    param(
        [string]$InstallerPath
    )
    
    if (-not (Test-Path $InstallerPath)) {
        throw "Installer not found at $InstallerPath"
    }
    
    Write-Step "Running .NET 10 Hosting Bundle installer (this may take several minutes)"
    
    try {
        $process = Start-Process -FilePath $InstallerPath -ArgumentList "/install", "/quiet", "/norestart" -Wait -PassThru
        
        if ($process.ExitCode -eq 0) {
            Write-Success "Installation completed successfully (exit code: 0)"
            return $true
        } else {
            Write-Error-Custom "Installation failed with exit code: $($process.ExitCode)"
            return $false
        }
    }
    catch {
        Write-Error-Custom "Installation error: $_"
        return $false
    }
}

function Install-HostingBundleRemote {
    [CmdletBinding()]
    param(
        [string]$ComputerName,
        [string]$InstallerUrl,
        [PSCredential]$Credential
    )
    
    $scriptBlock = {
        param($Url, $InstallPath)
        
        $downloadDir = Split-Path -Parent $InstallPath
        if (-not (Test-Path $downloadDir)) {
            New-Item -ItemType Directory -Path $downloadDir -Force | Out-Null
        }
        
        try {
            Write-Host "  → Downloading to $InstallPath on $env:COMPUTERNAME" -ForegroundColor Yellow
            $ProgressPreference = 'SilentlyContinue'
            Invoke-WebRequest -Uri $Url -OutFile $InstallPath -ErrorAction Stop
            
            if (-not (Test-Path $InstallPath)) {
                throw "Downloaded file not found"
            }
            
            Write-Host "  ✓ Download complete, starting installation" -ForegroundColor Green
            
            $process = Start-Process -FilePath $InstallPath -ArgumentList "/install", "/quiet", "/norestart" -Wait -PassThru
            
            if ($process.ExitCode -eq 0) {
                Write-Host "  ✓ Installation completed successfully" -ForegroundColor Green
                return $true
            } else {
                throw "Installer exited with code $($process.ExitCode)"
            }
        }
        catch {
            Write-Host "  ✗ Error: $_" -ForegroundColor Red
            return $false
        }
    }
    
    Write-Step "Installing on remote machine: $ComputerName"
    
    try {
        $result = Invoke-Command -ComputerName $ComputerName `
            -ScriptBlock $scriptBlock `
            -ArgumentList $InstallerUrl, $InstallPath `
            -Credential $Credential `
            -ErrorAction Stop
        
        return $result
    }
    catch {
        Write-Error-Custom "Remote installation failed: $_"
        return $false
    }
}

function Test-HostingBundleInstallation {
    [CmdletBinding()]
    param(
        [string]$ComputerName,
        [PSCredential]$Credential
    )
    
    $scriptBlock = {
        $hostingPath = "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall"
        $hostingKey = Get-ChildItem -Path $hostingPath -ErrorAction SilentlyContinue | 
            Where-Object { $_.GetValue("DisplayName") -like "*ASP.NET Core*Hosting*" } |
            Select-Object -First 1
        
        if ($hostingKey) {
            $version = $hostingKey.GetValue("DisplayVersion")
            return @{
                IsInstalled = $true
                Version = $version
                DisplayName = $hostingKey.GetValue("DisplayName")
            }
        }
        
        return @{ IsInstalled = $false }
    }
    
    Write-Step "Verifying installation"
    
    try {
        if ($ComputerName -eq "localhost" -or $ComputerName -eq $env:COMPUTERNAME) {
            $result = & $scriptBlock
        } else {
            if ($null -eq $Credential) {
                Write-Warning-Custom "Cannot verify remote installation without credentials"
                return $false
            }
            
            $result = Invoke-Command -ComputerName $ComputerName -ScriptBlock $scriptBlock -Credential $Credential
        }
        
        if ($result.IsInstalled) {
            Write-Success "ASP.NET Core Hosting Bundle installed: $($result.DisplayName)"
            Write-Success "Version: $($result.Version)"
            return $true
        } else {
            Write-Error-Custom "Hosting Bundle installation not verified"
            return $false
        }
    }
    catch {
        Write-Error-Custom "Verification failed: $_"
        return $false
    }
}

function Restart-IIS {
    [CmdletBinding()]
    param(
        [string]$ComputerName,
        [PSCredential]$Credential
    )
    
    $scriptBlock = {
        try {
            Write-Host "  → Stopping IIS" -ForegroundColor Yellow
            Stop-WebAppPool -Name "*" -ErrorAction SilentlyContinue
            iisreset /stop | Out-Null
            
            Start-Sleep -Seconds 2
            
            Write-Host "  → Starting IIS" -ForegroundColor Yellow
            iisreset /start | Out-Null
            
            Write-Host "  ✓ IIS restarted successfully" -ForegroundColor Green
            return $true
        }
        catch {
            Write-Host "  ✗ Error restarting IIS: $_" -ForegroundColor Red
            return $false
        }
    }
    
    Write-Step "Restarting IIS"
    
    try {
        if ($ComputerName -eq "localhost" -or $ComputerName -eq $env:COMPUTERNAME) {
            return & $scriptBlock
        } else {
            if ($null -eq $Credential) {
                Write-Warning-Custom "IIS restart requires remote credentials"
                return $false
            }
            
            return Invoke-Command -ComputerName $ComputerName -ScriptBlock $scriptBlock -Credential $Credential
        }
    }
    catch {
        Write-Error-Custom "IIS restart failed: $_"
        return $false
    }
}

# ==========================================
# Main Script Execution
# ==========================================

Write-Header ".NET 10 Hosting Bundle Installer"

# Validate local admin privileges
if (-not (Test-AdminPrivileges)) {
    Write-Error-Custom "This script must be run as Administrator"
    exit 1
}

Write-Success "Running with Administrator privileges"

# Check target machine
$targetDisplay = if ($ComputerName -eq "localhost") { "localhost" } else { $ComputerName }
Write-Host "Target Machine: $targetDisplay" -ForegroundColor Cyan

# Check current installation
Write-Step "Checking current .NET hosting bundle installation"
$currentVersion = Get-DotNetHostingVersion -ComputerName $ComputerName -Credential $Credential

if ($currentVersion -and -not $Force) {
    Write-Success "ASP.NET Core Hosting Bundle is already installed (version: $currentVersion)"
    $reinstall = Read-Host "Do you want to reinstall? (y/n)"
    
    if ($reinstall -notlike "y*") {
        Write-Host "Installation cancelled" -ForegroundColor Yellow
        exit 0
    }
}

# Test URL accessibility
Write-Step "Verifying download URL accessibility"
if (-not (Test-UrlAccessibility $DownloadUrl)) {
    Write-Error-Custom "Cannot access download URL: $DownloadUrl"
    Write-Host "Please verify:" -ForegroundColor Yellow
    Write-Host "  - Internet connectivity" -ForegroundColor Yellow
    Write-Host "  - Proxy settings if applicable" -ForegroundColor Yellow
    Write-Host "  - URL: $DownloadUrl" -ForegroundColor Yellow
    exit 1
}
Write-Success "Download URL is accessible"

# Download installer
if (-not (Download-HostingBundle -Url $DownloadUrl -SavePath $InstallPath)) {
    exit 1
}

# Install on target machine
if ($ComputerName -eq "localhost" -or $ComputerName -eq $env:COMPUTERNAME) {
    # Local installation
    if (-not (Install-HostingBundleLocal -InstallerPath $InstallPath)) {
        exit 1
    }
} else {
    # Remote installation
    if ($null -eq $Credential) {
        Write-Step "Requesting credentials for $ComputerName"
        $Credential = Get-Credential -Message "Enter credentials for $ComputerName" -UserName "$ComputerName\Administrator"
        
        if ($null -eq $Credential) {
            Write-Error-Custom "Credentials are required for remote installation"
            exit 1
        }
    }
    
    if (-not (Install-HostingBundleRemote -ComputerName $ComputerName -InstallerUrl $DownloadUrl -Credential $Credential)) {
        exit 1
    }
}

# Verify installation
if (-not (Test-HostingBundleInstallation -ComputerName $ComputerName -Credential $Credential)) {
    Write-Error-Custom "Installation verification failed"
    exit 1
}

# Restart IIS
if (-not (Restart-IIS -ComputerName $ComputerName -Credential $Credential)) {
    Write-Warning-Custom "IIS restart may require manual intervention"
}

# Summary
Write-Host ""
Write-Host "═════════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Success ".NET 10 Hosting Bundle installation completed successfully!"
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Verify IIS is running: iisreset /status" -ForegroundColor White
Write-Host "  2. Create or update your ASP.NET Core application in IIS" -ForegroundColor White
Write-Host "  3. Test your application endpoints" -ForegroundColor White
Write-Host ""
Write-Host "Documentation: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/" -ForegroundColor Cyan
Write-Host "═════════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
