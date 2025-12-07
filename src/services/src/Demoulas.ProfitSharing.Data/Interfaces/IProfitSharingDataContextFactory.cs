using Demoulas.Common.Data.Contexts.Interfaces;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDataContextFactory : IDataContextFactory<ProfitSharingDbContext, ProfitSharingReadOnlyDbContext>
{
    Task<T> UseWarehouseContext<T>(Func<DemoulasCommonDataContext, Task<T>> func);
}
