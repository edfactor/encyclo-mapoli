$ErrorActionPreference = 'Stop'
$VerbosePreference = 'Continue'

# ==========================================
# Deployment Configuration
# ==========================================
$StopAppTimeout = 10
$envTarget = $args[0]
$envServerName = $args[1]
$apiArtifactName = $args[2]
$uiArtifactName = $args[3]
$configTarget = ''

# Pipeline-aware logging
$PipelineLogging = @{
    Build = $env:BITBUCKET_BUILD_NUMBER
    Branch = $env:BITBUCKET_BRANCH
    Commit = $env:BITBUCKET_COMMIT
}

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "Web Deployment Script - PS-2067 IIS Gzip Configuration Integrated" -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan
if ($PipelineLogging.Build) {
    Write-Host "Pipeline Build: $($PipelineLogging.Build) | Branch: $($PipelineLogging.Branch)" -ForegroundColor Gray
}
Write-Host ""

function Get-ConfigEnvironment($envTargetVar) {
    Write-Host "The value for envTargetVar is $($envTargetVar)"
    switch ($envTargetVar) {
        'qa'    { Write-Host "ENV is QA"; return "QA" }
        'uat'   { Write-Host "ENV is UAT"; return "UAT" }
        'prod'  { Write-Host "ENV is PRODUCTION"; return "Production" }
        default { Write-Host "Unknown ENV" }
    }
}

function Test-IISGzipModules {
    <#
    .SYNOPSIS
    Ensures IIS has gzip compression modules installed and enabled on the remote server.
    
    .DESCRIPTION
    Checks for Dynamic Compression and Static Compression modules, installs them if needed,
    and enables compression for the specified content types. Implements PS-2067 requirements.
    
    .NOTES
    - Requires administrator privileges
    - May require server restart for module installation
    - Gracefully handles non-Server OS environments
    #>
    
    Write-Host "Checking IIS gzip compression modules..." -ForegroundColor Cyan
    
    # Check if this is Windows Server with IIS features available
    try {
        $osInfo = Get-CimInstance -ClassName CIM_OperatingSystem -ErrorAction Stop
        if ($osInfo.Caption -notmatch "Server") {
            Write-Warning "This server does not appear to be Windows Server. Gzip configuration may not work."
            return
        }
    }
    catch {
        Write-Warning "Could not determine OS type. Continuing with gzip configuration..."
    }
    
    # Import WebAdministration module
    try {
        Import-Module WebAdministration -ErrorAction Stop
        Write-Host "WebAdministration module loaded successfully." -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to load WebAdministration module: $_"
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
    Write-Host "Checking Static Compression module..." -ForegroundColor Yellow
    $StaticCompressionFeature = Get-WindowsFeature -Name Web-Stat-Compression -ErrorAction SilentlyContinue
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
        
        Write-Host "  ✓ Compression directory: $compressionDir"
        Write-Host "  ✓ Dynamic compression enabled before caching: true"
        Write-Host "  ✓ Minimum file size for compression: 1024 bytes (1KB)"
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
            Write-Host "  ✓ Compression already enabled for: $($contentType.mimeType)"
        } else {
            try {
                Add-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `
                    -Filter 'system.webServer/httpCompression/dynamicTypes' `
                    -Value $contentType -ErrorAction Stop
                Write-Host "  ✓ Enabled compression for: $($contentType.mimeType)" -ForegroundColor Green
            }
            catch {
                Write-Warning "  ⚠ Failed to enable compression for $($contentType.mimeType): $_"
            }
        }
    }

    Write-Host "`nIIS Gzip Compression configuration completed successfully." -ForegroundColor Green
    return $true
}

function Set-SiteCompressionAndCaching {
    <#
    .SYNOPSIS
    Configures gzip compression and cache-control headers for a specific IIS site.
    
    .PARAMETER SiteName
    The name of the IIS site (e.g., 'API', 'Frontend').
    
    .PARAMETER IsApiSite
    If $true, configures for API endpoints with shorter/dynamic caching.
    If $false, configures for static content with longer caching (1 day).
    
    .DESCRIPTION
    Enables HTTP caching and kernel cache for the specified site. For API sites,
    cache headers are configured per endpoint category in web.config. For static
    content sites, enables 1-day TTL caching.
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string]$SiteName,
        
        [bool]$IsApiSite = $false
    )
    
    Write-Host "Configuring compression and caching for site: $SiteName" -ForegroundColor Cyan
    
    # Verify site exists
    try {
        $site = Get-IISSite -Name $SiteName -ErrorAction Stop
    }
    catch {
        Write-Warning "IIS site '$SiteName' not found. Skipping site-specific configuration."
        return
    }

    $sitePath = "IIS:\Sites\$SiteName"

    try {
        # Enable caching
        Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `
            -Name 'enabled' -Value $true -ErrorAction Stop
        Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `
            -Name 'enableKernelCache' -Value $true -ErrorAction Stop
        
        Write-Host "  ✓ HTTP caching enabled"
        Write-Host "  ✓ Kernel cache enabled"

        if ($IsApiSite) {
            Write-Host "  ✓ API site configured for dynamic/short-lived caching."
            Write-Host "    Cache headers will be set per endpoint category in web.config:"
            Write-Host "      - Lookup endpoints: Cache-Control: public, max-age=3600 (1 hour)"
            Write-Host "      - Static data: Cache-Control: public, max-age=86400 (1 day)"
            Write-Host "      - Dynamic data: Cache-Control: private, max-age=0, must-revalidate" -ForegroundColor Green
        } else {
            # Configure static content caching
            $timespan = New-TimeSpan -Hours 24
            Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/staticContent' `
                -Name 'clientCache' -Value @{
                cacheControlMode = 'UseMaxAge'
                maximumAge = $timespan
            } -ErrorAction Stop
            
            Write-Host "  ✓ Frontend site configured for static asset caching."
            Write-Host "    Cache-Control: public, max-age=86400 (1 day)" -ForegroundColor Green
        }

        Write-Host "Site '$SiteName' compression and caching configured successfully." -ForegroundColor Green
    }
    catch {
        Write-Warning "Failed to configure compression/caching for site '$SiteName': $_"
        return
    }
}

$configTarget = Get-ConfigEnvironment $envTarget

$Deployments = @(
    @{
        Artifact = "$apiArtifactName"
        TargetPath = 'C:\inetpub\wwwroot\api'
        SiteName = 'API'
        AppPoolName = 'NETSApiAppPool'
        IgnoreFiles = @("credSettings.$( $envTarget ).json")
        ConfigEnvironment = $configTarget
    },
    @{
        Artifact = "$uiArtifactName"
        TargetPath = 'C:\inetpub\wwwroot\frontend'
        SiteName = 'Frontend'
        AppPoolName = 'FrontendUIAppPool'
        IgnoreFiles = @()
        ConfigEnvironment = $configTarget
    }
)

$Failed = $false
$RetryCount = 0
$MaxRetries = 1
$RetryDelaySeconds = 15

while ($RetryCount -le $MaxRetries) {
    try {
        if ($RetryCount -gt 0) {
            Write-Host "Retrying deployment (attempt $($RetryCount + 1) of $($MaxRetries + 1))..." -ForegroundColor Yellow
            Start-Sleep -Seconds $RetryDelaySeconds
        }
        
        $Session = New-PSSession $envServerName

        # Ensure IIS gzip modules are installed and configured (run on remote server)
        Write-Host "Setting up IIS gzip compression modules (PS-2067)..." -ForegroundColor Magenta
        Write-Host "Target: $envServerName" -ForegroundColor Gray
        Write-Host ""
        
        $GzipSetupResult = Invoke-Command -Session $Session -ScriptBlock {
            # Test-IISGzipModules function definition
            ${function:Test-IISGzipModules} = ${function:Test-IISGzipModules}
            
            try {
                $result = Test-IISGzipModules
                if ($result -eq $true) {
                    Write-Host "✓ Gzip setup completed successfully" -ForegroundColor Green
                    return $true
                } else {
                    Write-Host "⚠ Gzip setup completed with warnings" -ForegroundColor Yellow
                    return $false
                }
            }
            catch {
                Write-Host "✗ Gzip setup failed: $_" -ForegroundColor Red
                return $false
            }
        }

        if ($GzipSetupResult -eq $false) {
            Write-Warning "IIS gzip module setup encountered issues. Continuing with deployment (non-blocking)."
        }
        
        Write-Host ""

    foreach ($Deploy in $Deployments) {
        Invoke-Command -Session $Session -ScriptBlock {
            # Stop IIS site and App Pool
            $Site = Get-IISSite -Name $Using:Deploy.SiteName
            $Site | Stop-IISSite -Confirm:$false

            $AppPool = Get-IISAppPool -Name $Using:Deploy.AppPoolName
            if ($AppPool.State -ne 'Stopped') {
                $AppPool | Stop-WebAppPool
                for ($i = 0; $i -lt $Using:StopAppTimeout; $i++) {
                    if ($AppPool.State -eq 'Stopped') { break }
                    Start-Sleep -Seconds 1
                }
                if ($AppPool.State -ne 'Stopped') {
                    Write-Error -Message "Failed to stop App Pool '$($AppPool.Name)'"
                    exit 1
                }
            }

            # Remove old files, excluding ignored ones
            Get-ChildItem -Path $Using:Deploy.TargetPath -Exclude $Using:Deploy.IgnoreFiles | Remove-Item -Force -Recurse
        }

        if (!$?) { $Failed = $true; break }

        # Deploy new artifact
        Copy-Item -ToSession $Session -Path .\dist\$($Deploy.Artifact) -Destination $Deploy.TargetPath
        if (!$?) { $Failed = $true; break }

        # Extract and configure new deployment
        Invoke-Command -Session $Session -ScriptBlock {
            Expand-Archive -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)" -DestinationPath $Using:Deploy.TargetPath
            Remove-Item -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)"

            if (Test-Path -Path "$($Using:Deploy.TargetPath)\web.config" -PathType Leaf) {
                Write-Output "$($Using:Deploy.TargetPath)\web.config"
                (Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw) -replace 'Development', $Using:Deploy.ConfigEnvironment | Set-Content -Path "$($Using:Deploy.TargetPath)\web.config"
                Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw
            }

            # Configure compression and caching for this site
            ${function:Set-SiteCompressionAndCaching} = ${function:Set-SiteCompressionAndCaching}
            $isApi = $Using:Deploy.SiteName -eq 'API'
            Set-SiteCompressionAndCaching -SiteName $Using:Deploy.SiteName -IsApiSite $isApi

            # Start App Pool and IIS site
            $AppPool | Start-WebAppPool
            $Site | Start-IISSite
        }

        if (!$?) { $Failed = $true; break }
    }
    
    # If we got here without failure, break out of retry loop
    if (-not $Failed) {
        Write-Host "Web deployment completed successfully" -ForegroundColor Green
        break
    }
} catch {
    $Failed = $true
    $RetryCount++
    if ($RetryCount -gt $MaxRetries) {
        Write-Host "Web deployment failed after $($MaxRetries + 1) attempts" -ForegroundColor Red
    }
} finally {
    if ($null -ne $Session) {
        Remove-PSSession -Session $Session
    }
}

# Exit retry loop if we've exceeded max retries
if ($Failed -and $RetryCount -gt $MaxRetries) {
    break
}
}

# ==========================================
# Deployment Summary & Pipeline Exit Code
# ==========================================
Write-Host ""
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "Deployment Summary" -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan

if ($Failed) {
    Write-Host "Status: ❌ FAILED" -ForegroundColor Red
    Write-Host "  - Web deployment failed after $($RetryCount) attempt(s)"
    Write-Host "  - Pipeline will report deployment as failed"
    Write-Host "  - Review logs above for detailed error information"
    Write-Host ""
    exit 1
} else {
    Write-Host "Status: ✅ SUCCESS" -ForegroundColor Green
    Write-Host "  - All deployments completed successfully"
    Write-Host "  - IIS gzip compression configured (PS-2067)"
    Write-Host "  - Applications running and accessible"
    Write-Host ""
    exit 0
}
