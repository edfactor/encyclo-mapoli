using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Fixtures;
using FastEndpoints.Testing;
using Microsoft.Testing.Platform.Services;

namespace Demoulas.ProfitSharing.IntegrationTests.Helpers;

public abstract class TestClassBase : TestBase<IntegrationTestsFixture>
{
    protected IProfitSharingDataContextFactory ProfitSharingDataContextFactory { get; init; }
    
    protected TestClassBase(IntegrationTestsFixture fixture)
    {
        ProfitSharingDataContextFactory = fixture.Services.GetRequiredService<IProfitSharingDataContextFactory>();
    }
}
