using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using Demoulas.ProfitSharing.Common.Contracts.Response.MasterInquiry;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Master;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Endpoints;

/// <summary>
/// Collection definition for MasterInquiryFilteredDetails tests - disables parallelization
/// </summary>
[CollectionDefinition("MasterInquiryFilteredDetails", DisableParallelization = true)]
public class MasterInquiryFilteredDetailsCollection
{
    // This class defines the test collection but contains no tests
}

[Collection("MasterInquiryFilteredDetails")]
public class MasterInquiryFilteredDetailsTests : ApiTestBase<Api.Program>
{
    public MasterInquiryFilteredDetailsTests()
    {
    }

    [Fact(DisplayName = "Master Inquiry Filtered Details - Basic Employee Request", Skip = "Performance: Slow test due to correlated subqueries with mock data (2-3 min execution time)")]
    public async Task GetFilteredDetailsForEmployees()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);

        var request = new MasterInquiryMemberDetailsRequest { MemberType = 1, Skip = 0, Take = 25 };

        var response = await ApiClient.POSTAsync<MasterInquiryFilteredDetailsEndpoint, MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>(request);

        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }

    [Fact(DisplayName = "Master Inquiry Filtered Details - Filtered By Year And Month", Skip = "Performance: Slow test due to correlated subqueries with mock data (2-3 min execution time)")]
    public async Task GetFilteredDetailsByYearAndMonth()
    {
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);

        var request = new MasterInquiryMemberDetailsRequest { MemberType = 1, ProfitYear = 2024, MonthToDate = 3, Skip = 0, Take = 25 };

        var response = await ApiClient.POSTAsync<MasterInquiryFilteredDetailsEndpoint, MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>(request);

        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();
    }
}
