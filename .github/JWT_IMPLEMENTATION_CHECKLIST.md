# JWT Authentication Implementation Checklist

Use this checklist to verify the Oracle HCM JWT authentication feature is properly set up and tested.

## Pre-Testing Checklist

### Configuration
- [ ] Certificate PFX file exists at path specified in `OracleHcm:PfxFilePath`
- [ ] Certificate password is correctly stored in secrets (`secrets.json` or env var)
- [ ] Certificate has private key (not just public certificate)
- [ ] Certificate is not expired (check start date and expiration date)
- [ ] Public certificate is uploaded to Oracle HCM security configuration

### Code Integration
- [ ] `CertificateService` is registered in DI container
- [ ] `JwtTokenService` is registered in DI container with `OracleHcmConfig` dependency
- [ ] `ILogger<JwtTokenService>` can be injected (logging infrastructure in place)
- [ ] All 5 HTTP clients configured via `ConfigureHttpClients()`:
  - [ ] AtomFeedClient
  - [ ] EmployeeFullSyncClient
  - [ ] PayrollSyncClient
  - [ ] PayrollSyncService
  - [ ] OracleHcmHealthCheck

### Build Verification
- [ ] `dotnet build` completes successfully (0 warnings, 0 errors)
- [ ] No CA5350 analyzer errors (SHA-1 suppression applied for thumbprint)
- [ ] No compilation errors or warnings in certificate/JWT related code

## Testing Checklist

### Local Development Testing

#### Step 1: Enable Debug Logging
- [ ] Add to `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm": "Debug"
    }
  }
}
```

#### Step 2: Run Application
- [ ] Execute `aspire run` from project root
- [ ] Verify application starts without errors
- [ ] Check for any CRITICAL certificate logs (expiration issues)

#### Step 3: Trigger Oracle HCM API Call
- [ ] Make HTTP request to endpoint that calls Oracle HCM API
- [ ] Examples:
  - [ ] Get employee information (uses EmployeeFullSyncClient)
  - [ ] Sync payroll data (uses PayrollSyncService)
  - [ ] Check Oracle HCM health (uses OracleHcmHealthCheck)

#### Step 4: Verify JWT Generation Logs
- [ ] Look for `"JWT Token Generation - Header:"` log entry
  ```
  [DBG] JWT Token Generation - Header: {"alg":"RS256","typ":"JWT","x5t":"..."}
  ```
  - [ ] Contains `"alg":"RS256"` (algorithm)
  - [ ] Contains `"typ":"JWT"` (token type)
  - [ ] Contains `"x5t":"..."` (certificate thumbprint)

- [ ] Look for `"JWT Token Generation - Payload:"` log entry
  ```
  [DBG] JWT Token Generation - Payload: {"iss":"...","prn":"fusion","iat":...,"exp":...}
  ```
  - [ ] Contains `"iss":"..."` (issuer from certificate)
  - [ ] Contains `"prn":"fusion"` (principal from config)
  - [ ] Contains `"iat":...` (issued-at timestamp)
  - [ ] Contains `"exp":...` (expiration timestamp)

- [ ] Look for certificate details log
  ```
  [INF] JWT Token Generated - Certificate Subject: ..., Thumbprint: ...
  ```
  - [ ] Shows certificate subject CN/L values
  - [ ] Shows SHA-1 thumbprint

- [ ] Look for issuer extraction logs
  ```
  [DBG] Extracted CN from certificate subject: "..."
  ```
  - [ ] Shows extracted issuer from certificate
  - [ ] Matches what Oracle HCM expects

#### Step 5: Check API Response
- [ ] API returns success (200 OK) - **Indicates JWT is valid**
- [ ] API returns 401 Unauthorized - **Need to troubleshoot (see debugging guide)**
- [ ] API returns 500 Internal Server Error - **Check server logs for certificate/key issues**

### Troubleshooting Tests (If Unauthorized)

#### Issue: 401 Unauthorized Error
Follow these diagnostic steps:

- [ ] **Step 1: Verify JWT Header**
  - Look for log: `"JWT Token Generation - Header: ..."`
  - Verify x5t (thumbprint) is present
  - Compare x5t with expected certificate thumbprint

- [ ] **Step 2: Verify JWT Payload**
  - Look for log: `"JWT Token Generation - Payload: ..."`
  - Verify issuer (iss) matches Oracle HCM configuration
  - Verify principal (prn) is correct (default "fusion")
  - Verify expiration (exp) is in the future

- [ ] **Step 3: Verify Certificate**
  - Look for log: `"JWT Token Generated - Certificate Subject: ..."`
  - Verify certificate subject CN/L matches Oracle HCM's trusted issuer
  - Verify thumbprint matches uploaded certificate in Oracle HCM

- [ ] **Step 4: Check Issuer Extraction**
  - Look for logs: `"Extracted CN from certificate subject: ..."`
  - If not found, check for WARNING: `"Could not extract CN or L..."`
  - Verify extraction matches certificate subject format

- [ ] **Step 5: Verify Certificate Has Private Key**
  - Inspect certificate in keystore manager
  - Or test: `openssl pkey -in cert.key -check` (if separate key file)
  - Private key is essential for RS256 signing

#### Issue: Token Expiration
- [ ] Check log: `"...exp":...}` in payload
- [ ] Convert Unix timestamp to readable date: https://www.epochconverter.com/
- [ ] Compare with current time
- [ ] If expired, increase `JwtExpirationMinutes` in config (default 10, try 30)

#### Issue: Certificate Not Found
- [ ] Check `CertificateService` logs for CRITICAL entries
- [ ] Verify file path in `OracleHcm:PfxFilePath` is correct
- [ ] Check that file exists in specified location
- [ ] Check file permissions (readable by application user)
- [ ] Verify PFX file is not corrupted

#### Issue: Certificate Expired
- [ ] Check logs for `[ERR] Certificate will expire in...` or `[CRITICAL] Certificate...`
- [ ] Verify certificate dates: `openssl x509 -in cert.pem -noout -dates`
- [ ] Update certificate file and redeploy
- [ ] Ensure new certificate is uploaded to Oracle HCM

### Performance Testing

- [ ] **JWT Generation Performance**
  - [ ] JWT is generated fresh with each request (not cached)
  - [ ] Generation should be < 10ms per request
  - [ ] No noticeable performance impact vs basic auth

- [ ] **Certificate Caching**
  - [ ] Certificate is loaded once and cached (not reloaded per request)
  - [ ] Verify via logs: `"Retrieved certificate from cache"` appears frequently
  - [ ] Very rare `"loaded and cached"` entries (only at startup)

- [ ] **Concurrent Requests**
  - [ ] Multiple concurrent requests use thread-safe cache
  - [ ] No race conditions or contention
  - [ ] All requests generate valid JWTs simultaneously

### Security Testing

- [ ] **No Private Key in Logs**
  - [ ] Search logs for "PRIVATE KEY" or "BEGIN RSA" - should find nothing
  - [ ] Private key never appears in debug output

- [ ] **Token Not Fully Exposed in Logs**
  - [ ] Look for `"JWT Token (first 100 chars): eyJhbGc..."`
  - [ ] Only first 100 chars shown (not full token)
  - [ ] Full token never logged (would be security risk)

- [ ] **Configuration Not Hardcoded**
  - [ ] Certificate path from config (not hardcoded)
  - [ ] Certificate password from secrets (not hardcoded)
  - [ ] JWT principal from config (not hardcoded)

### Integration Testing

- [ ] **Multiple HTTP Clients**
  - [ ] Test each of 5 clients to ensure JWT injection works:
    - [ ] AtomFeedClient (atom feed sync)
    - [ ] EmployeeFullSyncClient (employee data)
    - [ ] PayrollSyncClient (payroll data)
    - [ ] PayrollSyncService (payroll service)
    - [ ] OracleHcmHealthCheck (health check)

- [ ] **Request Correlation**
  - [ ] Each JWT has unique iat/exp timestamps (not cached tokens)
  - [ ] Token expiration is respected (old tokens rejected after expiration)

## Documentation Checklist

- [ ] Read [JWT_QUICK_REFERENCE.md](.github/JWT_QUICK_REFERENCE.md)
- [ ] Read [JWT_DEBUGGING_GUIDE.md](.github/JWT_DEBUGGING_GUIDE.md)
- [ ] Read [PS-1951_IMPLEMENTATION_SUMMARY.md](.github/PS-1951_IMPLEMENTATION_SUMMARY.md)
- [ ] Understand JWT token structure (header/payload/signature)
- [ ] Know how to enable debug logging
- [ ] Know how to check logs for errors
- [ ] Have curl commands ready for manual API testing

## Pre-Production Checklist

### Final Verification (Before Merge)
- [ ] All local tests pass
- [ ] Build succeeds with 0 warnings, 0 errors
- [ ] Code review approved
- [ ] Security review complete
- [ ] QA testing complete

### Production Certificate Setup
- [ ] Production certificate obtained and validated
- [ ] Certificate subject CN/L matches Oracle HCM configuration
- [ ] Public certificate uploaded to Oracle HCM
- [ ] Certificate private key stored securely (vault/secrets manager)
- [ ] Application secrets updated with production certificate password

### Monitoring Setup
- [ ] Logs configured to capture DEBUG level for JWT service (in prod for initial rollout)
- [ ] Alerts set up for:
  - [ ] Certificate expiration (< 7 days)
  - [ ] High 401 Unauthorized error rate
  - [ ] JWT service exceptions
- [ ] Dashboard created for JWT authentication metrics

### Deployment
- [ ] Deployment tested in staging environment first
- [ ] Production certificate configured in deployment
- [ ] Secrets/vault verified before deployment
- [ ] Rollback plan documented (in case of auth failures)

## Post-Deployment Checklist

### Verification
- [ ] Application starts successfully
- [ ] First Oracle HCM API call succeeds
- [ ] JWT debug logs appear (if DEBUG level enabled)
- [ ] No 401 Unauthorized errors for normal operations
- [ ] No CRITICAL certificate expiration warnings

### Monitoring
- [ ] Watch logs for JWT generation patterns
- [ ] Monitor error rates for any authorization failures
- [ ] Track certificate expiration countdown (< 90 days warning)
- [ ] Verify performance is acceptable (no slowdown)

### Documentation Updates
- [ ] Update runbook with JWT authentication details
- [ ] Update incident response guide for 401 errors
- [ ] Document certificate renewal procedure
- [ ] Share JWT_DEBUGGING_GUIDE.md with operations team

## Support Contacts

- **JWT Implementation**: See [JWT_DEBUGGING_GUIDE.md](.github/JWT_DEBUGGING_GUIDE.md)
- **Certificate Issues**: Certificate management team
- **Oracle HCM Configuration**: Oracle HCM administrator
- **Secrets Management**: Security/DevOps team

---

## Sign-Off

| Role | Name | Date | Status |
|------|------|------|--------|
| Developer | __________ | __________ | ☐ Passed |
| QA | __________ | __________ | ☐ Passed |
| Security | __________ | __________ | ☐ Passed |
| DevOps | __________ | __________ | ☐ Passed |

---

*This checklist should be completed for each deployment of JWT authentication changes.*
*For questions or issues, refer to the JWT debugging documentation.*
