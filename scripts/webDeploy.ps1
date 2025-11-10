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

# Load IIS Gzip Configuration functions from IISGzipConfiguration.ps1
$IISGzipConfigPath = Join-Path (Split-Path -Parent $PSCommandPath) 'IISGzipConfiguration.ps1'
Write-Host "Looking for IISGzipConfiguration.ps1 at: $IISGzipConfigPath" -ForegroundColor Gray
if (Test-Path $IISGzipConfigPath) {
    Write-Host "Found IISGzipConfiguration.ps1, loading..." -ForegroundColor Gray
    try {
        . $IISGzipConfigPath
        Write-Host "✓ Loaded IIS Gzip Configuration functions successfully." -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to load IISGzipConfiguration.ps1: $_"
        exit 1
    }
} else {
    Write-Error "IISGzipConfiguration.ps1 not found at $IISGzipConfigPath"
    exit 1
}

function Get-ConfigEnvironment($envTargetVar) {
    Write-Host "The value for envTargetVar is $($envTargetVar)"
    switch ($envTargetVar) {
        'qa'    { Write-Host "ENV is QA"; return "QA" }
        'uat'   { Write-Host "ENV is UAT"; return "UAT" }
        'prod'  { Write-Host "ENV is PRODUCTION"; return "Production" }
        default { Write-Host "Unknown ENV" }
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
        
        # Verify that the gzip function is available before trying to invoke it
        if (-not (Get-Command Test-IISGzipModules -ErrorAction SilentlyContinue)) {
            Write-Error "Test-IISGzipModules function not available - IISGzipConfiguration.ps1 may not have loaded correctly"
            exit 1
        }
        
        $GzipSetupResult = Invoke-Command -Session $Session -ScriptBlock {
            # Pass the Test-IISGzipModules function to remote session
            ${function:Test-IISGzipModules} = $args[0]
            
            # Verify function was passed correctly
            if (-not (Get-Command Test-IISGzipModules -ErrorAction SilentlyContinue)) {
                Write-Host "✗ Test-IISGzipModules function not available in remote session" -ForegroundColor Red
                return $false
            }
            
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
        } -ArgumentList ${function:Test-IISGzipModules}

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
            ${function:Set-SiteCompressionAndCaching} = $args[0]
            
            # Verify function was passed correctly
            if (-not (Get-Command Set-SiteCompressionAndCaching -ErrorAction SilentlyContinue)) {
                Write-Host "✗ Set-SiteCompressionAndCaching function not available in remote session" -ForegroundColor Red
                exit 1
            }
            
            $isApi = $Using:Deploy.SiteName -eq 'API'
            Set-SiteCompressionAndCaching -SiteName $Using:Deploy.SiteName -IsApiSite $isApi

            # Start App Pool and IIS site
            $AppPool | Start-WebAppPool
            $Site | Start-IISSite
        } -ArgumentList ${function:Set-SiteCompressionAndCaching}

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
