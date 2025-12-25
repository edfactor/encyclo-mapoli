# Demoulas Common Libraries - Complete Guide

This is the main index for all Demoulas Common library documentation. The libraries provide a comprehensive suite of extensions, utilities, and base classes for building enterprise .NET applications.

## Package Documentation

### [Demoulas.Util](./demoulas.util.instructions.md)

Core utility extensions for common .NET types:

-   String Extensions (FirstCharToUpper, Base64 encoding, Contains)
-   DateTime Extensions (ToEndOfDay, ZeroTime, Age calculations)
-   DateOnly Extensions (ToDateOnly conversions)
-   Object Extensions (Dynamic property access)
-   IQueryable Extensions (Dynamic sorting with OrderByProperty)
-   HttpRequestMessage Extensions (cURL generation)
-   Environment Extensions (Test environment detection)

### [Demoulas.Common.Data](./demoulas.common.data.instructions.md)

Database and data access utilities:

-   DbContext Extensions (Bulk operations)
-   Database Context Configuration (ContextFactoryRequest)
-   Pagination Extensions (Efficient pagination with parallel queries)

### [Demoulas.Common.Api](./demoulas.common.api.instructions.md)

REST API development utilities and base classes:

-   **Endpoint Base Classes** (DemoulasEndpoint with automatic telemetry)
-   API Configuration Extensions (ConfigureDefaultEndpoints)
-   Exception Extensions (ProblemDetails conversion)
-   Middleware Extensions (Server timing, versioning)
-   Swagger/OpenAPI Extensions (Documentation generation)

### [Demoulas.Common.Pdf](./demoulas.common.pdf.instructions.md)

PDF generation utilities:

-   Container Extensions (Standardized headers and titles)
-   Page Extensions (Default margins and configurations)
-   Color Extensions (QuestPDF color conversions)

## Quick Start

### Installation

Add package references to your project:

```xml
<ItemGroup>
  <ProjectReference Include="..\Demoulas.Util\Demoulas.Util.csproj" />
  <ProjectReference Include="..\Demoulas.Common.Api\Demoulas.Common.Api.csproj" />
  <ProjectReference Include="..\Demoulas.Common.Data\Demoulas.Common.Data.csproj" />
  <ProjectReference Include="..\Demoulas.Common.Pdf\Demoulas.Common.Pdf.csproj" />
</ItemGroup>
```

### Common Imports

```csharp
// Utility extensions
using Demoulas.Util.Extensions;

// Data access
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.Common.Data.Contexts.DTOs.Context;

// API development
using Demoulas.Common.Api.Extensions;
using Demoulas.Common.Api.Endpoints;

// PDF generation
using Demoulas.Common.Pdf.Extensions;
```

## Best Practices

### General Guidelines

1. **Use extension methods** instead of manual implementations for common operations
2. **Leverage base classes** (especially API endpoint base classes) for consistent patterns
3. **Follow telemetry patterns** to ensure observability across all services
4. **Use pagination extensions** for all list endpoints to maintain performance
5. **Utilize environment detection** for test-specific configurations

### API Development

-   Always inherit from `DemoulasEndpoint<TRequest, TResponse>` or related base classes
-   Implement `HandleRequestAsync()` for business logic (never override `HandleAsync()`)
-   Mark sensitive fields using `GetSensitiveFields()` for compliance
-   Add custom telemetry tags for domain-specific tracking

### Database Access

-   Use `ContextFactoryRequest.Initialize()` with `configureDbContextOptions` for EF Core customization
-   Leverage `ToPaginationResultsAsync()` for efficient pagination with parallel queries
-   Use bulk operations (`BulkInsertAsync`) for large data imports

### Testing

-   Use `IsTestEnvironment()` to detect test contexts
-   Configure test-specific settings in `configureDbContextOptions` (e.g., sensitive data logging)
-   Never enable sensitive data logging in production

## Additional Resources

-   [Code Review Guidelines](./code-review.instructions.md)
-   [RESTful API Guidelines](./restful-api-guidelines.instructions.md)
-   [Security Guidelines](./security.instructions.md)

## Support

For questions or issues with these libraries, please contact the Demoulas Common Library development team or create an issue in the repository.

---

**Last Updated:** December 2025
