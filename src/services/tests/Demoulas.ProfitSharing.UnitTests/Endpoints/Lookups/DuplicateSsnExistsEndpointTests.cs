using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using System.Net.Http.Json;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

[Collection("Lookup Tests")]
public class DuplicateSsnExistsEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "Lookup: Duplicate SSN exists - returns success")]
    public async Task Get_ReturnsSuccess_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GetAsync("lookup/duplicate-ssns/exists");

        // Assert
        response.ShouldNotBeNull();
        response.IsSuccessStatusCode.ShouldBeTrue(response.ReasonPhrase);
    }

    [Fact(DisplayName = "Lookup: Duplicate SSN exists - returns boolean payload")]
    public async Task Get_ReturnsBooleanContent_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GetAsync("lookup/duplicate-ssns/exists");
        var value = await response.Content.ReadFromJsonAsync<bool>();

        // Assert
        value.ShouldBeOfType<bool>();
    }
}

