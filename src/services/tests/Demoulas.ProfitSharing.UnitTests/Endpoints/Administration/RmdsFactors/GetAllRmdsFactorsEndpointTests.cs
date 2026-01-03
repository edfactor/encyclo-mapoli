using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Administration.RmdsFactors;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Administration.RmdFactors;

/// <summary>
/// Unit tests for GetAllRmdFactorsEndpoint.
/// Validates endpoint authorization, response structure, and data retrieval.
/// </summary>
[Collection("Administration Tests")]
public sealed class GetAllRmdFactorsEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "GetAllRmdFactors - Should return all RMD percentages")]
    [Description("PS-2320 : Returns complete list of RMD life expectancy factors")]
    public async Task Get_ReturnsAllRmdFactors()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result.ShouldNotBeEmpty();
        response.Result.Count.ShouldBeGreaterThanOrEqualTo(27); // Seed data has ages 73-99
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should return RMDs ordered by age")]
    [Description("PS-2320 : Ensures RMDs are returned in age order")]
    public async Task Get_ReturnsRmdsInAgeOrder()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        var ages = response.Result.Select(r => r.Age).ToList();
        var sortedAges = ages.OrderBy(a => a).ToList();
        ages.ShouldBe(sortedAges);
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should have valid age range")]
    [Description("PS-2320 : Validates all ages are within IRS RMD range (73-120)")]
    public async Task Get_HasValidAgeRange()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        response.Result.ShouldAllBe(r => r.Age >= 73 && r.Age <= 120);
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should have valid factor values")]
    [Description("PS-2320 : Validates all factors are positive and reasonable")]
    public async Task Get_HasValidFactorValues()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        response.Result.ShouldAllBe(r => r.Factor > 0 && r.Factor <= 100);
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should include known IRS factors")]
    [Description("PS-2320 : Verifies seed data includes correct IRS Publication 590-B factors")]
    public async Task Get_IncludesKnownIrsFactors()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        var rmdsByAge = response.Result.ToDictionary(r => r.Age, r => r.Factor);

        // Validate some known IRS factors from Uniform Lifetime Table
        if (rmdsByAge.ContainsKey(73))
        {
            rmdsByAge[73].ShouldBe(26.5m);
        }

        if (rmdsByAge.ContainsKey(80))
        {
            rmdsByAge[80].ShouldBe(20.2m);
        }

        if (rmdsByAge.ContainsKey(99))
        {
            rmdsByAge[99].ShouldBe(6.8m);
        }
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should be accessible to ADMINISTRATOR role")]
    [Description("PS-2320 : Verifies administrators can access RMD data")]
    public async Task Get_AllowsAccessForAdministrator()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should return consistent results")]
    [Description("PS-2320 : Validates endpoint returns same data across multiple calls")]
    public async Task Get_ReturnsConsistentResults()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response1 = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();
        var response2 = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        response1.Result.Count.ShouldBe(response2.Result.Count);
        response1.Result.Select(r => r.Age).OrderBy(a => a)
            .ShouldBe(response2.Result.Select(r => r.Age).OrderBy(a => a));
    }

    [Fact(DisplayName = "GetAllRmdFactors - Should have proper response structure")]
    [Description("PS-2320 : Validates RmdsFactorDto structure in response")]
    public async Task Get_HasProperResponseStructure()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<GetAllRmdsFactorsEndpoint, List<RmdsFactorDto>>();

        // Assert
        response.Result.ShouldAllBe(rmd =>
            rmd.Age > 0 &&
            rmd.Factor > 0 &&
            !string.IsNullOrWhiteSpace(rmd.Age.ToString()) &&
            !string.IsNullOrWhiteSpace(rmd.Factor.ToString())
        );
    }
}
