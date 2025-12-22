using System;
using System.ComponentModel;
using System.Linq;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Administration;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
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
        TestResult<OracleHcmSyncMetadataResponse> response = await ApiClient
            .GETAsync<GetOracleHcmSyncMetadataEndpoint, EmptyRequest, OracleHcmSyncMetadataResponse>(new EmptyRequest());

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
        TestResult<OracleHcmSyncMetadataResponse> response = await ApiClient
            .GETAsync<GetOracleHcmSyncMetadataEndpoint, EmptyRequest, OracleHcmSyncMetadataResponse>(new EmptyRequest());

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

        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "Created",
            IsSortDescending = true
        };

        // Act
        TestResult<PaginatedResponseDto<DemographicSyncAuditRecordResponse>> response = await ApiClient
            .GETAsync<GetDemographicSyncAuditEndpoint, SortedPaginationRequestDto, PaginatedResponseDto<DemographicSyncAuditRecordResponse>>(request);

        // Assert - Verify endpoint returns valid paginated response structure
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.Total.ShouldBe(5); // Mock factory seeds 5 DemographicSyncAudit records
        response.Result.Results.ShouldNotBeNull();
        response.Result.Results.Count().ShouldBe(5);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint returns audit records from mock factory")]
    public async Task ExecuteAsync_ReturnsAuditRecords_FromMockFactory()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "Created",
            IsSortDescending = true
        };

        // Act
        TestResult<PaginatedResponseDto<DemographicSyncAuditRecordResponse>> response = await ApiClient
            .GETAsync<GetDemographicSyncAuditEndpoint, SortedPaginationRequestDto, PaginatedResponseDto<DemographicSyncAuditRecordResponse>>(request);

        // Assert - Verify audit records returned from mock data factory
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.Results.ShouldNotBeNull();
        var records = response.Result.Results.ToList();
        records.ShouldNotBeEmpty();
        records.Count.ShouldBe(5); // DemographicSyncAuditFaker generates 5 records
        response.Result.Total.ShouldBe(5);
        records.All(r => r.Id > 0).ShouldBeTrue();
        records.All(r => r.BadgeNumber > 0).ShouldBeTrue();
        records.All(r => !string.IsNullOrWhiteSpace(r.Message)).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint handles pagination parameters")]
    public async Task ExecuteAsync_WithPaginationParams_PagesProperly()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "Created",
            IsSortDescending = true
        };

        // Act
        TestResult<PaginatedResponseDto<DemographicSyncAuditRecordResponse>> response = await ApiClient
            .GETAsync<GetDemographicSyncAuditEndpoint, SortedPaginationRequestDto, PaginatedResponseDto<DemographicSyncAuditRecordResponse>>(request);

        // Assert - Verify results are ordered by Created desc (newest first)
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.Total.ShouldBe(5);
        var results = response.Result.Results.ToList();
        results.Count.ShouldBe(5);
        results.Zip(results.Skip(1), (a, b) => a.Created >= b.Created).All(x => x).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        var request = new SortedPaginationRequestDto
        {
            Skip = 0,
            Take = 50,
            SortBy = "Created",
            IsSortDescending = true
        };

        // Act
        TestResult<PaginatedResponseDto<DemographicSyncAuditRecordResponse>> response = await ApiClient
            .GETAsync<GetDemographicSyncAuditEndpoint, SortedPaginationRequestDto, PaginatedResponseDto<DemographicSyncAuditRecordResponse>>(request);

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

        // Act - Clear endpoint (seeded with audit records)
        TestResult<ClearAuditResponse> response = await ApiClient
            .POSTAsync<ClearDemographicSyncAuditEndpoint, EmptyRequest, ClearAuditResponse>(new EmptyRequest());

        // Assert - Verify endpoint returns valid response
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.DeletedCount.ShouldBe(5);
    }

    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        TestResult<ClearAuditResponse> response = await ApiClient
            .POSTAsync<ClearDemographicSyncAuditEndpoint, EmptyRequest, ClearAuditResponse>(new EmptyRequest());

        // Assert
        ((int)response.Response.StatusCode).ShouldBe(401);
    }
}
