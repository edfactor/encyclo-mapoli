# API Linting Results - Critical Issues Found

**Scan Date**: December 31, 2025  
**File**: sample-openapi.json  
**Status**: ❌ FAILED - 10 Errors, 12 Warnings, 11 Info messages

---

## 🔴 Critical Issues (10 Errors)

These **MUST** be fixed before deployment:

### 1. **HTTP Verbs in Paths** (2 errors)
**Lines**: 74, 103

❌ **WRONG**:
```
/api/get-employee/{id}
/api/update_employee/
```

✅ **CORRECT**:
```
/api/employees/{id}
/api/employees
```

**Rule**: Don't use HTTP verbs in URLs. Use HTTP methods (GET, POST, PUT, DELETE) instead.

**Fix**: 
- `/api/get-employee/{id}` → `/api/employees/{id}` (GET method on this path)
- `/api/update_employee/` → `/api/employees` (PUT method on this path)

---

### 2. **Path Naming Convention - kebab-case** (5 errors)
**Lines**: 18, 74, 103, 125

❌ **WRONG**:
```
/api/employees
/api/get-employee/{id}
/api/update_employee/
/api/distributions
```

✅ **CORRECT** (actual paths appear to use camelCase, should be kebab-case):
```
/api/employees           (✓ already correct)
/api/employees/{id}      (change endpoint, use kebab-case: /api/employee-details/{id})
/api/employee            (consolidated endpoint)
/api/profit-distributions (use hyphens, not underscores)
```

**Rule**: Zalando guideline - use kebab-case (lowercase with hyphens) for URL paths.

---

### 3. **Missing Operation Descriptions** (3 errors)
**Lines**: 46, 75, 104, 126

❌ **WRONG**:
```json
{
  "operationId": "createEmployee",
  "tags": ["Employees"],
  "summary": "Create a new employee",
  // Missing description
}
```

✅ **CORRECT**:
```json
{
  "operationId": "createEmployee",
  "tags": ["Employees"],
  "summary": "Create a new employee",
  "description": "Creates a new employee record. Requires ADMIN role. Returns the created employee with assigned ID.",
  "parameters": []
}
```

**Rule**: Every operation must have a detailed description explaining what it does.

**Affected operations**:
- POST /api/employees
- GET /api/get-employee/{id}
- PUT /api/update_employee/
- GET /api/distributions

---

### 4. **Trailing Slash in Path** (1 error)
**Line**: 103

❌ **WRONG**:
```
/api/update_employee/
```

✅ **CORRECT**:
```
/api/employees  (or /api/employee if singular)
```

**Rule**: Paths must not end with a trailing slash.

---

## 🟡 Warnings (12 Warnings)

These should be addressed for better API design:

### Missing Global Tags Definition
**Lines**: 21, 48, 77, 106, 128

**Issue**: Operations reference tags ("Employees", "Distributions") but they're not defined at API root.

**Fix**: Add to root of OpenAPI spec:
```yaml
tags:
  - name: Employees
    description: Employee management operations
  - name: Distributions
    description: Profit distribution operations
```

### Missing Security Requirements (5 warnings)
**Lines**: 19, 46, 75, 104, 126

**Issue**: Operations don't specify required authentication/authorization.

**Fix**: Add security to each operation:
```json
{
  "operationId": "getEmployees",
  "security": [
    {"bearerAuth": ["read:employees"]}
  ]
}
```

### Missing Trailing Slash Consistency (1 warning)
**Line**: 103 - `/api/update_employee/` has trailing slash (should be removed)

---

## 🔵 Information Messages (11 Info)

These are recommendations for better documentation:

1. **Missing API Audience** (1 info)
   - Add `x-audience` field to info section
   - Example: `"x-audience": "internal"`

2. **Missing Pagination Parameters** (3 info)
   - Collection endpoints should support `limit`, `offset` or `cursor`
   - Add to `/api/employees`, `/api/get-employee/{id}`, `/api/distributions`

3. **Missing Examples** (4 info)
   - Request/response examples improve documentation
   - Add examples in requestBody and response schemas

4. **Missing Authorization Documentation** (3 info)
   - Operation descriptions should document required roles/policies
   - Example: "Authorization: Requires ADMIN role"

5. **Missing x-audience Field** (1 info)
   - Info object should declare API audience

---

## 📋 Summary by Severity

| Severity | Count | Status | Action Required |
|----------|-------|--------|-----------------|
| **Error** | 10 | ❌ CRITICAL | Fix immediately |
| **Warning** | 12 | ⚠️ Important | Fix before deployment |
| **Info** | 11 | ℹ️ Nice-to-have | Fix for quality |
| **Hint** | 0 | ✓ OK | N/A |

---

## 🎯 Priority Fixes (Do These First)

### HIGH PRIORITY (Must fix - Breaks API contract):
1. ✅ Remove HTTP verbs from paths (`get-employee` → `employees/{id}`)
2. ✅ Add missing operation descriptions
3. ✅ Fix trailing slashes
4. ✅ Ensure kebab-case consistency

### MEDIUM PRIORITY (Should fix - Best practices):
5. ✅ Add global tags definition
6. ✅ Add security requirements to all operations
7. ✅ Add pagination parameters to collection endpoints

### LOW PRIORITY (Nice to have - Documentation quality):
8. ✅ Add request/response examples
9. ✅ Add authorization scope documentation
10. ✅ Add x-audience field

---

## 🔧 How to Fix

### 1. Use the Linter Iteratively
```powershell
# After each fix, re-run linter
.\scripts\Lint-Api.ps1 -OpenApiPath ".\openapi.json"
```

### 2. Fix One Category at a Time
- Start with errors (10)
- Then warnings (12)
- Then info (11)

### 3. Validate Before Each Commit
```bash
npm run lint:api  # from src/ui/
```

### 4. Keep Rules in Sync
- `.spectral.yaml` defines all rules
- Follow Zalando RESTful API Guidelines
- Document any deviations

---

## 📚 Reference Rules

All errors detected are from Zalando RESTful API Guidelines:

| Rule | Purpose | Severity |
|------|---------|----------|
| `no-http-verbs-in-path` | Use HTTP methods, not verbs in URLs | ERROR |
| `path-kebab-case` | Use lowercase with hyphens | ERROR |
| `path-keys-no-trailing-slash` | No trailing slashes | ERROR |
| `operation-description` | Every op must be described | ERROR |
| `operation-tags` | Tags must be globally defined | WARNING |
| `operation-security-defined` | Security requirements required | WARNING |

See `.spectral.yaml` for all 30+ rules.

---

## ✅ Next Steps

1. **Review this report** with your team
2. **Prioritize fixes** (start with errors)
3. **Update OpenAPI spec** in your codebase
4. **Run linter** after each batch of fixes
5. **Commit changes** with clear messages
6. **Monitor pipeline** for linting success

---

## 💡 Tips

- **Work incrementally**: Fix a few issues, validate, commit
- **Use examples**: Copy fixed examples from similar operations
- **Document decisions**: If you deviate from Zalando, explain why
- **Team alignment**: Share findings with API team
- **Automate checks**: Pre-commit hooks catch issues early

---

**Generated by**: Spectral API Linter v6.15.0  
**Standards**: Zalando RESTful API Guidelines  
**Configuration**: `.spectral.yaml`
