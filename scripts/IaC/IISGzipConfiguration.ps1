<#
.SYNOPSIS
IIS Gzip Compression Configuration Helper

.DESCRIPTION
This script provides functions to check, install, and configure gzip compression
on IIS servers for the Profit Sharing application. Implements PS-2067 requirements
for gzip compression and appropriate caching headers.

.REQUIREMENTS
- Windows Server 2019 or later with IIS 10.0+
- Administrator privileges
- WebAdministration PowerShell module

.USAGE
# Import the module functions
. .\IISGzipConfiguration.ps1

# Check and install gzip modules
Initialize-IISGzipCompression

# Configure a specific site
Configure-IISSiteCompression -SiteName 'API' -IsApiSite $true

#>

<#
.SYNOPSIS
Tests and installs required IIS compression modules.

.DESCRIPTION
Checks if Dynamic Compression and Static Compression modules are installed.
If not, installs them and configures server-level gzip settings.

.NOTES
- Requires administrator privileges
- May require server restart for module installation
#>
function Initialize-IISGzipCompression {
    [CmdletBinding()]
    param()

    Write-Host "Initializing IIS Gzip Compression Configuration..." -ForegroundColor Cyan
    
    # Check if this is Windows Server with IIS features available
    $osInfo = Get-CimInstance -ClassName CIM_OperatingSystem
    if ($osInfo -notmatch "Server" -and $osInfo -notmatch "Windows Server") {
        Write-Host "[!] This script is designed for Windows Server with IIS." -ForegroundColor Yellow
        Write-Host "  (This appears to be a development machine without IIS features)" -ForegroundColor Yellow
        return
    }
    
    # Import WebAdministration module
    Write-Host "Importing WebAdministration module..." -ForegroundColor Cyan
    try {
        Import-Module WebAdministration -ErrorAction Stop
        Write-Host "WebAdministration module loaded successfully." -ForegroundColor Green
    }
    catch {
        Write-Warning "WebAdministration module not available. This function requires Windows Server with IIS."
        return
    }

    # Check for Dynamic Compression module
    Write-Host "Checking Dynamic Compression module..." -ForegroundColor Yellow
    $DynamicCompressionFeature = Get-WindowsFeature -Name Web-Dyn-Compression -ErrorAction SilentlyContinue
    if ($DynamicCompressionFeature.InstallState -ne 'Installed') {
        Write-Host "Installing Dynamic Compression module..." -ForegroundColor Yellow
        try {
            Install-WindowsFeature -Name Web-Dyn-Compression -IncludeManagementTools | Out-Null
            Write-Host "Dynamic Compression module installed successfully." -ForegroundColor Green
        }
        catch {
            Write-Error "Failed to install Dynamic Compression: $_"
        }
    } else {
        Write-Host "Dynamic Compression module is already installed." -ForegroundColor Green
    }

    # Check for Static Compression module
    $StaticCompressionFeature = Get-WindowsFeature -Name Web-Stat-Compression
    if ($StaticCompressionFeature.InstallState -ne 'Installed') {
        Write-Host "Installing Static Compression module..." -ForegroundColor Yellow
        try {
            Install-WindowsFeature -Name Web-Stat-Compression -IncludeManagementTools | Out-Null
            Write-Host "Static Compression module installed successfully." -ForegroundColor Green
        }
        catch {
            Write-Error "Failed to install Static Compression: $_"
        }
    } else {
        Write-Host "Static Compression module is already installed." -ForegroundColor Green
    }

    # Configure server-level compression settings
    Write-Host "`nConfiguring server-level gzip settings..." -ForegroundColor Cyan
    
    try {
        # Set compression directory
        $compressionDir = '%SystemDrive%\inetpub\temp\IIS Temporary Compressed Files'
        Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `
            -Name 'directory' -Value $compressionDir -ErrorAction Stop
        
        # Enable Dynamic Compression
        Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `
            -Name 'dynamicCompressionBeforeCaching' -Value $true -ErrorAction Stop
        
        Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `
            -Name 'sendCacheHeaders' -Value $true -ErrorAction Stop
        
        # Set minimum file size for compression to 1KB (1024 bytes)
        Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `
            -Name 'minFileSizeForComp' -Value 1024 -ErrorAction Stop
        
        Write-Host "  - Compression directory: $compressionDir"
        Write-Host "  - Dynamic compression enabled before caching: true"
        Write-Host "  - Minimum file size for compression: 1024 bytes (1KB)"
        Write-Host "Server-level compression configured successfully." -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to configure server-level settings: $_"
        return
    }

    # Enable compression for common content types
    Write-Host "`nConfiguring compression for content types..." -ForegroundColor Cyan
    
    $ContentTypes = @(
        @{ mimeType = 'text/plain'; enabled = $true },
        @{ mimeType = 'text/html'; enabled = $true },
        @{ mimeType = 'text/xml'; enabled = $true },
        @{ mimeType = 'text/css'; enabled = $true },
        @{ mimeType = 'text/javascript'; enabled = $true },
        @{ mimeType = 'text/*'; enabled = $true },
        @{ mimeType = 'message/*'; enabled = $true },
        @{ mimeType = 'application/javascript'; enabled = $true },
        @{ mimeType = 'application/json'; enabled = $true },
        @{ mimeType = 'application/xml'; enabled = $true },
        @{ mimeType = 'application/atom+xml'; enabled = $true },
        @{ mimeType = 'application/x-www-form-urlencoded'; enabled = $true }
    )
    
    $existingDynamicTypes = (Get-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
        -Filter 'system.webServer/httpCompression/dynamicTypes').Collection
    
    foreach ($contentType in $ContentTypes) {
        $exists = $existingDynamicTypes | Where-Object { $_.mimeType -eq $contentType.mimeType }
        
        if ($exists) {
            Write-Host "  [OK] Compression already enabled for: $($contentType.mimeType)"
        } else {
            try {
                Add-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
                    -Filter 'system.webServer/httpCompression/dynamicTypes' `
                    -Value $contentType -ErrorAction Stop
                Write-Host "  [OK] Enabled compression for: $($contentType.mimeType)" -ForegroundColor Green
            }
            catch {
                Write-Warning "  [X] Failed to enable compression for $($contentType.mimeType): $_"
            }
        }
    }

    Write-Host "`nIIS Gzip Compression initialization completed." -ForegroundColor Green
}

<#
.SYNOPSIS
Sets gzip compression and caching for a specific IIS site.

.PARAMETER SiteName
The name of the IIS site (e.g., 'API', 'Frontend').

.PARAMETER IsApiSite
If $true, configures for API endpoints with shorter/dynamic caching.
If $false, configures for static content with longer caching (1 day).

.PARAMETER CacheDurationHours
For non-API sites, the duration in hours to cache static content (default: 24).

.EXAMPLE
Set-IISSiteCompression -SiteName 'API' -IsApiSite $true
Set-IISSiteCompression -SiteName 'Frontend' -IsApiSite $false -CacheDurationHours 24
#>
function Set-IISSiteCompression {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]$SiteName,
        
        [bool]$IsApiSite = $false,
        
        [int]$CacheDurationHours = 24
    )

    Write-Host "Configuring compression and caching for site: $SiteName" -ForegroundColor Cyan
    
    # Verify site exists
    $site = Get-IISSite -Name $SiteName -ErrorAction SilentlyContinue
    if ($null -eq $site) {
        Write-Error "IIS site '$SiteName' not found."
        return
    }

    $sitePath = "IIS:\Sites\$SiteName"

    try {
        # Enable caching
        Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `
            -Name 'enabled' -Value $true -ErrorAction Stop
        Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `
            -Name 'enableKernelCache' -Value $true -ErrorAction Stop
        
        Write-Host "  - HTTP caching enabled"
        Write-Host "  - Kernel cache enabled"

        if ($IsApiSite) {
            Write-Host "  [OK] API site configured for dynamic/short-lived caching."
            Write-Host "    Cache headers will be set per endpoint category in web.config:"
            Write-Host "      - Lookup endpoints: Cache-Control: public, max-age=3600 (1 hour)"
            Write-Host "      - Static data: Cache-Control: public, max-age=86400 (1 day)"
            Write-Host "      - Dynamic data: Cache-Control: private, max-age=0, must-revalidate"
        } else {
            # Configure static content caching
            $timespan = New-TimeSpan -Hours $CacheDurationHours
            Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/staticContent' `
                -Name 'clientCache' -Value @{
                cacheControlMode = 'UseMaxAge'
                maximumAge = $timespan
            } -ErrorAction Stop
            
            Write-Host "  [OK] Frontend site configured for static asset caching."
            Write-Host "    Cache-Control: public, max-age=$($CacheDurationHours * 3600) ($CacheDurationHours hours)"
        }

        Write-Host "Site '$SiteName' compression and caching configured successfully." -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to configure compression/caching for site '$SiteName': $_"
        return
    }
}

<#
.SYNOPSIS
Displays current compression configuration for a specific site or server.

.PARAMETER SiteName
The IIS site name. If not provided, displays server-level configuration.

.EXAMPLE
Get-IISCompressionStatus
Get-IISCompressionStatus -SiteName 'API'
#>
function Get-IISCompressionStatus {
    [CmdletBinding()]
    param(
        [string]$SiteName
    )

    # Import WebAdministration module
    try {
        Import-Module WebAdministration -ErrorAction Stop
    }
    catch {
        Write-Error "WebAdministration module not available. This function requires Windows Server with IIS."
        return
    }

    if ([string]::IsNullOrWhiteSpace($SiteName)) {
        Write-Host "Server-level Compression Configuration:" -ForegroundColor Cyan
        Write-Host "==========================================" -ForegroundColor Cyan
        
        $globalConfig = Get-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
            -Filter 'system.webServer/httpCompression'
        
        Write-Host "  Directory: $($globalConfig.directory)"
        Write-Host "  Minimum file size for compression: $($globalConfig.minFileSizeForComp) bytes"
        Write-Host "  Dynamic compression before caching: $($globalConfig.dynamicCompressionBeforeCaching)"
        Write-Host "  Send cache headers: $($globalConfig.sendCacheHeaders)"
        
        Write-Host "`nEnabled Content Types for Compression:"
        $dynamicTypes = (Get-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
            -Filter 'system.webServer/httpCompression/dynamicTypes').Collection
        
        if ($dynamicTypes.Count -eq 0) {
            Write-Host "  No dynamic types configured"
        } else {
            foreach ($type in $dynamicTypes) {
                Write-Host "    [OK] $($type.mimeType)" -ForegroundColor Green
            }
        }
    } else {
        $site = Get-IISSite -Name $SiteName -ErrorAction SilentlyContinue
        if ($null -eq $site) {
            Write-Error "IIS site '$SiteName' not found."
            return
        }

        Write-Host "Compression Configuration for site: $SiteName" -ForegroundColor Cyan
        Write-Host "==========================================" -ForegroundColor Cyan
        
        $sitePath = "IIS:\Sites\$SiteName"
        $cachingConfig = Get-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `
            -Name '*'
        
        Write-Host "  HTTP Caching Enabled: $($cachingConfig.enabled)"
        Write-Host "  Kernel Cache Enabled: $($cachingConfig.enableKernelCache)"
        
        $staticContentConfig = Get-WebConfigurationProperty -PSPath $sitePath `
            -Filter 'system.webServer/staticContent' -Name 'clientCache'
        
        if ($null -ne $staticContentConfig) {
            Write-Host "  Static Content Cache Mode: $($staticContentConfig.cacheControlMode)"
            if ($staticContentConfig.cacheControlMode -eq 'UseMaxAge') {
                Write-Host "  Static Content Max Age: $($staticContentConfig.maximumAge)"
            }
        }
    }
}

<#
.SYNOPSIS
Validates that all required gzip compression components are properly configured.

.OUTPUTS
PSCustomObject with validation results
#>
function Test-IISGzipConfigurationHealth {
    [CmdletBinding()]
    param()

    Write-Host "Validating IIS Gzip Compression Configuration..." -ForegroundColor Cyan
    
    $results = [PSCustomObject]@{
        IsHealthy = $true
        DynamicCompressionInstalled = $false
        StaticCompressionInstalled = $false
        CompressionEnabled = $false
        MinFileSizeSet = $false
        ContentTypesConfigured = $false
        Issues = @()
    }

    # Check Dynamic Compression
    $dynamicCompression = Get-WindowsFeature -Name Web-Dyn-Compression -ErrorAction SilentlyContinue
    if ($dynamicCompression.InstallState -eq 'Installed') {
        $results.DynamicCompressionInstalled = $true
        Write-Host "  [OK] Dynamic Compression module installed" -ForegroundColor Green
    } else {
        $results.IsHealthy = $false
        $results.Issues += "Dynamic Compression module is not installed"
        Write-Host "  [X] Dynamic Compression module not installed" -ForegroundColor Red
    }

    # Check Static Compression
    $staticCompression = Get-WindowsFeature -Name Web-Stat-Compression -ErrorAction SilentlyContinue
    if ($staticCompression.InstallState -eq 'Installed') {
        $results.StaticCompressionInstalled = $true
        Write-Host "  [OK] Static Compression module installed" -ForegroundColor Green
    } else {
        $results.IsHealthy = $false
        $results.Issues += "Static Compression module is not installed"
        Write-Host "  [X] Static Compression module not installed" -ForegroundColor Red
    }

    # Check server-level configuration (only if WebAdministration available)
    try {
        Import-Module WebAdministration -ErrorAction SilentlyContinue
        $globalConfig = Get-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
            -Filter 'system.webServer/httpCompression'
        
        if ($globalConfig.minFileSizeForComp -eq 1024) {
            $results.MinFileSizeSet = $true
            Write-Host "  [OK] Minimum file size for compression set to 1024 bytes (1KB)" -ForegroundColor Green
        } else {
            $results.IsHealthy = $false
            $results.Issues += "Minimum file size for compression not set to 1024 bytes (currently: $($globalConfig.minFileSizeForComp))"
            Write-Host "  [X] Minimum file size for compression: $($globalConfig.minFileSizeForComp) bytes (should be 1024)" -ForegroundColor Red
        }

        # Check that content types are configured
        $dynamicTypes = (Get-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
            -Filter 'system.webServer/httpCompression/dynamicTypes').Collection
        
        if ($dynamicTypes.Count -ge 10) {
            $results.ContentTypesConfigured = $true
            Write-Host "  [OK] $($dynamicTypes.Count) content types configured for compression" -ForegroundColor Green
        } else {
            $results.IsHealthy = $false
            $results.Issues += "Insufficient content types configured for compression ($($dynamicTypes.Count), expected at least 10)"
            Write-Host "  [X] Only $($dynamicTypes.Count) content types configured (expected at least 10)" -ForegroundColor Yellow
        }

        # Check if compression is enabled for dynamic content
        if ($globalConfig.dynamicCompressionBeforeCaching -eq $true) {
            $results.CompressionEnabled = $true
            Write-Host "  [OK] Dynamic compression before caching enabled" -ForegroundColor Green
        } else {
            $results.IsHealthy = $false
            $results.Issues += "Dynamic compression before caching not enabled"
            Write-Host "  [X] Dynamic compression before caching is disabled" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "  [!] WebAdministration module not available (expected on non-IIS systems)" -ForegroundColor Yellow
    }

    Write-Host ""
    if ($results.IsHealthy) {
        Write-Host "[OK] IIS Gzip Compression configuration is healthy." -ForegroundColor Green
    } else {
        Write-Host "[X] Issues detected in IIS Gzip Compression configuration:" -ForegroundColor Red
        foreach ($issue in $results.Issues) {
            Write-Host "  - $issue" -ForegroundColor Yellow
        }
    }

    return $results
}

# ==========================================
# Main Execution Block
# ==========================================
# If this script is run directly (not dot-sourced), execute the initialization
if ($MyInvocation.InvocationName -ne '.' -and $MyInvocation.InvocationName -ne '') {
    Write-Host "`n" -ForegroundColor White
    Write-Host "=================================================================╗" -ForegroundColor Cyan
    Write-Host "|  IIS Gzip Compression Configuration - PS-2067                 |" -ForegroundColor Cyan
    Write-Host "=================================================================╝" -ForegroundColor Cyan
    Write-Host ""
    
    # Check if running as administrator
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
    if (-not $isAdmin) {
        Write-Host "[!]️  WARNING: This script should be run as Administrator." -ForegroundColor Yellow
        Write-Host "   Some features may not work without elevated privileges.`n" -ForegroundColor Yellow
    }
    
    # Run initialization
    Write-Host "Starting IIS Gzip Compression initialization...`n" -ForegroundColor Green
    Initialize-IISGzipCompression
    
    Write-Host "`n================================================================" -ForegroundColor Cyan
    Write-Host "Initialization complete. You can now:" -ForegroundColor Green
    Write-Host "  • Configure specific sites: Set-IISSiteCompression -SiteName 'API' -IsApiSite `$true" -ForegroundColor White
    Write-Host "  • Check status: Get-IISCompressionStatus" -ForegroundColor White
    Write-Host "  • Validate health: Test-IISGzipConfigurationHealth" -ForegroundColor White
    Write-Host "================================================================`n" -ForegroundColor Cyan
}
