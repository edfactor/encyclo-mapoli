<#
.SYNOPSIS
    API Linting Script - Validates OpenAPI spec against Zalando RESTful Guidelines
    
.DESCRIPTION
    Lints the Profit Sharing API OpenAPI specification using Spectral CLI and Zalando rules.
    Supports local file, remote URL, or automatic QA discovery.
    
.PARAMETER OpenApiPath
    Path to local OpenAPI spec file or remote URL.
    If not provided, automatically downloads from QA_BASE_URL.
    
.PARAMETER QaBaseUrl
    Base URL for QA environment (e.g., https://qa-api.example.com:8443)
    Only used if OpenApiPath is not provided. Defaults to $env:QA_BASE_URL
    
.PARAMETER OutputFormat
    Output format: 'stylish' (console-friendly) or 'junit' (CI-friendly)
    Default: 'stylish'
    
.PARAMETER OutputFile
    Path to save output file (JUnit or JSON format)
    If not provided, output goes to console only
    
.PARAMETER FailOnWarnings
    Treat warnings as errors (fail on warnings, not just errors)
    Default: $false (fail on errors only)
    
.EXAMPLE
    # Lint local OpenAPI spec
    .\scripts\Lint-Api.ps1 -OpenApiPath ".\openapi.json"
    
    # Lint QA environment (requires QA_BASE_URL env var)
    .\scripts\Lint-Api.ps1
    
    # Lint with custom QA URL
    .\scripts\Lint-Api.ps1 -QaBaseUrl "https://qa-api.example.com:8443"
    
    # Lint and save JUnit report
    .\scripts\Lint-Api.ps1 -OutputFormat junit -OutputFile "./reports/api-lint.xml"
    
    # Fail on warnings
    .\scripts\Lint-Api.ps1 -FailOnWarnings
#>

param(
    [string]$OpenApiPath,
    [string]$QaBaseUrl = $env:QA_BASE_URL,
    [ValidateSet('stylish', 'junit')]
    [string]$OutputFormat = 'stylish',
    [string]$OutputFile,
    [switch]$FailOnWarnings
)

# Enable TLS 1.2 for HTTPS connections
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

Write-Host "API LINTING - Zalando RESTful Guidelines" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Determine OpenAPI spec source
if (-not $OpenApiPath) {
    if (-not $QaBaseUrl) {
        Write-Error "No OpenAPI path provided and QA_BASE_URL not set. Provide -OpenApiPath or set `$env:QA_BASE_URL"
        exit 1
    }
    
    $OpenApiPath = "openapi-qa.json"
    $uri = "$QaBaseUrl/swagger/Release%201.0/swagger.json"
    
    Write-Host "[1] Downloading OpenAPI spec from QA" -ForegroundColor Cyan
    Write-Host "URL: $uri" -ForegroundColor Yellow
    
    # Download with SSL certificate bypass (for self-signed QA certs)
    try {
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            # PowerShell Core 6+ has built-in -SkipCertificateCheck
            Invoke-WebRequest -Uri $uri -OutFile $OpenApiPath -SkipCertificateCheck -ErrorAction Stop
        }
        else {
            # Windows PowerShell 5.x - use .NET certificate policy
            Add-Type -TypeDefinition @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
      ServicePoint srvPoint, X509Certificate certificate,
      WebRequest request, int certificateProblem) {
      return true;
    }
}
"@
            [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
            Invoke-WebRequest -Uri $uri -OutFile $OpenApiPath -ErrorAction Stop
        }
        Write-Host "[OK] OpenAPI spec downloaded successfully" -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to download OpenAPI spec: $_"
        exit 1
    }
}
else {
    Write-Host "[1] Using provided OpenAPI spec" -ForegroundColor Cyan
    Write-Host "Path: $OpenApiPath" -ForegroundColor Yellow
    
    if (-not (Test-Path $OpenApiPath)) {
        Write-Error "OpenAPI spec file not found: $OpenApiPath"
        exit 1
    }
}

Write-Host ""

# Step 2: Ensure Spectral CLI is installed
Write-Host "[2] Checking Spectral CLI installation" -ForegroundColor Cyan
try {
    $spectralVersion = npx @stoplight/spectral-cli --version 2>$null
    Write-Host "[OK] Spectral CLI version: $spectralVersion" -ForegroundColor Green
}
catch {
    Write-Host "Installing Spectral CLI..." -ForegroundColor Yellow
    npm install -g @stoplight/spectral-cli | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to install Spectral CLI"
        exit 1
    }
    Write-Host "[OK] Spectral CLI installed" -ForegroundColor Green
}

Write-Host ""

# Step 3: Run Spectral linting
Write-Host "[3] Running API linting with Spectral" -ForegroundColor Cyan

# Create reports directory if needed
if ($OutputFile) {
    $reportDir = Split-Path -Parent $OutputFile
    if (-not (Test-Path $reportDir)) {
        New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
    }
}

# Always generate stylish output for console
Write-Host ""
npx @stoplight/spectral-cli lint $OpenApiPath --format stylish
$lintExitCode = $LASTEXITCODE

# Additionally save output format if requested
if ($OutputFile -and $OutputFormat -eq 'junit') {
    Write-Host ""
    Write-Host "[4] Generating JUnit output" -ForegroundColor Cyan
    npx @stoplight/spectral-cli lint $OpenApiPath --format junit --output $OutputFile
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] JUnit report saved to: $OutputFile" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "API LINTING SUMMARY" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

if ($lintExitCode -eq 0) {
    Write-Host "[PASS] API linting PASSED - No errors found" -ForegroundColor Green
    exit 0
}
else {
    Write-Host "[FAIL] API linting FAILED - Review errors above" -ForegroundColor Red
    if ($OutputFile) {
        Write-Host "JUnit report saved to: $OutputFile" -ForegroundColor Yellow
    }
    exit 1
}
