#How to run script: .\Get-ChangedFiles.ps1 -Author "developer@example.com" -Branch "main" -OutputFile "ChangedFilesReport.txt"

# Parameters
param (
    [string]$Author = "developer@example.com",  # Email or name of the developer
    [string]$Branch = "main",                  # Target branch to analyze
    [string]$OutputFile = "ChangedFilesReport.txt" # Output file for the report
)

# Ensure Git is installed
if (-not (Get-Command "git" -ErrorAction SilentlyContinue)) {
    Write-Error "Git is not installed or not available in the PATH."
    exit 1
}

# Navigate to the Git repository
if (-not (Test-Path ".git")) {
    Write-Error "This script must be run from the root of a Git repository."
    exit 1
}

# Get all commit hashes by the author on the specified branch
Write-Host "Collecting commits by $Author on branch $Branch..."
$Commits = git log $Branch --author="$Author" --pretty=format:"%H" --no-merges

if (-not $Commits) {
    Write-Error "No commits found by $Author on branch $Branch."
    exit 1
}

# Initialize an empty set to store unique changed files
$ChangedFiles = @{}

# Process each commit
foreach ($Commit in $Commits) {
    Write-Host "Processing commit: $Commit"
    try {
        # Check if the commit has a parent
        $ParentCommit = git rev-parse "$Commit^" 2>$null
        if ($ParentCommit) {
            # Normal case: diff with the parent commit
            $Files = git diff --name-only "$Commit^" "$Commit"
        } else {
            # Initial/root commit: use git diff-tree
            $Files = git diff-tree --no-commit-id --name-only -r "$Commit"
        }

        # Add files to the hash table for uniqueness
        foreach ($File in $Files) {
            $ChangedFiles[$File] = $true
        }
    } catch {
        Write-Warning "Failed to process commit $Commit: $(${_})"
    }
}

# Output unique changed files to the report
$ChangedFiles.Keys | Sort-Object | Out-File -Encoding UTF8 $OutputFile

Write-Host "Report generated: $OutputFile"
