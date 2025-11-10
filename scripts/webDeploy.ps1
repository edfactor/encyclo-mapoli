$ErrorActionPreference = 'Stop'$ErrorActionPreference = 'Stop'

$VerbosePreference = 'Continue'$VerbosePreference = 'Continue'



# ==========================================# ==========================================

# Deployment Configuration# Deployment Configuration

# ==========================================# ==========================================

$StopAppTimeout = 10$StopAppTimeout = 10

$envTarget = $args[0]$envTarget = $args[0]

$envServerName = $args[1]$envServerName = $args[1]

$apiArtifactName = $args[2]$apiArtifactName = $args[2]

$uiArtifactName = $args[3]$uiArtifactName = $args[3]

$configTarget = ''$configTarget = ''



# Pipeline-aware logging# Pipeline-aware logging

$PipelineLogging = @{$PipelineLogging = @{

    Build = $env:BITBUCKET_BUILD_NUMBER    Build = $env:BITBUCKET_BUILD_NUMBER

    Branch = $env:BITBUCKET_BRANCH    Branch = $env:BITBUCKET_BRANCH

    Commit = $env:BITBUCKET_COMMIT    Commit = $env:BITBUCKET_COMMIT

}}



Write-Host "=====================================================================" -ForegroundColor CyanWrite-Host "=====================================================================" -ForegroundColor Cyan

Write-Host "Web Deployment Script - PS-2067 IIS Gzip Configuration" -ForegroundColor CyanWrite-Host "Web Deployment Script - PS-2067 IIS Gzip Configuration Integrated" -ForegroundColor Cyan

Write-Host "=====================================================================" -ForegroundColor CyanWrite-Host "=====================================================================" -ForegroundColor Cyan

if ($PipelineLogging.Build) {if ($PipelineLogging.Build) {

    Write-Host "Pipeline Build: $($PipelineLogging.Build) | Branch: $($PipelineLogging.Branch)" -ForegroundColor Gray    Write-Host "Pipeline Build: $($PipelineLogging.Build) | Branch: $($PipelineLogging.Branch)" -ForegroundColor Gray

}}

Write-Host ""Write-Host ""



function Get-ConfigEnvironment($envTargetVar) {# Load IIS Gzip Configuration functions from IISGzipConfiguration.ps1

    Write-Host "The value for envTargetVar is $($envTargetVar)"$IISGzipConfigPath = Join-Path (Split-Path -Parent $PSCommandPath) 'IISGzipConfiguration.ps1'

    switch ($envTargetVar) {Write-Host "Looking for IISGzipConfiguration.ps1 at: $IISGzipConfigPath" -ForegroundColor Gray

        'qa'    { Write-Host "ENV is QA"; return "QA" }if (Test-Path $IISGzipConfigPath) {

        'uat'   { Write-Host "ENV is UAT"; return "UAT" }    Write-Host "Found IISGzipConfiguration.ps1, loading..." -ForegroundColor Gray

        'prod'  { Write-Host "ENV is PRODUCTION"; return "Production" }    try {

        default { Write-Host "Unknown ENV" }        . $IISGzipConfigPath

    }        Write-Host "✓ Loaded IIS Gzip Configuration functions successfully." -ForegroundColor Green

}    }

    catch {

$configTarget = Get-ConfigEnvironment $envTarget        Write-Error "Failed to load IISGzipConfiguration.ps1: $_"

        exit 1

$Deployments = @(    }

    @{} else {

        Artifact = "$apiArtifactName"    Write-Error "IISGzipConfiguration.ps1 not found at $IISGzipConfigPath"

        TargetPath = 'C:\inetpub\wwwroot\api'    exit 1

        SiteName = 'API'}

        AppPoolName = 'NETSApiAppPool'

        IgnoreFiles = @("credSettings.$( $envTarget ).json")function Get-ConfigEnvironment($envTargetVar) {

        ConfigEnvironment = $configTarget    Write-Host "The value for envTargetVar is $($envTargetVar)"

    },    switch ($envTargetVar) {

    @{        'qa'    { Write-Host "ENV is QA"; return "QA" }

        Artifact = "$uiArtifactName"        'uat'   { Write-Host "ENV is UAT"; return "UAT" }

        TargetPath = 'C:\inetpub\wwwroot\frontend'        'prod'  { Write-Host "ENV is PRODUCTION"; return "Production" }

        SiteName = 'Frontend'        default { Write-Host "Unknown ENV" }

        AppPoolName = 'FrontendUIAppPool'    }

        IgnoreFiles = @()}

        ConfigEnvironment = $configTarget

    }$configTarget = Get-ConfigEnvironment $envTarget

)

$Deployments = @(

$Failed = $false    @{

$RetryCount = 0        Artifact = "$apiArtifactName"

$MaxRetries = 1        TargetPath = 'C:\inetpub\wwwroot\api'

$RetryDelaySeconds = 15        SiteName = 'API'

        AppPoolName = 'NETSApiAppPool'

while ($RetryCount -le $MaxRetries) {        IgnoreFiles = @("credSettings.$( $envTarget ).json")

    try {        ConfigEnvironment = $configTarget

        if ($RetryCount -gt 0) {    },

            Write-Host "Retrying deployment (attempt $($RetryCount + 1) of $($MaxRetries + 1))..." -ForegroundColor Yellow    @{

            Start-Sleep -Seconds $RetryDelaySeconds        Artifact = "$uiArtifactName"

        }        TargetPath = 'C:\inetpub\wwwroot\frontend'

                SiteName = 'Frontend'

        $Session = New-PSSession $envServerName        AppPoolName = 'FrontendUIAppPool'

        IgnoreFiles = @()

        # PS-2067: Setup IIS gzip compression on remote server        ConfigEnvironment = $configTarget

        Write-Host "Setting up IIS gzip compression modules (PS-2067)..." -ForegroundColor Magenta    }

        Write-Host "Target: $envServerName" -ForegroundColor Gray)

        

        Invoke-Command -Session $Session -ScriptBlock {$Failed = $false

            try {$RetryCount = 0

                # Check if this is Windows Server with IIS features available$MaxRetries = 1

                $osInfo = Get-CimInstance -ClassName CIM_OperatingSystem -ErrorAction Stop$RetryDelaySeconds = 15

                if ($osInfo.Caption -notmatch "Server") {

                    Write-Warning "This server does not appear to be Windows Server. Gzip configuration may not work."while ($RetryCount -le $MaxRetries) {

                    return    try {

                }        if ($RetryCount -gt 0) {

                            Write-Host "Retrying deployment (attempt $($RetryCount + 1) of $($MaxRetries + 1))..." -ForegroundColor Yellow

                # Import WebAdministration module            Start-Sleep -Seconds $RetryDelaySeconds

                Import-Module WebAdministration -ErrorAction Stop        }

                        

                Write-Host "Checking IIS compression modules..." -ForegroundColor Yellow        $Session = New-PSSession $envServerName

                

                # Check Dynamic Compression        # Ensure IIS gzip modules are installed and configured (run on remote server)

                $DynamicCompressionFeature = Get-WindowsFeature -Name Web-Dyn-Compression -ErrorAction SilentlyContinue        Write-Host "Setting up IIS gzip compression modules (PS-2067)..." -ForegroundColor Magenta

                if ($DynamicCompressionFeature.InstallState -ne 'Installed') {        Write-Host "Target: $envServerName" -ForegroundColor Gray

                    Write-Host "Installing Dynamic Compression module..." -ForegroundColor Yellow        Write-Host ""

                    Install-WindowsFeature -Name Web-Dyn-Compression -IncludeManagementTools | Out-Null        

                    Write-Host "✓ Dynamic Compression module installed." -ForegroundColor Green        # Verify that the gzip function is available before trying to invoke it

                } else {        if (-not (Get-Command Test-IISGzipModules -ErrorAction SilentlyContinue)) {

                    Write-Host "✓ Dynamic Compression module is already installed." -ForegroundColor Green            Write-Error "Test-IISGzipModules function not available - IISGzipConfiguration.ps1 may not have loaded correctly"

                }            exit 1

                        }

                # Check Static Compression        

                $StaticCompressionFeature = Get-WindowsFeature -Name Web-Stat-Compression -ErrorAction SilentlyContinue        $GzipSetupResult = Invoke-Command -Session $Session -ScriptBlock {

                if ($StaticCompressionFeature.InstallState -ne 'Installed') {            # Pass the Test-IISGzipModules function to remote session

                    Write-Host "Installing Static Compression module..." -ForegroundColor Yellow            ${function:Test-IISGzipModules} = $args[0]

                    Install-WindowsFeature -Name Web-Stat-Compression -IncludeManagementTools | Out-Null            

                    Write-Host "✓ Static Compression module installed." -ForegroundColor Green            # Verify function was passed correctly

                } else {            if (-not (Get-Command Test-IISGzipModules -ErrorAction SilentlyContinue)) {

                    Write-Host "✓ Static Compression module is already installed." -ForegroundColor Green                Write-Host "✗ Test-IISGzipModules function not available in remote session" -ForegroundColor Red

                }                return $false

                            }

                # Configure server-level compression settings            

                Write-Host "Configuring server-level gzip settings..." -ForegroundColor Cyan            try {

                                $result = Test-IISGzipModules

                # Set compression directory                if ($result -eq $true) {

                Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `                    Write-Host "✓ Gzip setup completed successfully" -ForegroundColor Green

                    -Name 'directory' -Value '%SystemDrive%\inetpub\temp\IIS Temporary Compressed Files' -ErrorAction Stop                    return $true

                                } else {

                # Enable Dynamic Compression before caching                    Write-Host "⚠ Gzip setup completed with warnings" -ForegroundColor Yellow

                Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `                    return $false

                    -Name 'dynamicCompressionBeforeCaching' -Value $true -ErrorAction Stop                }

                            }

                # Set minimum file size for compression to 1KB            catch {

                Set-WebConfigurationProperty -PSPath 'MACHINE/WEBROOT/APPHOST' -Filter 'system.webServer/httpCompression' `                Write-Host "✗ Gzip setup failed: $_" -ForegroundColor Red

                    -Name 'minFileSizeForComp' -Value 1024 -ErrorAction Stop                return $false

                            }

                Write-Host "✓ Server-level compression configured (1KB threshold)" -ForegroundColor Green        } -ArgumentList ${function:Test-IISGzipModules}

                

                # Enable compression for common content types        if ($GzipSetupResult -eq $false) {

                $ContentTypes = @(            Write-Warning "IIS gzip module setup encountered issues. Continuing with deployment (non-blocking)."

                    'text/plain', 'text/html', 'text/xml', 'text/css', 'text/javascript',        }

                    'text/*', 'message/*', 'application/javascript', 'application/json',        

                    'application/xml', 'application/atom+xml', 'application/x-www-form-urlencoded'        Write-Host ""

                )

                        foreach ($Deploy in $Deployments) {

                $existingDynamicTypes = (Get-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `            Write-Host "Deploying $($Deploy.SiteName)..." -ForegroundColor Cyan

                    -Filter 'system.webServer/httpCompression/dynamicTypes').Collection            

                            Invoke-Command -Session $Session -ScriptBlock {

                foreach ($contentType in $ContentTypes) {                # Stop IIS site and App Pool

                    $exists = $existingDynamicTypes | Where-Object { $_.mimeType -eq $contentType }                $Site = Get-IISSite -Name $Using:Deploy.SiteName

                    if (-not $exists) {                $Site | Stop-IISSite -Confirm:$false

                        Add-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' `

                            -Filter 'system.webServer/httpCompression/dynamicTypes' `                $AppPool = Get-IISAppPool -Name $Using:Deploy.AppPoolName

                            -Value @{ mimeType = $contentType; enabled = $true } -ErrorAction SilentlyContinue                if ($AppPool.State -ne 'Stopped') {

                    }                    $AppPool | Stop-WebAppPool

                }                    for ($i = 0; $i -lt $Using:StopAppTimeout; $i++) {

                                        if ($AppPool.State -eq 'Stopped') { break }

                Write-Host "✓ Content types configured for compression" -ForegroundColor Green                        Start-Sleep -Seconds 1

            }                    }

            catch {                    if ($AppPool.State -ne 'Stopped') {

                Write-Warning "Gzip setup encountered an issue (non-blocking): $_"                        Write-Error -Message "Failed to stop App Pool '$($AppPool.Name)'"

            }                        exit 1

        }                    }

                        }

        Write-Host ""

                # Remove old files, excluding ignored ones

        foreach ($Deploy in $Deployments) {                Get-ChildItem -Path $Using:Deploy.TargetPath -Exclude $Using:Deploy.IgnoreFiles | Remove-Item -Force -Recurse

            Write-Host "Deploying $($Deploy.SiteName)..." -ForegroundColor Cyan            }

            

            Invoke-Command -Session $Session -ScriptBlock {            if (!$?) { $Failed = $true; break }

                # Stop IIS site and App Pool

                $Site = Get-IISSite -Name $Using:Deploy.SiteName            # Deploy new artifact

                $Site | Stop-IISSite -Confirm:$false            Write-Host "  Copying artifact to server..." -ForegroundColor Gray

            Copy-Item -ToSession $Session -Path .\dist\$($Deploy.Artifact) -Destination $Deploy.TargetPath

                $AppPool = Get-IISAppPool -Name $Using:Deploy.AppPoolName            if (!$?) { $Failed = $true; break }

                if ($AppPool.State -ne 'Stopped') {

                    $AppPool | Stop-WebAppPool            # Extract and configure new deployment

                    for ($i = 0; $i -lt $Using:StopAppTimeout; $i++) {            Write-Host "  Extracting and configuring deployment..." -ForegroundColor Gray

                        if ($AppPool.State -eq 'Stopped') { break }            Invoke-Command -Session $Session -ScriptBlock {

                        Start-Sleep -Seconds 1                Expand-Archive -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)" -DestinationPath $Using:Deploy.TargetPath

                    }                Remove-Item -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)"

                    if ($AppPool.State -ne 'Stopped') {

                        Write-Error -Message "Failed to stop App Pool '$($AppPool.Name)'"                if (Test-Path -Path "$($Using:Deploy.TargetPath)\web.config" -PathType Leaf) {

                        exit 1                    Write-Output "$($Using:Deploy.TargetPath)\web.config"

                    }                    (Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw) -replace 'Development', $Using:Deploy.ConfigEnvironment | Set-Content -Path "$($Using:Deploy.TargetPath)\web.config"

                }                    Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw

                }

                # Remove old files, excluding ignored ones

                Get-ChildItem -Path $Using:Deploy.TargetPath -Exclude $Using:Deploy.IgnoreFiles | Remove-Item -Force -Recurse                # Configure compression and caching for this site

            }                ${function:Set-SiteCompressionAndCaching} = $args[0]

                

            if (!$?) { $Failed = $true; break }                # Verify function was passed correctly

                if (-not (Get-Command Set-SiteCompressionAndCaching -ErrorAction SilentlyContinue)) {

            # Deploy new artifact                    Write-Host "✗ Set-SiteCompressionAndCaching function not available in remote session" -ForegroundColor Red

            Write-Host "  Copying artifact..." -ForegroundColor Gray                    exit 1

            Copy-Item -ToSession $Session -Path .\dist\$($Deploy.Artifact) -Destination $Deploy.TargetPath                }

            if (!$?) { $Failed = $true; break }                

                $isApi = $Using:Deploy.SiteName -eq 'API'

            # Extract and configure new deployment                Set-SiteCompressionAndCaching -SiteName $Using:Deploy.SiteName -IsApiSite $isApi

            Write-Host "  Extracting and configuring..." -ForegroundColor Gray

            Invoke-Command -Session $Session -ScriptBlock {                # Start App Pool and IIS site

                Expand-Archive -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)" -DestinationPath $Using:Deploy.TargetPath                $AppPool | Start-WebAppPool

                Remove-Item -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)"                $Site | Start-IISSite

            } -ArgumentList ${function:Set-SiteCompressionAndCaching}

                if (Test-Path -Path "$($Using:Deploy.TargetPath)\web.config" -PathType Leaf) {

                    Write-Output "$($Using:Deploy.TargetPath)\web.config"            if (!$?) { $Failed = $true; break }

                    (Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw) -replace 'Development', $Using:Deploy.ConfigEnvironment | Set-Content -Path "$($Using:Deploy.TargetPath)\web.config"            

                    Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw            Write-Host "  ✓ $($Deploy.SiteName) deployment completed" -ForegroundColor Green

                }        }

                        

                # Configure compression and caching for this site        # If we got here without failure, break out of retry loop

                try {        if (-not $Failed) {

                    $sitePath = "IIS:\Sites\$($Using:Deploy.SiteName)"            Write-Host ""

                                Write-Host "Web deployment completed successfully" -ForegroundColor Green

                    # Enable HTTP caching and kernel cache            break

                    Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `        }

                        -Name 'enabled' -Value $true -ErrorAction Stop    }

                    Set-WebConfigurationProperty -PSPath $sitePath -Filter 'system.webServer/caching' `    catch {

                        -Name 'enableKernelCache' -Value $true -ErrorAction Stop        Write-Error "Deployment error: $_"

                            $Failed = $true

                    Write-Host "  ✓ Compression and caching configured for $($Using:Deploy.SiteName)" -ForegroundColor Green        $RetryCount++

                }        if ($RetryCount -le $MaxRetries) {

                catch {            Write-Host "Retrying in $RetryDelaySeconds seconds..." -ForegroundColor Yellow

                    Write-Warning "Could not configure compression/caching for site: $_"        }

                }    }

    finally {

                # Start App Pool and IIS site        if ($null -ne $Session) {

                $AppPool | Start-WebAppPool            Remove-PSSession -Session $Session

                $Site | Start-IISSite        }

            }    }



            if (!$?) { $Failed = $true; break }    # Exit retry loop if we've exceeded max retries

        }    if ($Failed -and $RetryCount -gt $MaxRetries) {

                break

        # If we got here without failure, break out of retry loop    }

        if (-not $Failed) {}

            Write-Host ""

            Write-Host "Web deployment completed successfully" -ForegroundColor Green# ==========================================

            break# Deployment Summary & Pipeline Exit Code

        }# ==========================================

    }Write-Host ""

    catch {Write-Host "=====================================================================" -ForegroundColor Cyan

        Write-Error "Deployment error: $_"Write-Host "Deployment Summary" -ForegroundColor Cyan

        $Failed = $trueWrite-Host "=====================================================================" -ForegroundColor Cyan

        $RetryCount++

        if ($RetryCount -le $MaxRetries) {if ($Failed) {

            Write-Host "Retrying in $RetryDelaySeconds seconds..." -ForegroundColor Yellow    Write-Host "Status: ❌ FAILED" -ForegroundColor Red

        }    Write-Host "  - Web deployment failed after $($RetryCount) attempt(s)"

    }    Write-Host "  - Pipeline will report deployment as failed"

    finally {    Write-Host "  - Review logs above for detailed error information"

        if ($null -ne $Session) {    Write-Host ""

            Remove-PSSession -Session $Session    exit 1

        }} else {

    }    Write-Host "Status: ✅ SUCCESS" -ForegroundColor Green

    Write-Host "  - All deployments completed successfully"

    # Exit retry loop if we've exceeded max retries    Write-Host "  - IIS gzip compression configured (PS-2067)"

    if ($Failed -and $RetryCount -gt $MaxRetries) {    Write-Host "  - Applications running and accessible"

        break    Write-Host ""

    }    exit 0

}}


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
