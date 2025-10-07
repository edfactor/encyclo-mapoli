# UI Pipeline Cloud Migration Analysis

**Date**: October 7, 2025  
**Goal**: Move UI build and test steps to cloud runners where possible, simplify steps

## Current UI Steps Analysis

### 1. `test-frontend` - Unit Tests with Vitest ✅ CAN MOVE TO CLOUD

**Current**: Windows self-hosted with Volta  
**Dependencies**:
- Internal JFrog for `smart-ui-library` (private package)
- Volta for Node version management
- npm ci + vitest

**Why It CAN Move**:
- JFrog credentials available via pipeline variables (same as audit-frontend)
- Node 22 available in Docker images (`node:22-alpine`)
- No build artifacts needed - just runs tests
- Vitest is pure JavaScript, no Windows dependencies

**Proposed**: Cloud with `node:22-alpine` Docker image

**Simplifications**:
- Remove Volta (use Docker node version directly)
- Remove dotnet tool install (buildinfo not needed for tests)
- Remove conditional logic

---

### 2. `create-frontend-artifact-qa` - QA Build ✅ CAN MOVE TO CLOUD (with buildinfo separation)

**Current**: Windows self-hosted with Volta  
**Dependencies**:
- Internal JFrog for `smart-ui-library`
- ~~Demoulas.Common.Build.Cli (generates buildinfo.json)~~ ← **BLOCKED CLOUD MIGRATION**
- npm ci + vite build
- ~~Windows paths for artifact compression~~ ← Works in Linux with tar/zip

**NEW APPROACH - Separate buildinfo generation**:
1. **New Step**: `generate-buildinfo` (Windows) - Generates `.buildinfo.json` artifact
2. **Modified Step**: `create-frontend-artifact-*` (Cloud) - Downloads buildinfo artifact, builds, packages

**Why This ENABLES Cloud Migration**:
- buildinfo.json generated once on Windows, reused for all environments
- Cloud steps only need Node + JFrog access (no .NET tool needed)
- Linux tar/gzip works same as Windows Compress-Archive
- Vite build is pure Node, no OS dependencies

**Benefits**:
- ✅ Frees 3 Windows runners per release (QA + UAT + PROD builds)
- ✅ Faster builds on cloud (parallel execution)
- ✅ Cleaner separation of concerns
- ✅ buildinfo generated once, reused multiple times

---

### 3. `create-frontend-artifact-uat` - UAT Build ❌ CANNOT MOVE

**Same as QA** - Cannot move, but can simplify by:
- Removing duplicate code
- Sharing buildinfo generation logic
- Using environment variables for ENV parameter

---

### 4. `create-frontend-artifact-prod` - PROD Build ❌ CANNOT MOVE

**Same as QA/UAT** - Cannot move, but can simplify

---

### 5. `run-playwright-tests` - E2E Tests ❌ CANNOT MOVE

**Current**: Windows self-hosted with Volta  
**Dependencies**:
- Access to internal QA environment (https://appt23d...)
- Playwright browsers
- Volta for Node management
- .env from QA_Playwright variable

**Why It CANNOT Move**:
- Tests run against internal QA servers not accessible from cloud
- Network access required to internal environment

**Simplifications Possible**:
- Reduce complex .env parsing logic
- Use official Playwright Docker patterns (but still needs Windows for network access)

---

## Recommended Changes

### Phase 1: Move test-frontend to Cloud ✅

**Benefits**:
- Frees 1 Windows runner per pipeline
- Tests run in clean Docker environment
- Faster test execution (no Volta overhead)

**Implementation**:
```yaml
test-frontend: &test-frontend
  name: Build and Test Frontend UI
  image: node:22-alpine
  max-time: 10
  artifacts:
    - src/ui/coverage/**
    - src/ui/FE_Tests/**
  script:
    - cd src/ui
    - echo "Running frontend tests on cloud runner..."
    
    # Create .npmrc for JFrog access
    - |
      cat > .npmrc << EOF
      registry=https://demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/
      //demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/:_authToken=${JFROG_WORKSPACE_VARIABLE}
      EOF
    
    - npm ci
    - npm run test
```

**Simplifications**:
- ❌ Remove node_modules deletion (Docker starts fresh)
- ❌ Remove Volta (Docker has Node 22)
- ❌ Remove buildinfo generation (not needed for tests)
- ❌ Remove conditional tool install
- ✅ Direct npm ci + test

---

### Phase 2: Separate buildinfo Generation + Move UI Builds to Cloud ✅ GAME CHANGER

**New Architecture**:
```
Stage 1 (Windows):
  └─ generate-buildinfo → Artifact: .buildinfo.json

Stage 2 (Cloud - Parallel):
  ├─ create-frontend-artifact-qa (downloads buildinfo artifact)
  ├─ create-frontend-artifact-uat (downloads buildinfo artifact)
  └─ create-frontend-artifact-prod (downloads buildinfo artifact)
```

#### New Step: `generate-buildinfo`

```yaml
generate-buildinfo: &generate-buildinfo
  name: Generate Build Information
  runs-on:
    - windows
    - self.hosted
  artifacts:
    - .buildinfo.json
  script:
    - dotnet tool update --global Demoulas.Common.Build.Cli --no-cache
    - Demoulas.Common.Build.Cli --buildid $env:BITBUCKET_BUILD_NUMBER --Branch $env:BITBUCKET_BRANCH --CommitHash $env:BITBUCKET_COMMIT
    - echo "Generated .buildinfo.json"
```

#### Modified: Cloud-based artifact creation

```yaml
create-frontend-artifact-qa: &create-frontend-artifact-qa
  name: Create Frontend Artifact (QA)
  image: node:22-alpine
  max-time: 15
  artifacts:
    - dist/**
  script:
    - apk add --no-cache zip  # For artifact compression
    - cd src/ui
    - echo "Building QA frontend on cloud runner..."
    
    # Create .npmrc for JFrog access
    - |
      cat > .npmrc << EOF
      registry=https://demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/
      //demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/:_authToken=${JFROG_WORKSPACE_VARIABLE}
      EOF
    
    # Copy buildinfo from artifact (downloaded automatically by Bitbucket)
    - cp ../../.buildinfo.json ./public/.buildinfo.json
    
    # Install and build
    - npm ci
    - npm run build:qa
    
    # Package artifact
    - cp src/web.config build/
    - mkdir -p ../../dist
    - cd build && zip -r ../../../dist/Demoulas.ProfitSharing.UI.qa.zip * && cd ..
```

**Benefits**:
- ✅ **Frees 3 Windows runners per release** (QA + UAT + PROD builds move to cloud)
- ✅ **Parallel builds** - All 3 environments build simultaneously on cloud
- ✅ **Faster execution** - Cloud runners often faster than on-premises
- ✅ **buildinfo generated once** - Reused across all environments (consistency)
- ✅ **Simpler steps** - No .NET tool dependency in build steps

**Simplifications**:
- ❌ Remove Volta (Docker has Node 22)
- ❌ Remove dotnet tool from build steps
- ❌ Remove node_modules cleanup (Docker starts fresh)
- ✅ Replace PowerShell Compress-Archive with Linux zip
- ✅ Reuse buildinfo artifact (downloaded automatically)

---

### Phase 2: Simplify Frontend Artifact Creation (Windows) - DEPRECATED

**NOTE**: This approach is superseded by Phase 2 above (buildinfo separation).
The buildinfo separation approach is superior because it enables cloud migration.

```yaml
create-frontend-artifact: &create-frontend-artifact
  name: Create Frontend Artifact ($ENV)
  runs-on:
    - windows
    - self.hosted
    - volta
  artifacts:
    - dist/**
  script:
    - $envName = if ($env:ENVIRONMENT) { $env:ENVIRONMENT } else { "qa" }
    - cd src/UI
    
    # Create .npmrc
    - |
      @"
      registry=https://demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/
      //demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/:_authToken=$env:JFROG_WORKSPACE_VARIABLE
      "@ | Out-File -FilePath .npmrc -Encoding ASCII
    
    # Clean node_modules
    - if (Test-Path node_modules) { Remove-Item -Recurse -Force node_modules }
    
    # Install dependencies
    - npm ci
    
    # Generate buildinfo
    - dotnet tool update --global Demoulas.Common.Build.Cli --no-cache
    - Demoulas.Common.Build.Cli --buildid $env:BITBUCKET_BUILD_NUMBER --Branch $env:BITBUCKET_BRANCH --CommitHash $env:BITBUCKET_COMMIT
    - Copy-Item .buildinfo.json -Destination public -Force
    
    # Build
    - npm run build:$envName
    
    # Package
    - Copy-Item src\web.config -Destination ..\..\src\UI\build -Force
    - if (-not (Test-Path ..\..\dist)) { New-Item -Path ..\..\dist -ItemType Directory }
    - Compress-Archive -Path ..\..\src\UI\build\* -DestinationPath ..\..\dist\Demoulas.ProfitSharing.UI.$envName.zip -Force
```

Then use environment variables to call it:
```yaml
# In pipeline
- step:
    <<: *create-frontend-artifact
    name: Create Frontend Artifact (QA)
    environment:
      ENVIRONMENT: qa

- step:
    <<: *create-frontend-artifact
    name: Create Frontend Artifact (UAT)
    environment:
      ENVIRONMENT: uat
```

**Simplifications**:
- ✅ Reduce from 3 separate steps to 1 parameterized step
- ✅ Remove duplicate .npmrc creation code
- ✅ Remove duplicate node_modules cleanup
- ✅ Consolidate buildinfo generation
- ✅ Single maintenance point

---

## Resource Impact (UPDATED with buildinfo separation)

### Phase 1: Moving test-frontend to Cloud

**Per Pipeline Run**:
- **Frees 1 Windows runner** (test-frontend)
- **Uses ~3-5 cloud minutes** (npm ci + vitest)

### Phase 2: Buildinfo Separation + UI Builds to Cloud (MAJOR IMPROVEMENT)

**Per Release Pipeline**:
- **Before**: 
  - 3 Windows runners for UI builds (QA + UAT + PROD sequential)
  - ~30-45 minutes total Windows runner time
- **After**:
  - 1 Windows runner for buildinfo (~30 seconds)
  - 3 cloud runners for UI builds (parallel!)
  - ~10-15 minutes total cloud time (parallel execution)
  
**Savings**:
- **Frees 2-3 Windows runners** per release (builds run on cloud, only buildinfo on Windows)
- **Faster releases** - Parallel builds instead of sequential
- **More predictable** - Cloud resources scale better than on-premises

### Combined Impact (All Phases)

**Per PR Pipeline**:
- **Windows runners freed**: 3 (test-frontend + audit-frontend + secret-scan)
- **Cloud minutes used**: ~6-9 minutes

**Per Release Pipeline** (includes deployments):
- **Windows runners freed**: 4-5 (3 UI builds + test-frontend + audit-frontend)
- **Cloud minutes used**: ~20-25 minutes
  - test-frontend: ~3-5 min
  - audit-frontend: ~1-2 min
  - secret-scan: ~1-2 min
  - 3 UI builds (parallel): ~10-15 min total (not 30-45!)

**Monthly Estimate** (40 PRs + 10 releases):
- PR pipelines: 40 × 8 min = 320 minutes
- Release pipelines: 10 × 23 min = 230 minutes
- **Total: ~550 minutes/month**
- **Slightly over free tier** but massive Windows runner savings

**Trade-off Analysis**:
- Cost: ~$25-30/month for 50-100 extra cloud minutes (if over free tier)
- Benefit: Frees 4-5 Windows runners per pipeline × 10 releases = 40-50 Windows runner hours/month
- **ROI**: Excellent - on-premises Windows runners much more expensive than cloud minutes
- Clearer parameter passing

**No resource change** - still runs on Windows

---

## Implementation Priority (UPDATED)

### Phase 1: Quick Wins (Do First)
1. ✅ Move test-frontend to cloud - Immediate Windows runner savings (DONE)
2. ✅ Move audit-frontend to cloud - Already working (DONE)

### Phase 2: Game Changer (Do Next - HIGH VALUE)
1. ✅ **Create generate-buildinfo step** - Separate buildinfo generation
2. ✅ **Move UI artifact builds to cloud** - QA, UAT, PROD all to cloud
3. ✅ **Enable parallel UI builds** - All 3 environments build simultaneously

**Why Phase 2 is Critical**:
- Frees 3 more Windows runners per release
- Enables parallel builds (3-4x faster)
- Huge ROI even if slightly over free cloud minutes

### Cannot Do (Network Constraints)
- ❌ Backend builds (need internal JFrog with .NET packages)
- ❌ Playwright E2E (needs access to internal QA environment)
- ❌ Deployments (need internal server access)

---

## Risk Assessment (UPDATED)

| Change | Risk Level | Mitigation |
|--------|-----------|------------|
| test-frontend → cloud | **Very Low** | Same JFrog pattern as audit-frontend (already working) |
| generate-buildinfo step | **Very Low** | Pure extraction, no logic change |
| UI builds → cloud | **Low** | Vite build is Node-only, no OS dependencies |
| Linux zip vs Windows Compress-Archive | **Very Low** | Standard archive format, tested pattern |

**Risks**:
- Slightly over free cloud minutes (~50 min/month)
- Need to verify artifact download works in cloud steps
- Need to test zip command produces compatible archives

**Mitigations**:
- Cloud minutes are cheap ($0.50/hour) vs on-premises infrastructure
- Bitbucket artifact download is built-in, well-tested
- zip format is standard, compatible with PowerShell Expand-Archive

---

## Testing Checklist (UPDATED)

### Phase 1: test-frontend on Cloud
- [ ] Verify npm ci succeeds with JFrog auth
- [ ] Verify smart-ui-library downloads correctly
- [ ] Verify vitest runs and generates coverage
- [ ] Verify coverage artifacts are uploaded
- [ ] Check cloud minutes usage

### Phase 2: buildinfo Separation + Cloud UI Builds
- [ ] Verify generate-buildinfo step runs on Windows
- [ ] Verify .buildinfo.json artifact is created
- [ ] Verify cloud UI build steps download buildinfo artifact
- [ ] Verify buildinfo.json copied to public/ folder correctly
- [ ] Verify QA build produces correct artifact
- [ ] Verify UAT build produces correct artifact
- [ ] Verify PROD build produces correct artifact
- [ ] Verify zip archives are compatible with Windows deployment
- [ ] Verify parallel builds complete successfully
- [ ] Check total cloud minutes usage (should be ~20-25 per release)
- [ ] Verify deployment steps work with cloud-generated artifacts

### Artifact Verification
- [ ] Download QA artifact and verify web.config present
- [ ] Verify .buildinfo.json present in artifact
- [ ] Verify build/ directory structure matches expectations
- [ ] Test deployment to QA environment
- [ ] Compare artifact size (cloud vs Windows - should be similar)
- [ ] Verify QA build produces correct artifact
- [ ] Verify UAT build produces correct artifact
- [ ] Verify PROD build produces correct artifact
- [ ] Verify buildinfo.json content is correct
- [ ] Verify artifact deployment works unchanged

---

## Estimated Cloud Minutes Budget (Updated)

**Per Pipeline Run**:
- audit-frontend: ~1-2 minutes
- secret-scan: ~1-2 minutes
- test-frontend: ~3-5 minutes
- 3 UI builds (parallel): ~10-15 minutes
- **Total**: ~20-25 minutes per release pipeline

**Monthly Estimate** (40 PRs + 10 releases):
- PRs: 40 × 8 min = 320 minutes
- Releases: 10 × 23 min = 230 minutes
- **Total: ~550 minutes/month** (slightly over free tier, but huge Windows runner savings)

---

## Complete Implementation Code

### Step 1: New `generate-buildinfo` Step

Add this new step to the pipeline:

```yaml
generate-buildinfo: &generate-buildinfo
  name: Generate Build Information
  runs-on:
    - windows
    - self.hosted
  artifacts:
    - .buildinfo.json
  script:
    - echo "Generating build information..."
    - dotnet tool update --global Demoulas.Common.Build.Cli --no-cache
    - Demoulas.Common.Build.Cli --buildid $env:BITBUCKET_BUILD_NUMBER --Branch $env:BITBUCKET_BRANCH --CommitHash $env:BITBUCKET_COMMIT
    - if (!(Test-Path .buildinfo.json)) { Write-Error "buildinfo.json not generated"; exit 1 }
    - echo "Build info generated successfully"
    - Get-Content .buildinfo.json
```

### Step 2: Modified Cloud UI Artifact Steps

Replace existing Windows-based artifact steps with these cloud versions:

```yaml
create-frontend-artifact-qa: &create-frontend-artifact-qa
  name: Create Frontend Artifact (QA)
  image: node:22-alpine
  max-time: 15
  artifacts:
    - dist/**
  script:
    - apk add --no-cache zip
    - cd src/ui
    - echo "Building QA frontend on cloud runner..."
    
    # Create .npmrc for JFrog access
    - |
      cat > .npmrc << EOF
      registry=https://demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/
      //demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/:_authToken=${JFROG_WORKSPACE_VARIABLE}
      EOF
    
    # Copy buildinfo from artifact (downloaded automatically)
    - |
      if [ ! -f ../../.buildinfo.json ]; then
        echo "ERROR: .buildinfo.json not found in artifacts"
        exit 1
      fi
    - cp ../../.buildinfo.json ./public/.buildinfo.json
    - echo "Copied buildinfo.json to public/"
    
    # Install and build
    - npm ci
    - npm run build:qa
    
    # Package artifact (Linux zip instead of PowerShell Compress-Archive)
    - cp src/web.config build/
    - mkdir -p ../../dist
    - cd build && zip -r ../../../dist/Demoulas.ProfitSharing.UI.qa.zip . && cd ..
    - echo "QA artifact created successfully"

create-frontend-artifact-uat: &create-frontend-artifact-uat
  name: Create Frontend Artifact (UAT)
  image: node:22-alpine
  max-time: 15
  artifacts:
    - dist/**
  script:
    - apk add --no-cache zip
    - cd src/ui
    - echo "Building UAT frontend on cloud runner..."
    
    # Create .npmrc for JFrog access
    - |
      cat > .npmrc << EOF
      registry=https://demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/
      //demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/:_authToken=${JFROG_WORKSPACE_VARIABLE}
      EOF
    
    # Copy buildinfo from artifact
    - |
      if [ ! -f ../../.buildinfo.json ]; then
        echo "ERROR: .buildinfo.json not found in artifacts"
        exit 1
      fi
    - cp ../../.buildinfo.json ./public/.buildinfo.json
    - echo "Copied buildinfo.json to public/"
    
    # Install and build
    - npm ci
    - npm run build:uat
    
    # Package artifact
    - cp src/web.config build/
    - mkdir -p ../../dist
    - cd build && zip -r ../../../dist/Demoulas.ProfitSharing.UI.uat.zip . && cd ..
    - echo "UAT artifact created successfully"

create-frontend-artifact-prod: &create-frontend-artifact-prod
  name: Create Frontend Artifact (PROD)
  image: node:22-alpine
  max-time: 15
  artifacts:
    - dist/**
  script:
    - apk add --no-cache zip
    - cd src/ui
    - echo "Building PROD frontend on cloud runner..."
    
    # Create .npmrc for JFrog access
    - |
      cat > .npmrc << EOF
      registry=https://demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/
      //demoulas.jfrog.io/artifactory/api/npm/demoulas-npm-virt/:_authToken=${JFROG_WORKSPACE_VARIABLE}
      EOF
    
    # Copy buildinfo from artifact
    - |
      if [ ! -f ../../.buildinfo.json ]; then
        echo "ERROR: .buildinfo.json not found in artifacts"
        exit 1
      fi
    - cp ../../.buildinfo.json ./public/.buildinfo.json
    - echo "Copied buildinfo.json to public/"
    
    # Install and build
    - npm ci
    - npm run build:prod
    
    # Package artifact
    - mkdir -p ../../dist
    - cd build && zip -r ../../../dist/Demoulas.ProfitSharing.UI.prod.zip . && cd ..
    - echo "PROD artifact created successfully"
```

### Step 3: Pipeline Stage Configuration

Update your pipeline to use the new structure:

```yaml
pipelines:
  branches:
    develop:
      - stage:
          name: Build
          steps:
            - step: *build-and-test-backend
            - step: *test-frontend
            - step: *create-api-artifacts
            - step: *create-service-artifacts
            - step: *generate-buildinfo  # NEW: Generate once
      - stage:
          name: Create Artifacts (Parallel)
          steps:
            - parallel:
              - step: *create-frontend-artifact-qa  # Cloud - uses buildinfo artifact
              - step: *create-frontend-artifact-uat  # Cloud - uses buildinfo artifact
              - step: *create-frontend-artifact-prod # Cloud - uses buildinfo artifact
      - stage:
          name: Audit & Scan
          steps:
            - step: *audit-backend
            - step: *audit-frontend
            - step: *secret-scan
```

---

## Next Steps (UPDATED)

### Phase 1: Already Implemented ✅
1. ✅ test-frontend moved to cloud
2. ✅ audit-frontend moved to cloud
3. ✅ Frontend artifact steps simplified

### Phase 2: Buildinfo Separation (Recommended Implementation)
1. **Add generate-buildinfo step** (Windows, ~30 seconds)
2. **Test artifact download** - Verify .buildinfo.json available to cloud steps
3. **Convert one artifact step** - Start with QA, test thoroughly
4. **Convert remaining steps** - UAT and PROD
5. **Enable parallel execution** - Configure pipeline stages
6. **Monitor cloud minutes** - Track usage after deployment

### Validation Steps
1. Test in feature branch first
2. Verify artifact compatibility with deployment scripts
3. Check cloud minutes don't exceed budget significantly
4. Validate parallel builds complete successfully
5. Confirm Windows runner usage drops as expected

### Rollback Plan
- Keep old Windows-based steps commented in pipeline
- Can switch back by reverting to previous commit
- Deployment scripts don't need changes (same artifact format)

