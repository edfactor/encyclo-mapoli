using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Moq;
using FluentAssertions;
using FastEndpoints;
using JetBrains.Annotations;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Extensions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(DistributionsByAgeEndpoint))]
public class DistributionsByAgeEndpointTest : ApiTestBase<Program>
{
    private readonly Mock<IFrozenReportService> _frozenReportServiceMock;
    private readonly DistributionsByAgeEndpoint _endpoint;

    public DistributionsByAgeEndpointTest()
    {
        _frozenReportServiceMock = new Mock<IFrozenReportService>();
        _endpoint = new DistributionsByAgeEndpoint(_frozenReportServiceMock.Object);
    }

    [Fact]
    public void ReportFileName_ShouldReturnCorrectFileName()
    {
        // Act
        var result = _endpoint.ReportFileName;

        // Assert
        result.Should().Be("PROFIT SHARING DISTRIBUTIONS BY AGE");
    }

    [Fact]
    public async Task GetResponse_ShouldReturnExpectedResponse()
    {
        // Arrange
        var request = new DistributionsByAgeRequest { ProfitYear = 2023, ReportType = DistributionsByAgeRequest.Report.Total };
        var expectedResponse = DistributionsByAge.ResponseExample();
        _frozenReportServiceMock.Setup(x => x.GetDistributionsByAgeYear(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _endpoint.GetResponse(request, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task GenerateCsvContent_ShouldWriteCorrectContent()
    {
        // Arrange
        var request = DistributionsByAgeRequest.RequestExample();

        // Act
        DownloadClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);
        TestResult<StreamContent> response = await DownloadClient
            .GETAsync<DistributionsByAgeEndpoint, DistributionsByAgeRequest, StreamContent>(request);

            
        string content = await response.Response.Content.ReadAsStringAsync();
        content.Should().Contain("AGE,EMPS,AMOUNT");
        content.Should().Contain("30,2,2000");
        content.Should().Contain("HARDSHIP,5,1000");
        content.Should().Contain("DIST TTL,,6000");
    }
}
