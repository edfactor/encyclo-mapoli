using Demoulas.ProfitSharing.Endpoints.Endpoints.Lookups;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints.Lookups;

public class DuplicateSsnExistsEndpointTests : ApiTestBase<Api.Program>
{
    [Fact(DisplayName = "Lookup: Duplicate SSN exists - returns success")]
    public async Task Get_ReturnsSuccess_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var response = await ApiClient.GETAsync<DuplicateSsnExistsEndpoint, bool>();

        // Assert
        response.ShouldNotBeNull();
        response.Response.IsSuccessStatusCode.ShouldBeTrue(response.Response.ReasonPhrase);
    }

    [Fact(DisplayName = "Lookup: Duplicate SSN exists - returns boolean payload")]
    public async Task Get_ReturnsBooleanContent_WhenCalled()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        // Act
        var value = await ApiClient.GETAsync<DuplicateSsnExistsEndpoint, bool>();

        // Assert
        value.Result.ShouldBeOfType<bool>();
    }
}

