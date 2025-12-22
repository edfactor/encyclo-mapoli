#!/usr/bin/env pwsh

# Pre-push hook for Windows (PowerShell)
# Run lint, test, and tsc if UI files changed
# Run build and selective tests if backend files changed

Write-Host ""
Write-Host "========================================"
Write-Host "Pre-push hook starting..." -ForegroundColor Cyan
Write-Host "========================================"

# Determine what to compare against:
# 1. If upstream exists, compare against it
# 2. Otherwise, compare against origin/develop (main branch)
$upstream = git rev-parse --verify "@{u}" 2>$null
if ($LASTEXITCODE -eq 0) {
    $compareRef = "@{u}"
} else {
    $compareRef = "origin/develop"
}

Write-Host "Comparing against: $compareRef"
Write-Host ""

# =============================================================================
# UI CHECKS - Run if UI files changed
# =============================================================================

# Get changed UI source files (only lintable extensions)
$changedUiFiles = git diff --name-only "$compareRef..HEAD" 2>$null | Where-Object {
    $_ -match '^src/ui/src/' -and $_ -match '\.(ts|tsx|js|jsx)$'
}

if ($changedUiFiles) {
    Write-Host "UI files changed - running checks..." -ForegroundColor Yellow

    Push-Location src/ui

    # Convert paths from repo root to relative to src/ui for ESLint
    $lintFiles = $changedUiFiles | ForEach-Object { $_ -replace '^src/ui/', '' }
    $lintFileCount = ($lintFiles | Measure-Object).Count

    Write-Host ""
    Write-Host "Running lint on $lintFileCount changed file(s)..." -ForegroundColor Cyan
    $lintArgs = $lintFiles -join ' '
    npx eslint $lintFiles
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Lint failed! Push aborted." -ForegroundColor Red
        Pop-Location
        exit 1
    }

    Write-Host ""
    Write-Host "Finding tests related to changed files..." -ForegroundColor Cyan

    # Build list of test directories/patterns to run based on changed files
    $testPatterns = @()

    # Check each major area and add to test patterns if files changed there
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/utils/' }) {
        $testPatterns += "src/utils"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/hooks/' }) {
        $testPatterns += "src/hooks"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/components/' }) {
        $testPatterns += "src/components"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/reduxstore/' }) {
        $testPatterns += "src/reduxstore"
    }

    # Check each page area
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/pages/DecemberActivities/' }) {
        $testPatterns += "src/pages/DecemberActivities"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/pages/FiscalClose/' }) {
        $testPatterns += "src/pages/FiscalClose"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/pages/Beneficiaries/' }) {
        $testPatterns += "src/pages/Beneficiaries"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/pages/InquiriesAndAdjustments/' }) {
        $testPatterns += "src/pages/InquiriesAndAdjustments"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/pages/Reports/' }) {
        $testPatterns += "src/pages/Reports"
    }
    if ($changedUiFiles | Where-Object { $_ -match '^src/ui/src/pages/Distributions/' }) {
        $testPatterns += "src/pages/Distributions"
    }

    if ($testPatterns.Count -gt 0) {
        Write-Host "   Running tests for: $($testPatterns -join ', ')"
        npm run test -- --run $testPatterns
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Tests failed! Push aborted." -ForegroundColor Red
            Pop-Location
            exit 1
        }
    } else {
        Write-Host "   No test areas matched changed files - skipping tests"
    }

    Write-Host ""
    Write-Host "Running TypeScript check..." -ForegroundColor Cyan
    npx tsc -b --noEmit
    if ($LASTEXITCODE -ne 0) {
        Write-Host "TypeScript check failed! Push aborted." -ForegroundColor Red
        Pop-Location
        exit 1
    }

    Write-Host ""
    Write-Host "All UI checks passed!" -ForegroundColor Green
    Pop-Location
} else {
    Write-Host "No UI files changed - skipping UI checks" -ForegroundColor Gray
}

# =============================================================================
# BACKEND CHECKS - Run if backend files changed
# =============================================================================

# Get changed backend files (specific extensions like pipeline)
$changedBackendFiles = git diff --name-only "$compareRef..HEAD" 2>$null | Where-Object {
    $_ -match '^src/services/' -and
    $_ -match '\.(cs|csproj|sln|slnx|json|config)$' -and
    $_ -notmatch '/tests/'
}

if ($changedBackendFiles) {
    Write-Host ""
    Write-Host "Backend files changed - running checks..." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Changed files:"
    $changedBackendFiles | ForEach-Object { Write-Host "   $_" }
    Write-Host ""

    $runAllTests = $false
    $namespaceFilters = @()

    # Check for critical infrastructure files (same as pipeline)
    $criticalPatterns = 'Endpoints/Base/|Extensions/TelemetryExtensions|Common/Contracts/Result|Common/Contracts/Error|Data/Contexts/|Data/Migrations/'
    if ($changedBackendFiles | Where-Object { $_ -match $criticalPatterns }) {
        Write-Host "Critical infrastructure file changed - will run ALL tests" -ForegroundColor Yellow
        $runAllTests = $true
    }

    # Map files to namespace filters (only if not already running all)
    if (-not $runAllTests) {
        # Feature mappings (same as pipeline)
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Beneficiaries/|Services/Beneficiaries/' }) {
            $namespaceFilters += "Beneficiaries"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Lookups/|Services/Lookup/' }) {
            $namespaceFilters += "Lookups"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Distributions/' }) {
            $namespaceFilters += "Distributions"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Adjustments/' }) {
            $namespaceFilters += "Adjustments"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/YearEnd/|Services/YearEnd|Services/ProfitShareEdit' }) {
            $namespaceFilters += "YearEnd"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Military/|Services/Military/' }) {
            $namespaceFilters += "Military"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Validation/|Services/Validation/' }) {
            $namespaceFilters += "Validation"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/ItOperations/|Services/ItDevOps/' }) {
            $namespaceFilters += "ItOperations"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/Navigations/|Services/Navigations/' }) {
            $namespaceFilters += "Navigation"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/BeneficiaryInquiry/|Services/BeneficiaryInquiry/' }) {
            $namespaceFilters += "BeneficiaryInquiry"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Endpoints/ProfitDetails/' }) {
            $namespaceFilters += "ProfitDetails"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'OracleHcm/' }) {
            $namespaceFilters += "OracleHcm"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Services/Reports/|Endpoints/Reports/|Reporting/' }) {
            $namespaceFilters += "Reports"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Data/Entities/' }) {
            $namespaceFilters += "Services"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Demoulas\.ProfitSharing\.Api/' }) {
            $namespaceFilters += "Endpoints"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'CachingServices/' }) {
            $namespaceFilters += "Services"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Security/' }) {
            $namespaceFilters += "Security"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Common/' }) {
            $namespaceFilters += "Common"
        }
        if ($changedBackendFiles | Where-Object { $_ -match 'Services/Pay/|Services/Total' }) {
            $namespaceFilters += "Services"
        }

        # Get unique filters
        $namespaceFilters = $namespaceFilters | Select-Object -Unique

        # Check for unknown files (files that didn't match any pattern)
        $knownPatterns = 'Endpoints/Beneficiaries/|Services/Beneficiaries/|Endpoints/Lookups/|Services/Lookup/|Endpoints/Distributions/|Endpoints/Adjustments/|Endpoints/YearEnd/|Services/YearEnd|Services/ProfitShareEdit|Endpoints/Military/|Services/Military/|Endpoints/Validation/|Services/Validation/|Endpoints/ItOperations/|Services/ItDevOps/|Endpoints/Navigations/|Services/Navigations/|Endpoints/BeneficiaryInquiry/|Services/BeneficiaryInquiry/|Endpoints/ProfitDetails/|OracleHcm/|Services/Reports/|Endpoints/Reports/|Data/Entities/|Endpoints/Base/|Extensions/TelemetryExtensions|Common/Contracts/Result|Common/Contracts/Error|Data/Contexts/|Data/Migrations/|Demoulas\.ProfitSharing\.Api/|CachingServices/|Security/|Common/|Services/Pay/|Services/Total|Reporting/|Analyzers/'

        foreach ($file in $changedBackendFiles) {
            # Skip non-.cs files for unknown check (config files are ok)
            if ($file -notmatch '\.cs$') { continue }

            # Check if file matches any known pattern
            if ($file -notmatch $knownPatterns) {
                Write-Host "Unknown file pattern: $file - will run ALL tests" -ForegroundColor Yellow
                $runAllTests = $true
                break
            }
        }
    }

    # Build the solution
    Write-Host "Building backend solution..." -ForegroundColor Cyan
    Push-Location src/services

    dotnet build Demoulas.ProfitSharing.slnx --configuration Debug --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Backend build failed! Push aborted." -ForegroundColor Red
        Pop-Location
        exit 1
    }

    # Determine test executable path (Windows uses .exe extension)
    $testExe = "tests/Demoulas.ProfitSharing.UnitTests/bin/Debug/net10.0/Demoulas.ProfitSharing.UnitTests.exe"

    # Check if test executable exists
    if (-not (Test-Path $testExe)) {
        Write-Host "Test executable not found at: $testExe" -ForegroundColor Red
        Write-Host "   Try running: dotnet build Demoulas.ProfitSharing.slnx"
        Pop-Location
        exit 1
    }

    Write-Host ""
    if ($runAllTests -or $namespaceFilters.Count -eq 0) {
        Write-Host "Running ALL backend unit tests..." -ForegroundColor Cyan
        & $testExe
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Backend tests failed! Push aborted." -ForegroundColor Red
            Pop-Location
            exit 1
        }
    } else {
        Write-Host "Running SELECTIVE backend tests for: $($namespaceFilters -join ', ')" -ForegroundColor Green

        # Build namespace args
        $namespaceArgs = @()
        foreach ($pattern in $namespaceFilters) {
            $namespaceArgs += "-namespace"
            $namespaceArgs += "*$pattern*"
        }

        Write-Host "   Filter: $($namespaceArgs -join ' ')"
        & $testExe @namespaceArgs
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Backend tests failed! Push aborted." -ForegroundColor Red
            Pop-Location
            exit 1
        }
    }

    Pop-Location
    Write-Host ""
    Write-Host "All backend checks passed!" -ForegroundColor Green
} else {
    Write-Host "No backend files changed - skipping backend checks" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================"
Write-Host "Pre-push hook completed successfully" -ForegroundColor Green
Write-Host "========================================"
Write-Host ""

exit 0
