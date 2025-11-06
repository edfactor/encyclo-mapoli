# JWT Authentication Debugging Guide

## Overview
When encountering 401 Unauthorized errors with Oracle HCM API calls, use this guide to diagnose the issue systematically.

## Logging Setup

### Enable Debug Logging
Add to `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

### Key Log Messages to Look For

**JWT Generation**:
- `"JWT Token Generation - Header: ..."` - Decoded JWT header (alg, typ, x5t)
- `"JWT Token Generation - Payload: ..."` - Decoded JWT payload (iss, prn, iat, exp)
- `"JWT Token Generated - Certificate Subject: ..."` - Certificate details
- `"JWT Token (first 100 chars): ..."` - Generated token preview

**Issuer Extraction**:
- `"Extracted CN from certificate subject: ..."` - Common Name from certificate
- `"Extracted L (Locality) from certificate subject: ..."` - Locality from certificate
- `"Could not extract CN or L from certificate subject..."` - Warning if extraction failed

**Certificate Loading**:
- `"Retrieved certificate from cache: ..."` - Certificate loaded from cache
- `"Certificate loaded and cached: ..."` - New certificate loaded and cached
- Check `CertificateService` logs for certificate expiration warnings/errors

## Common Issues & Solutions

### 1. Invalid JWT Issuer (iss claim)
**Symptoms**: 401 Unauthorized from Oracle HCM
**Check**:
1. Look for `"Extracted CN from certificate subject"` log message
2. Verify the extracted value matches what Oracle HCM expects
3. Compare with what's configured in Oracle HCM security console

**Debug Example**:
```
Extracted CN from certificate subject: "Demoulas Cloud Issuing CA 2"
```

**Solution**:
- If extraction is wrong, manually check certificate: `openssl x509 -in cert.pem -text -noout`
- If CN/L don't match Oracle HCM config, update Oracle HCM's trusted issuer

### 2. Invalid JWT Signature
**Symptoms**: 401 Unauthorized, OAuth2 signature verification failed
**Check**:
1. Verify certificate has private key: Look for "Certificate loaded successfully" log
2. Verify RSA algorithm matches: Check `"Algorithm: RS256"` in logs
3. Verify certificate thumbprint (x5t): Compare with Oracle HCM uploaded certificate

**Debug Steps**:
```powershell
# Check certificate details
openssl x509 -in cert.pem -text -noout

# Verify certificate has private key
openssl pkey -in cert.key -check

# Get SHA-1 thumbprint (used as x5t)
openssl x509 -in cert.pem -noout -fingerprint -sha1
```

**Solution**:
- Ensure certificate and private key are in sync
- Re-upload public certificate to Oracle HCM
- Verify certificate is not expired

### 3. Token Expiration (exp claim)
**Symptoms**: 401 Unauthorized after delay, recurring auth failures
**Check**:
1. Look for `"JWT Token Generation - Payload: ..."` log
2. Extract `exp` value (Unix timestamp)
3. Compare with current time

**Debug Example**:
```bash
# Check current Unix timestamp
date +%s

# Compare with exp value in logs
# If exp < current time, token has expired
```

**Solution**:
- Tokens default to 10-minute expiration via `JwtExpirationMinutes` config
- Verify system clock is synchronized (NTP)
- If tokens are expiring too fast, increase `JwtExpirationMinutes` in `appsettings.json`:
```json
{
  "OracleHcm": {
    "JwtExpirationMinutes": 30
  }
}
```

### 4. Missing or Wrong Principal (prn claim)
**Symptoms**: 401 Unauthorized with "access denied" or "insufficient privileges"
**Check**:
1. Look for `"JWT Token Generation - Payload: ..."` log
2. Extract `prn` value (should match Oracle HCM user)
3. Verify user has required Oracle HCM privileges

**IMPORTANT**: The `prn` value is **NOT** found in your certificate. You must ask your Oracle HCM administrator:
- "Which user should we use for JWT authentication?"
- "Which user has the 'Fusion Applications Integration' privilege?"

**Solution**:
- The `JwtPrincipal` value must be configured with the actual Oracle HCM username your admin provides
- The example "fusion" is just a common default - your environment may use a different user
- Configure via `JwtPrincipal` in `appsettings.json`:
```json
{
  "OracleHcm": {
    "JwtPrincipal": "your-actual-oracle-hcm-username"
  }
}
```
- Verify user exists in Oracle HCM with required privileges
- Check Oracle HCM security console for user role assignments
- **Before production**: Confirm with Oracle HCM admin that this is the correct user

### 5. Certificate Not Found or Expired
**Symptoms**: CRITICAL log entries, certificate file issues
**Check**:
1. Look for `CertificateService` logs
2. Verify file path in `PfxFilePath` config
3. Check certificate expiration dates

**Solution**:
- Verify PFX file exists at configured path
- Update expired certificate and redeploy
- Check certificate validity:
```bash
openssl pkcs12 -in cert.pfx -text -noout -passin pass:YOUR_PASSWORD
```

## Debugging Checklist

### Before Testing
- [ ] JWT logging enabled (Debug level in appsettings.json)
- [ ] Certificate file exists at configured path
- [ ] Certificate has private key
- [ ] Certificate is not expired
- [ ] Public certificate uploaded to Oracle HCM

### During Request
- [ ] Check console/log output for JWT generation logs
- [ ] Verify all JWT components (header, payload, signature)
- [ ] Check certificate subject extraction logs
- [ ] Verify issuer matches Oracle HCM config

### If Still Failing
1. **Extract the JWT token from logs** (first 100 chars shown)
2. **Decode at jwt.io** (use public part only):
   - Paste full token in left panel
   - Verify header: `alg: RS256`, `typ: JWT`, `x5t: <base64>`
   - Verify payload: `iss`, `prn`, `iat`, `exp`

3. **Verify certificate chain**:
   ```bash
   openssl x509 -in cert.pem -noout -text
   ```

4. **Test with curl** (for API testing):
   ```bash
   # Get JWT token from logs
   JWT_TOKEN="eyJhbGc..."
   
   # Test Oracle HCM API
   curl -v \
     --cert cert.pem \
     --key key.pem \
     -H "Authorization: Bearer $JWT_TOKEN" \
     https://your-oracle-hcm-url/rest/api/...
   ```

## Log Examples

### Successful JWT Generation
```
[DBG] JWT Token Generation - Header: {"alg":"RS256","typ":"JWT","x5t":"AG1vSl82cRAk+PD9M4ltxeuSAA8="}
[DBG] JWT Token Generation - Payload: {"iss":"Demoulas Cloud Issuing CA 2","prn":"fusion","iat":1699270154,"exp":1699270754}
[DBG] JWT Token Generation - Algorithm: RS256, Issuer: Demoulas Cloud Issuing CA 2, Principal: fusion, Expiration (Unix): 1699270754
[INF] JWT Token Generated - Certificate Subject: CN = Demoulas Cloud Issuing CA 2, L = Demoulas Supermarkets Inc., O = Demoulas Inc., C = US, Thumbprint: D8E8F7B8E8F7B8E8F7B8E8F7B8E8F7B8E8F7B8E8
[DBG] JWT Token (first 100 chars): eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsIng1dCI6IkFHMXZTbDgyY1JBaytwRDlNNGx0eGV1U0FBOCJ9...
```

### Successful Certificate Extraction
```
[DBG] Extracted CN from certificate subject: "Demoulas Cloud Issuing CA 2"
```

### Certificate Expiration Warning
```
[ERR] Certificate will expire in 29 days (2025-12-06). Immediate renewal required.
[WRN] Certificate will expire in 87 days (2026-01-04). Plan renewal soon.
```

## Support Resources

- Oracle HCM JWT Documentation: https://www.oracle.com/webfolder/technetwork/tutorials/obe/fusionapps/HCM/JWT_API_Authentication_OBE/html/index.html
- JWT Token Decoder: https://jwt.io/
- OpenSSL Documentation: https://www.openssl.org/docs/
- Certificate Service: `CertificateService.cs` - Handles certificate loading and caching
- JWT Service: `JwtTokenService.cs` - Generates and signs JWT tokens
