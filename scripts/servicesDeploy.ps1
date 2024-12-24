param (
    [Parameter(Mandatory = $true)]
    [string]$ServiceName,

    [Parameter(Mandatory = $true)]
    [string]$InstallationPath,

    [Parameter(Mandatory = $true)]
    [string]$RemoteServer,

    [Parameter(Mandatory = $true)]
    [string]$ZipFilePath
)

# Step 1: Establish a PowerShell session to the remote server
Write-Host "Establishing remote session to $RemoteServer..."
$Session = New-PSSession -ComputerName $RemoteServer

try {
    # Step 2: Copy the ZIP file to the remote server's temporary folder
    $RemoteTempPath = "C:\Temp"
    $RemoteZipPath = Join-Path -Path $RemoteTempPath -ChildPath (Split-Path -Leaf $ZipFilePath)
    Write-Host "Copying ZIP file to remote server: $RemoteZipPath"
    Invoke-Command -Session $Session -ScriptBlock {
        param ($RemoteTempPath)
        if (-not (Test-Path $RemoteTempPath))
        {
            New-Item -Path $RemoteTempPath -ItemType Directory | Out-Null
        }
    } -ArgumentList $RemoteTempPath

    Copy-Item -Path $ZipFilePath -Destination $RemoteZipPath -ToSession $Session

    # Step 3: Deploy the service on the remote server
    Write-Host "Deploying service on remote server..."
    Invoke-Command -Session $Session -ScriptBlock {
        param ($ServiceName, $InstallationPath, $RemoteZipPath)
        Write-Host "Creating installation path: $InstallationPath"
        if (-not (Test-Path $InstallationPath))
        {
            New-Item -Path $InstallationPath -ItemType Directory | Out-Null
        }

        Write-Host "Extracting ZIP file: $RemoteZipPath"
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        [System.IO.Compression.ZipFile]::ExtractToDirectory($RemoteZipPath, $InstallationPath)

        $ServiceExePath = Join-Path -Path $InstallationPath -ChildPath "Demoulas.ProfitSharing.OracleHcm.Sync.exe"
        if (-not (Test-Path $ServiceExePath))
        {
            throw "Executable not found in extracted directory: $ServiceExePath"
        }

        Write-Host "Registering Windows Service: $ServiceName"
        sc.exe create $ServiceName binPath= "$ServiceExePath" start= auto
        sc.exe start $ServiceName

        Write-Host "Service $ServiceName deployed and started successfully!"
    } -ArgumentList $ServiceName, $InstallationPath, $RemoteZipPath

}
finally
{
    # Clean up the session
    Remove-PSSession -Session $Session
    Write-Host "Remote session closed."
}

Write-Host "Deployment completed successfully."
