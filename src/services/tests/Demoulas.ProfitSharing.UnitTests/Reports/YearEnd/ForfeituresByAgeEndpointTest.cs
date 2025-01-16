using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using FastEndpoints;
using FluentAssertions;
using JetBrains.Annotations;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(ForfeituresByAgeEndpoint))]
public class ForfeituresByAgeEndpointTest : ApiTestBase<Program>
{
    [Fact]
    public async Task GetResponse_ShouldReturnExpectedResponse()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<ForfeituresByAge> response = await ApiClient
            .GETAsync<ForfeituresByAgeEndpoint, FrozenReportsByAgeRequest, ForfeituresByAge>(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.ReportName.Should().Be("PROFIT SHARING FORFEITURES BY AGE");
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
            .GETAsync<ForfeituresByAgeEndpoint, FrozenReportsByAgeRequest, StreamContent>(request);


        string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        content.Should().Contain("AGE,EMPS,AMOUNT");
        content.Should().Contain("FORF TTL,,");
    }

    [Fact(DisplayName = "PS-502: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        TestResult<ForfeituresByAge> response = await ApiClient
            .GETAsync<ForfeituresByAgeEndpoint, FrozenReportsByAgeRequest, ForfeituresByAge>(request);

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
