# JWT Authentication Quick Reference

## Quick Fixes

### 401 Unauthorized? Follow These Steps:

1. **Enable Debug Logs**
```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

2. **Run Application & Capture Logs**
```powershell
# Run from project root
aspire run

# Make an API call that uses Oracle HCM
# Check logs for JWT generation messages
```

3. **Check Log Output For**:
- [ ] `"JWT Token Generation - Header:"` - ✅ Header generated successfully
- [ ] `"Extracted CN from certificate subject:"` - ✅ Issuer extracted correctly
- [ ] `"Certificate Subject:"` - ✅ Certificate loaded
- [ ] Error or warning messages - ❌ Something wrong

## Configuration

### Default JWT Settings
```json
{
  "OracleHcm": {
    "JwtPrincipal": "fusion",           // User principal - check with Oracle HCM admin
    "JwtSigningAlgorithm": "RS256",     // RSA with SHA-256 (do not change)
    "JwtExpirationMinutes": 10,         // Token lifetime in minutes
    "PfxFilePath": "path/to/cert.pfx",  // Certificate location
    "PfxPassword": "${SECURE_PFX_PASSWORD}" // Use secrets, NOT hardcoded
  }
}
```

### Per-Environment Override
```powershell
# Increase token lifetime for debugging
$env:OracleHcm__JwtExpirationMinutes = 30
aspire run
```

## Common Issues Summary

| Issue | Log Indicator | Fix |
|-------|---------------|-----|
| Wrong Issuer (iss claim) | `"Extracted CN from certificate subject: ???"` doesn't match Oracle HCM config | Update certificate issuer or Oracle HCM config |
| Signature Invalid | Token generated but 401 from Oracle HCM | Verify certificate private key present, re-upload public cert to Oracle HCM |
| Token Expired | Working then failing | Increase `JwtExpirationMinutes` or verify system clock sync |
| Wrong Principal (prn claim) | 401 "access denied" | Update `JwtPrincipal` config or verify user in Oracle HCM |
| Certificate Not Found | ERROR logs during startup | Check `PfxFilePath` and verify file exists |
| Certificate Expired | CRITICAL/ERROR logs | Renew certificate, update file, redeploy |

## JWT Token Structure

```
Header (Base64Url Encoded):
{
  "alg": "RS256",           ← Algorithm (RSA with SHA-256)
  "typ": "JWT",             ← Token type
  "x5t": "AG1vSl82cRAk+..." ← Certificate thumbprint (SHA-1, base64url)
}

Payload (Base64Url Encoded):
{
  "iss": "Demoulas Cloud Issuing CA 2",  ← Extracted from certificate subject
  "prn": "fusion",                        ← From config JwtPrincipal
  "iat": 1699270154,                      ← Issued at (Unix timestamp)
  "exp": 1699270754                       ← Expiration (Unix timestamp)
}

Signature (Base64Url Encoded):
RS256_SIGNATURE_USING_CERTIFICATE_PRIVATE_KEY
```

## Debug Commands

### View Certificate Details
```bash
# Show everything
openssl x509 -in cert.pem -text -noout

# Get SHA-1 thumbprint (x5t value)
openssl x509 -in cert.pem -noout -fingerprint -sha1

# Check if private key exists
openssl pkey -in cert.key -check

# For PFX files
openssl pkcs12 -in cert.pfx -text -noout -passin pass:PASSWORD
```

### Decode JWT Token (Online)
1. Copy JWT from logs (or full token if complete available)
2. Go to https://jwt.io/
3. Paste token in left panel
4. Verify header and payload match expectations

### Test API Call with curl
```bash
JWT_TOKEN="eyJhbGc..." # From logs
curl -v \
  --cert cert.pem \
  --key key.pem \
  -H "Authorization: Bearer $JWT_TOKEN" \
  https://your-oracle-hcm-url/rest/api/endpoint
```

## When to Check Each Component

**JWT Generation Issues** → Check `GenerateToken()` logs for header/payload/algorithm
**Certificate Issues** → Check `ValidateAndLogCertificate()` logs for expiration/validity  
**Issuer Issues** → Check `ExtractIssuerFromCertificate()` logs for CN/L extraction
**Signature Issues** → Check private key present and certificate matches

## Files Reference

- **JwtTokenService.cs** - Generates and signs JWT tokens (has all logging)
- **CertificateService.cs** - Loads and caches certificates
- **OracleHcmConfig.cs** - Configuration (JwtPrincipal, JwtExpirationMinutes, etc.)
- **OracleHcmExtension.cs** - Wires everything together, injects JWT into requests

---

For detailed troubleshooting, see [JWT_DEBUGGING_GUIDE.md](JWT_DEBUGGING_GUIDE.md)
