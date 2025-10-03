using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.Common.Data.Contexts.Factory;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Factories;

public sealed class DataContextFactory : DataContextFactoryBase<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>, IProfitSharingDataContextFactory
{
    private DataContextFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public static IProfitSharingDataContextFactory Initialize(IHostApplicationBuilder builder, IEnumerable<ContextFactoryRequest> contextFactoryRequests)
    {
        var serviceCollection = InitializeContexts(builder, contextFactoryRequests);

        return new DataContextFactory(serviceCollection.BuildServiceProvider());
    }


    /// <summary>
    /// Context to access Store related data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        using (Logger.BeginScope("Store Info DB Operation"))
        {
            await using AsyncServiceScope scope = ServiceProvider.CreateAsyncScope();
            DemoulasCommonDataContext dbContext = scope.ServiceProvider.GetRequiredService<DemoulasCommonDataContext>();
            return await func(dbContext);
        }
    }
}
