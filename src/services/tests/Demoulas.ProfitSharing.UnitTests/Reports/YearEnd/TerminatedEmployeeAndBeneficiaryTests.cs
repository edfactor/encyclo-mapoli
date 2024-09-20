using System.Diagnostics;
using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Request;
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
    private readonly ITestOutputHelper _testOutputHelper;

    public TerminatedEmployeeAndBeneficiaryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _yearEndClient = new YearEndClient(ApiClient, DownloadClient);
    }


    [Fact(DisplayName = "Test report with nobody applicable - sanity check")]
    public async Task DownloadTerminatedEmployeeAndBeneficiaryReport()
    {
        // Arrange
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);
        DateOnly fiscalStart = new DateOnly(2023, 1, 7);
        DateOnly fiscalEnd = new DateOnly(2024, 1, 2);
        decimal profitSharingYear = 2023.0m;

        // Act
        var stream = await _yearEndClient.DownloadTerminatedEmployeeAndBeneficiaryReport(fiscalStart, fiscalEnd,
            profitSharingYear, CancellationToken.None);

        // Assert
        stream.Should().NotBeNull();
        using var reader = new StreamReader(stream);
        string result = await reader.ReadToEndAsync();
        result.Should().BeEquivalentTo(@"

TOTALS
AMOUNT IN PROFIT SHARING                   0.00
VESTED AMOUNT                              0.00
TOTAL FORFEITURES                          0.00
TOTAL BENEFICIARY ALLOCTIONS               0.00
");

        _testOutputHelper.WriteLine(result);

    }


    [Fact(DisplayName = "Unauthorized")]
    public async Task Unauthorized()
    {
        // Arrange
        TerminatedEmployeeAndBeneficiaryReportRequestDto requestDto = new()
        {
            startDate = new DateOnly(2023, 1, 7), endDate = new DateOnly(2024, 1, 2), profitShareYear = 2023.0m
        };

        // Act
        var response =
            await DownloadClient
                .GETAsync<TerminatedEmployeeAndBeneficiaryReportEndpoint,
                    TerminatedEmployeeAndBeneficiaryReportRequestDto, string>(requestDto);

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
            TerminatedEmployeeAndBeneficiaryReportService.useThisForTodaysDateWhenTesting = new DateOnly(2024, 9, 7);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            TerminatedEmployeeAndBeneficiaryReportRequestDto requestDto = new()
            {
                startDate = new DateOnly(2023, 1, 7),
                endDate = new DateOnly(2024, 1, 2),
                profitShareYear = 2023.0m
            };
            var response =
                await DownloadClient
                    .GETAsync<TerminatedEmployeeAndBeneficiaryReportEndpoint,
                        TerminatedEmployeeAndBeneficiaryReportRequestDto, string>(requestDto);


            // Assert
            response.Response.Content.Should().NotBeNull();
            string actualText = await response.Response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(actualText);

            string expectedText = @"DJDE JDE=LANIQS,JDL=DFLT4,END,;
DON MULLIGAN
QPAY066    TERMINATION - PROFIT SHARING                    DATE SEP 07, 2024  YEAR:   2023.0                         PAGE:   000001
           FROM 01/07/2023 TO 01/02/2024

                                  BEGINNING  BENEFICIARY   DISTRIBUTION                 ENDING       VESTED    DATE      YTD VST     E
BADGE/PSN # EMPLOYEE NAME           BALANCE  ALLOCATION       AMOUNT       FORFEIT      BALANCE      BALANCE   TERM   PS HRS PCT AGE C

       9988 Smith, Nancy K           446.00       222.23        99.99-      277.91       846.15       122.24  230601    0.00 100  24


TOTALS
AMOUNT IN PROFIT SHARING                 846.15
VESTED AMOUNT                            122.24
TOTAL FORFEITURES                        277.91
TOTAL BENEFICIARY ALLOCTIONS             222.23
";
            expectedText.Should().BeEquivalentTo(actualText);
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
            TerminatedEmployeeAndBeneficiaryReportService.useThisForTodaysDateWhenTesting = new DateOnly(2024, 9, 7);

            // Act
            DownloadClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

            TerminatedEmployeeAndBeneficiaryReportRequestDto requestDto = new()
            {
                startDate = new DateOnly(2023, 1, 7),
                endDate = new DateOnly(2024, 1, 2),
                profitShareYear = 2023.0m
            };
            var response =
                await DownloadClient
                    .GETAsync<TerminatedEmployeeAndBeneficiaryReportEndpoint,
                        TerminatedEmployeeAndBeneficiaryReportRequestDto, string>(requestDto);

            // Assert
            response.Response.Content.Should().NotBeNull();
            string actualText = await response.Response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(actualText);

             string expectedText = @"DJDE JDE=LANIQS,JDL=DFLT4,END,;
DON MULLIGAN
QPAY066    TERMINATION - PROFIT SHARING                    DATE SEP 07, 2024  YEAR:   2023.0                         PAGE:   000001
           FROM 01/07/2023 TO 01/02/2024

                                  BEGINNING  BENEFICIARY   DISTRIBUTION                 ENDING       VESTED    DATE      YTD VST     E
BADGE/PSN # EMPLOYEE NAME           BALANCE  ALLOCATION       AMOUNT       FORFEIT      BALANCE      BALANCE   TERM   PS HRS PCT AGE C

     888888 One, Rogue I             379.44         0.00         0.00         0.00       379.44       379.44            0.00 100  34


TOTALS
AMOUNT IN PROFIT SHARING                 379.44
VESTED AMOUNT                            379.44
TOTAL FORFEITURES                          0.00
TOTAL BENEFICIARY ALLOCTIONS               0.00
";

             actualText.Should().BeEquivalentTo(expectedText);
        });
    }

}
