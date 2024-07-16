using System.ComponentModel;
using System.IO;
using System.Text;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;

[Description("Tests for Negative ETVA for SSNs on PayProfit")]
public class NegativeETVAForSSNsOnPayProfitTests : IClassFixture<ApiTestBase<Api.Program>>
{
    private readonly YearEndClient _client;

    public NegativeETVAForSSNsOnPayProfitTests(ApiTestBase<Api.Program> fixture)
    {
        _client = new YearEndClient(fixture.ApiClient, fixture.DownloadClient);
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (JSON)")]
    public async Task GetReportJson()
    {
        var response = await _client.GetNegativeETVAForSSNsOnPayProfitResponse(CancellationToken.None);

        response.Should().NotBeNull();
        response.ReportName.Should().BeEquivalentTo("Negative ETVA for SSNs on PayProfit");
    }

    [Fact(DisplayName = "PS-145 : Negative ETVA for SSNs on PayProfit (CSV)")]
    public async Task GetReportCsv()
    {
        var stream = await _client.DownloadNegativeETVAForSSNsOnPayProfitResponse(CancellationToken.None);
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        string result = await reader.ReadToEndAsync();
        result.Should().NotBeNullOrEmpty();
    }
}
