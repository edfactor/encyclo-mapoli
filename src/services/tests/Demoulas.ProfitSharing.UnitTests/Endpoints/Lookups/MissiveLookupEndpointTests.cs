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

public class MissiveLookupEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "Missive - Should return success with ADMINISTRATOR role")]
    [Description("PS-#### : Returns list of missives for administrator")]
    public async Task Get_ReturnsSuccess_WithAdministratorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Missive - Should return list with items")]
    [Description("PS-#### : Returns non-empty list of missives")]
    public async Task Get_ReturnsListWithItems_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        response.Result.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeEmpty();
        response.Result.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "Missive - Should return items with required properties")]
    [Description("PS-#### : Each missive has Id, Message, Description, and Severity")]
    public async Task Get_ReturnsItemsWithRequiredProperties_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        var firstItem = response.Result!.Items[0];
        firstItem.Id.ShouldBeGreaterThan(0);
        firstItem.Message.ShouldNotBeNullOrWhiteSpace();
        firstItem.Description.ShouldNotBeNullOrWhiteSpace();
        firstItem.Severity.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "Missive - Should return items ordered by message")]
    [Description("PS-#### : Missives are sorted alphabetically by message")]
    public async Task Get_ReturnsItemsOrderedByMessage_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        var items = response.Result!.Items;
        var sortedMessages = items.Select(x => x.Message).OrderBy(x => x).ToList();
        var actualMessages = items.Select(x => x.Message).ToList();

        actualMessages.ShouldBe(sortedMessages);
    }

    [Fact(DisplayName = "Missive - Should work with FINANCEMANAGER role")]
    [Description("PS-#### : Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Missive - Should work with AUDITOR role")]
    [Description("PS-#### : Allows access with auditor role")]
    public async Task Get_ReturnsSuccess_WithAuditorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Missive - Count should match Items length")]
    [Description("PS-#### : Count property equals number of items in list")]
    public async Task Get_CountMatchesItemsLength_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        response.Result!.Count.ShouldBe(response.Result.Items.Count);
    }

    [Fact(DisplayName = "Missive - Should return unique IDs")]
    [Description("PS-#### : Each missive has a unique ID")]
    public async Task Get_ReturnsUniqueIds_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        var ids = response.Result!.Items.Select(x => x.Id).ToList();
        var uniqueIds = ids.Distinct().ToList();

        uniqueIds.Count.ShouldBe(ids.Count);
    }

    [Fact(DisplayName = "Missive - Should contain valid severity values")]
    [Description("PS-#### : Severity should be Error, Warning, or Info")]
    public async Task Get_ContainsValidSeverityValues_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var validSeverities = new[] { "Error", "Warning", "Info", "Information" };

        // Act
        var response = await ApiClient.GETAsync<MissiveLookupEndpoint, ListResponseDto<MissiveResponse>>();

        // Assert
        var allSeverities = response.Result!.Items.Select(x => x.Severity).Distinct().ToList();

        foreach (var severity in allSeverities)
        {
            validSeverities.ShouldContain(severity, $"Unexpected severity value: {severity}");
        }
    }
}
