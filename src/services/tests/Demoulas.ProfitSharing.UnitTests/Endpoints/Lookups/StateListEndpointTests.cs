using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.UnitTests.Common;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

public class StateListEndpointTests : ApiTestBase<Api.Program>
{
    public StateListEndpointTests(ApiWebApplicationFactory<Api.Program> factory) : base(factory)
    {
    }

    [Fact(DisplayName = "StateList - Should return all states")]
    [Description("PS-1902 : Returns complete list of available states")]
    public async Task Get_ReturnsAllStates()
    {
        // Arrange & Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.ShouldNotBeNull();
        response.Results.ShouldNotBeNull();
        response.Results.ShouldNotBeEmpty();
        response.Results.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "StateList - Should return alphabetically sorted states")]
    [Description("PS-1902 : States should be sorted by abbreviation")]
    public async Task Get_ReturnsStatesSorted()
    {
        // Arrange & Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        var abbreviations = response.Results.Select(s => s.Abbreviation).ToList();
        var sortedAbbreviations = abbreviations.OrderBy(a => a).ToList();
        abbreviations.ShouldBe(sortedAbbreviations);
    }

    [Fact(DisplayName = "StateList - Should include common states")]
    [Description("PS-1902 : Verifies expected states are in the list")]
    public async Task Get_IncludesCommonStates()
    {
        // Arrange
        var expectedStates = new[] { "MA", "NH", "ME", "VT", "RI", "CT", "NY", "FL" };

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        var abbreviations = response.Results.Select(s => s.Abbreviation).ToList();
        foreach (var state in expectedStates)
        {
            abbreviations.ShouldContain(state);
        }
    }

    [Fact(DisplayName = "StateList - Should have valid abbreviations")]
    [Description("PS-1902 : All abbreviations should be 2 characters")]
    public async Task Get_AllAbbreviationsAreTwoCharacters()
    {
        // Arrange & Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Results.ShouldAllBe(s => s.Abbreviation.Length == 2);
    }

    [Fact(DisplayName = "StateList - Should have non-empty names")]
    [Description("PS-1902 : All states should have valid names")]
    public async Task Get_AllStatesHaveNames()
    {
        // Arrange & Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Results.ShouldAllBe(s => !string.IsNullOrWhiteSpace(s.Name));
    }

    [Fact(DisplayName = "StateList - Should work with ADMINISTRATOR role")]
    [Description("PS-1902 : Allows access with administrator role")]
    public async Task Get_ReturnsSuccess_WithAdministratorRole()
    {
        // Arrange
        await LoginAsAdministrator();

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.ShouldNotBeNull();
        response.Results.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "StateList - Should work with FINANCEMANAGER role")]
    [Description("PS-1902 : Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        await LoginAsFinanceManager();

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.ShouldNotBeNull();
        response.Results.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "StateList - Should work with READONLY role")]
    [Description("PS-1902 : Allows access with readonly role")]
    public async Task Get_ReturnsSuccess_WithReadOnlyRole()
    {
        // Arrange
        await LoginAsReadOnly();

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.ShouldNotBeNull();
        response.Results.ShouldNotBeEmpty();
    }
}
