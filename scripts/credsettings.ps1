param (
    [string]$FilePath,
    [string]$OutputPath,
    [hashtable]$Replacements,
    [string]$RemoteServer,
    [string]$RemotePath,
    [string]$Username,
    [string]$Password
)

# Read the content of the file
$content = Get-Content -Path $FilePath

# Perform find/replace
foreach ($key in $Replacements.Keys) {
    $content = $content -replace $key, $Replacements[$key]
}

# Write the updated content to the output file
Set-Content -Path $OutputPath -Value $content

# Copy the file to the remote server
$securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential($Username, $securePassword)
Invoke-Command -ComputerName $RemoteServer -Credential $credential -ScriptBlock {
    param($src, $dest)
    Copy-Item -Path $src -Destination $dest
} -ArgumentList $OutputPath, $RemotePath
