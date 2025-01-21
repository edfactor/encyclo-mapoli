param (
    [string]$FilePath,
    [string]$OutputPath,
    [string]$Replacements, # Accept JSON string
    [string]$RemoteServer,
    [string]$RemotePath
)

# Convert JSON string to Hashtable if needed
if ($Replacements -is [string]) {
    try {
        $Replacements = ConvertFrom-Json $Replacements
    } catch {
        throw "Failed to convert 'Replacements' JSON string to a Hashtable. Ensure it is a valid JSON string."
    }
}

if (-not $Replacements -or $Replacements.GetEnumerator().Count -eq 0) {
    throw "The 'Replacements' parameter is missing or invalid. Provide a valid JSON string or Hashtable."
}

Write-Host "Replacements provided: $Replacements"

# Example: Iterate through the hashtable
foreach ($key in $Replacements.Keys) {
    Write-Host "Replacing $key with $($Replacements[$key])"
}

# Proceed with the rest of your script...
