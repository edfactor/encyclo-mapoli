param (
    [string]$FilePath,
    [string]$OutputPath,
    [Object]$Replacements, # Accept both Hashtable and JSON string
    [string]$RemoteServer,
    [string]$RemotePath
)

# Convert JSON string to Hashtable if needed
if ($Replacements -is [string]) {
    $Replacements = ConvertFrom-Json $Replacements | ForEach-Object { @{$_.Name = $_.Value} }
}

if (-not $Replacements -or $Replacements.Count -eq 0) {
    throw "The 'Replacements' parameter is missing or invalid. Provide a valid Hashtable."
}

Write-Host "Replacements provided: $Replacements"
