# 401 Unauthorized - Quick Debug Card

**Copy-paste this checklist to debug your 401 error:**

## What You're Fixing

Three JWT claims cause 401 errors. All are now fixed or documented:

| Claim | What Was Wrong | How It's Fixed | Status |
|-------|----------------|----------------|--------|
| `iss` | Used `certificate.Subject` (who cert was issued TO) | Now uses `certificate.Issuer` (who ISSUED it) | ✅ FIXED |
| `alg` | Hardcoded "RS256" regardless of certificate | Extracts from certificate signature algorithm | ✅ FIXED |
| `prn` | Unclear source | Now documented: must be Oracle HCM user from admin | ✅ DOCUMENTED |

---

## Quick Fix Steps (5 minutes)

### 1️⃣ Verify Your appsettings Configuration

```json
{
  "OracleHcm": {
    "BaseAddress": "https://your-oracle-instance.oracle.com",
    "PfxFilePath": "C:\\path\\to\\certificate.pfx",
    "PfxPassword": "password-if-needed",
    "JwtPrincipal": "fusion",              // ⚠️ Ask admin - may be different
    "JwtExpirationMinutes": 10
  }
}
```

**Critical**: `JwtPrincipal` must be the **actual Oracle HCM username** with "Fusion Applications Integration" privilege. Default "fusion" may not match your environment.

### 2️⃣ Enable DEBUG Logging

In `appsettings.Development.json` (or your environment's appsettings):
```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

### 3️⃣ Run and Check Logs

```powershell
aspire run
```

**Look for these lines** (should see all of them):

```
[INF] Certificate signature algorithm detected: sha256RSA
[DBG] Extracted CN from certificate issuer: Demoulas Cloud Issuing CA 2
[DBG] JWT Token Generation - Header: {"alg":"RS256","typ":"JWT","x5t":"..."}
[DBG] JWT Token Generation - Payload: {"iss":"...","prn":"fusion","iat":...,"exp":...}
[DBG] JWT Token (first 100 chars): eyJ...
```

### 4️⃣ Make Test Request

```
GET https://localhost:7141/api/oracle-hcm/employees
```

### 5️⃣ If Still 401, Verify Claims Match

Look at JWT payload in logs:
- ✅ `iss` = Certificate's issuer (from log "Extracted CN from certificate issuer")
- ✅ `prn` = Oracle HCM username (from your config)
- ✅ `alg` = RS256/RS384/RS512 (depends on certificate)

---

## Most Common Causes (In Order)

1. **`JwtPrincipal` is wrong** → Ask Oracle HCM admin for exact username
2. **Certificate not found** → Verify `PfxFilePath` exists
3. **Certificate expired** → Check certificate validity dates
4. **Certificate has no private key** → Verify certificate is PKCS#12 (.pfx) format
5. **`iss` claim doesn't match** → Verify certificate issuer matches Oracle HCM's registry

---

## Debug Output Examples

### ✅ Good: What You Should See

```
[INF] Certificate signature algorithm detected: sha256RSA
[INF] Using JWT algorithm: RS256 (from certificate algorithm: sha256RSA)
[DBG] Extracted CN from certificate issuer: Demoulas Cloud Issuing CA 2
[DBG] JWT Token Generation - Payload: {"iss":"Demoulas Cloud Issuing CA 2","prn":"fusion",...}
[INF] JWT Token Generated - Certificate Subject: CN=your-cn, Thumbprint: abc123def456
```

### ❌ Bad: Common Error Patterns

**"Unable to determine certificate signature algorithm"**
- Fix: Certificate corrupted or not valid. Re-import certificate file.

**"Certificate does not contain a private key"**
- Fix: Wrong certificate format. Must be PKCS#12 (.pfx), not PEM (.pem) or DER (.cer).

**"Could not extract CN or L from certificate issuer"**
- Fix: Certificate issuer is malformed. Check certificate properties with Certificate Manager.

**"prn value is missing or empty"**
- Fix: `JwtPrincipal` not set in appsettings. Add valid Oracle HCM username.

---

## JWT Token Verification (Development Only)

Copy JWT from logs (the `eyJ...` token) and paste into https://jwt.io to verify:

```json
{
  "header": {
    "alg": "RS256",           // Must match certificate algorithm
    "typ": "JWT",
    "x5t": "..."              // Certificate thumbprint
  },
  "payload": {
    "iss": "...",             // Must be certificate issuer
    "prn": "fusion",          // Must be Oracle HCM user
    "iat": 1725449390,
    "exp": 1725450290
  }
}
```

---

## Certificate Check Checklist

- [ ] Certificate file exists: `Test-Path "C:\path\to\cert.pfx"` returns `$true`
- [ ] Certificate not expired: Open Certificate Manager, check Valid To date
- [ ] Certificate has private key: In Certificate Manager, key icon visible
- [ ] Key size 2048+ bits: Right-click → Properties → Details → Public Key
- [ ] Correct password: Password manager has correct `.pfx` password

---

## When to Ask Oracle HCM Admin

Ask for these specific items:
1. **Exact username (principal)** for the "Fusion Applications Integration" privilege user
2. **Certificate thumbprint** they have registered (should match yours)
3. **Expected issuer value** the JWT should have (should match certificate issuer)

---

## Files Modified in This Fix

- ✅ `JwtTokenService.cs` - Fixed `iss` extraction, added `alg` extraction
- ✅ `OracleHcmConfig.cs` - Clarified `prn` requirement, documented configuration
- ✅ `OracleHcmExtension.cs` - Already wires JWT token correctly

All builds clean: **0 Warnings, 0 Errors**

Latest commits:
```
a52d0fb22 PS-1951: Mark JwtSigningAlgorithm config property as deprecated
f7b5b4fc1 PS-1951: Extract JWT signing algorithm from certificate
826419cc0 PS-1951: Fix JWT issuer extraction to use certificate Issuer field
```

---

## Next Steps

1. **Update `appsettings` with correct `JwtPrincipal`** (ask admin)
2. **Enable DEBUG logging** (copy config above)
3. **Run `aspire run` and check logs** (should see all 5 lines above)
4. **Make test request** (should get 200 or different error, not 401)
5. **If still 401**, share the debug logs with full JWT token payload

