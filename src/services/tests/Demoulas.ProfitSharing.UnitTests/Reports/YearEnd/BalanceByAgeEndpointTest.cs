using System.Net;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentAssertions;
using FastEndpoints;
using JetBrains.Annotations;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

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
        response.Should().NotBeNull();
        response.Result.ReportName.Should().Be("PROFIT SHARING BALANCE BY AGE");
        response.Result.ReportType.Should().Be(request.ReportType);
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
        content.Should().Contain("AGE,EMPS,BALANCE,VESTED");
        content.Should().Contain("BEN");
    }

    [Fact(DisplayName = "PS-502: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {

        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        TestResult<BalanceByAge> response = await ApiClient
            .GETAsync<BalanceByAgeEndpoint, FrozenReportsByAgeRequest, BalanceByAge>(request);

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
