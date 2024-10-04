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
using Newtonsoft.Json;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(MilitaryAndRehireForfeituresEndpoint))]
public class MilitaryAndRehireForfeituresTests : ApiTestBase<Api.Program>
{
    private readonly MilitaryAndRehireForfeituresEndpoint _endpoint;

    public MilitaryAndRehireForfeituresTests()
    {
        MilitaryAndRehireService mockService = new MilitaryAndRehireService(MockDbContextFactory, new CalendarService(MockDbContextFactory));
        _endpoint = new MilitaryAndRehireForfeituresEndpoint(mockService);
    }

    
    [Fact(DisplayName = "PS-345: Check for Military (JSON)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
            {
                ReportName = "REHIRE'S PROFIT SHARING DATA",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse>
                {
                    Results = new List<MilitaryAndRehireForfeituresResponse> { setup.ExpectedResponse }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.GETAsync<MilitaryAndRehireForfeituresEndpoint, ProfitYearRequest, ReportResponseBase<MilitaryAndRehireForfeituresResponse>>(setup.Request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Should().HaveCountGreaterOrEqualTo(expectedResponse.Response.Results.Count());

#pragma warning disable S1481
            var expected = System.Text.Json.JsonSerializer.Serialize(expectedResponse.Response.Results);

            var actual = System.Text.Json.JsonSerializer.Serialize(response.Result.Response.Results);
#pragma warning restore S1481

            response.Result.Response.Results.Should().ContainEquivalentOf(expectedResponse.Response.Results);
        });
    }

    [Fact(DisplayName = "PS-345: Check for Military (CSV)")]
    public async Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<MilitaryAndRehireForfeituresEndpoint, ProfitYearRequest, StreamContent>(setup.Request);
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
            headers.Should().ContainInOrder("", "", "BADGE", "EMPLOYEE NAME", "SSN", "REHIRED", "PY-YRS", "YTD HOURS", "EC");

            await csv.ReadAsync();  // Read the header row (starting at column 2)
            csv.ReadHeader();

            // Validate the second row of headers
            var headers2 = csv.HeaderRecord;
            headers2.Should().NotBeNull();
            headers2.Should().ContainInOrder("", "", "", "", "", "YEAR", "FORFEITURES", "COMMENT");
        });
    }




    [Fact(DisplayName = "PS-345: Check to ensure unauthorized")]
    public async Task Unauthorized()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var response =
                await ApiClient.GETAsync<MilitaryAndRehireForfeituresEndpoint, PaginationRequestDto, ReportResponseBase<MilitaryAndRehireForfeituresResponse>>(setup.Request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-345: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = (short)DateTime.Today.Year };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse> { Results = new List<MilitaryAndRehireForfeituresResponse>() }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-345: Null Results")]
    public async Task GetResponse_Should_HandleNullResults()
    {
        // Arrange
        var request = new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = (short)DateTime.Today.Year };
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<MilitaryAndRehireForfeituresResponse> { Results = [] }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
        response.Response.Results.Should().BeEquivalentTo(expectedResponse.Response.Results);
    }

    [Fact(DisplayName = "PS-345: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;

        // Assert
        reportFileName.Should().Be("REHIRE'S PROFIT SHARING DATA");
    }

    private static async Task<(ProfitYearRequest Request, MilitaryAndRehireForfeituresResponse ExpectedResponse)> SetupTestEmployee(ProfitSharingDbContext c)
    {
        // Setup
        MilitaryAndRehireForfeituresResponse example = MilitaryAndRehireForfeituresResponse.ResponseExample();

        var demo = await c.Demographics.FirstAsync();
        demo.EmploymentStatusId = EmploymentStatus.Constants.Active;
        demo.ReHireDate = DateTime.Today.ToDateOnly();

        var profitYear = (short)demo.ReHireDate!.Value.Year;



        var payProfit = await c.PayProfits.FirstAsync(pp => pp.OracleHcmId == demo.OracleHcmId);
        payProfit.EnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
        payProfit.CurrentHoursYear = 2358;
        payProfit.ProfitYear = profitYear;

        var details = await c.ProfitDetails.Where(pd => pd.Ssn == demo.Ssn).ToListAsync();
        foreach (var detail in details)
        {
            detail.Forfeiture = short.MaxValue;
            detail.ProfitYear = profitYear;
            detail.Remark = "Test remarks";
            detail.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
            detail.Contribution = byte.MaxValue;
            detail.Earnings = byte.MaxValue;
        }

        await c.SaveChangesAsync();

        example.BadgeNumber = demo.BadgeNumber;
        example.Ssn = demo.Ssn.MaskSsn();
        example.FullName = demo.FullName;
        example.CompanyContributionYears = 0;
        example.HoursCurrentYear = payProfit.CurrentHoursYear ?? 0;
        example.ReHiredDate = demo.ReHireDate ?? SqlDateTime.MinValue.Value.ToDateOnly();
        example.Details = details.Select(pd => new MilitaryRehireProfitSharingDetailResponse
        {
            Forfeiture = pd.Forfeiture, Remark = pd.Remark, ProfitYear = pd.ProfitYear
        }).ToList();


        return (new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = profitYear }, example);
    }
}
