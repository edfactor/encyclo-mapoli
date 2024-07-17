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

    [Fact(DisplayName ="Check Duplicate SSNs")]
    public async Task GetDuplicateSSNsTest()
    {
        var response = await _yearEndClient.GetDuplicateSSNs(CancellationToken.None);
        response.Should().NotBeNull();
        response.Count.Should().Be(0); //Duplicate SSNs aren't allowed in our data model, prohibited by primary key on SSN in the demographics table.
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
}
