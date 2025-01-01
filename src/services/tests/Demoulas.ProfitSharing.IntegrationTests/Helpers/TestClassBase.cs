using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.IntegrationTests.Fixtures;
using FastEndpoints.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.IntegrationTests.Helpers;

public abstract class TestClassBase : TestBase<IntegrationTestsFixture>
{
    protected IProfitSharingDataContextFactory ProfitSharingDataContextFactory { get; init; }
    protected EndPointSetupHelper EndPointHelper { get; init; }

    protected TestClassBase(ITestOutputHelper o, IntegrationTestsFixture fixture)
    {
        EndPointHelper = EndPointSetupHelper.CreateInstance(o);
        ProfitSharingDataContextFactory = fixture.Services.GetRequiredService<IProfitSharingDataContextFactory>();
    }
}
