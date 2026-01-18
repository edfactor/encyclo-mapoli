using Bogus;
using Demoulas.Common.Contracts.Contracts.Request.Audit;
using Demoulas.Common.Data.Services.Entities.Entities.Audit;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.Audit;

/// <summary>
/// Unit tests for ProfitSharingProfitSharingAuditService.SearchAuditEventsAsync method.
/// 
/// Tests all filter scenarios:
/// - No filters (returns all events)
/// - Table name filtering (LIKE/Contains)
/// - Operation filtering (LIKE/Contains)
/// - Username filtering (LIKE/Contains)
/// - Date range filtering (StartTime/EndTime)
/// - Pagination (Skip/Take)
/// - ChangesJson conditional loading (NAVIGATION vs other tables)
/// - Multiple filters combined
/// - No matches scenario
/// </summary>
[Collection("SharedGlobalState")]
public sealed class AuditServiceSearchTests : ApiTestBase<Program>
{
    private readonly IProfitSharingAuditService _service;
    private readonly List<AuditEvent> _auditEvents;

    public AuditServiceSearchTests()
    {
        _auditEvents = CreateMockAuditEvents();
        MockDbContextFactory = new ScenarioFactory { AuditEvents = _auditEvents }.BuildMocks();
        _service = ServiceProvider?.GetRequiredService<IProfitSharingAuditService>()!;
    }

    [Fact]
    public async Task SearchAuditEvents_NoFilters_ReturnsAllEvents()
    {
        // Arrange
        var request = new AuditSearchRequestRequest()
        {
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Total.ShouldBe(_auditEvents.Count);
        result.Results.Count().ShouldBe(_auditEvents.Count);
    }

    [Fact]
    public async Task SearchAuditEvents_WithTableNameFilter_ReturnsMatchingEvents()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            TableName = "NAVIGATION",
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e => e.TableName != null && e.TableName.Contains("NAVIGATION"));
        result.Results.Count().ShouldBe(5); // 5 NAVIGATION events
        result.Total.ShouldBe(5);
    }

    [Fact]
    public async Task SearchAuditEvents_WithOperationFilter_ReturnsMatchingEvents()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            Operation = "Update",  // Will match Update operations
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e => e.Operation.Contains("Update"));
        result.Total.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task SearchAuditEvents_WithUsernameFilter_ReturnsMatchingEvents()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            UserName = "admin",  // Will match "admin.user" and "hr.admin"
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e => e.UserName.Contains("admin"));
        result.Total.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task SearchAuditEvents_WithDateRangeFilter_ReturnsEventsInRange()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow.AddDays(-7);
        var endDate = DateTimeOffset.UtcNow.AddDays(-1);

        var request = new AuditSearchRequestRequest
        {
            StartTime = startDate,
            EndTime = endDate,
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate);
    }

    [Fact]
    public async Task SearchAuditEvents_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            Skip = 2,
            Take = 3
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count().ShouldBe(3);
        result.Total.ShouldBe(_auditEvents.Count);
    }

    [Fact]
    public async Task SearchAuditEvents_NavigationTable_IncludesChangesJson()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            TableName = "NAVIGATION",
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e => e.ChangesJson != null);
        result.Results.Count().ShouldBe(5);
    }

    [Fact]
    public async Task SearchAuditEvents_NonNavigationTable_ExcludesChangesJson()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            TableName = "DEMOGRAPHIC",
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e => e.ChangesJson == null);
        result.Results.Count().ShouldBe(4); // 4 DEMOGRAPHIC events
    }

    [Fact]
    public async Task SearchAuditEvents_WithMultipleFilters_ReturnsMatchingEvents()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            TableName = "NAVIGATION",
            Operation = "Update",
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(e =>
            e.TableName != null && e.TableName.Contains("NAVIGATION") &&
            e.Operation.Contains("Update"));

        // Verify we have at least one matching event (created deterministically in CreateMockAuditEvents)
        result.Total.ShouldBeGreaterThanOrEqualTo(1);
        result.Results.Count().ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task SearchAuditEvents_NoMatches_ReturnsEmptyResults()
    {
        // Arrange
        var request = new AuditSearchRequestRequest
        {
            TableName = "NONEXISTENT_TABLE_XYZ",
            Skip = 0,
            Take = 100
        };

        // Act
        var result = await _service.SearchAuditEventsAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Results.ShouldBeEmpty();
        result.Total.ShouldBe(0);
    }

    /// <summary>
    /// Creates mock audit events using Faker with realistic test data.
    /// Uses a fixed seed for deterministic test results.
    /// </summary>
    private static List<AuditEvent> CreateMockAuditEvents()
    {
        // Use a fixed seed for deterministic random data generation
        Randomizer.Seed = new Random(42);

        var faker = new Faker();
        var auditEvents = new List<AuditEvent>();
        long currentId = 1;

        // Create NAVIGATION table events with ChangesJson
        // Ensure at least one Update operation exists for the multiple filters test
        var navigationFaker = new Faker<AuditEvent>()
            .RuleFor(e => e.Id, f => currentId++)
            .RuleFor(e => e.TableName, f => "NAVIGATION")
            .RuleFor(e => e.Operation, f => f.PickRandom("Insert", "Update", "Delete", "Archive"))
            .RuleFor(e => e.PrimaryKey, f => f.Random.Int(1, 1000).ToString())
            .RuleFor(e => e.UserName, f => f.PickRandom("admin.user", "finance.manager", "it.devops"))
            .RuleFor(e => e.CreatedAt, f => f.Date.RecentOffset(days: 30))
            .RuleFor(e => e.ChangesJson, f => new List<AuditChangeEntry>
            {
                new AuditChangeEntry
                {
                    Id = f.Random.Long(1, 10000),
                    ColumnName = "StatusId",
                    OriginalValue = f.Random.Int(1, 5).ToString(),
                    NewValue = f.Random.Int(1, 5).ToString()
                },
                new AuditChangeEntry
                {
                    Id = f.Random.Long(1, 10000),
                    ColumnName = "ModifiedDate",
                    OriginalValue = f.Date.Past().ToString(),
                    NewValue = f.Date.Recent().ToString()
                }
            })
            .RuleFor(e => e.ChangesHash, f => f.Random.Hash());

        auditEvents.AddRange(navigationFaker.Generate(4));

        // Add one guaranteed NAVIGATION Update event for the multiple filters test
        auditEvents.Add(new AuditEvent
        {
            Id = currentId++,
            TableName = "NAVIGATION",
            Operation = "Update",
            PrimaryKey = "999",
            UserName = "admin.user",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),
            ChangesJson = new List<AuditChangeEntry>
            {
                new AuditChangeEntry
                {
                    Id = 1,
                    ColumnName = "StatusId",
                    OriginalValue = "1",
                    NewValue = "2"
                }
            },
            ChangesHash = faker.Random.Hash()
        });

        // Create DEMOGRAPHIC table events without ChangesJson requirement
        var demographicFaker = new Faker<AuditEvent>()
            .RuleFor(e => e.Id, f => currentId++)
            .RuleFor(e => e.TableName, f => "DEMOGRAPHIC")
            .RuleFor(e => e.Operation, f => f.PickRandom("Insert", "Update", "Delete"))
            .RuleFor(e => e.PrimaryKey, f => f.Random.Int(1, 10000).ToString())
            .RuleFor(e => e.UserName, f => f.PickRandom("hr.admin", "payroll.clerk", "admin.user"))
            .RuleFor(e => e.CreatedAt, f => f.Date.RecentOffset(days: 30))
            .RuleFor(e => e.ChangesJson, f => new List<AuditChangeEntry>
            {
                new AuditChangeEntry
                {
                    Id = f.Random.Long(1, 10000),
                    ColumnName = "LastName",
                    OriginalValue = f.Name.LastName(),
                    NewValue = f.Name.LastName()
                }
            })
            .RuleFor(e => e.ChangesHash, f => f.Random.Hash());

        auditEvents.AddRange(demographicFaker.Generate(4));

        // Create Archive operations for reports
        var archiveFaker = new Faker<AuditEvent>()
            .RuleFor(e => e.Id, f => currentId++)
            .RuleFor(e => e.TableName, f => f.PickRandom("YearEndReport", "DistributionReport", "BeneficiaryReport"))
            .RuleFor(e => e.Operation, f => "Archive")
            .RuleFor(e => e.PrimaryKey, f => f.Random.Int(2020, 2025).ToString())
            .RuleFor(e => e.UserName, f => f.PickRandom("admin.user", "finance.manager"))
            .RuleFor(e => e.CreatedAt, f => f.Date.RecentOffset(days: 60))
            .RuleFor(e => e.ChangesJson, f => new List<AuditChangeEntry>
            {
                new AuditChangeEntry
                {
                    Id = f.Random.Long(1, 10000),
                    ColumnName = "Report",
                    OriginalValue = null,
                    NewValue = f.Random.Hash() // Simulating JSON payload
                }
            })
            .RuleFor(e => e.ChangesHash, f => f.Random.Hash());

        auditEvents.AddRange(archiveFaker.Generate(3));

        // Create some older events for date range testing
        var olderEventsFaker = new Faker<AuditEvent>()
            .RuleFor(e => e.Id, f => currentId++)
            .RuleFor(e => e.TableName, f => f.PickRandom("PAYPROFIT", "PROFITDETAIL"))
            .RuleFor(e => e.Operation, f => f.PickRandom("Insert", "Update"))
            .RuleFor(e => e.PrimaryKey, f => f.Random.Int(1, 10000).ToString())
            .RuleFor(e => e.UserName, f => f.PickRandom("system.process", "batch.job"))
            .RuleFor(e => e.CreatedAt, f => f.Date.BetweenOffset(DateTimeOffset.UtcNow.AddDays(-90), DateTimeOffset.UtcNow.AddDays(-31)))
            .RuleFor(e => e.ChangesJson, f => new List<AuditChangeEntry>
            {
                new AuditChangeEntry
                {
                    Id = f.Random.Long(1, 10000),
                    ColumnName = "Amount",
                    OriginalValue = f.Finance.Amount(0, 10000).ToString(),
                    NewValue = f.Finance.Amount(0, 10000).ToString()
                }
            })
            .RuleFor(e => e.ChangesHash, f => f.Random.Hash());

        auditEvents.AddRange(olderEventsFaker.Generate(3));

        return auditEvents;
    }
}
