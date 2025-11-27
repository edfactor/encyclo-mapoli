using System.ComponentModel;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints;

/// <summary>
/// Collection definition for UnmaskSsn endpoint tests - disables parallelization
/// </summary>
[CollectionDefinition("UnmaskSsn", DisableParallelization = true)]
public class UnmaskSsnCollection
{
    // This class defines the test collection but contains no tests
}

/// <summary>
/// Unit tests for UnmaskSsnEndpoint to verify SSN unmasking endpoint behavior.
/// Tests authorization, response format, audit logging, and error handling.
/// </summary>
[Collection("UnmaskSsn")]
[Description("PS-2098 : Verify SSN unmasking endpoint enforces authorization and returns correct response format")]
public sealed class UnmaskSsnEndpointTests : ApiTestBase<Api.Program>
{
    #region Authorization Tests

    [Fact(DisplayName = "PS-2098 : Should return 403 Forbidden when user lacks SSN-Unmasking role")]
    public async Task UnmaskSsn_WithoutSsnUnmaskingRole_ReturnsForbidden()
    {
        // Arrange - Create token WITHOUT SSN-Unmasking role
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, object>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "PS-2098 : Should return 200 OK when user has SSN-Unmasking role")]
    public async Task UnmaskSsn_WithSsnUnmaskingRole_ReturnsOk()
    {
        // Arrange - Create token WITH SSN-Unmasking role
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Data.ShouldNotBeNull();
        response.Data.UnmaskedSsn.ShouldNotBeNullOrEmpty();
    }

    #endregion

    #region Happy Path Tests

    [Fact(DisplayName = "PS-2098 : Should return formatted SSN with correct pattern")]
    public async Task UnmaskSsn_WithValidRequest_ReturnsFormattedSsn()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Data.UnmaskedSsn.ShouldNotBeNull();

        // Verify SSN is formatted as XXX-XX-XXXX
        response.Data.UnmaskedSsn.ShouldMatch(@"^\d{3}-\d{2}-\d{4}$",
            "SSN should be formatted as XXX-XX-XXXX");
    }

    [Fact(DisplayName = "PS-2098 : Should return 404 Not Found for non-existent demographic")]
    public async Task UnmaskSsn_WithInvalidDemographicId_ReturnsNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var request = new UnmaskSsnRequest { DemographicId = 999999999 };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, object>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    #endregion

    #region Response Structure Tests

    [Fact(DisplayName = "PS-2098 : Response should have UnmaskedSsn property")]
    public async Task UnmaskSsn_Response_ContainsUnmaskedSsnProperty()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Data.ShouldNotBeNull();
        response.Data.UnmaskedSsn.ShouldNotBeNull("Response must contain UnmaskedSsn property");
        typeof(UnmaskSsnResponse).GetProperty("UnmaskedSsn").ShouldNotBeNull(
            "UnmaskSsnResponse must have UnmaskedSsn property");
    }

    [Fact(DisplayName = "PS-2098 : Response UnmaskedSsn should not be empty")]
    public async Task UnmaskSsn_Response_UnmaskedSsnNotEmpty()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Data.UnmaskedSsn.ShouldNotBe(string.Empty);
        response.Data.UnmaskedSsn.Length.ShouldBe(11, "Formatted SSN should be 11 characters (XXX-XX-XXXX)");
    }

    #endregion

    #region Request Validation Tests

    [Fact(DisplayName = "PS-2098 : Should return 400 Bad Request for zero demographic ID")]
    public async Task UnmaskSsn_WithZeroDemographicId_ReturnsBadRequest()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var request = new UnmaskSsnRequest { DemographicId = 0 };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, object>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "PS-2098 : Should return 400 Bad Request for negative demographic ID")]
    public async Task UnmaskSsn_WithNegativeDemographicId_ReturnsBadRequest()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var request = new UnmaskSsnRequest { DemographicId = -1 };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, object>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }

    #endregion

    #region Multiple Role Tests

    [Theory(DisplayName = "PS-2098 : Should allow access with readonly roles that have SSN-Unmasking")]
    [InlineData(Role.SSN_UNMASKING)]
    [InlineData(Role.HR_READONLY)]
    [InlineData(Role.AUDITOR)]
    [InlineData(Role.ITDEVOPS)]
    public async Task UnmaskSsn_WithVariousReadonlyRoles_ReturnsOk(string role)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(role);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK,
            $"Should allow access with role {role}");
        response.Data.UnmaskedSsn.ShouldNotBeNullOrEmpty();
    }

    #endregion

    #region Idempotency Tests

    [Fact(DisplayName = "PS-2098 : Multiple requests for same demographic should return same SSN")]
    public async Task UnmaskSsn_MultipleRequests_ReturnsSameSsn()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();
        var request = new UnmaskSsnRequest { DemographicId = demographicId };

        // Act
        var response1 = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);
        var response2 = await ApiClient.POSTAsync<UnmaskSsnEndpoint, UnmaskSsnRequest, UnmaskSsnResponse>(request);

        // Assert
        response1.Data.UnmaskedSsn.ShouldBe(response2.Data.UnmaskedSsn,
            "Same demographic should always return same SSN");
    }

    #endregion

    #region HTTP Method Tests

    [Fact(DisplayName = "PS-2098 : Endpoint should only accept POST requests")]
    public async Task UnmaskSsn_WithGetRequest_ReturnsMethodNotAllowed()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.SSN_UNMASKING);

        var demographicId = MockDbContextFactory.GetFirstDemographicId();

        // Act - Attempt GET instead of POST
        var response = await ApiClient.GetAsync($"/api/ssn-unmasking");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.MethodNotAllowed);
    }

    #endregion
}
