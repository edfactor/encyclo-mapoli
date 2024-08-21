$StopAppTimeout = 5
$envTarget = $args[0]
$envServerName = $args[1]
$configTarget = ''

function get-ConfigEnvironment($envTargetVar)
{
    Write-Host "The value for envTargetVar is $($envTargetVar)"
    if ($envTargetVar -eq "qa"){
        Write-Host "ENV is QA"
        return "QA"
    }
    elseif ($envTargetVar -eq "uat"){
        Write-Host "ENV is UAT"
        return "UAT"
    }
    elseif ($envTargetVar -eq "prod"){
        Write-Host "ENV is PRODUCTION"
        return "Production"
    }
    else{
        Write-Host "Unknown ENV"
    }
}

$configTarget = get-ConfigEnvironment $envTarget

$Deployments = @(
    @{
        Artifact = 'Demoulas.ProfitSharing.Api.zip'
        TargetPath = 'C:\inetpub\wwwroot\api'
        SiteName = 'API'
        AppPoolName = 'NETSApiAppPool'
        IgnoreFiles = @("credSettings.$($envTarget).json")
        ConfigEnvironment = $configTarget
    },
    #@{
    #    Artifact = "Demoulas.Smart.UI.$($envTarget).zip"
    #    TargetPath = 'C:\inetpub\wwwroot\frontend'
    #    SiteName = 'Frontend'
    #    AppPoolName = 'FrontendUIAppPool'
    #    IgnoreFiles = @()
    #    ConfigEnvironment = $configTarget
    #}
)

$Failed = $false
try {
    $Session = New-PSSession $envServerName

    foreach ($Deploy in $Deployments) {
        Invoke-Command -Session $Session -ScriptBlock {
            $Site = Get-IISSite -Name $Using:Deploy.SiteName
            $Site | Stop-IISSite -Confirm:$false
            $AppPool = Get-IISAppPool -Name $Using:Deploy.AppPoolName
            if ($AppPool.State -ne 'Stopped') {
                $AppPool | Stop-WebAppPool
                for ($i = 0; $i -lt $Using:StopAppTimeout; $i++) {
                    if ($AppPool.State -eq 'Stopped') {
                        break
                    }
                    Start-Sleep -Seconds 1
                }
                if ($AppPool.State -ne 'Stopped') {
                    Write-Error -Message "Failed to stop App Pool '$($AppPool.Name)'"
                    exit 1
                }
            }
            Get-ChildItem -Path $Using:Deploy.TargetPath -Exclude $Using:Deploy.IgnoreFiles | Remove-Item -Force -Recurse
        }
        if (!$?) {$Failed = $true; break}
        
        Copy-Item -ToSession $Session -Path .\dist\$($Deploy.Artifact) -Destination $Deploy.TargetPath
        if (!$?) {$Failed = $true; break}
        
        Invoke-Command -Session $Session -ScriptBlock {
            Expand-Archive -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)" -DestinationPath $Using:Deploy.TargetPath
            Remove-Item -Force -Path "$($Using:Deploy.TargetPath)\$($Using:Deploy.Artifact)"
            if (Test-Path -Path "$($Using:Deploy.TargetPath)\web.config" -PathType Leaf) {
                Write-Output "$($Using:Deploy.TargetPath)\web.config"
                (Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw) -replace 'Development', $Using:Deploy.ConfigEnvironment | Set-Content -Path "$($Using:Deploy.TargetPath)\web.config"
                Get-Content -path "$($Using:Deploy.TargetPath)\web.config" -Raw
            }
            $AppPool | Start-WebAppPool
            $Site | Start-IISSite
        }
        
        if (!$?) {$Failed = $true; break}
    }
} catch {
    $Failed = $true
} finally {
    if ($null -ne $Session) {
        Remove-PSSession -Session $Session
        $Session = $null
    }
}

if ($Failed) {exit 1}