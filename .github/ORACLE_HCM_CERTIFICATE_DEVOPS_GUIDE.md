# Oracle HCM Certificate-Based Authentication - DevOps Deployment Guide

## Overview

This guide provides instructions for DevOps and Infrastructure teams to deploy the Profit Sharing application with certificate-based authentication to Oracle HCM in various environments.

**Deployment Scenarios:**
- 🏗️ QA Environment (certificate renewal testing)
- 🚀 Production Environment (high-security)
- 🔄 CI/CD Pipeline (Bitbucket Pipelines automation)

---

## Architecture

```
┌─────────────────────────────────────────┐
│   Deployment Environment (QA, Prod)     │
│                                         │
│  ┌─────────────────────────────────┐   │
│  │ Application Server              │   │
│  │                                 │   │
│  │  ┌────────────────────────────┐ │   │
│  │  │ Profit Sharing API         │ │   │
│  │  │                            │ │   │
│  │  │ ┌──────────────────────┐   │ │   │
│  │  │ │ CertificateService   │   │ │   │
│  │  │ │                      │   │ │   │
│  │  │ │ Loads cert from:     │   │ │   │
│  │  │ │ 1. Volume mount      │   │ │   │
│  │  │ │ 2. Environment vars  │   │ │   │
│  │  │ │ 3. Bitbucket Secrets │   │ │   │
│  │  │ └──────────────────────┘   │ │   │
│  │  │                            │ │   │
│  │  │ ┌──────────────────────┐   │ │   │
│  │  │ │ HttpClient           │   │ │   │
│  │  │ │ Mutual TLS           │   │ │   │
│  │  │ └──────────────────────┘   │ │   │
│  │  └────────────────────────────┘ │   │
│  │          │                       │   │
│  │          ▼                       │   │
│  │  ┌────────────────────────────┐ │   │
│  │  │ PFX Certificate File       │ │   │
│  │  │ (Volume Mount or Base64)   │ │   │
│  │  └────────────────────────────┘ │   │
│  └─────────────────────────────────┘   │
│          │                              │
│          ▼ (mTLS)                       │
│  ┌──────────────────────────────────┐  │
│  │ Oracle Fusion HCM                │  │
│  │ (Validates Client Certificate)   │  │
│  └──────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

---

## Prerequisites

### Bitbucket Secu Repository Variables
Contact your Oracle HCM administrator to obtain:

1. **PFX Certificate File** (in Base64 format for pipeline storage)
2. **Certificate Password** (stored in Bitbucket secure repository variables)
3. **Oracle HCM API Endpoint URLs**

### Tool Requirements
- PowerShell 7+ (for Windows runners)
- .NET 9 SDK
- OpenSSL (for certificate validation, optional)

---

## Bitbucket Pipelines Configuration

### Step 1: Store Secrets in Bitbucket Cloud

#### Access Bitbucket Repository Settings
1. Go to your repository in Bitbucket Cloud
2. Navigate to **Repository Settings → Pipelines → Repository Variables**
3. Create the following secure repository variables:

| Variable Name | Value | Secure | Type |
|---|---|---|---|
| `ORACLE_HCM_PFX_PASSWORD` | Certificate password | ✅ Yes | Password |
| `ORACLE_HCM_PFX_BASE64` | Base64-encoded certificate | ✅ Yes | Password |
| `ORACLE_HCM_BASE_ADDRESS` | `https://your-instance.oraclecloud.com` | ❌ No | String |
| `ORACLE_HCM_DEMOGRAPHIC_URL` | `/hcmRestApi/core/v1/demographics` | ❌ No | String |
| `ORACLE_HCM_PAYROLL_URL` | `/hcmRestApi/core/v1/payroll` | ❌ No | String |

#### Converting Certificate to Base64 (PowerShell)

```powershell
# Windows
[Convert]::ToBase64String([System.IO.File]::ReadAllBytes("C:\path\to\cert.pfx")) | Set-Clipboard

# Copy from clipboard to Bitbucket variable

# Or save to file
$base64 = [Convert]::ToBase64String([System.IO.File]::ReadAllBytes("C:\path\to\cert.pfx"))
$base64 | Out-File -FilePath "cert_base64.txt" -Encoding UTF8
```

```bash
# Linux/Mac
base64 -i /path/to/cert.pfx | pbcopy
# Or
base64 -i /path/to/cert.pfx > cert_base64.txt
```

### Step 2: Update bitbucket-pipelines.yml

Add deployment step to handle certificate setup:

```yaml
definitions:
  steps:
    deploy-with-certificate: &deploy-with-certificate
      name: Deploy with Oracle HCM Certificate
      image: ubuntu:latest
      script:
        # Variables (from Bitbucket secure variables)
        - PFX_PASSWORD=$ORACLE_HCM_PFX_PASSWORD
        - PFX_BASE64=$ORACLE_HCM_PFX_BASE64
        - ORACLE_HCM_BASE=$ORACLE_HCM_BASE_ADDRESS
        - ORACLE_HCM_DEMO_URL=$ORACLE_HCM_DEMOGRAPHIC_URL
        - ORACLE_HCM_PAYROLL_URL=$ORACLE_HCM_PAYROLL_URL
        
        # Create certificate from Base64
        - echo "Decoding PFX certificate from Base64..."
        - echo "$PFX_BASE64" | base64 -d > /tmp/oracle-hcm.pfx
        - ls -lh /tmp/oracle-hcm.pfx
        
        # Verify certificate
        - echo "Validating certificate..."
        - openssl pkcs12 -in /tmp/oracle-hcm.pfx -passin pass:$PFX_PASSWORD -noout
        - echo "✓ Certificate validation successful"
        
        # Build application
        - echo "Building application..."
        - cd src/services
        - dotnet build Demoulas.ProfitSharing.slnx --configuration Release
        
        # Create docker image with certificate
        - echo "Creating Docker image with certificate..."
        - docker build -t profit-sharing:$BITBUCKET_COMMIT -f Dockerfile .
        
        # Deploy to environment
        - echo "Deploying to $DEPLOYMENT_ENV..."
        # Your deployment steps here (kubectl apply, docker push, etc.)

pipelines:
  branches:
    develop:
      - step: *build-and-test-backend
      - step: *test-frontend
      - step:
          <<: *deploy-with-certificate
          deployment: staging
          trigger: manual
    main:
      - step: *build-and-test-backend
      - step: *test-frontend
      - step:
          <<: *deploy-with-certificate
          deployment: production
          trigger: manual
```

### Step 3: Docker Configuration

Update your Dockerfile to include certificate handling:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9 AS runtime
WORKDIR /app

# Copy application
COPY --from=builder /app/publish .

# Create certificate directory with proper permissions
RUN mkdir -p /app/certs && chmod 750 /app/certs

# Copy certificate (passed via build arg or volume)
# ARG PFX_FILE
# COPY ${PFX_FILE} /app/certs/oracle-hcm.pfx

# Set environment variables for configuration
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_EnableDiagnostics=0

# Note: Certificate password comes from secret management
EXPOSE 8080
ENTRYPOINT ["dotnet", "Demoulas.ProfitSharing.Api.dll"]
```

### Step 4: Kubernetes Deployment

If using Kubernetes, store certificate as a secret:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: oracle-hcm-certificate
  namespace: profit-sharing
type: Opaque
data:
  oracle-hcm.pfx: <base64-encoded-certificate>
  certificate-password: <base64-encoded-password>
```

Mount in deployment:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: profit-sharing-api
  namespace: profit-sharing
spec:
  template:
    spec:
      containers:
      - name: api
        image: profit-sharing:latest
        volumeMounts:
        - name: oracle-certs
          mountPath: /app/certs
          readOnly: true
        env:
        - name: OracleHcm__PfxFilePath
          value: /app/certs/oracle-hcm.pfx
        - name: OracleHcm__PfxPassword
          valueFrom:
            secretKeyRef:
              name: oracle-hcm-certificate
              key: certificate-password
      volumes:
      - name: oracle-certs
        secret:
          secretName: oracle-hcm-certificate
          items:
          - key: oracle-hcm.pfx
            path: oracle-hcm.pfx
```

---

## Configuration Management

### Environment Variables

Configure the application using environment variables:

```powershell
# Bitbucket Pipeline example
$env:OracleHcm__BaseAddress = "https://your-oracle-instance.oraclecloud.com"
$env:OracleHcm__DemographicUrl = "/hcmRestApi/core/v1/demographics"
$env:OracleHcm__PayrollUrl = "/hcmRestApi/core/v1/payroll"
$env:OracleHcm__PfxFilePath = "/app/certs/oracle-hcm.pfx"
$env:OracleHcm__PfxPassword = "certificate-password"
$env:OracleHcm__EnableSync = "true"
```

### Health Checks

The application includes a health check for Oracle HCM certificate:

```bash
# Verify certificate is loaded correctly
curl -i http://localhost:8080/health/checks/OracleHcm

# Response should include certificate validation status
{
  "name": "OracleHcm",
  "status": "Healthy",
  "description": "Certificate loaded: CN=Oracle Fusion HCM"
}
```

---

## Certificate Rotation

### Annual Renewal Process

1. **Request Renewal (30 days before expiration)**
   - Contact Oracle HCM administrator
   - Request new certificate with extended validity

2. **Testing (in QA)**
   ```bash
   # Update QA variables with new certificate
   # Deploy to QA
   # Verify sync operations work
   # Test both Employee Sync services
   ```

3. **Update Production**
   - Update production Bitbucket variables
   - Perform blue-green deployment
   - Monitor for authentication errors

4. **Cleanup**
   - Archive old certificate (encrypted)
   - Update documentation
   - Remove old certificate from Bitbucket variables

### Emergency Certificate Replacement

If certificate is compromised:

1. **Immediately update Bitbucket variables** with new certificate
2. **Trigger redeployment** to all environments
3. **Rotate certificate** on Oracle HCM side with administrator
4. **Document** incident and remediation steps

---

## Monitoring & Logging

### Logs to Monitor

In production logs, look for:

```
[INF] Certificate loaded and cached: oracle-hcm.pfx
[INF] Subject: CN=Oracle Fusion HCM, O=Demoulas
[INF] Thumbprint: A1B2C3D4E5F6...
[INF] Valid From: 2024-01-01, Valid To: 2025-12-31
```

### Warning Logs

**Certificate expiring soon:**
```
[WRN] Certificate is not yet valid. Valid From: 2025-12-01
```

**Certificate expired:**
```
[ERR] Certificate is expired. Valid To: 2024-06-01
```

### Metrics to Track

Set up monitoring for:

```promql
# Certificate load failures
increase(oracle_hcm_certificate_load_errors_total[1h])

# Authentication failures
increase(oracle_hcm_auth_failures_total[1h])

# Certificate expiration warning (days until expiration)
oracle_hcm_certificate_days_until_expiration
```

---

## Troubleshooting

### Certificate Not Found

**Error in Logs:**
```
FileNotFoundException: Certificate file not found: /app/certs/oracle-hcm.pfx
```

**Diagnosis:**
```bash
# Check if certificate is mounted
ls -la /app/certs/

# Check environment variable
echo $OracleHcm__PfxFilePath

# Check pod volume mounts (if Kubernetes)
kubectl describe pod <pod-name> -n profit-sharing
```

**Resolution:**
- Verify certificate is in Bitbucket variables
- Verify volume mount in deployment
- Trigger redeployment

### Certificate Password Incorrect

**Error in Logs:**
```
InvalidOperationException: Failed to load certificate from oracle-hcm.pfx.
This may indicate...the password is incorrect...
```

**Diagnosis:**
```bash
# Verify certificate and password match
openssl pkcs12 -in cert.pfx -passin pass:your-password -noout
# Should succeed silently
```

**Resolution:**
- Update password in Bitbucket variables
- Ensure password hasn't been rotated on Oracle side
- Contact Oracle HCM administrator if unsure

### Authentication Failures (401)

**Error Pattern:**
```
HttpStatusCode.Unauthorized from Oracle HCM API
```

**Diagnosis:**
1. Verify certificate is valid (not expired)
2. Verify certificate is registered with Oracle HCM
3. Check certificate subject matches expectations
4. Verify certificate thumbprint registered with Oracle

**Resolution:**
```bash
# Display certificate details
openssl pkcs12 -in cert.pfx -passin pass:your-password
# Compare Subject and Thumbprint with Oracle registration
```

### Certificate Expiration

**Proactive Check:**
```powershell
# PowerShell script to check expiration
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cert.Import("C:\certs\oracle-hcm.pfx", "password", [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::DefaultKeySet)
$daysLeft = ($cert.NotAfter - (Get-Date)).Days
Write-Host "Days until certificate expires: $daysLeft"
```

---

## Security Considerations

### 1. Secure Secret Storage
- ✅ Use Bitbucket secure repository variables
- ✅ Rotate passwords regularly
- ✅ Audit variable access logs
- ❌ Never commit certificates to git
- ❌ Never log certificate paths or passwords

### 2. Deployment Security
```yaml
# Use read-only filesystem where possible
securityContext:
  readOnlyRootFilesystem: true
  runAsNonRoot: true
  runAsUser: 1000

# Restrict certificate file permissions
- name: oracle-certs
  secret:
    secretName: oracle-hcm-certificate
    defaultMode: 0400  # r-------- (read-only for owner)
```

### 3. Mutual TLS Validation
The application validates:
- ✅ Oracle server certificate (in production)
- ✅ Client certificate expiration
- ✅ Certificate chain

Debug mode allows self-signed certificates; production enforces validation.

### 4. Audit Trail
All certificate operations are logged:
- Certificate loading
- Certificate validation
- Authentication successes/failures
- Certificate expiration warnings

---

## Rollback Procedure

If deployment fails:

```bash
# Rollback to previous certificate
# 1. Update Bitbucket variables with previous certificate
# 2. Trigger redeployment
# 3. Verify services reconnect to Oracle HCM

# Kubernetes rollback
kubectl rollout undo deployment/profit-sharing-api -n profit-sharing
```

---

## Performance Optimization

### Certificate Caching

The application caches certificates in memory:
- First request: Load certificate from file (slower)
- Subsequent requests: Use cached certificate (faster)
- Cache lifetime: Application lifetime (no expiration)

### Impact
- ~50-100ms overhead on first request using certificate
- <1ms overhead on cached requests
- Memory overhead: ~2KB per cached certificate

---

## Emergency Contacts

**If certificate issues occur:**
1. Check logs in Aspire Dashboard or application monitoring
2. Verify Bitbucket variables are up to date
3. Contact Oracle HCM administrator
4. Escalate to DevOps team

**Escalation Path:**
- Level 1: Check configuration and logs
- Level 2: Verify certificate with Oracle admin
- Level 3: Rollback and investigate root cause

---

## References

- [Oracle HCM JWT API Authentication](https://www.oracle.com/webfolder/technetwork/tutorials/obe/fusionapps/HCM/JWT_API_Authentication_OBE/html/index.html)
- [.NET X509Certificate2 Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2)
- [Kubernetes Secrets Documentation](https://kubernetes.io/docs/concepts/configuration/secret/)
- [OpenSSL PKCS12 Manual](https://www.openssl.org/docs/man1.1.1/man1/pkcs12.html)

---

**Last Updated:** November 2025
**Maintained By:** DevOps Team
