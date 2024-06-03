using Demoulas.Common.Contracts.Request;
using Demoulas.ProfitSharing.Services;
using Demoulas.ProfitSharing.Services.Mappers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using MockDataContextFactory = Demoulas.ProfitSharing.IntegrationTests.Mocks.MockDataContextFactory;

namespace Demoulas.ProfitSharing.IntegrationTests;

public class OracleDbContextTests
{

    [Fact]
    public async Task TestInsertAndRetrieveEntity()
    {
        var factory = MockDataContextFactory.InitializeForTesting();
        var mapper = new DemographicMapper(new AddressMapper(), new ContactInfoMapper());
        var ds = new DemographicsService(factory, mapper);

      
        await factory.UseReadOnlyContext(async c =>
        {
            //bool haveDefinitions = await c.Definitions.AnyAsync();
            //haveDefinitions.Should().BeTrue();

            bool haveCountries = await c.Countries.AnyAsync();
            haveCountries.Should().BeTrue();

            bool payClassifications = await c.PayClassifications.AnyAsync();
            payClassifications.Should().BeTrue();

            return haveCountries && payClassifications;
        });

        var demographics = await ds.GetAllDemographics(new PaginationRequestDto(), CancellationToken.None);
        demographics.Should().NotBeNull();
        demographics.Results.Should().HaveCountGreaterOrEqualTo(10);

        await Task.CompletedTask;
    }
}
