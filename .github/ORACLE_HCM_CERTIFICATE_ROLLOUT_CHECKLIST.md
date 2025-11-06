# Oracle HCM Certificate Authentication - Team Rollout Checklist

## 📋 Quick Overview

This checklist helps your team implement certificate-based authentication for Oracle HCM. All code is complete, tested, and ready to use.

**What Changed:** Authentication method updated from basic auth to certificate-based (mTLS)  
**Breaking Changes:** None - fully backwards compatible  
**Timeline:** Can migrate gradually or all at once  

---

## 🚀 Phase 1: Preparation (Week 1)

### Step 1: Get Certificate
- [ ] Contact Oracle HCM administrator
- [ ] Request PFX certificate file for Profit Sharing application
- [ ] Request certificate password
- [ ] Verify certificate validity dates (should be 1+ year out)
- [ ] Document certificate details in secure location

### Step 2: Review Documentation
- [ ] **Developers:** Read [Developer Guide](.github/ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md)
- [ ] **DevOps:** Read [DevOps Guide](.github/ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md)
- [ ] **QA/Testers:** Read [Quick Start Guide](.github/ORACLE_HCM_CERTIFICATE_QUICKSTART.md)
- [ ] **All:** Watch any video tutorials if available
- [ ] Hold team knowledge-sharing session

### Step 3: Set Up Development Environment
- [ ] Developers: Follow [30-minute local setup](./ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md#local-development-setup)
- [ ] Create `~/ProfitSharingCerts/` directory
- [ ] Place certificate in directory with proper permissions
- [ ] Enable .NET User Secrets
- [ ] Store certificate password
- [ ] Update appsettings.Development.json
- [ ] Test `aspire run` works

### Step 4: Verify Local Functionality  
- [ ] Application starts successfully
- [ ] Certificate loads without errors
- [ ] Check logs for "Certificate loaded and cached"
- [ ] Oracle HCM sync operations work
- [ ] No 401/Unauthorized errors
- [ ] Test both Employee Sync and Payroll Sync services

---

## 🏗️ Phase 2: QA Environment Setup (Week 2)

### Step 1: Prepare Bitbucket Variables
- [ ] Encode certificate as Base64:
  ```powershell
  [Convert]::ToBase64String([System.IO.File]::ReadAllBytes("path\to\cert.pfx")) | Set-Clipboard
  ```
- [ ] Create Bitbucket repository variable: `ORACLE_HCM_PFX_BASE64`
- [ ] Paste Base64 certificate (secure variable: YES)
- [ ] Create Bitbucket repository variable: `ORACLE_HCM_PFX_PASSWORD`
- [ ] Paste certificate password (secure variable: YES)
- [ ] Create non-secure variables for endpoints:
  - [ ] `ORACLE_HCM_BASE_ADDRESS`
  - [ ] `ORACLE_HCM_DEMOGRAPHIC_URL`
  - [ ] `ORACLE_HCM_PAYROLL_URL`

### Step 2: Update Bitbucket Pipelines
- [ ] Add certificate decoding step to pipeline
- [ ] Add certificate validation step (openssl pkcs12 check)
- [ ] Configure Docker/deployment to use certificate
- [ ] Update environment variables for deployment
- [ ] Test pipeline builds successfully

### Step 3: Deploy to QA
- [ ] Run manual Bitbucket pipeline deployment
- [ ] Monitor application startup logs
- [ ] Verify certificate loads successfully
- [ ] Check "Certificate loaded and cached" in logs
- [ ] Verify health checks pass

### Step 4: QA Testing
- [ ] Employee Delta Sync works
- [ ] Employee Full Sync works
- [ ] Payroll Sync works
- [ ] No authentication errors
- [ ] Monitor for 48 hours for stability
- [ ] Document any issues

---

## ✅ Phase 3: Production Deployment (Week 3-4)

### Step 1: Pre-Deployment Verification
- [ ] QA environment running stable for 48+ hours
- [ ] No authentication errors in QA
- [ ] Oracle HCM admin confirms certificate is registered
- [ ] Certificate validity verified (not expired)
- [ ] Monitoring and alerting configured
- [ ] Rollback procedure documented and tested

### Step 2: Production Deployment
- [ ] Deploy to production during low-traffic window
- [ ] Use blue-green deployment (if applicable)
- [ ] Monitor application startup logs closely
- [ ] Verify certificate loads in production
- [ ] Check all sync services operational
- [ ] Monitor for 2+ hours post-deployment

### Step 3: Post-Deployment Verification
- [ ] No 401/Unauthorized errors in production
- [ ] Sync operations running normally
- [ ] Certificate expiration warnings appear (if applicable)
- [ ] Metrics dashboard shows normal traffic
- [ ] User reports show no issues
- [ ] Monitoring alerts configured and working

### Step 4: Documentation Update
- [ ] Update runbook with new cert configuration
- [ ] Document certificate password storage location
- [ ] Add certificate rotation schedule to calendar
- [ ] Update incident response procedures
- [ ] Document rollback steps in runbook

---

## 🔄 Phase 4: Certificate Management (Ongoing)

### Monthly Tasks
- [ ] Monitor certificate expiration dates
- [ ] Review authentication logs for anomalies
- [ ] Check monitoring/alerting is functioning
- [ ] Verify backup certificate password storage

### Quarterly Tasks (Q1, Q2, Q3, Q4)
- [ ] Certificate security review with Oracle admin
- [ ] Review and rotate if needed
- [ ] Update documentation
- [ ] Conduct team knowledge refresh

### Annual Tasks (Before Expiration)
- [ ] **30 days before expiration:** Request certificate renewal
- [ ] **15 days before:** Test renewal in QA
- [ ] **7 days before:** Update production certificate
- [ ] **Day of renewal:** Monitor closely for errors
- [ ] **Post-renewal:** Archive old certificate securely

### Emergency Tasks (Certificate Compromised)
- [ ] [ ] IMMEDIATELY: Contact Oracle HCM admin
- [ ] [ ] IMMEDIATELY: Update Bitbucket variables with new certificate
- [ ] [ ] IMMEDIATELY: Trigger production deployment
- [ ] [ ] Rotate certificate on Oracle HCM side
- [ ] [ ] Verify sync operations
- [ ] [ ] Document incident and remediation

---

## 🧪 Testing Checklist

### Unit Tests
- [ ] Certificate loading from file
- [ ] Certificate caching (double-load returns same instance)
- [ ] Certificate expiration detection
- [ ] Password-protected certificate support
- [ ] Invalid password error handling
- [ ] Missing file error handling

### Integration Tests
- [ ] HTTP client authentication with certificate
- [ ] Fallback to basic auth when no certificate
- [ ] Configuration precedence (env > config > secrets)
- [ ] Oracle HCM connectivity

### End-to-End Tests
- [ ] Employee Delta Sync reads from Oracle
- [ ] Employee Full Sync reads from Oracle
- [ ] Payroll Sync reads from Oracle
- [ ] Data consistency between syncs
- [ ] Performance under load
- [ ] Resilience (circuit breaker, timeout, retry)

---

## 📊 Monitoring Checklist

### Metrics to Monitor
- [ ] Certificate load failures (should be 0)
- [ ] Authentication failures (should be minimal)
- [ ] Sync operation success rate (should be >99%)
- [ ] Request/response times (should be normal)
- [ ] Large responses (should be expected sizes)

### Alerts to Configure
- [ ] Certificate load failures → Critical
- [ ] Authentication error spike → Warning
- [ ] Certificate expiring in 30 days → Info
- [ ] Certificate expired → Critical
- [ ] High error rate → Warning

### Logs to Review
- [ ] Application startup logs (first 5 minutes)
- [ ] Certificate loading logs
- [ ] Authentication attempts
- [ ] Any 401 errors
- [ ] Certificate expiration warnings

---

## 🆘 Troubleshooting Quick Links

| Issue | Solution |
|-------|----------|
| Certificate file not found | See [Developer Guide - Troubleshooting](./ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md#troubleshooting) |
| Password incorrect | See [Developer Guide - Troubleshooting](./ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md#troubleshooting) |
| Certificate expired | See [DevOps Guide - Certificate Expiration](./ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md#certificate-expiration) |
| 401 Unauthorized | See [DevOps Guide - Troubleshooting](./ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md#troubleshooting) |
| Deployment failed | See [DevOps Guide - Troubleshooting](./ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md#troubleshooting) |

---

## 📞 Escalation Path

**Level 1: Developer/Local Issues**
- Check logs in Aspire Dashboard
- Review configuration
- Verify certificate file exists
- Consult [Developer Guide](./ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md)
- Post in #dev-oracle-hcm Slack

**Level 2: Deployment/DevOps Issues**
- Check Bitbucket pipeline logs
- Verify environment variables
- Test certificate decoding
- Consult [DevOps Guide](./ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md)
- Contact DevOps team

**Level 3: Oracle HCM Integration Issues**
- Verify certificate registered with Oracle
- Verify certificate thumbprint matches
- Contact Oracle HCM administrator
- Check certificate validity dates
- Request diagnostic access to Oracle instance

**Level 4: Critical Production Issues**
- Activate incident response
- Prepare rollback (switch back to basic auth)
- Escalate to management
- Document issue timeline
- Schedule post-mortem

---

## ✨ Success Criteria

**Development Phase Complete When:**
- ✅ All developers can `aspire run` successfully
- ✅ Certificate loads and logs appear
- ✅ Sync operations work
- ✅ No 401 errors
- ✅ Team passes knowledge check

**QA Phase Complete When:**
- ✅ QA environment running 48+ hours stable
- ✅ All sync operations working
- ✅ No authentication errors
- ✅ QA team signs off
- ✅ Monitoring shows normal metrics

**Production Phase Complete When:**
- ✅ Production stable 2+ hours post-deployment
- ✅ No 401 errors
- ✅ All sync operations working
- ✅ Metrics dashboard normal
- ✅ No user complaints
- ✅ Runbooks updated

**Ongoing Phase Complete When:**
- ✅ Quarterly reviews completed
- ✅ Certificate rotation schedule established
- ✅ Emergency procedures documented
- ✅ Team trained on procedures
- ✅ Monitoring alerts working

---

## 📑 Documentation Reference

| Document | Audience | Purpose |
|----------|----------|---------|
| [Quick Start](./ORACLE_HCM_CERTIFICATE_QUICKSTART.md) | Everyone | One-page reference |
| [Developer Guide](./ORACLE_HCM_CERTIFICATE_DEVELOPER_GUIDE.md) | Developers | Local setup & troubleshooting |
| [DevOps Guide](./ORACLE_HCM_CERTIFICATE_DEVOPS_GUIDE.md) | DevOps/Infra | Production deployment |
| [Implementation Guide](./ORACLE_HCM_CERTIFICATE_IMPLEMENTATION.md) | Engineers | Technical deep-dive |
| [Summary](./ORACLE_HCM_CERTIFICATE_SUMMARY.md) | Leadership | Executive summary |
| This Checklist | Project Managers | Rollout tracking |

---

## 🏁 Sign-Off

**Prepared By:** Development Team  
**Reviewed By:** [Required before rollout]  
**Approved By:** [Required before production]  

**Phase 1 Completion Date:** ____________  
**Phase 2 Completion Date:** ____________  
**Phase 3 Completion Date:** ____________  
**Phase 4 Status:** Ongoing  

---

**Notes:**
```
[Use this space to record team progress, blockers, and decisions]


```

---

**Last Updated:** November 2025  
**Version:** 1.0
