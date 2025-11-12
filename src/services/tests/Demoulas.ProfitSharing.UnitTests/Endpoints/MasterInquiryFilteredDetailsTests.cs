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

[CollectionDefinition("MasterInquiryFilteredDetails", DisableParallelization = true)]
public class MasterInquiryFilteredDetailsTests : ApiTestBase<Api.Program>
{
    public MasterInquiryFilteredDetailsTests()
    {
    }

    /// <summary>
    ///   This endpoint is computationally expensive with large mock datasets.
    ///   Increase timeout to allow for legitimate slowness.
    /// </summary>
    protected override TimeSpan? GetHttpClientTimeout() => TimeSpan.FromMinutes(5);

    [Fact(DisplayName = "Master Inquiry Filtered Details - Basic Employee Request")]
    public async Task GetFilteredDetailsForEmployees()
    {
        var overallTimer = Stopwatch.StartNew();
        _output.WriteLine("==== TEST START (GetFilteredDetailsForEmployees) ====");

        var tokenTimer = Stopwatch.StartNew();
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        tokenTimer.Stop();
        _output.WriteLine($"[TOKEN] Created token: {tokenTimer.ElapsedMilliseconds}ms");

        var request = new MasterInquiryMemberDetailsRequest { MemberType = 1, Skip = 0, Take = 25 };

        var queryTimer = Stopwatch.StartNew();
        var response = await ApiClient.POSTAsync<MasterInquiryFilteredDetailsEndpoint, MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>(request);
        queryTimer.Stop();
        _output.WriteLine($"[QUERY] POST request: {queryTimer.ElapsedMilliseconds}ms");

        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();

        overallTimer.Stop();
        _output.WriteLine($"[TOTAL] Test duration: {overallTimer.ElapsedMilliseconds}ms ({overallTimer.Elapsed.TotalMinutes:F2} minutes)");
        _output.WriteLine("==== TEST END ====");
    }

    [Fact(DisplayName = "Master Inquiry Filtered Details - Filtered By Year And Month")]
    public async Task GetFilteredDetailsByYearAndMonth()
    {
        var overallTimer = Stopwatch.StartNew();
        _output.WriteLine("==== TEST START (GetFilteredDetailsByYearAndMonth) ====");

        var tokenTimer = Stopwatch.StartNew();
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR, Role.EXECUTIVEADMIN);
        tokenTimer.Stop();
        _output.WriteLine($"[TOKEN] Created token: {tokenTimer.ElapsedMilliseconds}ms");

        var request = new MasterInquiryMemberDetailsRequest { MemberType = 1, ProfitYear = 2024, MonthToDate = 3, Skip = 0, Take = 25 };

        var queryTimer = Stopwatch.StartNew();
        var response = await ApiClient.POSTAsync<MasterInquiryFilteredDetailsEndpoint, MasterInquiryMemberDetailsRequest, PaginatedResponseDto<MasterInquiryResponseDto>>(request);
        queryTimer.Stop();
        _output.WriteLine($"[QUERY] POST request: {queryTimer.ElapsedMilliseconds}ms");

        response.ShouldNotBeNull();
        response.Response.EnsureSuccessStatusCode();

        overallTimer.Stop();
        _output.WriteLine($"[TOTAL] Test duration: {overallTimer.ElapsedMilliseconds}ms ({overallTimer.Elapsed.TotalMinutes:F2} minutes)");
        _output.WriteLine("==== TEST END ====");
    }
}
