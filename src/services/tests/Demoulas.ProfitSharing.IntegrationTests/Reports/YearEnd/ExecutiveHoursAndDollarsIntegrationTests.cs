using System.Reflection;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ExecutiveHoursAndDollars;
using Demoulas.ProfitSharing.Security;
using Demoulas.ProfitSharing.UnitTests.Extensions;
using FastEndpoints;
using FluentAssertions;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd;
public class ExecutiveHoursAndDollarsIntegrationTests : ApiIntegrationTestBase<Program>
{
    // Probably should define these somewhere more global, or be looked up dynamically 
    private const short YearThis = 2023;
    private const short YearLast = 2022;

    [Fact]
    public async Task Ensure_SMART_CSV_matches_READY_CSV_for_year_THIS()
    {
        string expectCsvContents = ReadEmbeddedResource("Demoulas.ProfitSharing.IntegrationTests.Resources.ExecutiveHoursAndDollars year=this.csv");
        expectCsvContents.Should().NotBeEmpty();

        DownloadClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var response =
            await DownloadClient.GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest, StreamContent>(
                new ExecutiveHoursAndDollarsRequest { ProfitYear = YearThis, HasExecutiveHoursAndDollars = true});

        string csvData = await response.Response.Content.ReadAsStringAsync();

        // Break SVS into lines
        var lines = csvData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Todays date
        lines[0].Should().NotBeEmpty();

        // Title check
        lines[1].Should().Be($"Executive Hours and Dollars for Year {YearThis}");

        // remaining content should match expectedCsvContent
        var actualCsvContents = string.Join("\n", lines.Skip(2));

        // Handles leading zeros, odd spacing, and case insensitive comparisons
        TolerantCsvComparisonUtility.ShouldBeTheSame(actualCsvContents, expectCsvContents);
    }

    [Fact]
    public async Task Ensure_SMART_CSV_matches_READY_CSV_for_year_LAST()
    {
        DownloadClient.CreateAndAssignTokenForClient(Role.ADMINISTRATOR);

        var response =
            await DownloadClient.GETAsync<ExecutiveHoursAndDollarsEndpoint, ExecutiveHoursAndDollarsRequest, StreamContent>(
                new ExecutiveHoursAndDollarsRequest { ProfitYear = YearLast, HasExecutiveHoursAndDollars = true});

        string csvData = await response.Response.Content.ReadAsStringAsync();

        // Break CVS into lines
        var lines = csvData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Todays date
        lines[0].Should().NotBeEmpty();

        // Title check
        lines[1].Should().Be($"Executive Hours and Dollars for Year {YearLast}");

        // remaining content should match expectedCsvContent
        var actualCsvContents = string.Join("\n", lines.Skip(2));

        // We do not have actual executive hours and dollars for 2022, so we expect just the headers
        actualCsvContents.Should().Be("BADGE,NAME,STR,EXEC HRS,EXEC DOLS,ORA HRS CUR,ORA DOLS CUR,FREQ,STATUS\n");
    }


    public static string ReadEmbeddedResource(string resourceName)
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        using (var reader = new StreamReader(stream!))
        {
            return reader.ReadToEnd();
        }
    }


}
