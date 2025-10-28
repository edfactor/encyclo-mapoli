# DemographicsService Refactoring Plan

## Executive Summary

This document outlines a comprehensive refactoring plan for `DemographicsService` and related classes to improve testability, maintainability, and adherence to SOLID principles.

## Current Problems

### 1. **Testability Issues**

- **Direct DbContext Access**: Service directly creates and manages DbContext instances via factory
- **Complex Method Logic**: `AddDemographicsStreamAsync` contains 300+ lines with multiple responsibilities
- **Hard to Mock**: Database operations, HTTP calls, and business logic are tightly coupled
- **No Isolation**: Cannot test individual pieces of logic without full database setup

### 2. **Architectural Issues**

- **Violation of Single Responsibility Principle**: One service handles matching, auditing, history, validation, and data access
- **Poor Separation of Concerns**: Business logic mixed with data access and infrastructure concerns
- **Difficult Debugging**: Complex nested logic makes troubleshooting difficult
- **Limited Reusability**: Business logic is embedded and cannot be reused

### 3. **Maintainability Issues**

- **High Cognitive Load**: Developers must understand entire flow to make changes
- **Fragile Changes**: Modifications in one area can break unrelated functionality
- **Difficult Code Review**: Large methods are hard to review effectively
- **Poor Documentation**: Complex logic not well-documented

## Proposed Architecture

### Overview

```
┌──────────────────────────────────────────────────────────────┐
│                    Presentation Layer                         │
│                   (FastEndpoints)                            │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│                Application Services Layer                     │
│  - DemographicsService (Orchestration)                       │
│  - Telemetry, Logging, Transaction Management                │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┐
│                   Domain Services Layer                       │
│  - DemographicMatchingService (Match Logic)                  │
│  - DemographicAuditService (Audit Logic)                     │
│  - DemographicHistoryService (History Tracking)              │
│  - DemographicValidationService (Business Rules)             │
└───────────────────────────┬──────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────┘
│                  Data Access Layer                            │
│  - IDemographicsRepository (Data Operations)                 │
│  - DemographicsRepository (EF Core Implementation)           │
└───────────────────────────────────────────────────────────────┘
```

## Refactoring Steps

### Phase 1: Extract Data Access Layer (Repository Pattern)

#### 1.1 Create Repository Interface

```csharp
public interface IDemographicsRepository
{
    // Query operations
    Task<List<Demographic>> GetByOracleIdsAsync(
        IEnumerable<long> oracleIds,
        CancellationToken ct);

    Task<List<Demographic>> GetBySsnAndBadgePairsAsync(
        IEnumerable<(int Ssn, int BadgeNumber)> pairs,
        CancellationToken ct);

    Task<Demographic?> GetBySsnAsync(int ssn, CancellationToken ct);

    Task<List<Demographic>> GetDuplicateSsnsAsync(
        IEnumerable<int> ssns,
        CancellationToken ct);

    // Command operations
    Task<int> InsertAsync(Demographic demographic, CancellationToken ct);
    Task<int> InsertBatchAsync(
        IEnumerable<Demographic> demographics,
        CancellationToken ct);

    Task UpdateAsync(Demographic demographic, CancellationToken ct);
    Task UpdateBatchAsync(
        IEnumerable<Demographic> demographics,
        CancellationToken ct);

    // Related entities
    Task UpdateRelatedSsnAsync(
        int oldSsn,
        int newSsn,
        CancellationToken ct);
}
```

#### 1.2 Implement Repository

```csharp
public class DemographicsRepository : IDemographicsRepository
{
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public DemographicsRepository(IProfitSharingDataContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Demographic>> GetByOracleIdsAsync(
        IEnumerable<long> oracleIds,
        CancellationToken ct)
    {
        return await _contextFactory.UseReadOnlyContext(async ctx =>
        {
            var idList = oracleIds.ToList();
            return await ctx.Demographics
                .TagWith($"GetByOracleIds-Count:{idList.Count}")
                .Where(d => idList.Contains(d.OracleHcmId))
                .Include(d => d.ContactInfo)
                .Include(d => d.Address)
                .ToListAsync(ct);
        }, ct);
    }

    // ... other implementations
}
```

### Phase 2: Extract Domain Services

#### 2.1 Create Demographic Matching Service

```csharp
public interface IDemographicMatchingService
{
    Task<DemographicMatchResult> MatchDemographicsAsync(
        IEnumerable<Demographic> incoming,
        CancellationToken ct);
}

public class DemographicMatchResult
{
    public List<Demographic> ExistingMatches { get; set; } = new();
    public List<Demographic> NewDemographics { get; set; } = new();
    public Dictionary<long, Demographic> PrimaryMatches { get; set; } = new();
    public Dictionary<(int, int), Demographic> FallbackMatches { get; set; } = new();
    public MatchStatistics Statistics { get; set; } = new();
}

public class MatchStatistics
{
    public int Requested { get; set; }
    public int PrimaryMatched { get; set; }
    public int FallbackPairs { get; set; }
    public int FallbackMatched { get; set; }
    public bool SkippedAllZeroBadgeFallback { get; set; }
}

public class DemographicMatchingService : IDemographicMatchingService
{
    private readonly IDemographicsRepository _repository;
    private readonly ILogger<DemographicMatchingService> _logger;

    public async Task<DemographicMatchResult> MatchDemographicsAsync(
        IEnumerable<Demographic> incoming,
        CancellationToken ct)
    {
        var result = new DemographicMatchResult();
        var incomingList = incoming.ToList();

        result.Statistics.Requested = incomingList.Count;

        // Primary matching by OracleHcmId
        var oracleIds = incomingList.Select(d => d.OracleHcmId).ToList();
        var primaryMatches = await _repository.GetByOracleIdsAsync(oracleIds, ct);
        result.Statistics.PrimaryMatched = primaryMatches.Count;

        var matchedOracleIds = primaryMatches.Select(d => d.OracleHcmId).ToHashSet();

        // Fallback matching by (SSN, BadgeNumber)
        var fallbackPairs = incomingList
            .Where(d => !matchedOracleIds.Contains(d.OracleHcmId))
            .Select(d => (d.Ssn, d.BadgeNumber))
            .Distinct()
            .ToList();

        result.Statistics.FallbackPairs = fallbackPairs.Count;

        if (ShouldSkipFallback(fallbackPairs))
        {
            result.Statistics.SkippedAllZeroBadgeFallback = true;
            _logger.LogCritical(
                "All fallback pairs have BadgeNumber == 0. Skipping fallback.");
            return result;
        }

        var fallbackMatches = await _repository
            .GetBySsnAndBadgePairsAsync(fallbackPairs, ct);
        result.Statistics.FallbackMatched = fallbackMatches.Count;

        // Combine results
        result.ExistingMatches = primaryMatches
            .Concat(fallbackMatches)
            .GroupBy(d => d.Id)
            .Select(g => g.First())
            .ToList();

        return result;
    }

    private static bool ShouldSkipFallback(
        List<(int Ssn, int BadgeNumber)> pairs)
    {
        return pairs.Count > 0 && pairs.All(p => p.BadgeNumber == 0);
    }
}
```

#### 2.2 Create Demographic Audit Service

```csharp
public interface IDemographicAuditService
{
    Task<List<DemographicSyncAudit>> DetectDuplicateSsnsAsync(
        IEnumerable<Demographic> demographics,
        IEnumerable<int> targetSsns,
        CancellationToken ct);

    Task<DemographicSyncAudit?> CheckSsnConflictAsync(
        Demographic existing,
        Demographic proposed,
        CancellationToken ct);

    Task AddAuditRecordsAsync(
        IEnumerable<DemographicSyncAudit> audits,
        CancellationToken ct);
}

public class DemographicAuditService : IDemographicAuditService
{
    private readonly IDemographicsRepository _repository;
    private readonly ITotalService _totalService;

    public async Task<DemographicSyncAudit?> CheckSsnConflictAsync(
        Demographic existing,
        Demographic proposed,
        CancellationToken ct)
    {
        if (existing.Ssn == proposed.Ssn)
            return null;

        // Check if proposed SSN already exists
        var matchToProposed = await _repository
            .GetBySsnAsync(proposed.Ssn, ct);

        if (matchToProposed == null)
        {
            return CreateAudit(
                existing,
                "SSN changed with no conflicts.",
                "SSN");
        }

        // Check if existing employee can be safely merged
        var balance = await _totalService.GetVestingBalanceForSingleMemberAsync(
            SearchBy.Ssn,
            existing.Ssn,
            (short)DateTime.Now.Year,
            ct);

        bool isTerminatedWithZeroBalance =
            matchToProposed.EmploymentStatusId == EmploymentStatus.Constants.Terminated
            && balance?.CurrentBalance == 0.0m;

        string message = isTerminatedWithZeroBalance
            ? "Duplicate SSN found for terminated user with zero balance."
            : "Duplicate SSN added for user.";

        return CreateAudit(existing, message, "SSN");
    }

    private static DemographicSyncAudit CreateAudit(
        Demographic demographic,
        string message,
        string propertyName)
    {
        return new DemographicSyncAudit
        {
            BadgeNumber = demographic.BadgeNumber,
            OracleHcmId = demographic.OracleHcmId,
            InvalidValue = demographic.Ssn.MaskSsn(),
            Message = message,
            UserName = Constants.SystemAccountName,
            PropertyName = propertyName
        };
    }
}
```

#### 2.3 Create Demographic History Service

```csharp
public interface IDemographicHistoryService
{
    Task<DemographicHistory?> GetCurrentHistoryAsync(
        int demographicId,
        CancellationToken ct);

    Task AddHistorySnapshotAsync(
        Demographic demographic,
        CancellationToken ct);

    Task CloseCurrentHistoryAsync(
        int demographicId,
        DateTimeOffset closedAt,
        CancellationToken ct);
}

public class DemographicHistoryService : IDemographicHistoryService
{
    private readonly IProfitSharingDataContextFactory _contextFactory;

    public async Task AddHistorySnapshotAsync(
        Demographic demographic,
        CancellationToken ct)
    {
        var history = DemographicHistory.FromDemographic(demographic);

        await _contextFactory.UseWritableContext(async ctx =>
        {
            ctx.DemographicHistories.Add(history);
            await ctx.SaveChangesAsync(ct);
        }, ct);
    }

    public async Task CloseCurrentHistoryAsync(
        int demographicId,
        DateTimeOffset closedAt,
        CancellationToken ct)
    {
        await _contextFactory.UseWritableContext(async ctx =>
        {
            var current = await ctx.DemographicHistories
                .TagWith($"CloseHistory-DemographicId:{demographicId}")
                .Where(h => h.DemographicId == demographicId
                    && closedAt >= h.ValidFrom
                    && closedAt < h.ValidTo)
                .FirstOrDefaultAsync(ct);

            if (current != null)
            {
                current.ValidTo = closedAt;
                await ctx.SaveChangesAsync(ct);
            }
        }, ct);
    }
}
```

### Phase 3: Refactor Main Service to Orchestrator

```csharp
public sealed class DemographicsService : IDemographicsServiceInternal
{
    private readonly IDemographicsRepository _repository;
    private readonly IDemographicMatchingService _matchingService;
    private readonly IDemographicAuditService _auditService;
    private readonly IDemographicHistoryService _historyService;
    private readonly DemographicMapper _mapper;
    private readonly ILogger<DemographicsService> _logger;

    public async Task AddDemographicsStreamAsync(
        DemographicsRequest[] employees,
        byte batchSize = byte.MaxValue,
        CancellationToken ct = default)
    {
        DemographicsIngestMetrics.EnsureInitialized();
        long startTicks = Environment.TickCount64;
        DateTimeOffset modification = DateTimeOffset.UtcNow;

        // Step 1: Map and prepare
        var incoming = PrepareDemographics(employees, modification);

        // Step 2: Match existing records
        var matchResult = await _matchingService
            .MatchDemographicsAsync(incoming, ct);

        // Step 3: Audit duplicate SSNs and conflicts
        var auditResults = await _auditService
            .DetectDuplicateSsnsAsync(
                matchResult.ExistingMatches,
                incoming.Select(d => d.Ssn),
                ct);

        // Step 4: Handle SSN conflicts
        await HandleSsnConflictsAsync(
            incoming,
            matchResult.ExistingMatches,
            ct);

        // Step 5: Insert new demographics
        var insertCount = await InsertNewDemographicsAsync(
            incoming,
            matchResult.ExistingMatches,
            ct);

        // Step 6: Update existing demographics
        var updateCount = await UpdateExistingDemographicsAsync(
            incoming,
            matchResult.ExistingMatches,
            modification,
            ct);

        // Step 7: Record metrics and telemetry
        RecordMetrics(matchResult.Statistics, insertCount, updateCount);
        LogSummary(matchResult.Statistics, insertCount, updateCount);
    }

    private List<Demographic> PrepareDemographics(
        DemographicsRequest[] employees,
        DateTimeOffset modification)
    {
        var demographics = _mapper.Map(employees).ToList();
        demographics.ForEach(d => d.ModifiedAtUtc = modification);
        return demographics;
    }

    // Other helper methods...
}
```

## Testing Strategy

### Unit Test Structure

#### 1. Repository Tests

```csharp
[Description("PS-XXXX: DemographicsRepository unit tests")]
public class DemographicsRepositoryTests
{
    [Fact]
    [Description("PS-XXXX: GetByOracleIdsAsync returns matching demographics")]
    public async Task GetByOracleIdsAsync_ReturnsMatchingDemographics()
    {
        // Arrange
        var (demographics, _, _, _, _) =
            TestDataBuilder.CreateDemographicsScenario();
        var factory = ScenarioDataContextFactory.Create(demographics);
        var repository = new DemographicsRepository(factory);
        var targetIds = demographics.Take(3).Select(d => d.OracleHcmId).ToList();

        // Act
        var results = await repository.GetByOracleIdsAsync(targetIds, default);

        // Assert
        results.Count.ShouldBe(3);
        results.All(r => targetIds.Contains(r.OracleHcmId)).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-XXXX: GetBySsnAndBadgePairsAsync handles all-zero badges")]
    public async Task GetBySsnAndBadgePairsAsync_AllZeroBadges_ReturnsEmpty()
    {
        // Arrange
        var factory = ScenarioDataContextFactory.Create();
        var repository = new DemographicsRepository(factory);
        var pairs = new List<(int Ssn, int BadgeNumber)>
        {
            (111111111, 0),
            (222222222, 0)
        };

        // Act
        var results = await repository
            .GetBySsnAndBadgePairsAsync(pairs, default);

        // Assert
        results.ShouldBeEmpty();
    }
}
```

#### 2. Matching Service Tests

```csharp
[Description("PS-XXXX: DemographicMatchingService unit tests")]
public class DemographicMatchingServiceTests
{
    [Fact]
    [Description("PS-XXXX: Primary matching by OracleHcmId succeeds")]
    public async Task MatchDemographicsAsync_PrimaryMatch_Success()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographics(count: 5);
        var mockRepo = new Mock<IDemographicsRepository>();
        mockRepo.Setup(r => r.GetByOracleIdsAsync(
                It.IsAny<IEnumerable<long>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new DemographicMatchingService(
            mockRepo.Object,
            Mock.Of<ILogger<DemographicMatchingService>>());

        var incoming = existing.Select(d => d with {
            ContactInfo = d.ContactInfo with {
                FirstName = "Updated"
            }
        }).ToList();

        // Act
        var result = await service.MatchDemographicsAsync(incoming, default);

        // Assert
        result.Statistics.Requested.ShouldBe(5);
        result.Statistics.PrimaryMatched.ShouldBe(5);
        result.Statistics.FallbackPairs.ShouldBe(0);
        result.ExistingMatches.Count.ShouldBe(5);
    }

    [Fact]
    [Description("PS-XXXX: Fallback matching with all-zero badges is skipped")]
    public async Task MatchDemographicsAsync_AllZeroBadges_SkipsFallback()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographics(count: 3);
        var mockRepo = new Mock<IDemographicsRepository>();
        mockRepo.Setup(r => r.GetByOracleIdsAsync(
                It.IsAny<IEnumerable<long>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Demographic>()); // No primary matches

        var service = new DemographicMatchingService(
            mockRepo.Object,
            Mock.Of<ILogger<DemographicMatchingService>>());

        var incoming = existing.Select(d => d with {
            BadgeNumber = 0,
            OracleHcmId = long.MaxValue // Force fallback
        }).ToList();

        // Act
        var result = await service.MatchDemographicsAsync(incoming, default);

        // Assert
        result.Statistics.SkippedAllZeroBadgeFallback.ShouldBeTrue();
        mockRepo.Verify(r => r.GetBySsnAndBadgePairsAsync(
            It.IsAny<IEnumerable<(int, int)>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
```

#### 3. Audit Service Tests

```csharp
[Description("PS-XXXX: DemographicAuditService unit tests")]
public class DemographicAuditServiceTests
{
    [Fact]
    [Description("PS-XXXX: CheckSsnConflictAsync detects terminated with zero balance")]
    public async Task CheckSsnConflictAsync_TerminatedZeroBalance_CreatesAudit()
    {
        // Arrange
        var existing = TestDataBuilder.CreateDemographic(ssn: 111111111);
        var proposed = existing with { Ssn = 222222222 };
        var matchToProposed = TestDataBuilder.CreateDemographic(
            ssn: 222222222,
            employmentStatus: EmploymentStatus.Constants.Terminated);

        var mockRepo = new Mock<IDemographicsRepository>();
        mockRepo.Setup(r => r.GetBySsnAsync(222222222, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matchToProposed);

        var mockTotalService = new Mock<ITotalService>();
        mockTotalService.Setup(t => t.GetVestingBalanceForSingleMemberAsync(
                It.IsAny<SearchBy>(),
                It.IsAny<int>(),
                It.IsAny<short>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BalanceEndpointResponse { CurrentBalance = 0 });

        var service = new DemographicAuditService(mockRepo.Object, mockTotalService.Object);

        // Act
        var audit = await service.CheckSsnConflictAsync(existing, proposed, default);

        // Assert
        audit.ShouldNotBeNull();
        audit.Message.ShouldContain("terminated user with zero balance");
    }
}
```

### Test Data Builders

#### Enhanced OracleEmployeeFactory

```csharp
public static class OracleEmployeeFactory
{
    public static OracleEmployeeBuilder Create()
    {
        return new OracleEmployeeBuilder();
    }

    public static OracleEmployee[] Generate(int count = 1)
    {
        var faker = new DemographicFaker();
        var list = faker.Generate(count);
        return list.Select(d => d.ToOracleFromDemographic()).ToArray();
    }
}

public class OracleEmployeeBuilder
{
    private readonly Demographic _demographic;

    public OracleEmployeeBuilder()
    {
        _demographic = new DemographicFaker().Generate();
    }

    public OracleEmployeeBuilder WithSsn(int ssn)
    {
        _demographic.Ssn = ssn;
        return this;
    }

    public OracleEmployeeBuilder WithBadgeNumber(int badge)
    {
        _demographic.BadgeNumber = badge;
        return this;
    }

    public OracleEmployeeBuilder WithOracleHcmId(long id)
    {
        _demographic.OracleHcmId = id;
        return this;
    }

    public OracleEmployeeBuilder AsTerminated()
    {
        _demographic.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
        _demographic.TerminationDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-6));
        return this;
    }

    public OracleEmployee Build()
    {
        return _demographic.ToOracleFromDemographic();
    }
}
```

#### Test Scenario Builder

```csharp
public static class TestDataBuilder
{
    public static (
        List<Demographic> demographics,
        List<DemographicHistory> histories,
        List<DemographicSyncAudit> audits,
        List<BeneficiaryContact> beneficiaries,
        List<ProfitDetail> profitDetails
    ) CreateDemographicsScenario(
        int demographicCount = 10,
        bool includeDuplicateSsns = false,
        bool includeTerminated = false)
    {
        var faker = new DemographicFaker();
        var demographics = faker.Generate(demographicCount);

        if (includeDuplicateSsns)
        {
            // Create intentional duplicate SSNs
            demographics[1].Ssn = demographics[0].Ssn;
        }

        if (includeTerminated)
        {
            demographics[0].EmploymentStatusId = EmploymentStatus.Constants.Terminated;
            demographics[0].TerminationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
        }

        var histories = demographics
            .Select(d => DemographicHistory.FromDemographic(d, d.Id))
            .ToList();

        return (demographics, histories, new(), new(), new());
    }

    public static Demographic CreateDemographic(
        int? ssn = null,
        int? badgeNumber = null,
        long? oracleHcmId = null,
        char? employmentStatus = null)
    {
        var demographic = new DemographicFaker().Generate();

        if (ssn.HasValue) demographic.Ssn = ssn.Value;
        if (badgeNumber.HasValue) demographic.BadgeNumber = badgeNumber.Value;
        if (oracleHcmId.HasValue) demographic.OracleHcmId = oracleHcmId.Value;
        if (employmentStatus.HasValue)
            demographic.EmploymentStatusId = employmentStatus.Value;

        return demographic;
    }
}
```

## Implementation Checklist

### Phase 1: Foundation

- [ ] Create `IDemographicsRepository` interface
- [ ] Implement `DemographicsRepository` with EF Core
- [ ] Create unit tests for repository
- [ ] Update DI registration

### Phase 2: Domain Services

- [ ] Create `IDemographicMatchingService` and implementation
- [ ] Create `IDemographicAuditService` and implementation
- [ ] Create `IDemographicHistoryService` and implementation
- [ ] Create unit tests for each service
- [ ] Update DI registration

### Phase 3: Test Infrastructure

- [ ] Create `OracleEmployeeBuilder` fluent builder
- [ ] Create `TestDataBuilder` scenario builder
- [ ] Create `TestScenarios` class with common scenarios
- [ ] Document test patterns in markdown

### Phase 4: Refactor Main Service

- [ ] Refactor `DemographicsService` to use new services
- [ ] Simplify `AddDemographicsStreamAsync` method
- [ ] Add comprehensive unit tests
- [ ] Update integration tests

### Phase 5: Documentation

- [ ] Create architecture diagram
- [ ] Document testing patterns
- [ ] Create example test scenarios
- [ ] Update developer onboarding docs

## Benefits

### Improved Testability

- **Isolated Testing**: Each service can be tested independently
- **Easy Mocking**: Interfaces make mocking simple
- **Fast Tests**: Unit tests don't require database
- **Comprehensive Coverage**: Can test edge cases easily

### Better Maintainability

- **Single Responsibility**: Each class has one clear purpose
- **Easy to Understand**: Smaller, focused methods
- **Safe Changes**: Modifications isolated to specific areas
- **Better Code Reviews**: Smaller, focused pull requests

### Enhanced Debugging

- **Clear Flow**: Orchestration pattern makes flow obvious
- **Focused Logging**: Each service logs its operations
- **Easy Breakpoints**: Smaller methods easier to debug
- **Clear Stack Traces**: Better error messages

### Improved Reusability

- **Composable Services**: Mix and match services
- **Shared Logic**: Business rules in one place
- **Easy Extensions**: Add new features without modifying existing code

## Migration Strategy

### Backward Compatibility

- Keep existing `DemographicsService` interface
- Add new services alongside existing code
- Gradually migrate callers to new architecture
- Remove old code once migration complete

### Rollout Plan

1. **Week 1-2**: Create repository and domain services
2. **Week 3-4**: Write comprehensive unit tests
3. **Week 5-6**: Refactor main service
4. **Week 7**: Update integration tests
5. **Week 8**: Documentation and code review

### Risk Mitigation

- **Feature Flags**: Control rollout with feature toggles
- **Parallel Running**: Run old and new code side-by-side
- **Monitoring**: Enhanced telemetry to detect issues
- **Rollback Plan**: Keep old code until verified

## Success Metrics

### Code Quality

- **Test Coverage**: Achieve >90% code coverage
- **Cyclomatic Complexity**: Reduce average from 20+ to <10
- **Method Length**: No methods >50 lines
- **Code Duplication**: Eliminate duplicate logic

### Performance

- **Execution Time**: Maintain or improve performance
- **Memory Usage**: No significant increase
- **Database Calls**: Optimize query patterns

### Developer Experience

- **Onboarding Time**: Reduce from 2 weeks to 3 days
- **Bug Fix Time**: Reduce average time by 50%
- **Code Review Time**: Reduce from 2 hours to 30 minutes

## Conclusion

This refactoring plan provides a clear path to transform `DemographicsService` from a monolithic, hard-to-test class into a well-structured, maintainable, and testable architecture. The repository pattern isolates data access, domain services encapsulate business logic, and the test infrastructure makes it easy to write comprehensive tests.

The benefits include:

- Dramatically improved testability
- Better maintainability and code quality
- Easier debugging and troubleshooting
- Enhanced reusability of business logic
- Better developer experience

This refactoring aligns with SOLID principles and industry best practices, setting a foundation for future enhancements and making the codebase more resilient to change.
