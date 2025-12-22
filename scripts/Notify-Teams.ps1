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

# Extract Jira ticket numbers from branch name and commit message
$jiraTickets = @()
$jiraPattern = 'PS-\d+'

# Check branch name
if ($Branch -match $jiraPattern) {
    $jiraTickets += $matches[0]
}

# Get commit message if available
if ($Commit -and $env:BITBUCKET_CLONE_DIR) {
    try {
        Push-Location $env:BITBUCKET_CLONE_DIR
        $commitMessage = git log -1 --pretty=%B $Commit 2>$null
        if ($commitMessage -match $jiraPattern) {
            $ticket = $matches[0]
            if ($ticket -notin $jiraTickets) {
                $jiraTickets += $ticket
            }
        }
        Pop-Location
    }
    catch {
        # Silently continue if git command fails
        if ((Get-Location).Path -ne $PWD.Path) {
            Pop-Location
        }
    }
}

# Remove duplicates and sort
$jiraTickets = $jiraTickets | Select-Object -Unique | Sort-Object

# Power Automate Adaptive Card format
$payload = @{
    type = "message"
    attachments = @(
        @{
            contentType = "application/vnd.microsoft.card.adaptive"
            content = @{
                type = "AdaptiveCard"
                body = @(
                    @{
                        type = "TextBlock"
                        size = "Large"
                        weight = "Bolder"
                        text = $title
                        wrap = $true
                    }
                    @{
                        type = "FactSet"
                        facts = @(
                            @{
                                title = "Environment:"
                                value = $Environment
                            }
                            @{
                                title = "Status:"
                                value = $Status
                            }
                            @{
                                title = "Build:"
                                value = $BuildNumber
                            }
                            @{
                                title = "Branch:"
                                value = $Branch
                            }
                            @{
                                title = "Step:"
                                value = $Step
                            }
                            @{
                                title = "Commit:"
                                value = $Commit.Substring(0, [Math]::Min(8, $Commit.Length))
                            }
                            @{
                                title = "Timestamp:"
                                value = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss UTC")
                            }
                        )
                    }
                )
                '$schema' = "http://adaptivecards.io/schemas/adaptive-card.json"
                version = "1.4"
            }
        }
    )
} 

# Add Jira tickets if found
if ($jiraTickets.Count -gt 0) {
    $jiraText = ($jiraTickets | ForEach-Object { "**$_**" }) -join ", "
    $payload.attachments[0].content.body += @{
        type = "TextBlock"
        text = "**Jira Tickets:** $jiraText"
        wrap = $true
        spacing = "Medium"
    }
}

# Add error message if present
if (-not [string]::IsNullOrWhiteSpace($ErrorMessage)) {
    $payload.attachments[0].content.body += @{
        type = "TextBlock"
        text = "**Error:** $ErrorMessage"
        wrap = $true
        color = "Attention"
    }
}

# Add action buttons (Build URL and Jira tickets)
$actions = @()

# Add build URL button
if (-not [string]::IsNullOrWhiteSpace($env:BITBUCKET_BUILD_URL)) {
    $actions += @{
        type = "Action.OpenUrl"
        title = "View Build"
        url = $env:BITBUCKET_BUILD_URL
    }
}

# Add Jira ticket buttons
foreach ($ticket in $jiraTickets) {
    $actions += @{
        type = "Action.OpenUrl"
        title = $ticket
        url = "https://demoulas.atlassian.net/browse/$ticket"
    }
}

# Add action set if we have any actions
if ($actions.Count -gt 0) {
    $payload.attachments[0].content.body += @{
        type = "ActionSet"
        actions = $actions
    }
}

$payloadJson = $payload | ConvertTo-Json -Depth 10

try {
    if ([string]::IsNullOrWhiteSpace($WebhookUrl)) {
        Write-Warning "TEAMS_WEBHOOK_URL environment variable is not set. Skipping Teams notification."
        Write-Host "To enable Teams notifications, add TEAMS_WEBHOOK_URL as a Bitbucket repository variable."
        return
    }
    
    Write-Host "Sending payload to Power Automate:"
    Write-Host $payloadJson
    
    $response = Invoke-WebRequest -Uri $WebhookUrl -Method Post -Body $payloadJson -ContentType "application/json; charset=utf-8" -ErrorAction Stop
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
