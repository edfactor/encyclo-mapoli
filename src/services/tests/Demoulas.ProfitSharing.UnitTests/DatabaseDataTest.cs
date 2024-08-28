using System;


using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Api;
using Demoulas.ProfitSharing.Client;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.UnitTests.Base;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demoulas.ProfitSharing.UnitTests;

public class DatabaseDataTest : IClassFixture<ApiTestBase<Program>>
{

    [Fact(DisplayName = "Ensure database entity mappings are working.")]
    public void BasicTest()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<DatabaseDataTest>().Build();
        string connectionString = configuration["ConnectionStrings:ProfitSharing"]!;
        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>().UseOracle(connectionString).EnableSensitiveDataLogging().LogTo(Console.WriteLine).Options;
        var ctx = new ProfitSharingDbContext(options);

        Assert.True(ctx.Demographics.ToList().Count > 0);
        Assert.True(ctx.Beneficiaries.ToList().Count > 0);
        Assert.True(ctx.PayProfits.ToList().Count > 0);
        Assert.True(ctx.ProfitDetails.ToList().Count > 0);
        Assert.True(ctx.Distributions.ToList().Count > 0);

    }
}
