using System.Data.SqlTypes;
using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using FluentAssertions;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Services.Reports;
using JetBrains.Annotations;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.Util.Extensions;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(MilitaryAndRehireProfitSummaryEndpoint))]
public class MilitaryAndRehireProfitSummaryTests : ApiTestBase<Api.Program>
{
    private readonly MilitaryAndRehireProfitSummaryEndpoint _endpoint;

    public MilitaryAndRehireProfitSummaryTests()
    {
        MilitaryAndRehireService mockService = new MilitaryAndRehireService(MockDbContextFactory, new CalendarService(MockDbContextFactory));
        _endpoint = new MilitaryAndRehireProfitSummaryEndpoint(mockService);
    }

    
    [Fact(DisplayName = "PS-346: Check for Military (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>
            {
                ReportName = "MILITARY TERM-REHIRE",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<MilitaryAndRehireProfitSummaryResponse>
                {
                    Results = new List<MilitaryAndRehireProfitSummaryResponse> { setup.ExpectedResponse }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.GETAsync<MilitaryAndRehireProfitSummaryEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>>(setup.Request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Should().HaveCountGreaterThan(expectedResponse.Response.Results.Count());
        });
    }

    [Fact(DisplayName = "PS-346: Check for Military (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<MilitaryAndRehireProfitSummaryEndpoint, PaginationRequestDto, StreamContent>(setup.Request);
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
            headers.Should().ContainInOrder("", "", "BADGE", "SSN", "NAME", "STR", "HIRE DT", "REHIRE DT", "TERM DT", "STATUS", "BEG BAL", "BEG VEST", "CUR HRS", "PLAN YEARS", "ENROLL", "YEAR", "CMNT", "FORT AMT");
        });
    }




    [Fact(DisplayName = "PS-346: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var response =
                await ApiClient.GETAsync<MilitaryAndRehireProfitSummaryEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>>(setup.Request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-346: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = (short)DateTime.Today.Year };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>
        {
            ReportName = "MILITARY TERM-REHIRE",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireProfitSummaryResponse> { Results = new List<MilitaryAndRehireProfitSummaryResponse>() }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-346: Null Results")]
    public async Task GetResponse_Should_HandleNullResults()
    {
        // Arrange
        var request = new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = (short)DateTime.Today.Year };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>
        {
            ReportName = "MILITARY TERM-REHIRE",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireProfitSummaryResponse> { Results = [] }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-346: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.Should().Be("MILITARY TERM-REHIRE");
    }

    private static async Task<(ProfitYearRequest Request, MilitaryAndRehireProfitSummaryResponse ExpectedResponse)> SetupTestEmployee(ProfitSharingDbContext c)
    {
        // Setup
        MilitaryAndRehireProfitSummaryResponse example = MilitaryAndRehireProfitSummaryResponse.ResponseExample();

        var demo = await c.Demographics.FirstAsync();
        demo.EmploymentStatusId = EmploymentStatus.Constants.Active;
        demo.ReHireDate = DateTime.Today.ToDateOnly();
        

        var payProfit = await c.PayProfits.FirstAsync(pp => pp.Ssn == demo.Ssn);
        payProfit.EnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
        payProfit.CompanyContributionYears = 3;
        payProfit.HoursCurrentYear = 2358;

        var details = await c.ProfitDetails.Where(pd => pd.Ssn == demo.Ssn).ToListAsync();
        foreach (var detail in details)
        {
            detail.Forfeiture = short.MaxValue;
            detail.ProfitYear = 2021;
            detail.Remark = "Test remarks";
            detail.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
        }

        await c.SaveChangesAsync();

        example.BadgeNumber = demo.BadgeNumber;
        example.Ssn = demo.Ssn.MaskSsn();
        example.FullName = demo.FullName;
        example.CompanyContributionYears = payProfit.CompanyContributionYears;
        example.HoursCurrentYear = payProfit.HoursCurrentYear ?? 0;
        example.ReHiredDate = demo.ReHireDate ?? SqlDateTime.MinValue.Value.ToDateOnly();


        return (new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = (short)demo.ReHireDate!.Value.Year}, example);
    }
}
