using Demoulas.Common.Data.Contexts.DTOs.Request;
using Demoulas.ProfitSharing.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockDataContextFactory = Demoulas.ProfitSharing.IntegrationTests.Mocks.MockDataContextFactory;

namespace Demoulas.ProfitSharing.IntegrationTests;

public class OracleDbContextTests
{

    private string? GetConnectionString()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<OracleDbContextTests>()
            .Build();

        return builder.GetConnectionString("ProfitSharing");
    }


    [Fact]
    public async Task TestInsertAndRetrieveEntity()
    {
        string? connectionString = GetConnectionString();
        var factory = MockDataContextFactory.InitializeForTesting(new ServiceCollection(), connectionString);
        var ds = new DemographicsService(factory);

        await factory.UseWritableContext(async c =>
        {
            await c.Database.EnsureDeletedAsync();
            await c.Database.MigrateAsync();
        });

        await factory.UseReadOnlyContext(async c =>
        {
            bool haveDefinitions = await c.Definitions.AnyAsync();
            haveDefinitions.Should().BeTrue();

            bool haveCountries = await c.Countries.AnyAsync();
            haveCountries.Should().BeTrue();

            return haveDefinitions && haveCountries;
        });

        _ = await ds.GetAllDemographics(new PaginationRequestDto(), CancellationToken.None);

        await Task.CompletedTask;
    }
}
