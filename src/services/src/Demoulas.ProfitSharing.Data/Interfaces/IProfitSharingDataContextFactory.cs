using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.StoreInfo.Entities.Contexts;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDataContextFactory : IDataContextFactory<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>
{
    Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default);
    Task<T> UseStoreInfoContext<T>(Func<StoreInfoDbContext, Task<T>> func);
}
