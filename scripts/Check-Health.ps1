param (
    [string]$url,
    [string]$envServerName
)

if (-not $url) {
    Write-Host "Error: URL parameter is missing."
    exit 1
}

# Ensure the base URL ends without a trailing slash
if ($url.EndsWith("/")) {
    $url = $url.TrimEnd("/")
}

# Properly construct the full URL
$fullUrl = "$($url):8443/health"
Write-Host "Base url: $url"
Write-Host "Checking health at: $fullUrl"

# Send a request to the /health endpoint with certificate validation skipped
# SkipCertificateCheck Requires Powershell 7 or higher
try {
    $Session = New-PSSession $envServerName
    Invoke-Command -Session $Session -ScriptBlock {
        param ($fullUrl)

        $response = Invoke-WebRequest -Uri $fullUrl -Method GET -SkipCertificateCheck -UseBasicParsing
        $content = $response.Content | ConvertFrom-Json

      if ($content.status -eq 'Healthy') {
          Write-Host "API is Healthy."
          exit 0
      } else {
          Write-Host "API health check failed: Unexpected status value."
          exit 1
      }
    } -ArgumentList $fullUrl
  } catch {
      Write-Host "API health check failed: $_"
      exit 1
  }
