# RESTful API Guidelines Instructions

**Reference:** [RESTful API Guidelines](https://opensource.zalando.com/restful-api-guidelines/)  
**Project:** Smart Profit Sharing - Demoulas.ProfitSharing.Endpoints  
**Last Updated:** November 8, 2025

This document provides team guidelines for building RESTful API endpoints that conform to Zalando's RESTful API guidelines. These rules apply to all new endpoints and services under `src/services/src/Demoulas.ProfitSharing.Endpoints/`.

---

## Table of Contents

1. [Quick Reference](#quick-reference)
2. [URL & Path Design](#url--path-design)
3. [HTTP Methods & Semantics](#http-methods--semantics)
4. [Request & Response Formats](#request--response-formats)
5. [Security & Authorization](#security--authorization)
6. [Error Handling](#error-handling)
7. [Pagination & Performance](#pagination--performance)
8. [Status Codes](#status-codes)
9. [Headers & Metadata](#headers--metadata)
10. [Backwards Compatibility](#backwards-compatibility)
11. [Documentation & OpenAPI](#documentation--openapi)
12. [New Endpoint Checklist](#new-endpoint-checklist)

---

## Quick Reference

### DO ✅
- Use **resource-oriented design** (nouns, not verbs)
- Use **kebab-case** for path segments: `/profit-sharing-distributions`
- Use **plural** resource names: `/distributions`, `/beneficiaries`
- Use HTTP methods correctly: GET (read), POST (create), PUT (update), DELETE (remove)
- Use **domain-specific** names: `/profit-year-distributions` not just `/distributions`
- Return **JSON objects** as top-level (not arrays)
- Use **snake_case** for JSON properties and query parameters
- Return proper **error responses** with Problem JSON format
- Implement **pagination** on all collection endpoints
- Add **comprehensive telemetry** with business metrics

### DON'T ❌
- Use **verbs in URLs**: ~~`POST /update-enrollment`~~ → `PUT /profit-years/{id}`
- Use **trailing slashes**: ~~`/distributions/`~~ → `/distributions`
- Use **empty path segments**: ~~`GET /`~~ → `GET /resource-name`
- Return **arrays as top-level**: ~~`[ {...}, {...} ]`~~ → `{ "results": [{...}] }`
- Use **mixed case** in paths: ~~`/ProfitYearDistributions`~~ → `/profit-year-distributions`
- Expose **stack traces** in error responses
- Trust **client-provided roles** (always validate server-side)
- Use **URL versioning**: ~~`/v1/distributions`~~ → Use media-type versioning instead
- Skip **input validation** (validate all user input)
- Implement **API-specific authentication** (use Okta OAuth 2.0)

---

## URL & Path Design

### Rule: Resource-Oriented Design (MUST)

**Guideline:** [MUST keep URLs verb-free [141]](https://opensource.zalando.com/restful-api-guidelines/#141)

Use **nouns** (resources) in URLs, not **verbs** (actions). Let HTTP methods indicate the action.

#### ❌ WRONG (Verb-based)
```
POST   /update-enrollment
POST   /final
POST   /disbursement
GET    /search-members
GET    /download-certificates
POST   /validate-checksum
```

#### ✅ RIGHT (Resource-based)
```
PUT    /profit-years/{id}/enrollment           # Update enrollment resource
POST   /year-end-runs                          # Create year-end-run resource
POST   /beneficiary-disbursements              # Create disbursement resource
GET    /members?name=Smith                     # Query members collection
GET    /certificates?status=frozen             # Query certificates with filter
POST   /report-validations                     # Create validation resource
```

**Mapping Actions to Resources:**

| Requirement | Old Pattern | New Pattern | HTTP Method |
|---|---|---|---|
| Initialize enrollment | `POST /update-enrollment` | `PUT /profit-years/{id}/enrollment` | PUT |
| Run year-end process | `POST /final` | `POST /year-end-runs` | POST |
| Create disbursement | `POST /disbursement` | `POST /beneficiary-disbursements` | POST |
| Find members | `GET /search` | `GET /members?filter=...` | GET |
| Download file | `GET /download` | `GET /certificates` + Accept header | GET |

### Rule: Kebab-Case for Path Segments (MUST)

**Guideline:** [MUST use kebab-case for path segments [129]](https://opensource.zalando.com/restful-api-guidelines/#129)

Use hyphens to separate words in URL path segments (NOT underscores, NOT camelCase, NOT PascalCase).

#### ❌ WRONG
```
/profit_year_distributions     # snake_case
/ProfitYearDistributions       # PascalCase
/profitYearDistributions       # camelCase
/profityeardistributions       # No separators
```

#### ✅ RIGHT
```
/profit-year-distributions
/year-end-runs
/beneficiary-disbursements
/master-inquiry-results
```

**C# Example:**
```csharp
public override void Configure()
{
    // ✅ Correct: kebab-case with lowercase
    Get("profit-year-distributions");
    Post("beneficiary-disbursements");
    Put("members/{id}/enrollment");
    Delete("distributions/{id}");
}
```

### Rule: Pluralize Resource Names (MUST)

**Guideline:** [MUST pluralize resource names [134]](https://opensource.zalando.com/restful-api-guidelines/#134)

Collections use plural; items use singular identifiers.

#### ❌ WRONG
```
/distribution           # Singular for collection
/member/{id}           # Singular
/beneficiary           # Singular
```

#### ✅ RIGHT
```
/distributions         # Plural for collection
/distributions/{id}    # Plural with ID
/members              # Plural for collection
/members/{id}         # Plural with ID
/beneficiaries        # Plural
```

**C# Endpoint Example:**
```csharp
// BEFORE: Singular (wrong)
public class CreateDistributionEndpoint : ProfitSharingEndpoint<...>
{
    public override void Configure()
    {
        Post("/");                      // ❌ Empty path; unclear
        Group<DistributionGroup>();    // ❌ Singular group
    }
}
// URL: POST /distribution

// AFTER: Plural (correct)
public class CreateDistributionEndpoint : ProfitSharingEndpoint<...>
{
    public override void Configure()
    {
        Post("");                       // ✅ Empty path means group path
        Group<DistributionsGroup>();   // ✅ Plural group
    }
}
// URL: POST /distributions
```

### Rule: Domain-Specific Resource Names (MUST)

**Guideline:** [MUST use domain-specific resource names [142]](https://opensource.zalando.com/restful-api-guidelines/#142)

Use names from your business domain, not generic terms.

#### ❌ TOO GENERIC
```
/items
/records
/data
/entries
```

#### ✅ DOMAIN-SPECIFIC
```
/profit-sharing-distributions
/year-end-participants
/member-enrollments
/beneficiary-disbursements
/profit-calculations
```

**Naming Convention for This Project:**

| Business Object | Resource Name | Example Path |
|---|---|---|
| Profit Distribution | `profit-sharing-distributions` or `distributions` (in context) | `/distributions/{id}` |
| Year-End Run | `year-end-runs` | `/year-end-runs` |
| Member | `members` | `/members/{id}` |
| Beneficiary | `beneficiaries` | `/beneficiaries/{id}` |
| Disbursement | `beneficiary-disbursements` | `/beneficiary-disbursements` |
| Member Enrollment | `enrollments` (in context of profit-year) | `/profit-years/{id}/enrollment` |

### Rule: Identify Sub-Resources via Path Segments (MUST)

**Guideline:** [MUST identify resources and sub-resources via path segments [143]](https://opensource.zalando.com/restful-api-guidelines/#143)

Use nested paths for one-to-many relationships.

#### ✅ ONE-TO-MANY RELATIONSHIPS
```
GET    /members/{member-id}/enrollments           # All enrollments for a member
POST   /members/{member-id}/enrollments           # Create enrollment
GET    /members/{member-id}/enrollments/{id}      # Specific enrollment
DELETE /members/{member-id}/enrollments/{id}      # Delete enrollment
```

#### ✅ ONE-TO-ONE RELATIONSHIPS
```
GET    /members/{id}/contact                      # Member's contact (singular)
PUT    /members/{id}/contact                      # Update member's contact
```

#### ✅ ALTERNATIVELY: Flat Structure with Foreign Keys
```
GET    /enrollments?member_id=123                 # Filter by member
GET    /contacts?member_id=123                    # Filter by member
```

**Guideline:** [SHOULD limit number of sub-resource levels [147]](https://opensource.zalando.com/restful-api-guidelines/#147)

Maximum **≤ 3 levels** of nesting.

#### ✅ GOOD (2 levels)
```
/profit-years/{id}/members/{id}/enrollments
```

#### ❌ AVOID (4+ levels)
```
/profit-years/{id}/members/{id}/enrollments/{id}/history/{id}/details
```

---

## HTTP Methods & Semantics

### Rule: Use HTTP Methods Correctly (MUST)

**Guideline:** [MUST use HTTP methods correctly [148]](https://opensource.zalando.com/restful-api-guidelines/#148)

#### GET - Safe, Idempotent, Cacheable
```csharp
public override void Configure()
{
    Get("members/{id}");
    Get("distributions");
    Get("distributions?status=active");
}
```

**Characteristics:**
- ✅ Does not modify server state
- ✅ Can be called multiple times safely
- ✅ Results can be cached
- ✅ Can include query parameters
- ❌ Cannot include request body (or body must be ignored)

#### POST - Creates Resource (NOT idempotent by default)
```csharp
public override void Configure()
{
    Post("distributions");       // Create new distribution
    Post("members");            // Create new member
    Post("report-validations"); // Create validation task
}
```

**Characteristics:**
- ✅ Creates new resource (server generates ID)
- ✅ Typically returns 201 (Created)
- ❌ NOT idempotent (same call = multiple resources)
- ✅ Can include request body

**Response Codes:**
- `201 Created` - Resource created (include Location header with new resource URI)
- `200 OK` - Idempotent POST (if designed that way)
- `202 Accepted` - Asynchronous processing started

#### PUT - Updates Resource (Idempotent)
```csharp
public override void Configure()
{
    Put("members/{id}");              // Update full member record
    Put("profit-years/{id}");         // Update profit year
    Put("distributions/{id}");        // Replace distribution
}
```

**Characteristics:**
- ✅ Updates existing resource
- ✅ Client provides all required fields
- ✅ Idempotent (same call = same result)
- ✅ Returns 200 (OK) or 204 (No Content)
- ✅ Can include request body

**Response Codes:**
- `200 OK` - Resource updated; return updated resource
- `204 No Content` - Resource updated; no response body
- `404 Not Found` - Resource doesn't exist

#### DELETE - Removes Resource (Idempotent)
```csharp
public override void Configure()
{
    Delete("distributions/{id}");     // Remove distribution
    Delete("members/{id}");           // Remove member
    Delete("beneficiaries/{id}");     // Remove beneficiary
}
```

**Characteristics:**
- ✅ Removes resource
- ✅ Idempotent (DELETE twice = same result)
- ✅ Returns 200 (OK), 204 (No Content), or 202 (Accepted)
- ❌ Typically no request body

**Response Codes:**
- `200 OK` - Resource deleted; return deleted resource
- `204 No Content` - Resource deleted; no response body
- `202 Accepted` - Deletion queued (asynchronous)
- `404 Not Found` - Resource doesn't exist

#### PATCH - Partial Update (with JSON Merge Patch)
```csharp
public override void Configure()
{
    Patch("members/{id}");      // Partial update of member
}
```

**Use PATCH when:**
- Only updating specific fields
- Using JSON Merge Patch (`application/merge-patch+json`)

**Prefer PUT** when:
- Updating entire resource
- Client provides complete object
- Simple semantics needed

### Rule: Fulfill Common Method Properties (MUST)

**Guideline:** [MUST fulfill common method properties [149]](https://opensource.zalando.com/restful-api-guidelines/#149)

Methods MUST be **safe**, **idempotent**, and **cacheable** as defined:

| Method | Safe | Idempotent | Cacheable |
|--------|------|-----------|-----------|
| GET    | ✅ Yes | ✅ Yes | ✅ Yes |
| HEAD   | ✅ Yes | ✅ Yes | ✅ Yes |
| POST   | ❌ No | ⚠️ Design for idempotency | ⚠️ Only if safe |
| PUT    | ❌ No | ✅ Yes | ❌ No |
| PATCH  | ❌ No | ⚠️ Design for idempotency | ❌ No |
| DELETE | ❌ No | ✅ Yes | ❌ No |

**Design for Idempotent POST:**

If same POST request may be called multiple times, design it as idempotent:

```csharp
// ✅ Idempotent POST using secondary key
public class CreateDistributionEndpoint : ProfitSharingEndpoint<CreateDistributionRequest, DistributionResponse>
{
    public override void Configure()
    {
        Post("distributions");
    }
    
    public override async Task<DistributionResponse> ExecuteAsync(CreateDistributionRequest req, CancellationToken ct)
    {
        // Use secondary key (e.g., idempotency-key in header) to detect duplicates
        var existingDistribution = await _service.FindByIdempotencyKeyAsync(req.IdempotencyKey, ct);
        if (existingDistribution != null)
            return existingDistribution;  // Return existing (idempotent)
        
        return await _service.CreateAsync(req, ct);
    }
}
```

---

## Request & Response Formats

### Rule: Use JSON as Payload Format (MUST)

**Guideline:** [MUST use JSON as payload data interchange format [167]](https://opensource.zalando.com/restful-api-guidelines/#167)

All request and response bodies MUST be JSON.

### Rule: Use Snake_Case for JSON Properties (MUST)

**Guideline:** [MUST property names must be snake_case (and never camelCase) [118]](https://opensource.zalando.com/restful-api-guidelines/#118)

Configure your JSON serializer to output snake_case:

#### C# Configuration
```csharp
// In Program.cs or appsettings.json
services.AddFastEndpoints(x =>
{
    x.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// OR explicit configuration
services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.WriteIndented = true;
});
```

#### Example
```csharp
// C# class (PascalCase)
public class MemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string SocialSecurityNumber { get; set; }
}

// JSON output (snake_case) ✅
{
  "id": 123,
  "first_name": "John",
  "last_name": "Doe",
  "social_security_number": "***-**-6789"
}

// NOT (camelCase) ❌
{
  "id": 123,
  "firstName": "John",
  "lastName": "Doe",
  "socialSecurityNumber": "***-**-6789"
}
```

### Rule: Always Return JSON Objects as Top-Level (MUST)

**Guideline:** [MUST always return JSON objects as top-level data structures [110]](https://opensource.zalando.com/restful-api-guidelines/#110)

Return objects (with `{ }` braces), NOT arrays.

#### ❌ WRONG
```json
[
  { "id": 1, "name": "Distribution 1" },
  { "id": 2, "name": "Distribution 2" }
]
```

#### ✅ RIGHT
```json
{
  "results": [
    { "id": 1, "name": "Distribution 1" },
    { "id": 2, "name": "Distribution 2" }
  ]
}
```

**Reason:** Allows for future expansion (pagination, metadata, etc.) without breaking clients.

**C# Pattern:**
```csharp
// ❌ Wrong
public class ListDistributionsEndpoint : Endpoint<EmptyRequest, List<DistributionDto>>
{
    // Returns array directly - not extensible
}

// ✅ Correct
public class ListDistributionsEndpoint : Endpoint<EmptyRequest, DistributionListResponse>
{
    // Returns object with results array
}

public class DistributionListResponse
{
    [JsonPropertyName("results")]
    public List<DistributionDto> Results { get; set; }
    
    // Future: Add pagination, metadata, etc.
    public PaginationMetadata Pagination { get; set; }
}
```

### Rule: Use Snake_Case for Query Parameters (MUST)

**Guideline:** [MUST use snake_case (never camelCase) for query parameters [130]](https://opensource.zalando.com/restful-api-guidelines/#130)

Query parameters should be snake_case:

#### ❌ WRONG
```
GET /members?profitYear=2024&memberType=employee
GET /members?firstName=John&lastName=Doe
```

#### ✅ RIGHT
```
GET /members?profit_year=2024&member_type=employee
GET /members?first_name=John&last_name=Doe
```

**C# FastEndpoints Configuration:**
```csharp
public class SearchMembersRequest
{
    [QueryParam]
    [JsonPropertyName("profit_year")]
    public int ProfitYear { get; set; }
    
    [QueryParam]
    [JsonPropertyName("member_type")]
    public string MemberType { get; set; }  // "employee" or "beneficiary"
}
```

---

## Security & Authorization

### Rule: Secure All Endpoints (MUST)

**Guideline:** [MUST secure endpoints [104]](https://opensource.zalando.com/restful-api-guidelines/#104)

Every endpoint MUST be protected with authentication and authorization.

#### FastEndpoints Pattern
```csharp
public override void Configure()
{
    Get("members");
    Policies("CanViewMembers");  // ✅ Apply policy
    AllowAnonymous();             // ❌ Never do this
}
```

#### Policy-Based Authorization (REQUIRED)
```csharp
// Define in Security/Policies.cs
public static class Policy
{
    public const string CanViewDistributions = nameof(CanViewDistributions);
    public const string CanManageDistributions = nameof(CanManageDistributions);
    public const string CanViewReports = nameof(CanViewReports);
}

// Configure in Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.CanViewDistributions, policy =>
        policy.RequireRole("ADMINISTRATOR", "FINANCEMANAGER", "READONLY"));
    
    options.AddPolicy(Policy.CanManageDistributions, policy =>
        policy.RequireRole("ADMINISTRATOR", "FINANCEMANAGER"));
});

// Use in endpoints
public override void Configure()
{
    Get("distributions");
    Policies(Policy.CanViewDistributions);  // ✅ Enforced
}
```

### Rule: Validate Authorization Server-Side (MUST - CRITICAL)

**CRITICAL SECURITY REQUIREMENT:** Always re-validate user permissions server-side. NEVER trust client-provided roles.

```csharp
// ❌ WRONG: Trust client headers (SECURITY VULNERABILITY)
var roles = req.Headers["x-impersonating-roles"];
var user = new { Roles = roles.Split(',') };

// ✅ CORRECT: Re-validate against authenticated user
public override async Task<IResult> ExecuteAsync(ImpersonationRequest req, CancellationToken ct)
{
    // Get authenticated user from context
    var authenticatedUserId = HttpContext.User.FindFirst("sub")?.Value;
    
    // Query database for user's allowed roles
    var allowedRoles = await _authService.GetAllowedRolesAsync(authenticatedUserId, ct);
    
    // Validate requested roles are subset of allowed
    if (!req.RequestedRoles.All(r => allowedRoles.Contains(r)))
        throw new UnauthorizedAccessException("Cannot assume requested roles");
    
    // Proceed with impersonation context
    ...
}
```

See: `COPILOT_INSTRUCTIONS.md` - Authentication & Authorization (A01/A07)

---

## Error Handling

### Rule: Support Problem JSON Format (MUST)

**Guideline:** [MUST support problem JSON [176]](https://opensource.zalando.com/restful-api-guidelines/#176)

All error responses MUST follow Problem JSON schema (RFC 7807).

#### Problem JSON Structure
```json
{
  "type": "https://api.example.com/errors/validation-error",
  "title": "Validation Failed",
  "status": 400,
  "detail": "The 'profit_year' field must be between 2000 and 2099",
  "instance": "/distributions/123"
}
```

#### C# Pattern
```csharp
public override async Task<Results<Ok<T>, BadRequest, ProblemHttpResult>> ExecuteAsync(
    TRequest req, CancellationToken ct)
{
    // ✅ Use domain errors
    var result = await _service.GetAsync(req.Id, ct);
    
    if (!result.IsSuccess)
    {
        return result.Error.ErrorCode switch
        {
            ErrorCode.NotFound => TypedResults.NotFound(),
            ErrorCode.ValidationFailed => TypedResults.BadRequest(),
            _ => TypedResults.Problem(
                detail: result.Error.Message,
                statusCode: 500)
        };
    }
    
    return TypedResults.Ok(result.Value);
}
```

### Rule: Never Expose Stack Traces (MUST)

**Guideline:** [MUST not expose stack traces [177]](https://opensource.zalando.com/restful-api-guidelines/#177)

**CRITICAL SECURITY:** Never include stack traces, SQL queries, or internal details in HTTP responses.

#### ❌ WRONG
```json
{
  "error": "NullReferenceException at line 42",
  "stack_trace": "at MasterInquiryService.GetMembers() in MasterInquiryService.cs:line 42",
  "sql": "SELECT * FROM MEMBERS WHERE ID = NULL"
}
```

#### ✅ RIGHT
```json
{
  "type": "https://api.example.com/errors/member-not-found",
  "title": "Member Not Found",
  "status": 404,
  "detail": "Member with ID 99999 does not exist"
}
```

**C# Logging Pattern:**
```csharp
// ✅ Log internally (safe)
_logger.LogError(ex, "Error processing member {MemberId}: {ErrorMessage}",
    memberId, ex.Message);

// ✅ Return generic message to client
return Results.Problem(
    detail: "An unexpected error occurred. Please contact support.",
    statusCode: 500);
```

---

## Pagination & Performance

### Rule: Support Pagination on Collections (MUST)

**Guideline:** [MUST support pagination [159]](https://opensource.zalando.com/restful-api-guidelines/#159)

Every collection endpoint MUST support pagination.

#### Query Parameters
```
GET /distributions?limit=20&cursor=abc123
```

- `limit` - Number of results to return (default: 20, max: 1000)
- `cursor` - Pagination cursor (opaque string for next/previous page)

#### Response Structure
```json
{
  "results": [ {...}, {...} ],
  "pagination": {
    "cursor": "next_page_cursor_xyz",
    "limit": 20,
    "has_more": true
  }
}
```

#### C# Pattern
```csharp
public class PaginatedResponseDto<T>
{
    [JsonPropertyName("results")]
    public List<T> Results { get; set; }
    
    [JsonPropertyName("pagination")]
    public PaginationMetadata Pagination { get; set; }
}

public class PaginationMetadata
{
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
    
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
    
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}

// In endpoint
public class ListDistributionsEndpoint : 
    Endpoint<ListDistributionsRequest, PaginatedResponseDto<DistributionDto>>
{
    public override void Configure()
    {
        Get("distributions");
    }
    
    public override async Task<PaginatedResponseDto<DistributionDto>> ExecuteAsync(
        ListDistributionsRequest req, CancellationToken ct)
    {
        var limit = Math.Min(req.Limit ?? 20, 1000);
        
        var (items, nextCursor, hasMore) = 
            await _service.GetPaginatedAsync(req.Cursor, limit, ct);
        
        return new PaginatedResponseDto<DistributionDto>
        {
            Results = items,
            Pagination = new()
            {
                Cursor = nextCursor,
                Limit = limit,
                HasMore = hasMore
            }
        };
    }
}
```

### Rule: Prefer Cursor-Based Pagination (SHOULD)

**Guideline:** [SHOULD prefer cursor-based pagination, avoid offset-based [160]](https://opensource.zalando.com/restful-api-guidelines/#160)

**Why cursors over offset:**
- ✅ Handles data changes between requests (inserts/deletes)
- ✅ Efficient for large datasets
- ✅ No N+1 query issues

**Don't use offset-based:**
```
❌ GET /members?offset=100&limit=20  # Bad - prone to gaps/duplicates
```

**Use cursor-based:**
```
✅ GET /members?cursor=abc123&limit=20  # Good - stable pagination
```

---

## Status Codes

### Rule: Use Proper HTTP Status Codes (MUST)

**Guideline:** [MUST use official HTTP status codes [243]](https://opensource.zalando.com/restful-api-guidelines/#243)

#### Success Codes (2xx)

| Code | Method | Meaning |
|------|--------|---------|
| 200 | GET, PUT, PATCH | Success; resource returned |
| 201 | POST | Created; return new resource + Location header |
| 204 | PUT, PATCH, DELETE | Success; no content |
| 202 | POST, PUT, PATCH, DELETE | Accepted; async processing |

```csharp
// GET - Return resource
public override void Configure()
{
    Get("members/{id}");
}
// Returns 200 OK with member details

// POST - Create and return 201
public override void Configure()
{
    Post("distributions");
}
// Returns 201 Created with new distribution

// DELETE - Return 204
public override void Configure()
{
    Delete("distributions/{id}");
}
// Returns 204 No Content
```

#### Client Error Codes (4xx)

| Code | Meaning | When to Use |
|------|---------|------------|
| 400 | Bad Request | Invalid input, validation failed |
| 401 | Unauthorized | Missing/invalid authentication |
| 403 | Forbidden | Authenticated but not authorized |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Concurrent update conflict (optimistic lock) |
| 422 | Unprocessable Entity | Semantically invalid (e.g., invalid state transition) |
| 429 | Too Many Requests | Rate limit exceeded |

```csharp
// Validation error -> 400
if (!request.IsValid)
    return Results.BadRequest(new { error = "Invalid request" });

// Unauthorized -> 401
if (!authenticatedUser)
    return Results.Unauthorized();

// Forbidden -> 403
if (!userHasPermission)
    return Results.Forbid();

// Not found -> 404
if (member == null)
    return Results.NotFound();
```

#### Server Error Codes (5xx)

| Code | Meaning | When to Use |
|------|---------|------------|
| 500 | Internal Server Error | Unhandled exception, bug |
| 502 | Bad Gateway | Upstream service error |
| 503 | Service Unavailable | Maintenance, overload |

---

## Headers & Metadata

### Rule: Support X-Flow-ID (MUST)

**Guideline:** [MUST support X-Flow-ID [233]](https://opensource.zalando.com/restful-api-guidelines/#233)

All requests/responses MUST include X-Flow-ID for correlation.

```csharp
// Request header
GET /members HTTP/1.1
X-Flow-ID: 550e8400-e29b-41d4-a716-446655440000

// Response header
HTTP/1.1 200 OK
X-Flow-ID: 550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json
```

**C# Implementation (via middleware or base endpoint):**
```csharp
public override async Task<IResult> ExecuteAsync(TRequest req, CancellationToken ct)
{
    var flowId = HttpContext.Request.Headers["X-Flow-ID"].ToString();
    if (string.IsNullOrEmpty(flowId))
    {
        flowId = Guid.NewGuid().ToString();
    }
    
    // Include in response
    HttpContext.Response.Headers["X-Flow-ID"] = flowId;
    
    // Use in logging
    using var scope = _logger.BeginScope(new { FlowId = flowId });
    _logger.LogInformation("Processing request");
    
    return await base.ExecuteAsync(req, ct);
}
```

### Rule: Use Kebab-Case for Header Names (SHOULD)

**Guideline:** [SHOULD use kebab-case with uppercase separate words for HTTP headers [132]](https://opensource.zalando.com/restful-api-guidelines/#132)

```
✅ X-Flow-ID
✅ Content-Type
✅ X-Request-Id

❌ x_flow_id (snake_case)
❌ xFlowId (camelCase)
```

---

## Backwards Compatibility

### Rule: Never Break Backwards Compatibility (MUST)

**Guideline:** [MUST not break backward compatibility [106]](https://opensource.zalando.com/restful-api-guidelines/#106)

When changing APIs:

**DO:**
- ✅ Add optional fields
- ✅ Add new endpoints
- ✅ Deprecate old endpoints (with warning)
- ✅ Support both old and new for 6+ months

**DON'T:**
- ❌ Remove fields
- ❌ Change field types
- ❌ Change HTTP methods for existing endpoints
- ❌ Change URL structure without migration path

**Example: Adding a Field**
```csharp
// Old version (still supported)
public class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// New version (backward compatible)
public class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [JsonPropertyName("employment_status")]
    public string EmploymentStatus { get; set; } = "active";  // ✅ Optional with default
}
```

### Rule: Use Semantic Versioning (MUST)

**Guideline:** [MUST use semantic versioning [116]](https://opensource.zalando.com/restful-api-guidelines/#116)

Version format: `MAJOR.MINOR.PATCH`

- `MAJOR` - Breaking changes
- `MINOR` - Backwards-compatible additions
- `PATCH` - Bug fixes

Example: `1.2.3`

**Document in OpenAPI:**
```yaml
info:
  title: Profit Sharing API
  version: 1.2.3  # ✅ Semantic versioning
```

---

## Documentation & OpenAPI

### Rule: Publish OpenAPI Specification (MUST)

**Guideline:** [MUST publish OpenAPI specification for APIs [192]](https://opensource.zalando.com/restful-api-guidelines/#192)

OpenAPI 3.1 specification MUST be published for all APIs.

**File location:** `/src/services/Demoulas.ProfitSharing.openapi.yaml`

**Minimal Example:**
```yaml
openapi: 3.1.0
info:
  title: Profit Sharing API
  version: 1.0.0
  x-api-id: profit-sharing-api-uuid
  x-audience: component-internal
  description: |
    Core API for profit sharing calculations and member management.
    
    Authentication: OAuth 2.0 Bearer (Okta)
    Roles: ADMINISTRATOR, FINANCEMANAGER, READONLY

servers:
  - url: https://api.example.com
    description: Production

paths:
  /distributions:
    get:
      summary: List distributions
      operationId: listDistributions
      parameters:
        - name: profit_year
          in: query
          schema:
            type: integer
        - name: limit
          in: query
          schema:
            type: integer
            default: 20
      responses:
        '200':
          description: Success
          content:
            application/json:
              schema:
                type: object
                properties:
                  results:
                    type: array
                    items:
                      $ref: '#/components/schemas/Distribution'
                  pagination:
                    $ref: '#/components/schemas/Pagination'

    post:
      summary: Create distribution
      operationId: createDistribution
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateDistributionRequest'
      responses:
        '201':
          description: Created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Distribution'
        '400':
          description: Bad Request
          content:
            application/problem+json:
              schema:
                $ref: '#/components/schemas/Problem'

components:
  schemas:
    Distribution:
      type: object
      properties:
        id:
          type: integer
        profit_year:
          type: integer
        gross_amount:
          type: number
          format: decimal

    Problem:
      type: object
      properties:
        type:
          type: string
        title:
          type: string
        status:
          type: integer
        detail:
          type: string
```

---

## New Endpoint Checklist

Use this checklist when creating any new endpoint to ensure compliance:

### Design Phase
- [ ] **Resource-based URL** - Uses nouns (not verbs)
  - Example: `POST /beneficiary-disbursements` not `POST /create-disbursement`
- [ ] **Correct HTTP method** - GET/POST/PUT/DELETE match semantics
- [ ] **Kebab-case path** - All path segments lowercase with hyphens
  - Example: `/profit-year-distributions/{id}` not `/ProfitYearDistributions/{id}`
- [ ] **Plural resource names** - Collections use plural
  - Example: `/distributions/{id}` not `/distribution/{id}`
- [ ] **Domain-specific names** - Use business terminology
  - Example: `/beneficiary-disbursements` not `/payments`
- [ ] **≤3 nesting levels** - Don't over-nest resources

### API Design
- [ ] **Status codes defined** - 200, 201, 204, 400, 404, 500 documented
- [ ] **Request/response DTOs** - Clear, documented structure
- [ ] **Pagination implemented** - Collection endpoints support `limit` and `cursor`
- [ ] **Error handling** - Problem JSON format for errors
- [ ] **Security policy** - Endpoint has appropriate authorization

### Implementation
- [ ] **Snake_case JSON** - JSON serializer configured for snake_case
- [ ] **Snake_case query params** - Query parameters use snake_case
- [ ] **Top-level JSON objects** - Never return arrays at root
- [ ] **Telemetry added** - Business metrics and sensitive fields declared
- [ ] **Logger injected** - `ILogger<T>` in constructor for correlation
- [ ] **Input validation** - FluentValidation rules defined

### Documentation
- [ ] **Summary provided** - Endpoint `Summary()` property populated
- [ ] **Description detailed** - Explain purpose and usage
- [ ] **Example request** - `ExampleRequest` property shown
- [ ] **Example responses** - Response examples for each status code
- [ ] **OpenAPI updated** - Specification includes new endpoint

### Security Review
- [ ] **Endpoint secured** - Policy applied via `Policies()`
- [ ] **Authentication required** - Not marked `AllowAnonymous`
- [ ] **Server-side validation** - Roles validated server-side
- [ ] **No PII in logs** - Sensitive fields masked in telemetry
- [ ] **No stack traces exposed** - Error responses generic

### Testing
- [ ] **Unit tests** - Happy path and error cases
- [ ] **Integration tests** - HTTP method semantics verified
- [ ] **Authorization tests** - Policy enforcement tested
- [ ] **Security tests** - Unauthenticated/unauthorized access blocked

---

## References

- [Zalando RESTful API Guidelines](https://opensource.zalando.com/restful-api-guidelines/)
- Project: [COPILOT_INSTRUCTIONS.md](../copilot-instructions.md)
- Architecture: [Architecture Overview](../../README.md)
- Related: [endpoints.instructions.md](endpoints.instructions.md)

---

## Quick Links by Rule Number

| Rule | Title |
|------|-------|
| [104] | MUST secure endpoints |
| [105] | MUST define and assign permissions |
| [110] | MUST always return JSON objects as top-level |
| [116] | MUST use semantic versioning |
| [118] | MUST property names must be snake_case |
| [129] | MUST use kebab-case for path segments |
| [130] | MUST use snake_case for query parameters |
| [134] | MUST pluralize resource names |
| [141] | MUST keep URLs verb-free |
| [142] | MUST use domain-specific resource names |
| [143] | MUST identify resources and sub-resources |
| [147] | SHOULD limit number of sub-resource levels |
| [148] | MUST use HTTP methods correctly |
| [159] | MUST support pagination |
| [160] | SHOULD prefer cursor-based pagination |
| [167] | MUST use JSON as payload |
| [176] | MUST support problem JSON |
| [177] | MUST not expose stack traces |
| [192] | MUST publish OpenAPI specification |
| [215] | MUST provide API identifiers |
| [219] | MUST provide API audience |
| [233] | MUST support X-Flow-ID |
| [243] | MUST use official HTTP status codes |

---

**Last Updated:** November 8, 2025  
**Status:** Active  
**Maintained By:** Architecture Team
