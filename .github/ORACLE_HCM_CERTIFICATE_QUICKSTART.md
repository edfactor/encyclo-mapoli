# Oracle HCM Certificate Authentication - Quick Start

## 📋 What's New?

The Profit Sharing application now supports **certificate-based authentication (mutual TLS)** for Oracle HCM integration, replacing basic authentication.

**Quick Facts:**
- 🔒 More secure (no passwords in memory)
- ✅ Recommended for production
- 📦 Backwards compatible (basic auth still works)
- 🚀 Ready to deploy

---

## 🎯 For Developers (Local Setup)

**TL;DR:** 30-minute setup

```powershell
# 1. Get certificate from Oracle admin
# 2. Place in ~/ProfitSharingCerts/oracle-hcm.pfx

# 3. Enable user secrets
cd src/services/src/Demoulas.ProfitSharing.AppHost
dotnet user-secrets init

# 4. Store certificate password
dotnet user-secrets set "OracleHcm:PfxPassword" "your-password"

# 5. Update appsettings.Development.json (see below)

# 6. Run
aspire run
```

**appsettings.Development.json:**
```json
{
  "OracleHcm": {
    "BaseAddress": "https://your-oracle-instance.oraclecloud.com",
    "DemographicUrl": "/hcmRestApi/core/v1/demographics",
    "PfxFilePath": "C:/Users/YourUsername/ProfitSharingCerts/oracle-hcm.pfx",
    "PfxPassword": null
  }
}
```

📖 **Full Guide:** [ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md](./.github/ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md)

---

## 🚀 For DevOps (Deployment)

**Key Steps:**

1. **Store secrets in Bitbucket:**
   - Create repository variables for certificate (Base64) and password
   - Reference in pipeline environment

2. **Decode certificate in pipeline:**
   ```bash
   echo "$PFX_BASE64" | base64 -d > /tmp/oracle-hcm.pfx
   ```

3. **Mount in deployment:**
   - Docker: COPY certificate into image
   - Kubernetes: Use Secrets volume

4. **Configure environment variables:**
   ```bash
   OracleHcm__PfxFilePath=/app/certs/oracle-hcm.pfx
   OracleHcm__PfxPassword=<from-secrets>
   ```

📖 **Full Guide:** [ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md](./.github/ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md)

---

## 📚 Implementation Reference

See [ORACLE_HCM_CERTIFICATE_IMPLEMENTATION.md](./.github/ORACLE_HCM_CERTIFICATE_IMPLEMENTATION.md) for:
- Architecture details
- Code examples
- Testing patterns
- Performance metrics
- Troubleshooting

---

## ⚙️ Configuration

### Automatic Authentication Selection

The application **automatically chooses** authentication based on configuration:

```csharp
// If PfxFilePath is set → Use certificate (mTLS)
if (!string.IsNullOrWhiteSpace(config.PfxFilePath))
    usesCertificateAuth = true;

// Else if Username is set → Use basic auth (legacy)
else if (!string.IsNullOrWhiteSpace(config.Username))
    usesBasicAuth = true;

// Else → No authentication
```

### Configuration Precedence (First Match Wins)

1. **Environment Variables**
   ```bash
   OracleHcm__PfxFilePath=/path/to/cert.pfx
   OracleHcm__PfxPassword=password
   ```

2. **appsettings.{Environment}.json**
   ```json
   { "OracleHcm": { "PfxFilePath": "..." } }
   ```

3. **.NET User Secrets** (development only)
   ```powershell
   dotnet user-secrets set "OracleHcm:PfxPassword" "..."
   ```

4. **appsettings.json** (base config)

---

## 🔍 Code Changes Summary

### Modified Files

| File | Changes |
|------|---------|
| `OracleHcmConfig.cs` | Added `PfxFilePath`, `PfxPassword`, `UseCertificateAuthentication` property |
| `OracleHcmExtension.cs` | Register `CertificateService`, configure HTTP clients with certificate handling |
| `BuildOracleHcmAuthClient()` | Support certificate-based auth with fallback to basic auth |
| `ConfigureHttpClients()` | Attach certificates to HttpClientHandler for mTLS |

### New Files

| File | Purpose |
|------|---------|
| `ICertificateService.cs` | Interface for certificate loading |
| `CertificateService.cs` | Loads, caches, and validates PFX certificates |

### Backwards Compatibility

✅ **Fully compatible** - Existing basic auth configurations continue to work unchanged

---

## 🧪 Testing Your Setup

### Verify Certificate Loading

```powershell
# Check certificate is recognized
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cert.Import("path/to/oracle-hcm.pfx", "password", [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::DefaultKeySet)
Write-Host "Certificate loaded: $($cert.Subject)"
Write-Host "Valid from: $($cert.NotBefore) to $($cert.NotAfter)"
```

### Verify Configuration

```powershell
# Run app and check logs for:
# ✓ "Certificate loaded and cached: oracle-hcm.pfx"
# ✓ Certificate subject and thumbprint
# ✓ Certificate validity dates

aspire run
```

### Test Oracle HCM Connection

```powershell
# Health check endpoint (if implemented)
curl http://localhost:8080/health/checks/OracleHcm
```

---

## 📋 Common Tasks

### Store Certificate Password (Development)

```powershell
cd src/services/src/Demoulas.ProfitSharing.AppHost
dotnet user-secrets set "OracleHcm:PfxPassword" "your-password"
```

### View Stored Secrets

```powershell
dotnet user-secrets list
```

### Clear All Secrets

```powershell
dotnet user-secrets clear
```

### Validate Certificate File

```bash
# Linux/Mac/Windows (with OpenSSL)
openssl pkcs12 -in oracle-hcm.pfx -passin pass:your-password -noout
# Should succeed silently if password is correct
```

### Convert Certificate to Base64 (for Bitbucket)

```powershell
# Windows
[Convert]::ToBase64String([System.IO.File]::ReadAllBytes("C:\path\to\cert.pfx")) | Set-Clipboard
```

```bash
# Linux/Mac
base64 -i /path/to/cert.pfx | pbcopy
```

---

## ⚠️ Security Notes

### DO ✅
- ✅ Store PFX file outside repository
- ✅ Use appropriate file permissions (600 Linux, NTFS ACL Windows)
- ✅ Store password in .NET User Secrets (dev) or Bitbucket Variables (prod)
- ✅ Rotate certificates annually
- ✅ Rotate immediately if compromised

### DON'T ❌
- ❌ Commit PFX files to git
- ❌ Store passwords in appsettings.json
- ❌ Log certificate passwords
- ❌ Use self-signed certificates in production
- ❌ Share certificates via email or chat

---

## 🐛 Troubleshooting

### Certificate File Not Found
```
Check: Test-Path "C:/Users/YourUsername/ProfitSharingCerts/oracle-hcm.pfx"
Fix: Verify path in appsettings uses forward slashes
```

### Password Incorrect
```
Fix: dotnet user-secrets set "OracleHcm:PfxPassword" "correct-password"
Check: Verify with OpenSSL: openssl pkcs12 -in cert.pfx -passin pass:pwd -noout
```

### Certificate Expired
```
Contact: Oracle HCM administrator for certificate renewal
```

### Oracle HCM 401 Unauthorized
```
Check: Certificate registered with Oracle HCM
Check: Certificate not expired: cert.NotAfter > DateTime.Now
Check: Certificate subject matches Oracle expectations
```

---

## 📞 Support

| Question | Reference |
|----------|-----------|
| How do I set up locally? | [Developer Guide](./.github/ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md) |
| How do I deploy? | [DevOps Guide](./.github/ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md) |
| How does it work? | [Implementation Guide](./.github/ORACLE_HCM_CERTIFICATE_IMPLEMENTATION.md) |
| Something's broken | See Troubleshooting section above |
| Still stuck? | Post in #dev-oracle-hcm or escalate to DevOps |

---

## 📝 Checklist

**Before committing code:**
- [ ] No PFX files in git
- [ ] No passwords in config files
- [ ] Secrets stored in .NET User Secrets
- [ ] appsettings has `"PfxPassword": null`
- [ ] Configuration documentation updated

**Before deploying to QA:**
- [ ] Certificate obtained from Oracle HCM admin
- [ ] Base64 certificate stored in Bitbucket
- [ ] Certificate password in Bitbucket secure variables
- [ ] Pipeline updated to decode and mount certificate
- [ ] Health check passes

**Before deploying to Production:**
- [ ] QA deployment successful
- [ ] Certificate validated with Oracle HCM team
- [ ] Monitoring/alerting configured
- [ ] Rollback procedure documented
- [ ] Team notified of deployment

---

**Version:** 1.0  
**Last Updated:** November 2025  
**Maintained By:** Development & DevOps Teams
