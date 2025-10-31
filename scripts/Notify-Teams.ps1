param(
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string]$WebhookUrl = $env:TEAMS_WEBHOOK_URL,
    [string]$Status = "SUCCESS",
    [string]$Environment = "QA",
    [string]$ErrorMessage = "",
    [string]$BuildNumber = $env:BITBUCKET_BUILD_NUMBER,
    [string]$Branch = $env:BITBUCKET_BRANCH,
    [string]$Commit = $env:BITBUCKET_COMMIT,
    [string]$Step = "Deployment"
)

$statusEmoji = @{
    "SUCCESS" = "[SUCCESS]"
    "FAILURE" = "[FAILURE]"
    "IN_PROGRESS" = "[IN PROGRESS]"
}

$emoji = $statusEmoji[$Status]
if (-not $emoji) { $emoji = "[INFO]" }

$title = "$emoji $Environment $Step - $Status"

# Power Automate Workflow format
$payload = @{
    title = $title
    status = $Status
    environment = $Environment
    buildNumber = $BuildNumber
    branch = $Branch
    commit = $Commit
    timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss UTC")
    errorMessage = $ErrorMessage
    buildUrl = $env:BITBUCKET_BUILD_URL
    step = $Step
} | ConvertTo-Json

try {
    if ([string]::IsNullOrWhiteSpace($WebhookUrl)) {
        Write-Warning "TEAMS_WEBHOOK_URL environment variable is not set. Skipping Teams notification."
        Write-Host "To enable Teams notifications, add TEAMS_WEBHOOK_URL as a Bitbucket repository variable."
        return
    }
    
    Write-Host "Sending payload to Power Automate:"
    Write-Host $payload
    
    $response = Invoke-WebRequest -Uri $WebhookUrl -Method Post -Body $payload -ContentType "application/json; charset=utf-8" -ErrorAction Stop
    Write-Host "[OK] Teams notification sent successfully"
    Write-Host "Response Status Code: $($response.StatusCode)"
    Write-Host "Response Content: $($response.Content)"
}
catch {
    Write-Error "Failed to send Teams notification: $_"
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Error "Response Body: $responseBody"
    }
    # Don't exit on notification failure - deployment might have succeeded
}
