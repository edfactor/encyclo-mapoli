using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using FluentAssertions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Services.Reports;
using JetBrains.Annotations;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(CurrentYearWagesEndpoint))]
public class CurrentYearWageReportTests : ApiTestBase<Api.Program>
{
    private readonly CurrentYearWagesEndpoint _endpoint;

    public CurrentYearWageReportTests()
    {
        WagesService mockService = new WagesService(MockDbContextFactory);
        _endpoint = new CurrentYearWagesEndpoint(mockService);
    }


    [Fact(DisplayName = "PS-312: Get current year wages (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {

        var expectedResponse = new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = "EJR PROF-DOLLAR-EXTRACT YEAR=THIS",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<WagesCurrentYearResponse> { Results = new List<WagesCurrentYearResponse> { } }
        };

        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, PaginationRequestDto, ReportResponseBase<WagesCurrentYearResponse>>(new PaginationRequestDto());

        // Assert
        response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Result.Response.Results.Should().BeAssignableTo<List<WagesCurrentYearResponse>>();
        response.Result.Response.Results.Should().HaveCountGreaterThan(0);

    }

    [Fact(DisplayName = "PS-312: Get current year wages (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        // Act
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await DownloadClient.GETAsync<CurrentYearWagesEndpoint, PaginationRequestDto, StreamContent>(new PaginationRequestDto());
        response.Response.Content.Should().NotBeNull();

        string result = await response.Response.Content.ReadAsStringAsync();
        result.Should().NotBeNullOrEmpty();

    }

    [Fact(DisplayName = "PS-312: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {


        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, PaginationRequestDto, ReportResponseBase<WagesCurrentYearResponse>>(new PaginationRequestDto());

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

    }

    [Fact(DisplayName = "PS-312: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.Should().Be("EJR PROF-DOLLAR-EXTRACT YEAR=THIS");
    }
}
