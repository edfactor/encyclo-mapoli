param(
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
    "SUCCESS" = "✅"
    "FAILURE" = "❌"
    "IN_PROGRESS" = "🔄"
}

$emoji = $statusEmoji[$Status]
if (-not $emoji) { $emoji = "ℹ️" }

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
    Invoke-WebRequest -Uri $WebhookUrl -Method Post -Body $payload -ContentType "application/json" -ErrorAction Stop | Out-Null
    Write-Host "✓ Teams notification sent successfully"
}
catch {
    Write-Error "Failed to send Teams notification: $_"
    # Don't exit on notification failure - deployment might have succeeded
}
