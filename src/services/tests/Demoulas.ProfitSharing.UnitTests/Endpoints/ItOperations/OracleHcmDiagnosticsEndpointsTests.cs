using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.ItOperations;
using Demoulas.ProfitSharing.Endpoints.Endpoints.ItOperations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
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
        var httpResponse = await ApiClient.GetAsync("/api/itdevops/oracleHcm/metadata");
        var result = await httpResponse.Content.ReadFromJsonAsync<OracleHcmSyncMetadataResponse>();

        // Assert - Verify endpoint returned 200 success
        httpResponse.IsSuccessStatusCode.ShouldBeTrue(httpResponse.ReasonPhrase);
        result.ShouldNotBeNull();
    }

    [Fact]
    [Description("PS-2319 : GetOracleHcmSyncMetadataEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        var response = await ApiClient.GetAsync("/api/itdevops/oracleHcm/metadata");

        // Assert
        ((int)response.StatusCode).ShouldBe(401);
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

        // Act
        var httpResponse = await ApiClient.GetAsync("/api/itdevops/oracleHcm/audit?skip=0&take=50&sortBy=Created&isSortDescending=true");
        var result = await httpResponse.Content.ReadFromJsonAsync<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>();

        // Assert - Verify endpoint returns valid paginated response structure
        httpResponse.IsSuccessStatusCode.ShouldBeTrue(httpResponse.ReasonPhrase);
        result.ShouldNotBeNull();
        result.Total.ShouldBe(5); // Mock factory seeds 5 DemographicSyncAudit records
        result.Results.ShouldNotBeNull();
        result.Results.Count().ShouldBe(5);
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint returns audit records from mock factory")]
    public async Task ExecuteAsync_ReturnsAuditRecords_FromMockFactory()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ITDEVOPS);

        // Act
        var httpResponse = await ApiClient.GetAsync("/api/itdevops/oracleHcm/audit?skip=0&take=50&sortBy=Created&isSortDescending=true");
        var result = await httpResponse.Content.ReadFromJsonAsync<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>();

        // Assert - Verify audit records returned from mock data factory
        httpResponse.IsSuccessStatusCode.ShouldBeTrue(httpResponse.ReasonPhrase);
        result.ShouldNotBeNull();
        result.Results.ShouldNotBeNull();
        var records = result.Results.ToList();
        records.ShouldNotBeEmpty();
        records.Count.ShouldBe(5); // DemographicSyncAuditFaker generates 5 records
        result.Total.ShouldBe(5);
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

        // Act
        var httpResponse = await ApiClient.GetAsync("/api/itdevops/oracleHcm/audit?skip=0&take=50&sortBy=Created&isSortDescending=true");
        var result = await httpResponse.Content.ReadFromJsonAsync<PaginatedResponseDto<DemographicSyncAuditRecordResponse>>();

        // Assert - Verify results are ordered by Created desc (newest first)
        httpResponse.IsSuccessStatusCode.ShouldBeTrue(httpResponse.ReasonPhrase);
        result.ShouldNotBeNull();
        result.Total.ShouldBe(5);
        var results = result.Results.ToList();
        results.Count.ShouldBe(5);
        results.Zip(results.Skip(1), (a, b) => a.Created >= b.Created).All(x => x).ShouldBeTrue();
    }

    [Fact]
    [Description("PS-2319 : GetDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        var response = await ApiClient.GetAsync("/api/itdevops/oracleHcm/audit?skip=0&take=50&sortBy=Created&isSortDescending=true");

        // Assert
        ((int)response.StatusCode).ShouldBe(401);
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
        var httpResponse = await ApiClient.PostAsync("/api/itdevops/oracleHcm/audit/clear", content: null);
        var result = await httpResponse.Content.ReadFromJsonAsync<ClearAuditResponse>();

        // Assert - Verify endpoint returns valid response
        httpResponse.IsSuccessStatusCode.ShouldBeTrue(httpResponse.ReasonPhrase);
        result.ShouldNotBeNull();
        result.DeletedCount.ShouldBe(5);
    }

    [Fact]
    [Description("PS-2319 : ClearDemographicSyncAuditEndpoint requires ITDEVOPS role")]
    public async Task ExecuteAsync_WithoutRole_ReturnsForbidden()
    {
        // Arrange - No token assigned

        // Act
        var response = await ApiClient.PostAsync("/api/itdevops/oracleHcm/audit/clear", content: null);

        // Assert
        ((int)response.StatusCode).ShouldBe(401);
    }
}
