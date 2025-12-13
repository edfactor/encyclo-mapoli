using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
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
        var response = await ApiClient.GETAsync<GetOracleHcmSyncMetadataEndpoint, OracleHcmSyncMetadataResponse>();

        // Assert - Verify endpoint returned 200 success
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        TestResult<OracleHcmSyncMetadataResponse> response = await ApiClient.GETAsync<GetOracleHcmSyncMetadataEndpoint, OracleHcmSyncMetadataResponse>();

        // Assert
        ((int)response.Response.StatusCode).ShouldBe(401);
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
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act - Call endpoint (default pagination parameters: pageNumber=1, pageSize=50)
        var response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>();

        // Assert - Verify endpoint returns valid paginated response structure
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.PageNumber.ShouldBe(1);
        response.Result.PageSize.ShouldBe(50);
        response.Result.TotalCount.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint returns empty page when no records")]
    public async Task ExecuteAsync_NoRecords_ReturnsEmptyPage()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        var response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>();

        // Assert - Verify proper pagination response structure
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.Records.ShouldBeEmpty();
        response.Result.TotalCount.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint handles pagination parameters")]
    public async Task ExecuteAsync_WithPaginationParams_PagesProperly()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act - Default pagination should return 50 items per page
        var response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>();

        // Assert - Verify pagination defaults are applied
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.PageNumber.ShouldBe(1);
        response.Result.PageSize.ShouldBe(50);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        var response = await ApiClient.GETAsync<GetDemographicSyncAuditEndpoint, DemographicSyncAuditPageResponse>();

        // Assert
        ((int)response.Response.StatusCode).ShouldBe(401);
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
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act - Clear endpoint (with empty database)
        TestResult<ClearAuditResponse> response = await ApiClient.POSTAsync<ClearDemographicSyncAuditEndpoint, ClearAuditResponse>();

        // Assert - Verify endpoint returns valid response
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
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
        ((int)response.Response.StatusCode).ShouldBe(401);
    }
}
