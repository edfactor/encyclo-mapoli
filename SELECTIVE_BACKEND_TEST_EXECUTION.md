# Selective Backend Test Execution via Dependency Graph

**Created:** December 22, 2025
**Status:** Implemented (git diff approach)
**Author:** Claude Code Analysis
**Implementation Date:** December 22, 2025

---

## Implementation Summary

A git diff-based approach has been implemented with the following changes to `bitbucket-pipelines.yml`:

### Changes Made

1. **New Step Definition: `selective-backend-analysis`** (lines 165-345)

   - Uses `git diff --name-only "origin/$targetBranch" HEAD` to get changed files (no fetch needed - origin refs are already available in Bitbucket Pipeline clones)
   - Detects global configuration changes (Directory.Build.props, Directory.Packages.props, global.json)
   - Maps changed file paths to affected projects
   - Writes decision to `selective-test-decision.json` artifact
   - Decision values: `RUN_ALL`, `SKIP_ALL`, or `SELECTIVE`

2. **Modified: `build-and-test-backend-unit`** (lines 21-43)

   - Checks for `selective-test-decision.json` at start of PR builds
   - Skips execution if decision is `SKIP_ALL` or `SELECTIVE` with `runUnitTests=false`

3. **Modified: `build-and-test-backend-architecture`** (lines 96-118)

   - Checks for `selective-test-decision.json` at start of PR builds
   - Skips execution if decision is `SKIP_ALL` or `SELECTIVE` with `runArchitectureTests=false`

4. **Updated PR Pipeline** (lines 1380-1391)
   - Added selective-backend-analysis step between Stage 2 (audits) and Stage 3 (tests)
   - Analysis runs only when backend files change (same changeset condition as tests)

### How It Works

```
PR Build Pipeline Flow:
┌─────────────────────────────────────────────────────────────────┐
│ Stage 1: Frontend Lint (conditional)                            │
├─────────────────────────────────────────────────────────────────┤
│ Stage 2: Audits + Secret Scan (parallel)                        │
├─────────────────────────────────────────────────────────────────┤
│ Stage 2.5: Selective Backend Analysis ◄─── NEW                  │
│   ├─ Run git diff against origin/$targetBranch                  │
│   ├─ Check for global config changes → RUN_ALL                  │
│   ├─ Map file paths to affected projects                        │
│   └─ Write decision to selective-test-decision.json             │
├─────────────────────────────────────────────────────────────────┤
│ Stage 3: Tests (parallel)                                       │
│   ├─ Unit Tests: reads decision → RUN or SKIP                   │
│   ├─ Architecture Tests: reads decision → RUN or SKIP           │
│   └─ Frontend Tests (unchanged)                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Decision Logic

| Scenario                    | Decision  | Unit Tests | Architecture Tests |
| --------------------------- | --------- | ---------- | ------------------ |
| Global config change        | RUN_ALL   | RUN        | RUN                |
| No backend changes          | SKIP_ALL  | SKIP       | SKIP               |
| Common/Data/Services change | SELECTIVE | RUN        | RUN                |
| OracleHcm only change       | SELECTIVE | RUN        | SKIP               |
| Analysis error              | RUN_ALL   | RUN        | RUN                |

---

## Executive Summary

This document outlines a plan to implement selective backend test execution for PR builds. Currently, the pipeline runs all backend unit tests whenever any backend file changes. By building a dependency graph and mapping tests to source files, we can significantly reduce CI build times by only running tests that are actually impacted by the changes.

---

## Table of Contents

1. [Current State Analysis](#current-state-analysis)
2. [Project Dependency Graph](#project-dependency-graph)
3. [Recommended Approaches](#recommended-approaches)
4. [Implementation Plan](#implementation-plan)
5. [Key Considerations](#key-considerations)
6. [Risk Mitigation](#risk-mitigation)

---

## Current State Analysis

### Frontend (Current Implementation)

The frontend already implements selective testing in `bitbucket-pipelines.yml`:

```yaml
# PR build detected - checking for changed files
changed_files=$(git diff --name-only "origin/$BITBUCKET_PR_DESTINATION_BRANCH"...HEAD | grep '^src/ui/src/' || true)

# Build test patterns based on changed areas
if echo "$changed_files" | grep -q '^src/ui/src/utils/'; then
    test_patterns="$test_patterns src/utils"
fi
# ... more directory mappings
```

**Why this works for frontend:**

- Flat directory structure with clear boundaries
- Each directory (`utils/`, `hooks/`, `components/`) maps directly to test directories
- Limited cross-directory dependencies

### Backend (Current State)

Currently runs **ALL** unit tests on any backend file change:

```yaml
condition:
  changesets:
    includePaths:
      - "src/services/**/*.cs"
      - "src/services/**/*.csproj"
```

**Why selective testing is harder for backend:**

- Complex project dependencies (Services → Data → Common, etc.)
- A change in `Common` should trigger tests in `Services`, `Endpoints`, `Api`, etc.
- Test organization (by feature/domain) doesn't map 1:1 to file paths
- Transitive dependencies must be considered

### Solution Structure

```
src/services/
├── src/
│   ├── Demoulas.ProfitSharing.Api/
│   ├── Demoulas.ProfitSharing.Analyzers/
│   ├── Demoulas.ProfitSharing.AppHost/
│   ├── Demoulas.ProfitSharing.CachingServices/
│   ├── Demoulas.ProfitSharing.Common/
│   ├── Demoulas.ProfitSharing.Data/
│   ├── Demoulas.ProfitSharing.Data.Cli/
│   ├── Demoulas.ProfitSharing.EmployeeDelta.Sync/
│   ├── Demoulas.ProfitSharing.EmployeeFull.Sync/
│   ├── Demoulas.ProfitSharing.EmployeePayroll.Sync/
│   ├── Demoulas.ProfitSharing.Endpoints/
│   ├── Demoulas.ProfitSharing.OracleHcm/
│   ├── Demoulas.ProfitSharing.Reporting/
│   ├── Demoulas.ProfitSharing.Security/
│   └── Demoulas.ProfitSharing.Services/
└── tests/
    ├── Demoulas.ProfitSharing.IntegrationTests/
    ├── Demoulas.ProfitSharing.UnitTests/
    ├── Demoulas.ProfitSharing.UnitTests.Architecture/
    ├── Demoulas.ProfitSharing.UnitTests.Common/
    └── yematchrunner/
```

---

## Project Dependency Graph

### Visual Representation

```
                         ┌─────────────────────┐
                         │         Api         │
                         └──────────┬──────────┘
                                    │
            ┌───────────────────────┼───────────────────────┐
            ▼                       ▼                       ▼
    ┌───────────────┐       ┌───────────────┐       ┌───────────────┐
    │   Endpoints   │       │   Services    │       │   Security    │
    └───────┬───────┘       └───────┬───────┘       └───────────────┘
            │                       │
    ┌───────┴───────┐       ┌───────┴───────┬───────────────┬───────────────┐
    ▼               ▼       ▼               ▼               ▼               ▼
┌───────┐     ┌─────────┐ ┌───────┐   ┌──────────┐   ┌───────────┐   ┌───────────┐
│ Data  │     │Security │ │ Data  │   │ Caching  │   │ Reporting │   │ Security  │
└───┬───┘     └─────────┘ └───┬───┘   └──────────┘   └───────────┘   └───────────┘
    │                         │
    ▼                         ▼
┌───────┐               ┌───────┐
│Common │               │Common │
└───────┘               └───────┘
```

### Dependency Matrix (from .csproj analysis)

| Project             | Dependencies                                                  |
| ------------------- | ------------------------------------------------------------- |
| **Api**             | Endpoints, Security, Services                                 |
| **Endpoints**       | Data, Security, Analyzers                                     |
| **Services**        | CachingServices, Common, Data, Security, Reporting, Analyzers |
| **Data**            | Common                                                        |
| **OracleHcm**       | Common, Data                                                  |
| **Reporting**       | Common                                                        |
| **Security**        | (base project)                                                |
| **Common**          | (base project)                                                |
| **CachingServices** | Common                                                        |

### Inverse Dependency Matrix (Impact Analysis)

This shows: "If project X changes, which projects are affected?"

| If Changed          | Affects (Direct + Transitive)                                         |
| ------------------- | --------------------------------------------------------------------- |
| **Common**          | Data, Services, Reporting, CachingServices, OracleHcm, Endpoints, Api |
| **Data**            | Services, Endpoints, OracleHcm, Api                                   |
| **Security**        | Services, Endpoints, Api                                              |
| **Services**        | Api                                                                   |
| **Endpoints**       | Api                                                                   |
| **Reporting**       | Services, Api                                                         |
| **CachingServices** | Services, Api                                                         |
| **OracleHcm**       | (only UnitTests)                                                      |

---

## Recommended Approaches

### Option 1: Use `dotnet-affected` Tool (Simplest)

An existing open-source tool that does exactly this: [dotnet-affected](https://github.com/leonardochaia/dotnet-affected)

#### Implementation

```yaml
# bitbucket-pipelines.yml
build-and-test-backend-selective: &build-and-test-backend-selective
  name: Build and Test Backend (Selective)
  runs-on:
    - windows
    - self.hosted
  script:
    # Install the tool
    - dotnet tool install --global dotnet-affected

    # Get affected projects
    - |
      $affected = dotnet affected `
        --solution src/services/Demoulas.ProfitSharing.slnx `
        --from origin/$env:BITBUCKET_PR_DESTINATION_BRANCH `
        --format text

      Write-Host "Affected projects: $affected"

      # Check if any test projects are affected
      if ($affected -match "UnitTests") {
        Write-Host "Running unit tests..."
        dotnet test src/services/tests/Demoulas.ProfitSharing.UnitTests
      }

      if ($affected -match "Architecture") {
        Write-Host "Running architecture tests..."
        dotnet test src/services/tests/Demoulas.ProfitSharing.UnitTests.Architecture
      }
```

#### Pros

- Minimal implementation effort
- Well-maintained open-source tool
- Handles transitive dependencies correctly
- Works with any .NET solution

#### Cons

- Project-level granularity only (runs entire test project)
- External dependency
- May not handle all edge cases

---

### Option 2: Custom PowerShell Script (More Control)

Build a custom script that:

1. Parses project references from .csproj files
2. Builds a dependency graph
3. Maps test files to source files via conventions
4. Filters tests based on changed files

#### Script: `scripts/dependency-analysis/Get-AffectedTests.ps1`

```powershell
<#
.SYNOPSIS
    Determines which tests to run based on changed files.

.DESCRIPTION
    Parses project dependencies, analyzes changed files, and returns
    a list of test filters or test file patterns to run.

.PARAMETER BaseBranch
    The branch to compare against (default: origin/develop)

.PARAMETER SolutionPath
    Path to the solution file

.EXAMPLE
    .\Get-AffectedTests.ps1 -BaseBranch "origin/develop"
#>
param(
    [string]$BaseBranch = "origin/develop",
    [string]$SolutionPath = "src/services/Demoulas.ProfitSharing.slnx"
)

# Configuration
$TestProjectPath = "src/services/tests/Demoulas.ProfitSharing.UnitTests"

#region Helper Functions

function Get-ProjectDependencies {
    param([string]$ProjectPath)

    $csproj = [xml](Get-Content $ProjectPath)
    $refs = $csproj.Project.ItemGroup.ProjectReference.Include | Where-Object { $_ }

    return $refs | ForEach-Object {
        [System.IO.Path]::GetFileNameWithoutExtension($_)
    }
}

function Build-DependencyGraph {
    param([string]$SolutionDir)

    $projects = Get-ChildItem -Path $SolutionDir -Filter "*.csproj" -Recurse
    $graph = @{}

    foreach ($proj in $projects) {
        $projName = $proj.BaseName
        $deps = Get-ProjectDependencies -ProjectPath $proj.FullName
        $graph[$projName] = $deps
    }

    return $graph
}

function Get-TransitiveDependents {
    param(
        [hashtable]$InverseGraph,
        [string]$Project,
        [System.Collections.Generic.HashSet[string]]$Visited = $null
    )

    if ($null -eq $Visited) {
        $Visited = [System.Collections.Generic.HashSet[string]]::new()
    }

    if ($Visited.Contains($Project)) {
        return @()
    }

    [void]$Visited.Add($Project)
    $dependents = @($Project)

    if ($InverseGraph.ContainsKey($Project)) {
        foreach ($dependent in $InverseGraph[$Project]) {
            $dependents += Get-TransitiveDependents -InverseGraph $InverseGraph -Project $dependent -Visited $Visited
        }
    }

    return $dependents | Select-Object -Unique
}

function Invert-DependencyGraph {
    param([hashtable]$Graph)

    $inverse = @{}

    foreach ($project in $Graph.Keys) {
        foreach ($dep in $Graph[$project]) {
            if (-not $inverse.ContainsKey($dep)) {
                $inverse[$dep] = @()
            }
            $inverse[$dep] += $project
        }
    }

    return $inverse
}

#endregion

#region Main Logic

# Step 1: Get changed files
Write-Host "Fetching changes from $BaseBranch..."
$changedFiles = git diff --name-only "$BaseBranch...HEAD" |
    Where-Object { $_ -match '^src/services/' -and $_ -match '\.(cs|csproj)$' }

if (-not $changedFiles) {
    Write-Host "No backend files changed."
    return ""
}

Write-Host "Changed files:`n$($changedFiles -join "`n")"

# Step 2: Build dependency graph
Write-Host "`nBuilding dependency graph..."
$solutionDir = Split-Path $SolutionPath -Parent
$graph = Build-DependencyGraph -SolutionDir $solutionDir
$inverseGraph = Invert-DependencyGraph -Graph $graph

# Step 3: Determine affected projects
$affectedProjects = [System.Collections.Generic.HashSet[string]]::new()

foreach ($file in $changedFiles) {
    # Extract project name from path
    if ($file -match 'src/services/src/([^/]+)/') {
        $folderName = $Matches[1]
        $projectName = "Demoulas.ProfitSharing.$($folderName -replace 'Demoulas\.ProfitSharing\.', '')"

        # Get all projects affected by this change (transitive)
        $affected = Get-TransitiveDependents -InverseGraph $inverseGraph -Project $projectName
        foreach ($p in $affected) {
            [void]$affectedProjects.Add($p)
        }
    }

    # Handle test file changes directly
    if ($file -match 'src/services/tests/') {
        [void]$affectedProjects.Add("UnitTests")
    }
}

Write-Host "`nAffected projects: $($affectedProjects -join ', ')"

# Step 4: Map to test patterns
$testPatterns = @()

# Project to test directory mapping
$projectTestMap = @{
    "Demoulas.ProfitSharing.Services" = @("Services", "CalendarServiceTests", "TotalServiceTests")
    "Demoulas.ProfitSharing.Endpoints" = @("Endpoints")
    "Demoulas.ProfitSharing.Common" = @("Common", "Contracts")
    "Demoulas.ProfitSharing.Data" = @("Data")
    "Demoulas.ProfitSharing.OracleHcm" = @("OracleHcm")
    "Demoulas.ProfitSharing.Security" = @("Security")
    "Demoulas.ProfitSharing.Reporting" = @("Reports")
    "Demoulas.ProfitSharing.Analyzers" = @("Analyzers")
}

foreach ($project in $affectedProjects) {
    if ($projectTestMap.ContainsKey($project)) {
        $testPatterns += $projectTestMap[$project]
    }
}

# Step 5: Build dotnet test filter
$testPatterns = $testPatterns | Select-Object -Unique

if ($testPatterns.Count -eq 0) {
    Write-Host "No test patterns matched. Running all tests."
    return ""
}

# Create filter for dotnet test --filter
$filter = ($testPatterns | ForEach-Object { "FullyQualifiedName~$_" }) -join " | "

Write-Host "`nTest filter: $filter"
return $filter

#endregion
```

#### Pros

- Full control over logic
- File-level granularity possible
- Can integrate project-specific rules
- No external dependencies

#### Cons

- More maintenance required
- Must handle edge cases manually
- More complex implementation

---

### Option 3: Hybrid Approach with Configuration (Recommended)

Combines the best of both approaches with a configuration-driven system.

#### Configuration File: `scripts/dependency-analysis/test-project-map.json`

```json
{
  "version": "1.0",
  "description": "Maps source projects to test directories and patterns",
  "globalPatterns": {
    "runAllTestsOn": [
      "Directory.Build.props",
      "Directory.Packages.props",
      "global.json",
      "**/SharedGlobalStateCollection.cs"
    ]
  },
  "projects": {
    "Demoulas.ProfitSharing.Services": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Services"],
      "testFilePatterns": [
        "CalendarServiceTests.cs",
        "TotalServiceTests.cs",
        "DistributionServiceTests.cs",
        "PayServiceTests.cs"
      ]
    },
    "Demoulas.ProfitSharing.Endpoints": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Endpoints"],
      "includeSubdirectories": true
    },
    "Demoulas.ProfitSharing.Common": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Common", "Contracts"],
      "triggersArchitectureTests": true
    },
    "Demoulas.ProfitSharing.Data": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Data"]
    },
    "Demoulas.ProfitSharing.OracleHcm": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["OracleHcm"]
    },
    "Demoulas.ProfitSharing.Security": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Security"],
      "testFilePatterns": ["*MaskingOperatorTests.cs"]
    },
    "Demoulas.ProfitSharing.Reporting": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Reports"]
    },
    "Demoulas.ProfitSharing.Analyzers": {
      "testProject": "Demoulas.ProfitSharing.UnitTests",
      "testDirectories": ["Analyzers"]
    }
  },
  "testProjects": {
    "Demoulas.ProfitSharing.UnitTests": {
      "path": "tests/Demoulas.ProfitSharing.UnitTests",
      "framework": "xunit"
    },
    "Demoulas.ProfitSharing.UnitTests.Architecture": {
      "path": "tests/Demoulas.ProfitSharing.UnitTests.Architecture",
      "framework": "xunit",
      "runOnAnyChange": false
    }
  }
}
```

#### Pipeline Integration

```yaml
build-and-test-backend-selective: &build-and-test-backend-selective
  name: Build and Test Backend (Selective)
  runs-on:
    - windows
    - self.hosted
  script:
    - |
      # Fetch destination branch for comparison
      git fetch origin "$env:BITBUCKET_PR_DESTINATION_BRANCH" --depth=1

      # Run the analysis script
      $testFilter = pwsh -File scripts/dependency-analysis/Get-AffectedTests.ps1 `
        -BaseBranch "origin/$env:BITBUCKET_PR_DESTINATION_BRANCH" `
        -ConfigPath "scripts/dependency-analysis/test-project-map.json"

      if ($testFilter -eq "ALL") {
        Write-Host "Running ALL tests (global change detected)"
        dotnet test src/services/Demoulas.ProfitSharing.slnx --configuration Release
      }
      elseif ($testFilter -eq "NONE") {
        Write-Host "No tests affected by changes"
      }
      else {
        Write-Host "Running selective tests with filter: $testFilter"
        dotnet test src/services/tests/Demoulas.ProfitSharing.UnitTests `
          --configuration Release `
          --filter "$testFilter"
      }
```

---

## Implementation Plan

### Phase 1: Setup (1-2 hours)

1. Create directory structure:

   ```
   scripts/
   └── dependency-analysis/
       ├── Get-AffectedTests.ps1
       ├── test-project-map.json
       └── README.md
   ```

2. Create initial configuration file with project mappings

3. Write the PowerShell analysis script

### Phase 2: Testing (2-4 hours)

1. Test locally with various change scenarios:

   - Single file in Services
   - Multiple files across projects
   - Common project change (transitive test)
   - Test file only change
   - Build configuration change

2. Validate test filter output matches expectations

### Phase 3: Pipeline Integration (1-2 hours)

1. Add the selective test step to `bitbucket-pipelines.yml`
2. Keep the existing full test as fallback
3. Add logging for debugging

### Phase 4: Monitoring & Refinement (Ongoing)

1. Monitor build times and test coverage
2. Adjust mappings based on actual test failures
3. Add edge cases as discovered

---

## Key Considerations

| Consideration               | Recommendation                                                                             |
| --------------------------- | ------------------------------------------------------------------------------------------ |
| **Transitive Dependencies** | If `Common` changes, must run tests for all dependent projects (Services, Endpoints, etc.) |
| **Test File Naming**        | Leverage convention: `{ClassName}Tests.cs` tests `{ClassName}.cs`                          |
| **New Test Files**          | If a test file is new/added, always run it                                                 |
| **Shared Test Fixtures**    | If `SharedGlobalStateCollection.cs` changes, run ALL tests                                 |
| **Build Config Changes**    | Changes to `.csproj`, `Directory.Build.props`, `global.json` → run ALL tests               |
| **Fallback Strategy**       | On any analysis error, fall back to running all tests                                      |
| **Architecture Tests**      | Only run when structural changes detected (Common, Endpoints, etc.)                        |

---

## Risk Mitigation

### Risk: Missing Test Coverage

**Mitigation:**

- Run full test suite on `develop` and `release` branch builds (not just PRs)
- Periodic full test runs even on PRs (e.g., nightly)
- Conservative mapping - when in doubt, include more tests

### Risk: False Negatives (Tests should run but don't)

**Mitigation:**

- Log all decisions for debugging
- Include transitive dependencies
- Special handling for shared fixtures
- Monitor test failures in production

### Risk: Analysis Script Failures

**Mitigation:**

- Always fall back to running all tests on script error
- Add timeout to analysis script
- Comprehensive error handling

### Risk: Increased Complexity

**Mitigation:**

- Start simple (project-level), add file-level later
- Document all decisions
- Regular review and cleanup of configuration

---

## Appendix: Test Directory Structure

Current test organization in `Demoulas.ProfitSharing.UnitTests`:

```
Demoulas.ProfitSharing.UnitTests/
├── Analyzers/
│   ├── AnalyzerVerifier.cs
│   └── ComplianceAnalyzersTests.cs
├── Common/
│   ├── DateOnlyExtensionsTests.cs
│   ├── SharedGlobalStateCollection.cs
│   ├── SsnExtensionsTests.cs
│   └── Validators/
├── Contracts/
│   ├── DtoCommonExtensionsTests.cs
│   ├── ErrorTests.cs
│   └── ResultTests.cs
├── Data/
│   ├── ComputedFullNameColumnTests.cs
│   └── Interceptors/
├── Endpoints/
│   ├── Adjustments/
│   ├── Beneficiaries/
│   ├── BeneficiaryInquiry/
│   ├── Distributions/
│   ├── ItOperations/
│   ├── Lookups/
│   ├── Navigations/
│   ├── ProfitDetails/
│   └── Validation/
├── MasterInquiry/
├── OracleHcm/
│   ├── Mappers/
│   ├── Repositories/
│   └── Services/
├── Reports/
│   ├── Adhoc/
│   ├── PostFrozen/
│   └── YearEnd/
└── Services/
    ├── AllocTransferValidationServiceTests.cs
    ├── BalanceEquationValidationServiceTests.cs
    ├── ChecksumValidationServiceTests.cs
    ├── EmployeeMasterInquiryServiceTests.cs
    └── MockDistributionTests.cs
```

---

## Next Steps

1. **Review this document** with the team
2. **Choose an approach** (Option 1, 2, or 3)
3. **Implement in a feature branch**
4. **Test with various PR scenarios**
5. **Deploy to pipeline**
6. **Monitor and iterate**

---

## References

- [dotnet-affected GitHub](https://github.com/leonardochaia/dotnet-affected)
- [MSBuild Project Reference Documentation](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items)
- [dotnet test --filter Documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests)
- [Bitbucket Pipelines Changesets](https://support.atlassian.com/bitbucket-cloud/docs/use-conditions-with-your-pipelines/)
