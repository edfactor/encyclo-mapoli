using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Elastic.Clients.Elasticsearch;
using FastEndpoints;
using FluentAssertions;
using JetBrains.Annotations;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(DistributionsByAgeEndpoint))]
public class DistributionsByAgeEndpointTest : ApiTestBase<Program>
{
    [Fact]
    public async Task GetResponse_ShouldReturnExpectedResponse()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<DistributionsByAge> response = await ApiClient
            .GETAsync<DistributionsByAgeEndpoint, FrozenReportsByAgeRequest, DistributionsByAge>(request);

        // Assert
        response.Should().NotBeNull();
        response.Result.ReportName.Should().Be("PROFIT SHARING DISTRIBUTIONS BY AGE");
        response.Result.ReportType.Should().Be(request.ReportType);

        var ftCount = response.Result.Response.Results.Sum(c => c.EmployeeCount);
        var hardshipCount = response.Result.Response.Results.Where(c=> c.HardshipAmount > 0).Sum(c => c.EmployeeCount);

        ftCount.Should().Be(response.Result.RegularTotalEmployees);
        hardshipCount.Should().Be(response.Result.HardshipTotalEmployees);
    }

    [Fact]
    public async Task GenerateCsvContent_ShouldWriteCorrectContent()
    {
        // Arrange
        var request = FrozenReportsByAgeRequest.RequestExample();

        // Act
        DownloadClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<StreamContent> response = await DownloadClient
            .GETAsync<DistributionsByAgeEndpoint, FrozenReportsByAgeRequest, StreamContent>(request);


        string content = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        content.Should().Contain("AGE,EMPS,AMOUNT");
        content.Should().Contain("HARDSHIP,");
        content.Should().Contain("DIST TTL,,");
    }

    [Fact(DisplayName = "PS-401: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        var request = new FrozenReportsByAgeRequest { ProfitYear = 2023, ReportType = FrozenReportsByAgeRequest.Report.Total };


        // Act
        TestResult<DistributionsByAge> response = await ApiClient
            .GETAsync<DistributionsByAgeEndpoint, FrozenReportsByAgeRequest, DistributionsByAge>(request);

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
