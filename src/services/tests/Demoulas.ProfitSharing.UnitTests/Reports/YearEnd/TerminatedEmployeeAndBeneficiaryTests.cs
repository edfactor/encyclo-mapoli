using System.Text;
using System.Text.Json;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using IdGen;
using Xunit;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Military;
using FastEndpoints;
using System.Net;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using System.Threading;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Elastic.CommonSchema;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.Wages;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class TerminatedEmployeeAndBeneficiaryTests:ApiTestBase<Program>
{
    private readonly YearEndClient _yearEndClient;
    private readonly ITestOutputHelper _testOutputHelper;

    public TerminatedEmployeeAndBeneficiaryTests( ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _yearEndClient = new YearEndClient(ApiClient, DownloadClient);
    }
    

    [Fact(DisplayName ="Get Terminated Employee And Beneficiary Report")]
    public async  Task DownloadTerminatedEmployeeAndBeneficiaryReport()
    {
        _yearEndClient.CreateAndAssignTokenForClient(Role.FINANCEMANAGER);

        DateOnly fiscalStart = new DateOnly(2023, 1, 7);
        DateOnly fiscalEnd = new DateOnly(2024, 1, 2);
        decimal profitSharingYear = 2023.0m;

        var stream =  await _yearEndClient.DownloadTerminatedEmployeeAndBeneficiaryReport(fiscalStart, fiscalEnd, profitSharingYear, CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

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
        TerminatedEmployeeAndBeneficiaryReportRequestDto requestDto = new()
        {
            startDate = new DateOnly(2023, 1, 7),
            endDate = new DateOnly(2024, 1, 2),
            profitShareYear = 2023.0m
        };

        var response =
            await DownloadClient.GETAsync<TerminatedEmployeeAndBeneficiaryReportEndpoint, TerminatedEmployeeAndBeneficiaryReportRequestDto, string>(requestDto);
        
        response.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}


