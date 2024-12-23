param (
    [string]$ServiceName, # Name of the Windows Service
    [string]$ZipFilePath, # Path to the .zip file (local machine)
    [string]$RemoteServer, # Remote server name or IP
    [string]$InstallPath, # Path on the remote server to extract and install the service
    [string]$ServiceExe           # Name of the .exe file for the service
)

# Variables
$RemoteTempPath = "\\$RemoteServer\C$\Temp\ServiceDeploy"
$UnzipTool = "C:\Windows\System32\WindowsPowerShell\v1.0\Modules\Microsoft.PowerShell.Archive\Expand-Archive.ps1"

# Function to Copy the ZIP File to the Remote Server
function Copy-ZipFile
{
    Write-Host "Copying ZIP file to $RemoteServer..."
    if (-not (Test-Path $ZipFilePath))
    {
        throw "The ZIP file '$ZipFilePath' does not exist."
    }

    New-Item -ItemType Directory -Force -Path $RemoteTempPath | Out-Null
    Copy-Item -Path $ZipFilePath -Destination "$RemoteTempPath\$( $ServiceName ).zip" -Force
}

# Function to Extract ZIP File on the Remote Server
function Extract-ZipFile
{
    Write-Host "Extracting ZIP file on $RemoteServer..."
    $RemoteZipPath = "$RemoteTempPath\$( $ServiceName ).zip"
    $RemoteUnzipPath = "$InstallPath\$ServiceName"

    Invoke-Command -ComputerName $RemoteServer -ScriptBlock {
        param ($ZipPath, $UnzipPath)
        if (-not (Test-Path $ZipPath))
        {
            throw "The ZIP file '$ZipPath' does not exist on the remote server."
        }

        # Ensure the installation directory exists
        New-Item -ItemType Directory -Force -Path $UnzipPath | Out-Null

        # Extract the ZIP file
        Expand-Archive -Path $ZipPath -DestinationPath $UnzipPath -Force
    } -ArgumentList $RemoteZipPath, $RemoteUnzipPath
}

# Function to Install or Update the Service
function Install-Service
{
    Write-Host "Installing or updating the service on $RemoteServer..."
    $RemoteServicePath = "$InstallPath\$ServiceName\$ServiceExe"

    Invoke-Command -ComputerName $RemoteServer -ScriptBlock {
        param ($ServiceName, $ServicePath)

        # Check if the service exists
        $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
        if ($service)
        {
            Write-Host "Service '$ServiceName' exists. Stopping and updating it..."
            Stop-Service -Name $ServiceName -Force
            Set-Service -Name $ServiceName -BinaryPathName $ServicePath
        }
        else
        {
            Write-Host "Service '$ServiceName' does not exist. Installing it..."
            New-Service -Name $ServiceName -BinaryPathName $ServicePath -DisplayName $ServiceName -StartupType Automatic
        }

        Start-Service -Name $ServiceName
    } -ArgumentList $ServiceName, $RemoteServicePath
}

# Main Script Execution
try {
    Copy-ZipFile
    Extract-ZipFile
    Install-Service
    Write-Host "Service '$ServiceName' successfully deployed to $RemoteServer."
} catch {
    Write-Error $_.Exception.Message
    exit 1
}
