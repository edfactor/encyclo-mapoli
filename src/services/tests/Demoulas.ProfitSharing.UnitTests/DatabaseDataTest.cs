using System.Diagnostics;
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

        //Assert.True(await ctx.Demographics.CountAsync() > 0);
        
        await ctx.Beneficiaries.Take(5).ToListAsync();
        
        Assert.True(await ctx.PayProfits.CountAsync() > 0);
        Assert.True(await ctx.ProfitDetails.CountAsync() > 0);
        Assert.True(await ctx.Distributions.CountAsync() > 0);

    }
}
