param (
    [string]$url
)

# Send a request to the /health endpoint
try {
    $response = Invoke-WebRequest -Uri $url -Method GET -UseBasicParsing
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
