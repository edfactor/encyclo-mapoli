# Teams Notifications Setup Guide

## Step 1: Add Your Power Automate Webhook URL as a Repository Variable

1. Go to your **Bitbucket repository** on bitbucket.org
2. Click **Settings** (gear icon) → **Repository settings**
3. Navigate to **Pipelines** → **Repository variables**
4. Click **Add variable**
5. Fill in:
   - **Name:** `TEAMS_WEBHOOK_URL`
   - **Value:** [Your Power Automate webhook URL]
   - **Check the "Secure" checkbox** to mask it in logs
6. Click **Save**

### Your Webhook URL:
```
https://defaultce0691a156834f59974f30298e8159.d1.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/7da515faaaf3450ca17c41c00d389cd4/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=AcRgaKlaXOe29HbDR5BEapYg8AGAvNv_W6_P3ilUVT0
```

## Step 2: How It Works

The pipeline now automatically sends notifications to your Teams channel when:

### QA Deployment
- ✅ **Deployment succeeds** → "✅ QA Deployment - SUCCESS"
- ❌ **Deployment fails** → "❌ QA Deployment - FAILURE" with error details

### API Health Check
- ✅ **Health check passes** → "✅ QA Health Check - SUCCESS"
- ❌ **Health check fails** → "❌ QA Health Check - FAILURE"

## Step 3: Data Sent to Teams

Each notification includes:
- Deployment status (SUCCESS/FAILURE)
- Environment (QA, UAT, etc.)
- Build number
- Git branch
- Commit hash
- Timestamp
- Error message (if failure)
- Link to the build

## Step 4: Configure Your Power Automate Workflow

In your Power Automate workflow, you can access these fields:
- `title` - Display title
- `status` - SUCCESS or FAILURE
- `environment` - QA or UAT
- `buildNumber` - Bitbucket build ID
- `branch` - Git branch name
- `commit` - Commit hash
- `timestamp` - When the event occurred
- `errorMessage` - Error details (if failure)
- `buildUrl` - Link to Bitbucket build
- `step` - Deployment, Health Check, etc.

Example workflow card:
```
Title: [title]
Status: [status]
Environment: [environment]
Build: [buildNumber]
Branch: [branch]
Commit: [commit]
Timestamp: [timestamp]
Error: [errorMessage]
```

## Testing the Integration

To test your webhook integration:
```powershell
$url = "https://defaultce0691a156834f59974f30298e8159.d1.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/7da515faaaf3450ca17c41c00d389cd4/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=AcRgaKlaXOe29HbDR5BEapYg8AGAvNv_W6_P3ilUVT0"

$payload = @{
    title = "Test Notification"
    status = "SUCCESS"
    environment = "QA"
    buildNumber = "999"
    branch = "develop"
    commit = "abc123"
    timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss UTC")
    errorMessage = ""
    buildUrl = "https://bitbucket.org/test"
    step = "Test"
} | ConvertTo-Json

Invoke-WebRequest -Uri $url -Method Post -Body $payload -ContentType "application/json"
```

## Troubleshooting

If notifications aren't appearing:

1. **Verify the webhook URL** - Check that it's exactly as provided
2. **Check Bitbucket logs** - Look at the pipeline logs for the "Teams notification" message
3. **Verify Power Automate flow** - Make sure the workflow is enabled and active
4. **Test manually** - Run the PowerShell test command above
5. **Check Teams permissions** - Ensure your Power Automate connector has permission to post to the channel
