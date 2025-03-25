using System.Data.SqlTypes;
using System.Globalization;
using System.Net;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.Util.Extensions;
using FastEndpoints;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(RehireForfeituresEndpoint))]
public class MilitaryAndRehireForfeituresTests : ApiTestBase<Program>
{
    private readonly RehireForfeituresEndpoint _endpoint;

    public MilitaryAndRehireForfeituresTests()
    {
        ITerminationAndRehireService mockService = ServiceProvider?.GetRequiredService<ITerminationAndRehireService>()!;
        _endpoint = new RehireForfeituresEndpoint(mockService);
    }


    [Fact(DisplayName = "PS-345: Check for Military (JSON)")]
    public Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<RehireForfeituresResponse>
            {
                ReportName = "REHIRE'S PROFIT SHARING DATA",
                ReportDate = DateTimeOffset.Now,
                Response = new PaginatedResponseDto<RehireForfeituresResponse>
                {
                    Results = new List<RehireForfeituresResponse> { setup.ExpectedResponse }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.GETAsync<RehireForfeituresEndpoint, ProfitYearRequest, ReportResponseBase<RehireForfeituresResponse>>(
                    setup.Request);

            // Assert
            response.Result.ReportName.Should().BeEquivalentTo(expectedResponse.ReportName);
            response.Result.Response.Results.Should().HaveCountGreaterThanOrEqualTo(expectedResponse.Response.Results.Count());

#pragma warning disable S1481
            var expected = JsonSerializer.Serialize(expectedResponse.Response.Results);

            var actual = JsonSerializer.Serialize(response.Result.Response.Results);
#pragma warning restore S1481

            response.Result.Response.Results.First().Should().BeEquivalentTo(expectedResponse.Response.Results.First());
        });
    }

    [Fact(DisplayName = "PS-345: Check for Military (CSV)")]
    public Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest_CSV()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response = await DownloadClient.GETAsync<RehireForfeituresEndpoint, ProfitYearRequest, StreamContent>(setup.Request);
            response.Response.Content.Should().NotBeNull();

            string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            result.Should().NotBeNullOrEmpty();

            // Assert CSV format
            using var reader = new StringReader(result);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            // Read the first two rows (date and report name)
            await csv.ReadAsync(); // First row is the date
            string? dateLine = csv.GetField(0);
            dateLine.Should().NotBeNullOrEmpty();

            await csv.ReadAsync(); // Second row is the report name
            string? reportNameLine = csv.GetField(0);
            reportNameLine.Should().NotBeNullOrEmpty();

            // Start reading the actual CSV content from row 2 (0-based index)
            await csv.ReadAsync(); // Read the header row (starting at column 2)
            csv.ReadHeader();

            // Validate the headers
            var headers = csv.HeaderRecord;
            headers.Should().NotBeNull();
            headers.Should().ContainInOrder("", "", "BADGE", "EMPLOYEE NAME", "SSN", "REHIRED", "PY-YRS", "YTD HOURS", "EC");

            await csv.ReadAsync(); // Read the header row (starting at column 2)
            csv.ReadHeader();

            // Validate the second row of headers
            var headers2 = csv.HeaderRecord;
            headers2.Should().NotBeNull();
            headers2.Should().ContainInOrder("", "", "", "", "", "YEAR", "FORFEITURES", "COMMENT");
        });
    }


    [Fact(DisplayName = "PS-345: Check to ensure unauthorized")]
    public Task Unauthorized()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var response =
                await ApiClient.GETAsync<RehireForfeituresEndpoint, PaginationRequestDto, ReportResponseBase<RehireForfeituresResponse>>(setup.Request);

            response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });
    }

    [Fact(DisplayName = "PS-345: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = RehireForfeituresRequest.RequestExample();
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<RehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<RehireForfeituresResponse> { Results = new List<RehireForfeituresResponse>() }
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
        var request = RehireForfeituresRequest.RequestExample();
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<RehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<RehireForfeituresResponse> { Results = [] }
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

    private static async Task<(ProfitYearRequest Request, RehireForfeituresResponse ExpectedResponse)> SetupTestEmployee(ProfitSharingDbContext c)
    {
        // Setup
        RehireForfeituresResponse example = RehireForfeituresResponse.ResponseExample();

        var demo = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync(CancellationToken.None);
        demo.EmploymentStatusId = EmploymentStatus.Constants.Active;
        demo.ReHireDate = new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local).ToDateOnly();

        var profitYear = (short)Math.Min(demo.ReHireDate!.Value.Year, 2024);


        var payProfit = await c.PayProfits.FirstAsync(pp => pp.DemographicId == demo.Id);
        payProfit.EnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
        payProfit.CurrentHoursYear = 2358;
        payProfit.ProfitYear = profitYear;

        var details = await c.ProfitDetails.Where(pd => pd.Ssn == demo.Ssn).ToListAsync(CancellationToken.None);
        foreach (var detail in details)
        {
            detail.Forfeiture = short.MaxValue;
            detail.ProfitYear = profitYear;
            detail.Remark = "Test remarks";
            detail.ProfitCodeId = ProfitCode.Constants.OutgoingForfeitures.Id;
            detail.Contribution = byte.MaxValue;
            detail.Earnings = byte.MaxValue;
        }

        await c.SaveChangesAsync(CancellationToken.None);

        example.BadgeNumber = demo.BadgeNumber;
        example.Ssn = demo.Ssn.MaskSsn();
        example.FullName = demo.ContactInfo.FullName;
        example.CompanyContributionYears = 0;
        example.HoursCurrentYear = payProfit.CurrentHoursYear;
        example.ReHiredDate = demo.ReHireDate ?? SqlDateTime.MinValue.Value.ToDateOnly();
        example.Details = details.Select(pd => new MilitaryRehireProfitSharingDetailResponse { Forfeiture = pd.Forfeiture, Remark = pd.Remark, ProfitYear = pd.ProfitYear })
            .ToList();


        return (new ProfitYearRequest { Skip = 0, Take = 10, ProfitYear = profitYear }, example);
    }
}
