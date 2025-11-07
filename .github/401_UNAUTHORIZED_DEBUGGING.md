# Debugging 401 Unauthorized from Oracle HCM API

## Overview

When debugging a 401 Unauthorized response from Oracle HCM, there are typically **three JWT claim issues** that cause authentication failures. This guide provides systematic steps to identify and fix the root cause.

## Quick Reference: The Three Critical JWT Claims

| Claim | Source | Fixed In | Notes |
|-------|--------|----------|-------|
| `iss` (Issuer) | `certificate.Issuer` field (who ISSUED the cert) | ✅ Commit `826419cc0` | Was using `certificate.Subject` (wrong) |
| `alg` (Algorithm) | Certificate signature algorithm (sha256RSA → RS256) | ✅ Commit `f7b5b4fc1` | Was hardcoded to "RS256" (potentially wrong) |
| `prn` (Principal) | Oracle HCM admin provided username | ✅ Documented | NOT in certificate - requires `appsettings.json` config |

---

## Step 1: Verify Configuration (User Action Required)

### Check appsettings.json or Secrets

Your `OracleHcm` configuration must include:

```json
{
  "OracleHcm": {
    "BaseAddress": "https://your-oracle-hcm-instance.oracle.com",
    "DemographicUrl": "/hcmRestApi/resources/11.13.18.05/workers",
    "UseCertificateAuthentication": true,
    "PfxFilePath": "/path/to/your/certificate.pfx",
    "PfxPassword": "your-certificate-password",
    "JwtPrincipal": "fusion",           // ⚠️ CRITICAL: Verify with Oracle HCM admin
    "JwtExpirationMinutes": 10
  }
}
```

### Critical Configuration Checks

```powershell
# 1. Verify certificate path exists and is accessible
Test-Path "C:\path\to\certificate.pfx"  # Should return $true

# 2. Verify certificate file has private key (required for JWT signing)
# Open Certificate Manager (certmgr.msc) and verify:
# - Certificate has a private key
# - Key size is 2048 or 4096 bits
# - CN (Common Name) is set correctly
# - Not expired

# 3. Verify JwtPrincipal matches actual Oracle HCM user
# Ask your Oracle HCM administrator for the exact username
# Default "fusion" may not match your environment
```

---

## Step 2: Enable Debug Logging

### Update Logging Configuration

Set `Demoulas.ProfitSharing.OracleHcm` to `Debug` level:

**In appsettings.Development.json** (already set):
```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

**For other environments**, update your environment's `appsettings.{Environment}.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

### Start the Application

```powershell
# From project root
aspire run
```

---

## Step 3: Make a Test Request and Capture Logs

### Example: Query Employees Endpoint

```csharp
// Via API or test client
GET https://localhost:7141/api/oracle-hcm/employees

// Monitor console/log output for:
// [INF] Certificate signature algorithm detected: sha256RSA
// [DBG] Extracted CN from certificate issuer: ...
// [DBG] JWT Token Generation - Header: ...
// [DBG] JWT Token Generation - Payload: ...
// [DBG] JWT Token (first 100 chars): eyJ...
```

### What to Look For in Logs

#### 1. Certificate Loading (Should See This)
```
[INF] Certificate loaded successfully from: C:\path\to\certificate.pfx
[INF] Certificate Subject: CN=YourCN, ...
[INF] Certificate Issuer: CN=Demoulas Cloud Issuing CA 2, ...
[INF] Certificate expires: 2025-12-31
```

#### 2. Algorithm Detection (Should See This)
```
[DBG] Certificate signature algorithm detected: sha256RSA
[INF] Using JWT algorithm: RS256 (from certificate algorithm: sha256RSA)
```

#### 3. Issuer Extraction (Should See This)
```
[DBG] Extracted CN from certificate issuer: Demoulas Cloud Issuing CA 2
```

#### 4. JWT Token Generation (Should See This)
```
[DBG] JWT Token Generation - Header: {"alg":"RS256","typ":"JWT","x5t":"aBc..."}
[DBG] JWT Token Generation - Payload: {"iss":"Demoulas Cloud Issuing CA 2","prn":"fusion","iat":1234567890,"exp":1234567890}
[INF] JWT Token Generated - Certificate Subject: CN=YourCN, Thumbprint: abc123...
```

#### 5. HTTP Request (Should See Debug Output)
```
[DBG] Sending request to Oracle HCM: GET https://oracle.com/hcmRestApi/resources/11.13.18.05/workers
[DBG] Authorization header: Bearer eyJhbGci...
```

---

## Step 4: Validate JWT Token Claims

### Option A: Inspect JWT at jwt.io (Unsafe - Use for Dev Only)

1. Copy the JWT token from logs (the `eyJ...` value)
2. Go to https://jwt.io (development only - never use production data here)
3. Paste token in "Encoded" field
4. Verify the decoded claims:

```json
{
  "header": {
    "alg": "RS256",              // ✅ Must be RS256 (or RS384/RS512)
    "typ": "JWT",
    "x5t": "aBcDeF..."           // ✅ Must be certificate SHA-1 thumbprint
  },
  "payload": {
    "iss": "Demoulas Cloud...",  // ✅ Must be certificate ISSUER (not subject)
    "prn": "fusion",              // ✅ Must match Oracle HCM admin's principal
    "iat": 1725449390,
    "exp": 1725450290             // ✅ Must be future timestamp
  }
}
```

### Option B: Programmatic Validation (Secure)

```csharp
// Create a unit test or console app to verify JWT claims
using System.IdentityModel.Tokens.Jwt;

string jwtToken = "eyJ..."; // From logs
var handler = new JwtSecurityTokenHandler();
var jwtSecurityToken = handler.ReadJwtToken(jwtToken);

Console.WriteLine($"Algorithm: {jwtSecurityToken.Header["alg"]}");
Console.WriteLine($"Issuer: {jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value}");
Console.WriteLine($"Principal: {jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "prn")?.Value}");
Console.WriteLine($"Issued: {jwtSecurityToken.IssuedAt}");
Console.WriteLine($"Expires: {jwtSecurityToken.ValidTo}");
```

---

## Step 5: Common Issues and Fixes

### Issue 1: ❌ `"prn" value is wrong or missing`

**Symptom**: JWT payload shows `"prn":"fusion"` but Oracle HCM rejects it

**Cause**: Your Oracle HCM environment uses a different principal username

**Fix**: 
1. Contact your Oracle HCM administrator and ask for the **exact username** with "Fusion Applications Integration" privilege
2. Update `appsettings.json`:
```json
{
  "OracleHcm": {
    "JwtPrincipal": "actual-oracle-user"  // Replace "fusion" with correct name
  }
}
```

### Issue 2: ❌ `"iss" value doesn't match Oracle HCM's issuer registry`

**Symptom**: JWT shows `"iss":"YourCompany Inc"` but Oracle HCM expects `"iss":"Demoulas Cloud Issuing CA 2"`

**Cause**: Certificate issuer (who signed it) doesn't match what Oracle HCM expects

**Fix**:
1. Check the **log output** for the extracted issuer:
```
[DBG] Extracted CN from certificate issuer: Demoulas Cloud Issuing CA 2
```
2. Verify this matches Oracle HCM's expected issuer (check with admin)
3. If wrong, you may need a different certificate

### Issue 3: ❌ `"alg" is not RS256`

**Symptom**: JWT shows `"alg":"RS256"` but certificate uses SHA-384

**Cause**: Certificate and JWT algorithm mismatch

**Fix**:
1. Check the **log output** for detected algorithm:
```
[DBG] Certificate signature algorithm detected: sha384RSA
[INF] Using JWT algorithm: RS384
```
2. If algorithm was wrong (before fix), rebuild to pick up latest code with algorithm extraction
3. Verify the correct `alg` appears in JWT token

### Issue 4: ❌ Certificate thumbprint (`x5t`) mismatch

**Symptom**: Oracle HCM rejects JWT with "invalid thumbprint"

**Cause**: Certificate thumbprint doesn't match what Oracle HCM has registered

**Fix**:
1. Check certificate thumbprint in logs:
```
[INF] JWT Token Generated - Certificate Subject: CN=YourCN, Thumbprint: abc123...
```
2. Also check JWT `x5t` claim (should be same in base64url format)
3. Verify with Oracle HCM admin that they have this certificate's thumbprint registered

### Issue 5: ❌ Certificate expired or not-yet-valid

**Symptom**: TLS handshake fails or certificate validation errors

**Cause**: Certificate not in valid time range

**Fix**:
1. Check certificate expiration in logs:
```
[INF] Certificate expires: 2025-12-31
[ERR] Certificate will expire in 5 days
```
2. Obtain a new certificate if expired
3. If not-yet-valid, check system date/time

---

## Step 6: Test mTLS Connection (Optional)

### Verify Certificate-Based TLS Handshake

```powershell
# Test mTLS connection using OpenSSL or similar
openssl s_client -connect oracle.hcm.example.com:443 `
  -cert C:\path\to\certificate.pfx `
  -key C:\path\to\certificate.pfx `
  -passin pass:certificate-password

# Look for: "Verify return code: 0 (ok)" indicating successful handshake
```

---

## Step 7: Verify All Three Fixes Applied

Make sure you're running the latest code with all three fixes:

```powershell
# Check git commits
git log --oneline | head -10

# Should see:
# a52d0fb22 PS-1951: Mark JwtSigningAlgorithm config property as deprecated
# f7b5b4fc1 PS-1951: Extract JWT signing algorithm from certificate
# 826419cc0 PS-1951: Fix JWT issuer extraction to use certificate Issuer field
```

If you don't see these commits, pull the latest code:
```powershell
git pull origin develop
# or
git checkout feature/PS-1951-oracle-hcm-certificate-authentication
```

---

## Step 8: If Still 401 Unauthorized

### Capture Full Debug Output

1. Set logging to DEBUG level (done above)
2. Redirect logs to a file for analysis:
```powershell
# In PowerShell
aspire run > oracle-hcm-debug.log 2>&1
```

3. Make test request and wait for failure

4. Share relevant log lines:
```
- Certificate loading and validation
- Algorithm extraction
- Issuer extraction
- JWT token generation (header, payload, first 100 chars of token)
- HTTP request being sent
- Response from Oracle HCM (including any error details)
```

### Key Information to Collect

- [ ] Certificate thumbprint (from logs)
- [ ] Certificate issuer CN (from logs)
- [ ] JWT algorithm (from logs)
- [ ] JWT `iss` claim value (from logs)
- [ ] JWT `prn` claim value (from configuration)
- [ ] Certificate expiration date (from certificate manager)
- [ ] Oracle HCM admin's expected `prn` value (ask admin)
- [ ] Oracle HCM admin's registered certificate thumbprint (ask admin)

---

## Troubleshooting Checklist

- [ ] **Configuration Correct**
  - [ ] `PfxFilePath` points to valid certificate file
  - [ ] `PfxPassword` is correct (if certificate is password-protected)
  - [ ] `JwtPrincipal` matches actual Oracle HCM user (verified with admin)
  - [ ] `BaseAddress` is correct Oracle HCM instance URL

- [ ] **Certificate Valid**
  - [ ] Certificate exists and is readable
  - [ ] Certificate has private key (required for signing)
  - [ ] Certificate is not expired
  - [ ] Certificate key size is 2048+ bits
  - [ ] Certificate issuer matches Oracle HCM's registry

- [ ] **JWT Claims Correct** (Verify in Logs)
  - [ ] `alg` = RS256/RS384/RS512 (from certificate algorithm)
  - [ ] `iss` = certificate's issuer CN (from `certificate.Issuer` field)
  - [ ] `prn` = Oracle HCM principal username (from config)
  - [ ] `x5t` = certificate thumbprint (base64url SHA-1)
  - [ ] `iat` and `exp` are valid Unix timestamps

- [ ] **Code Latest**
  - [ ] Latest commits pulled (`826419cc0`, `f7b5b4fc1`, `a52d0fb22`)
  - [ ] Build succeeded with no errors
  - [ ] DEBUG logging enabled in configuration

- [ ] **mTLS Connection**
  - [ ] Certificate is properly loaded by `OracleHcmCertificateService`
  - [ ] HttpClientHandler has certificate in `ClientCertificates` collection
  - [ ] TLS handshake succeeds (test with openssl)

---

## Support

For additional help:
- Check the logs under `logs/` directory
- Review JWT token using jwt.io (development only)
- Contact Oracle HCM admin to verify:
  - Expected `prn` (principal) value
  - Registered certificate thumbprint
  - Expected issuer value

---

## References

- [TELEMETRY_QUICK_REFERENCE.md](./TELEMETRY_QUICK_REFERENCE.md) - Logging patterns
- [JWT Token Service Implementation](../src/services/src/Demoulas.ProfitSharing.OracleHcm/Services/JwtTokenService.cs)
- [OracleHcm Configuration](../src/services/src/Demoulas.ProfitSharing.OracleHcm/Configuration/OracleHcmConfig.cs)
- [Oracle HCM REST API Authentication Documentation](https://docs.oracle.com/en/cloud/saas/human-capital-management/22a/facua/chp_security.htm)

