using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using JetBrains.Annotations;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(BalanceByAgeEndpoint))]
public class BalanceByAgeEndpointTest : ApiTestBase<Program>
{

    [Fact]
    public async Task GetResponse_ShouldReturnExpectedResponse()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<BalanceByAge> response = await ApiClient
            .GETAsync<BalanceByAgeEndpoint, FrozenReportsByAgeRequest, BalanceByAge>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Result.ReportName.ShouldBe("PROFIT SHARING BALANCE BY AGE");
        response.Result.ReportType.ShouldBe(request.ReportType);
    }

    [Fact]
    public async Task GenerateCsvContent_ShouldWriteCorrectContent()
    {
        // Arrange
        var request = FrozenReportsByAgeRequest.RequestExample();

        // Act
        DownloadClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<StreamContent> response = await DownloadClient
            .GETAsync<BalanceByAgeEndpoint, FrozenReportsByAgeRequest, StreamContent>(request);


        string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        content.ShouldContain("AGE,EMPS,BALANCE,VESTED");
        content.ShouldContain("BEN");
    }

    [Fact(DisplayName = "PS-502: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {

        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        TestResult<BalanceByAge> response = await ApiClient
            .GETAsync<BalanceByAgeEndpoint, FrozenReportsByAgeRequest, BalanceByAge>(request);

        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
