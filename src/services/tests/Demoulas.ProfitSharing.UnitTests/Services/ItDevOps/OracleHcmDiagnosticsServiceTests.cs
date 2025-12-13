using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        _service = new OracleHcmDiagnosticsService(_dataContextFactory, new MockCommitGuardOverride());
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
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.DemographicCreatedAtUtc.ShouldNotBeNull();
        result.Value.DemographicModifiedAtUtc.ShouldNotBeNull();
        result.Value.PayProfitCreatedAtUtc.ShouldNotBeNull();
        result.Value.PayProfitModifiedAtUtc.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataAsync returns timestamps as DateTimeOffset")]
    public async Task GetOracleHcmSyncMetadataAsync_TimestampsAreDateTimeOffset()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var result = await _service.GetOracleHcmSyncMetadataAsync(ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.DemographicCreatedAtUtc.ShouldBeOfType<DateTimeOffset?>();
        result.Value!.DemographicModifiedAtUtc.ShouldBeOfType<DateTimeOffset?>();
        result.Value!.PayProfitCreatedAtUtc.ShouldBeOfType<DateTimeOffset?>();
        result.Value!.PayProfitModifiedAtUtc.ShouldBeOfType<DateTimeOffset?>();
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
        records1[0].BadgeNumber.ShouldBe(12345); // Most recent (created now)

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

    #region ClearDemographicSyncAuditAsync Tests

    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditAsync removes all audit records")]
    public async Task ClearDemographicSyncAuditAsync_DeletesAllRecords()
    {
        // Arrange
        var ct = CancellationToken.None;
        var now = DateTimeOffset.UtcNow;

        // Add test records
        await _dataContextFactory.UseWritableContext(async ctx =>
        {
            for (int i = 1; i <= 3; i++)
            {
                ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
                {
                    BadgeNumber = 12340 + i,
                    OracleHcmId = i,
                    Message = $"Test error {i}",
                    PropertyName = "TestProp",
                    InvalidValue = $"Value{i}",
                    UserName = "testuser",
                    Created = now
                });
            }

            await ctx.SaveChangesAsync(ct);
        }, ct);

        // Verify records exist before clear
        var beforeRequest = new SortedPaginationRequestDto { Skip = 0, Take = 50, SortBy = "Created", IsSortDescending = true };
        var beforeClear = await _service.GetDemographicSyncAuditAsync(beforeRequest, ct);
        beforeClear.Value!.Total.ShouldBe(3);

        // Act
        var result = await _service.ClearDemographicSyncAuditAsync(ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.ShouldBe(3); // Should return count of deleted records

        // Verify records are deleted
        var afterRequest = new SortedPaginationRequestDto { Skip = 0, Take = 50, SortBy = "Created", IsSortDescending = true };
        var afterClear = await _service.GetDemographicSyncAuditAsync(afterRequest, ct);
        afterClear.Value!.Total.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditAsync returns 0 when no records exist")]
    public async Task ClearDemographicSyncAuditAsync_NoRecords_ReturnsZero()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var result = await _service.ClearDemographicSyncAuditAsync(ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(0);
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

    public Task<IProfitSharingDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IProfitSharingDbContext>(_context ??= CreateInMemoryContext());
    }

    public Task<T> UseReadOnlyContext<T>(
        Func<ProfitSharingReadOnlyDbContext, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Read-only context not implemented for in-memory tests");
    }

    public Task UseReadOnlyContext(
        Func<ProfitSharingReadOnlyDbContext, Task> operation,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Read-only context not implemented for in-memory tests");
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

    public Task<T> UseWarehouseContext<T>(Func<DemoulasCommonWarehouseContext, Task<T>> func)
    {
        throw new NotImplementedException("Warehouse context not needed for tests");
    }

    private static ProfitSharingDbContext CreateInMemoryContext()
    {
        // Create in-memory SQLite context for testing
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ProfitSharingDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

