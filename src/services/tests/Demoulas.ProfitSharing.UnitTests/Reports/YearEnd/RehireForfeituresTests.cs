using System.Globalization;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Demoulas.ProfitSharing.UnitTests.Common.Extensions;
using Demoulas.Util.Extensions;
using FastEndpoints;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[TestSubject(typeof(UnforfeituresEndpoint))]
public class RehireForfeituresTests : ApiTestBase<Program>
{
    private readonly UnforfeituresEndpoint _endpoint;

    public RehireForfeituresTests()
    {
        IUnforfeitService mockService = ServiceProvider?.GetRequiredService<IUnforfeitService>()!;
        IAuditService auditService = ServiceProvider?.GetRequiredService<IAuditService>()!;
        var logger = ServiceProvider?.GetRequiredService<ILogger<UnforfeituresEndpoint>>()!;
        _endpoint = new UnforfeituresEndpoint(mockService, auditService, logger);
    }


    [Fact(DisplayName = "PS-345: Check for Military (JSON)")]
    public Task GetResponse_Should_ReturnReportResponse_WhenCalledWithValidRequest()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var expectedResponse = new ReportResponseBase<UnforfeituresResponse>
            {
                ReportName = "REHIRE'S PROFIT SHARING DATA",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = ReferenceData.DsmMinValue,
                EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
                Response = new PaginatedResponseDto<UnforfeituresResponse>
                {
                    Results = new List<UnforfeituresResponse> { setup.ExpectedResponse }
                }
            };

            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
            var response =
                await ApiClient.POSTAsync<UnforfeituresEndpoint, StartAndEndDateRequest, ReportResponseBase<UnforfeituresResponse>>(
                    setup.Request);

            // Assert
            Assert.Equal(expectedResponse.ReportName, response.Result.ReportName);
            Assert.True(response.Result.Response.Results.Count() >= expectedResponse.Response.Results.Count());
            expectedResponse.Response.Results.First().ShouldBeEquivalentTo(response.Result.Response.Results.First(),
                nameof(UnforfeituresResponse.NetBalanceLastYear),
                nameof(UnforfeituresResponse.VestedBalanceLastYear),
                nameof(UnforfeituresResponse.CompanyContributionYears)
            );
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
            var response = await DownloadClient.POSTAsync<UnforfeituresEndpoint, StartAndEndDateRequest, StreamContent>(setup.Request);
            response.Response.Content.ShouldNotBeNull();

            // CSV assertions
            Assert.NotNull(response.Response.Content);
            string result = await response.Response.Content.ReadAsStringAsync(CancellationToken.None);
            result.ShouldNotBeNullOrEmpty();
            using var reader = new StringReader(result);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            await csv.ReadAsync();
            string? dateLine = csv.GetField(0);
            Assert.False(string.IsNullOrEmpty(dateLine));
            await csv.ReadAsync();
            string? reportNameLine = csv.GetField(0);
            Assert.False(string.IsNullOrEmpty(reportNameLine));
            await csv.ReadAsync();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;
            headers.ShouldNotBeNull();
            headers.ShouldBe(new[] { "", "", "BADGE", "EMPLOYEE NAME", "SSN", "REHIRED", "HIRE DATE", "BEGINNING BALANCE", "BEGIN VESTED AMOUNT", "EC" });

            await csv.ReadAsync();
            csv.ReadHeader();
            var headers2 = csv.HeaderRecord;
            headers2.ShouldNotBeNull();
            headers2.ShouldBe(new[] { "", "", "", "", "", "YEAR", "FORFEITURES", "COMMENT" });
        });
    }


    [Fact(DisplayName = "PS-345: Check to ensure unauthorized")]
    public Task Unauthorized()
    {
        return MockDbContextFactory.UseWritableContext(async c =>
        {
            var setup = await SetupTestEmployee(c);

            var response =
                await ApiClient.POSTAsync<UnforfeituresEndpoint, StartAndEndDateRequest, ReportResponseBase<UnforfeituresResponse>>(setup.Request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.Response.StatusCode);
        });
    }

    [Fact(DisplayName = "PS-345: Empty Results")]
    public async Task GetResponse_Should_HandleEmptyResults()
    {
        // Arrange
        var request = StartAndEndDateRequest.RequestExample();
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<UnforfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = ReferenceData.DsmMinValue,
            EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
            Response = new PaginatedResponseDto<UnforfeituresResponse> { Results = new List<UnforfeituresResponse>() }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.ShouldBe(expectedResponse.ReportName);
        response.Response.Results.ShouldBe(expectedResponse.Response.Results, ignoreOrder: true);
    }

    [Fact(DisplayName = "PS-345: Null Results")]
    public async Task GetResponse_Should_HandleNullResults()
    {
        // Arrange
        var request = StartAndEndDateRequest.RequestExample();
        var cancellationToken = CancellationToken.None;
        var expectedResponse = new ReportResponseBase<UnforfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = ReferenceData.DsmMinValue,
            EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
            Response = new PaginatedResponseDto<UnforfeituresResponse> { Results = [] }
        };

        // Act
        var response = await _endpoint.GetResponse(request, cancellationToken);

        // Assert
        response.ReportName.ShouldBe(expectedResponse.ReportName);
        response.Response.Results.ShouldBe(expectedResponse.Response.Results, ignoreOrder: true);
    }

    [Fact(DisplayName = "PS-345: Report name is correct")]
    public void ReportFileName_Should_ReturnCorrectValue()
    {
        // Act
        var reportFileName = _endpoint.ReportFileName;
        reportFileName.ShouldBe("REHIRE'S PROFIT SHARING DATA");
    }

    private static async Task<(StartAndEndDateRequest Request, UnforfeituresResponse ExpectedResponse)> SetupTestEmployee(ProfitSharingDbContext c)
    {
        // Setup
        UnforfeituresResponse example = UnforfeituresResponse.ResponseExample();

        var demo = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync(CancellationToken.None);
        demo.EmploymentStatusId = EmploymentStatus.Constants.Active;
        demo.EmploymentStatus = new EmploymentStatus { Id = EmploymentStatus.Constants.Active, Name = "Active" };
        demo.ReHireDate = new DateTime(2024, 12, 01, 01, 01, 01, DateTimeKind.Local).ToDateOnly();
        demo.HireDate = new DateTime(2017, 10, 04, 01, 01, 01, DateTimeKind.Local).ToDateOnly();
        demo.TerminationDate = new DateTime(2021, 10, 04, 01, 01, 01, DateTimeKind.Local).ToDateOnly();

        var profitYear = (short)Math.Min(demo.ReHireDate!.Value.Year, 2024);


        var payProfit = await c.PayProfits.Include(p => p.Enrollment).FirstAsync(pp => pp.DemographicId == demo.Id);
        payProfit.EnrollmentId = Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
        // Don't override the Enrollment navigation property - it should match the Enrollments DbSet
        payProfit.CurrentHoursYear = 1255.4m;
        payProfit.HoursExecutive = 0;
        payProfit.CurrentIncomeYear = 12345.67m;
        payProfit.IncomeExecutive = 0;
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
        example.ReHiredDate = demo.ReHireDate ?? ReferenceData.DsmMinValue;
        example.Details = details.Select(pd => new RehireTransactionDetailResponse
        {
            Forfeiture = pd.Forfeiture,
            Remark = pd.Remark,
            ProfitYear = pd.ProfitYear,
            HoursTransactionYear = payProfit.CurrentHoursYear,
            ProfitCodeId = 0
        })
            .ToList();


        return (
            new StartAndEndDateRequest
            {
                Skip = 0,
                Take = 10,
                BeginningDate = example.ReHiredDate.AddDays(-5),
                EndingDate = example.ReHiredDate.AddDays(5)
            }, example);
    }
}
