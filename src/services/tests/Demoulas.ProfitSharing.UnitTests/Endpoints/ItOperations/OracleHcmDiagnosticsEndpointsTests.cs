using System.ComponentModel;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.ItOperations;

[Collection("IT Operations Tests")]
[Description("PS-2319 : OracleHcm sync diagnostics endpoints")]
public class GetOracleHcmSyncMetadataEndpointTests : ApiTestBase<Program>
{
    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataEndpoint returns sync metadata")]
    public async Task ExecuteAsync_ReturnsSyncMetadata()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        TestResult<OracleHcmSyncMetadataResponse> response = await ApiClient.GETAsync<GetOracleHcmSyncMetadataEndpoint, OracleHcmSyncMetadataResponse>();

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(200);
        response.Result.ShouldNotBeNull();
        response.Result.DemographicCreatedAtUtc.ShouldNotBeNull();
        response.Result.DemographicModifiedAtUtc.ShouldNotBeNull();
        response.Result.PayProfitCreatedAtUtc.ShouldNotBeNull();
        response.Result.PayProfitModifiedAtUtc.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        TestResult<OracleHcmSyncMetadataResponse> response = await ApiClient.GETAsync<GetOracleHcmSyncMetadataEndpoint, OracleHcmSyncMetadataResponse>();

        // Assert
        response.StatusCode.ShouldBe(401);
    }
}

[Collection("IT Operations Tests")]
[Description("PS-2319 : OracleHcm sync audit endpoint")]
public class GetDemographicSyncAuditEndpointTests : ApiTestBase<Program>
{
    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint returns paginated audit records")]
    public async Task ExecuteAsync_ReturnsPaginatedAuditRecords()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Add test audit records
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            for (int i = 1; i <= 5; i++)
            {
                ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
                {
                    BadgeNumber = 12340 + i,
                    OracleHcmId = i,
                    Message = $"Sync error {i}",
                    PropertyName = $"Property{i}",
                    InvalidValue = $"Value{i}",
                    UserName = "testuser",
                    Created = now.AddMinutes(-i)
                });
            }

            await ctx.SaveChangesAsync();
        });

        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        TestResult<DemographicSyncAuditPageResponse> response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>("/api/itdevops/oracleHcm/audit?pageNumber=1&pageSize=2");

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(200);
        response.Result.ShouldNotBeNull();
        response.Result.Records.Count.ShouldBe(2);
        response.Result.PageNumber.ShouldBe(1);
        response.Result.PageSize.ShouldBe(2);
        response.Result.TotalCount.ShouldBe(5);
        response.Result.TotalPages.ShouldBe(3);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint returns empty page when no records")]
    public async Task ExecuteAsync_NoRecords_ReturnsEmptyPage()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        TestResult<DemographicSyncAuditPageResponse> response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>("/api/itdevops/oracleHcm/audit");

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(200);
        response.Result.ShouldNotBeNull();
        response.Result.Records.ShouldBeEmpty();
        response.Result.TotalCount.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint handles pagination parameters")]
    public async Task ExecuteAsync_WithPaginationParams_PagesProperly()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Add test audit records
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            for (int i = 1; i <= 10; i++)
            {
                ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
                {
                    BadgeNumber = 12340 + i,
                    OracleHcmId = i,
                    Message = $"Sync error {i}",
                    PropertyName = "TestProp",
                    InvalidValue = $"Value{i}",
                    UserName = "testuser",
                    Created = now.AddMinutes(-i)
                });
            }

            await ctx.SaveChangesAsync();
        });

        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act - Get page 2 with 3 items per page
        TestResult<DemographicSyncAuditPageResponse> response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>("/api/itdevops/oracleHcm/audit?pageNumber=2&pageSize=3");

        // Assert
        response.StatusCode.ShouldBe(200);
        response.Result.Records.Count.ShouldBe(3);
        response.Result.PageNumber.ShouldBe(2);
        response.Result.PageSize.ShouldBe(3);
        response.Result.TotalPages.ShouldBe(4);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        TestResult<DemographicSyncAuditPageResponse> response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>();

        // Assert
        response.StatusCode.ShouldBe(401);
    }
}

[Collection("IT Operations Tests")]
[Description("PS-2319 : OracleHcm clear audit endpoint")]
public class ClearDemographicSyncAuditEndpointTests : ApiTestBase<Program>
{
    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditEndpoint deletes all audit records")]
    public async Task ExecuteAsync_ClearsAllAuditRecords()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Add test audit records
        await MockDbContextFactory.UseWritableContext(async ctx =>
        {
            for (int i = 1; i <= 3; i++)
            {
                ctx.DemographicSyncAudit.Add(new DemographicSyncAudit
                {
                    BadgeNumber = 12340 + i,
                    OracleHcmId = i,
                    Message = $"Sync error {i}",
                    PropertyName = "TestProp",
                    InvalidValue = $"Value{i}",
                    UserName = "testuser",
                    Created = now
                });
            }

            await ctx.SaveChangesAsync();
        });

        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        TestResult<ClearAuditResponse> response = await ApiClient.POSTAsync<ClearDemographicSyncAuditEndpoint, ClearAuditResponse>();

        // Assert
        response.ShouldNotBeNull();
        response.StatusCode.ShouldBe(200);
        response.Result.ShouldNotBeNull();
        response.Result.DeletedCount.ShouldBe(3);

        // Verify records are deleted
        var remainingRecords = await MockDbContextFactory.UseReadOnlyContext(async ctx =>
            await ctx.DemographicSyncAudit.CountAsync());
        remainingRecords.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditEndpoint returns 0 when no records exist")]
    public async Task ExecuteAsync_NoRecords_ReturnsZero()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        TestResult<ClearAuditResponse> response = await ApiClient.POSTAsync<ClearDemographicSyncAuditEndpoint, ClearAuditResponse>();

        // Assert
        response.StatusCode.ShouldBe(200);
        response.Result.DeletedCount.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        TestResult<ClearAuditResponse> response = await ApiClient.POSTAsync<ClearDemographicSyncAuditEndpoint, ClearAuditResponse>();

        // Assert
        response.StatusCode.ShouldBe(401);
    }
}
