param (
    [string]$Url
)

if (-not $Url) {
    Write-Error "The URL parameter is required."
    exit 1
}

Write-Host "Checking health status for: $Url"

# Make the HTTP request
$response = Invoke-WebRequest -Uri $Url -Method GET -UseBasicParsing -ErrorAction Stop

if ($response.StatusCode -ne 200) {
    Write-Error "Health check failed: Received status code $($response.StatusCode)"
    exit 1
}

# Parse the response body (assumes JSON)
$body = $response.Content | ConvertFrom-Json

if ($body.status -ne "healthy") {
    Write-Error "Health check failed: Status is $($body.status)"
    exit 1
}

Write-Host "Health check passed: Status is $($body.status)"
