param (
    [string]$Environment,
    [string]$FilePath,
    [string]$OutputPath,
    [string]$RemoteServer,
    [string]$RemotePath
)

$profitSharing = $env:ConnectionStrings_ProfitSharing_QA
$commonConnectionString = $env:ConnectionStrings_ProfitSharing_QA


[hashtable]$Replacements = @{
    "CONNECTIONSTRINGS_PROFITSHARING_SLUG" = $profitSharing
    "CONNECTIONSTRINGS_COMMON_SLUG" = $commonConnectionString
    "ORACLEHCM_BASE_URL_SLUG" = $env:QA_ORACLEHCM_BASE_URL_SLUG
    "ORACLEHCM_USERNAME_SLUG" = $env:QA_ORACLEHCM_USERNAME_SLUG
    "ORACLEHCM_PASSWORD_SLUG" = $env:QA_ORACLEHCM_PASSWORD_SLUG
}

if (-not $Replacements -or $Replacements.GetEnumerator().Count -eq 0) {
    throw "The 'Replacements' parameter is missing or invalid. Provide a valid JSON string or Hashtable."
}

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

# Copy the file to the remote server
$Session = $null
try {
    Write-Host "Creating a remote session to $RemoteServer"
    $Session = New-PSSession -ComputerName $RemoteServer
    
    # Ensure the remote path exists
    Write-Host "Checking if remote path exists: ${RemotePath}"
    Invoke-Command -Session $Session -ScriptBlock {
        param ($RemotePath)
        if (-not (Test-Path -Path $RemotePath)) {
            Write-Host "Remote path does not exist. Creating: $RemotePath"
            New-Item -ItemType Directory -Path $RemotePath -Force
        }
    } -ArgumentList $RemotePath

    # Copy the file
    Write-Host "Copying file to ${RemoteServer}:${RemotePath}"
    Copy-Item -ToSession $Session -Path $OutputPath -Destination $RemotePath

    Write-Host "File copied successfully to ${RemoteServer}:${RemotePath}"
} catch {
    Write-Error "Failed to copy the file to ${RemoteServer}:${RemotePath}. Error: $_"
} finally {
    if ($Session -ne $null) {
        Write-Host "Removing remote session"
        Remove-PSSession -Session $Session
    }
}
