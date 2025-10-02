# Enhanced Telemetry Implementation Guide

This guide shows how to use the new telemetry metrics to track database vs. business logic timing and cache effectiveness.

## Overview

Based on the log analysis, we identified these key performance improvement areas:
1. **Slow database queries** (1,200-1,524ms execution times)
2. **Repeated subqueries** (same vesting logic calculated 5+ times)
3. **N+1 query patterns** (46-51ms per navigation lookup)
4. **Missing result caching** for lookup tables

The enhanced telemetry helps track:
- `DatabaseQueryDurationMs` - Time spent in database queries (separate from business logic)
- `BusinessLogicDurationMs` - Time spent in business logic (calculations, mappings)
- `CacheHitsTotal` / `CacheMissesTotal` - Cache effectiveness metrics
- `DatabaseOperationsTotal` - Count of database operations by type (read/write)

## New Metrics Added

### 1. Database Query Duration
```csharp
EndpointTelemetry.DatabaseQueryDurationMs.Record(durationMs,
    new("endpoint", "UnforfeituresEndpoint"),
    new("query_name", "GetUnforfeitures"),
    new("operation_type", "read"));
```

### 2. Business Logic Duration
```csharp
EndpointTelemetry.BusinessLogicDurationMs.Record(durationMs,
    new("endpoint", "UnforfeituresEndpoint"),
    new("operation_name", "CalculateVesting"));
```

### 3. Cache Metrics
```csharp
// Cache hit
EndpointTelemetry.CacheHitsTotal.Add(1,
    new("cache_type", "NavigationStatus"),
    new("endpoint", "GetFrozenDemographicsEndpoint"));

// Cache miss
EndpointTelemetry.CacheMissesTotal.Add(1,
    new("cache_type", "NavigationStatus"),
    new("endpoint", "GetFrozenDemographicsEndpoint"));
```

### 4. Database Operations Counter
```csharp
EndpointTelemetry.DatabaseOperationsTotal.Add(1,
    new("endpoint", "UpdateDemographicsEndpoint"),
    new("query_name", "UpdateEmployeeRecord"),
    new("operation_type", "write"));
```

## Usage Pattern 1: Manual Timing with Stopwatch

For existing endpoints, add explicit timing around database calls and business logic:

```csharp
public override async Task<Results<Ok<UnforfeituresResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(
    UnforfeituresRequest req, CancellationToken ct)
{
    return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
    {
        List<UnforfeitureEntity> data;

        // Time database query separately
        using (DatabaseTelemetryScope.StartQuery("GetUnforfeitures", nameof(UnforfeituresEndpoint), "read"))
        {
            data = await _unforfeitureService.GetUnforfeituresAsync(req, ct);
        }

        UnforfeituresResponseDto response;

        // Time business logic (mapping, calculations) separately
        using (BusinessLogicTelemetryScope.Start("MapToDto", nameof(UnforfeituresEndpoint)))
        {
            response = new UnforfeituresResponseDto
            {
                Records = data.Select(e => new UnforfeitureDto
                {
                    Ssn = e.Ssn,
                    BadgeNumber = e.BadgeNumber,
                    Amount = e.Amount,
                    // ... other mappings
                }).ToList()
            };

            // Record result count
            EndpointTelemetry.RecordCountsProcessed.Record(response.Records.Count,
                new("record_type", "unforfeiture"),
                new("endpoint", nameof(UnforfeituresEndpoint)));
        }

        // Business operations metric
        EndpointTelemetry.BusinessOperationsTotal.Add(1,
            new("operation", "unforfeitures-query"),
            new("endpoint", nameof(UnforfeituresEndpoint)),
            new("profit_year", req.ProfitYear.ToString()));

        return Result<UnforfeituresResponseDto>.Success(response);
    });
}
```

## Usage Pattern 2: Using-Scoped Telemetry Helpers

The `DatabaseTelemetryScope` and `BusinessLogicTelemetryScope` classes provide automatic timing via `IDisposable`:

```csharp
// Database query timing (automatic on dispose)
using (var dbScope = DatabaseTelemetryScope.StartQuery(
    queryName: "GetFrozenDemographics",
    endpointName: nameof(GetFrozenDemographicsEndpoint),
    operationType: "read"))
{
    demographics = await _dbContext.FrozenDemographics
        .Where(d => d.ProfitYear == req.ProfitYear)
        .ToListAsync(ct);
}

// Business logic timing (automatic on dispose)
using (var bizScope = BusinessLogicTelemetryScope.Start(
    operationName: "CalculateVestingPercent",
    endpointName: nameof(GetFrozenDemographicsEndpoint)))
{
    foreach (var demo in demographics)
    {
        demo.VestingPercent = CalculateVestingPercentage(demo);
    }
}
```

## Usage Pattern 3: Cache Telemetry with CacheTelemetryWrapper

For cached lookup tables (e.g., Navigation Status, Calendar Years):

```csharp
public class NavigationService
{
    private readonly CacheTelemetryWrapper _cache;
    private readonly ProfitSharingReadOnlyDbContext _dbContext;

    public NavigationService(IMemoryCache memoryCache, ProfitSharingReadOnlyDbContext dbContext)
    {
        _cache = new CacheTelemetryWrapper(memoryCache);
        _dbContext = dbContext;
    }

    public async Task<NavigationStatus?> GetNavigationStatusAsync(int navigationId, CancellationToken ct)
    {
        var cacheKey = $"nav_status_{navigationId}";

        // GetOrCreateAsync automatically records cache hit/miss
        return await _cache.GetOrCreateAsync(
            key: cacheKey,
            factory: async () =>
            {
                // This only runs on cache miss
                using (DatabaseTelemetryScope.StartQuery("GetNavigationStatus", "NavigationService", "read"))
                {
                    return await _dbContext.NavigationStatuses
                        .FirstOrDefaultAsync(n => n.NavigationId == navigationId, ct);
                }
            },
            cacheType: "NavigationStatus",
            expirationMinutes: 60,
            endpointName: "NavigationService");
    }
}
```

## Usage Pattern 4: Manual Cache Hit/Miss Recording

If you have existing caching logic and can't use `CacheTelemetryWrapper`:

```csharp
private async Task<CalendarYear?> GetCalendarYearAsync(int year, CancellationToken ct)
{
    var cacheKey = $"calendar_year_{year}";

    if (_memoryCache.TryGetValue<CalendarYear>(cacheKey, out var cached))
    {
        // Manually record cache hit
        CacheTelemetryExtensions.RecordCacheHit("CalendarYear", nameof(MyEndpoint));
        return cached;
    }

    // Manually record cache miss
    CacheTelemetryExtensions.RecordCacheMiss("CalendarYear", nameof(MyEndpoint));

    // Fetch from database
    using (DatabaseTelemetryScope.StartQuery("GetCalendarYear", nameof(MyEndpoint), "read"))
    {
        var entity = await _dbContext.CalendarYears.FindAsync(new object[] { year }, ct);
        if (entity != null)
        {
            _memoryCache.Set(cacheKey, entity, TimeSpan.FromHours(24));
        }
        return entity;
    }
}
```

## Complete Example: Optimized Endpoint with Full Telemetry

Here's a complete refactored endpoint showing all telemetry patterns:

```csharp
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Endpoints.Extensions;
using Microsoft.Extensions.Caching.Memory;

public class OptimizedUnforfeituresEndpoint : ProfitSharingEndpoint<UnforfeituresRequest, Results<Ok<UnforfeituresResponseDto>, NotFound, ProblemHttpResult>>
{
    private readonly IUnforfeitureService _service;
    private readonly ILogger<OptimizedUnforfeituresEndpoint> _logger;
    private readonly CacheTelemetryWrapper _cache;

    public OptimizedUnforfeituresEndpoint(
        IUnforfeitureService service,
        ILogger<OptimizedUnforfeituresEndpoint> logger,
        IMemoryCache memoryCache)
        : base(Navigation.Constants.SOME_NAV_ID)
    {
        _service = service;
        _logger = logger;
        _cache = new CacheTelemetryWrapper(memoryCache);
    }

    public override void Configure()
    {
        Get("/unforfeitures");
        // ... configuration
    }

    public override async Task<Results<Ok<UnforfeituresResponseDto>, NotFound, ProblemHttpResult>> ExecuteAsync(
        UnforfeituresRequest req, CancellationToken ct)
    {
        return await this.ExecuteWithTelemetry(HttpContext, _logger, req, async () =>
        {
            // Step 1: Check cache for navigation status lookup (reducing N+1 queries)
            var navigationStatuses = await _cache.GetOrCreateAsync(
                key: "all_navigation_statuses",
                factory: async () =>
                {
                    using (DatabaseTelemetryScope.StartQuery("GetAllNavigationStatuses", nameof(OptimizedUnforfeituresEndpoint), "read"))
                    {
                        return await _service.GetAllNavigationStatusesAsync(ct);
                    }
                },
                cacheType: "NavigationStatus",
                expirationMinutes: 60,
                endpointName: nameof(OptimizedUnforfeituresEndpoint));

            // Step 2: Execute main query with timing
            List<UnforfeitureEntity> data;
            using (DatabaseTelemetryScope.StartQuery("GetUnforfeitures", nameof(OptimizedUnforfeituresEndpoint), "read"))
            {
                data = await _service.GetUnforfeituresAsync(req, ct);
            }

            // Step 3: Business logic (mapping + calculations) with timing
            UnforfeituresResponseDto response;
            using (BusinessLogicTelemetryScope.Start("MapAndCalculate", nameof(OptimizedUnforfeituresEndpoint)))
            {
                response = new UnforfeituresResponseDto
                {
                    Records = data.Select(e => new UnforfeitureDto
                    {
                        Ssn = e.Ssn,
                        BadgeNumber = e.BadgeNumber,
                        Amount = e.Amount,
                        NavigationStatus = navigationStatuses.FirstOrDefault(n => n.NavigationId == e.NavigationId)?.Status ?? "Unknown"
                    }).ToList()
                };

                // Record result count
                EndpointTelemetry.RecordCountsProcessed.Record(response.Records.Count,
                    new("record_type", "unforfeiture"),
                    new("endpoint", nameof(OptimizedUnforfeituresEndpoint)));
            }

            // Step 4: Business operations metric
            EndpointTelemetry.BusinessOperationsTotal.Add(1,
                new("operation", "unforfeitures-query"),
                new("endpoint", nameof(OptimizedUnforfeituresEndpoint)),
                new("profit_year", req.ProfitYear.ToString()));

            _logger.LogInformation(
                "Retrieved {Count} unforfeitures for year {Year} (correlation: {CorrelationId})",
                response.Records.Count, req.ProfitYear, HttpContext.TraceIdentifier);

            return Result<UnforfeituresResponseDto>.Success(response);
        }, "Ssn"); // Declare sensitive field access
    }
}
```

## Querying Metrics in Production

Once deployed, these metrics can be queried in Prometheus/Grafana:

### Database Query Performance by Endpoint
```promql
# Average database query duration by endpoint
rate(ps_database_query_duration_ms_sum[5m]) / rate(ps_database_query_duration_ms_count[5m])

# P95 database query duration
histogram_quantile(0.95, rate(ps_database_query_duration_ms_bucket[5m]))
```

### Business Logic Performance
```promql
# Average business logic duration by endpoint
rate(ps_business_logic_duration_ms_sum[5m]) / rate(ps_business_logic_duration_ms_count[5m])
```

### Cache Effectiveness
```promql
# Cache hit ratio by cache type
sum(rate(ps_cache_hits_total[5m])) by (cache_type) 
/ 
(sum(rate(ps_cache_hits_total[5m])) by (cache_type) + sum(rate(ps_cache_misses_total[5m])) by (cache_type))

# Cache miss rate spike alert
rate(ps_cache_misses_total[5m]) > 100
```

### Database Operations by Type
```promql
# Read vs write operations
sum(rate(ps_database_operations_total[5m])) by (operation_type)

# Slow queries alert (> 1 second)
ps_database_query_duration_ms > 1000
```

## Expected Performance Improvements

Based on the log analysis, implementing these patterns should yield:

1. **Caching Navigation Status lookups**: Reduce 46-51ms per N+1 query → ~5-10x improvement
2. **Database view for vesting calculations**: Eliminate repeated subqueries → ~30-40% query time reduction
3. **Composite indexes**: Improve `WHERE` clause performance → ~20-30% query time reduction
4. **Separate telemetry tracking**: Identify exact bottlenecks (DB vs logic) for targeted optimization

## Migration Checklist

To add enhanced telemetry to an existing endpoint:

- [ ] Add `using Demoulas.ProfitSharing.Common.Telemetry;`
- [ ] Wrap database queries with `DatabaseTelemetryScope.StartQuery(...)`
- [ ] Wrap business logic with `BusinessLogicTelemetryScope.Start(...)`
- [ ] Replace direct `IMemoryCache` with `CacheTelemetryWrapper` (or use manual recording)
- [ ] Add `EndpointTelemetry.RecordCountsProcessed` for result sizes
- [ ] Keep existing `EndpointTelemetry.BusinessOperationsTotal` metrics
- [ ] Test locally and verify metrics appear in logs/console

## References

- **Log Analysis**: `docs/ProfitSharing-Api-rurwcgee-20251002062102.txt` (lines with 1,200-1,524ms queries)
- **Telemetry Guide**: `TELEMETRY_GUIDE.md` (comprehensive reference)
- **Quick Reference**: `TELEMETRY_QUICK_REFERENCE.md` (cheat sheet)
