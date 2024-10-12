#if DEBUG || RUSS
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.UnitTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Demoulas.ProfitSharing.UnitTests;

public class DatabaseDataTest : IClassFixture<ApiTestBase<Program>>
{
    private readonly ITestOutputHelper _output;

    public DatabaseDataTest(ITestOutputHelper output)
    {
        _output = output;
    }


    [Fact(DisplayName = "Ensure database entity mappings are working.")]
    public async Task BasicTest()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<DatabaseDataTest>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>().UseOracle(connectionString).EnableSensitiveDataLogging()
            .LogTo(_output.WriteLine).Options;
        var ctx = new ProfitSharingDbContext(options);

        await ctx.Demographics.Take(5).ToListAsync();
        await ctx.Beneficiaries.Take(5).ToListAsync();
        await ctx.PayProfits.Take(5).ToListAsync();
        await ctx.ProfitDetails.Take(5).ToListAsync();
        await ctx.Distributions.Take(5).ToListAsync();

        Assert.True(true);
    }
}
#endif
