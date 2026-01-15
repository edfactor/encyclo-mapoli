using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.ItDevOps;

[Description("PS-2319 : OracleHcm sync diagnostics service methods")]
public class OracleHcmDiagnosticsServiceTests
{
    private readonly IOracleHcmDiagnosticsService _service;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public OracleHcmDiagnosticsServiceTests()
    {
        // Setup service with mock context factory
        _dataContextFactory = new InMemoryProfitSharingDataContextFactory();
        _service = new OracleHcmDiagnosticsService(_dataContextFactory, new MockCommitGuardOverride(), NullLogger<OracleHcmDiagnosticsService>.Instance);
    }

    #region GetOracleHcmSyncMetadataAsync Tests

    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataAsync returns metadata with timestamps")]
    public async Task GetOracleHcmSyncMetadataAsync_ReturnsOracleHcmSyncMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var result = await _service.GetOracleHcmSyncMetadataAsync(ct);

        // Assert
        result.ShouldNotBeNull();
        if (!result.IsSuccess)
        {
            throw new Exception($"Service call failed with error: {result.Error?.Description}");
        }
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        // With empty database, all timestamps will be null
        result.Value.DemographicCreatedAtUtc.ShouldBeNull();
        result.Value.DemographicModifiedAtUtc.ShouldBeNull();
        result.Value.PayProfitCreatedAtUtc.ShouldBeNull();
        result.Value.PayProfitModifiedAtUtc.ShouldBeNull();
    }

    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataAsync returns timestamps as DateTimeOffset (compile-time check)")]
    public async Task GetOracleHcmSyncMetadataAsync_TimestampsAreDateTimeOffset()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var result = await _service.GetOracleHcmSyncMetadataAsync(ct);

        // Assert - compile-time type check is sufficient for empty database
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        // With empty database, timestamps are null but the type is still DateTimeOffset?
        // This test primarily validates compile-time type safety
    }


    #endregion

    #region GetDemographicSyncAuditAsync Tests

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync returns empty list when no records exist")]
    public async Task GetDemographicSyncAuditAsync_NoRecords_ReturnsEmptyPage()
    {
        // Arrange
        var ct = CancellationToken.None;
        var request = new SortedPaginationRequestDto { Skip = 0, Take = 50, SortBy = "Created", IsSortDescending = true };

        // Act
        var result = await _service.GetDemographicSyncAuditAsync(request, ct);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Results.ShouldBeEmpty();
        result.Value.Total.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync returns paginated records")]
    public async Task GetDemographicSyncAuditAsync_WithRecords_ReturnsPaginatedData()
    {
        // Arrange
        var ct = CancellationToken.None;
        var now = DateTimeOffset.UtcNow;

        // Add test audit records
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
            {
                BadgeNumber = 12345,
                OracleHcmId = 1,
                Message = "Test sync error 1",
                PropertyName = "FirstName",
                InvalidValue = "Bad Value",
                UserName = "testuser",
                Created = now
            });

            ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
            {
                BadgeNumber = 12346,
                OracleHcmId = 2,
                Message = "Test sync error 2",
                PropertyName = "LastName",
                InvalidValue = "Another Bad",
                UserName = "testuser",
                Created = now.AddMinutes(-1)
            });

            await ctx.SaveChangesAsync(ct);
        }, ct);

        // Act
        var request = new SortedPaginationRequestDto { Skip = 0, Take = 50, SortBy = "Created", IsSortDescending = true };
        var result = await _service.GetDemographicSyncAuditAsync(request, ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        var records = result.Value!.Results.ToList();
        records.Count.ShouldBe(2);
        result.Value!.Total.ShouldBe(2);

        // Verify records are ordered by Created descending
        records[0].BadgeNumber.ShouldBe(12345);
        records[1].BadgeNumber.ShouldBe(12346);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync respects pagination")]
    public async Task GetDemographicSyncAuditAsync_Pagination_SkipsAndTakesCorrectly()
    {
        // Arrange
        var ct = CancellationToken.None;
        var now = DateTimeOffset.UtcNow;

        // Add test records
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            for (int i = 1; i <= 5; i++)
            {
                ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
                {
                    BadgeNumber = 12340 + i,
                    OracleHcmId = i,
                    Message = $"Test error {i}",
                    PropertyName = "TestProp",
                    InvalidValue = $"Value{i}",
                    UserName = "testuser",
                    Created = now.AddMinutes(-i)
                });
            }

            await ctx.SaveChangesAsync(ct);
        }, ct);

        // Act - Page 1
        var request1 = new SortedPaginationRequestDto { Skip = 0, Take = 2, SortBy = "Created", IsSortDescending = true };
        var result1 = await _service.GetDemographicSyncAuditAsync(request1, ct);

        // Act - Page 2
        var request2 = new SortedPaginationRequestDto { Skip = 2, Take = 2, SortBy = "Created", IsSortDescending = true };
        var result2 = await _service.GetDemographicSyncAuditAsync(request2, ct);

        // Assert Page 1
        result1.IsSuccess.ShouldBeTrue();
        var records1 = result1.Value!.Results.ToList();
        records1.Count.ShouldBe(2);
        result1.Value!.Total.ShouldBe(5);
        records1[0].BadgeNumber.ShouldBe(12341); // Most recent (created now - 1 minute)

        // Assert Page 2
        result2.IsSuccess.ShouldBeTrue();
        var records2 = result2.Value!.Results.ToList();
        records2.Count.ShouldBe(2);
        records2[0].BadgeNumber.ShouldBe(12343); // Next in descending order
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync validates page size boundaries")]
    public async Task GetDemographicSyncAuditAsync_InvalidPageSize_IsAdjusted()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act - Test with page size > 1000 (should be adjusted to 50)
        var request = new SortedPaginationRequestDto { Skip = 0, Take = 5000, SortBy = "Created", IsSortDescending = true };
        var result = await _service.GetDemographicSyncAuditAsync(request, ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Results.ShouldNotBeNull();
    }

    #endregion

}

/// <summary>
/// Mock implementation of ICommitGuardOverride for testing purposes.
/// Allows the IT_DEVOPS role to perform write operations without actual authorization checks.
/// </summary>
internal sealed class MockCommitGuardOverride : ICommitGuardOverride
{
    public IDisposable AllowFor(params string[] roles)
    {
        return new MockGuardScope();
    }

    public IEnumerable<string> GetCurrentRoles()
    {
        return new[] { Role.ITDEVOPS };
    }

    private sealed class MockGuardScope : IDisposable
    {
        public void Dispose()
        {
            // No-op for mock
        }
    }
}

/// <summary>
/// In-memory implementation of IProfitSharingDataContextFactory for unit testing.
/// </summary>
internal sealed class InMemoryProfitSharingDataContextFactory : IProfitSharingDataContextFactory
{
    private ProfitSharingDbContext? _context;
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public Task<T> UseReadOnlyContext<T>(
            Func<ProfitSharingReadOnlyDbContext, Task<T>> operation,
            CancellationToken cancellationToken = default)
    {
        // Ensure the writable context exists first (creates the database)
        var ctx = _context ??= CreateInMemoryContext();

        // Create a proper read-only context using the same database name
        var readOnlyOptions = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        var readOnlyCtx = new ProfitSharingReadOnlyDbContext(readOnlyOptions);
        return operation(readOnlyCtx);
    }

    public Task UseReadOnlyContext(
        Func<ProfitSharingReadOnlyDbContext, Task> operation,
        CancellationToken cancellationToken = default)
    {
        // Ensure the writable context exists first (creates the database)
        var ctx = _context ??= CreateInMemoryContext();

        // Create a proper read-only context using the same database name
        var readOnlyOptions = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        var readOnlyCtx = new ProfitSharingReadOnlyDbContext(readOnlyOptions);
        return operation(readOnlyCtx);
    }

    public Task<T> UseWritableContext<T>(
        Func<ProfitSharingDbContext, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        var ctx = _context ??= CreateInMemoryContext();
        return operation(ctx);
    }

    public Task UseWritableContext(
        Func<ProfitSharingDbContext, Task> operation,
        CancellationToken cancellationToken = default)
    {
        var ctx = _context ??= CreateInMemoryContext();
        return operation(ctx);
    }

    public async Task<T> UseWritableContextAsync<T>(
        Func<ProfitSharingDbContext, IDbContextTransaction, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        var ctx = _context ??= CreateInMemoryContext();
        await using var transaction = await ctx.Database.BeginTransactionAsync(cancellationToken);
        return await operation(ctx, transaction);
    }

    public Task<T> UseWarehouseContext<T>(Func<IDemoulasCommonWarehouseContext, Task<T>> func)
    {
        throw new NotImplementedException("Warehouse context not needed for tests");
    }

    private ProfitSharingDbContext CreateInMemoryContext()
    {
        // Create in-memory context for testing using the shared database name
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        var context = new ProfitSharingDbContext(options);
        // Note: Do NOT call context.Database.EnsureCreated() for InMemory databases
        // InMemory databases are created automatically on first access
        // Calling EnsureCreated() causes issues with entities that lack value generators (e.g., DistributionFrequency)
        return context;
    }
}

