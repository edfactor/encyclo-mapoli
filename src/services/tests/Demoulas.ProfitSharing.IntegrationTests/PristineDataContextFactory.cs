using System.Diagnostics;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demoulas.ProfitSharing.IntegrationTests;

/**
 * Used to connect to the live/qa/testing database context.  Ideally one that is kept in a pristine condition, so integration tests
 * can find a reliable place to start from.  Here pristine means the obfuscated data is imported w/o additional database changes.
 */
internal sealed class PristineDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly ProfitSharingReadOnlyDbContext _readOnlyCtx;

    public PristineDataContextFactory(string connectionString, bool debug = false)
    {
        DbContextOptions<ProfitSharingReadOnlyDbContext> readOnlyOptions;
        if (debug)
        {
            // Dumps sql
            readOnlyOptions =
                new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString)
                    .EnableSensitiveDataLogging().LogTo(s => Debug.WriteLine(s)).Options;
        }
        else
        {

            readOnlyOptions =
                new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString)
                    .Options;
        }
        
        ProfitSharingReadOnlyDbContext readOnlyCtx = new ProfitSharingReadOnlyDbContext(readOnlyOptions);

        _readOnlyCtx = readOnlyCtx;
    }

    public Task UseWritableContext(Func<ProfitSharingDbContext, Task> func,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<T> UseWritableContextAsync<T>(Func<ProfitSharingDbContext, IDbContextTransaction, Task<T>> action, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        return func(_readOnlyCtx);
    }

    public Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        throw new NotImplementedException();
    }
}
