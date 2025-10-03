# Install Git Hooks Script
# Run this script to set up git hooks for the Smart Profit Sharing project

$ErrorActionPreference = "Stop"

Write-Host "Installing git hooks for Smart Profit Sharing..." -ForegroundColor Cyan

$repoRoot = git rev-parse --show-toplevel
if ($LASTEXITCODE -ne 0) {
    Write-Error "Not in a git repository. Please run this script from within the repository."
    exit 1
}

$hooksDir = Join-Path $repoRoot ".git\hooks"
$preCommitHook = Join-Path $hooksDir "pre-commit"

# Check if .git/hooks directory exists
if (-not (Test-Path $hooksDir)) {
    Write-Error "Git hooks directory not found: $hooksDir"
    exit 1
}

# Create the pre-commit hook
Write-Host "Creating pre-commit hook..." -ForegroundColor Yellow

$preCommitContent = @'
#!/bin/sh
# Pre-commit hook to run dotnet format on staged C# files

echo "Running dotnet format on services..."

# Navigate to services directory and run format
cd "$(git rev-parse --show-toplevel)/src/services" || exit 1

# Check if there are any staged C# files in the services directory
STAGED_CS_FILES=$(git diff --cached --name-only --diff-filter=ACM | grep -E "^src/services/.*\.cs$" || true)

if [ -z "$STAGED_CS_FILES" ]; then
    echo "No C# files staged in services directory. Skipping dotnet format."
    exit 0
fi

# Run dotnet format on the solution
echo "Formatting C# files in services directory..."
dotnet format Demoulas.ProfitSharing.slnx --verify-no-changes --verbosity quiet

FORMAT_EXIT_CODE=$?

if [ $FORMAT_EXIT_CODE -ne 0 ]; then
    echo ""
    echo "❌ Code formatting issues detected!"
    echo "Running 'dotnet format' to fix formatting..."
    dotnet format Demoulas.ProfitSharing.slnx
    
    echo ""
    echo "✅ Formatting applied. Please review changes and stage them:"
    echo "   git add -u"
    echo "   git commit"
    echo ""
    exit 1
fi

echo "✅ Code formatting check passed!"
exit 0
'@

Set-Content -Path $preCommitHook -Value $preCommitContent -NoNewline

# Grant execute permissions on Windows
Write-Host "Setting execute permissions..." -ForegroundColor Yellow
icacls $preCommitHook /grant Everyone:RX | Out-Null

Write-Host ""
Write-Host "✅ Git hooks installed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "The pre-commit hook will now run 'dotnet format' on staged C# files in src/services." -ForegroundColor Cyan
Write-Host ""
Write-Host "To test the hook:" -ForegroundColor White
Write-Host "  1. Make a change to a C# file in src/services" -ForegroundColor Gray
Write-Host "  2. Stage the file: git add <file>" -ForegroundColor Gray
Write-Host "  3. Attempt to commit: git commit -m 'Test commit'" -ForegroundColor Gray
Write-Host ""
Write-Host "To bypass the hook (use sparingly): git commit --no-verify" -ForegroundColor Yellow
Write-Host ""
