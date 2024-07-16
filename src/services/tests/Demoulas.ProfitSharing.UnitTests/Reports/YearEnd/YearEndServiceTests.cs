using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client.Reports.YearEnd;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;

namespace Demoulas.ProfitSharing.UnitTests.Reports.YearEnd;
public class YearEndServiceTests:IClassFixture<ApiTestBase<Program>>
{
    private readonly YearEndClient _yearEndClient;

    public YearEndServiceTests(ApiTestBase<Program> fixture)
    {
        _yearEndClient = new YearEndClient(fixture.ApiClient, fixture.DownloadClient);
    }

    [Fact(DisplayName ="Check Duplicate SSNs")]
    public async Task GetDuplicateSSNsTest()
    {
        var response = await _yearEndClient.GetDuplicateSSNs(CancellationToken.None);
        response.Should().NotBeNull();
        response.Count.Should().Be(0); //Duplicate SSNs aren't allowed in our data model, prohibited by primary key on SSN in the demographics table.
    }
}
