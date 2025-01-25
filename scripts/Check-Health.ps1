param (
    [string]$url
)

if (-not $url) {
    Write-Host "Error: URL parameter is missing."
    exit 1
}

# Ensure the base URL ends without a trailing slash
if ($url.EndsWith("/")) {
    $url = $url.TrimEnd("/")
}

# Combine the base URL with the port and endpoint
$fullUrl = "$($url):8443/health"
Write-Host "Base url: $url"
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
