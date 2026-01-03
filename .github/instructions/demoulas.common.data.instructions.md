# Demoulas.Common.Data - Database and Data Access Utilities

**Packages:** `Demoulas.Common.Data.Contexts`, `Demoulas.Common.Data.Services`  
**Namespace:** `Demoulas.Common.Data.Contexts.Extensions`, `Demoulas.Common.Data.Contexts.DTOs.Context`

This package provides database and data access utilities including bulk operations, context configuration, and efficient pagination for Entity Framework Core with Oracle databases.

## Table of Contents

1. [DbContext Extensions](#dbcontext-extensions)
2. [Database Context Configuration](#database-context-configuration)
3. [Pagination Extensions](#pagination-extensions)

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

await dbContext.BulkInsertAsync(customers, cancellationToken);
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
        options.UseOracle(oracle =>
            oracle.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
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

**Important Notes:**

- The library automatically applies default Oracle configurations (compatibility, naming conventions, interceptors)
- The `configureDbContextOptions` parameter adds customizations **on top of** these defaults
- Do not use `EnableSensitiveDataLogging()` in production environments as it may log sensitive information
- For read-only contexts, the library automatically sets `QueryTrackingBehavior.NoTracking` unless overridden

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

## Best Practices

1. **Context Configuration**: Always use `ContextFactoryRequest.Initialize()` with the `configureDbContextOptions` parameter when you need to customize EF Core behavior
2. **DbContext Options**: Use `configureDbContextOptions` for development-specific settings like `EnableSensitiveDataLogging()`, but ensure these are never enabled in production
3. **Pagination**: Always use `ToPaginationResultsAsync()` for database queries to support sorting and filtering with parallel execution
4. **Bulk Operations**: Use `BulkInsertAsync()` for large data imports instead of individual inserts
5. **Read-Only Contexts**: Leverage automatic `NoTracking` behavior for read-only database contexts
6. **Parallel Queries**: The pagination extension automatically runs count and data queries in parallel for optimal performance
7. **Timeouts**: Configure query timeouts in pagination requests to prevent long-running queries

## Import Statements

```csharp
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Microsoft.EntityFrameworkCore;
```

---

**See Also:**

- [Main Documentation](./demoulas.common.instructions.md)
- [Util Extensions](./demoulas.util.instructions.md)
- [API Extensions](./demoulas.common.api.instructions.md)
- [Microsoft EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
