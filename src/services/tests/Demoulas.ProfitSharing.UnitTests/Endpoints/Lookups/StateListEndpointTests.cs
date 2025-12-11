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

[Collection("Lookup Tests")]
public class StateListEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "StateList - Should return all states")]
    [Description("PS-1902 : Returns complete list of available states")]
    public async Task Get_ReturnsAllStates()
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

    [Fact(DisplayName = "StateList - Should include expected states")]
    [Description("PS-1902 : Verifies common states are present in the list")]
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

    [Fact(DisplayName = "StateList - Should order states alphabetically")]
    [Description("PS-1902 : Ensures states are returned in alphabetical order by abbreviation")]
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

    [Fact(DisplayName = "StateList - Should have valid abbreviations")]
    [Description("PS-1902 : Validates all state abbreviations are 2 characters")]
    public async Task Get_HasValidStateAbbreviations()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Result.Items.ShouldAllBe(s => s.Abbreviation.Length == 2);
    }

    [Fact(DisplayName = "StateList - Should have non-empty names")]
    [Description("PS-1902 : Validates all states have proper names")]
    public async Task Get_HasNonEmptyStateNames()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Result.Items.ShouldAllBe(s => !string.IsNullOrWhiteSpace(s.Name));
    }

    [Fact(DisplayName = "StateList - Should be accessible to ADMINISTRATOR role")]
    [Description("PS-1902 : Verifies administrators can access state list")]
    public async Task Get_AllowsAccessForAdministrator()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.Items.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "StateList - Should be accessible to FINANCEMANAGER role")]
    [Description("PS-1902 : Verifies finance managers can access state list")]
    public async Task Get_AllowsAccessForFinanceManager()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.Items.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "StateList - Should be accessible to AUDITOR role")]
    [Description("PS-1902 : Verifies auditors can access state list")]
    public async Task Get_AllowsAccessForAuditor()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);

        // Act
        var response = await ApiClient.GETAsync<StateListEndpoint, ListResponseDto<StateListResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.Items.ShouldNotBeEmpty();
    }
}
