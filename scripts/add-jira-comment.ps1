<#
Add-jira-comment.ps1

Post a comment to a Jira ticket using Jira Cloud REST API.

Usage (recommended from CI):
  $env:JIRA_BASE_URL = 'https://yourcompany.atlassian.net'
  $env:JIRA_USER_EMAIL = 'ci-bot@yourcompany.com'
  $env:JIRA_API_TOKEN = '***'          # keep this secret in pipeline variables
  ./scripts/add-jira-comment.ps1 -JiraKey PS-1234 -PrUrl "$env:CI_PR_URL" -Comment "PR opened: $env:CI_PR_URL"

Environment variables (required):
 - JIRA_BASE_URL    e.g. https://demoulas.atlassian.net
 - JIRA_USER_EMAIL  service account email or user email
 - JIRA_API_TOKEN   API token (do NOT commit this file with secrets)

Parameters:
 -JiraKey   (string) The Jira issue key to comment on (e.g. PS-1234)
 -PrUrl     (string) URL of the PR to include in the comment
 -Comment   (string) Optional custom comment body; if omitted a default comment is used

Security: Store `JIRA_API_TOKEN` in your CI secret store (Bitbucket repository variables, GitHub Secrets, etc.).

This script is a convenience helper. If your organization requires use of the Atlassian MCP integration, prefer wiring MCP calls in CI instead of direct REST calls.
#>

param(
    [Parameter(Mandatory=$true)][string]$JiraKey,
    [Parameter(Mandatory=$false)][string]$PrUrl,
    [Parameter(Mandatory=$false)][string]$Comment
)

function Fail([string]$msg){
    Write-Error $msg
    exit 2
}

# Validate env
$baseUrl = $env:JIRA_BASE_URL
$user = $env:JIRA_USER_EMAIL
$token = $env:JIRA_API_TOKEN

if (-not $baseUrl) { Fail 'JIRA_BASE_URL environment variable is required.' }
if (-not $user) { Fail 'JIRA_USER_EMAIL environment variable is required.' }
if (-not $token) { Fail 'JIRA_API_TOKEN environment variable is required.' }

if (-not $Comment) {
    if ($PrUrl) {
        $Comment = "PR opened: $PrUrl"
    } else {
        $Comment = "Automated comment from CI: PR created referencing this ticket."
    }
}

$url = "$baseUrl/rest/api/3/issue/$JiraKey/comment"

# Basic auth header
# Build Basic auth header (avoid ambiguous interpolation)
$pair = [System.Text.Encoding]::UTF8.GetBytes($user + ':' + $token)
$b64 = [Convert]::ToBase64String($pair)
$headers = @{ Authorization = ('Basic ' + $b64); 'Content-Type' = 'application/json' }

$body = @{ body = $Comment } | ConvertTo-Json

try {
    $resp = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body -ErrorAction Stop
    Write-Host "Added comment to $JiraKey (id: $($resp.id))"
    exit 0
} catch {
    Write-Error ("Failed to add comment to {0}: {1}" -f $JiraKey, $_.Exception.Message)
    exit 3
}
