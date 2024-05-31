using Demoulas.Common.Contracts.Request;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Mappers;
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
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());
        var ds = new DemographicsService(factory, mapper);

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

            bool payClassifications = await c.PayClassifications.AnyAsync();
            payClassifications.Should().BeTrue();

            return haveDefinitions && haveCountries && payClassifications;
        });

        _ = await ds.GetAllDemographics(new PaginationRequestDto(), CancellationToken.None);

        await Task.CompletedTask;
    }
}
