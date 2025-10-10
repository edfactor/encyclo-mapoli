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

public class CommentTypeEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "CommentType - Should return success with ADMINISTRATOR role")]
    [Description("PS-#### : Returns list of comment types for administrator")]
    public async Task Get_ReturnsSuccess_WithAdministratorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CommentType - Should return list with items")]
    [Description("PS-#### : Returns non-empty list of comment types")]
    public async Task Get_ReturnsListWithItems_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        response.Result.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeEmpty();
        response.Result.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "CommentType - Should return items with required properties")]
    [Description("PS-#### : Each comment type has Id and Name")]
    public async Task Get_ReturnsItemsWithRequiredProperties_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        var firstItem = response.Result!.Items[0];
        ((int)firstItem.Id).ShouldBeGreaterThan(0);
        firstItem.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "CommentType - Should return items ordered by name")]
    [Description("PS-#### : Comment types are sorted alphabetically by name")]
    public async Task Get_ReturnsItemsOrderedByName_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        var items = response.Result!.Items;
        var sortedNames = items.Select(x => x.Name).OrderBy(x => x).ToList();
        var actualNames = items.Select(x => x.Name).ToList();

        actualNames.ShouldBe(sortedNames);
    }

    [Fact(DisplayName = "CommentType - Should work with FINANCEMANAGER role")]
    [Description("PS-#### : Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CommentType - Should work with AUDITOR role")]
    [Description("PS-#### : Allows access with auditor role")]
    public async Task Get_ReturnsSuccess_WithAuditorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "CommentType - Count should match Items length")]
    [Description("PS-#### : Count property equals number of items in list")]
    public async Task Get_CountMatchesItemsLength_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        response.Result!.Count.ShouldBe(response.Result.Items.Count);
    }

    [Fact(DisplayName = "CommentType - Should return unique IDs")]
    [Description("PS-#### : Each comment type has a unique ID")]
    public async Task Get_ReturnsUniqueIds_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<CommentTypeEndpoint, ListResponseDto<CommentTypeResponse>>();

        // Assert
        var ids = response.Result!.Items.Select(x => x.Id).ToList();
        var uniqueIds = ids.Distinct().ToList();

        uniqueIds.Count.ShouldBe(ids.Count);
    }
}
