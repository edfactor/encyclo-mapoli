using System.ComponentModel;
using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

[Collection("Lookup Tests")]
public class StateTaxEndpointTests : ApiTestBase<Api.Program>
{

    [Fact(DisplayName = "StateTax - Should handle case insensitive state lookup")]
    [Description("PS-#### : Returns tax rate regardless of case in state abbreviation")]
    public async Task Get_ReturnsStateTaxRate_WhenLowercaseStateProvided()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var request = new StateTaxLookupRequest { State = "nh" };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.ShouldNotBeNull();
        response.Result.State.ShouldBe("NH");
        response.Result.StateTaxRate.ShouldBe(0.00m);
    }

    [Fact(DisplayName = "StateTax - Should return different rates for different states")]
    [Description("PS-#### : Returns correct tax rates for multiple states")]
    public async Task Get_ReturnsCorrectRate_ForDifferentStates()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act - California
        var caResponse = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(
            new StateTaxLookupRequest { State = "CA" });

        // Act - Texas
        var txResponse = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(
            new StateTaxLookupRequest { State = "TX" });

        // Assert
        caResponse.Result.State.ShouldBe("CA");
        caResponse.Result.StateTaxRate.ShouldBe(13.3m);

        txResponse.Result.State.ShouldBe("TX");
        txResponse.Result.StateTaxRate.ShouldBe(0m);
    }

    [Fact(DisplayName = "StateTax - Should return error for non-existent state")]
    [Description("PS-#### : Returns error when state is not found")]
    public async Task Get_ReturnsError_WhenStateNotFound()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new StateTaxLookupRequest { State = "ZZ" }; // Non-existent state

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeFalse();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
    }

    [Fact(DisplayName = "StateTax - Should return error for empty state")]
    [Description("PS-#### : Returns not found when state parameter is empty (route binding fails)")]
    public async Task Get_ReturnsError_WhenStateIsEmpty()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new StateTaxLookupRequest { State = "" };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeFalse();
        // Empty state causes route binding to fail, returning 404 Not Found
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "StateTax - Should return error for whitespace state")]
    [Description("PS-#### : Returns not found when state parameter is whitespace (route binding fails)")]
    public async Task Get_ReturnsError_WhenStateIsWhitespace()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new StateTaxLookupRequest { State = "   " };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeFalse();
        // Whitespace state causes route binding to fail, returning 404 Not Found
        response.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "StateTax - Should work with DISTRIBUTIONSCLERK role")]
    [Description("PS-#### : Allows access with distributions clerk role")]
    public async Task Get_ReturnsSuccess_WithDistributionsClerkRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.DISTRIBUTIONSCLERK);

        var request = new StateTaxLookupRequest { State = "NY" };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.State.ShouldBe("NY");
        response.Result.StateTaxRate.ShouldBe(8.82m);
    }

    [Fact(DisplayName = "StateTax - Should work with FINANCEMANAGER role")]
    [Description("PS-#### : Allows access with finance manager role")]
    public async Task Get_ReturnsSuccess_WithFinanceManagerRole()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        var request = new StateTaxLookupRequest { State = "NY" };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.State.ShouldBe("NY");
        response.Result.StateTaxRate.ShouldBe(8.82m);
    }

    [Theory(DisplayName = "StateTax - Should handle various valid state codes")]
    [Description("PS-#### : Returns tax rates for multiple valid states")]
    [InlineData("NH", 0.00)]
    [InlineData("CA", 13.30)]
    [InlineData("TX", 0.00)]
    [InlineData("NY", 8.82)]
    public async Task Get_ReturnsCorrectRate_ForValidStates(string stateCode, decimal expectedRate)
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var request = new StateTaxLookupRequest { State = stateCode };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.State.ShouldBe(stateCode.ToUpperInvariant());
        response.Result.StateTaxRate.ShouldBe(expectedRate);
    }

    [Fact(DisplayName = "StateTax - Should handle zero tax rate states")]
    [Description("PS-#### : Returns zero tax rate for states without sales tax")]
    public async Task Get_ReturnsZeroRate_ForNoTaxStates()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var request = new StateTaxLookupRequest { State = "NH" };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
        response.Result.State.ShouldBe("NH");
        response.Result.StateTaxRate.ShouldBe(0.00m);
    }

    [Fact(DisplayName = "StateTax - Should return proper response structure")]
    [Description("PS-#### : Returns response with correct structure and data types")]
    public async Task Get_ReturnsCorrectResponseStructure_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var request = new StateTaxLookupRequest { State = "NH" };

        // Act
        var response = await ApiClient.GETAsync<StateTaxEndpoint, StateTaxLookupRequest, StateTaxLookupResponse>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Result.ShouldNotBeNull();
        response.Result.ShouldBeOfType<StateTaxLookupResponse>();
        response.Result.State.ShouldBeOfType<string>();
        response.Result.StateTaxRate.ShouldBeOfType<decimal>();
        response.Result.State.ShouldNotBeNullOrEmpty();
    }
}
