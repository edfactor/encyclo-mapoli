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
        response.ShouldNotBeNull();
        response.Result.ReportName.ShouldBe("PROFIT SHARING FORFEITURES BY AGE");
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
            .GETAsync<ForfeituresByAgeEndpoint, FrozenReportsByAgeRequest, StreamContent>(request);


        string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        content.ShouldContain("AGE,EMPS,AMOUNT");
        content.ShouldContain("FORF TTL,,");
    }

    [Fact]
    public async Task GetResponse_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        ApiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid_token");
        TestResult<ForfeituresByAge> response = await ApiClient
            .GETAsync<ForfeituresByAgeEndpoint, FrozenReportsByAgeRequest, ForfeituresByAge>(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
