
param (
    [string]$envTarget,
    [string]$envServerName,
    [string]$profitSharingConnectionString,
    [int]$stopAppTimeout = 5,
    [switch]$Verbose
)

# Function to set environment variables on the target server
function Set-EnvironmentVariables {
    param (
        [string]$serverName,
        [string]$profitSharingConnectionString
    )

    Invoke-Command -ComputerName $serverName -ScriptBlock {
        param ($profitSharingConnectionString)
        [System.Environment]::SetEnvironmentVariable("ConnectionStrings:ProfitSharing", $profitSharingConnectionString, [System.EnvironmentVariableTarget]::Machine)
    } -ArgumentList $profitSharingConnectionString
}

# Function to get configuration environment
function Get-ConfigEnvironment {
    param (
        [string]$target
    )

    switch ($target) {
        "qa"    { return "QA" }
        "uat"   { return "UAT" }
        "prod"  { return "Production" }
        default { Write-Warning "Unknown environment: $target"; return $null }
    }
}

# Function to stop the IIS App Pool
function Stop-AppPool {
    param (
        [string]$appPoolName,
        [int]$timeout
    )

    try {
        Write-Host "Stopping IIS App Pool: $appPoolName"
        Stop-WebAppPool -Name $appPoolName
        Start-Sleep -Seconds $timeout
    } catch {
        Write-Error "Failed to stop IIS App Pool: $appPoolName"
        throw
    }
}

# Function to start the IIS App Pool
function Start-AppPool {
    param (
        [string]$appPoolName
    )

    try {
        Write-Host "Starting IIS App Pool: $appPoolName"
        Start-WebAppPool -Name $appPoolName
    } catch {
        Write-Error "Failed to start IIS App Pool: $appPoolName"
        throw
    }
}

# Function to deploy an artifact
function Deploy-Artifact {
    param (
        [string]$artifact,
        [string]$targetPath,
        [array]$ignoreFiles
    )

    try {
        Write-Host "Deploying $artifact to $targetPath"

        # Expand the archive to a temporary location
        $tempPath = Join-Path $env:TEMP "deploy_temp"
        Expand-Archive -Path $artifact -DestinationPath $tempPath -Force

        # Remove ignored files
        foreach ($ignoreFile in $ignoreFiles) {
            $fullPath = Join-Path $tempPath $ignoreFile
            if (Test-Path $fullPath) {
                Remove-Item -Path $fullPath -Force
                Write-Host "Ignored file: $fullPath"
            }
        }

        # Move the files to the target path
        Copy-Item -Path "$tempPath\*" -Destination $targetPath -Recurse -Force

        # Clean up temporary files
        Remove-Item -Path $tempPath -Recurse -Force
    } catch {
        Write-Error "Failed to deploy $artifact to $targetPath"
        throw
    }
}

# Function to validate the deployment setup
function Validate-Deployment {
    param (
        [string]$artifact,
        [string]$appPoolName
    )

    if (-not (Test-Path $artifact)) {
        Write-Error "Artifact not found: $artifact"
        throw "Artifact not found: $artifact"
    }

    try {
        Get-WebAppPoolState -Name $appPoolName | Out-Null
    } catch {
        Write-Error "IIS App Pool does not exist: $appPoolName"
        throw "IIS App Pool does not exist: $appPoolName"
    }
}

# Set environment variables on the server
Set-EnvironmentVariables -serverName $envServerName -profitSharingConnectionString $profitSharingConnectionString

# Get the config environment based on target
$configTarget = Get-ConfigEnvironment -target $envTarget

# Verbose output
if ($Verbose) {
    Write-Host "Deployment target: $envTarget"
    Write-Host "Server name: $envServerName"
    Write-Host "Config environment: $configTarget"
}

# Deployments for both API and UI
$Deployments = @(
    @{
        Artifact = "Demoulas.ProfitSharing.Api.zip"
        TargetPath = "C:\inetpub\wwwroot\api"
        SiteName = "API"
        AppPoolName = "NETSApiAppPool"
        ConfigEnvironment = $configTarget
    },
    @{
        Artifact = "Demoulas.ProfitSharing.UI.$($envTarget).zip"
        TargetPath = "C:\inetpub\wwwroot\frontend"
        SiteName = "Frontend"
        AppPoolName = "NETSFrontendAppPool"
        ConfigEnvironment = $configTarget
    }
)

# Validate, stop app pool, deploy, and restart app pool for each deployment
ForEach ($deployment in $Deployments) {
    Validate-Deployment -artifact $deployment.Artifact -appPoolName $deployment.AppPoolName

    Stop-AppPool -appPoolName $deployment.AppPoolName -timeout $stopAppTimeout
    
    Deploy-Artifact -artifact $deployment.Artifact -targetPath $deployment.TargetPath -ignoreFiles $deployment.IgnoreFiles
    
    Start-AppPool -appPoolName $deployment.AppPoolName
}

Write-Host "Deployment completed successfully."
