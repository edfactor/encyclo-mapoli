using System.Text.Json;
using System.Text;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Base;
using Elastic.CommonSchema;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class YearEndServiceTests:IClassFixture<ApiTestBase<Program>>
{
    private readonly YearEndClient _yearEndClient;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;


    public YearEndServiceTests(ApiTestBase<Program> fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _dataContextFactory = fixture.MockDbContextFactory;
        _yearEndClient = new YearEndClient(fixture.ApiClient, fixture.DownloadClient);
    }

    [Fact(DisplayName ="PS-147: Check Duplicate SSNs")]
    public async Task GetDuplicateSSNsTest()
    {
        var response = await _yearEndClient.GetDuplicateSSNs(CancellationToken.None);
        response.Should().NotBeNull();
        response.Results.Count.Should().Be(0); //Duplicate SSNs aren't allowed in our data model, prohibited by primary key on SSN in the demographics table.
    }

    [Fact(DisplayName ="PS-150: Payprofit badges w/o Demographics")]
    public async Task GetPayProfitBadgesNotInDemographics()
    {
        
        await _dataContextFactory.UseWritableContext(async c =>
        {
            var response = await _yearEndClient.GetPayProfitBadgesNotInDemographics(CancellationToken.None);
            response.Should().NotBeNull();
            response.Results.Should().HaveCount(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            await c.PayProfits.Take(mismatchedValues).ForEachAsync(pp =>
            {
                if (pp.Demographic != null)
                {
                    pp.Demographic.BadgeNumber = pp.EmployeeBadge + c.Demographics.Count() + 1;
                }
            });

            await c.SaveChangesAsync();

            response = await _yearEndClient.GetPayProfitBadgesNotInDemographics(CancellationToken.None);
            response.Should().NotBeNull();
            response.Results.Should().HaveCount(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            //Revert changes back
            await c.PayProfits.Take(mismatchedValues).ForEachAsync(pp =>
            {
                if (pp.Demographic != null)
                {
                    pp.Demographic.BadgeNumber = pp.EmployeeBadge;
                }
            });

            await c.SaveChangesAsync();
        });
    }

    [Fact(DisplayName = "PS-151: Demographic badges without payprofit")]
    public async Task GetDemogrpahicBadgesWithoutPayProfitTests()
    {
        await _dataContextFactory.UseWritableContext(async c =>
        {
            var response = await _yearEndClient.GetDemographicBadgesNotInPayProfit(CancellationToken.None);
            response.Should().NotBeNull();
            response.Results.Should().HaveCount(0);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            byte mismatchedValues = 5;

            await c.Demographics.Take(mismatchedValues).ForEachAsync(dem =>
            {
                dem.BadgeNumber = dem.BadgeNumber + c.PayProfits.Count() + 1;
            });

            await c.SaveChangesAsync();

            response = await _yearEndClient.GetDemographicBadgesNotInPayProfit(CancellationToken.None);
            response.Should().NotBeNull();
            response.Results.Should().HaveCount(mismatchedValues);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

            await c.Demographics.Take(mismatchedValues).ForEachAsync(dem =>
            {
                dem.BadgeNumber = dem.BadgeNumber - c.PayProfits.Count() - 1;
            });

            await c.SaveChangesAsync();
        });
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (JSON)")]
    public async Task GetNegativeETVAReportJson()
    {
        byte negativeValues = 5;
        await _dataContextFactory.UseWritableContext(async c =>
        {
            await c.PayProfits.Take(negativeValues).ForEachAsync(pp =>
            {
                pp.EarningsEtvaValue *= -1;
            });


            await c.SaveChangesAsync();
        });


        var response = await _yearEndClient.GetNegativeETVAForSSNsOnPayProfitResponse(CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("Negative ETVA for SSNs on PayProfit");
        response.Results.Should().HaveCount(negativeValues);

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (CSV)")]
    public async Task GetNegativeETVAReportCsv()
    {
        var stream = await _yearEndClient.DownloadNegativeETVAForSSNsOnPayProfitResponse(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }


    [Fact(DisplayName = "PS-149 : Mismatched Ssns Payprofit and Demographics On Same Badge (JSON)")]
    public async Task GetMismatchedSsnsPayprofitAndDemographicsOnSameBadgeJson()
    {
        var response = await _yearEndClient.GetMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("MISMATCHED SSNs PAYPROFIT AND DEMO ON SAME BADGE");

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-149 : Mismatched Ssns Payprofit and Demographics On Same Badge (CSV)")]
    public async Task GetMismatchedSsnsPayprofitAndDemographicsOnSameBadgeCsv()
    {
        var stream = await _yearEndClient.DownloadMismatchedSsnsPayprofitAndDemographicsOnSameBadge(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }






    [Fact(DisplayName = "PS-148 : Payroll Duplicate Ssns On Payprofit (JSON)")]
    public async Task GetPayrollDuplicateSsnsOnPayprofitJson()
    {
        var response = await _yearEndClient.GetPayrollDuplicateSsnsOnPayprofit(CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("PAYROLL DUPLICATE SSNs ON PAYPROFIT");

        _testOutputHelper.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact(DisplayName = "PS-148 : Payroll Duplicate Ssns On Payprofit (CSV)")]
    public async Task GetPayrollDuplicateSsnsOnPayprofitCsv()
    {
        var stream = await _yearEndClient.DownloadPayrollDuplicateSsnsOnPayprofit(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();

        _testOutputHelper.WriteLine(result);
    }
}
