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
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Demoulas.ProfitSharing.Common.Contracts.Request;

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


    [Fact(DisplayName = "PS-351: Get current year wages (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {

        var expectedResponse = new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = $"EJR PROF-DOLLAR-EXTRACT YEAR={2023}",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<WagesCurrentYearResponse> { Results = new List<WagesCurrentYearResponse> { } }
        };

        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, ProfitYearRequest, ReportResponseBase<WagesCurrentYearResponse>>(new ProfitYearRequest{ ProfitYear = 2023});

        // Assert
        response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Result.Response.Results.Should().BeAssignableTo<List<WagesCurrentYearResponse>>();
        response.Result.Response.Results.Should().HaveCountGreaterThan(0);

    }

    [Fact(DisplayName = "PS-351: Get current year wages (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        // Act
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await DownloadClient.GETAsync<CurrentYearWagesEndpoint, ProfitYearRequest, StreamContent>(new ProfitYearRequest { ProfitYear = 2023 });
        response.Response.Content.Should().NotBeNull();

        string result = await response.Response.Content.ReadAsStringAsync();
        result.Should().NotBeNullOrEmpty();

        // Assert CSV format
        using var reader = new StringReader(result);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Read the first two rows (date and report name)
        await csv.ReadAsync();  // First row is the date
        string? dateLine = csv.GetField(0);
        dateLine.Should().NotBeNullOrEmpty();

        await csv.ReadAsync();  // Second row is the report name
        string? reportNameLine = csv.GetField(0);
        reportNameLine.Should().NotBeNullOrEmpty();

        // Start reading the actual CSV content from row 2 (0-based index)
        await csv.ReadAsync();  // Read the header row (starting at column 2)
        csv.ReadHeader();

        // Validate the headers
        var headers = csv.HeaderRecord;
        headers.Should().NotBeNull();
        headers.Should().ContainInOrder("", "", "BADGE", "HOURS YR", "DOLLARS YR");

    }

    [Fact(DisplayName = "PS-351: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {


        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, PaginationRequestDto, ReportResponseBase<WagesCurrentYearResponse>>(new PaginationRequestDto());

        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

    }

    [Fact(DisplayName = "PS-351: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.Should().Be("EJR PROF-DOLLAR-EXTRACT YEAR=THIS");
    }
}
