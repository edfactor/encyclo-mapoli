# Define parameters
param (
    [string]$EnvName = "qa",                     # Environment name
    [string]$ReplacementsJson = "{}"            # JSON string for replacements
)

# Convert JSON string to a hashtable
try {
    $replacements = $ReplacementsJson | ConvertFrom-Json | ForEach-Object { $_.psobject.Properties } | ForEach-Object {
    [ordered]@{ $_.Name = $_.Value }
}

} catch {
    Write-Error "Failed to parse replacements JSON: $_"
    exit 1
}

# Path to the original and modified files
$inputFile = "credsettings.json"
$outputFile = "credsettings.modified.json"
$outputDir = ".\publishOutput"
$finalOutputFile = "$outputDir\credsettings.$EnvName.json"

# Read the JSON file content
if (Test-Path $inputFile) {
    $fileContent = Get-Content -Path $inputFile -Raw
} else {
    Write-Error "Input file '$inputFile' not found."
    exit 1
}

# Perform the replacements
foreach ($key in $replacements.Keys) {
    $fileContent = $fileContent -replace $key, $replacements[$key]
}

# Save the modified JSON content
Set-Content -Path $outputFile -Value $fileContent
Write-Host "Modified JSON saved to $outputFile"

# Create the output directory if it doesn't exist
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}

# Copy the modified file to the output directory with the environment suffix
Copy-Item -Path $outputFile -Destination $finalOutputFile -Force
Write-Host "Final JSON file saved to $finalOutputFile"
