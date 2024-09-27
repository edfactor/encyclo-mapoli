using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;
using Demoulas.ProfitSharing.UnitTests.Base;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class TerminatedEmployeeAndBeneficiaryTests : ApiTestBase<Program>
{
    private readonly YearEndClient _yearEndClient;
    
    public TerminatedEmployeeAndBeneficiaryTests()
    {
        _yearEndClient = new YearEndClient(ApiClient, DownloadClient);
    }

    readonly TerminatedEmployeeAndBeneficiaryReportRequestDto requestDto = new()
    {
        StartDate = new DateOnly(2023, 1, 7),
        EndDate = new DateOnly(2024, 1, 2),
        ProfitShareYear = 2023.0m
    };

    [Fact(DisplayName = "Test report with nobody applicable - sanity check")]
    public async Task DownloadTerminatedEmployeeAndBeneficiaryReport()
    {
        // Arrange
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        // Act
        var response =
            await ApiClient
                .GETAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                    TerminatedEmployeeAndBeneficiaryReportRequestDto, TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>> (requestDto);

        // Assert
        response.Response.Content.Should().NotBeNull();
        response.Result.ReportName.Should().BeEquivalentTo("Terminated Employee and Beneficiary Report");

        response.Result.Response.Total.ShouldBeEquivalentTo(0);
        response.Result.Response.Results.Count().ShouldBeEquivalentTo(0);

        response.Result.TotalEndingBalance.ShouldBeEquivalentTo(0m);
        response.Result.TotalVested.ShouldBeEquivalentTo(0m);
        response.Result.TotalForfeit.ShouldBeEquivalentTo(0m);
        response.Result.TotalBeneficiaryAllocation.ShouldBeEquivalentTo(0m);
        
    }


    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Act
        var response =
            await ApiClient
                .GETAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                    TerminatedEmployeeAndBeneficiaryReportRequestDto, TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>>(requestDto);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "Test Single Terminated Employee")]
    public async Task TerminatedEmployee()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            var demo = await c.Demographics.FirstAsync();
            demo.BadgeNumber = 9988;
            demo.TerminationCodeId = TerminationCode.Constants.AnotherJob;
            demo.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
            demo.TerminationDate = new DateOnly(2023, 6, 1);
            demo.FullName = "Smith, Nancy K";
            demo.DateOfBirth = new DateOnly(2000, 1, 1);

            var pp = await c.PayProfits.FirstAsync(pp => pp.Ssn == demo.Ssn);
            pp.BadgeNumber = 9988;
            pp.EnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions;
            pp.NetBalanceLastYear = 446;
            pp.CompanyContributionYears = 10;

            var details = await c.ProfitDetails.Where(pd => pd.Ssn == demo.Ssn).ToListAsync();
            // Here we move these 5 records out of the way.
            foreach (var detail in details)
            {
                detail.ProfitYear = 2021;
            }
            details[0].ProfitCodeId = ProfitCode.Constants.IncomingContributions;
            details[0].ProfitYear = 2023;
            details[0].Forfeiture = 277.91m;

            details[1].ProfitCodeId = ProfitCode.Constants.IncomingQdroBeneficiary;
            details[1].ProfitYear = 2023;
            details[1].Contribution = 222.23m;
            details[1].Earnings = 0m;
            details[1].Forfeiture = 0m;

            details[2].ProfitCodeId = ProfitCode.Constants.OutgoingDirectPayments;
            details[2].ProfitYear = 2023;
            details[2].Forfeiture = 99.99m;

            await c.SaveChangesAsync();

            // Lock age and todays' date computation when testing. 
            TerminatedEmployeeAndBeneficiaryReportService.SetTodayDateForTestingOnly(new DateOnly(2024, 9, 7));

            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            // Act
            var response =
                await ApiClient
                    .GETAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                        TerminatedEmployeeAndBeneficiaryReportRequestDto, TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>>(requestDto);

            // Assert
            response.Response.Content.Should().NotBeNull();
            response.Result.ReportName.Should().BeEquivalentTo("Terminated Employee and Beneficiary Report");

            response.Result.Response.Total.ShouldBeEquivalentTo(1);
            response.Result.Response.Results.Count().ShouldBeEquivalentTo(1);

            response.Result.TotalEndingBalance.ShouldBeEquivalentTo(846.15m);
            response.Result.TotalVested.ShouldBeEquivalentTo(122.24m);
            response.Result.TotalForfeit.ShouldBeEquivalentTo(277.91m);
            response.Result.TotalBeneficiaryAllocation.ShouldBeEquivalentTo(222.23m);

            TerminatedEmployeeAndBeneficiaryDataResponseDto member = response.Result.Response.Results.First();
            member.Should().ShouldBeEquivalentTo(new TerminatedEmployeeAndBeneficiaryDataResponseDto()
            {
                Name = "Smith, Nancy K",
                BadgePSn = "9988",
                BeginningBalance = 446m,
                BeneficiaryAllocation = 222.23m,
                DistributionAmount = -99.99m,
                Forfeit = 846.15m,
                EndingBalance = 846.15m,
                VestedBalance = 122.24m,
                DateTerm = new DateOnly(2023, 6, 1),
                YtdPsHours = 0m,
                VestedPercent = 100,
                Age = 24,
                EnrollmentCode = null
            });
            
        });
    }

    [Fact(DisplayName = "Test Single Beneficiary")]
    public async Task SingleBeneficiary()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            var bene = await c.Beneficiaries.FirstAsync();
            bene.Psn = 888888;
            bene.Amount = 379.44m;
            bene.FirstName = "Rogue";
            bene.LastName = "One";
            bene.MiddleName = "I";
            bene.DateOfBirth = new DateOnly(1990, 1, 1);

            await c.SaveChangesAsync();

            // Lock age and todays' date computation when testing. 
            TerminatedEmployeeAndBeneficiaryReportService.SetTodayDateForTestingOnly(new DateOnly(2024, 9, 7));

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            var response =
                await DownloadClient
                    .GETAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                        TerminatedEmployeeAndBeneficiaryReportRequestDto, TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>>(requestDto);

            // Assert
            response.Response.Content.Should().NotBeNull();
            response.Result.ReportName.Should().BeEquivalentTo("Terminated Employee and Beneficiary Report");

            response.Result.Response.Total.ShouldBeEquivalentTo(1);
            response.Result.Response.Results.Count().ShouldBeEquivalentTo(1);

            response.Result.TotalEndingBalance.ShouldBeEquivalentTo(379.44);
            response.Result.TotalVested.ShouldBeEquivalentTo(379.44);
            response.Result.TotalForfeit.ShouldBeEquivalentTo(0m);
            response.Result.TotalBeneficiaryAllocation.ShouldBeEquivalentTo(0m);

            TerminatedEmployeeAndBeneficiaryDataResponseDto member = response.Result.Response.Results.First();
            member.Should().ShouldBeEquivalentTo(new TerminatedEmployeeAndBeneficiaryDataResponseDto()
            {
                Name = "One, Rogue I",
                BadgePSn = "888888",
                BeginningBalance = 379.44m,
                BeneficiaryAllocation = 0m,
                DistributionAmount = 0m,
                Forfeit = 0m,
                EndingBalance = 379.44m,
                VestedBalance = 379.44m,
                DateTerm = null,
                YtdPsHours = 0m,
                VestedPercent = 100,
                Age = 34,
                EnrollmentCode = null
            });

        });
    }

}
