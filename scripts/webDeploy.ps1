$StopAppTimeout = 10
$envTarget = $args[0]
$envServerName = $args[1]
$apiArtifactName = $args[2]
$uiArtifactName = $args[3]
$configTarget = ''

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
        Artifact = "$apiArtifactName";
       TargetPath = 'C:\inetpub\wwwroot\api';
       SiteName = 'API';
       AppPoolName = 'NETSApiAppPool';
       IgnoreFiles = @("credSettings.$($envTarget).json");
       ConfigEnvironment = $configTarget },

    @{
        Artifact = "$uiArtifactName";
       TargetPath = 'C:\inetpub\wwwroot\frontend';
       SiteName = 'Frontend';
       AppPoolName = 'FrontendUIAppPool';
       IgnoreFiles = @();
       ConfigEnvironment = $configTarget }
)

$Failed = $false
try {
    $Session = New-PSSession -ComputerName $envServerName

    # Update .NET workloads
    Invoke-Command -Session $Session -ScriptBlock { dotnet workload update }

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

            # Enable HTTP/2 if not already enabled
            Import-Module IISAdministration
            $binding = Get-IISSite -Name $Using:Deploy.SiteName | Get-IISSiteBinding -BindingInformation '*:8443:'
            if ($binding) {
                Set-IISSiteBinding -BindingInformation '*:8443:' -Protocol https -EnableHttp2 $true
                Write-Host "HTTP/2 enabled for site: $($Using:Deploy.SiteName)"
            } else {
                Write-Host "HTTPS binding not found for site: $($Using:Deploy.SiteName). Cannot enable HTTP/2."
            }
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
                (Get-Content -Path "$($Using:Deploy.TargetPath)\web.config" -Raw) -replace 'Development', $Using:Deploy.ConfigEnvironment | Set-Content -Path "$($Using:Deploy.TargetPath)\web.config"
                Get-Content -Path "$($Using:Deploy.TargetPath)\web.config" -Raw
            }

            # Start App Pool and IIS site
            $AppPool | Start-WebAppPool
            $Site | Start-IISSite
        }

        if (!$?) { $Failed = $true; break }
    }
} catch {
    $Failed = $true
} finally {
    if ($null -ne $Session) {
        Remove-PSSession -Session $Session
    }
}

if ($Failed) { exit 1 }
