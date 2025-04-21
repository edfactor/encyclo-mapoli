using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.Naviations;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Navigations;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Navigations;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints;

public class NavigationServiceTests : ApiTestBase<Program>
{
    [Fact(DisplayName = "PS-1009: Navigation")]
    public async Task GetNavigations()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        var request = new NavigationRequestDto() {  };
        var response = await ApiClient.GETAsync<GetNavigationEndpoint, NavigationRequestDto, NavigationResponseDto>(request);
        response.Should().NotBeNull();
    }


}
