using System.Net;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
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

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

public class TerminatedEmployeeAndBeneficiaryTests : ApiTestBase<Program>
{
    private readonly TerminatedEmployeeAndBeneficiaryDataEndpoint _endpoint;

    public TerminatedEmployeeAndBeneficiaryTests()
    {
        TerminatedEmployeeAndBeneficiaryReportService mockService =
            new TerminatedEmployeeAndBeneficiaryReportService(MockDbContextFactory);
        _endpoint = new TerminatedEmployeeAndBeneficiaryDataEndpoint(mockService);
    }

    private readonly ProfitYearRequest _requestDto = new ProfitYearRequest()
    {
        ProfitYear = 2023
    };

    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Act
        var response =
            await ApiClient
                .GETAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                    ProfitYearRequest, TerminatedEmployeeAndBeneficiaryResponse>(_requestDto);

        // Assert
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact(DisplayName = "Test Single Terminated Employee")]
    public async Task TerminatedEmployee()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            var demo = await c.Demographics.Include(demographic => demographic.ContactInfo).FirstAsync();
            demo.BadgeNumber = 9988;
            demo.TerminationCodeId = TerminationCode.Constants.AnotherJob;
            demo.EmploymentStatusId = EmploymentStatus.Constants.Terminated;
            demo.TerminationDate = new DateOnly(2023, 6, 1);
            demo.ContactInfo.FullName = "Smith, Nancy K";
            demo.DateOfBirth = new DateOnly(2000, 1, 1);

            var pp = await c.PayProfits.FirstAsync(pp => pp.OracleHcmId == demo.OracleHcmId);
            pp.EnrollmentId = Enrollment.Constants.NewVestingPlanHasContributions;

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

            var response = await _endpoint.GetResponse(_requestDto, CancellationToken.None);
           

            response!.ReportName.Should().BeEquivalentTo("Terminated Employee and Beneficiary Report");

            response.Response.Total.ShouldBeEquivalentTo(1);
            response.Response.Results.Count().ShouldBeEquivalentTo(1);

            response.TotalEndingBalance.ShouldBeEquivalentTo(846.15m);
            response.TotalVested.ShouldBeEquivalentTo(122.24m);
            response.TotalForfeit.ShouldBeEquivalentTo(277.91m);
            response.TotalBeneficiaryAllocation.ShouldBeEquivalentTo(222.23m);

            TerminatedEmployeeAndBeneficiaryDataResponseDto member = response.Response.Results.First();
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

    [Fact(DisplayName = "PS-300 | Test Single Beneficiary")]
    public async Task SingleBeneficiary()
    {
        await MockDbContextFactory.UseWritableContext(async c =>
        {
            // Arrange
            var bene = await c.Beneficiaries
                .Include(beneficiary => beneficiary.Contact)
                .ThenInclude(beneficiaryContact => beneficiaryContact!)
                .Include(beneficiary => beneficiary.Demographic)
                .ThenInclude(d => d!.PayProfits)
                .FirstAsync();
            bene.Amount = 379.44m;
            bene.Contact!.FirstName = "Rogue";
            bene.Contact.LastName = "One";
            bene.Contact.MiddleName = "I";
            bene.Contact.DateOfBirth = new DateOnly(1990, 1, 1);
            bene.Demographic!.PayProfits[0].ProfitYear = 2023;

            await c.SaveChangesAsync();

           
            // Act
            ApiClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            var response =
                await ApiClient
                    .GETAsync<TerminatedEmployeeAndBeneficiaryDataEndpoint,
                        ProfitYearRequest, TerminatedEmployeeAndBeneficiaryResponse>(_requestDto);

            // Assert
            response.Response.Content.Should().NotBeNull();
            response.Result.ReportName.Should().BeEquivalentTo("Terminated Employee and Beneficiary Report");

            response.Result.Response.Total.Should().BeGreaterOrEqualTo(1);
            response.Result.Response.Results.Should().HaveCountGreaterOrEqualTo(1);

            response.Result.TotalEndingBalance.Should().Be((decimal)379.44);
            response.Result.TotalVested.Should().Be((decimal)379.44);
            response.Result.TotalForfeit.Should().Be(0m);
            response.Result.TotalBeneficiaryAllocation.Should().Be(0m);

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
