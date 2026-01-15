# Demoulas.Common.Contracts - Shared Contracts and Interfaces

**Package:** `Demoulas.Common.Contracts`  
**Namespace:** `Demoulas.Common.Contracts.*`

This package contains shared Data Transfer Objects (DTOs), interfaces, validators, and configuration models used across all Demoulas.Common packages. It has no business logic and serves as the foundational contracts layer.

## Table of Contents

1. [Interfaces](#interfaces)
2. [Configuration](#configuration)
3. [Request/Response Contracts](#requestresponse-contracts)
4. [Validators](#validators)
5. [Constants](#constants)

---

## Interfaces

### IEnvironmentProvider

**Namespace:** `Demoulas.Common.Contracts.Interfaces`

Provides environment detection capabilities for application configuration.

```csharp
public interface IEnvironmentProvider
{
    bool IsDevelopment();
    bool IsProduction();
    bool IsQA();
    bool IsUAT();
    bool IsTestEnvironment();
    bool IsDevelopmentOrQA();
    bool IsNonProduction();
    string GetEnvironmentName();
}
```

**Usage:**

```csharp
public class MyService
{
    private readonly IEnvironmentProvider _environment;

    public MyService(IEnvironmentProvider environment)
    {
        _environment = environment;
    }

    public async Task ProcessAsync()
    {
        if (_environment.IsProduction())
        {
            // Production-specific logic
        }
        else if (_environment.IsDevelopmentOrQA())
        {
            // Development/QA logic
        }
    }
}
```

### IAppUser

**Namespace:** `Demoulas.Common.Contracts.Interfaces`

Represents the authenticated user in the application.

```csharp
public interface IAppUser
{
    string? EmployeeNumber { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<Claim> Claims { get; }
}
```

### IAppVersionInfo

**Namespace:** `Demoulas.Common.Contracts.Interfaces`

Provides application version information for diagnostics and monitoring.

```csharp
public interface IAppVersionInfo
{
    string Version { get; }
    string ShortGitHash { get; }
    string BuildId { get; }
    DateTimeOffset BuildDateTime { get; }
    string AssemblyVersion { get; }
}
```

**Implemented by:** `Demoulas.Common.Api.Contracts.AppVersionInfo`

### IRolePermissonService

**Namespace:** `Demoulas.Common.Contracts.Interfaces`

Service for role and permission management.

```csharp
public interface IRolePermissonService
{
    Task<IEnumerable<string>> GetPermissionsForRoleAsync(string roleName);
    Task<IEnumerable<string>> GetRolesForUserAsync(string userId);
}
```

### IPermissionService

**Namespace:** `Demoulas.Common.Contracts.Interfaces`

Service for checking user permissions.

```csharp
public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permission);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);
}
```

---

## Configuration

### LoggingConfig

**Namespace:** `Demoulas.Common.Contracts.Configuration`

Unified logging configuration for Demoulas.Common.Logging v2.0+.

```csharp
public class LoggingConfig
{
    public string? ProjectName { get; set; }
    public string? Namespace { get; set; }
    public FileSystemLogConfig? FileSystem { get; set; }
    public SumoLogicConfig? SumoLogic { get; set; }
    public DynatraceConfig? Dynatrace { get; set; }
    public bool UseSensitiveDataMasking { get; set; } = true;
    public ushort? DestructureDepth { get; set; } = 3;
}
```

### OktaConfiguration

**Namespace:** `Demoulas.Common.Contracts.Configuration`

Okta OIDC authentication configuration.

```csharp
public class OktaConfiguration
{
    public string? OktaDomain { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Audience { get; set; }
    public string? Issuer { get; set; }
    public bool? EnableTokenRenewal { get; set; }
}
```

**Example Configuration (appsettings.json):**

```json
{
    "Okta": {
        "OktaDomain": "dev-123456.okta.com",
        "ClientId": "0oa1234567890abcdef",
        "ClientSecret": "secret",
        "Audience": "api://default",
        "Issuer": "https://dev-123456.okta.com/oauth2/default",
        "EnableTokenRenewal": true
    }
}
```

### CultureConfiguration

**Namespace:** `Demoulas.Common.Contracts.Configuration`

Culture and date formatting configuration.

```csharp
public class CultureConfiguration
{
    public string CultureName { get; set; } = "en-US";
    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string TimeFormat { get; set; } = "HH:mm:ss";
}
```

---

## Request/Response Contracts

### PaginationRequestDto

**Namespace:** `Demoulas.Common.Contracts.Contracts.Request`

Base pagination request with validation.

```csharp
public class PaginationRequestDto
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; } = 0;

    [Range(1, 1000)]
    public int Take { get; set; } = 25;

    [Range(0, 3600)]
    public int? QueryTimeoutSeconds { get; set; }

    public string? ResultHash { get; set; }
}
```

**Usage:**

```csharp
public class GetProductsRequest : PaginationRequestDto
{
    public string? CategoryFilter { get; set; }
    public bool? InStock { get; set; }
}
```

### SortedPaginationRequestDto

**Namespace:** `Demoulas.Common.Contracts.Contracts.Request`

Pagination with sorting capabilities.

```csharp
public class SortedPaginationRequestDto : PaginationRequestDto
{
    public string? SortBy { get; set; }
    public bool? IsSortDescending { get; set; }
}
```

**Multi-Sort Support:**

```csharp
// Sort by multiple properties: "Name,-Price,Category"
// Prefix with '-' for descending
public string? SortBy { get; set; } = "Name,-Price";
```

### PaginatedResponseDto<T>

**Namespace:** `Demoulas.Common.Contracts.Contracts.Response`

Standard paginated response wrapper.

```csharp
public class PaginatedResponseDto<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public string? ResultHash { get; set; }
    public bool? IsPartialResult { get; set; }
    public bool? TimeoutOccurred { get; set; }
}
```

**Example Response:**

```json
{
    "data": [
        { "id": 1, "name": "Product A" },
        { "id": 2, "name": "Product B" }
    ],
    "totalCount": 150,
    "skip": 0,
    "take": 25,
    "resultHash": "abc123def456",
    "isPartialResult": false,
    "timeoutOccurred": false
}
```

### StoreDetailDto

**Namespace:** `Demoulas.Common.Contracts.Contracts.Response`

Store information DTO.

```csharp
public class StoreDetailDto
{
    public int StoreId { get; set; }
    public string Name { get; set; }
    public string DisplayName => $"{StoreId} - {Name}";
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}
```

### AccountingPeriodDto

**Namespace:** `Demoulas.Common.Contracts.Contracts.Response`

Accounting period information.

```csharp
public class AccountingPeriodDto
{
    public int Year { get; set; }
    public byte WeekNo { get; set; }
    public byte Period { get; set; }
    public byte Quarter { get; set; }
    public byte PayrollQuarter { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
```

### CurrentUserDto

**Namespace:** `Demoulas.Common.Contracts.Contracts.Response`

Current authenticated user information for API responses.

```csharp
public class CurrentUserDto
{
    public string? EmployeeNumber { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}";
    public IEnumerable<string> Roles { get; set; }
}
```

---

## Validators

### PaginationRequestDtoValidator

**Namespace:** `Demoulas.Common.Contracts.Validators`

FluentValidation validator for pagination requests.

```csharp
public class PaginationRequestDtoValidator : AbstractValidator<PaginationRequestDto>
{
    public PaginationRequestDtoValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be 0 or greater");

        RuleFor(x => x.Take)
            .InclusiveBetween(1, 1000)
            .WithMessage("Take must be between 1 and 1000");

        RuleFor(x => x.QueryTimeoutSeconds)
            .InclusiveBetween(0, 3600)
            .When(x => x.QueryTimeoutSeconds.HasValue)
            .WithMessage("Query timeout must be between 0 and 3600 seconds");
    }
}
```

### SortedPaginationRequestDtoValidator

**Namespace:** `Demoulas.Common.Contracts.Validators`

Validator with multi-sort support.

```csharp
public class SortedPaginationRequestDtoValidator : AbstractValidator<SortedPaginationRequestDto>
{
    public SortedPaginationRequestDtoValidator()
    {
        Include(new PaginationRequestDtoValidator());

        RuleFor(x => x.SortBy)
            .Must(BeValidSortExpression)
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
            .WithMessage("Invalid sort expression");
    }
}
```

---

## Constants

### Telemetry

**Namespace:** `Demoulas.Common.Contracts.Constants`

OpenTelemetry-related constants.

```csharp
public static class Telemetry
{
    public const string ActivitySourceName = "Demoulas.Common";
    public const string MeterName = "Demoulas.Common";
}
```

---

## Usage Examples

### Example 1: Environment-Based Configuration

```csharp
public class DatabaseConfig
{
    private readonly IEnvironmentProvider _environment;

    public DatabaseConfig(IEnvironmentProvider environment)
    {
        _environment = environment;
    }

    public string GetConnectionString()
    {
        if (_environment.IsProduction())
        {
            return "Production connection string";
        }
        else if (_environment.IsQA())
        {
            return "QA connection string";
        }
        else
        {
            return "Development connection string";
        }
    }
}
```

### Example 2: Pagination Endpoint

```csharp
public class GetProductsEndpoint : Endpoint<GetProductsRequest, PaginatedResponseDto<ProductDto>>
{
    public override void Configure()
    {
        Get("/api/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        GetProductsRequest req,
        CancellationToken ct)
    {
        var result = await _productService.GetProductsAsync(req, ct);

        await SendAsync(new PaginatedResponseDto<ProductDto>
        {
            Data = result.Items,
            TotalCount = result.TotalCount,
            Skip = req.Skip,
            Take = req.Take,
            ResultHash = ComputeHash(result.Items)
        });
    }
}
```

### Example 3: Permission Checking

```csharp
public class OrderService
{
    private readonly IPermissionService _permissions;
    private readonly IAppUser _currentUser;

    public async Task<bool> CanApproveOrderAsync(int orderId)
    {
        var employeeNumber = _currentUser.EmployeeNumber;
        return await _permissions.HasPermissionAsync(
            employeeNumber,
            "orders.approve"
        );
    }
}
```

---

## Best Practices

### DTO Design

1. **Keep DTOs simple** - no business logic
2. **Use data annotations** for validation
3. **Nullable reference types** for optional properties
4. **Computed properties** for derived values (e.g., `DisplayName`)

### Validation

1. **Use FluentValidation** for complex validation
2. **Data annotations** for simple required/range checks
3. **Validate at API boundary** before business logic

### Configuration

1. **Use strongly-typed configuration** classes
2. **Provide sensible defaults** where possible
3. **Document required vs optional** settings
4. **Environment-specific** values via IConfiguration

---

## Multi-Targeting Support

This package supports **net8.0, net9.0, and net10.0**. No framework-specific considerations.

---

## Related Documentation

-   [Demoulas.Common.Api Instructions](demoulas.common.api.instructions.md) - API implementation
-   [Demoulas.Common.Data Instructions](demoulas.common.data.instructions.md) - Data access patterns
-   [Demoulas.Common.Logging Instructions](demoulas.common.logging.instructions.md) - Logging configuration
-   [Security Instructions](demoulas.common.security.instructions.md) - Security best practices
-   [Code Review Instructions](code-review.instructions.md) - Review standards
