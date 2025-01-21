param (
    [string]$FilePath,
    [string]$OutputPath,
    [hashtable]$Replacements,
    [string]$RemoteServer,
    [string]$RemotePath
)

# Convert JSON to hashtable if passed as string
if ($Replacements -is [string]) {
    $Replacements = ConvertFrom-Json $Replacements
}

Write-Host "Replacements: $Replacements"

# Process File
$content = Get-Content -Path $FilePath
foreach ($key in $Replacements.Keys) {
    $content = $content -replace $key, $Replacements[$key]
}
Set-Content -Path $OutputPath -Value $content

# Copy file to remote server (Example logic, adjust as needed)
Write-Host "Copying $OutputPath to ${RemoteServer}:${RemotePath}"


