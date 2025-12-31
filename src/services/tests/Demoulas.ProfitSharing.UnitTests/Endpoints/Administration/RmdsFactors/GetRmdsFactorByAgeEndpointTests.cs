using System.ComponentModel;
using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Administration.RmdFactors;

/// <summary>
/// Unit tests for GetRmdFactorByAgeEndpoint.
/// Validates retrieval of specific RMD factors by age.
/// </summary>
[Collection("Administration Tests")]
public sealed class GetRmdFactorByAgeEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "GetRmdFactorByAge - Should return RMD for valid age")]
    [Description("PS-XXXX : Retrieves RMD factor for age 73")]
    public async Task Get_ReturnsRmdForValidAge()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte age = 73;

        // Act
        var response = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = age });

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result.Age.ShouldBe(age);
        response.Result.Factor.ShouldBe(26.5m); // IRS factor for age 73
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should return 404 for non-existent age")]
    [Description("PS-XXXX : Returns NotFound when age not in database")]
    public async Task Get_Returns404ForNonExistentAge()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte age = 120; // Beyond seeded data

        // Act
        var response = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = age });

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should return correct factor for age 80")]
    [Description("PS-XXXX : Validates mid-range IRS factor")]
    public async Task Get_ReturnsCorrectFactorForAge80()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte age = 80;

        // Act
        var response = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = age });

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.Age.ShouldBe(age);
        response.Result.Factor.ShouldBe(20.2m); // IRS factor for age 80
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should return correct factor for age 99")]
    [Description("PS-XXXX : Validates maximum seeded age factor")]
    public async Task Get_ReturnsCorrectFactorForAge99()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte age = 99;

        // Act
        var response = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = age });

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.Age.ShouldBe(age);
        response.Result.Factor.ShouldBe(6.8m); // IRS factor for age 99
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should be accessible to ADMINISTRATOR role")]
    [Description("PS-XXXX : Verifies administrators can access RMD by age")]
    public async Task Get_AllowsAccessForAdministrator()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = 73 });

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should handle boundary ages")]
    [Description("PS-XXXX : Tests minimum and maximum age boundaries")]
    public async Task Get_HandlesBoundaryAges()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act - Minimum seeded age (73)
        var minResponse = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = 73 });

        // Act - Maximum seeded age (99)
        var maxResponse = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = 99 });

        // Act - Below minimum (72)
        var belowMinResponse = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = 72 });

        // Assert
        minResponse.Response.IsSuccessStatusCode.ShouldBeTrue();
        maxResponse.Response.IsSuccessStatusCode.ShouldBeTrue();
        belowMinResponse.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should return proper response structure")]
    [Description("PS-2320 : Validates RmdFactorDto structure")]
    public async Task Get_ReturnsProperResponseStructure()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = 73 });

        // Assert
        response.Result.ShouldNotBeNull();
        response.Result.Age.ShouldBeGreaterThan((byte)0);
        response.Result.Factor.ShouldBeGreaterThan(0m);
    }

    [Fact(DisplayName = "GetRmdFactorByAge - Should return consistent results")]
    [Description("PS-2320 : Validates endpoint returns same data across multiple calls")]
    public async Task Get_ReturnsConsistentResults()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        const byte age = 80;

        // Act
        var response1 = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = age });
        var response2 = await ApiClient.GETAsync<GetRmdsFactorByAgeEndpoint, GetRmdsFactorByAgeRequest, RmdsFactorDto>(
            new GetRmdsFactorByAgeRequest { Age = age });

        // Assert
        response1.Result.Age.ShouldBe(response2.Result.Age);
        response1.Result.Factor.ShouldBe(response2.Result.Factor);
    }
}
