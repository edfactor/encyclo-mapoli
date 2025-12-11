using System.Globalization;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.Headers;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.Util.Extensions;
using FastEndpoints;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(CurrentYearWagesEndpoint))]
public class CurrentYearWageReportTests : ApiTestBase<Api.Program>
{
    private readonly CurrentYearWagesEndpoint _endpoint;

    public CurrentYearWageReportTests()
    {
        var calendarService = ServiceProvider!.GetRequiredService<ICalendarService>();
        var logger = ServiceProvider!.GetRequiredService<ILogger<CurrentYearWagesEndpoint>>();

        // Mock IDemographicReaderService to return live demographics
        var mockDemographicReader = new Mock<IDemographicReaderService>();
        mockDemographicReader
            .Setup(d => d.BuildDemographicQuery(It.IsAny<IProfitSharingDbContext>(), It.IsAny<bool>()))
            .ReturnsAsync((IProfitSharingDbContext ctx, bool useFrozen) => ctx.Demographics);

        WagesService mockService = new WagesService(MockDbContextFactory, calendarService, mockDemographicReader.Object);
        _endpoint = new CurrentYearWagesEndpoint(mockService, logger);
    }


    [Fact(DisplayName = "PS-351: Get current year wages (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {

        var expectedResponse = new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = $"YTD Wages Extract (PROF-DOLLAR-EXTRACT) - {2023}",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = ReferenceData.DsmMinValue,
            EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
            Response = new PaginatedResponseDto<WagesCurrentYearResponse> { Results = new List<WagesCurrentYearResponse> { } }
        };

        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);
        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, WagesCurrentYearRequest, ReportResponseBase<WagesCurrentYearResponse>>(
                new WagesCurrentYearRequest { ProfitYear = 2023, UseFrozenData = false });

        // Assert
        response.Result.ReportName.ShouldBe(expectedResponse.ReportName);
        response.Result.Response.Results.ShouldBeAssignableTo<List<WagesCurrentYearResponse>>();
        response.Result.Response.Results.Count().ShouldBeGreaterThan(0);

    }

    [Fact(DisplayName = "PS-351: Get current year wages (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        // Act
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        var response = await DownloadClient.GETAsync<CurrentYearWagesEndpoint, WagesCurrentYearRequest, StreamContent>(
            new WagesCurrentYearRequest { ProfitYear = 2023, UseFrozenData = false });
        response.Response.Content.ShouldNotBeNull();

        string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();

        // Assert CSV format
        using var reader = new StringReader(result);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        // Read the first two rows (date and report name)
        await csv.ReadAsync();  // First row is the date
        string? dateLine = csv.GetField(0);
        dateLine.ShouldNotBeNullOrEmpty();

        await csv.ReadAsync();  // Second row is the report name
        string? reportNameLine = csv.GetField(0);
        reportNameLine.ShouldNotBeNullOrEmpty();

        // Start reading the actual CSV content from row 2 (0-based index)
        await csv.ReadAsync();  // Read the header row (starting at column 2)
        csv.ReadHeader();

        // Validate the headers
        var headers = csv.HeaderRecord;
        headers.ShouldNotBeNull();
        headers.ShouldBe(new[] { "", "", "BADGE", "HOURS YR", "DOLLARS YR" });

    }

    [Fact(DisplayName = "PS-351: Get current year wages with frozen data (JSON) - Archive mode")]
    public async Task GetResponse_Should_ReturnArchiveReportResponse_WhenCalledWithFrozenData()
    {
        // Act
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);
        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, WagesCurrentYearRequest, ReportResponseBase<WagesCurrentYearResponse>>(
                new WagesCurrentYearRequest { ProfitYear = 2023, UseFrozenData = true });

        // Assert
        response.Result.ReportName.ShouldContain("Archive");
        response.Result.ReportName.ShouldBe($"YTD Wages Extract (PROF-DOLLAR-EXTRACT) - 2023 - Archive");
        response.Result.Response.Results.ShouldBeAssignableTo<List<WagesCurrentYearResponse>>();
    }

    [Fact(DisplayName = "PS-351: Verify X-Demographic-Data-Source header is set to Frozen for frozen data")]
    public async Task GetResponse_Should_SetFrozenHeader_WhenUsingFrozenData()
    {
        // Arrange
        ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER, Role.EXECUTIVEADMIN);

        // Act
        var response = await ApiClient.GETAsync<CurrentYearWagesEndpoint, WagesCurrentYearRequest, ReportResponseBase<WagesCurrentYearResponse>>(
            new WagesCurrentYearRequest { ProfitYear = 2023, UseFrozenData = true });

        // Assert
        response.Response.Headers.TryGetValues(DemographicHeaders.Source, out var sourceValues).ShouldBeTrue("X-Demographic-Data-Source header should be present");
        sourceValues!.Single().ShouldBe("Frozen", "Header should indicate frozen data when UseFrozenData is true");
    }

    [Fact(DisplayName = "PS-351: Verify frozen data CSV includes Archive in report name")]
    public async Task GetResponse_Should_ReturnArchiveReportName_InCSV_WhenUseFrozenDataIsTrue()
    {
        // Arrange
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await DownloadClient.GETAsync<CurrentYearWagesEndpoint, WagesCurrentYearRequest, StreamContent>(
            new WagesCurrentYearRequest { ProfitYear = 2023, UseFrozenData = true });

        string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();

        // Assert
        using var reader = new StringReader(result);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        await csv.ReadAsync();  // First row is the date
        await csv.ReadAsync();  // Second row is the report name

        string? reportNameLine = csv.GetField(0);
        reportNameLine.ShouldNotBeNull();
        reportNameLine.ShouldContain("Archive"); // CSV report name should include Archive suffix when using frozen data
    }

    [Fact(DisplayName = "PS-351: Verify live data CSV does not include Archive in report name")]
    public async Task GetResponse_Should_ReturnLiveReportName_InCSV_WhenUseFrozenDataIsFalse()
    {
        // Arrange
        DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response = await DownloadClient.GETAsync<CurrentYearWagesEndpoint, WagesCurrentYearRequest, StreamContent>(
            new WagesCurrentYearRequest { ProfitYear = 2023, UseFrozenData = false });

        string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
        result.ShouldNotBeNullOrEmpty();

        // Assert
        using var reader = new StringReader(result);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        await csv.ReadAsync();  // First row is the date
        await csv.ReadAsync();  // Second row is the report name

        string? reportNameLine = csv.GetField(0);
        reportNameLine.ShouldNotBeNull();
        reportNameLine.ShouldNotContain("Archive"); // CSV report name should not include Archive suffix when using live data
    }

    [Fact(DisplayName = "PS-351: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        var response =
            await ApiClient.GETAsync<CurrentYearWagesEndpoint, PaginationRequestDto, ReportResponseBase<WagesCurrentYearResponse>>(new PaginationRequestDto());

        response.Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

    }

    [Fact(DisplayName = "PS-351: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.ShouldBe("YTD Wages Extract (PROF-DOLLAR-EXTRACT)");
    }
}
