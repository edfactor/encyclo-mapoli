using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints;

public class MasterInquiryFilteredDetailsTests : ApiTestBase<Api.Program>
{
    public MasterInquiryFilteredDetailsTests(ITestOutputHelper testOutputHelper)
    {
    }

    [Fact(DisplayName = "Master Inquiry Filtered Details - Basic Employee Request")]
    public async Task GetFilteredDetailsForEmployees()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        var request = new MasterInquiryMemberDetailsRequest { MemberType = 1, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryFilteredDetailsEndpoint, MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "Master Inquiry Filtered Details - Filtered By Year And Month")]
    public async Task GetFilteredDetailsByYearAndMonth()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        var request = new MasterInquiryMemberDetailsRequest { MemberType = 1, ProfitYear = 2024, MonthToDate = 3, Skip = 0, Take = 25 };
        var response = await ApiClient.GETAsync<MasterInquiryFilteredDetailsEndpoint, MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>(request);
        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }
} 
