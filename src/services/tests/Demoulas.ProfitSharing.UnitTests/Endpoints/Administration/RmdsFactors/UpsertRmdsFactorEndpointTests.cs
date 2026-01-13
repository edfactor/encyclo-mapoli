using System.ComponentModel;
using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Administration.RmdFactors;

/// <summary>
/// Unit tests for UpsertRmdFactorEndpoint.
/// Validates add/update operations for RMD factors.
/// </summary>
[Collection("Administration Tests")]
public sealed class UpsertRmdFactorEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "UpsertRmdFactor - Should add new RMD percentage")]
    [Description("PS-2320 : Creates new RMD record for age not in database")]
    public async Task Post_AddsNewRmdFactor()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 100,
            Factor = 6.3m
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result.Age.ShouldBe(request.Age);
        response.Result.Factor.ShouldBe(request.Factor);
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should update existing RMD percentage")]
    [Description("PS-2320 : Updates factor for existing age")]
    public async Task Post_UpdatesExistingRmdFactor()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 73, // Exists in seed data with factor 26.5
            Factor = 27.0m // New factor
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result.Age.ShouldBe(request.Age);
        response.Result.Factor.ShouldBe(request.Factor);
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should validate age minimum boundary")]
    [Description("PS-2320 : Rejects age below 73 (minimum RMD age)")]
    public async Task Post_RejectsAgeBelowMinimum()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 72, // Below minimum RMD age of 73
            Factor = 27.0m
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should validate age maximum boundary")]
    [Description("PS-2320 : Rejects age above 120")]
    public async Task Post_RejectsAgeAboveMaximum()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 121, // Above maximum age of 120
            Factor = 5.0m
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should validate factor greater than zero")]
    [Description("PS-2320 : Rejects factor value of 0")]
    public async Task Post_RejectsZeroFactor()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 75,
            Factor = 0m // Invalid: must be > 0
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should validate factor less than or equal to 100")]
    [Description("PS-2320 : Rejects unreasonably large factor values")]
    public async Task Post_RejectsFactorAbove100()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 75,
            Factor = 101m // Invalid: must be <= 100
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "UpsertRmdsFactor - Should accept valid factor with decimal precision")]
    [Description("PS-2320 : Accepts factor with one decimal place (e.g., 26.5)")]
    public async Task Post_AcceptsValidDecimalFactor()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 101,
            Factor = 5.9m // Valid decimal factor
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.Factor.ShouldBe(5.9m);
    }

    [Fact(DisplayName = "UpsertRmdsFactor - Should be accessible to ADMINISTRATOR role only")]
    [Description("PS-2320 : Verifies only administrators can modify Rmds data")]
    public async Task Post_AllowsAccessForAdministratorOnly()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new RmdsFactorRequest
        {
            Age = 102,
            Factor = 5.8m
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact(DisplayName = "UpsertRmdsFactor - Should reject access for non-admin roles")]
    [Description("PS-2320 : Ensures AUDITOR role cannot modify Rmds data")]
    public async Task Post_RejectsAccessForNonAdminRoles()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);
        var request = new RmdsFactorRequest
        {
            Age = 103,
            Factor = 5.7m
        };

        // Act
        var response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact(DisplayName = "UpsertRmdsFactor - Should support multiple sequential operations")]
    [Description("PS-2320 : Validates repeated add/update operations")]
    public async Task Post_SupportsMultipleOperations()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte testAge = 104;

        // Act 1 - Add
        var addRequest = new RmdsFactorRequest { Age = testAge, Factor = 5.5m };
        var addResponse = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(addRequest);

        // Assert 1
        addResponse.Response.IsSuccessStatusCode.ShouldBeTrue();
        addResponse.Result.Factor.ShouldBe(5.5m);

        // Act 2 - Update
        var updateRequest = new RmdsFactorRequest { Age = testAge, Factor = 5.6m };
        var updateResponse = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(updateRequest);

        // Assert 2
        updateResponse.Response.IsSuccessStatusCode.ShouldBeTrue();
        updateResponse.Result.Factor.ShouldBe(5.6m);

        // Act 3 - Update again
        var update2Request = new RmdsFactorRequest { Age = testAge, Factor = 5.7m };
        var update2Response = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(update2Request);

        // Assert 3
        update2Response.Response.IsSuccessStatusCode.ShouldBeTrue();
        update2Response.Result.Factor.ShouldBe(5.7m);
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should handle all valid age ranges")]
    [Description("PS-2320 : Tests upsert for minimum, mid, and maximum ages")]
    public async Task Post_HandlesAllValidAgeRanges()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act & Assert - Minimum age (73)
        var minRequest = new RmdsFactorRequest { Age = 73, Factor = 26.5m };
        var minResponse = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(minRequest);
        minResponse.Response.IsSuccessStatusCode.ShouldBeTrue();

        // Act & Assert - Mid-range age (90)
        var midRequest = new RmdsFactorRequest { Age = 90, Factor = 12.2m };
        var midResponse = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(midRequest);
        midResponse.Response.IsSuccessStatusCode.ShouldBeTrue();

        // Act & Assert - Maximum age (120)
        var maxRequest = new RmdsFactorRequest { Age = 120, Factor = 2.0m };
        var maxResponse = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(maxRequest);
        maxResponse.Response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact(DisplayName = "UpsertRmdFactor - Should preserve data integrity after update")]
    [Description("PS-2320 : Verifies updated values are persisted correctly")]
    public async Task Post_PreservesDataIntegrityAfterUpdate()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte testAge = 105;

        // Act 1 - Add initial value
        var addRequest = new RmdsFactorRequest { Age = testAge, Factor = 5.4m };
        await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(addRequest);

        // Act 2 - Update value
        var updateRequest = new RmdsFactorRequest { Age = testAge, Factor = 5.3m };
        var updateResponse = await ApiClient.POSTAsync<UpsertRmdsFactorEndpoint, RmdsFactorRequest, RmdsFactorDto>(updateRequest);

        // Act 3 - Retrieve to verify
        var getResponse = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = testAge });

        // Assert
        updateResponse.Result.Factor.ShouldBe(5.3m);
        getResponse.Result.Factor.ShouldBe(5.3m); // Verify persistence
    }
}
