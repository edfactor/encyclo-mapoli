# Validation & Boundary Checks Patterns

Complete reference for implementing input validation and boundary checks in the Profit Sharing application. **Validation is a security and correctness requirement** - never rely solely on client-side checks.

## Overview

All incoming data MUST be validated with explicit boundary checks at both server and client boundaries. This policy applies to **every endpoint and page** that accepts user input.

**Security Rationale**:
- Prevents data exfiltration via unbounded queries
- Protects against denial-of-service via expensive operations
- Ensures data integrity and business rule compliance
- Validates enum values to prevent injection attacks

## Server-Side Validation (MANDATORY)

### Required Validation Rules

All request DTOs must enforce:

✅ **Numeric Ranges**: Min/max for integers/floats (page size, amounts, counts)  
✅ **String Length Limits**: Min/max length and allowed character sets  
✅ **Collection Size Limits**: Max items in array/list payloads  
✅ **Pagination Bounds**: Max page size, max offset/skip  
✅ **File Upload Limits**: Max file size, allowed content types  
✅ **Date/Time Ranges**: Not before/after bounds, timezone normalization  
✅ **Enum Validation**: Reject unknown or out-of-range enum numeric values  
✅ **Required Fields**: Nullability constraints and required fields

### FluentValidation Pattern (Recommended)

Use FluentValidation for declarative, testable validation:

```csharp
// DTO
public class SearchRequest
{
    public int PageSize { get; init; } = 50;
    public int Offset { get; init; }
    public string? Query { get; init; }
}

// FluentValidation validator
public class SearchRequestValidator : AbstractValidator<SearchRequest>
{
    public SearchRequestValidator()
    {
        // Numeric range with sensible limits
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 1000)
            .WithMessage("PageSize must be between 1 and 1000.");

        RuleFor(x => x.Offset)
            .InclusiveBetween(0, 1_000_000)
            .WithMessage("Offset must be between 0 and 1,000,000.");

        // String length when not null
        RuleFor(x => x.Query)
            .MaximumLength(200)
            .WithMessage("Query must be at most 200 characters long.")
            .When(x => x.Query != null);
    }
}
```

**Registration** (Program.cs / DI):
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<SearchRequestValidator>();
```

FastEndpoints will automatically pick up FluentValidation validators and return structured `ValidationProblem` responses.

### Complex Validation Examples

#### Date Range Validation

```csharp
public class DateRangeRequestValidator : AbstractValidator<DateRangeRequest>
{
    public DateRangeRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date.");

        // Business rule: date range cannot exceed 1 year
        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
            .WithMessage("Date range cannot exceed 365 days.")
            .When(x => x.StartDate != default && x.EndDate != default);

        // Business rule: no future dates for historical reports
        RuleFor(x => x.EndDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("End date cannot be in the future.")
            .When(x => x.IsHistoricalReport);
    }
}
```

#### Enum Validation

```csharp
public class EmployeeSearchRequestValidator : AbstractValidator<EmployeeSearchRequest>
{
    public EmployeeSearchRequestValidator()
    {
        // Validate enum is defined
        RuleFor(x => x.EmploymentStatus)
            .IsInEnum()
            .WithMessage("Invalid employment status value.");

        // Additional business rule validation
        RuleFor(x => x.EmploymentStatus)
            .NotEqual(EmploymentStatus.Unknown)
            .WithMessage("Employment status must be specified.");
    }
}
```

#### Collection Size Limits

```csharp
public class BulkUpdateRequestValidator : AbstractValidator<BulkUpdateRequest>
{
    public BulkUpdateRequestValidator()
    {
        RuleFor(x => x.EmployeeIds)
            .NotEmpty()
            .WithMessage("At least one employee ID is required.");

        RuleFor(x => x.EmployeeIds)
            .Must(ids => ids.Count <= 5000)
            .WithMessage("Cannot update more than 5,000 employees at once.");

        // Validate individual IDs
        RuleForEach(x => x.EmployeeIds)
            .GreaterThan(0)
            .WithMessage("Employee IDs must be positive integers.");
    }
}
```

#### File Upload Validation

```csharp
public class FileUploadRequestValidator : AbstractValidator<FileUploadRequest>
{
    private static readonly string[] AllowedExtensions = { ".csv", ".xlsx", ".txt" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public FileUploadRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required.");

        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage($"File size cannot exceed {MaxFileSizeBytes / 1024 / 1024} MB.")
            .When(x => x.File != null);

        RuleFor(x => x.File.FileName)
            .Must(filename => AllowedExtensions.Any(ext => 
                filename.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Only {string.Join(", ", AllowedExtensions)} files are allowed.")
            .When(x => x.File != null);
    }
}
```

### Complex Filter Validation

Prevent unbounded or expensive queries:

```csharp
public class EmployeeFilterRequestValidator : AbstractValidator<EmployeeFilterRequest>
{
    public EmployeeFilterRequestValidator()
    {
        // Require at least one filter to prevent full table scan
        RuleFor(x => x)
            .Must(x => HasAtLeastOneFilter(x))
            .WithMessage("At least one filter criteria is required (SSN, Badge, Name, or Department).");

        // Prevent degenerate badge number filter (badge = 0 matches too many)
        RuleFor(x => x.BadgeNumber)
            .GreaterThan(0)
            .WithMessage("Badge number must be greater than zero.")
            .When(x => x.BadgeNumber.HasValue);

        // Require minimum length for name search to avoid wide scans
        RuleFor(x => x.LastName)
            .MinimumLength(2)
            .WithMessage("Last name search requires at least 2 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));
    }

    private bool HasAtLeastOneFilter(EmployeeFilterRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Ssn) ||
               request.BadgeNumber.HasValue && request.BadgeNumber > 0 ||
               !string.IsNullOrWhiteSpace(request.LastName) ||
               !string.IsNullOrWhiteSpace(request.Department);
    }
}
```

### Custom Validation Logic

For complex business rules:

```csharp
public class ContributionRequestValidator : AbstractValidator<ContributionRequest>
{
    private readonly IProfitSharingDbContext _dbContext;

    public ContributionRequestValidator(IProfitSharingDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Contribution amount must be greater than zero.");

        RuleFor(x => x.ContributionDate)
            .MustAsync(async (request, date, ct) => 
                await IsValidContributionDateAsync(request, date, ct))
            .WithMessage("Contribution date must be after employee hire date and not in the future.");
    }

    private async Task<bool> IsValidContributionDateAsync(
        ContributionRequest request, 
        DateTime contributionDate, 
        CancellationToken ct)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.OracleHcmId == request.EmployeeId, ct);

        if (employee == null)
            return false;

        // Cannot contribute before hire date
        if (contributionDate < employee.HireDate)
            return false;

        // Cannot contribute in the future
        if (contributionDate > DateTime.UtcNow)
            return false;

        return true;
    }
}
```

### Validation Response Format

FastEndpoints automatically returns structured validation errors:

```json
{
  "errors": {
    "PageSize": ["PageSize must be between 1 and 1000."],
    "Query": ["Query must be at most 200 characters long."]
  },
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

## Client-Side Validation (Recommended)

Client-side validation improves UX but **is NOT a substitute** for server-side checks.

### TypeScript Validation

Mirror server-side constraints:

```typescript
interface SearchInput {
  pageSize: number;
  offset: number;
  query?: string;
}

function validateSearchInput(input: SearchInput): string[] {
  const errors: string[] = [];

  // Numeric ranges
  if (input.pageSize < 1 || input.pageSize > 1000) {
    errors.push('Page size must be between 1 and 1000');
  }

  if (input.offset < 0 || input.offset > 1_000_000) {
    errors.push('Offset must be between 0 and 1,000,000');
  }

  // String length
  if (input.query && input.query.length > 200) {
    errors.push('Query must be at most 200 characters long');
  }

  return errors;
}

// Usage in component
const handleSubmit = () => {
  const errors = validateSearchInput(formData);
  if (errors.length > 0) {
    setValidationErrors(errors);
    return;
  }

  // Proceed with API call
  dispatch(searchEmployees(formData));
};
```

### React Hook Form Integration

```typescript
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const searchSchema = z.object({
  pageSize: z.number().min(1).max(1000),
  offset: z.number().min(0).max(1_000_000),
  query: z.string().max(200).optional(),
});

type SearchFormData = z.infer<typeof searchSchema>;

function SearchForm() {
  const { register, handleSubmit, formState: { errors } } = useForm<SearchFormData>({
    resolver: zodResolver(searchSchema),
    defaultValues: {
      pageSize: 50,
      offset: 0,
    },
  });

  const onSubmit = (data: SearchFormData) => {
    // Data is validated - proceed with API call
    dispatch(searchEmployees(data));
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input type="number" {...register('pageSize', { valueAsNumber: true })} />
      {errors.pageSize && <span>{errors.pageSize.message}</span>}
      
      <input type="number" {...register('offset', { valueAsNumber: true })} />
      {errors.offset && <span>{errors.offset.message}</span>}
      
      <input type="text" {...register('query')} />
      {errors.query && <span>{errors.query.message}</span>}
      
      <button type="submit">Search</button>
    </form>
  );
}
```

### UI Guardrails

Prevent users from creating expensive queries:

```typescript
// Pagination controls
const MAX_PAGE_SIZE = 1000;
const DEFAULT_PAGE_SIZE = 50;

// Disable "Export All" if result count exceeds threshold
const canExportAll = totalRecords <= 10000;

// Require filter before enabling search
const isSearchEnabled = hasAtLeastOneFilter(filters);

// Show warning for potentially expensive operations
{dateRange.days > 365 && (
  <Alert severity="warning">
    Date range exceeds 1 year. Consider using a shorter range for better performance.
  </Alert>
)}
```

## Edge Cases to Test

### Required Test Coverage

Every validator MUST have unit tests covering:

✅ **Happy path**: Valid input passes validation  
✅ **Boundary cases**: Min, max, empty, null values  
✅ **Invalid enums**: Unknown or out-of-range enum values  
✅ **Oversized collections**: Arrays exceeding limits (e.g., > 5k items)  
✅ **Very large numbers**: Exceeding database column bounds  
✅ **Date edge cases**: Future dates, dates before business epoch  
✅ **String edge cases**: Empty strings, whitespace-only, very long strings  
✅ **Degenerate inputs**: Null payloads, all-zeros, empty filters

### Example Test Cases

```csharp
public class SearchRequestValidatorTests
{
    private readonly SearchRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new SearchRequest { PageSize = 50, Offset = 0, Query = "test" };
        var result = _validator.Validate(request);
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]      // Below minimum
    [InlineData(-1)]     // Negative
    [InlineData(1001)]   // Above maximum
    public void PageSize_OutOfRange_FailsValidation(int pageSize)
    {
        var request = new SearchRequest { PageSize = pageSize };
        var result = _validator.Validate(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(SearchRequest.PageSize));
    }

    [Fact]
    public void Query_ExceedsMaxLength_FailsValidation()
    {
        var request = new SearchRequest 
        { 
            PageSize = 50, 
            Query = new string('x', 201) // 201 characters
        };
        var result = _validator.Validate(request);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(SearchRequest.Query));
    }

    [Fact]
    public void Offset_Negative_FailsValidation()
    {
        var request = new SearchRequest { PageSize = 50, Offset = -1 };
        var result = _validator.Validate(request);
        result.IsValid.ShouldBeFalse();
    }
}
```

### Integration Test Pattern

```csharp
public class SearchEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SearchEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_WithInvalidPageSize_Returns400BadRequest()
    {
        // Arrange
        var request = new { pageSize = 5000, offset = 0 }; // Exceeds max

        // Act
        var response = await _client.PostAsJsonAsync("/api/search", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("PageSize must be between 1 and 1000");
    }
}
```

## Service-Layer Guards

Keep validation OUT of service methods - validate at the endpoint/boundary:

```csharp
// ❌ WRONG: Validation logic in service
public class EmployeeService
{
    public async Task<Result<Employee>> GetEmployeeAsync(int badgeNumber)
    {
        if (badgeNumber <= 0)
            return Result<Employee>.Failure(Error.InvalidBadgeNumber);

        // Business logic...
    }
}

// ✅ CORRECT: Validation at endpoint, service receives well-formed input
public class GetEmployeeEndpoint : Endpoint<GetEmployeeRequest, Employee>
{
    public override void Configure()
    {
        Get("/api/employees/{badgeNumber}");
        Validator<GetEmployeeRequestValidator>(); // Validates badgeNumber > 0
    }

    public override async Task<Employee> ExecuteAsync(GetEmployeeRequest req, CancellationToken ct)
    {
        // Input is validated - service receives well-formed request
        return await _employeeService.GetEmployeeAsync(req.BadgeNumber);
    }
}
```

## Trimming and Normalization

When normalizing input, document behavior clearly:

```csharp
public class NameSearchRequestValidator : AbstractValidator<NameSearchRequest>
{
    public NameSearchRequestValidator()
    {
        // Trim and normalize before validation
        RuleFor(x => x.LastName)
            .Transform(name => name?.Trim())
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MinimumLength(2)
            .WithMessage("Last name must be at least 2 characters.")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters.");

        // Uppercase SSN for consistency
        RuleFor(x => x.Ssn)
            .Transform(ssn => ssn?.ToUpperInvariant())
            .Matches(@"^\d{3}-\d{2}-\d{4}$")
            .WithMessage("SSN must be in format XXX-XX-XXXX.")
            .When(x => !string.IsNullOrWhiteSpace(x.Ssn));
    }
}
```

## Quality Gates

### Pull Request Requirements

PRs that add new endpoints or UI pages MUST include:

✅ Validation for all incoming inputs  
✅ At least one unit test covering an invalid boundary case  
✅ Integration test verifying 400 response for invalid input  
✅ Documentation of validation rules in PR description

### Code Review Checklist

- [ ] All request DTOs have validators
- [ ] Validators enforce numeric ranges, string lengths, collection sizes
- [ ] Complex filters require minimum criteria to prevent wide scans
- [ ] Enum values are validated
- [ ] Unit tests cover happy path + boundary cases
- [ ] Integration tests verify HTTP 400 responses
- [ ] Client-side validation mirrors server-side constraints (where applicable)
- [ ] Error messages are clear and actionable

## Security Notes

### Preventing Data Exfiltration

Proper boundary checking reduces risk of:

- **Unbounded queries**: Requiring filters prevents full table scans
- **Large result sets**: Page size limits prevent excessive data extraction
- **Enumeration attacks**: Offset limits prevent systematic data extraction

### Combining with Telemetry

Use validation failures as security signals:

```csharp
_logger?.LogWarning(
    "Validation failed for {Endpoint} - {Errors} (User: {UserId})",
    nameof(SearchEndpoint),
    string.Join(", ", validationErrors),
    User.Identity?.Name);

// Emit telemetry for security monitoring
EndpointTelemetry.ValidationFailuresTotal.Add(1,
    new("endpoint", nameof(SearchEndpoint)),
    new("error_type", "boundary_exceeded"));
```

See `TELEMETRY_GUIDE.md` for comprehensive security monitoring patterns.

## Related Documentation

- `TELEMETRY_GUIDE.md` - Security monitoring and alerting
- `READ_ONLY_FUNCTIONALITY.md` - Authorization patterns
- FastEndpoints validation documentation: https://fast-endpoints.com/docs/validation
- FluentValidation documentation: https://docs.fluentvalidation.net/

## Best Practices Summary

### DO:
- ✅ Validate ALL incoming data at the server boundary
- ✅ Use FluentValidation for declarative, testable validators
- ✅ Enforce numeric ranges, string lengths, collection sizes
- ✅ Require filters to prevent expensive queries
- ✅ Return structured 400 responses with field-level errors
- ✅ Mirror server-side constraints in client-side validation
- ✅ Test happy paths AND boundary cases
- ✅ Log validation failures for security monitoring

### DO NOT:
- ❌ Rely solely on client-side validation
- ❌ Put validation logic in service methods
- ❌ Allow unbounded queries or unlimited result sets
- ❌ Accept enum values without validation
- ❌ Skip unit tests for validators
- ❌ Hardcode limits (use configuration)
- ❌ Return generic error messages

---

**Last Updated**: October 2025  
**Maintained By**: Platform Team  
**Questions**: Contact #platform-engineering
