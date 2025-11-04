using System.Diagnostics;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

// ReSharper disable ClassNeverInstantiated.Local

namespace Demoulas.ProfitSharing.IntegrationTests;

/**
 * Here pristine means the "obfuscated data is imported w/o additional database changes."
 * Used to connect to the live/qa/testing database context.  Ideally one that is kept in a pristine condition, so integration tests
 * can find a reliable place to start from.
 */
public sealed class PristineDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly ProfitSharingDbContext _ctx;
    private readonly ProfitSharingReadOnlyDbContext _readOnlyCtx;
    public string ConnectionString { get; }

    public PristineDataContextFactory(bool debug = false)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Api.Program>()
            .Build();
        ConnectionString = configuration["ConnectionStrings:ProfitSharing"]!;

        _readOnlyCtx = setUpReadOnlyCtx(ConnectionString, debug);
        _ctx = setUpWriteCtx(ConnectionString, debug);
    }

    public Task UseWritableContext(Func<ProfitSharingDbContext, Task> func,
        CancellationToken cancellationToken = default)
    {
        return func(_ctx);
    }

    public Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func,
        CancellationToken cancellationToken = new())
    {
        return func(_ctx);
    }

    public Task<T> UseWritableContextAsync<T>(Func<ProfitSharingDbContext, IDbContextTransaction, Task<T>> action, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UseReadOnlyContext(Func<ProfitSharingReadOnlyDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        return func(_readOnlyCtx);
    }

    public Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        return func(_readOnlyCtx);
    }

    public Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        return func(_readOnlyCtx);
    }

    public Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        throw new NotImplementedException();
    }

    private static ProfitSharingDbContext setUpWriteCtx(string connectionString, bool debug)
    {
        DbContextOptionsBuilder<ProfitSharingDbContext> optionsBuilderWriter = new DbContextOptionsBuilder<ProfitSharingDbContext>()
            .UseOracle(connectionString, opts => opts.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
        if (debug)
        {
            // Dumps sql
            optionsBuilderWriter.EnableSensitiveDataLogging().LogTo(s => Debug.WriteLine(s));
        }

        ProfitSharingDbContext ctx = new(optionsBuilderWriter.Options);

        return ctx;
    }

    private static ProfitSharingReadOnlyDbContext setUpReadOnlyCtx(string connectionString, bool debug)
    {
        DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext> optionsBuilder = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>()
            .UseOracle(connectionString, opts => opts.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19));
        if (debug)
        {
            optionsBuilder.EnableSensitiveDataLogging().LogTo(s => Debug.WriteLine(s));
        }

        ProfitSharingReadOnlyDbContext readOnlyCtx = new(optionsBuilder.Options);

        return readOnlyCtx;
    }
}
