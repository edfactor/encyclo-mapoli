using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDataContextFactory : IDataContextFactory<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>
{
    Task<T> UseWarehouseContext<T>(Func<IDemoulasCommonWarehouseContext, Task<T>> func);
}
