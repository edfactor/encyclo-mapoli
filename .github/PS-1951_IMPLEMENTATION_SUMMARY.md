# PS-1951: Oracle HCM Certificate Authentication - Implementation Complete

## Executive Summary

Successfully implemented comprehensive Oracle HCM certificate-based authentication combining **mTLS (mutual TLS) at the connection layer** and **JWT tokens (RS256) at the request layer**. Added extensive logging infrastructure for debugging authorization failures.

**Status**: ✅ **IMPLEMENTATION COMPLETE** - Ready for testing and code review

---

## What Was Implemented

### 1. Certificate Service (`CertificateService.cs`)
Robust certificate loading with:
- ✅ Three loading methods: file, stream, byte array
- ✅ Thread-safe caching with ReaderWriterLockSlim
- ✅ Double-check pattern for cache efficiency
- ✅ Certificate expiration monitoring (tiered alerts)
- ✅ IDisposable resource cleanup
- ✅ Comprehensive error handling with CRITICAL logging

**Key Methods**:
```csharp
public X509Certificate2 GetCertificate(string pfxFilePath, string? password)
public X509Certificate2 GetCertificateFromStream(Stream pfxStream, string? password, string? sourceIdentifier)
public X509Certificate2 GetCertificateFromBytes(byte[] pfxData, string? password, string? sourceIdentifier)
```

### 2. JWT Token Service (`JwtTokenService.cs` - NEW)
Oracle HCM-compliant JWT generation with RS256 signing:
- ✅ Full JWT token generation with proper header/payload/signature
- ✅ RS256 signing using certificate's private RSA key
- ✅ SHA-1 thumbprint extraction for x5t header claim (Oracle HCM requirement)
- ✅ Certificate issuer extraction from subject CN or L
- ✅ RFC 7515 compliant base64url encoding
- ✅ **Comprehensive DEBUG/INFO logging for troubleshooting** ← Phase 8
- ✅ Configuration externalization (principal, algorithm, expiration)

**Key Methods**:
```csharp
public string GenerateToken(X509Certificate2 certificate, string issuer, 
    string principal, int expirationMinutes)
public string GenerateTokenWithDefaults(X509Certificate2 certificate, 
    int expirationMinutes)
```

**Generated JWT Token Structure**:
```
Header:
  alg: RS256 (RSA with SHA-256)
  typ: JWT
  x5t: <certificate SHA-1 thumbprint, base64url>

Payload:
  iss: <certificate issuer extracted from subject>
  prn: <principal from config, default: "fusion">
  iat: <issued-at timestamp>
  exp: <expiration timestamp>

Signature: RS256_USING_CERTIFICATE_PRIVATE_KEY
```

### 3. mTLS Integration
All 5 HTTP clients configured with certificate-based mutual TLS:
- ✅ AtomFeedClient
- ✅ EmployeeFullSyncClient
- ✅ PayrollSyncClient
- ✅ PayrollSyncService
- ✅ OracleHcmHealthCheck

**Implementation**: Certificates attached to `HttpClientHandler.ClientCertificates` collection for client certificate authentication.

### 4. JWT Injection into Requests
Each HTTP request includes bearer token:
```
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsIng1dCI6IkFHMXZTbDgyY1JBaytwRDlNNGx0eGV1U0FBOCJ9...
```

Token is generated with each request to ensure freshness (configurable expiration, default 10 minutes).

### 5. Configuration Externalization (`OracleHcmConfig.cs`)
Externalized JWT settings:
- ✅ `JwtPrincipal` (default: "fusion") - User principal claim
- ✅ `JwtSigningAlgorithm` (default: "RS256") - Signing algorithm
- ✅ `JwtExpirationMinutes` (default: 10) - Token lifetime in minutes

**Configuration Example** (appsettings.json):
```json
{
  "OracleHcm": {
    "PfxFilePath": "/path/to/certificate.pfx",
    "PfxPassword": "${SECURE_PFX_PASSWORD}",
    "JwtPrincipal": "fusion",
    "JwtSigningAlgorithm": "RS256",
    "JwtExpirationMinutes": 10
  }
}
```

### 6. Comprehensive Logging Infrastructure (Phase 8 - Current)
Added extensive DEBUG/INFO logging for troubleshooting authorization failures:

**JWT Token Service Logs**:
- `[DEBUG] JWT Token Generation - Header: {...}` - Header JSON with alg/typ/x5t
- `[DEBUG] JWT Token Generation - Payload: {...}` - Payload JSON with iss/prn/iat/exp
- `[DEBUG] JWT Token Generation - Algorithm: {algorithm}, Issuer: {issuer}, Principal: {principal}, Expiration: {exp}` - Key details
- `[INFO] JWT Token Generated - Certificate Subject: {...}, Thumbprint: {...}` - Certificate details
- `[DEBUG] JWT Token (first 100 chars): {...}` - Token preview for inspection
- `[DEBUG] Extracted CN from certificate subject: {...}` - Issuer extraction (CN)
- `[DEBUG] Extracted L from certificate subject: {...}` - Fallback issuer extraction (L)
- `[WARNING] Could not extract CN or L from certificate subject...` - Extraction failure warning

**Certificate Service Logs**:
- Expiration monitoring: CRITICAL (< 7 days), ERROR (7-30 days), WARNING (30-90 days)
- Cache operations with certificate identifiers

---

## Testing the Implementation

### 1. Enable Debug Logging
Add to `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

### 2. Run Application
```powershell
cd d:\source\Demoulas\smart-profit-sharing
aspire run
```

### 3. Make API Request
Trigger any endpoint that calls Oracle HCM API. JWT logs will appear in output.

### 4. Inspect Logs
Look for:
```
[DBG] JWT Token Generation - Header: {"alg":"RS256","typ":"JWT","x5t":"..."}
[DBG] JWT Token Generation - Payload: {"iss":"...","prn":"fusion","iat":...,"exp":...}
[INF] JWT Token Generated - Certificate Subject: ...
```

### 5. If Unauthorized (401)
Refer to [JWT_DEBUGGING_GUIDE.md](JWT_DEBUGGING_GUIDE.md) for systematic troubleshooting:
- Verify JWT header has correct algorithm and thumbprint
- Verify JWT payload has correct issuer and principal
- Verify certificate loaded successfully
- Verify issuer matches Oracle HCM expectations
- Check certificate is not expired

---

## Build Status

✅ **CLEAN BUILD - 0 Warnings, 0 Errors**

```
Build succeeded.
0 Warning(s)
0 Error(s)
Time Elapsed 00:00:47.36
```

**Build Configuration**: Release

**Analyzer Notes**:
- CA5350 (Weak cryptography SHA-1) suppressed in `CalculateThumbprint()` method
  - **Justification**: Oracle HCM requires SHA-1 thumbprint in JWT x5t header claim (OpenID Connect standard)
  - This is a security requirement from the Oracle HCM API, not a code deficiency

---

## Git History

Latest commits on `feature/PS-1951-oracle-hcm-certificate-authentication`:

```
9d854a8e3 PS-1951: Add JWT debugging documentation and quick reference guide
d986e0dc4 PS-1951: Add comprehensive JWT debugging and logging for troubleshooting authorization errors
[earlier commits: configuration externalization, JWT implementation, mTLS wiring, certificate refactoring]
```

---

## Files Modified/Created

### New Files
- `src/services/src/Demoulas.ProfitSharing.OracleHcm/Services/JwtTokenService.cs` (165 lines)
- `src/services/src/Demoulas.ProfitSharing.OracleHcm/Services/IJwtTokenService.cs` (Interface)
- `.github/JWT_DEBUGGING_GUIDE.md` (Comprehensive troubleshooting guide)
- `.github/JWT_QUICK_REFERENCE.md` (Quick reference for common issues)

### Modified Files
- `src/services/src/Demoulas.ProfitSharing.OracleHcm/Services/CertificateService.cs` (Refactored to remove artificial async patterns)
- `src/services/src/Demoulas.ProfitSharing.OracleHcm/Config/OracleHcmConfig.cs` (Added JWT properties)
- `src/services/src/Demoulas.ProfitSharing.OracleHcm/OracleHcmExtension.cs` (Wired JWT into HTTP clients)

---

## How It Works (Flow Diagram)

```
1. Application Startup
   ↓
2. Certificate Service Loads Certificate
   ├─ Loads from file: PfxFilePath
   ├─ Extracts private key for signing
   └─ Caches certificate with SHA-1 thumbprint
   ↓
3. HTTP Client Request to Oracle HCM
   ├─ mTLS Layer: Certificate attached to connection
   └─ Request Layer: JWT token generated and injected
   ↓
4. JWT Token Generation
   ├─ Extract certificate issuer from subject (CN or L)
   ├─ Create header: {alg: RS256, typ: JWT, x5t: <thumbprint>}
   ├─ Create payload: {iss: <issuer>, prn: <principal>, iat: <now>, exp: <future>}
   ├─ Sign with RS256: header.payload signed with certificate private key
   ├─ Generate token: base64(header).base64(payload).base64(signature)
   └─ Inject in Authorization: Bearer <token> header
   ↓
5. Oracle HCM Validates Request
   ├─ Verify certificate (mTLS) - validates connection authenticity
   ├─ Verify JWT signature - ensures token wasn't tampered
   ├─ Verify JWT claims - checks issuer, principal, expiration
   └─ Grant/Deny Access
   ↓
6. Request Completes or Fails with 401 Unauthorized
   └─ Debug logs show JWT components if failure
```

---

## Troubleshooting

### Quick Diagnostics
1. **401 Unauthorized**: Check `JWT_DEBUGGING_GUIDE.md` for systematic troubleshooting
2. **Certificate not found**: Verify `PfxFilePath` in appsettings.json
3. **Token expired**: Check `JwtExpirationMinutes` config, verify system clock
4. **Wrong issuer**: Check certificate subject CN/L matches Oracle HCM expectations

### Common Issues
See [JWT_QUICK_REFERENCE.md](.github/JWT_QUICK_REFERENCE.md) for table of common issues and fixes.

### Detailed Troubleshooting
See [JWT_DEBUGGING_GUIDE.md](.github/JWT_DEBUGGING_GUIDE.md) for comprehensive guide including:
- Log message examples
- Debug commands
- JWT token structure verification
- Certificate validation procedures
- curl testing examples

---

## Security Considerations

✅ **Private Key Protection**:
- Certificate private key never logged or exposed
- Private key only used in-memory for signing
- Credentials stored via ASP.NET Secrets Manager, not hardcoded

✅ **JWT Token Security**:
- RS256 signing ensures token authenticity
- Token thumbprint (x5t) ties token to specific certificate
- Token expiration limits window of vulnerability (default 10 minutes)
- New token generated with each request

✅ **Certificate Security**:
- Certificate expiration monitored with CRITICAL alerts (< 7 days)
- Thread-safe caching prevents race conditions
- Supports password-protected PFX files

✅ **Logging Security**:
- Private keys never logged
- JWT tokens shown only as previews (first 100 chars)
- Sensitive configuration from secrets, not source code

---

## Next Steps

### Immediate (Development/Testing)
1. ✅ Implementation complete - build verified
2. 🔄 **Test with Oracle HCM**: Run application, check debug logs for JWT generation
3. 🔄 **Verify authorization**: If 401 errors, use JWT_DEBUGGING_GUIDE.md to diagnose
4. 🔄 **Validate all 5 HTTP clients**: Ensure all Oracle HCM endpoints use JWT correctly

### Pre-Merge
5. ⏳ Code review on GitHub/Bitbucket
6. ⏳ QA testing with production-like certificate
7. ⏳ Security review of logging and PII handling

### Post-Merge
8. ⏳ Update CLAUDE.md with JWT authentication pattern
9. ⏳ Monitor production logs for authorization patterns
10. ⏳ Set up alerts for certificate expiration (< 7 days)

---

## Documentation References

- **JWT Debugging Guide**: [.github/JWT_DEBUGGING_GUIDE.md](.github/JWT_DEBUGGING_GUIDE.md)
- **JWT Quick Reference**: [.github/JWT_QUICK_REFERENCE.md](.github/JWT_QUICK_REFERENCE.md)
- **Oracle HCM JWT Docs**: https://www.oracle.com/webfolder/technetwork/tutorials/obe/fusionapps/HCM/JWT_API_Authentication_OBE/html/index.html
- **RFC 7515 (JWT Signing)**: https://tools.ietf.org/html/rfc7515

---

## Summary

### Phase 1: Certificate Service Refactoring
✅ Removed artificial async wrappers (Task.Run)
✅ Implemented true synchronous API
✅ Added thread-safe caching with ReaderWriterLockSlim
✅ Implemented IDisposable for resource cleanup

### Phase 2: JWT Service Implementation
✅ Created JWT token service with RS256 signing
✅ Implemented certificate issuer extraction
✅ Implemented RFC 7515 compliant base64url encoding
✅ Created IJwtTokenService interface

### Phase 3: mTLS Integration
✅ Attached certificates to all 5 HTTP clients
✅ Configured ClientCertificates for mutual TLS
✅ Verified build success

### Phase 4: JWT Injection
✅ Injected JWT token in Authorization header
✅ Integrated JWT generation into request pipeline
✅ Removed all basic auth code

### Phase 5: Configuration Externalization
✅ Added JWT config properties to OracleHcmConfig
✅ Externalized principal, algorithm, expiration
✅ Made issuer extraction use certificate subject

### Phase 6: Comprehensive Logging (Current)
✅ Added DEBUG/INFO logging to JWT service
✅ Added logging for certificate operations
✅ Added logging for issuer extraction
✅ Created troubleshooting documentation

**Overall Status**: ✅ **COMPLETE - Ready for Testing & Code Review**

---

*Implementation completed on September 2025*
*Feature Branch*: `feature/PS-1951-oracle-hcm-certificate-authentication`
*Latest Commit*: `9d854a8e3` - JWT debugging documentation commit
