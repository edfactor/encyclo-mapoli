$StopAppTimeout = 5
$envTarget = $args[0]
$envServerName = $args[1]
$configTarget = ''

function get-ConfigEnvironment($envTargetVar)
{
    Write-Host "The value for envTargetVar is $( $envTargetVar )"
    if ($envTargetVar -eq "qa")
    {
        Write-Host "ENV is QA"
        return "QA"
    }
    elseif ($envTargetVar -eq "uat")
    {
        Write-Host "ENV is UAT"
        return "UAT"
    }
    elseif ($envTargetVar -eq "prod")
    {
        Write-Host "ENV is PRODUCTION"
        return "Production"
    }
    else
    {
        Write-Host "Unknown ENV"
    }
}

$configTarget = get-ConfigEnvironment $envTarget

$Deployments = @(
    @{
        Artifact = 'Demoulas.ProfitSharing.OracleHcm.Sync.zip'
        TargetPath = 'C:\NextGenApplications\ps'
        ServiceExecutable = 'C:\NextGenApplications\ps\Demoulas.ProfitSharing.OracleHcm.Sync.exe'
        ServiceName = 'Demoulas Smart File Handler'
        DisplayName = 'Demoulas Smart File Handler'
        IgnoreFiles = @("credSettings.$( $envTarget ).json")
        ConfigEnvironment = $configTarget
    }
)

$Failed = $false
try {
    $Session = New-PSSession $envServerName


    foreach ($Deploy in $Deployments)
    {
        Write-Host $Deploy
        Invoke-Command -Session $Session -ScriptBlock {
            $ServiceInfo = Get-Service -Name $Using:Deploy.ServiceName -ErrorAction SilentlyContinue
            if (($ServiceInfo -ne $null) -and ($ServiceInfo.Length -gt 0))
            {
                $ServiceInfo = Get-Service -Name $Using:Deploy.ServiceName | Where-Object { $_.Status -eq "Running" } -ErrorAction SilentlyContinue
                Write-Host "Attempting to Stop Running Service" $ServiceInfo
                if (($ServiceInfo -ne $null) -and ($ServiceInfo.Length -gt 0))
                {
                    Write-Host 'Stopping Service'
                    Stop-Service -Name $Using:Deploy.ServiceName -verbose -ErrorAction SilentlyContinue
                    while ((Get-Service -Name $Using:Deploy.ServiceName).Status -ne 'Stopped')
                    {
                        Write-Host "Waiting for Service " $ServiceInfo " to stop."
                        Start-Sleep 2
                    }
                    Write-Host 'Stopped Service'
                    $ServiceInfo.Refresh()
                }
            }

            Write-Host "Checking existence of folder" $Using:Deploy.TargetPath
            $FolderExists = Test-Path -Path $Using:Deploy.TargetPath
            if ($FolderExists -eq $true)
            {
                Get-ChildItem -Path $Using:Deploy.TargetPath -Exclude $Using:Deploy.IgnoreFiles | Remove-Item -Force -Recurse
            }
            else
            {
                Write-Host "creating new folder" $Using:Deploy.TargetPath
                New-Item -ItemType Directory -Force -Path $Using:Deploy.TargetPath
            }

        }
        if (!$?)
        {
            $Failed = $true; break
        }


        Write-Host 'copying artifacts'
        Copy-Item -ToSession $Session -Path .\dist\$($Deploy.Artifact) -Destination $Deploy.TargetPath
        if (!$?)
        {
            $Failed = $true; break
        }

        Write-Host 'copied artifacts'

        # Set the environment variable on the remote machine
        Invoke-Command -Session $Session -ScriptBlock {
            [System.Environment]::SetEnvironmentVariable("DOTNET_ENVIRONMENT", $Using:Deploy.ConfigEnvironment, [System.EnvironmentVariableTarget]::Machine)
        }

        Invoke-Command -Session $Session -ScriptBlock {
            Expand-Archive -Force -Path "$( $Using:Deploy.TargetPath )\$( $Using:Deploy.Artifact )" -DestinationPath $Using:Deploy.TargetPath
            Remove-Item -Force -Path "$( $Using:Deploy.TargetPath )\$( $Using:Deploy.Artifact )"

            $ServiceInfo = Get-Service -Name $Using:Deploy.ServiceName -ErrorAction SilentlyContinue
            Write-Host $ServiceInfo
            if ($ServiceInfo -eq $null)
            {
                Write-Host 'Installing service'
                New-Service -Name $Using:Deploy.ServiceName -BinaryPathName "$( $Using:Deploy.ServiceExecutable )" -StartupType AutomaticDelayedStart
            }

            $ServiceInfo = Get-Service -Name $Using:Deploy.ServiceName
            Write-Host "Starting Service with param " $Using:Deploy.ConfigEnvironment
            $ServiceInfo.Start()
        }
        if (!$?)
        {
            $Failed = $true; break
        }
    }
}
catch
{
    $Failed = $true
}
finally
{
    if ($null -ne $Session)
    {
        Remove-PSSession -Session $Session
        $Session = $null
    }
}

if ($Failed)
{
    exit 1
}