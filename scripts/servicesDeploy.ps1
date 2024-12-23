$envTarget = $args[0]
$envServerName = $args[1]
$configTarget = ''

function Get-ConfigEnvironment($envTargetVar) {
    Write-Host "The value for envTargetVar is $($envTargetVar)"
    switch ($envTargetVar) {
        "qa" { Write-Host "ENV is QA"; return "QA" }
        "uat" { Write-Host "ENV is UAT"; return "UAT" }
        "prod" { Write-Host "ENV is PRODUCTION"; return "Production" }
        default { Write-Host "Unknown ENV" }
    }
}

function Deploy-Service($Artifact, $TargetPath, $ServiceExecutable, $ServiceName, $DisplayName, $ConfigEnvironment, $Session) {
    Invoke-Command -Session $Session -ScriptBlock {
        $ServiceInfo = Get-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue
        if ($ServiceInfo) {
            Write-Host "Attempting to Stop Running Service" $ServiceInfo
            Stop-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue
            while ((Get-Service -Name $Using:ServiceName).Status -ne 'Stopped') {
                Write-Host "Waiting for Service to stop."
                Start-Sleep 2
            }
            Write-Host 'Stopped Service'
        }

        Write-Host "Checking existence of folder $($Using:TargetPath)"
        if (Test-Path -Path $Using:TargetPath) {
            Get-ChildItem -Path $Using:TargetPath -Exclude "credSettings.$($Using:ConfigEnvironment).json" | Remove-Item -Force -Recurse
        } else {
            Write-Host "Creating folder $($Using:TargetPath)"
            New-Item -ItemType Directory -Force -Path $Using:TargetPath
        }
    }

    if (!$?) { return $false }

    Write-Host 'Copying artifacts'
    Copy-Item -ToSession $Session -Path .\dist\$Artifact -Destination $TargetPath
    if (!$?) { return $false }

    Invoke-Command -Session $Session -ScriptBlock {
        Expand-Archive -Force -Path "$($Using:TargetPath)\$Artifact" -DestinationPath $Using:TargetPath
        Remove-Item -Force -Path "$($Using:TargetPath)\$Artifact"

        $file = "$($Using:TargetPath)\buildSettings.json"
        $json = Get-Content -Raw $file | ConvertFrom-Json
        $json.environment = $Using:ConfigEnvironment
        $json | ConvertTo-Json -Depth 10 | Set-Content $file

        if (!(Get-Service -Name $Using:ServiceName -ErrorAction SilentlyContinue)) {
            Write-Host 'Installing service'
            New-Service -Name $Using:ServiceName -BinaryPathName "$($Using:ServiceExecutable)" -StartupType "AutomaticDelayedStart"
        }

        Start-Service -Name $Using:ServiceName
    }

    return $?
}

$configTarget = Get-ConfigEnvironment $envTarget

$Failed = $false

try {
    $Session = New-PSSession $envServerName

    $Deployments = @(
        @{ Artifact = 'Demoulas.Smart.FileHandler.zip'; TargetPath = 'C:\SmartFileHandler'; ServiceExecutable = 'C:\SmartFileHandler\Demoulas.Smart.FileHandler.exe'; ServiceName = 'Demoulas Smart File Handler'; DisplayName = 'Demoulas Smart File Handler' },
        @{ Artifact = 'Demoulas.Smart.IngestionServices.zip'; TargetPath = 'C:\SmartFileIngestion'; ServiceExecutable = 'C:\SmartFileIngestion\Demoulas.Smart.IngestionServices.exe'; ServiceName = 'Demoulas Smart File Ingestion'; DisplayName = 'Demoulas Smart File Ingestion' }
    )

    foreach ($Deploy in $Deployments) {
        $result = Deploy-Service -Artifact $Deploy.Artifact -TargetPath $Deploy.TargetPath -ServiceExecutable $Deploy.ServiceExecutable -ServiceName $Deploy.ServiceName -DisplayName $Deploy.DisplayName -ConfigEnvironment $configTarget -Session $Session
        if (!$result) { $Failed = $true; break }
    }
} catch {
    $Failed = $true
} finally {
    if ($Session) {
        Remove-PSSession -Session $Session
    }
}

if ($Failed) { exit 1 }
