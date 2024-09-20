using System.Net;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Demoulas.ProfitSharing.Security;
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


    [Fact(DisplayName = "Get Terminated Employee And Beneficiary Report")]
    public async Task DownloadTerminatedEmployeeAndBeneficiaryReport()
    {
        // Arange
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

    [Fact(DisplayName = "terminatedEmployee")]
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
            // Here we move these records out of the way.
            foreach (var detail in details)
            {
                detail.ProfitYear = 2021;
            }

            await c.SaveChangesAsync();

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
            string result = await response.Response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(result);

            result.Should().BeEquivalentTo(@"DJDE JDE=LANIQS,JDL=DFLT4,END,;
DON MULLIGAN
QPAY066    TERMINATION - PROFIT SHARING                    DATE SEP 19, 2024  YEAR:   2023.0                         PAGE:   000001
           FROM 01/07/2023 TO 01/02/2024

                                  BEGINNING  BENEFICIARY   DISTRIBUTION                 ENDING       VESTED    DATE      YTD VST     E
BADGE/PSN # EMPLOYEE NAME           BALANCE  ALLOCATION       AMOUNT       FORFEIT      BALANCE      BALANCE   TERM   PS HRS PCT AGE C

       9988 Smith, Nancy K           446.00         0.00         0.00         0.00       446.00         0.00  230601    0.00 100  24


TOTALS
AMOUNT IN PROFIT SHARING                 446.00
VESTED AMOUNT                              0.00
TOTAL FORFEITURES                          0.00
TOTAL BENEFICIARY ALLOCTIONS               0.00
");
        });
    }
}
