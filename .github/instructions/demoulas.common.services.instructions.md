---
applyTo: "src/services/src/**/*.*"
paths: "src/services/src/**/*.*"
---

# Demoulas Common Services - Extension Methods Guide

This document provides a comprehensive guide to all extension methods available in the `demoulas.common` solution. These extensions enhance functionality across various .NET types and services.

## Table of Contents

1. [String Extensions](#string-extensions)
2. [DateTime Extensions](#datetime-extensions)
3. [DateOnly Extensions](#dateonly-extensions)
4. [Object Extensions](#object-extensions)
5. [IQueryable Extensions](#iqueryable-extensions)
6. [HttpRequestMessage Extensions](#httprequestmessage-extensions)
7. [Environment Extensions](#environment-extensions)
8. [DbContext Extensions](#dbcontext-extensions)
9. [Database Context Configuration](#database-context-configuration)
10. [Pagination Extensions](#pagination-extensions)
11. [API Endpoint Extensions](#api-endpoint-extensions)
12. [Exception Extensions](#exception-extensions)
13. [Middleware Extensions](#middleware-extensions)
14. [Swagger Extensions](#swagger-extensions)
15. [PDF Extensions](#pdf-extensions)

---

## String Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `StringExtension`

### FirstCharToUpper()

Converts the first character of a string to uppercase while leaving the rest unchanged.

**Syntax:**

```csharp
public static string FirstCharToUpper(this string input)
```

**Parameters:**

- `input` (string): The input string to process

**Returns:** A string with its first character converted to uppercase

**Exceptions:**

- `ArgumentNullException`: Thrown when input is null
- `ArgumentException`: Thrown when input is empty

**Example:**

```csharp
string text = "hello world";
string result = text.FirstCharToUpper(); // "Hello world"
```

**Performance Note:** The .NET 9 implementation uses `string.Create` for a single allocation, providing 10-15% performance improvement over earlier versions.

---

### Contains()

Performs a culture-sensitive string containment check.

**Syntax:**

```csharp
public static bool Contains(this string input, string searched, StringComparison comparison)
```

**Parameters:**

- `input` (string): The source string to search in
- `searched` (string): The substring to search for
- `comparison` (StringComparison): The comparison type (e.g., `OrdinalIgnoreCase`)

**Returns:** True if the substring is found; otherwise, false

**Example:**

```csharp
string text = "Hello World";
bool found = text.Contains("world", StringComparison.OrdinalIgnoreCase); // true
```

---

### Base64Encode()

Encodes a plain text string to Base64.

**Syntax:**

```csharp
public static string Base64Encode(this string plainText)
```

**Parameters:**

- `plainText` (string): The plain text string to encode

**Returns:** A Base64-encoded string

**Example:**

```csharp
string encoded = "Hello World".Base64Encode(); // "SGVsbG8gV29ybGQ="
```

---

### Base64Decode()

Decodes a Base64-encoded string back to plain text.

**Syntax:**

```csharp
public static string Base64Decode(this string base64EncodedData)
```

**Parameters:**

- `base64EncodedData` (string): The Base64-encoded string to decode

**Returns:** The decoded plain text string

**Example:**

```csharp
string decoded = "SGVsbG8gV29ybGQ=".Base64Decode(); // "Hello World"
```

---

## DateTime Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `DateTimeExtension`

### ToEndOfDay()

Calculates the end of the given date (23:59:59).

**Syntax:**

```csharp
public static DateTime ToEndOfDay(this DateTime dateTime)
```

**Parameters:**

- `dateTime` (DateTime): The date to process

**Returns:** A DateTime representing 11:59:59 PM on the given date

**Example:**

```csharp
DateTime date = new DateTime(2024, 12, 13, 10, 30, 0);
DateTime endDay = date.ToEndOfDay(); // 2024-12-13 23:59:59
```

---

### ZeroTime()

Gets a date with the time zeroed out (00:00:00).

**Syntax:**

```csharp
public static DateTime ZeroTime(this DateTime dateTime)
```

**Parameters:**

- `dateTime` (DateTime): The date to process

**Returns:** A DateTime with time set to 00:00:00

**Example:**

```csharp
DateTime date = new DateTime(2024, 12, 13, 10, 30, 45);
DateTime zeroed = date.ZeroTime(); // 2024-12-13 00:00:00
```

---

### NullOnUnixStart()

Returns null if the date is on or before January 1, 1970 (Unix epoch).

**Syntax:**

```csharp
public static DateTime? NullOnUnixStart(this DateTime? date)
```

**Parameters:**

- `date` (DateTime?): The nullable date to check

**Returns:** The original date if after Unix epoch, otherwise null

**Example:**

```csharp
DateTime? oldDate = new DateTime(1960, 1, 1);
DateTime? result = oldDate.NullOnUnixStart(); // null

DateTime? validDate = new DateTime(2024, 12, 13);
DateTime? result2 = validDate.NullOnUnixStart(); // 2024-12-13
```

---

### LastYearDay()

Calculates the corresponding date of the same day and week number in the previous year.

**Syntax:**

```csharp
public static DateTime LastYearDay(this DateTime todayDate)
```

**Parameters:**

- `todayDate` (DateTime): The current date

**Returns:** The date of the same day and week number in the previous year

**Example:**

```csharp
DateTime date = new DateTime(2024, 12, 13); // Friday, week 50
DateTime lastYear = date.LastYearDay(); // Friday of week 50 in 2023
```

---

### Age()

Calculates the age in years based on a birth date.

**Syntax:**

```csharp
// Overload 1: Using DateTime with current date
public static short Age(this DateTime birthDate)

// Overload 2: Using DateTime with specific reference date
public static short Age(this DateTime birthDate, DateTime fromDateTime)

// Overload 3: Using DateTimeOffset with current UTC date
public static short Age(this DateTimeOffset birthDate)

// Overload 4: Using DateTimeOffset with specific reference date
public static short Age(this DateTimeOffset birthDate, DateTimeOffset fromDateTime)
```

**Parameters:**

- `birthDate` (DateTime/DateTimeOffset): The date of birth
- `fromDateTime` (DateTime/DateTimeOffset, optional): Reference date for calculation (defaults to today/now)

**Returns:** The calculated age as a short integer

**Example:**

```csharp
DateTime birthDate = new DateTime(2000, 5, 15);
short age = birthDate.Age(); // Current age

short ageAt = birthDate.Age(new DateTime(2024, 12, 13)); // Age at specific date
```

---

### DsmMinDate()

Gets the Demoulas system minimum date (January 1, 1917). This is the founding date of Market Basket.

**Syntax:**

```csharp
public static DateTime DsmMinDate()
```

**Returns:** A DateTime representing January 1, 1917

**Example:**

```csharp
DateTime minDate = DateTimeExtension.DsmMinDate(); // 1917-01-01
```

---

## DateOnly Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `DateOnlyExtensions`

### ToDateOnly()

Converts DateTime or DateTimeOffset to DateOnly.

**Syntax:**

```csharp
// Overload 1: From DateTime (uses local kind by default)
public static DateOnly ToDateOnly(this DateTime dateTime)

// Overload 2: From DateTime with specified kind
public static DateOnly ToDateOnly(this DateTime dateTime, DateTimeKind kind)

// Overload 3: From DateTimeOffset
public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset)

// Overload 4: From DateTimeOffset with specified kind
public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset, DateTimeKind kind)
```

**Parameters:**

- `dateTime` (DateTime): The DateTime to convert
- `dateTimeOffset` (DateTimeOffset): The DateTimeOffset to convert
- `kind` (DateTimeKind, optional): The DateTimeKind to use for conversion

**Returns:** A DateOnly representation of the provided date/time

**Example:**

```csharp
DateTime dt = new DateTime(2024, 12, 13, 10, 30, 0);
DateOnly dateOnly = dt.ToDateOnly(); // 2024-12-13

DateTimeOffset dto = new DateTimeOffset(2024, 12, 13, 10, 30, 0, TimeSpan.Zero);
DateOnly dateOnly2 = dto.ToDateOnly(); // 2024-12-13
```

---

## Object Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `ObjectExtension`

### GetPropertyValue()

Dynamically retrieves the value of a property from an object.

**Syntax:**

```csharp
public static object? GetPropertyValue(this object source, string propertyName)
```

**Parameters:**

- `source` (object): The object to retrieve the property from
- `propertyName` (string): The name of the property

**Returns:** The property value, or null if the property doesn't exist

**Example:**

```csharp
var customer = new { Name = "John", Age = 30 };
object? name = customer.GetPropertyValue("Name"); // "John"
```

---

### GetPropertyValueAsString()

Retrieves a property value and converts it to a string.

**Syntax:**

```csharp
public static string? GetPropertyValueAsString(this object source, string propertyName)
```

**Parameters:**

- `source` (object): The object to retrieve the property from
- `propertyName` (string): The name of the property

**Returns:** A string representation of the property value, or null

**Example:**

```csharp
var product = new { Price = 19.99m };
string? price = product.GetPropertyValueAsString("Price"); // "19.99"
```

---

### GetPropertyValueAsDecimal()

Retrieves a property value and converts it to a decimal.

**Syntax:**

```csharp
public static decimal? GetPropertyValueAsDecimal(this object source, string propertyName, decimal defaultValue = 0)
```

**Parameters:**

- `source` (object): The object to retrieve the property from
- `propertyName` (string): The name of the property
- `defaultValue` (decimal, optional): Default value if conversion fails

**Returns:** A decimal representation of the property value, or the default value

**Example:**

```csharp
var order = new { Total = "150.50" };
decimal? total = order.GetPropertyValueAsDecimal("Total"); // 150.50

decimal? invalid = order.GetPropertyValueAsDecimal("NonExistent", 0); // 0
```

---

## IQueryable Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `IQueryableExtension`

### OrderByProperty()

Orders IQueryable results by a property name dynamically.

**Syntax:**

```csharp
public static IQueryable<TEntity> OrderByProperty<TEntity>(
    this IQueryable<TEntity> source,
    string orderByProperty,
    bool descending = false,
    bool ignoreNullValues = false)
```

**Type Parameters:**

- `TEntity`: The type of elements in the sequence

**Parameters:**

- `source` (IQueryable<TEntity>): The source queryable
- `orderByProperty` (string): The property name to order by (case-insensitive for strings)
- `descending` (bool, optional): Whether to sort in descending order
- `ignoreNullValues` (bool, optional): Whether to move null values to the bottom of the sort

**Returns:** An ordered IQueryable<TEntity>

**Exceptions:**

- `ArgumentException`: Thrown if the property doesn't exist on the entity type

**Features:**

- Case-insensitive sorting for string properties
- Null value handling to push them to bottom of results
- Supports multi-column sorting with comma-separated property names

**Example:**

```csharp
var customers = dbContext.Customers.AsQueryable();

// Simple ascending sort
var sorted = customers.OrderByProperty("LastName");

// Descending sort
var sortedDesc = customers.OrderByProperty("CreatedDate", descending: true);

// Ignore nulls
var sortedIgnoreNulls = customers.OrderByProperty("Email", ignoreNullValues: true);

// Multi-column sort
var multiSort = customers.OrderByProperty("LastName,FirstName");
```

---

## HttpRequestMessage Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `HttpRequestMessageExtension`

### GenerateCurlCommand()

Generates a cURL command string from an HttpRequestMessage.

**Syntax:**

```csharp
public static string GenerateCurlCommand(this HttpRequestMessage request, string url)
```

**Parameters:**

- `request` (HttpRequestMessage): The HTTP request message
- `url` (string): The complete URL being requested

**Returns:** A formatted cURL command string that can be copied to Postman or terminal

**Features:**

- Includes all headers
- Properly escapes quotes in header values
- Adds compression and insecure flags for testing
- Ready to paste into terminal or Postman

**Example:**

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/users");
request.Headers.Add("Authorization", "Bearer token123");

string curl = request.GenerateCurlCommand("https://api.example.com/users");
// curl -X GET "https://api.example.com/users" -H "Authorization: Bearer token123" --compressed --insecure
```

---

## Environment Extensions

**Namespace:** `Demoulas.Util.Extensions`  
**Class:** `EnvironmentExtensions`

### IsTestEnvironment()

Determines if the current environment is a test environment and updates the environment name to "Testing".

**Syntax:**

```csharp
// Overload 1: For IHostEnvironment
public static bool IsTestEnvironment(this IHostEnvironment environment)

// Overload 2: For IWebHostEnvironment
public static bool IsTestEnvironment(this IWebHostEnvironment environment)
```

**Parameters:**

- `environment` (IHostEnvironment/IWebHostEnvironment): The host environment

**Returns:** True if the environment is a test environment, otherwise false

**Recognized Test Environments:**

- Environment name: "Testing" or "Test"
- Application names: "ReSharperTestRunner" or "testHost"

**Side Effect:**

- Sets the environment name to "Testing" if a test environment is detected

**Example:**

```csharp
public void Configure(IHostEnvironment environment)
{
    if (environment.IsTestEnvironment())
    {
        // Configure test-specific settings
        // environment.EnvironmentName is now "Testing"
    }
}
```

---

## DbContext Extensions

**Namespace:** `Demoulas.Common.Data.Contexts.Extensions`  
**Class:** `DbContextExtensions`

### BulkInsertAsync()

Bulk inserts a list of entities into the database asynchronously.

**Syntax:**

```csharp
public static async Task BulkInsertAsync<T>(
    this DbContext context,
    List<T> itemsToInsert,
    CancellationToken cancellationToken,
    Dictionary<string, Type>? converterTypes = null,
    Dictionary<string, Func<dynamic, dynamic>>? converters = null)
    where T : class
```

**Type Parameters:**

- `T`: The entity type to bulk insert

**Parameters:**

- `context` (DbContext): The database context
- `itemsToInsert` (List<T>): The entities to insert
- `cancellationToken` (CancellationToken): Cancellation token for the operation
- `converterTypes` (Dictionary<string, Type>, optional): Column type conversions
- `converters` (Dictionary<string, Func<dynamic, dynamic>>, optional): Custom value converters

**Features:**

- Manages database connection automatically
- Supports column type conversions for schema compatibility
- Supports custom value converters
- Asynchronous operation with cancellation support

**Example:**

```csharp
var customers = new List<Customer>
{
    new Customer { Name = "John Doe", Email = "john@example.com" },
    new Customer { Name = "Jane Smith", Email = "jane@example.com" }
};

await dbContext.BulkInsertAsync(
    customers,
    cancellationToken);
```

---

## Database Context Configuration

**Namespace:** `Demoulas.Common.Data.Contexts.DTOs.Context`  
**Class:** `ContextFactoryRequest`

### Overview

The `ContextFactoryRequest` class is used to configure and initialize DbContext instances in the Demoulas Common library. It provides a fluent API for setting up database contexts with support for connection strings, custom EF Core options, interceptors, and role-based commit guards.

### ContextFactoryRequest.Initialize()

Creates a new context factory request with customizable configuration options.

**Syntax:**

```csharp
public static ContextFactoryRequest Initialize<TContext>(
    string connectionName,
    Action<OracleEntityFrameworkCoreSettings>? configureSettings = null,
    Action<DbContextOptionsBuilder>? configureDbContextOptions = null,
    Func<IServiceProvider, IEnumerable<IInterceptor>>? interceptorFactory = null,
    IEnumerable<string>? denyCommitRoles = null)
    where TContext : DbContext
```

**Type Parameters:**

- `TContext`: The type of DbContext to configure (must inherit from DbContext)

**Parameters:**

- `connectionName` (string, required): The name of the connection string in configuration
- `configureSettings` (Action, optional): Delegate to configure Aspire Oracle settings (health checks, retries, etc.)
- `configureDbContextOptions` (Action, optional): **Delegate to customize EF Core DbContextOptions**
- `interceptorFactory` (Func, optional): Factory function to create custom EF Core interceptors
- `denyCommitRoles` (IEnumerable<string>, optional): Roles that are denied write access to this context

**Returns:** A configured `ContextFactoryRequest` instance

**Key Feature - configureDbContextOptions:**

The `configureDbContextOptions` parameter allows you to customize EF Core behavior for your DbContext. This is essential when you need to:

- Enable sensitive data logging or detailed errors for debugging
- Configure query behavior (tracking, split queries, command timeouts)
- Override default Oracle settings
- Add additional EF Core features or optimizations

**Common Use Cases:**

1. **Development/Debugging Configuration:**

```csharp
ContextFactoryRequest.Initialize<AppDbContext>(
    connectionName: "AppDb",
    configureDbContextOptions: options =>
    {
        options.EnableSensitiveDataLogging();  // Show parameter values in logs
        options.EnableDetailedErrors();         // Show detailed error information
    })
```

2. **Query Behavior Customization:**

```csharp
ContextFactoryRequest.Initialize<AppReadOnlyDbContext>(
    connectionName: "AppDb",
    configureDbContextOptions: options =>
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.UseOracle(oracle => oracle.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    })
```

3. **Command Timeout Configuration:**

```csharp
ContextFactoryRequest.Initialize<AppDbContext>(
    connectionName: "AppDb",
    configureDbContextOptions: options =>
    {
        options.UseOracle(oracle => oracle.CommandTimeout(120)); // 2 minutes
    })
```

4. **Comprehensive Configuration (All Parameters):**

```csharp
ContextFactoryRequest.Initialize<AppDbContext>(
    connectionName: "AppDb",
    configureSettings: settings =>
    {
        settings.DisableHealthChecks = false;   // Enable health checks
        settings.DisableRetry = false;          // Enable retries
    },
    configureDbContextOptions: options =>
    {
        options.EnableSensitiveDataLogging();
        options.UseOracle(oracle =>
        {
            oracle.CommandTimeout(120);
            oracle.EnableRetryOnFailure(maxRetryCount: 3);
        });
    },
    interceptorFactory: serviceProvider =>
    {
        // Add custom interceptors
        return new[] { new AuditInterceptor(), new PerformanceInterceptor() };
    },
    denyCommitRoles: new[] { "ReadOnlyUser", "Auditor" })
```

5. **API/Web Application Example:**

```csharp
// In Program.cs or service configuration
List<ContextFactoryRequest> contextRequests = new()
{
    // Production context with minimal logging
    ContextFactoryRequest.Initialize<ProductionDbContext>("Production"),

    // Development context with enhanced debugging
    ContextFactoryRequest.Initialize<DevDbContext>(
        connectionName: "Development",
        configureDbContextOptions: options =>
        {
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        })
};

await builder.AddDatabaseServices(contextRequests);
```

**Important Notes:**

- The library automatically applies default Oracle configurations (compatibility, naming conventions, interceptors)
- The `configureDbContextOptions` parameter adds customizations **on top of** these defaults
- Do not use `EnableSensitiveDataLogging()` in production environments as it may log sensitive information
- For read-only contexts, the library automatically sets `QueryTrackingBehavior.NoTracking` unless overridden

**Integration with Database Services:**

```csharp
// Basic registration
builder.AddDatabaseServices<AppContextFactory, AppDbContext, AppReadOnlyDbContext>(
    (services, requests) =>
    {
        requests.Add(ContextFactoryRequest.Initialize<AppDbContext>("MyConnection"));
    },
    init: (builder, requests) => new AppContextFactory(builder, requests));

// With custom configuration
builder.AddDatabaseServices<AppContextFactory, AppDbContext, AppReadOnlyDbContext>(
    (services, requests) =>
    {
        requests.Add(ContextFactoryRequest.Initialize<AppDbContext>(
            connectionName: "MyConnection",
            configureDbContextOptions: options =>
            {
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }));
    },
    init: (builder, requests) => new AppContextFactory(builder, requests));
```

**See Also:**

- [Microsoft.EntityFrameworkCore.DbContextOptionsBuilder Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder)
- [Aspire Oracle EntityFrameworkCore Settings](https://learn.microsoft.com/en-us/dotnet/api/aspire.oracle.entityframeworkcore.oracleentityframeworkcoresettings)
- Demoulas.Common.Data.Contexts readme.md

---

## Pagination Extensions

**Namespace:** `Demoulas.Common.Data.Contexts.Extensions`  
**Class:** `PaginationExtensions`

### ToPaginationResultsAsync()

Paginates EF Core query results with sorting, timeout support, and telemetry. Runs count and data queries in parallel for efficiency.

**Syntax:**

```csharp
// Basic pagination
public static async Task<PaginatedResponseDto<TEntity>> ToPaginationResultsAsync<TEntity>(
    this IQueryable<TEntity> source,
    PaginationRequestDto request,
    CancellationToken cancellationToken = default)
    where TEntity : class

// With mapping/projection
public static async Task<PaginatedResponseDto<TResult>> ToPaginationResultsAsync<TEntity, TResult>(
    this IQueryable<TEntity> source,
    PaginationRequestDto request,
    Func<IQueryable<TEntity>, IQueryable<TResult>> mapper,
    CancellationToken cancellationToken = default)
    where TEntity : class
    where TResult : class
```

**Type Parameters:**

- `TEntity`: The database entity type
- `TResult`: The result type (for mapping overload)

**Parameters:**

- `source` (IQueryable<TEntity>): The source query
- `request` (PaginationRequestDto): Pagination request with Skip, Take, and optional SortBy
- `mapper` (Func, optional): A function to transform/project entities to results
- `cancellationToken` (CancellationToken, optional): Cancellation token

**Returns:** A PaginatedResponseDto<T> containing results, total count, and metadata

**Features:**

- Automatic parallel execution (count and data queries run simultaneously)
- Dynamic sorting with type-safe validation
- Query timeout with graceful degradation
- Result hashing for cache validation
- OpenTelemetry integration
- Support for both entity and mapped/projected results

**Example:**

```csharp
// Simple pagination
var request = new PaginationRequestDto { Skip = 0, Take = 20 };
var result = await dbContext.Customers
    .Where(c => c.IsActive)
    .ToPaginationResultsAsync(request);

// With sorting
var requestWithSort = new SortedPaginationRequestDto
{
    Skip = 0,
    Take = 20,
    SortBy = "LastName",
    IsSortDescending = false
};
var sorted = await dbContext.Customers.ToPaginationResultsAsync(requestWithSort);

// With projection to DTO
var result = await dbContext.Customers
    .Where(c => c.IsActive)
    .ToPaginationResultsAsync(
        request,
        customers => customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            FullName = $"{c.FirstName} {c.LastName}"
        }));

// With timeout
var timedRequest = new PaginationRequestDto
{
    Skip = 0,
    Take = 20,
    QueryTimeoutSeconds = 5
};
var timedResult = await dbContext.Customers.ToPaginationResultsAsync(timedRequest);
```

---

## API Endpoint Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`  
**Class:** `EndpointExtension`

### ConfigureDefaultEndpoints()

Configures default endpoints and services for a WebApplicationBuilder with comprehensive security, logging, and API documentation settings.

**Syntax:**

```csharp
public static WebApplicationBuilder ConfigureDefaultEndpoints(
    this WebApplicationBuilder builder,
    bool addOktaSecurity = false,
    string[]? meterNames = null,
    string[]? activitySourceNames = null,
    IRolePermissionService? rolePermissionService = null,
    List<JsonConverter>? jsonConverters = null,
    Action<CultureConfiguration>? cultureConfigurationAction = null,
    Action<JsonOptions>? jsonOptionsAction = null,
    Action<HstsOptions>? hstsOptionsAction = null,
    Func<AuthenticationFailedContext, Task>? onAuthenticationFailed = null,
    Func<JwtBearerChallengeContext, Task>? onChallenge = null,
    Func<ForbiddenContext, Task>? onForbidden = null)
```

**Parameters:**

- `builder` (WebApplicationBuilder): The builder to configure
- `addOktaSecurity` (bool, optional): Enable Okta authentication
- `meterNames` (string[], optional): Meter names for telemetry
- `activitySourceNames` (string[], optional): Activity source names for tracing
- `rolePermissionService` (IRolePermissionService, optional): Role-based permission service
- `jsonConverters` (List<JsonConverter>, optional): Custom JSON converters
- `cultureConfigurationAction` (Action, optional): Culture configuration callback
- `jsonOptionsAction` (Action, optional): JSON options customization callback
- `hstsOptionsAction` (Action, optional): HSTS options customization
- `onAuthenticationFailed` (Func, optional): Authentication failure handler
- `onChallenge` (Func, optional): JWT challenge handler
- `onForbidden` (Func, optional): Forbidden response handler

**Returns:** The configured WebApplicationBuilder

**Configured Components:**

- Kestrel HTTP/HTTPS settings
- Authentication (JWT and optionally Okta)
- Authorization and role-based permissions
- Comprehensive logging with Serilog
- JSON serialization with custom converters
- OpenTelemetry telemetry and tracing
- HSTS headers for non-development environments
- Security headers via NetEscapades
- Request compression
- Response compression
- API versioning

**Example:**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDefaultEndpoints(
    addOktaSecurity: true,
    meterNames: new[] { "MyApp.Metrics" },
    cultureConfigurationAction: culture =>
    {
        culture.DefaultCulture = "en-US";
    });

var app = builder.Build();
```

---

## Exception Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`  
**Class:** `ExceptionExtensions`

### ToProblemDetails()

Converts an exception to an HTTP ProblemDetails response.

**Syntax:**

```csharp
// Overload 1: For general exceptions
public static ProblemDetails ToProblemDetails(
    this Exception ex,
    string? title = null,
    string? details = null,
    string? instance = null,
    string? type = null,
    int? statusCode = null)

// Overload 2: For validation exceptions
public static ValidationProblemDetails ToProblemDetails(
    this ValidationException ex,
    string? title = null,
    string? details = null,
    string? instance = null,
    string? type = null)
```

**Parameters:**

- `ex` (Exception/ValidationException): The exception to convert
- `title` (string, optional): Custom title (defaults to exception message)
- `details` (string, optional): Additional details
- `instance` (string, optional): URI identifying the specific occurrence
- `type` (string, optional): URI identifying the problem type
- `statusCode` (int, optional): HTTP status code

**Returns:** ProblemDetails or ValidationProblemDetails

**Default Status Code Mapping:**

- ValidationException â†’ 400 (Bad Request)
- ArgumentException â†’ 400 (Bad Request)
- KeyNotFoundException â†’ 404 (Not Found)
- UnauthorizedAccessException â†’ 401 (Unauthorized)
- NotImplementedException â†’ 501 (Not Implemented)
- UniqueConstraintException â†’ 409 (Conflict)
- ReferenceConstraintException â†’ 409 (Conflict)
- All others â†’ 500 (Internal Server Error)

**Example:**

```csharp
try
{
    // Some operation
}
catch (ValidationException ex)
{
    var problemDetails = ex.ToProblemDetails(
        title: "Validation Failed",
        instance: "/api/customers");

    return BadRequest(problemDetails);
}
```

---

## Middleware Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`  
**Class:** `MiddlewareExtensions`

### UseCommonMiddleware()

Adds custom middleware to the request pipeline including server timing and version headers.

**Syntax:**

```csharp
public static IApplicationBuilder UseCommonMiddleware(this IApplicationBuilder builder)
```

**Parameters:**

- `builder` (IApplicationBuilder): The application builder

**Returns:** The configured IApplicationBuilder

**Middleware Added:**

- Status code pages middleware
- Server timing middleware (for performance metrics)
- Version middleware (adds version header to responses)

**Example:**

```csharp
var app = builder.Build();

app.UseCommonMiddleware(); // Add custom middleware

app.MapControllers();
app.Run();
```

---

## Swagger Extensions

**Namespace:** `Demoulas.Common.Api.Extensions`  
**Class:** `SwaggerExtension`

### AddSwagger()

Adds Swagger/OpenAPI documentation to a WebApplicationBuilder with comprehensive configuration options.

**Syntax:**

```csharp
public static WebApplicationBuilder AddSwagger(
    this WebApplicationBuilder builder,
    int version = 1,
    string title = "Demoulas Super Markets, Inc",
    string? description = null,
    ContactDetails? contactDetails = null,
    Action<DocumentSettings>? tagDescriptions = null,
    Action<AspNetCoreOpenApiDocumentGeneratorSettings>? documentSettingsAction = null,
    Action<OktaSettings>? oktaSettingsAction = null,
    List<IOperationProcessor>? operationProcessors = null,
    bool excludeNonFastEndpoints = false,
    ILogger? logger = null)
```

**Parameters:**

- `builder` (WebApplicationBuilder): The builder to configure
- `version` (int, optional): API version (default: 1)
- `title` (string, optional): API title
- `description` (string, optional): API description (auto-generated if not provided)
- `contactDetails` (ContactDetails, optional): Contact information
- `tagDescriptions` (Action, optional): Configure tag descriptions
- `documentSettingsAction` (Action, optional): Configure document settings
- `oktaSettingsAction` (Action, optional): Configure Okta security
- `operationProcessors` (List, optional): Custom operation processors
- `excludeNonFastEndpoints` (bool, optional): Only show FastEndpoints in docs
- `logger` (ILogger, optional): Logger for registration info

**Features:**

- Generates description from app name, environment, and build number
- Configures API versioning if version > 1
- JWT Bearer authentication setup
- Optional Okta OAuth2 flow
- Security headers configuration
- Tag case and schema customization

**Example:**

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddSwagger(
    version: 2,
    title: "Customer API",
    description: "API for managing customers",
    contactDetails: new ContactDetails
    {
        Name = "Support Team",
        Email = "support@example.com"
    },
    oktaSettingsAction: settings =>
    {
        settings.Enabled = true;
    });

var app = builder.Build();
```

---

## PDF Extensions

**Namespace:** `Demoulas.Common.Pdf.Extensions`

### ColorExtensions.GetQuestColor()

Converts System.Drawing.Color to QuestPDF color with special handling for black.

**Syntax:**

```csharp
internal static QColor GetQuestColor(this Color color)
```

**Parameters:**

- `color` (Color): The System.Drawing.Color to convert

**Returns:** A QuestPDF Color (QColor)

**Note:** Special handling for black color due to QuestPDF's color handling quirks.

**Example:**

```csharp
Color drawingColor = Color.Black;
QColor questColor = drawingColor.GetQuestColor();
```

---

### ContainerExtensions.AddDemoulasHeader()

Adds a standardized Demoulas header to a PDF document container.

**Syntax:**

```csharp
public static void AddDemoulasHeader(
    this IContainer container,
    string title,
    string leftHeaderString = "",
    string rightHeaderString = "",
    bool showPageNumber = true)
```

**Parameters:**

- `container` (IContainer): The PDF container
- `title` (string): The header title (displayed in center)
- `leftHeaderString` (string, optional): Text for left column
- `rightHeaderString` (string, optional): Text for right column
- `showPageNumber` (bool, optional): Show page number in right column

**Features:**

- Three-column header layout
- Black borders on header row
- Optional page numbering
- Configurable left/right header text

**Example:**

```csharp
container.AddDemoulasHeader(
    title: "Invoice Report",
    leftHeaderString: "Company Name",
    rightHeaderString: "December 2024",
    showPageNumber: true);
```

---

### ContainerExtensions.AddDemoulasTitle()

Adds a standardized centered title to the document container.

**Syntax:**

```csharp
public static void AddDemoulasTitle(this IContainer container, string title)
```

**Parameters:**

- `container` (IContainer): The PDF container
- `title` (string): The title text to display

**Example:**

```csharp
container.AddDemoulasTitle("Quarterly Report");
```

---

### PageDescriptorExtensions.AddDefaultMargin()

Adds default 0.635 cm (1/4 inch) margin to all sides of the page.

**Syntax:**

```csharp
public static void AddDefaultMargin(this PageDescriptor pageDescriptor)
```

**Parameters:**

- `pageDescriptor` (PageDescriptor): The page descriptor to configure

**Example:**

```csharp
page.AddDefaultMargin();
```

---

### PageDescriptorExtensions.AddPageDefaults()

Applies default page configurations including size, color, text style, and margins.

**Syntax:**

```csharp
public static void AddPageDefaults(this PageDescriptor pageDescriptor)
```

**Parameters:**

- `pageDescriptor` (PageDescriptor): The page descriptor to configure

**Default Settings:**

- Page size: Letter (portrait)
- Page color: White
- Text style: Default Demoulas style
- Margins: 0.635 cm on all sides

**Example:**

```csharp
page.AddPageDefaults();
```

---

## Best Practices

1. **String Extensions**: Use `FirstCharToUpper()` instead of manual string manipulation for consistent capitalization
2. **DateTime Extensions**: Use specialized methods like `ToEndOfDay()` and `ZeroTime()` to ensure consistent date/time handling
3. **Database Context Configuration**: Always use `ContextFactoryRequest.Initialize()` with the `configureDbContextOptions` parameter when you need to customize EF Core behavior. Avoid creating workarounds or custom initialization code.
4. **DbContext Options**: Use `configureDbContextOptions` for development-specific settings like `EnableSensitiveDataLogging()`, but ensure these are never enabled in production environments
5. **Pagination**: Always use `ToPaginationResultsAsync()` for database queries to support sorting and filtering
6. **Exception Handling**: Leverage `ToProblemDetails()` for consistent error responses in APIs
7. **PDF Generation**: Use the Demoulas-specific extensions for consistent styling and formatting
8. **Environment Detection**: Use `IsTestEnvironment()` to safely detect test contexts without manual checks
9. **Dynamic Queries**: Use `OrderByProperty()` for dynamic sorting instead of manual LINQ-to-SQL construction
10. **Context Configuration**: Leverage all available parameters in `ContextFactoryRequest.Initialize()` (configureSettings, configureDbContextOptions, interceptorFactory, denyCommitRoles) instead of creating custom solutions

## Import Statements

Add these namespaces to use the extensions:

```csharp
using Demoulas.Util.Extensions;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Pdf.Extensions;
```

---

## Support

For questions or issues with these extensions, please contact the Demoulas Common Library development team or refer to the specific project documentation.
