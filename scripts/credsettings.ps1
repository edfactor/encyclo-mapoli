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

# Iterate through the hashtable
foreach ($key in $Replacements.Keys) {
    Write-Host "Replacing $key with $($Replacements[$key])"
}

# Read the content of the file
if (-not (Test-Path -Path $FilePath)) {
    throw "The file specified by 'FilePath' does not exist: $FilePath"
}

Write-Host "Reading content from $FilePath"
$content = Get-Content -Path $FilePath -Raw

# Perform find and replace for each key-value pair in the hashtable
foreach ($key in $Replacements.Keys) {
    $value = $Replacements[$key]
    Write-Host "Replacing '$key' with '$value'"
    $content = $content -replace [regex]::Escape($key), $value
}

# Write the modified content to the output file
Write-Host "Writing modified content to $OutputPath"
Set-Content -Path $OutputPath -Value $content

Write-Host "File has been successfully updated and saved to $OutputPath"
