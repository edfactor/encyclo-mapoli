

using System.Globalization;
using System.Net;
using CsvHelper.Configuration;
using CsvHelper;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Reports;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class ExecutiveHoursAndDollars : ApiTestBase<Api.Program>
{
    private readonly ExecutiveHoursAndDollarsEndpoint _endpoint;
    private const int ProfitShareTestYear = 1975; // used to avoid pre-canned data.
    private readonly string _expectedReportName = $"Executive Hours and Dollars for Year {ProfitShareTestYear}";
    private static readonly ProfitYearRequest _request = new () { ProfitYear = ProfitShareTestYear, Skip = 0, Take = 10 };
    private static readonly ExecutiveHoursAndDollarsResponse _example = ExecutiveHoursAndDollarsResponse.ResponseExample();


    public ExecutiveHoursAndDollars()
    {
        ExecutiveHoursAndDollarsService mockService = new ExecutiveHoursAndDollarsService(MockDbContextFactory);
        _endpoint = new ExecutiveHoursAndDollarsEndpoint(mockService);
    }


    [Fact(DisplayName = "PS-360: Executive Hours and Dollars (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
            {
                ReportName = _expectedReportName,
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<ExecutiveHoursAndDollarsResponse>
                {
                    Results = new List<ExecutiveHoursAndDollarsResponse> { _example }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.GETAsync<ExecutiveHoursAndDollarsEndpoint, ProfitYearRequest, ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(_request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Count().Should().Be(1);
            response.Result.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
        });
    }

    [Fact(DisplayName = "PS-360: Executive Hours and Dollars (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<ExecutiveHoursAndDollarsEndpoint, ProfitYearRequest, StreamContent>(_request);
            response.Response.Content.Should().NotBeNull();

            string csvData = await response.Response.Content.ReadAsStringAsync();

            // Verify CSV file
            string[] lines = csvData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            lines[1].Should().Be(_expectedReportName);
            lines[2].Should().Be("BADGE,NAME,STR,EXEC HRS,EXEC DOLS,ORA HRS CUR,ORA DOLS CUR,FREQ,STATUS");
            lines[3].Should().Be(@"1,""John, Null E"",2,3,4,5,6,2,a");
            lines[4].Should().Be("");

        });
    }

    [Fact(DisplayName = "PS-360: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            await SetupTestEmployee(c);

            var response =
                await ApiClient.GETAsync<ExecutiveHoursAndDollarsEndpoint, PaginationRequestDto, ReportResponseBase<ExecutiveHoursAndDollarsResponse>>(_request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-360: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var expectedResponse = new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
        {
            ReportName = _expectedReportName,
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<ExecutiveHoursAndDollarsResponse> { Results = new List<ExecutiveHoursAndDollarsResponse>() }
        };

        // Act
        var response = await _endpoint.GetResponse(_request, CancellationToken.None);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-360: Null Results")]
    public async Task GetResponse_Should_HandleNullResults()
    {
        // Arrange
        var expectedResponse = new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
        {
            ReportName = _expectedReportName,
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<ExecutiveHoursAndDollarsResponse> { Results = [] }
        };

        // Act
        var response = await _endpoint.GetResponse(_request, CancellationToken.None);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-360: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        var reportFileName = _endpoint.ReportFileName;
        reportFileName.Should().Be("Executive Hours and Dollars");
    }

    private static async Task SetupTestEmployee(ProfitSharingDbContext c)
    {
        var demo = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync();
        var pp = await c.PayProfits.FirstAsync(pp => pp.OracleHcmId == demo.OracleHcmId);

        demo.BadgeNumber = _example.BadgeNumber;
        demo.ContactInfo.FullName = _example.FullName;
        demo.StoreNumber = _example.StoreNumber;
        pp.IncomeExecutive = _example.IncomeExecutive;
        pp.HoursExecutive = _example.HoursExecutive;
        pp.CurrentIncomeYear = _example.CurrentIncomeYear;
        pp.CurrentHoursYear = _example.CurrentHoursYear;
        demo.PayFrequencyId = _example.PayFrequencyId;
        demo.EmploymentStatusId = _example.EmploymentStatusId;
        pp.ProfitYear = 1975;
        pp.Demographic = demo;

        await c.SaveChangesAsync();
    }
}
