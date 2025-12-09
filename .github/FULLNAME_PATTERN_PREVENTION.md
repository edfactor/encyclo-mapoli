# Pre-commit Checks for FullName Pattern

## Automated Detection

Add this to your pre-commit hooks or CI/CD pipeline to catch violations early.

### PowerShell Script: `check-fullname-pattern.ps1`

```powershell
<#
.SYNOPSIS
Checks for FullName pattern violations in staged or modified files.

.DESCRIPTION
Detects:
- Backend Response DTOs using "Name" property instead of "FullName"
- Backend services assigning FullName without ComputeFullNameWithInitial
- Frontend components using .name property instead of .fullName
- Manual firstName/lastName concatenation in frontend code

.EXAMPLE
./check-fullname-pattern.ps1 -CheckStaged
#>

param(
    [switch]$CheckStaged = $false,
    [switch]$CheckUnstaged = $false,
    [switch]$CheckAll = $false
)

$violations = @()

# Determine which files to check
if ($CheckStaged) {
    $files = git diff --cached --name-only | Where-Object { $_ -match '\.(cs|tsx?|ts)$' }
} elseif ($CheckUnstaged) {
    $files = git diff --name-only | Where-Object { $_ -match '\.(cs|tsx?|ts)$' }
} elseif ($CheckAll) {
    $files = git ls-files | Where-Object { $_ -match '\.(cs|tsx?|ts)$' }
} else {
    $files = git diff HEAD --name-only | Where-Object { $_ -match '\.(cs|tsx?|ts)$' }
}

foreach ($file in $files) {
    if (-not (Test-Path $file)) { continue }

    $content = Get-Content $file -Raw
    $lineNumber = 0

    # Backend Response DTOs check
    if ($file -match 'Response\.cs$' -and $file -notmatch 'Archive|Test') {
        foreach ($line in (Get-Content $file)) {
            $lineNumber++

            # Flag "public string Name" (but allow "public string FullName" and "public string FrequencyName", "StatusName", etc.)
            if ($line -match 'public\s+string\s+Name\s*(?:{|=|;)' -and $line -notmatch 'FullName|FrequencyName|StatusName|TaxCodeName|KindName|TypeName') {
                $violations += @{
                    File = $file
                    Line = $lineNumber
                    Severity = "HIGH"
                    Issue = "Response DTO using 'Name' instead of 'FullName'"
                    Suggestion = "Rename 'public string Name' to 'public string FullName'"
                }
            }
        }
    }

    # Backend FullName assignment check
    if ($file -match 'Service\.cs$') {
        foreach ($line in (Get-Content $file)) {
            $lineNumber++

            # Flag FullName assignment without computation
            if ($line -match 'FullName\s*=\s*' -and
                $line -notmatch 'ComputeFullNameWithInitial' -and
                $line -notmatch '\.FullName\s*\?' -and  # Fallback pattern ok
                $line -notmatch '//.*FullName') {  # Commented code ok

                # Check if next few lines contain the computation
                $nextContext = $content -split '\n' | Select-Object -Skip ($lineNumber - 1) | Select-Object -First 5 | Select-String 'ComputeFullNameWithInitial'

                if (-not $nextContext) {
                    $violations += @{
                        File = $file
                        Line = $lineNumber
                        Severity = "HIGH"
                        Issue = "FullName assigned without ComputeFullNameWithInitial"
                        Suggestion = "Use: FullName = DtoCommonExtensions.ComputeFullNameWithInitial(lastName, firstName, middleName)"
                    }
                }
            }
        }
    }

    # Frontend .name property check
    if ($file -match '\.(tsx?|ts)$' -and $file -notmatch '__test__|\.test\.' -and $file -notmatch 'types/') {
        foreach ($line in (Get-Content $file)) {
            $lineNumber++

            # Flag .name property access (but allow headerName, displayName, kindName, etc.)
            if ($line -match '\.\b(name)\b' -and
                $line -notmatch '\.(headerName|displayName|kindName|statusName|frequencyName|typeName|roleName|pathName|taxCodeName|employmentTypeName|departmentName|payClassificationName|relationshipName)' -and
                $line -notmatch 'params\.data|checkedValue|params\.value' -and
                $line -notmatch '\/\/.*\.name' -and
                $line -notmatch 'key\s*:\s*.*\.name|href\s*:') {

                $violations += @{
                    File = $file
                    Line = $lineNumber
                    Severity = "MEDIUM"
                    Issue = "Using .name property (should use .fullName)"
                    Code = $line.Trim()
                }
            }

            # Flag manual firstName/lastName concatenation
            if ($line -match '(firstName|lastName)\s*\+' -or
                $line -match '\$\{.*firstName.*lastName' -or
                $line -match "firstName.*lastName|lastName.*firstName") {

                if ($line -notmatch '\/\/|comment|WRONG|❌') {
                    $violations += @{
                        File = $file
                        Line = $lineNumber
                        Severity = "MEDIUM"
                        Issue = "Manual firstName/lastName concatenation (should use .fullName)"
                        Code = $line.Trim()
                    }
                }
            }
        }
    }
}

# Report violations
if ($violations.Count -eq 0) {
    Write-Host "✅ No FullName pattern violations found" -ForegroundColor Green
    exit 0
}

Write-Host "❌ FullName pattern violations detected:" -ForegroundColor Red
Write-Host ""

foreach ($violation in $violations) {
    $color = if ($violation.Severity -eq "HIGH") { "Red" } else { "Yellow" }
    Write-Host "$($violation.Severity): $($violation.File):$($violation.Line)" -ForegroundColor $color
    Write-Host "  Issue: $($violation.Issue)" -ForegroundColor Gray
    if ($violation.Suggestion) {
        Write-Host "  Suggestion: $($violation.Suggestion)" -ForegroundColor Cyan
    }
    if ($violation.Code) {
        Write-Host "  Code: $($violation.Code)" -ForegroundColor Gray
    }
    Write-Host ""
}

exit 1
```

### Integration with Git Hooks

Save the script to `scripts/check-fullname-pattern.ps1`, then add to `.git/hooks/pre-commit`:

```bash
#!/bin/bash
# Pre-commit hook: Check FullName pattern

if ! pwsh -NoProfile -Command "& scripts/check-fullname-pattern.ps1 -CheckStaged"; then
    echo ""
    echo "❌ FullName pattern violations detected. Please fix before committing."
    echo "📖 See: .github/FULLNAME_CONSOLIDATION_GUIDE.md"
    exit 1
fi

exit 0
```

Make executable: `chmod +x .git/hooks/pre-commit`

### Installation Instructions

1. **Copy the script**

   ```bash
   cp scripts/check-fullname-pattern.ps1 .github/hooks/
   chmod +x .github/hooks/check-fullname-pattern.ps1
   ```

2. **Setup git hook** (run once per developer)

   ```bash
   cp .github/hooks/pre-commit .git/hooks/
   chmod +x .git/hooks/pre-commit
   ```

3. **Or use Husky** (alternative for Node projects)
   ```bash
   npm install husky --save-dev
   npx husky add .husky/pre-commit "pwsh -NoProfile -Command \"& scripts/check-fullname-pattern.ps1 -CheckStaged\""
   ```

## Code Review Checklist

### For Pull Request Reviewers

- [ ] New response DTOs use `FullName` (not `Name`)
- [ ] Services compute FullName using `ComputeFullNameWithInitial()`
- [ ] Queries include `LastName`, `FirstName`, `MiddleName` fields
- [ ] TypeScript DTOs have `fullName` property
- [ ] Components use `object.fullName` (no manual concatenation)
- [ ] No `.name` property access on person objects in frontend
- [ ] Grid columns use `field: "fullName"` not `field: "employeeName"`
- [ ] Unit tests verify "LastName, FirstName M" format

## ESLint Custom Rule (Optional)

For TypeScript projects, consider adding a custom ESLint rule:

```typescript
// .eslintrc-custom/rules/no-person-name-concatenation.js

module.exports = {
  meta: {
    type: "problem",
    docs: {
      description:
        "Prevent manual firstName/lastName concatenation - use fullName from backend",
      category: "Best Practices",
    },
  },
  create(context) {
    return {
      BinaryExpression(node) {
        // Detect firstName + lastName pattern
        if (node.operator === "+") {
          const left = context.getSourceCode().getText(node.left);
          const right = context.getSourceCode().getText(node.right);

          if (
            (left.includes("firstName") && right.includes("lastName")) ||
            (left.includes("lastName") && right.includes("firstName"))
          ) {
            context.report({
              node,
              message:
                "Use 'fullName' from backend instead of concatenating firstName/lastName",
            });
          }
        }
      },
      TemplateLiteral(node) {
        // Detect template literal interpolation
        const text = context.getSourceCode().getText(node);
        if (text.includes("firstName") && text.includes("lastName")) {
          context.report({
            node,
            message:
              "Use 'fullName' from backend instead of template interpolation",
          });
        }
      },
    };
  },
};
```

## CI/CD Pipeline Integration

Add to your pipeline (e.g., GitHub Actions, Azure Pipelines):

```yaml
# GitHub Actions example
- name: Check FullName Pattern
  run: |
    pwsh -NoProfile -Command "& scripts/check-fullname-pattern.ps1 -CheckAll"
  if: always()
```

---

**Benefits of These Checks**:

- ✅ Catches violations before merge
- ✅ Educates new developers on the pattern
- ✅ Prevents regression to old concatenation approach
- ✅ Consistent naming across codebase
- ✅ Better maintainability
