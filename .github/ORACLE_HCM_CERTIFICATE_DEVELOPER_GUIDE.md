# Oracle HCM Certificate-Based Authentication - Developer Setup Guide

## Overview

The Profit Sharing application has been updated to support **certificate-based authentication (mutual TLS)** for Oracle HCM integration, in addition to the legacy basic authentication method.

**Key Benefits:**
- ✅ More secure than basic authentication (no passwords in memory)
- ✅ Industry-standard for API security
- ✅ Supports Oracle Fusion Cloud security requirements
- ✅ Backwards compatible (basic auth still works for development)

## Authentication Methods

### Recommended: Certificate-Based (Production)
- Uses PFX/PKCS#12 certificates
- Implements mutual TLS (mTLS)
- Certificate password stored securely
- No exposed credentials in logs

### Legacy: Basic Authentication (Development Only)
- Uses username and password
- Credentials stored in .NET User Secrets
- Suitable for local development
- **Not recommended for production**

---

## Local Development Setup

### Step 1: Obtain Your PFX Certificate

From your Oracle Fusion HCM administrator, obtain:
1. A `.pfx` file (PFX/PKCS#12 format)
2. The certificate password

Store these securely. **Do not commit the PFX file to version control.**

### Step 2: Place the PFX File Locally

Create a local folder for certificates (not in the repo):

**Windows (PowerShell):**
```powershell
# Create certificate directory in your user profile
$certDir = "$env:USERPROFILE\ProfitSharingCerts"
New-Item -ItemType Directory -Path $certDir -Force

# Copy your certificate there (example)
Copy-Item "C:\path\to\your\certificate.pfx" "$certDir\oracle-hcm.pfx"

# Verify permissions (should be readable by your user only)
Get-Item $certDir | Select-Object -ExpandProperty FullName
```

**Linux/Mac:**
```bash
# Create certificate directory
mkdir -p ~/ProfitSharingCerts
chmod 700 ~/ProfitSharingCerts

# Copy your certificate there
cp /path/to/certificate.pfx ~/ProfitSharingCerts/oracle-hcm.pfx

# Verify permissions (should be 600)
chmod 600 ~/ProfitSharingCerts/oracle-hcm.pfx
ls -la ~/ProfitSharingCerts/
```

### Step 3: Store Configuration Using .NET Secrets

The .NET User Secrets tool securely stores sensitive values on your local machine.

#### Enable User Secrets (One-Time Setup)

Navigate to the AppHost project and enable secrets:

```powershell
cd src/services/src/Demoulas.ProfitSharing.AppHost
dotnet user-secrets init
```

This creates a `.gitignored` `secrets.json` file in your user profile.

#### Store the Certificate Password

```powershell
# From the AppHost directory
cd src/services/src/Demoulas.ProfitSharing.AppHost

# Store the certificate password
dotnet user-secrets set "OracleHcm:PfxPassword" "your-certificate-password-here"

# Verify it was stored
dotnet user-secrets list
```

**Windows Path:** `%APPDATA%\microsoft\UserSecrets\<user-secrets-id>\secrets.json`
**Linux/Mac Path:** `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`

### Step 4: Configure appsettings.json

Update your local `appsettings.Development.json` in the AppHost:

```json
{
  "OracleHcm": {
    "EnableSync": true,
    "BaseAddress": "https://your-oracle-hcm-instance.oraclecloud.com",
    "DemographicUrl": "/hcmRestApi/core/v1/demographics",
    "PayrollUrl": "/hcmRestApi/core/v1/payroll",
    "PfxFilePath": "C:/Users/YourUsername/ProfitSharingCerts/oracle-hcm.pfx",
    "PfxPassword": null,
    "RestFrameworkVersion": "9",
    "Limit": 75,
    "IntervalInHours": 255,
    "PayrollIntervalInHours": 150,
    "DeltaIntervalInMinutes": 15
  }
}
```

**Important Notes:**
- `PfxPassword` should be `null` in `appsettings.json`—it will be loaded from user secrets
- `PfxFilePath` can be a relative or absolute path
- Update `BaseAddress` and URL endpoints to match your Oracle instance

### Step 5: Start Development

Run the application with Aspire:

```powershell
# From project root
aspire run
```

The application will:
1. Load the PFX file from `PfxFilePath`
2. Load the password from .NET User Secrets
3. Authenticate with Oracle HCM using mutual TLS

---

## Configuration Reference

### appsettings.json (Committed to Version Control)

```json
{
  "OracleHcm": {
    "EnableSync": true,
    "BaseAddress": "https://oracle-hcm-instance.example.com",
    "DemographicUrl": "/hcmRestApi/core/v1/demographics",
    "PayrollUrl": "/hcmRestApi/core/v1/payroll",
    "PfxFilePath": "~/ProfitSharingCerts/oracle-hcm.pfx",
    "PfxPassword": null,
    "RestFrameworkVersion": "9"
  }
}
```

### .NET User Secrets (NOT in Version Control)

Store sensitive values using the secrets tool:

```powershell
# Set certificate password
dotnet user-secrets set "OracleHcm:PfxPassword" "actual-password"

# View all secrets
dotnet user-secrets list

# Remove a secret
dotnet user-secrets remove "OracleHcm:PfxPassword"

# Clear all secrets
dotnet user-secrets clear
```

### Certificate Service Behavior

The `CertificateService` automatically:
- ✅ Loads certificates from `PfxFilePath`
- ✅ Handles password-protected certificates
- ✅ Caches certificates in memory for performance
- ✅ Validates certificate expiration dates
- ✅ Provides detailed logging for troubleshooting
- ✅ Handles both absolute and relative paths

---

## Troubleshooting

### Certificate File Not Found

**Error:**
```
FileNotFoundException: Certificate file not found: oracle-hcm.pfx
```

**Solution:**
```powershell
# Verify the file exists
Test-Path "C:\Users\YourUsername\ProfitSharingCerts\oracle-hcm.pfx"

# Check path in appsettings is correct (use forward slashes)
# ✅ Good: "C:/Users/YourUsername/ProfitSharingCerts/oracle-hcm.pfx"
# ❌ Bad: "C:\\Users\\YourUsername\\ProfitSharingCerts\\oracle-hcm.pfx"
```

### Certificate Password Incorrect

**Error:**
```
InvalidOperationException: Failed to load certificate from oracle-hcm.pfx. 
This may indicate the file is corrupted, the password is incorrect, 
or the file is not a valid PFX certificate.
```

**Solution:**
```powershell
# Verify the stored password matches the certificate
# Re-set the password
dotnet user-secrets remove "OracleHcm:PfxPassword"
dotnet user-secrets set "OracleHcm:PfxPassword" "correct-password-here"

# Restart the application
```

### Certificate Expired

**Warning in Logs:**
```
Certificate is expired. Valid To: 2024-01-01
```

**Solution:**
- Contact your Oracle HCM administrator to renew the certificate
- Replace the PFX file with a new one
- Restart the application

### Certificate Not Yet Valid

**Warning in Logs:**
```
Certificate is not yet valid. Valid From: 2025-12-01
```

**Solution:**
- Wait until the certificate's "Valid From" date
- Or contact administrator to reissue with earlier validity date

### Debugging Certificate Loading

Enable debug logging to see certificate loading details:

```json
{
  "Logging": {
    "LogLevel": {
      "Demoulas.ProfitSharing.OracleHcm.Services.CertificateService": "Debug"
    }
  }
}
```

---

## Security Best Practices

### 1. Protect Your Certificate Files
```powershell
# Windows: Use NTFS permissions
icacls "C:\Users\YourUsername\ProfitSharingCerts\oracle-hcm.pfx" /grant:r "%USERNAME%:F" /inheritance:r

# Linux/Mac: Use file permissions
chmod 600 ~/ProfitSharingCerts/oracle-hcm.pfx
```

### 2. Never Commit Certificates or Passwords
```
# .gitignore (already configured in repo)
ProfitSharingCerts/
secrets.json
*.pfx
```

### 3. Use .NET User Secrets in Development
- ✅ Automatic encryption by .NET
- ✅ Stored in user profile, not in code
- ✅ Different per developer machine

### 4. Rotate Certificates Regularly
- Plan quarterly certificate renewal with Oracle HCM team
- Test renewal process in QA environment first
- Document renewal procedure in wiki

### 5. Log Audit Trail
The `CertificateService` logs:
- Certificate loading (with filename only, path masked)
- Certificate subject and thumbprint
- Validity dates
- Expiration warnings

---

## Migration from Basic Auth to Certificates

If currently using basic authentication:

### Before (Basic Auth)
```json
{
  "OracleHcm": {
    "Username": "hcm_user",
    "Password": "secret-password"
  }
}
```

### After (Certificate Auth)
```json
{
  "OracleHcm": {
    "PfxFilePath": "~/ProfitSharingCerts/oracle-hcm.pfx",
    "PfxPassword": null
  }
}
```

### Migration Steps
1. Obtain PFX certificate from Oracle HCM admin
2. Follow local setup steps above
3. Update `appsettings.json` to use `PfxFilePath`
4. Remove `Username` and `Password` from config
5. Test with `aspire run`

---

## Support

### For Issues:
1. Check logs in Aspire Dashboard for certificate loading errors
2. Verify certificate file exists and is readable
3. Verify password is stored correctly in secrets
4. Check certificate validity dates
5. Contact Oracle HCM administrator if certificate issues persist

### For Questions:
- See [ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md](./ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md) for DevOps setup
- See [ORACLE_HCM_CERTIFICATE_IMPLEMENTATION.md](./ORACLE_HCM_CERTIFICATE_IMPLEMENTATION.md) for implementation details
- Post in #dev-oracle-hcm Slack channel

---

**Last Updated:** November 2025
