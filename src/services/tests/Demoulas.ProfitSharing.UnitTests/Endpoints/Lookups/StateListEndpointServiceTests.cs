using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

/// <summary>
/// Unit tests for StateListEndpoint to verify the endpoint correctly uses StateService
/// and returns properly formatted state list responses.
/// </summary>
[Collection("Lookup Tests")]
public sealed class StateListEndpointServiceTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "StateListEndpoint - Should return all states")]
    [Description("PS-2161 : Verifies endpoint retrieves all states from service")]
    public async Task Get_ReturnsAllStatesFromService()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeEmpty();
        response.Result.Items.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "StateListEndpoint - Should return states with correct structure")]
    [Description("PS-2161 : Validates StateListResponse DTO structure")]
    public async Task Get_ReturnsCorrectResponseStructure()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Result.Items.ShouldAllBe(s =>
            !string.IsNullOrWhiteSpace(s.Abbreviation) &&
            !string.IsNullOrWhiteSpace(s.Name) &&
            s.Abbreviation.Length == 2
        );
    }

    [Fact(DisplayName = "StateListEndpoint - Should return states ordered alphabetically")]
    [Description("PS-XXXX : Verifies states are returned in alphabetical order")]
    public async Task Get_ReturnsStatesInAlphabeticalOrder()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        var abbreviations = response.Result.Items.Select(s => s.Abbreviation).ToList();
        var sortedAbbreviations = abbreviations.OrderBy(a => a).ToList();
        abbreviations.ShouldBe(sortedAbbreviations);
    }

    [Fact(DisplayName = "StateListEndpoint - Should have valid abbreviations")]
    [Description("PS-XXXX : Validates all state abbreviations are 2 characters")]
    public async Task Get_HasValidStateAbbreviations()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Result.Items.ShouldAllBe(s => s.Abbreviation.Length == 2);
    }

    [Fact(DisplayName = "StateListEndpoint - Should have non-empty names")]
    [Description("PS-XXXX : Validates all states have proper names")]
    public async Task Get_HasNonEmptyStateNames()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Result.Items.ShouldAllBe(s => !string.IsNullOrWhiteSpace(s.Name));
    }

    [Fact(DisplayName = "StateListEndpoint - Should include expected states")]
    [Description("PS-XXXX : Verifies common states are present in the list")]
    public async Task Get_IncludesExpectedStates()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        var abbreviations = response.Result.Items.Select(s => s.Abbreviation).ToList();
        abbreviations.ShouldContain("MA");
        abbreviations.ShouldContain("NH");
    }

    [Fact(DisplayName = "StateListEndpoint - Should not contain duplicates")]
    [Description("PS-XXXX : Verifies distinct state abbreviations")]
    public async Task Get_ShouldNotContainDuplicateStates()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        var abbreviations = response.Result.Items.Select(s => s.Abbreviation).ToList();
        abbreviations.Distinct().Count().ShouldBe(abbreviations.Count);
    }

    [Fact(DisplayName = "StateListEndpoint - Should return HTTP 200 OK")]
    [Description("PS-XXXX : Verifies successful response status code")]
    public async Task Get_ReturnsOkStatusCode()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }

    [Fact(DisplayName = "StateListEndpoint - Should be accessible to ADMINISTRATOR role")]
    [Description("PS-XXXX : Verifies administrators can access state list")]
    public async Task Get_AllowsAccessForAdministrator()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact(DisplayName = "StateListEndpoint - Should have ListResponseDto wrapper")]
    [Description("PS-XXXX : Verifies response is wrapped in ListResponseDto")]
    public async Task Get_ReturnsListResponseDtoWrapper()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Result.ShouldBeOfType<ListResponseDto<StateListResponse>>();
        response.Result.Items.ShouldNotBeNull();
    }

    [Fact(DisplayName = "StateListEndpoint - Should map abbreviations correctly")]
    [Description("PS-XXXX : Verifies state abbreviation to name mapping")]
    public async Task Get_MapsStateAbbreviationsCorrectly()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        var statesByAbbr = response.Result.Items.ToDictionary(s => s.Abbreviation, s => s.Name);

        if (statesByAbbr.ContainsKey("MA"))
        {
            statesByAbbr["MA"].ShouldBe("Massachusetts");
        }

        if (statesByAbbr.ContainsKey("NH"))
        {
            statesByAbbr["NH"].ShouldBe("New Hampshire");
        }

        if (statesByAbbr.ContainsKey("CA"))
        {
            statesByAbbr["CA"].ShouldBe("California");
        }
    }

    [Fact(DisplayName = "StateListEndpoint - Should handle multiple requests")]
    [Description("PS-XXXX : Verifies consistent results across multiple requests")]
    public async Task Get_HandleMultipleRequests()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response1 = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();
        var response2 = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response1.Result.Items.Count.ShouldBe(response2.Result.Items.Count);
        response1.Result.Items.Select(s => s.Abbreviation).OrderBy(a => a)
            .ShouldBe(response2.Result.Items.Select(s => s.Abbreviation).OrderBy(a => a));
    }
}
