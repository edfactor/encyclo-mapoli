# Service Layer Architecture

**Project**: Demoulas Profit Sharing Application
**Location**: `./src/services/src/Demoulas.ProfitSharing.Services/`
**Technology**: .NET 9, C# 13, EF Core 9 with Oracle Provider
**Lines of Code**: ~13,784
**Last Updated**: 2025-10-23

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture Principles](#architecture-principles)
3. [Directory Structure](#directory-structure)
4. [Service Registration & Dependency Injection](#service-registration--dependency-injection)
5. [Core Service Patterns](#core-service-patterns)
6. [Service Categories](#service-categories)
7. [Data Access Patterns](#data-access-patterns)
8. [Caching Strategies](#caching-strategies)
9. [Validation & Business Rules](#validation--business-rules)
10. [Error Handling & Result Types](#error-handling--result-types)
11. [Transaction Management](#transaction-management)
12. [Performance Optimization](#performance-optimization)
13. [Testing Guidelines](#testing-guidelines)
14. [Common Pitfalls & Solutions](#common-pitfalls--solutions)
15. [Creating New Services](#creating-new-services)

---

## Overview

The Services layer implements all business logic for the Demoulas Profit Sharing application. This layer sits between FastEndpoints (presentation) and EF Core Data contexts (persistence), orchestrating complex profit-sharing calculations, year-end processing, demographic management, reporting, and validation workflows.

**Key Responsibilities**:

- Business logic execution and orchestration
- Complex profit-sharing calculations (contributions, earnings, vesting, forfeitures)
- Data transformation between entities and DTOs
- Distributed caching for performance
- Cross-cutting concerns (history tracking, validation, auditing)
- Integration with external systems (Oracle HCM, reporting engines)

**Design Philosophy**:

- **Single Responsibility**: Each service focuses on a specific domain area
- **Interface Segregation**: Services expose focused interfaces in `Common.Interfaces`
- **Dependency Inversion**: Services depend on abstractions (interfaces), not concrete implementations
- **Async-First**: All I/O operations use async/await patterns
- **No Direct DbContext in Endpoints**: Endpoints call services; services use `IProfitSharingDataContextFactory`

---

## Architecture Principles

### 1. **Separation of Concerns**

Services are isolated from HTTP concerns. They:

- Do NOT accept `HttpContext` or web-specific types (except specialized middleware services)
- Return domain objects or DTOs, not HTTP responses
- Use `CancellationToken` for operation cancellation
- Throw domain exceptions or return `Result<T>` for error handling

### 2. **Context Factory Pattern**

All services use `IProfitSharingDataContextFactory` instead of directly injecting `DbContext`:

```csharp
public class CalendarService : ICalendarService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public async Task<CalendarResponseDto> GetYearDatesAsync(short year, CancellationToken ct)
    {
        // Read-only operation
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ctx.UseReadOnlyContext(); // Auto-applies AsNoTracking
            return await ctx.AccountingPeriods
                .TagWith($"GetYearDates-{year}")
                .Where(r => r.WeekNo == 1)
                .FirstOrDefaultAsync(ct);
        }, ct);
    }
}
```

**Benefits**:

- Consistent context lifecycle management
- Automatic transaction handling for writable operations
- Read-only optimizations (no tracking, streaming queries)
- Explicit read vs. write intent

### 3. **Frozen vs. Live Demographic Data**

The `IDemographicReaderService` provides a critical abstraction for accessing demographic data in two modes:

**Live Mode** (default):

```csharp
var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: false);
```

**Frozen Mode** (for year-end processing):

```csharp
var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: true);
```

Frozen demographics represent a **point-in-time snapshot** created during year-end processing to ensure consistency across reports and calculations even as live employee data changes.

### 4. **Domain-Driven Service Organization**

Services are grouped by domain area:

- **Core Domain**: `TotalService`, `YearEndService`, `ProfitShareUpdateService`
- **Demographics**: `DemographicReaderService`, `EmployeeLookupService`
- **Beneficiaries**: `BeneficiaryService`, `BeneficiaryDisbursementService`
- **Reports**: `ReportRunnerService`, `BreakdownReportService`, `FrozenReportService`
- **Validation**: `ChecksumValidationService`, `CrossReferenceValidationService`
- **Infrastructure**: `NavigationService`, `CalendarService`, `CachingServices`

---

## Directory Structure

```
Demoulas.ProfitSharing.Services/
├── Audit/
│   └── AuditService.cs                    # Audit trail management
├── Beneficiaries/
│   ├── BeneficiaryService.cs              # CRUD for beneficiaries
│   └── BeneficiaryDisbursementService.cs  # Beneficiary payment processing
├── BeneficiaryInquiry/
│   └── BeneficiaryInquiryService.cs       # Search and lookup
├── Caching/
│   ├── ILookupCache.cs                    # Generic cache interface
│   ├── LookupCache.cs                     # Version-based distributed cache
│   ├── StateTaxCache.cs                   # State tax lookup cache
│   ├── ProfitCodeCache.cs                 # Profit code lookup cache
│   └── HostedServices/
│       └── StateTaxCacheWarmerHostedService.cs  # Pre-warm caches on startup
├── Certificates/
│   └── CertificateService.cs              # Profit-sharing certificate generation
├── Extensions/
│   ├── ServicesExtension.cs               # DI registration for all services
│   ├── DemographicHeaderExtensions.cs     # Helper extensions
│   └── ProfitDetailExtensions.cs          # Profit detail helpers
├── Internal/
│   ├── Interfaces/                        # Internal-only service contracts
│   │   ├── IDemographicReaderService.cs
│   │   ├── IEmbeddedSqlService.cs
│   │   └── IInternalProfitShareUpdateService.cs
│   ├── ServiceDto/                        # Internal DTOs (not exposed to endpoints)
│   │   ├── Member.cs
│   │   ├── MemberSlice.cs
│   │   ├── ParticipantTotalDto.cs
│   │   └── ProfitShareUpdateMember.cs
│   └── ProfitShareUpdate/                 # Year-end calculation internals
│       ├── MemberFinancials.cs
│       ├── EmployeeFinancials.cs
│       └── BeneficiaryFinancials.cs
├── ItDevOps/
│   ├── FrozenService.cs                   # Frozen demographic snapshots
│   └── TableMetadataService.cs            # Database metadata inspection
├── LogMasking/
│   ├── SensitiveValueMaskingOperator.cs   # Serilog SSN/PII masking
│   └── UnformattedSocialSecurityNumberMaskingOperator.cs
├── Lookup/
│   ├── EmployeeLookupService.cs           # Employee search
│   └── StateTaxLookupService.cs           # State tax queries
├── MasterInquiry/
│   ├── MasterInquiryService.cs            # Unified member search
│   ├── EmployeeMasterInquiryService.cs    # Employee-specific queries
│   └── BeneficiaryMasterInquiryService.cs # Beneficiary-specific queries
├── MergeProfitDetails/
│   └── MergeProfitDetailsService.cs       # Merge duplicate profit records
├── Middleware/
│   ├── EndpointInstrumentationMiddleware.cs  # Telemetry tracking
│   └── DemographicHeaderMiddleware.cs     # Demographic context propagation
├── Military/
│   └── MilitaryService.cs                 # Military service adjustments
├── Navigations/
│   ├── NavigationService.cs               # Role-based navigation tree
│   └── NavigationPrerequisiteValidator.cs # Workflow prerequisites
├── ProfitMaster/
│   └── ProfitMasterService.cs             # Profit master record management
├── ProfitShareEdit/
│   ├── ProfitShareEditService.cs          # Manual profit adjustments
│   ├── ProfitShareUpdateService.cs        # Year-end profit calculations
│   ├── EmployeeProcessorHelper.cs         # Employee processing logic
│   └── BeneficiariesProcessingHelper.cs   # Beneficiary processing logic
├── Reports/
│   ├── ReportRunnerService.cs             # Report orchestration
│   ├── BreakdownReportService.cs          # Store breakdown reports
│   ├── CleanupReportService.cs            # Data cleanup reports
│   ├── DuplicateNamesAndBirthdaysService.cs
│   ├── ExecutiveHoursAndDollarsService.cs
│   ├── FrozenReportService.cs             # Frozen data reports
│   ├── NegativeEtvaReportService.cs       # Negative balance reports
│   ├── PayBenReportService.cs             # Payroll beneficiary reports
│   ├── PostFrozenService.cs               # Post-freeze validation
│   └── TerminatedEmployeeAndBeneficiaryReport/
│       ├── TerminatedEmployeeReportService.cs
│       └── TextReportGenerator.cs
├── Serialization/
│   ├── RoleContextMiddleware.cs           # Role context for serialization
│   ├── MaskingAmbientRoleContext.cs       # Thread-local role context
│   └── MaskingJsonConverterFactory.cs     # Role-based JSON masking
├── Utilities/
│   └── CsvStringHandler.cs                # CSV parsing utilities
├── Validation/
│   ├── ChecksumValidationService.cs       # SHA256 checksum validation
│   ├── CrossReferenceValidationService.cs # Cross-report validation
│   ├── ArchivedValueService.cs            # Retrieve archived values
│   ├── AllocTransferValidationService.cs  # Allocation transfer checks
│   └── BalanceEquationValidationService.cs # Balance equation checks
├── CalendarService.cs                     # Fiscal calendar operations
├── DemographicReaderService.cs            # Frozen/live demographic abstraction
├── DistributionService.cs                 # Distribution calculations
├── EmbeddedSqlService.cs                  # Raw SQL for performance
├── EnrollmentSummarizer.cs                # Enrollment summary logic
├── FakeSsnService.cs                      # Fake SSN generation (dev/test)
├── ForfeitureAdjustmentService.cs         # Forfeiture adjustments
├── MissiveService.cs                      # Communication/notification
├── PayClassificationService.cs            # Pay classification logic
├── PayProfitUpdateService.cs              # PayProfit record updates
├── ProfitDetailReversalsService.cs        # Reverse profit transactions
├── TotalService.cs                        # Core totals calculations
└── YearEndService.cs                      # Year-end processing (PAY426 equivalent)
```

**Total Files**: ~100+ service classes
**Total Lines of Code**: ~13,784 lines

---

## Service Registration & Dependency Injection

All services are registered in `ServicesExtension.AddProjectServices()`:

```csharp
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        // Scoped services (per-request lifecycle)
        builder.Services.AddScoped<IPayClassificationService, PayClassificationService>();
        builder.Services.AddScoped<ICertificateService, CertificateService>();
        builder.Services.AddScoped<IDistributionService, DistributionService>();
        builder.Services.AddScoped<IBeneficiaryService, BeneficiaryService>();
        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<INavigationService, NavigationService>();

        // Singleton services (application-wide, stateless)
        builder.Services.AddSingleton<IFakeSsnService, FakeSsnService>();
        builder.Services.AddSingleton<IAccountingPeriodsService, AccountingPeriodsService>();
        builder.Services.AddSingleton<ICalendarService, CalendarService>();

        // Caching singletons (manage distributed cache access)
        builder.Services.AddSingleton<StateTaxCache>();
        builder.Services.AddSingleton<ProfitCodeCache>();

        // Validation services
        builder.Services.AddScoped<IChecksumValidationService, ChecksumValidationService>();
        builder.Services.AddScoped<ICrossReferenceValidationService, CrossReferenceValidationService>();

        // Cache warmers (only in non-test environments)
        if (!builder.Environment.IsTestEnvironment())
        {
            builder.Services.AddHostedService<StateTaxCacheWarmerHostedService>();
        }

        return builder;
    }
}
```

**Lifecycle Guidelines**:

- **Scoped**: Services that depend on request context or DbContext (majority)
- **Singleton**: Stateless services, caches, configuration services
- **Transient**: Rarely used; prefer scoped for predictable lifecycle

---

## Core Service Patterns

### Pattern 1: Read-Only Query Service

**Characteristics**:

- Returns DTOs or domain objects
- Uses `UseReadOnlyContext` for optimized queries
- No state mutations
- Cache-friendly

**Example**: `CalendarService`

```csharp
public sealed class CalendarService : ICalendarService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDistributedCache _distributedCache;
    private const string YearDatesCacheKey = "CalendarService_YearDates";

    public async Task<CalendarResponseDto> GetYearStartAndEndAccountingDatesAsync(
        short calendarYear,
        CancellationToken ct = default)
    {
        var cacheKey = $"{YearDatesCacheKey}_{calendarYear}";
        var cached = await _distributedCache.GetAsync(cacheKey, ct);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<CalendarResponseDto>(cached)!;
        }

        var result = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ctx.UseReadOnlyContext(); // Auto AsNoTracking

            var fiscalDates = await ctx.AccountingPeriods
                .TagWith($"GetYearDates-{calendarYear}")
                .Where(r => r.WeekendingDate >= new DateOnly(calendarYear, 1, 1) &&
                           (r.WeekNo == 1 || r.WeekNo >= 52))
                .GroupBy(r => 1)
                .Select(g => new
                {
                    StartingDate = g.Min(r => r.WeekendingDate).AddDays(-6),
                    EndingDate = g.Where(r => r.Period == 12)
                        .OrderByDescending(r => r.WeekNo)
                        .Select(r => r.WeekendingDate)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);

            return new CalendarResponseDto
            {
                FiscalBeginDate = fiscalDates.StartingDate,
                FiscalEndDate = fiscalDates.EndingDate
            };
        }, ct);

        // Cache for 4 hours
        var serialized = JsonSerializer.SerializeToUtf8Bytes(result);
        await _distributedCache.SetAsync(
            cacheKey,
            serialized,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4)
            },
            ct);

        return result;
    }
}
```

**Key Points**:

- Uses `UseReadOnlyContext` for read operations
- Implements distributed caching with `IDistributedCache`
- Tags queries with business context (`TagWith`)
- Serializes DTOs with `System.Text.Json`

### Pattern 2: Write Service with Transaction

**Characteristics**:

- Mutates database state
- Uses `UseWritableContextAsync` for automatic transactions
- Returns success/failure indicators or domain events
- Validates inputs before persistence

**Example**: `BeneficiaryService.CreateBeneficiary`

```csharp
public class BeneficiaryService : IBeneficiaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public async Task<CreateBeneficiaryResponse> CreateBeneficiary(
        CreateBeneficiaryRequest req,
        CancellationToken ct)
    {
        // UseWritableContextAsync provides automatic transaction management
        var result = await _dataContextFactory.UseWritableContextAsync(
            async (ctx, transaction) =>
        {
            // 1. Validate referenced entities exist
            var beneficiaryContact = await ctx.BeneficiaryContacts
                .FirstOrDefaultAsync(x => x.Id == req.BeneficiaryContactId, ct);
            if (beneficiaryContact == null)
            {
                throw new InvalidOperationException("Beneficiary Contact does not exist");
            }

            var demographicQuery = await _demographicReaderService
                .BuildDemographicQuery(ctx, useFrozenData: false);
            var demographic = await demographicQuery
                .Where(x => x.BadgeNumber == req.EmployeeBadgeNumber)
                .SingleOrDefaultAsync(ct);
            if (demographic == null)
            {
                throw new InvalidOperationException("Employee Badge does not exist");
            }

            // 2. Validate business rules
            if (req.FirstLevelBeneficiaryNumber.HasValue &&
                (req.FirstLevelBeneficiaryNumber < 0 ||
                 req.FirstLevelBeneficiaryNumber > 9))
            {
                throw new InvalidOperationException(
                    "FirstLevelBeneficiaryNumber must be between 1 and 9");
            }

            if (req.Percentage < 0 || req.Percentage > 100)
            {
                throw new InvalidOperationException("Invalid percentage");
            }

            // 3. Calculate derived values
            var psnSuffix = await FindPsn(req, ctx, ct);

            // 4. Create entity
            var beneficiary = new Beneficiary
            {
                Id = 0,
                BadgeNumber = req.EmployeeBadgeNumber,
                PsnSuffix = psnSuffix,
                DemographicId = demographic.Id,
                Contact = beneficiaryContact,
                BeneficiaryContactId = req.BeneficiaryContactId,
                Relationship = req.Relationship,
                KindId = req.KindId,
                Percent = req.Percentage
            };

            ctx.Add(beneficiary);

            // 5. Save changes
            await ctx.SaveChangesAsync(ct);

            // 6. Commit transaction (if provided)
            if (transaction != null)
            {
                await transaction.CommitAsync(ct);
            }

            // 7. Return response DTO
            return new CreateBeneficiaryResponse
            {
                BeneficiaryId = beneficiary.Id,
                PsnSuffix = psnSuffix,
                EmployeeBadgeNumber = req.EmployeeBadgeNumber,
                // ... other fields
            };
        }, ct);

        return result;
    }
}
```

**Transaction Guarantees**:

- `UseWritableContextAsync` automatically begins a transaction
- Transaction commits only if no exceptions thrown
- Automatic rollback on exception
- Can manually commit via provided `transaction` parameter

### Pattern 3: Orchestration Service

**Characteristics**:

- Coordinates multiple service calls
- Implements complex business workflows
- May call multiple repositories/services
- Returns aggregated results

**Example**: `ProfitShareUpdateService`

```csharp
internal sealed class ProfitShareUpdateService : IInternalProfitShareUpdateService
{
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly TotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;

    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdate(
        ProfitShareUpdateRequest request,
        CancellationToken ct,
        bool includeZeroAmounts)
    {
        // 1. Orchestrate employee processing (complex calculation helper)
        (List<MemberFinancials> members, bool employeeExceededMaxContribution) =
            await EmployeeProcessorHelper.ProcessEmployees(
                _dbContextFactory,
                _calendarService,
                _totalService,
                _demographicReaderService,
                request,
                new AdjustmentsSummaryDto(),
                ct);

        // 2. Orchestrate beneficiary processing (may modify employees)
        await BeneficiariesProcessingHelper.ProcessBeneficiaries(
            _dbContextFactory,
            _totalService,
            members,
            request,
            ct);

        // 3. Filter and sort results
        members = members.OrderBy(m => m.Name).ToList();
        if (!includeZeroAmounts)
        {
            members = members.Where(m => !m.IsAllZeros()).ToList();
        }

        // 4. Aggregate totals
        ProfitShareUpdateTotals totals = new();
        foreach (var member in members)
        {
            totals.BeginningBalance += member.CurrentAmount;
            totals.Distributions += member.Distributions;
            totals.TotalContribution += member.Contributions;
            // ... accumulate all totals
        }

        // 5. Return outcome
        return new ProfitShareUpdateOutcome(
            members,
            adjustments,
            totals,
            employeeExceededMaxContribution);
    }
}
```

**Orchestration Best Practices**:

- Break complex workflows into smaller helper classes
- Use internal DTOs for intermediate results
- Aggregate results at the end
- Return rich outcome objects (not just primitives)

### Pattern 4: Validation Service

**Characteristics**:

- Validates complex business rules
- Returns structured validation results
- Often used in year-end or regulatory workflows

**Example**: `ChecksumValidationService`

```csharp
public sealed class ChecksumValidationService : IChecksumValidationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<ChecksumValidationService> _logger;

    public async Task<Result<ChecksumValidationResponse>> ValidateReportFieldsAsync(
        short profitYear,
        string reportType,
        Dictionary<string, decimal> fieldsToValidate,
        CancellationToken ct)
    {
        // 1. Validate inputs
        if (string.IsNullOrWhiteSpace(reportType))
        {
            return Result<ChecksumValidationResponse>.Failure(
                Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(reportType)] = new[] { "Report type cannot be null or empty." }
                }));
        }

        // 2. Query for archived checksums
        var archived = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.ReportChecksums
                .Where(r => r.ProfitYear == profitYear && r.ReportType == reportType)
                .OrderByDescending(r => r.CreatedAtUtc)
                .FirstOrDefaultAsync(ct), ct);

        if (archived == null)
        {
            return Result<ChecksumValidationResponse>.Failure(
                Error.EntityNotFound($"No archived report for {reportType} year {profitYear}"));
        }

        // 3. Validate each field
        var fieldResults = new Dictionary<string, FieldValidationResult>();
        var mismatchedFields = new List<string>();

        foreach (var (fieldName, fieldValue) in fieldsToValidate)
        {
            var providedHash = SHA256.HashData(
                JsonSerializer.SerializeToUtf8Bytes(fieldValue));
            var providedChecksum = Convert.ToBase64String(providedHash);

            // Compare with archived checksum
            bool matches = string.Equals(
                providedChecksum,
                archivedChecksum,
                StringComparison.Ordinal);

            fieldResults[fieldName] = new FieldValidationResult
            {
                Matches = matches,
                ProvidedValue = fieldValue,
                ProvidedChecksum = providedChecksum,
                ArchivedChecksum = archivedChecksum
            };

            if (!matches) mismatchedFields.Add(fieldName);
        }

        // 4. Return validation response
        return Result<ChecksumValidationResponse>.Success(new ChecksumValidationResponse
        {
            ProfitYear = profitYear,
            ReportType = reportType,
            IsValid = mismatchedFields.Count == 0,
            FieldResults = fieldResults,
            MismatchedFields = mismatchedFields
        });
    }
}
```

**Validation Patterns**:

- Use `Result<T>` for validation outcomes
- Return detailed field-level validation results
- Log validation failures for audit trail
- Use structured error dictionaries

---

## Service Categories

### 1. Core Domain Services

**Purpose**: Implement critical profit-sharing business logic

| Service                       | Responsibility                                         | Key Methods                                                        |
| ----------------------------- | ------------------------------------------------------ | ------------------------------------------------------------------ |
| `TotalService`                | Calculate participant balances, vesting, distributions | `GetVestingBalanceForSingleMemberAsync()`, `TotalVestingBalance()` |
| `YearEndService`              | Year-end processing (PAY426 equivalent)                | `RunFinalYearEndUpdates()`, `ComputeChange()`                      |
| `ProfitShareUpdateService`    | Apply contributions and earnings                       | `ProfitShareUpdate()`, `ProfitShareUpdateInternal()`               |
| `DistributionService`         | Process distributions and withdrawals                  | `CalculateDistribution()`, `ApplyDistribution()`                   |
| `ForfeitureAdjustmentService` | Handle forfeitures and adjustments                     | `AdjustForfeitures()`, `RecalculateVesting()`                      |

**Example - TotalService**:

```csharp
public sealed class TotalService : ITotalService
{
    // Retrieves vesting balance for a single member
    public async Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMemberAsync(
        SearchBy searchBy,
        int badgeNumberOrSsn,
        short profitYear,
        CancellationToken ct)
    {
        var calendarInfo = await _calendarService
            .GetYearStartAndEndAccountingDatesAsync(profitYear, ct);

        return await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService
                .BuildDemographicQuery(ctx);

            var result = await (
                from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                join d in demographics on t.Ssn equals d.Ssn
                where d.BadgeNumber == badgeNumberOrSsn
                select new BalanceEndpointResponse
                {
                    Ssn = t.Ssn.MaskSsn(),
                    CurrentBalance = t.CurrentBalance ?? 0,
                    VestedBalance = t.VestedBalance ?? 0,
                    VestingPercent = t.VestingPercent ?? 0,
                    YearsInPlan = t.YearsInPlan ?? 0
                })
                .FirstOrDefaultAsync(ct);

            return result;
        }, ct);
    }

    // Internal method - reusable query for vesting balance
    internal IQueryable<ParticipantTotalVestingBalance> TotalVestingBalance(
        IProfitSharingDbContext ctx,
        short profitYear,
        DateOnly asOfDate)
    {
        return _embeddedSqlService.TotalVestingBalanceAlt(
            ctx,
            profitYear,
            profitYear,
            asOfDate);
    }
}
```

### 2. Demographic Services

**Purpose**: Manage employee/beneficiary demographic data

| Service                    | Responsibility                                |
| -------------------------- | --------------------------------------------- |
| `DemographicReaderService` | Frozen vs. live demographic abstraction       |
| `EmployeeLookupService`    | Search employees by various criteria          |
| `BeneficiaryService`       | CRUD operations for beneficiaries             |
| `MasterInquiryService`     | Unified search across employees/beneficiaries |

**Critical Pattern - Frozen Demographics**:

```csharp
public sealed class DemographicReaderService : IDemographicReaderService
{
    private readonly IFrozenService _frozenService;
    private readonly IHttpContextAccessor _http;

    public async Task<IQueryable<Demographic>> BuildDemographicQuery(
        IProfitSharingDbContext ctx,
        bool useFrozenData = false)
    {
        if (useFrozenData)
        {
            // FROZEN: Use point-in-time snapshot for year-end consistency
            var frozenState = await _frozenService.GetActiveFrozenDemographic();

            // Store metadata in HttpContext for header middleware
            _http.HttpContext!.Items[ItemKey] = new DataWindowMetadata(
                IsFrozen: true,
                ProfitYear: frozenState.ProfitYear,
                WindowEnd: frozenState.AsOfDateTime);

            return FrozenService.GetDemographicSnapshot(ctx, frozenState.ProfitYear);
        }

        // LIVE: Use current demographic data
        _http.HttpContext!.Items[ItemKey] = new DataWindowMetadata(
            IsFrozen: false,
            ProfitYear: null,
            WindowEnd: DateTimeOffset.UtcNow);

        return ctx.Demographics;
    }
}
```

**Why Frozen Demographics?**

- Year-end calculations require stable demographic data
- Reports must show consistent results even as employees change
- Regulatory compliance requires point-in-time snapshots

### 3. Report Services

**Purpose**: Generate profit-sharing reports and summaries

| Service                           | Report Type                        |
| --------------------------------- | ---------------------------------- |
| `ReportRunnerService`             | Orchestrates all report generation |
| `BreakdownReportService`          | Store breakdown reports            |
| `FrozenReportService`             | Frozen demographic reports         |
| `TerminatedEmployeeReportService` | Termination reports                |
| `ExecutiveHoursAndDollarsService` | Executive compensation             |
| `CleanupReportService`            | Data cleanup and integrity         |

**Example - ReportRunnerService**:

```csharp
public class ReportRunnerService : IReportRunnerService
{
    private readonly Dictionary<string, Func<CancellationToken, Task<Dictionary<string, object>>>> _reports;

    public ReportRunnerService(
        ITerminatedEmployeeService terminatedEmployeeService,
        IProfitShareUpdateService profitShareUpdateService,
        // ... inject all report services
        )
    {
        short wallClockYear = (short)DateTime.Now.Year;

        // Register all reports in dictionary
        _reports = new Dictionary<string, Func<CancellationToken, Task<Dictionary<string, object>>>>
        {
            ["terminations"] = async ct => await Handle("terminations", ct, async () =>
            {
                var result = await terminatedEmployeeService
                    .GetReportAsync(StartAndEndDateRequest.RequestExample(), ct);
                return (result.Response.Total, result.Response.Results.Count());
            }),

            ["profitShareUpdate"] = async ct => await Handle("profitShareUpdate", ct, async () =>
            {
                var result = await profitShareUpdateService
                    .ProfitShareUpdate(ProfitShareUpdateRequest.RequestExample(), ct);
                return (result.Response.Total, result.Response.Results.Count());
            }),

            // ... more reports
        };

        // Special "all" entry runs all reports
        _reports["all"] = async ct =>
        {
            var allResults = new List<Dictionary<string, object>>();
            foreach (var kvp in _reports.Where(r => r.Key != "all"))
            {
                allResults.Add(await kvp.Value(ct));
            }
            return new Dictionary<string, object>
            {
                ["name"] = "all",
                ["reports"] = allResults
            };
        };
    }

    public async Task<Dictionary<string, object>> IncludeReportInformation(
        string? reportSelector,
        CancellationToken ct)
    {
        if (reportSelector != null && _reports.TryGetValue(reportSelector, out var runner))
        {
            return await runner(ct);
        }
        return new Dictionary<string, object> { ["error"] = "report not found" };
    }
}
```

### 4. Validation Services

**Purpose**: Enforce data integrity and cross-reference checks

| Service                            | Validation Type                                     |
| ---------------------------------- | --------------------------------------------------- |
| `ChecksumValidationService`        | SHA256 checksum validation against archived reports |
| `CrossReferenceValidationService`  | Cross-report field validation orchestration         |
| `ArchivedValueService`             | Retrieve archived values for comparison             |
| `AllocTransferValidationService`   | Allocation transfer validation                      |
| `BalanceEquationValidationService` | Balance equation integrity checks                   |

**Key Innovation - Checksum-Based Validation**:

The validation services use SHA256 checksums to detect data drift between current calculations and archived year-end reports. This ensures regulatory compliance and prevents silent data corruption.

```csharp
// Archive report with checksums during year-end
var reportChecksum = new ReportChecksum
{
    ProfitYear = profitYear,
    ReportType = "BreakdownReport",
    KeyFieldsChecksumJson = new Dictionary<string, KeyValuePair<decimal, byte[]>>
    {
        ["TotalContributions"] = new KeyValuePair<decimal, byte[]>(
            12345.67m,
            SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(12345.67m))
        ),
        ["TotalEarnings"] = new KeyValuePair<decimal, byte[]>(
            98765.43m,
            SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(98765.43m))
        )
    },
    CreatedAtUtc = DateTimeOffset.UtcNow
};
await ctx.ReportChecksums.AddAsync(reportChecksum, ct);

// Validate report later
var validationResult = await _checksumValidationService.ValidateReportFieldsAsync(
    profitYear: 2024,
    reportType: "BreakdownReport",
    fieldsToValidate: new Dictionary<string, decimal>
    {
        ["TotalContributions"] = 12345.67m,
        ["TotalEarnings"] = 98765.43m
    },
    ct);

if (!validationResult.Value.IsValid)
{
    // Data drift detected - investigate!
}
```

### 5. Caching Services

**Purpose**: Improve performance with distributed caching

| Service                              | Cache Type                                           |
| ------------------------------------ | ---------------------------------------------------- |
| `LookupCache<TKey, TValue, TEntity>` | Generic version-based lookup cache                   |
| `StateTaxCache`                      | State tax lookup cache                               |
| `ProfitCodeCache`                    | Profit code lookup cache                             |
| `NavigationService`                  | Role-based navigation tree with version invalidation |

**Version-Based Cache Invalidation**:

```csharp
public class LookupCache<TKey, TValue, TEntity> : ILookupCache<TKey, TValue>
{
    private readonly IDistributedCache _cache;
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly string _lookupName;

    private string VersionKey => $"lookup:{_lookupName}:version";
    private string GetDataKey(int version) => $"lookup:{_lookupName}:data:v{version}";

    public async ValueTask<IReadOnlyDictionary<TKey, TValue>> GetAllAsync(CancellationToken ct)
    {
        // 1. Get current version (or initialize to 1)
        var version = await GetOrInitializeVersionAsync(ct);
        var dataKey = GetDataKey(version);

        // 2. Try cache
        var cachedBytes = await _cache.GetAsync(dataKey, ct);
        if (cachedBytes != null)
        {
            return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(cachedBytes)!;
        }

        // 3. Load from DB
        var data = await LoadFromDatabaseAsync(ct);

        // 4. Cache with version key
        await _cache.SetAsync(
            dataKey,
            JsonSerializer.SerializeToUtf8Bytes(data),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            },
            ct);

        return data;
    }

    public async Task InvalidateAsync(CancellationToken ct)
    {
        // Increment version counter (no expiration - persists indefinitely)
        var currentVersion = await GetOrInitializeVersionAsync(ct);
        var newVersion = currentVersion + 1;

        await _cache.SetStringAsync(
            VersionKey,
            newVersion.ToString(),
            new DistributedCacheEntryOptions(), // No expiration
            ct);

        // Old versioned keys become unreachable (automatic invalidation)
    }
}
```

**Benefits**:

- No pattern-based cache deletion needed
- Version increments invalidate all cached data instantly
- Version counter persists indefinitely (small 4-byte value)
- Graceful degradation on cache failures

### 6. Middleware Services

**Purpose**: Cross-cutting concerns applied via ASP.NET middleware

| Service                             | Middleware Responsibility                                |
| ----------------------------------- | -------------------------------------------------------- |
| `RoleContextMiddleware`             | Capture user roles for JSON serialization masking        |
| `DemographicHeaderMiddleware`       | Add frozen/live demographic metadata to response headers |
| `EndpointInstrumentationMiddleware` | Telemetry and request tracking                           |

**Example - RoleContextMiddleware**:

```csharp
public sealed class RoleContextMiddleware
{
    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Extract roles from claims
            var rolesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var claim in context.User.Claims.Where(c =>
                c.Type == ClaimTypes.Role || c.Type == "roles"))
            {
                rolesSet.Add(claim.Value);
            }

            // Check for privileged roles
            bool isItDevOps = rolesSet.Contains(Role.ITDEVOPS);
            bool isExecAdmin = rolesSet.Contains(Role.EXECUTIVEADMIN);

            // Store in thread-local context for JSON serialization
            MaskingAmbientRoleContext.Current = new RoleContextSnapshot(
                rolesSet.ToArray(),
                isItDevOps,
                isExecAdmin);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            MaskingAmbientRoleContext.Clear();
        }
    }
}
```

**Why This Pattern?**

- JSON serialization happens deep in the stack (System.Text.Json converters)
- Converters don't have access to HttpContext
- Thread-local storage provides ambient context
- Middleware ensures cleanup after request

---

## Data Access Patterns

### Pattern 1: Read-Only Context with AsNoTracking

**Use When**: Querying data without modification

```csharp
public async Task<List<EmployeeDto>> GetActiveEmployeesAsync(CancellationToken ct)
{
    return await _dataContextFactory.UseReadOnlyContext(async ctx =>
    {
        ctx.UseReadOnlyContext(); // Auto-applies AsNoTracking

        var demographicQuery = await _demographicReaderService
            .BuildDemographicQuery(ctx, useFrozenData: false);

        return await demographicQuery
            .TagWith("GetActiveEmployees")
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Active)
            .Select(d => new EmployeeDto
            {
                BadgeNumber = d.BadgeNumber,
                FullName = d.ContactInfo.FullName,
                HireDate = d.HireDate
            })
            .ToListAsync(ct);
    }, ct);
}
```

**Performance Benefits**:

- No change tracking overhead (~30% faster)
- Reduced memory allocation
- Ideal for read-heavy operations

### Pattern 2: Writable Context with Automatic Transaction

**Use When**: Creating, updating, or deleting entities

```csharp
public async Task<CreateEmployeeResponse> CreateEmployeeAsync(
    CreateEmployeeRequest req,
    CancellationToken ct)
{
    return await _dataContextFactory.UseWritableContextAsync(
        async (ctx, transaction) =>
    {
        // Transaction automatically started

        // 1. Validate
        if (await ctx.Demographics.AnyAsync(d => d.Ssn == req.Ssn, ct))
        {
            throw new InvalidOperationException("SSN already exists");
        }

        // 2. Create entity
        var demographic = new Demographic
        {
            Ssn = req.Ssn,
            BadgeNumber = req.BadgeNumber,
            HireDate = req.HireDate,
            ContactInfo = new ContactInfo
            {
                FirstName = req.FirstName,
                LastName = req.LastName
            }
        };

        ctx.Demographics.Add(demographic);

        // 3. Save
        await ctx.SaveChangesAsync(ct);

        // 4. Commit (optional - auto-commits if no exception)
        await transaction.CommitAsync(ct);

        // 5. Return response
        return new CreateEmployeeResponse { DemographicId = demographic.Id };

    }, ct); // Transaction auto-rolls back on exception
}
```

**Transaction Guarantees**:

- Isolation level: Read Committed (Oracle default)
- Automatic rollback on exception
- No need for explicit `try/catch` for rollback
- Can nest service calls within same transaction

### Pattern 3: Bulk Updates with ExecuteUpdateAsync

**Use When**: Updating many rows without loading entities into memory

```csharp
public async Task ResetNavigationStatusesAsync(short profitYear, CancellationToken ct)
{
    await _dataContextFactory.UseWritableContext(async ctx =>
    {
        // Bulk update - single SQL statement, no entity loading
        var rowsUpdated = await ctx.Navigations
            .TagWith($"ResetNavigationStatuses-{profitYear}")
            .Where(n => n.StatusId != NavigationStatus.Constants.NotStarted)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    n => n.StatusId,
                    NavigationStatus.Constants.NotStarted),
                ct);

        _logger.LogInformation("Reset {RowCount} navigation statuses", rowsUpdated);
        return rowsUpdated;
    }, ct);
}
```

**Performance Comparison**:

- **Old Way (Load + Update)**: `SELECT * FROM navigations; UPDATE navigations SET ...` (N roundtrips)
- **New Way (ExecuteUpdate)**: `UPDATE navigations SET ... WHERE ...` (1 roundtrip)
- **Speed**: 10-100x faster for large datasets

### Pattern 4: Dynamic Filtering with Expression Trees

**Use When**: Building queries based on runtime conditions

```csharp
public async Task<List<MemberDto>> SearchMembersAsync(
    MemberSearchRequest req,
    CancellationToken ct)
{
    return await _dataContextFactory.UseReadOnlyContext(async ctx =>
    {
        ctx.UseReadOnlyContext();

        var query = ctx.Members.AsQueryable();

        // Dynamic filtering based on request
        if (req.BadgeNumber.HasValue)
        {
            query = query.Where(m => m.BadgeNumber == req.BadgeNumber.Value);
        }

        if (!string.IsNullOrEmpty(req.LastName))
        {
            query = query.Where(m => EF.Functions.Like(
                m.ContactInfo.LastName,
                $"%{req.LastName}%"));
        }

        if (req.HireDateFrom.HasValue)
        {
            query = query.Where(m => m.HireDate >= req.HireDateFrom.Value);
        }

        if (req.EmploymentStatusIds?.Any() == true)
        {
            query = query.Where(m => req.EmploymentStatusIds.Contains(m.EmploymentStatusId));
        }

        // Apply sorting
        query = req.SortBy switch
        {
            "BadgeNumber" => query.OrderBy(m => m.BadgeNumber),
            "LastName" => query.OrderBy(m => m.ContactInfo.LastName),
            "HireDate" => query.OrderBy(m => m.HireDate),
            _ => query.OrderBy(m => m.BadgeNumber)
        };

        // Pagination
        return await query
            .TagWith($"SearchMembers-{req.SortBy}")
            .Skip(req.Offset)
            .Take(req.PageSize)
            .Select(m => new MemberDto { /* ... */ })
            .ToListAsync(ct);
    }, ct);
}
```

### Pattern 5: Precompute Lookups for Batch Operations

**Use When**: Processing large batches with related data lookups

```csharp
public async Task ProcessPayProfitBatchAsync(
    List<PayProfitDto> payProfits,
    CancellationToken ct)
{
    await _dataContextFactory.UseWritableContextAsync(async (ctx, tx) =>
    {
        // 1. Precompute all lookups before looping (avoid N+1)
        var demographicIds = payProfits.Select(pp => pp.DemographicId).Distinct().ToHashSet();

        var demographicLookup = await ctx.Demographics
            .Where(d => demographicIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d, ct);

        var ssnSet = demographicLookup.Values.Select(d => d.Ssn).ToHashSet();

        var balanceLookup = await _totalService.GetTotalBalanceSet(ctx, 2024)
            .Where(b => ssnSet.Contains(b.Ssn))
            .ToDictionaryAsync(b => b.Ssn, b => b.TotalAmount, ct);

        // 2. Process batch using precomputed lookups (fast in-memory)
        foreach (var payProfit in payProfits)
        {
            var demographic = demographicLookup[payProfit.DemographicId];
            var balance = balanceLookup.GetValueOrDefault(demographic.Ssn, 0);

            // Process with preloaded data
            payProfit.CurrentBalance = balance;
            payProfit.CalculatedContribution = CalculateContribution(payProfit, balance);
        }

        // 3. Save all changes in one batch
        await ctx.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

    }, ct);
}
```

**Performance Impact**:

- **Before**: 1000 pay profits × 2 queries each = 2000 DB roundtrips
- **After**: 2 precompute queries + 1 batch save = 3 DB roundtrips
- **Speed**: ~600x faster

### Pattern 6: Query Tagging for Production Traceability

**Use When**: Always (for all complex queries)

```csharp
public async Task<ReportDto> GenerateYearEndReportAsync(short year, CancellationToken ct)
{
    return await _dataContextFactory.UseReadOnlyContext(async ctx =>
    {
        ctx.UseReadOnlyContext();

        // Tag with business context for Oracle tracing
        var data = await ctx.ProfitDetails
            .TagWith($"YearEndReport-{year}-User:{_appUser.UserName}")
            .Where(pd => pd.ProfitYear == year)
            .GroupBy(pd => pd.Ssn)
            .Select(g => new { Ssn = g.Key, Total = g.Sum(pd => pd.Contribution) })
            .ToListAsync(ct);

        return new ReportDto { Year = year, Data = data };
    }, ct);
}
```

**Benefits**:

- Visible in Oracle's V$SQL and AWR reports
- Correlates queries to application operations
- Helps DBAs identify slow queries
- Required for complex year-end operations

### Pattern 7: Avoid Null-Coalescing in LINQ Queries (Oracle Issue)

**CRITICAL**: Oracle EF Core provider has translation issues with `??` operator

```csharp
// ❌ BAD - Oracle provider may fail
var query = ctx.Members
    .Select(m => new MemberDto
    {
        MiddleName = m.ContactInfo.MiddleName ?? "N/A" // ❌ May fail with Oracle
    });

// ✅ GOOD - Use explicit conditional
var query = ctx.Members
    .Select(m => new MemberDto
    {
        MiddleName = m.ContactInfo.MiddleName != null
            ? m.ContactInfo.MiddleName
            : "N/A"
    });

// ✅ ALTERNATIVE - Materialize to memory first (if dataset small)
var members = await ctx.Members.AsEnumerable()
    .Select(m => new MemberDto
    {
        MiddleName = m.ContactInfo.MiddleName ?? "N/A" // ✅ Safe in LINQ-to-Objects
    })
    .ToList();
```

---

## Caching Strategies

### Strategy 1: Version-Based Invalidation (Recommended)

**Use When**: Caching lookup data with infrequent updates

```csharp
public class StateTaxCache
{
    private readonly LookupCache<string, StateTaxDto, StateTax> _cache;

    public StateTaxCache(
        IDistributedCache cache,
        IProfitSharingDataContextFactory contextFactory,
        ILogger<StateTaxCache> logger)
    {
        _cache = new LookupCache<string, StateTaxDto, StateTax>(
            cache,
            contextFactory,
            logger,
            lookupName: "StateTaxes",
            queryBuilder: q => q.Where(st => st.IsActive),
            keySelector: st => st.StateCode,
            valueSelector: st => new StateTaxDto
            {
                StateCode = st.StateCode,
                TaxRate = st.TaxRate
            },
            getDbSetFunc: ctx => ctx.StateTaxes,
            absoluteExpiration: TimeSpan.FromHours(4)
        );
    }

    public async ValueTask<StateTaxDto?> GetByStateAsync(string stateCode, CancellationToken ct)
    {
        return await _cache.GetAsync(stateCode, ct);
    }

    public async Task InvalidateAsync(CancellationToken ct)
    {
        // Increments version counter - all cached data instantly invalid
        await _cache.InvalidateAsync(ct);
    }
}
```

**How It Works**:

1. Version counter stored in Redis: `lookup:StateTaxes:version = 5`
2. Cache key includes version: `lookup:StateTaxes:data:v5`
3. On invalidate, version increments to 6
4. Next request uses key `lookup:StateTaxes:data:v6` (cache miss, reload from DB)
5. Old key `v5` becomes unreachable (no manual deletion needed)

### Strategy 2: Absolute + Sliding Expiration

**Use When**: Caching frequently accessed data with moderate update frequency

```csharp
public async Task<CalendarResponseDto> GetYearDatesAsync(short year, CancellationToken ct)
{
    var cacheKey = $"CalendarService_YearDates_{year}";

    // Try cache first
    var cached = await _distributedCache.GetAsync(cacheKey, ct);
    if (cached != null)
    {
        return JsonSerializer.Deserialize<CalendarResponseDto>(cached)!;
    }

    // Load from DB
    var result = await LoadFromDatabase(year, ct);

    // Cache with combined expiration
    await _distributedCache.SetAsync(
        cacheKey,
        JsonSerializer.SerializeToUtf8Bytes(result),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4), // Max 4 hours
            SlidingExpiration = TimeSpan.FromHours(1) // Extend if accessed
        },
        ct);

    return result;
}
```

**Behavior**:

- **Absolute**: Cache entry removed after 4 hours regardless of access
- **Sliding**: If accessed within 1 hour, expiration extends by 1 hour (up to 4 hour max)
- **Result**: Frequently accessed data stays cached; stale data expires

### Strategy 3: Cache Warming on Startup

**Use When**: Critical lookup data needed immediately on app start

```csharp
public class StateTaxCacheWarmerHostedService : IHostedService
{
    private readonly StateTaxCache _cache;
    private readonly ILogger<StateTaxCacheWarmerHostedService> _logger;

    public async Task StartAsync(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Warming StateTax cache...");

            // Force cache load on startup
            var allStateTaxes = await _cache.GetAllAsync(ct);

            _logger.LogInformation(
                "StateTax cache warmed with {Count} entries",
                allStateTaxes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to warm StateTax cache");
            // Don't throw - allow app to start even if cache warming fails
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}

// Register in ServicesExtension.cs
if (!builder.Environment.IsTestEnvironment())
{
    builder.Services.AddHostedService<StateTaxCacheWarmerHostedService>();
}
```

### Strategy 4: Graceful Degradation on Cache Failure

**Always implement** - cache should enhance performance, not break functionality

```csharp
public async ValueTask<IReadOnlyDictionary<TKey, TValue>> GetAllAsync(CancellationToken ct)
{
    try
    {
        // Try cache first
        var version = await GetOrInitializeVersionAsync(ct);
        var cached = await _cache.GetAsync(GetDataKey(version), ct);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(cached)!;
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Cache read failed, falling back to database");
        // Continue to database load
    }

    // Cache miss or failure - load from DB
    var data = await LoadFromDatabaseAsync(ct);

    // Try to cache (but don't fail if cache write fails)
    try
    {
        await SetCacheAsync(dataKey, data, ct);
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Cache write failed (non-fatal)");
    }

    return data;
}
```

---

## Validation & Business Rules

### Rule Enforcement Patterns

#### 1. Input Validation (FluentValidation in Endpoints)

**Where**: Endpoint validators (not in services)

```csharp
// Endpoint validator (not shown in Services layer)
public class CreateBeneficiaryRequestValidator : AbstractValidator<CreateBeneficiaryRequest>
{
    public CreateBeneficiaryRequestValidator()
    {
        RuleFor(x => x.Percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Percentage must be between 0 and 100");

        RuleFor(x => x.EmployeeBadgeNumber)
            .GreaterThan(0)
            .WithMessage("Badge number must be positive");
    }
}
```

#### 2. Business Rule Validation (In Services)

**Where**: Service methods before persistence

```csharp
public async Task<CreateBeneficiaryResponse> CreateBeneficiary(
    CreateBeneficiaryRequest req,
    CancellationToken ct)
{
    return await _dataContextFactory.UseWritableContextAsync(async (ctx, tx) =>
    {
        // Business rule: Beneficiary contact must exist
        var contact = await ctx.BeneficiaryContacts
            .FirstOrDefaultAsync(x => x.Id == req.BeneficiaryContactId, ct);
        if (contact == null)
        {
            throw new InvalidOperationException("Beneficiary Contact does not exist");
        }

        // Business rule: Employee must exist
        var demographic = await _demographicReaderService
            .BuildDemographicQuery(ctx, false)
            .Where(x => x.BadgeNumber == req.EmployeeBadgeNumber)
            .SingleOrDefaultAsync(ct);
        if (demographic == null)
        {
            throw new InvalidOperationException("Employee Badge does not exist");
        }

        // Business rule: Total beneficiary percentages cannot exceed 100%
        var existingTotal = await ctx.Beneficiaries
            .Where(b => b.BadgeNumber == req.EmployeeBadgeNumber)
            .SumAsync(b => b.Percent, ct);

        if (existingTotal + req.Percentage > 100)
        {
            throw new InvalidOperationException(
                $"Total percentage would exceed 100% (current: {existingTotal})");
        }

        // Rules passed - create entity
        var beneficiary = new Beneficiary { /* ... */ };
        ctx.Add(beneficiary);
        await ctx.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new CreateBeneficiaryResponse { /* ... */ };
    }, ct);
}
```

#### 3. Cross-Entity Validation

**Where**: Validation services for complex checks

```csharp
public async Task<Result<ValidationResponse>> ValidateYearEndConsistencyAsync(
    short profitYear,
    CancellationToken ct)
{
    var errors = new List<string>();

    // Rule 1: Total contributions = sum of individual contributions
    var (totalContributions, individualSum) = await _dataContextFactory
        .UseReadOnlyContext(async ctx =>
    {
        var total = await ctx.ProfitSharingTotals
            .Where(t => t.ProfitYear == profitYear)
            .SumAsync(t => t.TotalContributions, ct);

        var individual = await ctx.ProfitDetails
            .Where(pd => pd.ProfitYear == profitYear &&
                        pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions)
            .SumAsync(pd => pd.Contribution, ct);

        return (total, individual);
    }, ct);

    if (Math.Abs(totalContributions - individualSum) > 0.01m)
    {
        errors.Add($"Total contributions mismatch: {totalContributions} vs {individualSum}");
    }

    // Rule 2: No negative balances (except ETVA)
    var negativeBalances = await _dataContextFactory.UseReadOnlyContext(ctx =>
        _totalService.GetTotalBalanceSet(ctx, profitYear)
            .Where(b => b.TotalAmount < 0)
            .CountAsync(ct), ct);

    if (negativeBalances > 0)
    {
        errors.Add($"Found {negativeBalances} negative balances");
    }

    // Return validation result
    return errors.Any()
        ? Result<ValidationResponse>.Failure(Error.Validation(errors))
        : Result<ValidationResponse>.Success(new ValidationResponse { IsValid = true });
}
```

---

## Error Handling & Result Types

### Pattern 1: Result<T> for Domain Operations

**Use When**: Service methods that may fail for business reasons

```csharp
public async Task<Result<ChecksumValidationResponse>> ValidateReportFieldsAsync(
    short profitYear,
    string reportType,
    Dictionary<string, decimal> fieldsToValidate,
    CancellationToken ct)
{
    // Input validation
    if (string.IsNullOrWhiteSpace(reportType))
    {
        return Result<ChecksumValidationResponse>.Failure(
            Error.Validation(new Dictionary<string, string[]>
            {
                [nameof(reportType)] = new[] { "Report type cannot be null or empty." }
            }));
    }

    try
    {
        // Query for archived checksums
        var archived = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.ReportChecksums
                .Where(r => r.ProfitYear == profitYear && r.ReportType == reportType)
                .FirstOrDefaultAsync(ct), ct);

        if (archived == null)
        {
            return Result<ChecksumValidationResponse>.Failure(
                Error.EntityNotFound($"No archived report for {reportType} year {profitYear}"));
        }

        // Perform validation
        var response = PerformChecksumValidation(fieldsToValidate, archived);

        return Result<ChecksumValidationResponse>.Success(response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error validating checksums for {ReportType}", reportType);

        return Result<ChecksumValidationResponse>.Failure(
            Error.Unexpected($"Failed to validate checksums: {ex.Message}"));
    }
}
```

**Endpoint Consumption**:

```csharp
public override async Task<Results<Ok<ChecksumValidationResponse>, NotFound, ProblemHttpResult>>
    ExecuteAsync(ValidateReportRequest req, CancellationToken ct)
{
    var result = await _validationService.ValidateReportFieldsAsync(
        req.ProfitYear,
        req.ReportType,
        req.FieldsToValidate,
        ct);

    // Convert Result<T> to HTTP response
    return result.ToHttpResult(Error.EntityNotFound);
}
```

### Pattern 2: Exceptions for Exceptional Conditions

**Use When**: Truly exceptional conditions (programmer errors, infrastructure failures)

```csharp
public async Task ProcessYearEndAsync(short profitYear, CancellationToken ct)
{
    // Validate year is within acceptable range
    if (profitYear < 2000 || profitYear > DateTime.UtcNow.Year + 1)
    {
        throw new ArgumentOutOfRangeException(
            nameof(profitYear),
            $"Profit year {profitYear} is invalid");
    }

    await _dataContextFactory.UseWritableContextAsync(async (ctx, tx) =>
    {
        // Critical operation - if DB connection fails, throw
        var payProfits = await ctx.PayProfits
            .Where(pp => pp.ProfitYear == profitYear)
            .ToListAsync(ct); // May throw SqlException

        // Business logic...

        await ctx.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

    }, ct); // Transaction auto-rolls back on exception
}
```

### Pattern 3: Structured Logging for Diagnostics

```csharp
public async Task<BalanceEndpointResponse?> GetVestingBalanceAsync(
    int badgeNumber,
    short profitYear,
    CancellationToken ct)
{
    _logger.LogInformation(
        "Retrieving vesting balance for badge {BadgeNumber}, year {ProfitYear}",
        badgeNumber,
        profitYear);

    try
    {
        var result = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Query logic...
        }, ct);

        if (result == null)
        {
            _logger.LogWarning(
                "No vesting balance found for badge {BadgeNumber}, year {ProfitYear}",
                badgeNumber,
                profitYear);
        }
        else
        {
            _logger.LogDebug(
                "Vesting balance for badge {BadgeNumber}: {VestedBalance:C}",
                badgeNumber,
                result.VestedBalance);
        }

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error retrieving vesting balance for badge {BadgeNumber}, year {ProfitYear}",
            badgeNumber,
            profitYear);
        throw;
    }
}
```

**Log Levels**:

- **LogDebug**: Detailed diagnostic info (disabled in production)
- **LogInformation**: General flow (method entry/exit, key decisions)
- **LogWarning**: Unexpected but recoverable (missing data, fallback logic)
- **LogError**: Failures requiring investigation
- **LogCritical**: Data integrity issues (duplicate SSNs, balance mismatches)

---

## Transaction Management

### Pattern 1: Single-Context Transaction (Most Common)

**Automatic transaction via UseWritableContextAsync**:

```csharp
public async Task UpdateEmployeeStatusAsync(int badgeNumber, int newStatusId, CancellationToken ct)
{
    await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
    {
        // 1. Find employee
        var employee = await ctx.Demographics
            .FirstOrDefaultAsync(d => d.BadgeNumber == badgeNumber, ct);
        if (employee == null)
        {
            throw new InvalidOperationException("Employee not found");
        }

        // 2. Update status
        employee.EmploymentStatusId = newStatusId;
        employee.StatusChangeDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // 3. Create audit record
        var audit = new EmploymentStatusAudit
        {
            DemographicId = employee.Id,
            OldStatusId = employee.EmploymentStatusId,
            NewStatusId = newStatusId,
            ChangedDate = DateTime.UtcNow,
            ChangedBy = _appUser.UserName
        };
        ctx.EmploymentStatusAudits.Add(audit);

        // 4. Save (all changes in same transaction)
        await ctx.SaveChangesAsync(ct);

        // 5. Transaction auto-commits if no exception
        // Can explicitly commit: await transaction.CommitAsync(ct);

    }, ct); // Auto-rollback on exception
}
```

### Pattern 2: Multiple SaveChanges in Transaction

**Use When**: Intermediate saves needed for generated keys

```csharp
public async Task CreateEmployeeWithBeneficiariesAsync(
    CreateEmployeeRequest req,
    CancellationToken ct)
{
    await _dataContextFactory.UseWritableContextAsync(async (ctx, transaction) =>
    {
        // 1. Create employee (need ID for beneficiaries)
        var employee = new Demographic { /* ... */ };
        ctx.Demographics.Add(employee);
        await ctx.SaveChangesAsync(ct); // Generate employee.Id

        // 2. Create beneficiaries using generated ID
        foreach (var benefReq in req.Beneficiaries)
        {
            var beneficiary = new Beneficiary
            {
                DemographicId = employee.Id, // Use generated ID
                BeneficiaryContactId = benefReq.ContactId,
                Percent = benefReq.Percent
            };
            ctx.Beneficiaries.Add(beneficiary);
        }
        await ctx.SaveChangesAsync(ct);

        // 3. Create initial PayProfit record
        var payProfit = new PayProfit
        {
            DemographicId = employee.Id,
            ProfitYear = (short)DateTime.UtcNow.Year,
            CurrentHoursYear = 0
        };
        ctx.PayProfits.Add(payProfit);
        await ctx.SaveChangesAsync(ct);

        // All 3 saves in same transaction - all commit together
        await transaction.CommitAsync(ct);

    }, ct); // All rolled back together on exception
}
```

### Pattern 3: Distributed Transaction (Rare - Avoid if Possible)

**Use When**: Must coordinate across multiple databases (Oracle + SQL Server)

```csharp
// ⚠️ ADVANCED PATTERN - Use only when absolutely necessary
public async Task SynchronizeTwoSystemsAsync(SyncRequest req, CancellationToken ct)
{
    using var scope = new TransactionScope(
        TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
        TransactionScopeAsyncFlowOption.Enabled);

    try
    {
        // Update Oracle database
        await _profitSharingDataContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await ctx.Demographics
                .FirstAsync(d => d.BadgeNumber == req.BadgeNumber, ct);
            demographic.LastSyncDate = DateTime.UtcNow;
            await ctx.SaveChangesAsync(ct);
        }, ct);

        // Update SQL Server database (hypothetical)
        await _externalSystemContextFactory.UseWritableContext(async ctx =>
        {
            var externalRecord = await ctx.ExternalEmployees
                .FirstAsync(e => e.BadgeNumber == req.BadgeNumber, ct);
            externalRecord.SyncStatus = "Completed";
            await ctx.SaveChangesAsync(ct);
        }, ct);

        // Commit distributed transaction
        scope.Complete();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Distributed transaction failed, rolling back both systems");
        // TransactionScope auto-rolls back both contexts
        throw;
    }
}
```

**⚠️ Warning**: Distributed transactions are complex, slow, and can cause deadlocks. Prefer eventual consistency patterns (message queues, outbox pattern) whenever possible.

---

## Performance Optimization

### Optimization 1: Projection to DTOs

**Don't**: Load full entities when you only need a few fields

```csharp
// ❌ BAD - Loads all columns, tracks entities
var employees = await ctx.Demographics
    .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Active)
    .ToListAsync(ct);

var dtos = employees.Select(e => new EmployeeDto
{
    BadgeNumber = e.BadgeNumber,
    FullName = e.ContactInfo.FullName
}).ToList();
```

**Do**: Project to DTOs in database query

```csharp
// ✅ GOOD - Selects only needed columns, no tracking
var dtos = await ctx.Demographics
    .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Active)
    .Select(e => new EmployeeDto
    {
        BadgeNumber = e.BadgeNumber,
        FullName = e.ContactInfo.FullName
    })
    .ToListAsync(ct);
```

**Performance**: 3-5x faster, 80% less memory

### Optimization 2: Avoid N+1 Queries

**Don't**: Loop with individual queries

```csharp
// ❌ BAD - N+1 queries
var employees = await ctx.Demographics.Take(100).ToListAsync(ct);
foreach (var emp in employees)
{
    var balance = await ctx.ProfitDetails
        .Where(pd => pd.Ssn == emp.Ssn)
        .SumAsync(pd => pd.Contribution, ct); // 100 queries!
}
```

**Do**: Precompute all lookups before loop

```csharp
// ✅ GOOD - 2 queries total
var employees = await ctx.Demographics.Take(100).ToListAsync(ct);
var ssns = employees.Select(e => e.Ssn).ToHashSet();

var balanceLookup = await ctx.ProfitDetails
    .Where(pd => ssns.Contains(pd.Ssn))
    .GroupBy(pd => pd.Ssn)
    .Select(g => new { Ssn = g.Key, Balance = g.Sum(pd => pd.Contribution) })
    .ToDictionaryAsync(x => x.Ssn, x => x.Balance, ct);

foreach (var emp in employees)
{
    var balance = balanceLookup.GetValueOrDefault(emp.Ssn, 0); // In-memory lookup
}
```

**Performance**: 100x faster for 100 employees

### Optimization 3: Bulk Operations

**Don't**: Load entities for bulk updates

```csharp
// ❌ BAD - Loads 10,000 entities into memory
var payProfits = await ctx.PayProfits
    .Where(pp => pp.ProfitYear == 2024)
    .ToListAsync(ct); // 10,000 rows loaded

foreach (var pp in payProfits)
{
    pp.ZeroContributionReasonId = 0;
}
await ctx.SaveChangesAsync(ct); // Sends 10,000 UPDATE statements
```

**Do**: Use ExecuteUpdateAsync for single SQL statement

```csharp
// ✅ GOOD - Single UPDATE statement, no entity loading
var rowsUpdated = await ctx.PayProfits
    .Where(pp => pp.ProfitYear == 2024)
    .ExecuteUpdateAsync(
        setters => setters.SetProperty(pp => pp.ZeroContributionReasonId, 0),
        ct);
```

**Performance**: 100-500x faster for large datasets

### Optimization 4: Asynchronous Streaming

**Don't**: Load entire large result set into memory

```csharp
// ❌ BAD - Loads 100,000 rows into memory
var allDetails = await ctx.ProfitDetails
    .Where(pd => pd.ProfitYear == 2024)
    .ToListAsync(ct); // 100,000 rows × 200 bytes = 20MB

await ProcessBatchAsync(allDetails); // All in memory at once
```

**Do**: Stream and process in batches

```csharp
// ✅ GOOD - Processes in batches, constant memory
await foreach (var batch in ctx.ProfitDetails
    .Where(pd => pd.ProfitYear == 2024)
    .AsAsyncEnumerable()
    .Buffer(1000) // 1000 rows at a time
    .WithCancellation(ct))
{
    await ProcessBatchAsync(batch); // Only 1000 rows in memory
}
```

**Performance**: Constant memory usage regardless of result size

### Optimization 5: Compiled Queries (Advanced)

**Use When**: Same query executed repeatedly with different parameters

```csharp
public class TotalService
{
    // Compiled query - EF Core caches execution plan
    private static readonly Func<ProfitSharingReadOnlyDbContext, int, short, Task<decimal?>>
        _getBalanceCompiledQuery = EF.CompileAsyncQuery(
            (ProfitSharingReadOnlyDbContext ctx, int ssn, short profitYear) =>
                ctx.ProfitDetails
                    .Where(pd => pd.Ssn == ssn && pd.ProfitYear <= profitYear)
                    .Sum(pd => pd.Contribution));

    public async Task<decimal> GetBalanceAsync(int ssn, short profitYear, CancellationToken ct)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Uses cached execution plan - 20-30% faster
            var balance = await _getBalanceCompiledQuery(ctx, ssn, profitYear);
            return balance ?? 0;
        }, ct);
    }
}
```

**Performance**: 20-30% faster for hot paths

---

## Testing Guidelines

### Unit Test Structure

**Test File Organization**:

```
Demoulas.ProfitSharing.Tests/
└── Services/
    ├── BeneficiaryServiceTests.cs
    ├── CalendarServiceTests.cs
    ├── TotalServiceTests.cs
    ├── Validation/
    │   └── ChecksumValidationServiceTests.cs
    └── Fixtures/
        └── ServiceTestFixture.cs
```

### Testing Pattern 1: Service with In-Memory Database

```csharp
public class BeneficiaryServiceTests
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly BeneficiaryService _sut; // System Under Test

    public BeneficiaryServiceTests()
    {
        // Use in-memory database for tests
        _contextFactory = new InMemoryDataContextFactory();
        var demographicReaderService = new DemographicReaderService(/* ... */);
        var totalService = new TotalService(/* ... */);

        _sut = new BeneficiaryService(
            _contextFactory,
            demographicReaderService,
            totalService);
    }

    [Description("PS-1234 : Create beneficiary validates percentage within range")]
    [Fact]
    public async Task CreateBeneficiary_WithValidPercentage_ShouldSucceed()
    {
        // Arrange
        await SeedTestDataAsync(); // Setup test data

        var request = new CreateBeneficiaryRequest
        {
            EmployeeBadgeNumber = 12345,
            BeneficiaryContactId = 1,
            Percentage = 50,
            Relationship = "Spouse",
            KindId = 1
        };

        // Act
        var result = await _sut.CreateBeneficiary(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Percent.ShouldBe(50);
        result.EmployeeBadgeNumber.ShouldBe(12345);
    }

    [Description("PS-1234 : Create beneficiary rejects percentage over 100")]
    [Fact]
    public async Task CreateBeneficiary_WithPercentageOver100_ShouldThrow()
    {
        // Arrange
        var request = new CreateBeneficiaryRequest
        {
            EmployeeBadgeNumber = 12345,
            BeneficiaryContactId = 1,
            Percentage = 150, // Invalid
            Relationship = "Spouse",
            KindId = 1
        };

        // Act & Assert
        var ex = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _sut.CreateBeneficiary(request, CancellationToken.None));

        ex.Message.ShouldContain("Invalid percentage");
    }

    private async Task SeedTestDataAsync()
    {
        await _contextFactory.UseWritableContextAsync(async (ctx, tx) =>
        {
            ctx.Demographics.Add(new Demographic
            {
                Id = 1,
                BadgeNumber = 12345,
                Ssn = 123456789,
                ContactInfo = new ContactInfo
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
            });

            ctx.BeneficiaryContacts.Add(new BeneficiaryContact
            {
                Id = 1,
                Ssn = 987654321,
                ContactInfo = new ContactInfo
                {
                    FirstName = "Jane",
                    LastName = "Doe"
                }
            });

            await ctx.SaveChangesAsync();
            await tx.CommitAsync();
        }, CancellationToken.None);
    }
}
```

### Testing Pattern 2: Testing Cached Services

```csharp
public class CalendarServiceTests
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly IDistributedCache _cache;
    private readonly CalendarService _sut;

    public CalendarServiceTests()
    {
        _contextFactory = new InMemoryDataContextFactory();
        _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

        _sut = new CalendarService(_contextFactory, /* ... */, _cache);
    }

    [Description("PS-1235 : Calendar service uses cache on second request")]
    [Fact]
    public async Task GetYearDates_CalledTwice_ShouldUseCacheOnSecondCall()
    {
        // Arrange
        await SeedAccountingPeriodsAsync(2024);

        // Act - First call (cache miss)
        var result1 = await _sut.GetYearStartAndEndAccountingDatesAsync(2024, CancellationToken.None);

        // Clear in-memory DB to prove second call uses cache
        await ClearDatabaseAsync();

        // Act - Second call (cache hit)
        var result2 = await _sut.GetYearStartAndEndAccountingDatesAsync(2024, CancellationToken.None);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result2.FiscalBeginDate.ShouldBe(result1.FiscalBeginDate);
        result2.FiscalEndDate.ShouldBe(result1.FiscalEndDate);
    }
}
```

### Testing Pattern 3: Testing Result<T> Services

```csharp
public class ChecksumValidationServiceTests
{
    [Description("PS-1721 : Validation returns success when checksums match")]
    [Fact]
    public async Task ValidateReportFields_MatchingChecksums_ShouldReturnSuccess()
    {
        // Arrange
        await SeedArchivedReportAsync();

        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalContributions"] = 12345.67m,
            ["TotalEarnings"] = 98765.43m
        };

        // Act
        var result = await _sut.ValidateReportFieldsAsync(
            profitYear: 2024,
            reportType: "BreakdownReport",
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.IsValid.ShouldBeTrue();
        result.Value.MismatchedFields.ShouldBeEmpty();
    }

    [Description("PS-1721 : Validation returns failure when checksums mismatch")]
    [Fact]
    public async Task ValidateReportFields_MismatchedChecksums_ShouldReturnFailure()
    {
        // Arrange
        await SeedArchivedReportAsync();

        var fieldsToValidate = new Dictionary<string, decimal>
        {
            ["TotalContributions"] = 99999.99m // Different value
        };

        // Act
        var result = await _sut.ValidateReportFieldsAsync(
            profitYear: 2024,
            reportType: "BreakdownReport",
            fieldsToValidate,
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue(); // Service succeeded
        result.Value!.IsValid.ShouldBeFalse(); // But validation failed
        result.Value.MismatchedFields.ShouldContain("TotalContributions");
    }
}
```

### Testing Best Practices

1. **Always use `[Description]` attribute** with Jira ticket number:

    ```csharp
    [Description("PS-1234 : Brief description of test scenario")]
    ```

2. **Use Shouldly assertions** (not xUnit Assert):

    ```csharp
    result.ShouldNotBeNull();
    result.Value.ShouldBe(expected);
    collection.ShouldBeEmpty();
    exception.Message.ShouldContain("error text");
    ```

3. **Test boundary cases**:

    - Minimum/maximum values
    - Empty collections
    - Null values (where applicable)
    - Edge dates (leap years, fiscal year boundaries)

4. **Isolate tests** - each test should:

    - Set up its own data
    - Not depend on other tests
    - Clean up after itself (or use fresh in-memory DB)

5. **Test negative cases**:
    - Invalid inputs
    - Missing entities
    - Business rule violations

---

## Common Pitfalls & Solutions

### Pitfall 1: Using `??` in EF LINQ Queries (Oracle)

**Problem**: Oracle EF Core provider fails to translate null-coalescing operator

```csharp
// ❌ BREAKS with Oracle
var query = ctx.Members.Select(m => new MemberDto
{
    MiddleName = m.ContactInfo.MiddleName ?? "N/A"
});
```

**Solution**: Use explicit conditional or materialize first

```csharp
// ✅ GOOD - Explicit conditional
var query = ctx.Members.Select(m => new MemberDto
{
    MiddleName = m.ContactInfo.MiddleName != null
        ? m.ContactInfo.MiddleName
        : "N/A"
});

// ✅ ALTERNATIVE - Materialize then use ??
var members = await ctx.Members.ToListAsync(ct);
var dtos = members.Select(m => new MemberDto
{
    MiddleName = m.ContactInfo.MiddleName ?? "N/A" // Safe in LINQ-to-Objects
});
```

### Pitfall 2: N+1 Queries in Loops

**Problem**: Querying database inside loops

```csharp
// ❌ N+1 queries
var employees = await ctx.Demographics.Take(100).ToListAsync(ct);
foreach (var emp in employees)
{
    var balance = await ctx.ProfitDetails
        .Where(pd => pd.Ssn == emp.Ssn)
        .SumAsync(pd => pd.Contribution, ct); // 100 queries!
}
```

**Solution**: Precompute all data before loop

```csharp
// ✅ GOOD - 2 queries
var employees = await ctx.Demographics.Take(100).ToListAsync(ct);
var balanceLookup = await ctx.ProfitDetails
    .Where(pd => employees.Select(e => e.Ssn).Contains(pd.Ssn))
    .GroupBy(pd => pd.Ssn)
    .ToDictionaryAsync(g => g.Key, g => g.Sum(pd => pd.Contribution), ct);

foreach (var emp in employees)
{
    var balance = balanceLookup.GetValueOrDefault(emp.Ssn, 0);
}
```

### Pitfall 3: Forgetting to Use Read-Only Context

**Problem**: Using writable context for read-only queries

```csharp
// ❌ Inefficient - tracks entities unnecessarily
public async Task<List<EmployeeDto>> GetEmployeesAsync(CancellationToken ct)
{
    return await _dataContextFactory.UseWritableContext(async ctx =>
    {
        return await ctx.Demographics
            .Select(d => new EmployeeDto { /* ... */ })
            .ToListAsync(ct);
    }, ct); // Unnecessary transaction overhead
}
```

**Solution**: Use read-only context with `UseReadOnlyContext()`

```csharp
// ✅ GOOD - No tracking, optimized
public async Task<List<EmployeeDto>> GetEmployeesAsync(CancellationToken ct)
{
    return await _dataContextFactory.UseReadOnlyContext(async ctx =>
    {
        ctx.UseReadOnlyContext(); // Auto AsNoTracking

        return await ctx.Demographics
            .TagWith("GetEmployees")
            .Select(d => new EmployeeDto { /* ... */ })
            .ToListAsync(ct);
    }, ct);
}
```

### Pitfall 4: Not Handling Cache Failures

**Problem**: Cache exceptions break application flow

```csharp
// ❌ BAD - Cache failure breaks app
public async Task<List<StateTaxDto>> GetStateTaxesAsync(CancellationToken ct)
{
    var cached = await _cache.GetAsync("StateTaxes", ct); // Throws on Redis failure
    if (cached != null)
    {
        return JsonSerializer.Deserialize<List<StateTaxDto>>(cached)!;
    }

    return await LoadFromDatabaseAsync(ct);
}
```

**Solution**: Degrade gracefully on cache failures

```csharp
// ✅ GOOD - Cache enhances performance but doesn't break app
public async Task<List<StateTaxDto>> GetStateTaxesAsync(CancellationToken ct)
{
    try
    {
        var cached = await _cache.GetAsync("StateTaxes", ct);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<List<StateTaxDto>>(cached)!;
        }
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Cache read failed, falling back to database");
    }

    // Load from DB (always works)
    var data = await LoadFromDatabaseAsync(ct);

    // Try to cache (non-fatal if fails)
    try
    {
        await _cache.SetAsync("StateTaxes", JsonSerializer.SerializeToUtf8Bytes(data), ct);
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Cache write failed (non-fatal)");
    }

    return data;
}
```

### Pitfall 5: Loading Full Entities for Bulk Updates

**Problem**: Loading thousands of entities for simple updates

```csharp
// ❌ SLOW - Loads 10,000 entities
var records = await ctx.PayProfits
    .Where(pp => pp.ProfitYear == 2024)
    .ToListAsync(ct); // Loads all columns, tracks entities

foreach (var record in records)
{
    record.StatusId = 1;
}
await ctx.SaveChangesAsync(ct); // Sends 10,000 UPDATE statements
```

**Solution**: Use ExecuteUpdateAsync for single SQL statement

```csharp
// ✅ FAST - Single UPDATE statement
var rowsUpdated = await ctx.PayProfits
    .Where(pp => pp.ProfitYear == 2024)
    .ExecuteUpdateAsync(
        setters => setters.SetProperty(pp => pp.StatusId, 1),
        ct);
```

### Pitfall 6: Not Tagging Complex Queries

**Problem**: Unidentifiable queries in production Oracle traces

```csharp
// ❌ No context in Oracle V$SQL
var report = await ctx.ProfitDetails
    .Where(pd => pd.ProfitYear == year)
    .GroupBy(pd => pd.Ssn)
    .ToListAsync(ct);
```

**Solution**: Always tag queries with business context

```csharp
// ✅ GOOD - Visible in Oracle traces
var report = await ctx.ProfitDetails
    .TagWith($"YearEndReport-{year}-User:{_appUser.UserName}")
    .Where(pd => pd.ProfitYear == year)
    .GroupBy(pd => pd.Ssn)
    .ToListAsync(ct);
```

---

## Creating New Services

### Step 1: Define Interface in Common.Interfaces

```csharp
// Location: Demoulas.ProfitSharing.Common/Interfaces/IMilitaryService.cs
namespace Demoulas.ProfitSharing.Common.Interfaces;

public interface IMilitaryService
{
    /// <summary>
    /// Retrieves military service adjustments for a profit year.
    /// </summary>
    Task<List<MilitaryAdjustmentDto>> GetMilitaryAdjustmentsAsync(
        short profitYear,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a military service adjustment for an employee.
    /// </summary>
    Task<Result<MilitaryAdjustmentDto>> CreateAdjustmentAsync(
        CreateMilitaryAdjustmentRequest request,
        CancellationToken cancellationToken);
}
```

### Step 2: Implement Service

```csharp
// Location: Demoulas.ProfitSharing.Services/Military/MilitaryService.cs
namespace Demoulas.ProfitSharing.Services.Military;

public sealed class MilitaryService : IMilitaryService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<MilitaryService> _logger;

    public MilitaryService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILogger<MilitaryService> logger)
    {
        _dataContextFactory = dataContextFactory;
        _logger = logger;
    }

    public async Task<List<MilitaryAdjustmentDto>> GetMilitaryAdjustmentsAsync(
        short profitYear,
        CancellationToken ct)
    {
        _logger.LogInformation("Retrieving military adjustments for year {ProfitYear}", profitYear);

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ctx.UseReadOnlyContext();

            return await ctx.MilitaryAdjustments
                .TagWith($"GetMilitaryAdjustments-{profitYear}")
                .Where(ma => ma.ProfitYear == profitYear)
                .Select(ma => new MilitaryAdjustmentDto
                {
                    Id = ma.Id,
                    BadgeNumber = ma.BadgeNumber,
                    AdjustmentAmount = ma.Amount,
                    AdjustmentDate = ma.AdjustmentDate
                })
                .ToListAsync(ct);
        }, ct);
    }

    public async Task<Result<MilitaryAdjustmentDto>> CreateAdjustmentAsync(
        CreateMilitaryAdjustmentRequest request,
        CancellationToken ct)
    {
        // Input validation
        if (request.Amount <= 0)
        {
            return Result<MilitaryAdjustmentDto>.Failure(
                Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(request.Amount)] = new[] { "Amount must be greater than zero." }
                }));
        }

        _logger.LogInformation(
            "Creating military adjustment for badge {BadgeNumber}, amount {Amount}",
            request.BadgeNumber,
            request.Amount);

        try
        {
            var result = await _dataContextFactory.UseWritableContextAsync(
                async (ctx, transaction) =>
            {
                // Verify employee exists
                var employee = await ctx.Demographics
                    .FirstOrDefaultAsync(d => d.BadgeNumber == request.BadgeNumber, ct);
                if (employee == null)
                {
                    return Result<MilitaryAdjustmentDto>.Failure(
                        Error.EntityNotFound($"Employee with badge {request.BadgeNumber} not found"));
                }

                // Create adjustment
                var adjustment = new MilitaryAdjustment
                {
                    BadgeNumber = request.BadgeNumber,
                    ProfitYear = request.ProfitYear,
                    Amount = request.Amount,
                    AdjustmentDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    CreatedBy = "System" // Or get from IAppUser
                };

                ctx.MilitaryAdjustments.Add(adjustment);
                await ctx.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                var dto = new MilitaryAdjustmentDto
                {
                    Id = adjustment.Id,
                    BadgeNumber = adjustment.BadgeNumber,
                    AdjustmentAmount = adjustment.Amount,
                    AdjustmentDate = adjustment.AdjustmentDate
                };

                return Result<MilitaryAdjustmentDto>.Success(dto);
            }, ct);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Created military adjustment {AdjustmentId} for badge {BadgeNumber}",
                    result.Value!.Id,
                    request.BadgeNumber);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating military adjustment for badge {BadgeNumber}",
                request.BadgeNumber);

            return Result<MilitaryAdjustmentDto>.Failure(
                Error.Unexpected($"Failed to create adjustment: {ex.Message}"));
        }
    }
}
```

### Step 3: Register Service in DI

```csharp
// Location: Demoulas.ProfitSharing.Services/Extensions/ServicesExtension.cs
public static class ServicesExtension
{
    public static IHostApplicationBuilder AddProjectServices(this IHostApplicationBuilder builder)
    {
        // ... existing registrations

        // Add your new service
        builder.Services.AddScoped<IMilitaryService, MilitaryService>();

        return builder;
    }
}
```

### Step 4: Create Unit Tests

```csharp
// Location: Demoulas.ProfitSharing.Tests/Services/Military/MilitaryServiceTests.cs
public class MilitaryServiceTests
{
    private readonly IProfitSharingDataContextFactory _contextFactory;
    private readonly ILogger<MilitaryService> _logger;
    private readonly MilitaryService _sut;

    public MilitaryServiceTests()
    {
        _contextFactory = new InMemoryDataContextFactory();
        _logger = new NullLogger<MilitaryService>();
        _sut = new MilitaryService(_contextFactory, _logger);
    }

    [Description("PS-1500 : Get military adjustments returns all for year")]
    [Fact]
    public async Task GetMilitaryAdjustments_ForYear_ReturnsAllAdjustments()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act
        var result = await _sut.GetMilitaryAdjustmentsAsync(2024, CancellationToken.None);

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(2);
    }

    [Description("PS-1500 : Create adjustment with valid data succeeds")]
    [Fact]
    public async Task CreateAdjustment_WithValidData_ShouldSucceed()
    {
        // Arrange
        await SeedEmployeeAsync();
        var request = new CreateMilitaryAdjustmentRequest
        {
            BadgeNumber = 12345,
            ProfitYear = 2024,
            Amount = 500.00m
        };

        // Act
        var result = await _sut.CreateAdjustmentAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.AdjustmentAmount.ShouldBe(500.00m);
    }

    [Description("PS-1500 : Create adjustment with zero amount fails")]
    [Fact]
    public async Task CreateAdjustment_WithZeroAmount_ShouldFail()
    {
        // Arrange
        var request = new CreateMilitaryAdjustmentRequest
        {
            BadgeNumber = 12345,
            ProfitYear = 2024,
            Amount = 0 // Invalid
        };

        // Act
        var result = await _sut.CreateAdjustmentAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeOfType<ValidationError>();
    }
}
```

### Checklist for New Services

- [ ] Interface defined in `Common.Interfaces`
- [ ] Service implementation in appropriate subfolder
- [ ] Registered in `ServicesExtension.AddProjectServices()`
- [ ] Uses `IProfitSharingDataContextFactory` (not direct DbContext)
- [ ] Uses `UseReadOnlyContext()` for queries
- [ ] Uses `UseWritableContextAsync()` for mutations
- [ ] Implements structured logging with ILogger
- [ ] Returns `Result<T>` for operations that can fail
- [ ] Tags complex queries with `TagWith()`
- [ ] Handles cache failures gracefully (if caching)
- [ ] Unit tests with `[Description]` attributes
- [ ] Tests cover happy path and error cases
- [ ] Tests use Shouldly assertions
- [ ] Documentation added to this file (if significant pattern)
