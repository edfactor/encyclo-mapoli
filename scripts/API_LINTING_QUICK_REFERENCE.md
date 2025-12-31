# API Linting Quick Reference

## One-Liners for Common Tasks

### Quick Lint (Auto-Download from QA)

```powershell
# Windows PowerShell
$env:QA_BASE_URL = "https://qa-api.example.com"; .\scripts\Lint-Api.ps1
```

```bash
# macOS/Linux
QA_BASE_URL=https://qa-api.example.com ./scripts/lint-api.sh
```

### Lint Local OpenAPI File

```powershell
# Windows
.\scripts\Lint-Api.ps1 -OpenApiPath ".\openapi.json"
```

```bash
# macOS/Linux
./scripts/lint-api.sh --openapi ./openapi.json
```

### Save JUnit Report

```powershell
# Windows
.\scripts\Lint-Api.ps1 -OutputFile "./reports/api-lint.xml" -OutputFormat junit
```

```bash
# macOS/Linux
./scripts/lint-api.sh --output ./reports/api-lint.xml
```

### Via npm

```bash
cd src/ui
npm run lint:api
```

---

## Script Locations & Documentation

| File                           | Platform    | Purpose                       |
| ------------------------------ | ----------- | ----------------------------- |
| `scripts/Lint-Api.ps1`         | Windows     | Primary linting script        |
| `scripts/lint-api.sh`          | macOS/Linux | Cross-platform alternative    |
| `scripts/API_LINTING_GUIDE.md` | All         | Complete documentation        |
| `.spectral.yaml`               | All         | Linting rules & configuration |

---

## Common Issues & Fixes

| Issue                      | Solution                                                          |
| -------------------------- | ----------------------------------------------------------------- |
| "QA_BASE_URL not set"      | `$env:QA_BASE_URL = "https://qa-api.example.com"`                 |
| "Spectral not found"       | Script installs automatically via npx                             |
| "Script execution blocked" | `powershell -ExecutionPolicy Bypass -File .\scripts\Lint-Api.ps1` |
| "npm run lint:api fails"   | Ensure `src/ui` exists and you're in project root                 |
| "SSL certificate error"    | Scripts automatically bypass self-signed certs                    |

---

## Integration Examples

### Pre-Commit Hook (Optional)

```bash
# .git/hooks/pre-commit
#!/bin/bash
./scripts/lint-api.sh --openapi ./openapi.json || exit 1
```

### GitHub Actions (Optional)

```yaml
- name: Lint API
  run: pwsh -File ./scripts/Lint-Api.ps1 -QaBaseUrl "${{ secrets.QA_BASE_URL }}"
```

### Local Workflow

```bash
# 1. Make API changes
# 2. Run aspire app
aspire run

# 3. Lint before committing
npm run lint:api  # or ./scripts/Lint-Api.ps1

# 4. Fix any errors
# 5. Push to GitHub/Bitbucket
```

---

## Full Help Documentation

```powershell
# Windows
Get-Help .\scripts\Lint-Api.ps1 -Full
```

```bash
# macOS/Linux
./scripts/lint-api.sh --help
```

---

## Files Modified in This Setup

```
✅ scripts/Lint-Api.ps1                   (Windows linting script)
✅ scripts/lint-api.sh                    (macOS/Linux linting script)
✅ scripts/API_LINTING_GUIDE.md           (Full documentation)
✅ src/ui/package.json                    (Added npm run lint:api)
✅ bitbucket-pipelines.yml                (Simplified CI/CD step)
📋 .spectral.yaml                         (Rules, unchanged)
```

---

## Need Help?

- **Quick Start**: This file
- **Detailed Guide**: `scripts/API_LINTING_GUIDE.md`
- **Implementation Details**: `API_LINTING_SETUP_SUMMARY.md`
- **PowerShell Help**: `Get-Help .\scripts\Lint-Api.ps1`
- **Bash Help**: `./scripts/lint-api.sh --help`

---

## Key Features

✅ **One Command**: Easy local linting  
✅ **No Setup**: Installs dependencies automatically  
✅ **QA Ready**: Downloads OpenAPI from QA or accepts local files  
✅ **CI-Friendly**: JUnit output for Bitbucket integration  
✅ **Developer-Friendly**: Readable console output  
✅ **Cross-Platform**: Windows PowerShell, bash, npm  
✅ **DRY**: Single source of truth (no pipeline duplication)  
✅ **Self-Documenting**: Built-in help and guides
