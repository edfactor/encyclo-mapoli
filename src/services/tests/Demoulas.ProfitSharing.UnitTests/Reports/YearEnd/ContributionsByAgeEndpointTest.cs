using System.Net;
using Demoulas.ProfitSharing.Api;
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

[TestSubject(typeof(ContributionsByAgeEndpoint))]
public class ContributionsByAgeEndpointTest : ApiTestBase<Program>
{
    [Fact]
    public async Task GetResponse_ShouldReturnExpectedResponse()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<ContributionsByAge> response = await ApiClient
            .GETAsync<ContributionsByAgeEndpoint, FrozenReportsByAgeRequest, ContributionsByAge>(request);

        // Assert
        response.ShouldNotBeNull();
        response.Result.ReportName.ShouldBe("PROFIT SHARING CONTRIBUTIONS BY AGE");
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
            .GETAsync<ContributionsByAgeEndpoint, FrozenReportsByAgeRequest, StreamContent>(request);


        string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        content.ShouldContain("AGE,EMPS,AMOUNT");
        content.ShouldContain("CONT TTL,,");
    }

    [Fact(DisplayName = "PS-502: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        TestResult<ContributionsByAge> response = await ApiClient
            .GETAsync<ContributionsByAgeEndpoint, FrozenReportsByAgeRequest, ContributionsByAge>(request);

        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
