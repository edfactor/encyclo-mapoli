using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Services.ItDevOps;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services.ItDevOps;

[Description("PS-2319 : OracleHcm sync diagnostics service methods")]
public class OracleHcmDiagnosticsServiceTests : ServiceTestBase
{
    private IOracleHcmDiagnosticsService _service = null!;

    public OracleHcmDiagnosticsServiceTests(ServiceTestBaseFixture fixture) : base(fixture)
    {
    }

    public override void Setup()
    {
        base.Setup();
        _service = ServiceProvider.GetRequiredService<IOracleHcmDiagnosticsService>();
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

        // Act
        var result = await _service.GetDemographicSyncAuditAsync(1, 50, ct);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Records.ShouldBeEmpty();
        result.Value.PageNumber.ShouldBe(1);
        result.Value.PageSize.ShouldBe(50);
        result.Value.TotalCount.ShouldBe(0);
        result.Value.TotalPages.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync returns paginated records")]
    public async Task GetDemographicSyncAuditAsync_WithRecords_ReturnsPaginatedData()
    {
        // Arrange
        var ct = CancellationToken.None;
        var now = DateTimeOffset.UtcNow;

        // Add test audit records
        await TestDbContextFactory.UseWritableContext(async ctx =>
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
        var result = await _service.GetDemographicSyncAuditAsync(1, 50, ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Records.Count.ShouldBe(2);
        result.Value.PageNumber.ShouldBe(1);
        result.Value.PageSize.ShouldBe(50);
        result.Value.TotalCount.ShouldBe(2);
        result.Value.TotalPages.ShouldBe(1);

        // Verify records are ordered by Created descending
        result.Value.Records[0].BadgeNumber.ShouldBe(12345);
        result.Value.Records[1].BadgeNumber.ShouldBe(12346);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync respects pagination")]
    public async Task GetDemographicSyncAuditAsync_Pagination_SkipsAndTakesCorrectly()
    {
        // Arrange
        var ct = CancellationToken.None;
        var now = DateTimeOffset.UtcNow;

        // Add test records
        await TestDbContextFactory.UseWritableContext(async ctx =>
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
        var result1 = await _service.GetDemographicSyncAuditAsync(1, 2, ct);

        // Act - Page 2
        var result2 = await _service.GetDemographicSyncAuditAsync(2, 2, ct);

        // Assert Page 1
        result1.IsSuccess.ShouldBeTrue();
        result1.Value.Records.Count.ShouldBe(2);
        result1.Value.TotalPages.ShouldBe(3);
        result1.Value.Records[0].BadgeNumber.ShouldBe(12345); // Most recent (created now)

        // Assert Page 2
        result2.IsSuccess.ShouldBeTrue();
        result2.Value.Records.Count.ShouldBe(2);
        result2.Value.Records[0].BadgeNumber.ShouldBe(12343); // Next in descending order
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditAsync validates page size boundaries")]
    public async Task GetDemographicSyncAuditAsync_InvalidPageSize_IsAdjusted()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act - Test with page size > 1000 (should be adjusted to 50)
        var result = await _service.GetDemographicSyncAuditAsync(1, 5000, ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.PageSize.ShouldBe(50); // Should be adjusted to default
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
        await TestDbContextFactory.UseWritableContext(async ctx =>
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
        var beforeClear = await _service.GetDemographicSyncAuditAsync(1, 50, ct);
        beforeClear.Value.TotalCount.ShouldBe(3);

        // Act
        var result = await _service.ClearDemographicSyncAuditAsync(ct);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(3); // Should return count of deleted records

        // Verify records are deleted
        var afterClear = await _service.GetDemographicSyncAuditAsync(1, 50, ct);
        afterClear.Value.TotalCount.ShouldBe(0);
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
