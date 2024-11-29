using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update.DbHelpers;
 internal sealed class DbFactory  :IProfitSharingDataContextFactory
{
    ProfitSharingReadOnlyDbContext ro;

    public DbFactory(ProfitSharingReadOnlyDbContext ro)
    {
        this.ro = ro;
    }

    public Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        return func(ro);
    }

    public Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        throw new NotImplementedException();
    }
}
