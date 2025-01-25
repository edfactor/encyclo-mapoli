param (
    [string]$url
)

# Combine the base URL with the endpoint
$fullUrl = "$url:8443/health"
Write-Host "Checking health at: $fullUrl"

# Send a request to the /health endpoint
try {
    $response = Invoke-WebRequest -Uri $fullUrl -Method GET -UseBasicParsing
    $content = $response.Content | ConvertFrom-Json

    if ($content.status -eq 'healthy') {
        Write-Host "API is healthy."
        exit 0
    } else {
        Write-Host "API health check failed: Unexpected status value."
        exit 1
    }
} catch {
    Write-Host "API health check failed: $_"
    exit 1
}
