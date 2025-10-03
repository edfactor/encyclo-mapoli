using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Mocks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Demoulas.ProfitSharing.UnitTests.Services;

public class MockDistributionTests
{
    [Fact]
    public async Task MockDistribution_ShouldPersistAndBeQueryable()
    {
        // Arrange
        var factory = MockDataContextFactory.InitializeForTesting();

        // Act - Create a distribution
        var distributionId = 0L;
        await factory.UseWritableContext(async ctx =>
        {
            var distribution = new Distribution
            {
                Ssn = 123456789,
                PaymentSequence = 1,
                EmployeeName = "Test Employee",
                StatusId = 'A',
                FrequencyId = 'M',
                FederalTaxAmount = 100.00m,
                StateTaxAmount = 50.00m,
                GrossAmount = 1000.00m,
                TaxCodeId = '1',
                Memo = "Test distribution"
            };

            ctx.Distributions.Add(distribution);
            await ctx.SaveChangesAsync();
            distributionId = distribution.Id;
        });

        // Assert - Verify the distribution was created and can be found
        distributionId.ShouldBeGreaterThan(0);

        var foundDistribution = await factory.UseReadOnlyContext(async ctx =>
        {
            return await ctx.Distributions
                .Where(d => d.Id == distributionId)
                .FirstOrDefaultAsync();
        });

        foundDistribution.ShouldNotBeNull();
        foundDistribution.Id.ShouldBe(distributionId);
        foundDistribution.EmployeeName.ShouldBe("Test Employee");
        foundDistribution.Memo.ShouldBe("Test distribution");
    }
}
