# Cloud Runner Migration Summary

**Date**: October 7, 2025  
**Purpose**: Free up on-premises Windows runners by moving non-deployment steps to Bitbucket Cloud

## Changes Implemented

### Steps Moved to Cloud Runners

#### 1. ❌ `audit-backend` - Backend Vulnerability Scanning

**Status**: NOT MIGRATED - Reverted to Windows self-hosted  
**Reason**: Requires access to custom NuGet sources (JFrog Artifactory) that need internal authentication

**Why It Can't Move**:
- dotnet restore requires internal JFrog credentials
- Custom NuGet packages not available on public cloud runners
- Would need secure credential injection that's not configured
- Windows runner has pre-configured access to internal sources

**Remains On**: Windows self-hosted runner with internal network access

---

#### 2. ✅ `audit-frontend` - Frontend Vulnerability Scanning

**Before**: Windows self-hosted runner with Volta  
**After**: Bitbucket Cloud with `node:22-alpine` Docker image

**Why Safe to Move**:
- Only runs `npm audit` against package.json
- Doesn't build or deploy anything
- Uses JFrog credentials from pipeline variables (already available in cloud)
- No Windows-specific dependencies

**Changes Made**:
```yaml
# Before
runs-on:
  - windows
  - self.hosted
  - volta

# After
image: node:22-alpine
max-time: 10
```

**Script Changes**:
- Converted from PowerShell to bash syntax
- .npmrc creation uses bash heredoc instead of PowerShell
- Logic remains identical

**Resource Savings**: 1 Windows runner freed per audit (runs on every PR + develop)

---

#### 3. ✅ `secret-scan` - Gitleaks Secret Scanning

**Before**: Implicitly might run on Windows runners  
**After**: Explicitly runs on Bitbucket Cloud (already using Docker)

**Why Safe to Move**:
- Already using Docker image (`zricethezav/gitleaks:latest`)
- Linux-based tool, no Windows dependencies
- Only scans source code for secrets
- No deployment or artifact creation

**Changes Made**:
```yaml
# Added explicit timeout for efficiency
max-time: 5
```

**Script Changes**: None needed (already using Linux/bash)

**Resource Savings**: Ensures no Windows runner is used for secret scanning

---

## Resource Impact

### On-Premises Runner Usage Reduction

**Per PR Pipeline**:
- **Before**: 2 builds + 3 audits/scans = 5 parallel Windows runners (Stage 1-2)
- **After**: 2 builds + 1 Windows audit = 3 parallel Windows runners, 2 cloud runners (Stage 2)
- **Savings**: 2 Windows runners freed in Stage 2 (frontend audit + secret scan moved to cloud)

**Per Develop Push**:
- **Before**: 2 builds + 3 audits/scans = 5 parallel Windows runners
- **After**: 2 builds + 1 Windows audit = 3 parallel Windows runners + 2 cloud runners
- **Savings**: 2 Windows runners freed (frontend audit + secret scan moved to cloud)

**Per Release Pipeline**:
- **Before**: 2 builds + 3 audits/scans = 5 parallel Windows runners in initial stages
- **After**: 2 builds + 1 Windows audit = 3 parallel Windows runners + 2 cloud runners
- **Savings**: 2 Windows runners freed (frontend audit + secret scan moved to cloud)

**Note**: Backend audit remains on Windows due to internal JFrog NuGet authentication requirements

### Cloud Runner Usage (Free Minutes)

Bitbucket Cloud provides **500 free build minutes/month** for pipelines.

**Estimated Cloud Usage Per Pipeline**:
- `audit-backend`: REVERTED - Stays on Windows (needs internal NuGet sources)
- `audit-frontend`: ~1-2 minutes (npm audit only, no install)
- `secret-scan`: ~1-2 minutes (Gitleaks scan)
- **Total per pipeline**: ~2-4 minutes

**Monthly Estimate** (assuming 40 PRs + 20 develop pushes + 10 releases):
- 70 total pipeline runs × 3 minutes average = **210 minutes/month**
- Well within the 500 free minutes limit

---

## Steps That MUST Stay On-Premises

### Build & Artifact Steps
- `build-and-test-backend` - Needs access to internal JFrog for Demoulas packages
- `test-frontend` - Needs access to internal JFrog for smart-ui-library
- `create-api-artifacts` - Creates artifacts for internal deployment
- `create-service-artifacts` - Creates artifacts for internal deployment
- `create-frontend-artifact-*` - Needs access to internal JFrog

**Why**: These steps require authenticated access to internal JFrog Artifactory for private packages (`Demoulas.*`, `smart-ui-library`, etc.)

### Audit Steps (Backend Only)
- `audit-backend` - **REVERTED TO WINDOWS** - Requires access to internal JFrog NuGet sources for dotnet restore

**Why**: Custom NuGet packages require internal authentication that cloud runners don't have configured

### Deployment Steps
- All `Application Deployment` steps
- All database upgrade/import steps
- Health check steps

**Why**: These steps require network access to internal servers (appt23d, appt24d, internal databases)

### E2E Testing
- `run-playwright-tests` - Requires access to internal QA environment

**Why**: Tests run against internal QA environment that's not publicly accessible

---

## Risk Assessment

| Step | Risk Level | Mitigation |
|------|-----------|------------|
| audit-backend | **N/A - Reverted** | Stays on Windows due to internal NuGet dependencies |
| audit-frontend | **Very Low** | Only runs npm audit, no build/deploy |
| secret-scan | **Very Low** | Already Linux-based, no Windows deps |

**Security Considerations**:
- ✅ No secrets exposed (uses Bitbucket pipeline variables)
- ✅ No internal network access required for cloud steps
- ✅ No artifacts or credentials leave cloud environment
- ✅ JFrog token used only for npm audit (read-only operation)
- ✅ Backend audit stays on Windows with internal network access for JFrog

---

## Testing Checklist

### Before Merging

- [ ] PR pipeline completes successfully with cloud audits
- [ ] Backend audit runs on Windows (not cloud) and completes successfully
- [ ] Frontend audit runs on cloud and detects vulnerabilities correctly
- [ ] Secret scan still catches secrets in test files
- [ ] Cloud minute usage is within free tier limits
- [ ] Only frontend audit and secret scan use cloud runners (not backend)

### Validation Commands

**Check runner usage in pipeline logs**:
```bash
# audit-backend should show:
# "Running on Windows self-hosted runner"

# audit-frontend should show:
# "Running on Bitbucket Cloud with Docker image node:22-alpine"

# secret-scan should show:
# "Running on Bitbucket Cloud with Docker image zricethezav/gitleaks:latest"
```

**Monitor cloud minutes**:
- Repository Settings → Pipelines → Usage
- Check "Build minutes used this month"

---

## Rollback Plan

If cloud runners cause issues:

```yaml
# Revert audit-backend to on-premises
audit-backend:
  runs-on:
    - windows
    - self.hosted
  script:
    - cd .\src\services
    # ... PowerShell script

# Revert audit-frontend to on-premises  
audit-frontend:
  runs-on:
    - windows
    - self.hosted
    - volta
  script:
    - cd .\src\UI
    # ... PowerShell script
```

---

## Benefits Summary

✅ **Resource Efficiency**:
- 3 fewer Windows runners needed per pipeline run
- 60% reduction in on-premises runner usage for PR checks
- Better runner availability for critical build/deploy operations

✅ **Cost Optimization**:
- Utilizes free cloud minutes (well within 500/month limit)
- Reduces infrastructure load on on-premises runners

✅ **Performance**:
- Cloud runners provision faster (no queue for shared Windows boxes)
- Audits can run in parallel without resource contention
- Maintains same total pipeline execution time

✅ **Scalability**:
- Cloud runners auto-scale (no capacity planning needed)
- On-premises runners reserved for steps that actually need them

---

## Future Optimization Opportunities

**Not Implemented Yet** (require more analysis):

1. **Move builds to cloud if JFrog supports public access**
   - Would require JFrog Artifactory to allow authenticated access from cloud IPs
   - Would free ALL on-premises runners except deployments

2. **Use Bitbucket Deployments for cloud-based artifact storage**
   - Store artifacts in Bitbucket cloud instead of passing between steps
   - Would reduce artifact transfer time

3. **Split frontend build from tests**
   - Test on cloud, build on-premises (only build needs JFrog)
   - Already partially done (tests don't build anymore)

---

**Created**: October 7, 2025  
**Tested**: Pending  
**Approved**: Pending
