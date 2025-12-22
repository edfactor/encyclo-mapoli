<#
.SYNOPSIS
    Automatically adds ExampleRequest and ResponseExamples to FastEndpoints Summary() blocks.

.DESCRIPTION
    Uses Roslyn CodeAnalysis to parse C# endpoint files, discover Example() methods on 
    request/response DTOs, and inject missing examples into Summary() blocks with correct 
    HTTP status codes.

.PARAMETER WhatIf
    Preview changes without modifying files (dry-run mode).

.PARAMETER Apply
    Apply changes to files. Required to make actual modifications.

.PARAMETER Path
    Path to endpoints directory. Defaults to ../src/services/src/Demoulas.ProfitSharing.Endpoints

.EXAMPLE
    .\Add-OpenApiExamples.ps1 -WhatIf
    Preview what changes would be made.

.EXAMPLE
    .\Add-OpenApiExamples.ps1 -Apply
    Apply changes to all endpoint files.

.NOTES
    Requires: Microsoft.CodeAnalysis.CSharp NuGet package (auto-installed if missing)
    Author: Auto-generated
    Date: 2025-12-18
#>

[CmdletBinding(DefaultParameterSetName = 'WhatIf')]
param(
    [Parameter(ParameterSetName = 'WhatIf')]
    [switch]$WhatIf,
    
    [Parameter(ParameterSetName = 'Apply')]
    [switch]$Apply,
    
    [Parameter()]
    [string]$Path
)

# Initialize counters
$script:Stats = @{
    TotalScanned           = 0
    RequestExamplesAdded   = 0
    ResponseExamplesAdded  = 0
    AlreadyComplete        = 0
    SkippedNoExampleMethod = 0
    Errors                 = 0
    ModifiedFiles          = @()
}

#region Roslyn Setup

function Install-RoslynPackages {
    [CmdletBinding()]
    param()
    
    Write-Host "Checking for Roslyn CodeAnalysis packages..." -ForegroundColor Cyan
    
    $packagesDir = Join-Path $PSScriptRoot "packages"
    if (-not (Test-Path $packagesDir)) {
        New-Item -ItemType Directory -Path $packagesDir | Out-Null
    }
    
    $requiredPackages = @(
        @{ Name = "Microsoft.CodeAnalysis.CSharp"; Version = "4.8.0" }
        @{ Name = "Microsoft.CodeAnalysis.CSharp.Workspaces"; Version = "4.8.0" }
        @{ Name = "Microsoft.CodeAnalysis.Workspaces.MSBuild"; Version = "4.8.0" }
        @{ Name = "Microsoft.Build.Locator"; Version = "1.6.1" }
    )
    
    foreach ($package in $requiredPackages) {
        $packagePath = Join-Path $packagesDir "$($package.Name).$($package.Version)"
        
        if (-not (Test-Path $packagePath)) {
            Write-Host "  Installing $($package.Name) $($package.Version)..." -ForegroundColor Yellow
            
            $nugetExe = Join-Path $packagesDir "nuget.exe"
            if (-not (Test-Path $nugetExe)) {
                Write-Host "  Downloading NuGet.exe..." -ForegroundColor Yellow
                Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nugetExe
            }
            
            & $nugetExe install $package.Name -Version $package.Version -OutputDirectory $packagesDir -NonInteractive | Out-Null
        }
    }
    
    Write-Host "Loading Roslyn assemblies..." -ForegroundColor Cyan
    
    # Load assemblies
    $assemblies = @(
        "Microsoft.CodeAnalysis.dll"
        "Microsoft.CodeAnalysis.CSharp.dll"
        "Microsoft.CodeAnalysis.Workspaces.dll"
        "Microsoft.CodeAnalysis.CSharp.Workspaces.dll"
        "Microsoft.CodeAnalysis.Workspaces.MSBuild.dll"
        "Microsoft.Build.Locator.dll"
    )
    
    foreach ($assembly in $assemblies) {
        $assemblyPath = Get-ChildItem -Path $packagesDir -Filter $assembly -Recurse -File | 
        Where-Object { $_.FullName -like "*netstandard2.0*" -or $_.FullName -like "*net6.0*" } | 
        Select-Object -First 1
        
        if ($assemblyPath) {
            Add-Type -Path $assemblyPath.FullName -ErrorAction SilentlyContinue
        }
    }
    
    Write-Host "Roslyn packages loaded successfully." -ForegroundColor Green
}

#endregion

#region Helper Functions

function Get-EndpointFiles {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$RootPath
    )
    
    $endpointsPath = Join-Path $RootPath "Endpoints"
    if (-not (Test-Path $endpointsPath)) {
        throw "Endpoints directory not found: $endpointsPath"
    }
    
    Get-ChildItem -Path $endpointsPath -Filter "*Endpoint.cs" -Recurse -File
}

function Get-RequestResponseTypes {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$ClassDeclaration
    )
    
    # Match patterns like: ProfitSharingEndpoint<TRequest, TResponse>
    if ($ClassDeclaration -match 'ProfitSharingEndpoint<([^,>]+),\s*([^>]+)>') {
        $requestType = $matches[1].Trim()
        $responseType = $matches[2].Trim()
        
        # Unwrap Results<Ok<T>, ...> to get inner T
        if ($responseType -match 'Results<Ok<([^>]+)>') {
            $responseType = $matches[1].Trim()
        }
        
        return @{
            RequestType  = $requestType
            ResponseType = $responseType
        }
    }
    
    # Handle EndpointWithoutRequest
    if ($ClassDeclaration -match 'ProfitSharingResultResponseEndpoint<([^>]+)>') {
        $responseType = $matches[1].Trim()
        if ($responseType -match 'ListResponseDto<([^>]+)>') {
            $responseType = $matches[1].Trim()
        }
        return @{
            RequestType  = $null
            ResponseType = $responseType
        }
    }
    
    if ($ClassDeclaration -match 'ProfitSharingResponseEndpoint<([^>]+)>') {
        return @{
            RequestType  = $null
            ResponseType = $matches[1].Trim()
        }
    }
    
    return $null
}

function Find-ExampleMethod {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$TypeName,
        
        [Parameter(Mandatory)]
        [string]$ContractsPath
    )
    
    # Search for the type file
    $typeFile = Get-ChildItem -Path $ContractsPath -Filter "$TypeName.cs" -Recurse -File | Select-Object -First 1
    
    if (-not $typeFile) {
        return $null
    }
    
    $content = Get-Content -Path $typeFile.FullName -Raw
    
    # Look for Example(), RequestExample(), ResponseExample(), SampleRequest(), or SampleResponse() methods
    # Handle both 'public static' and 'public static new' modifiers
    if ($content -match 'public\s+static(\s+new)?\s+\w+\s+(RequestExample|ResponseExample|Example|SampleRequest|SampleResponse)\s*\(\s*\)') {
        return $matches[2]  # Note: $matches[2] because $matches[1] is the optional 'new' capture group
    }
    
    return $null
}

function Get-HttpStatusCode {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$ConfigureMethodBody
    )
    
    # Determine HTTP status code from verb
    if ($ConfigureMethodBody -match 'Post\s*\(') {
        return 201  # Created
    }
    elseif ($ConfigureMethodBody -match 'Delete\s*\(') {
        return 204  # No Content
    }
    elseif ($ConfigureMethodBody -match '(Get|Put|Patch)\s*\(') {
        return 200  # OK
    }
    
    return 200  # Default
}

function Test-HasExistingExample {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$SummaryBlock,
        
        [Parameter(Mandatory)]
        [ValidateSet('Request', 'Response')]
        [string]$ExampleType
    )
    
    if ($ExampleType -eq 'Request') {
        return $SummaryBlock -match 's\.ExampleRequest\s*='
    }
    else {
        return $SummaryBlock -match 's\.ResponseExamples\s*='
    }
}

function Add-ExampleToSummary {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Content,
        
        [Parameter(Mandatory)]
        [string]$RequestType,
        
        [Parameter()]
        [string]$ResponseType,
        
        [Parameter(Mandatory)]
        [string]$RequestExampleMethod,
        
        [Parameter()]
        [string]$ResponseExampleMethod,
        
        [Parameter(Mandatory)]
        [int]$StatusCode
    )
    
    # Find Summary() block
    if ($Content -notmatch '(?s)Summary\s*\(\s*s\s*=>\s*\{(.*?)\}\s*\)') {
        return $null
    }
    
    $summaryBlock = $matches[0]
    $summaryContent = $matches[1]
    
    # Check what's already present
    $hasRequestExample = Test-HasExistingExample -SummaryBlock $summaryBlock -ExampleType 'Request'
    $hasResponseExample = Test-HasExistingExample -SummaryBlock $summaryBlock -ExampleType 'Response'
    
    $addedRequest = $false
    $addedResponse = $false
    $newSummaryBlock = $summaryBlock
    
    # Add request example if missing
    if (-not $hasRequestExample -and $RequestExampleMethod -and $RequestType) {
        $requestExample = "            s.ExampleRequest = $RequestType.$RequestExampleMethod();"
        
        # Insert after summary/description
        if ($summaryContent -match '(s\.Summary\s*=.*?;|s\.Description\s*=.*?;)') {
            $newSummaryBlock = $newSummaryBlock -replace '(s\.Summary\s*=.*?;)', "`$1`r`n$requestExample"
        }
        else {
            # Insert at beginning of block
            $newSummaryBlock = $newSummaryBlock -replace '\{', "{`r`n$requestExample"
        }
        
        $addedRequest = $true
    }
    
    # Add response example if missing
    if (-not $hasResponseExample -and $ResponseExampleMethod -and $ResponseType) {
        $responseExample = @"
            s.ResponseExamples = new Dictionary<int, object>
            {
                { $StatusCode, $ResponseType.$ResponseExampleMethod() }
            };
"@
        
        # Insert before closing brace
        $newSummaryBlock = $newSummaryBlock -replace '\}\s*\)', "$responseExample`r`n        });"
        
        $addedResponse = $true
    }
    
    if ($addedRequest -or $addedResponse) {
        $newContent = $Content -replace [regex]::Escape($summaryBlock), $newSummaryBlock
        return @{
            Content       = $newContent
            AddedRequest  = $addedRequest
            AddedResponse = $addedResponse
        }
    }
    
    return $null
}

function Process-EndpointFile {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [System.IO.FileInfo]$File,
        
        [Parameter(Mandatory)]
        [string]$ContractsPath,
        
        [Parameter(Mandatory)]
        [bool]$ApplyChanges
    )
    
    try {
        $content = Get-Content -Path $File.FullName -Raw
        
        # Extract class declaration
        if ($content -notmatch 'public\s+(?:sealed\s+)?class\s+\w+\s*:\s*([^\r\n]+)') {
            Write-Verbose "Skipping $($File.Name): Cannot find class declaration"
            $script:Stats.Errors++
            return
        }
        
        $classDeclaration = $matches[1]
        
        # Get request/response types
        $types = Get-RequestResponseTypes -ClassDeclaration $classDeclaration
        if (-not $types) {
            Write-Verbose "Skipping $($File.Name): Cannot extract types from: $classDeclaration"
            $script:Stats.Errors++
            return
        }
        
        # Find Configure() method to determine HTTP status code
        $statusCode = 200
        if ($content -match '(?s)public\s+override\s+void\s+Configure\s*\(\s*\)\s*\{(.*?)\}') {
            $configureBody = $matches[1]
            $statusCode = Get-HttpStatusCode -ConfigureMethodBody $configureBody
        }
        
        # Check for Example() methods
        $requestExampleMethod = $null
        $responseExampleMethod = $null
        
        if ($types.RequestType) {
            $requestExampleMethod = Find-ExampleMethod -TypeName $types.RequestType -ContractsPath $ContractsPath
        }
        
        if ($types.ResponseType) {
            $responseExampleMethod = Find-ExampleMethod -TypeName $types.ResponseType -ContractsPath $ContractsPath
        }
        
        # Skip if no example methods found
        if (-not $requestExampleMethod -and -not $responseExampleMethod) {
            Write-Verbose "Skipping $($File.Name): No Example() methods found for types"
            $script:Stats.SkippedNoExampleMethod++
            return
        }
        
        # Check if already has examples
        $hasSummary = $content -match 'Summary\s*\('
        if (-not $hasSummary) {
            Write-Verbose "Skipping $($File.Name): No Summary() block found"
            $script:Stats.Errors++
            return
        }
        
        $summaryBlock = $matches[0]
        $hasRequestExample = Test-HasExistingExample -SummaryBlock $summaryBlock -ExampleType 'Request'
        $hasResponseExample = Test-HasExistingExample -SummaryBlock $summaryBlock -ExampleType 'Response'
        
        if ($hasRequestExample -and $hasResponseExample) {
            Write-Verbose "Skipping $($File.Name): Already has both examples"
            $script:Stats.AlreadyComplete++
            return
        }
        
        # Add missing examples
        $result = Add-ExampleToSummary -Content $content `
            -RequestType $types.RequestType `
            -ResponseType $types.ResponseType `
            -RequestExampleMethod $requestExampleMethod `
            -ResponseExampleMethod $responseExampleMethod `
            -StatusCode $statusCode
        
        if ($result) {
            if ($result.AddedRequest) {
                $script:Stats.RequestExamplesAdded++
            }
            if ($result.AddedResponse) {
                $script:Stats.ResponseExamplesAdded++
            }
            
            if ($ApplyChanges) {
                Set-Content -Path $File.FullName -Value $result.Content -NoNewline
                Write-Host "  Modified: $($File.Name)" -ForegroundColor Green
            }
            else {
                Write-Host "  Would modify: $($File.Name)" -ForegroundColor Yellow
            }
            
            $script:Stats.ModifiedFiles += $File.FullName
        }
        else {
            Write-Verbose "Skipping $($File.Name): No changes needed"
            $script:Stats.AlreadyComplete++
        }
    }
    catch {
        Write-Warning "Error processing $($File.Name): $_"
        $script:Stats.Errors++
    }
}

#endregion

#region Main Execution

function Main {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "OpenAPI Example Auto-Generator" -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
    
    # Determine mode
    $applyChanges = $Apply.IsPresent
    if (-not $applyChanges -and -not $WhatIf.IsPresent) {
        Write-Host "No mode specified. Running in WhatIf mode (dry-run)." -ForegroundColor Yellow
        Write-Host "Use -Apply to make actual changes.`n" -ForegroundColor Yellow
    }
    
    # Determine paths
    $scriptDir = $PSScriptRoot
    $repoRoot = Split-Path $scriptDir -Parent
    
    if (-not $Path) {
        $Path = Join-Path $repoRoot "src\services\src\Demoulas.ProfitSharing.Endpoints"
    }
    
    if (-not (Test-Path $Path)) {
        Write-Error "Endpoints path not found: $Path"
        return
    }
    
    $contractsPath = Join-Path $repoRoot "src\services\src\Demoulas.ProfitSharing.Common\Contracts"
    if (-not (Test-Path $contractsPath)) {
        Write-Error "Contracts path not found: $contractsPath"
        return
    }
    
    Write-Host "Endpoints path: $Path" -ForegroundColor Gray
    Write-Host "Contracts path: $contractsPath" -ForegroundColor Gray
    Write-Host "Mode: $(if ($applyChanges) { 'APPLY' } else { 'WHATIF' })`n" -ForegroundColor Gray
    
    # Install Roslyn (optional - using regex parsing instead for simplicity)
    # Install-RoslynPackages
    
    # Get all endpoint files
    Write-Host "Scanning for endpoint files..." -ForegroundColor Cyan
    $endpointFiles = Get-EndpointFiles -RootPath $Path
    $script:Stats.TotalScanned = $endpointFiles.Count
    
    Write-Host "Found $($endpointFiles.Count) endpoint files.`n" -ForegroundColor Green
    
    # Process each file
    Write-Host "Processing endpoint files..." -ForegroundColor Cyan
    foreach ($file in $endpointFiles) {
        Write-Verbose "Processing: $($file.Name)"
        Process-EndpointFile -File $file -ContractsPath $contractsPath -ApplyChanges $applyChanges
    }
    
    # Display summary
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "Summary Report" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Total endpoints scanned:       $($script:Stats.TotalScanned)" -ForegroundColor White
    Write-Host "Request examples added:        $($script:Stats.RequestExamplesAdded)" -ForegroundColor Green
    Write-Host "Response examples added:       $($script:Stats.ResponseExamplesAdded)" -ForegroundColor Green
    Write-Host "Already complete (skipped):    $($script:Stats.AlreadyComplete)" -ForegroundColor Gray
    Write-Host "Skipped (no Example methods):  $($script:Stats.SkippedNoExampleMethod)" -ForegroundColor Yellow
    Write-Host "Errors:                        $($script:Stats.Errors)" -ForegroundColor Red
    Write-Host "Files modified:                $($script:Stats.ModifiedFiles.Count)" -ForegroundColor $(if ($applyChanges) { 'Green' } else { 'Yellow' })
    
    if ($script:Stats.ModifiedFiles.Count -gt 0 -and $script:Stats.ModifiedFiles.Count -le 20) {
        Write-Host "`nModified files:" -ForegroundColor Cyan
        foreach ($file in $script:Stats.ModifiedFiles) {
            Write-Host "  - $(Split-Path $file -Leaf)" -ForegroundColor Gray
        }
    }
    
    Write-Host "`n========================================`n" -ForegroundColor Cyan
    
    if (-not $applyChanges) {
        Write-Host "This was a dry-run. No files were modified." -ForegroundColor Yellow
        Write-Host "Run with -Apply to make actual changes.`n" -ForegroundColor Yellow
    }
    else {
        Write-Host "Changes applied successfully!" -ForegroundColor Green
        Write-Host "Run 'dotnet build' to verify compilation.`n" -ForegroundColor Cyan
    }
}

# Run main
Main

#endregion
