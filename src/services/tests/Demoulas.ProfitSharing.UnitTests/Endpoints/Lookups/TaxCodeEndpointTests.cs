using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
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
public class TaxCodeEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "TaxCode - Should return success with ADMINISTRATOR role")]
    [Description("Returns list of tax codes for administrator")]
    public async Task Get_ReturnsSuccess_WithAdministratorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "TaxCode - Should return list with items")]
    [Description("Returns non-empty list of tax codes")]
    public async Task Get_ReturnsListWithItems_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        response.Result.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeNull();
        response.Result.Items.ShouldNotBeEmpty();
        response.Result.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "TaxCode - Should return items with required properties")]
    [Description("Each tax code has Id and Name")]
    public async Task Get_ReturnsItemsWithRequiredProperties_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        var firstItem = response.Result!.Items[0];
        firstItem.Id.ShouldNotBe('\0'); // Not default char
        firstItem.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "TaxCode - Should return items ordered by id then name")]
    [Description("Tax codes are sorted by id, then name")]
    public async Task Get_ReturnsItemsOrderedByIdThenName_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        var items = response.Result!.Items;
        var sortedItems = items
            .OrderBy(x => x.Id)
            .ThenBy(x => x.Name)
            .Select(x => (x.Id, x.Name))
            .ToList();
        var actualItems = items.Select(x => (x.Id, x.Name)).ToList();

        actualItems.ShouldBe(sortedItems);
    }

    [Fact(DisplayName = "TaxCode - Should work with FINANCEMANAGER role")]
    [Description("Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "TaxCode - Should work with AUDITOR role")]
    [Description("Allows access with auditor role")]
    public async Task Get_ReturnsSuccess_WithAuditorRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.AUDITOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        response.Response.IsSuccessStatusCode.ShouldBeTrue();
        response.Result.ShouldNotBeNull();
    }

    [Fact(DisplayName = "TaxCode - Count should match Items length")]
    [Description("Count property equals number of items in list")]
    public async Task Get_CountMatchesItemsLength_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        response.Result!.Count.ShouldBe(response.Result.Items.Count);
    }

    [Fact(DisplayName = "TaxCode - Should return unique IDs")]
    [Description("Each tax code has a unique ID")]
    public async Task Get_ReturnsUniqueIds_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        var ids = response.Result!.Items.Select(x => x.Id).ToList();
        var uniqueIds = ids.Distinct().ToList();

        uniqueIds.Count.ShouldBe(ids.Count);
    }

    [Fact(DisplayName = "TaxCode - Should contain expected tax codes")]
    [Description("Response should contain standard tax codes like A, B, M")]
    public async Task Get_ContainsStandardTaxCodes_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<TaxCodeEndpoint, TaxCodeLookupRequest, ListResponseDto<TaxCodeResponse>>(
            new TaxCodeLookupRequest());

        // Assert
        var taxCodeIds = response.Result!.Items.Select(x => x.Id).ToList();

        // Should contain common tax codes
        taxCodeIds.ShouldContain('A'); // Qualifies for 5- or 10-year averaging
        taxCodeIds.ShouldContain('B'); // Qualifies for death benefit exclusion
        taxCodeIds.ShouldContain('7'); // Normal distribution
    }
}
