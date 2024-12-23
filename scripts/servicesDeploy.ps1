param(
    [string]$envTarget,
    [string]$envServerName,
    [string]$backgroundServiceArtifact,
    [string]$backgroundServiceExecutable,
    [string]$backgroundServiceName,
    [string]$backgroundServiceTargetPath
)

# Function for environment configuration mapping
function Get-ConfigEnvironment($envTargetVar) {
    Write-Host "[$( Get-Date -Format 'yyyy-MM-dd HH:mm:ss' )] Mapping environment for: $( $envTargetVar )"
    switch ($envTargetVar) {
        "qa" {
            return "QA"
        }
        "uat" {
            return "UAT"
        }
        "prod" {
            return "Production"
        }
        default {
            Write-Error "Unknown environment '$envTargetVar'. Exiting."
            exit 1
        }
    }
}

# Function to deploy a service
function Deploy-Service($Artifact, $TargetPath, $ServiceExecutable, $ServiceName, $ConfigEnvironment, $Session)
{
    try
    {
        # Stop service (if it exists)
        Invoke-Command -Session $Session -ScriptBlock {
            $ServiceInfo = Get-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue
            if ($ServiceInfo)
            {
                Write-Host "Stopping running service: $Using:ServiceName"
                Stop-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue
                $maxAttempts = 30
                $attempt = 0
                while ((Get-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue).Status -ne 'Stopped' -and $attempt -lt $maxAttempts)
                {
                    Write-Host "Waiting for service to stop... ($( $attempt + 1 )/$maxAttempts)"
                    Start-Sleep -Seconds 2
                    $attempt++
                }
                if ((Get-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue).Status -ne 'Stopped')
                {
                    throw "Service $Using:ServiceName failed to stop within the timeout."
                }
            }
        }

        # Clean and prepare target path
        Invoke-Command -Session $Session -ScriptBlock {
            if (Test-Path -Path "$Using:TargetPath")
            {
                Write-Host "Cleaning target folder: $Using:TargetPath"
                Get-ChildItem -Path "$Using:TargetPath" -Exclude "credSettings.$( $Using:ConfigEnvironment ).json" | Remove-Item -Force -Recurse
            }
            else
            {
                Write-Host "Creating target folder: $Using:TargetPath"
                New-Item -ItemType Directory -Force -Path "$Using:TargetPath"
            }
        }

        # Copy artifacts
        Write-Host "Copying artifacts to target: $TargetPath"
        Copy-Item -ToSession $Session -Path ".\dist\$Artifact" -Destination $TargetPath -ErrorAction Stop

        # Extract, update config, and handle the service installation
        Invoke-Command -Session $Session -ScriptBlock {
            Expand-Archive -Force -Path "$Using:TargetPath\$Using:Artifact" -DestinationPath "$Using:TargetPath"
            Remove-Item -Force -Path "$Using:TargetPath\$Using:Artifact"

            # Update environment settings in the buildSettings file
            $file = "$Using:TargetPath\buildSettings.json"
            if (-not (Test-Path $file))
            {
                throw "buildSettings.json file not found at $Using:TargetPath"
            }
            $json = Get-Content -Raw $file | ConvertFrom-Json
            $json.environment = $Using:ConfigEnvironment
            $json | ConvertTo-Json -Depth 10 | Set-Content $file

            # Install and start the service if not already registered
            if (!(Get-Service -Name "$Using:ServiceName" -ErrorAction SilentlyContinue))
            {
                Write-Host "Installing service: $Using:ServiceName"
                $BinaryFullPath = Join-Path -Path "$Using:TargetPath" -ChildPath $Using:ServiceExecutable
                Write-Host "BinaryPathName is '$BinaryFullPath'"
                New-Service -Name "$Using:ServiceName" -BinaryPathName "$BinaryFullPath" -StartupType "AutomaticDelayedStart"
            }
            Start-Service -Name "$Using:ServiceName"
        }

        return $true
    }
    catch
    {
        Write-Error "Error during Deploy-Service execution: $_"
        return $false
    }
}

# Main Execution
Write-Host "[$( Get-Date -Format 'yyyy-MM-dd HH:mm:ss' )] Starting script execution."

# Validate parameters
if (-not $envTarget)
{
    Write-Error "Environment target (envTarget) not specified!"
    exit 1
}
if (-not $envServerName)
{
    Write-Error "Environment server name (envServerName) not specified!"
    exit 1
}
if (-not $backgroundServiceArtifact)
{
    Write-Error "Artifact for the background service not specified!"
    exit 1
}
if (-not $backgroundServiceExecutable)
{
    Write-Error "Background service executable path not specified!"
    exit 1
}
if (-not $backgroundServiceName)
{
    Write-Error "Background service name not specified!"
    exit 1
}
if (-not $backgroundServiceTargetPath)
{
    Write-Error "Target path for deployment not specified!"
    exit 1
}

$configTarget = Get-ConfigEnvironment $envTarget
$Failed = $false

try {
    Write-Host "[$( Get-Date -Format 'yyyy-MM-dd HH:mm:ss' )] Establishing session with server: $envServerName"
    $Session = New-PSSession -ComputerName $envServerName -ErrorAction Stop

    $Deployments = @(
        @{
            Artifact = $backgroundServiceArtifact
            TargetPath = $backgroundServiceTargetPath
            ServiceExecutable = $backgroundServiceExecutable
            ServiceName = $backgroundServiceName
        }
    )
    foreach ($Deploy in $Deployments) {
        $result = Deploy-Service -Artifact $Deploy.Artifact -TargetPath "$Deploy.TargetPath" -ServiceExecutable $Deploy.ServiceExecutable -ServiceName "$Deploy.ServiceName" -ConfigEnvironment $configTarget -Session $Session
        if (-not $result)
        {
            $Failed = $true
            break
        }
    }
} catch {
    Write-Error "Script encountered an error: $_"
    $Failed = $true
} finally {
    if ($Session) {
        Write-Host "Cleaning up session."
        Remove-PSSession -Session $Session
    }
}

if ($Failed)
{
    Write-Error "Deployment failed. Exiting with status code 1."
    exit 1
}
else
{
    Write-Host "Deployment completed successfully."
    exit 0
}