# API Linting Guide

This document explains how to lint the Profit Sharing API OpenAPI specification against Zalando RESTful API Guidelines.

## Quick Start

### Local Development (Recommended)

#### Option 1: PowerShell Script (All Platforms)

From the project root:

```powershell
# Lint against QA environment (auto-downloads OpenAPI spec)
$env:QA_BASE_URL = "https://qa-api.example.com"
.\scripts\Lint-Api.ps1

# Lint a local OpenAPI file
.\scripts\Lint-Api.ps1 -OpenApiPath "./openapi.json"

# Save JUnit report
.\scripts\Lint-Api.ps1 -OutputFile "./reports/api-lint.xml" -OutputFormat junit
```

#### Option 2: npm Script (from src/ui/)

```bash
# Requires QA_BASE_URL environment variable
npm run lint:api

# Or with explicit URL
$env:QA_BASE_URL="https://qa-api.example.com"
npm run lint:api
```

### CI/CD Pipeline

The BitBucket pipeline automatically lints the API during pull request checks:

```yaml
api-lint: &api-lint
  name: "API Linting (Zalando Guidelines)"
  # Configured to run with centralized script
```

## Configuration

### Spectral Configuration File: `.spectral.yaml`

The linting rules are defined in `.spectral.yaml` at the project root. This file:

- Extends Spectral's OpenAPI preset (`spectral:oas`)
- Enforces Zalando RESTful API Guidelines
- Defines severity levels for each rule (error, warning, info, off)

**Key Rules**:

- ✅ Operations must have `operationId`
- ✅ Paths must use kebab-case
- ✅ POST/PUT/PATCH must have request body
- ✅ All operations must define responses
- ✅ Responses should have schema definitions
- ✅ Properties should have descriptions
- ✅ Use standard HTTP status codes

To customize rules, edit `.spectral.yaml` and re-run the linter.

### Environment Variables

| Variable      | Required | Default | Description                                              |
| ------------- | -------- | ------- | -------------------------------------------------------- |
| `QA_BASE_URL` | Yes\*    | None    | Base URL for QA API (e.g., `https://qa-api.example.com`) |
| \*            | \*       | \*      | \*Only needed when downloading from QA environment       |

## Script Usage

### PowerShell Script: `scripts/Lint-Api.ps1`

#### Parameters

- **`-OpenApiPath`** (string): Path to local OpenAPI spec file or remote URL

  - If not provided, automatically downloads from QA
  - Example: `./openapi.json`, `https://api.example.com/openapi.json`

- **`-QaBaseUrl`** (string): Base URL for QA environment

  - Default: `$env:QA_BASE_URL`
  - Used only if `OpenApiPath` not provided
  - Example: `https://qa-api.example.com:8443`

- **`-OutputFormat`** (stylish|junit): Output format

  - `stylish`: Console-friendly output (default)
  - `junit`: JUnit XML for CI systems

- **`-OutputFile`** (string): Path to save output file

  - Only used with JUnit format
  - Example: `./reports/api-lint.xml`

- **`-FailOnWarnings`** (switch): Treat warnings as errors
  - Default: Fail on errors only

#### Examples

```powershell
# Download from QA and lint
$env:QA_BASE_URL = "https://qa-api.example.com"
.\scripts\Lint-Api.ps1

# Lint local file
.\scripts\Lint-Api.ps1 -OpenApiPath ".\openapi.json"

# Save JUnit report
.\scripts\Lint-Api.ps1 -OutputFile "./reports/api-lint.xml" -OutputFormat junit

# Fail on warnings
.\scripts\Lint-Api.ps1 -FailOnWarnings
```

## Workflow

### For Developers

1. **Make API changes** in your FastEndpoints code
2. **Generate OpenAPI** (Swagger runs automatically during `aspire run`)
3. **Lint the API**:
   ```powershell
   .\scripts\Lint-Api.ps1 -OpenApiPath "http://localhost:5000/swagger/Release%201.0/swagger.json"
   # or
   npm run lint:api  # (from src/ui/)
   ```
4. **Fix any errors** before pushing to main branch

### For CI/CD Pipeline

The pipeline automatically:

1. ✅ Downloads OpenAPI from QA environment
2. ✅ Runs Spectral linting with Zalando rules
3. ✅ Saves JUnit report for BitBucket integration
4. ✅ Fails the build if errors are found

## Troubleshooting

### "No OpenAPI path provided and QA_BASE_URL not set"

**Cause**: Missing environment variable or parameter

**Fix**:

```powershell
# Set environment variable
$env:QA_BASE_URL = "https://qa-api.example.com"
.\scripts\Lint-Api.ps1

# Or provide path directly
.\scripts\Lint-Api.ps1 -OpenApiPath "./openapi.json"
```

### "Failed to download OpenAPI spec"

**Cause**: Can't reach QA environment or SSL certificate issues

**Fix**:

```powershell
# For local testing, save OpenAPI from Swagger UI:
# 1. Run: aspire run
# 2. Navigate to: http://localhost:5000/swagger
# 3. Download: http://localhost:5000/swagger/Release%201.0/swagger.json
# 4. Save as: ./openapi.json
# 5. Lint: .\scripts\Lint-Api.ps1 -OpenApiPath "./openapi.json"
```

### "Spectral CLI not found"

**Cause**: Spectral not installed globally

**Fix**:

```powershell
npm install -g @stoplight/spectral-cli
```

Or the script will attempt to install it automatically via `npx`.

## Best Practices

1. **Lint Early**: Run the linter before opening pull requests
2. **Use Local Files**: Download OpenAPI locally and lint offline when possible
3. **Review Rules**: Keep `.spectral.yaml` in sync with API design standards
4. **Document Changes**: When updating `.spectral.yaml`, explain why in commit message
5. **Fail Fast**: Address linting errors immediately—don't accumulate technical debt

## Related Documentation

- [Zalando RESTful API Guidelines](https://opensource.zalando.com/restful-api-guidelines/) - Official guidelines
- [Spectral Documentation](https://docs.stoplight.io/docs/spectral) - Linting rules syntax
- [OpenAPI 3.0 Specification](https://spec.openapis.org/oas/v3.0.3) - API spec standard

## Contributing

To add or modify linting rules:

1. Update `.spectral.yaml` with new rules
2. Test locally: `.\scripts\Lint-Api.ps1 -OpenApiPath ./openapi.json`
3. Document in this file
4. Include Jira ticket reference in commit message

For example rules and patterns, see `.spectral.yaml` comments or Spectral documentation.
