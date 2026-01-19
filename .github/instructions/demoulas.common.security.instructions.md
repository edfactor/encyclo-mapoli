---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# Application Security Guidelines

Rules

-   Enforce authz on every request, server-side only.
-   Use parameterized queries. Avoid raw SQL unless fully parameterized.
-   Do not log secrets or PII.
-   Use approved crypto and secure transport (TLS).

See also

-   demoulas.common.instructions.md
-   smart.code-review.instructions.md

---

### R – Repudiation

**Threat:** Attacker denies performing an action (no audit trail).

**Questions to Ask:**

-   Are critical actions logged with user identity and timestamp?
-   Can logs be tampered with or deleted?
-   Do we have non-repudiation for financial transactions?

**Mitigations:**

-   Comprehensive audit logging
-   Immutable log storage
-   Correlation IDs across distributed systems
-   Digital signatures for transactions

---

### I – Information Disclosure

**Threat:** Attacker gains access to sensitive information.

**Questions to Ask:**

-   Are error messages revealing stack traces, SQL, or file paths?
-   Is PII properly masked in logs and responses?
-   Can an attacker enumerate valid usernames or IDs?

**Mitigations:**

-   Generic error messages in production
-   Mask PII in logs (SSN: `***-**-1234`, Email: `u***@c***.com`)
-   Rate limiting on authentication endpoints
-   No detailed timing differences for valid/invalid users

---

### D – Denial of Service (DoS)

**Threat:** Attacker makes the system unavailable to legitimate users.

**Questions to Ask:**

-   Can an attacker exhaust resources (CPU, memory, DB connections)?
-   Are there unbounded loops or recursive operations?
-   Is rate limiting implemented on expensive endpoints?

**Mitigations:**

-   Rate limiting on APIs (per user, per IP)
-   Pagination with max page size (e.g., 1000)
-   Timeout on long-running operations
-   Input validation to reject degenerate queries

---

### E – Elevation of Privilege

**Threat:** Attacker gains higher privileges than authorized.

**Questions to Ask:**

-   Can a regular user access admin-only endpoints?
-   Are role checks performed server-side on every request?
-   Could an attacker manipulate tokens to gain admin rights?

**Mitigations:**

-   Server-side authorization checks (never trust client)
-   Principle of least privilege
-   Centralized role/policy enforcement
-   Regular audits of user permissions

---

## Secure-by-Default Design Principles

### 1. Least Privilege

**Principle:** Users/services get minimum permissions needed to perform their function.

**Examples:**

-   Database connection uses read-only account for queries
-   API keys scoped to specific operations (not admin keys everywhere)
-   Users start with zero permissions; explicitly grant access

---

### 2. Fail Closed (Fail Secure)

**Principle:** On error or uncertainty, deny access rather than allow.

**Examples:**

-   Authorization check throws exception → default to 403 Forbidden
-   Cache miss during permission lookup → deny access, log error
-   Token validation fails → reject request (don't fall back to anonymous)

```csharp
// ✅ RIGHT: Fail closed
var isAuthorized = false;
try
{
    isAuthorized = await _authService.CheckPermissionAsync(userId, resource, ct);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Authorization check failed for user {UserId}", userId);
    return Forbid(); // Deny access on error
}

if (!isAuthorized)
    return Forbid();
```

---

### 3. Don't Trust Client Input (Ever)

**Principle:** All input from the client is potentially malicious.

**Examples:**

-   Validate all query parameters, request bodies, headers
-   Don't trust `x-user-role` headers (re-validate server-side)
-   Sanitize file uploads (check type, size, content)

**Validation Checklist:**

-   [ ] **Numeric ranges:** Min/max for integers, floats
-   [ ] **String lengths:** Max length constraints
-   [ ] **Enum validation:** Reject unknown enum values
-   [ ] **Date ranges:** Not before/after bounds
-   [ ] **Required fields:** Non-null, non-empty checks
-   [ ] **Degenerate inputs:** Prevent all-zero IDs, empty filters that scan entire tables

---

### 4. Defense in Depth

**Principle:** Use multiple layers of security so failure of one doesn't compromise the system.

**Layers:**

1. **Network:** Firewall, VPN, network segmentation
2. **Application:** Input validation, authentication, authorization
3. **Data:** Encryption at rest, column-level encryption for PII
4. **Monitoring:** Intrusion detection, anomaly detection, alerting

**Example:** Even if an attacker bypasses the firewall, they still face authentication, authorization, encrypted data, and monitoring alerts.

---

### 5. Separation of Duties

**Principle:** Critical operations require multiple approvals/roles.

**Examples:**

-   Code deployment requires developer + approver
-   Financial transactions require initiator + approver
-   Database schema changes require DBA + architect review

---

### 6. Secure Defaults

**Principle:** Out-of-the-box configuration should be secure.

**Examples:**

-   HTTPS enabled by default (not HTTP)
-   Authentication required by default (not `AllowAnonymous`)
-   Restrictive CORS policy (not `AllowAnyOrigin()`)
-   Minimal logging verbosity in production (not debug mode)

---

## Authentication & Authorization (IAM Pillar)

### Server-Side Validation (CRITICAL)

**Rule:** Always re-validate user permissions server-side. NEVER trust client-provided roles.

```csharp
// ❌ WRONG: Trust client header (SECURITY VULNERABILITY)
var roles = req.Headers["x-impersonating-roles"];
var user = new { Roles = roles.Split(',') };

// ✅ RIGHT: Re-validate against authenticated user
var authenticatedUserId = HttpContext.User.FindFirst("sub")?.Value;
var allowedRoles = await _authService.GetAllowedRolesAsync(authenticatedUserId, ct);
if (!requestedRoles.All(r => allowedRoles.Contains(r)))
    throw new UnauthorizedAccessException("Cannot assume requested roles");
```

### Role-Based Access Control (RBAC)

**Pattern:** Centralize role definitions and policy mappings.

```csharp
// Define policies in one place
public static class Policy
{
    public const string ViewReports = nameof(ViewReports);
    public const string ManageUsers = nameof(ManageUsers);
    public const string AdminOnly = nameof(AdminOnly);
}

// Map policies to roles
public static readonly Dictionary<string, string[]> PolicyRoleMap = new()
{
    { Policy.ViewReports, new[] { "Administrator", "FinanceManager", "ReadOnly" } },
    { Policy.ManageUsers, new[] { "Administrator" } },
    { Policy.AdminOnly, new[] { "Administrator" } }
};

// Apply in endpoints
[Authorize(Policy = Policy.ViewReports)]
public async Task<IActionResult> GetReport() { ... }
```

### Minimal Claims Extraction

**Rule:** Only extract claims you actually need from authentication tokens.

```csharp
// ✅ RIGHT: Extract only necessary claims
var userId = HttpContext.User.FindFirst("sub")?.Value; // User ID
var email = HttpContext.User.FindFirst("email")?.Value; // If needed for audit

// ❌ WRONG: Extract and log unmasked email (PII exposure)
_logger.LogInformation("User {Email} accessed resource", email);

// ✅ RIGHT: Mask before logging
var maskedEmail = MaskEmail(email);
_logger.LogInformation("User {MaskedEmail} accessed resource", maskedEmail);
```

---

## Input Validation & Injection Prevention

### Server-Side Validation (MANDATORY)

**All inputs MUST be validated server-side. Client-side validation is UX only, never security.**

**Validation Rules:**

1. **Type validation:** Ensure correct data type (int, string, enum, date)
2. **Range validation:** Min/max for numbers, dates
3. **Length validation:** Min/max string length
4. **Format validation:** Regex for emails, phone numbers, SSN
5. **Enum validation:** Reject unknown/out-of-range enum values
6. **Collection size:** Max items in arrays/lists
7. **Business rules:** Cross-field validation, state transitions

**Validation Example (FluentValidation):**

```csharp
public class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize must be between 1 and 1000.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => x.SearchTerm != null)
            .WithMessage("SearchTerm must be <= 100 characters.");

        RuleFor(x => x.BadgeNumbers)
            .Must(badges => badges == null || !badges.All(b => b == 0))
            .WithMessage("All-zero badge numbers not allowed (degenerate query).");
    }
}
```

### Parameterized Queries (MANDATORY)

**NEVER build SQL queries by string concatenation or interpolation.**

```csharp
// ❌ WRONG: SQL Injection vulnerability
var query = $"SELECT * FROM Users WHERE Username = '{username}' AND Password = '{password}'";
var user = await db.Database.ExecuteSqlRawAsync(query);

// ✅ RIGHT: ORM parameterization (EF Core)
var user = await db.Users
    .Where(u => u.Username == username)
    .FirstOrDefaultAsync(ct);

// ✅ RIGHT: Explicit parameterization (if raw SQL needed)
var user = await db.Users
    .FromSqlRaw("SELECT * FROM Users WHERE Username = {0}", username)
    .FirstOrDefaultAsync(ct);
```

### Degenerate Query Guards

**Prevent queries that would scan entire tables or cause performance issues.**

```csharp
// ❌ WRONG: Allows all-zero badge numbers (scans entire Demographics table)
var demographics = await db.Demographics
    .Where(d => badgeNumbers.Contains(d.BadgeNumber))
    .ToListAsync(ct);

// ✅ RIGHT: Reject degenerate inputs
if (badgeNumbers.All(b => b == 0))
    throw new ValidationException("All-zero badge numbers not allowed.");

// Then proceed with query
var demographics = await db.Demographics
    .Where(d => badgeNumbers.Contains(d.BadgeNumber))
    .ToListAsync(ct);
```

---

## Data Security & PII Protection

### PII Masking (CRITICAL)

**All Personally Identifiable Information (PII) MUST be masked in logs, telemetry, and error messages.**

**Common PII Fields:**

-   Social Security Numbers (SSN)
-   Email addresses
-   Phone numbers
-   Bank account numbers
-   Full names (in some contexts)
-   Dates of birth

**Masking Patterns:**

```csharp
// SSN: 123-45-6789 → ***-**-6789
public static string MaskSsn(string ssn)
{
    if (string.IsNullOrEmpty(ssn) || ssn.Length < 4)
        return "***";
    return $"***-**-{ssn[^4..]}";
}

// Email: user@company.com → u***@c***.com
public static string MaskEmail(string email)
{
    if (string.IsNullOrEmpty(email) || !email.Contains('@'))
        return "***";
    var parts = email.Split('@');
    return $"{parts[0][0]}***@{parts[1][0]}***.{parts[1].Split('.')[^1]}";
}

// Phone: (555) 123-4567 → (***) ***-4567
public static string MaskPhone(string phone)
{
    if (string.IsNullOrEmpty(phone) || phone.Length < 4)
        return "***";
    return $"(***) ***-{phone[^4..]}";
}
```

**Logging Best Practices:**

```csharp
// ❌ WRONG: Unmasked PII in logs
_logger.LogInformation("Processing employee: SSN {Ssn}, Email {Email}",
    employee.Ssn, employee.Email);

// ✅ RIGHT: Masked PII
_logger.LogInformation("Processing employee: SSN {MaskedSsn}, Email {MaskedEmail}",
    MaskSsn(employee.Ssn), MaskEmail(employee.Email));

// ✅ BETTER: Use structured logging with automatic masking
// (Configure masking operators in logging pipeline)
_logger.LogInformation("Processing employee: SSN {Ssn}, Email {Email}",
    employee.Ssn, employee.Email); // Masking operators handle it
```

### Sensitive Field Access Tracking

**Track when sensitive fields are accessed for security auditing.**

```csharp
// Declare sensitive fields in telemetry calls
await this.ExecuteWithTelemetry(HttpContext, _logger, request, async () =>
{
    var member = await _service.GetMemberAsync(request.Id, ct);
    return member;
}, "Ssn", "DateOfBirth", "Email"); // ← Sensitive fields accessed
```

### Data Classification

**Classify data sensitivity to apply appropriate controls:**

| Classification   | Examples                              | Controls                              |
| ---------------- | ------------------------------------- | ------------------------------------- |
| **Public**       | Product catalog, public announcements | No special protection                 |
| **Internal**     | Business reports, internal memos      | Require authentication                |
| **Confidential** | Employee data, financial records      | Require authorization, mask in logs   |
| **Restricted**   | SSN, passwords, bank accounts         | Encrypt at rest/transit, audit access |

---

## Transport Security

### HTTPS Enforcement (MANDATORY)

**All production traffic MUST use HTTPS (TLS 1.2 or higher).**

**Configuration:**

-   HTTPS termination at load balancer (recommended)
-   Or HTTPS in application with `UseHttpsRedirection()` and `UseHsts()`

```csharp
// Minimal setup (load balancer handles HTTPS)
app.UseHsts(); // HTTP Strict Transport Security

// Full setup (app-level HTTPS)
app.UseHttpsRedirection();
app.UseHsts();
```

### Required Security Headers

**All responses MUST include these security headers:**

```http
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
Content-Security-Policy: default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'
Referrer-Policy: no-referrer
Permissions-Policy: geolocation=(), microphone=(), camera=()
```

**Implementation:**

```csharp
// Use NetEscapades.AspNetCore.SecurityHeaders or middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});
```

### CORS Restrictions

**CORS MUST be restrictive. NEVER use `AllowAnyOrigin()` in production.**

```csharp
// ❌ WRONG: Allow any origin (MITM vulnerability)
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// ✅ RIGHT: Development - localhost only
services.AddCors(options =>
{
    options.AddPolicy("Development", builder =>
    {
        builder.WithOrigins("http://localhost:3100", "http://127.0.0.1:3100")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// ✅ RIGHT: Production - specific domains only
services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins("https://app.example.com", "https://www.example.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```

---

## Error Handling & Information Disclosure

### Generic Error Messages (CRITICAL)

**NEVER expose stack traces, SQL queries, file paths, or internal details in HTTP responses.**

```csharp
// ❌ WRONG: Detailed error message (information disclosure)
return Problem(
    detail: $"NullReferenceException at {ex.StackTrace}",
    statusCode: 500);

// ✅ RIGHT: Generic error message
_logger.LogError(ex, "Error processing request for user {UserId}", userId);
return Problem(
    detail: "An unexpected error occurred. Please contact support.",
    statusCode: 500);
```

### Problem JSON Format (RFC 7807)

**All errors MUST follow Problem JSON structure:**

```json
{
    "type": "https://api.example.com/errors/validation-error",
    "title": "Validation Failed",
    "status": 400,
    "detail": "The 'pageSize' field must be between 1 and 1000.",
    "instance": "/api/search",
    "errors": {
        "pageSize": ["Must be between 1 and 1000"]
    }
}
```

### Correlation IDs (MANDATORY)

**Every error MUST include a correlation ID for tracing without exposing sensitive details.**

```csharp
// Automatic correlation ID from middleware
var correlationId = HttpContext.TraceIdentifier;

// Log with correlation ID
_logger.LogError(ex, "Database error (correlation: {CorrelationId})", correlationId);

// Return generic error with correlation ID
return Problem(
    detail: $"An error occurred. Please provide correlation ID {correlationId} to support.",
    statusCode: 500);
```

---

## Dependency Security

### Monthly Audits (MANDATORY)

**Check for outdated packages and known vulnerabilities monthly (minimum).**

```bash
# .NET
dotnet list package --outdated
dotnet list package --vulnerable

# Node.js
npm audit
npm outdated
```

### Critical CVE Response (MANDATORY)

**Critical and high-severity vulnerabilities MUST be patched within 48 hours.**

**Process:**

1. Identify vulnerability via security advisory or scan
2. Check if vulnerability affects your usage
3. Update to patched version or apply workaround
4. Test thoroughly (unit + integration tests)
5. Deploy emergency patch if production affected

### Dependency Review Checklist

**Before adding a new package:**

-   [ ] **Active maintenance:** Last commit within 6 months
-   [ ] **Community trust:** GitHub stars > 100, multiple contributors
-   [ ] **Known vulnerabilities:** Check CVE databases
-   [ ] **License compatibility:** OSS license compatible with your project
-   [ ] **Minimal dependencies:** Fewer transitive dependencies = smaller attack surface

---

## Detection & Response (Logging & Telemetry)

### Comprehensive Telemetry (MANDATORY)

**All endpoints MUST implement telemetry for security monitoring.**

**What to Log:**

-   Authentication attempts (success, failure)
-   Authorization failures (attempted privilege escalation)
-   Sensitive field access (SSN, email, etc.)
-   Data modifications (create, update, delete)
-   Configuration changes
-   Errors and exceptions

**What NOT to Log:**

-   Unmasked PII (SSN, email, phone, passwords)
-   Authentication tokens or session IDs
-   Full request/response bodies with sensitive data

**Telemetry Pattern:**

```csharp
public override async Task<TResponse> ExecuteAsync(TRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        // Business logic
        var result = await _service.ProcessAsync(req, ct);

        // Business metrics
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "sensitive-data-access"),
            new("endpoint", nameof(MyEndpoint)));

        return result;
    }, "Ssn", "Email"); // ← Declare sensitive fields accessed
}
```

### Alerting Rules

**Configure alerts for suspicious patterns:**

-   **Multiple failed login attempts** (5+ in 5 minutes)
-   **Unauthorized access attempts** (HTTP 403 spike)
-   **Large data exports** (>10MB response size)
-   **Sensitive field access spike** (100+ SSN accesses/hour by single user)
-   **Error rate spike** (>5% error rate for 5 minutes)

---

## Security Testing Checklist

### Authentication & Authorization Tests

-   [ ] Unauthenticated requests rejected (HTTP 401)
-   [ ] Unauthorized access attempts rejected (HTTP 403)
-   [ ] Server-side role validation (not client-provided roles)
-   [ ] Token expiration enforced
-   [ ] Session hijacking prevented

### Input Validation Tests

-   [ ] Invalid inputs rejected (negative numbers, oversized strings)
-   [ ] Boundary cases (min, max, empty, null)
-   [ ] SQL injection attempts blocked
-   [ ] XSS payloads sanitized
-   [ ] Degenerate queries rejected

### Data Security Tests

-   [ ] PII masked in logs
-   [ ] Sensitive fields encrypted at rest (if applicable)
-   [ ] HTTPS enforced (HTTP redirects to HTTPS)
-   [ ] Security headers present
-   [ ] CORS restrictions enforced

### Error Handling Tests

-   [ ] Generic error messages (no stack traces)
-   [ ] Correlation IDs present
-   [ ] HTTP status codes correct (400, 401, 403, 404, 500)

---

## Defense in Depth Strategy

**Defense in Depth = Multiple layers of security so failure of one doesn't compromise the system.**

### Layer 1: Network Security

-   Firewall rules (allow only necessary ports)
-   VPN for internal services
-   Network segmentation (DMZ, internal, data tier)

### Layer 2: Application Security

-   Input validation
-   Authentication & authorization
-   Output encoding
-   Rate limiting

### Layer 3: Data Security

-   Encryption at rest (database, file storage)
-   Encryption in transit (TLS)
-   Column-level encryption for highly sensitive fields

### Layer 4: Monitoring & Detection

-   Centralized logging (SIEM)
-   Intrusion detection systems (IDS)
-   Anomaly detection (ML-based)
-   Security Information and Event Management (SIEM)

### Layer 5: Incident Response

-   Incident response plan documented
-   Regular security drills
-   Post-incident reviews
-   Rollback procedures tested

---

## Quick Security Review Questions

**Ask these during code review:**

1. **Access Control:** Does this endpoint check user permissions server-side?
2. **Input Validation:** Are all user inputs validated (type, range, length)?
3. **PII Protection:** Is sensitive data masked in logs/responses?
4. **Injection Prevention:** Are queries parameterized (no string concatenation)?
5. **Error Handling:** Do errors expose internal details (stack traces, SQL)?
6. **Transport Security:** Is HTTPS enforced with security headers?
7. **Dependency Security:** Are packages up-to-date with no known CVEs?
8. **Telemetry:** Is sensitive field access logged for auditing?
9. **Fail Closed:** Does the code deny access on errors (not allow)?
10. **Defense in Depth:** Are multiple security layers present?

---

## Additional Resources

### OWASP Resources

-   [OWASP Top 10 Web Application Security Risks](https://owasp.org/www-project-top-ten/)
-   [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
-   [OWASP API Security Top 10](https://owasp.org/www-project-api-security/)

### STRIDE Resources

-   [Microsoft Threat Modeling Tool](https://learn.microsoft.com/en-us/azure/security/develop/threat-modeling-tool)
-   [STRIDE Threat Modeling Framework](https://learn.microsoft.com/en-us/azure/security/develop/threat-modeling-tool-threats)

### Security Training

-   [YouTube: "OWASP Top 10 Explained" (search for recent videos)](https://www.youtube.com/results?search_query=owasp+top+10+explained)
-   [YouTube: "Defense in Depth Security Strategy"](https://www.youtube.com/results?search_query=defense+in+depth+security)
-   [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)

---

**Last Updated:** December 23, 2025  
**Maintained By:** Security & Architecture Teams  
**Review Frequency:** Quarterly (or after major security incidents)
