
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using global::Demoulas.Common.Data.Services.Entities.Contexts;
using global::Demoulas.ProfitSharing.Data.Contexts;
using global::Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.IntegrationTests;

/**
 * Used to connect to the live/qa/testing database context.  Ideally one that is kept in a pristine condition, so integration tests
 * can find a reliable place to start from.  Here pristine means the obfuscated data is imported w/o additional database changes.
 */
sealed class PristineDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly ProfitSharingReadOnlyDbContext _readOnlyCtx;

    public PristineDataContextFactory(string connectionString)
    {
        //        var options = new DbContextOptionsBuilder<ProfitSharingDbContext>().UseOracle(connectionString).Options; // .EnableSensitiveDataLogging()
        //.LogTo(_output.WriteLine).Options
        //var ctx = new ProfitSharingDbContext(options)

        var readOnlyOptions = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString).Options; // .EnableSensitiveDataLogging()
        //.LogTo(_output.WriteLine).Options

        var readOnlyCtx = new ProfitSharingReadOnlyDbContext(readOnlyOptions);

        _readOnlyCtx = readOnlyCtx;
    }

    public Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        return func(_readOnlyCtx);
    }

    public Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        throw new NotImplementedException();
    }
};
