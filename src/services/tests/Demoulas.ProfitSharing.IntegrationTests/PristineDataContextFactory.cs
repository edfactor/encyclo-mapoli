using System.Diagnostics;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// ReSharper disable ClassNeverInstantiated.Local

namespace Demoulas.ProfitSharing.IntegrationTests;

/**
 * Used to connect to the live/qa/testing database context.  Ideally one that is kept in a pristine condition, so integration tests
 * can find a reliable place to start from.  Here pristine means the obfuscated data is imported w/o additional database changes.
 */
internal sealed class PristineDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly ProfitSharingDbContext _ctx;
    private readonly ProfitSharingReadOnlyDbContext _readOnlyCtx;

    public PristineDataContextFactory(string connectionString, bool debug = false)
    {
        _readOnlyCtx = setUpReadOnlyCtx(connectionString, debug);
        _ctx = setUpWriteCtx(connectionString, debug);
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

    public Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        return func(_readOnlyCtx);
    }

    public Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        throw new NotImplementedException();
    }

    private static ProfitSharingDbContext setUpWriteCtx(string connectionString, bool debug)
    {
        DbContextOptionsBuilder<ProfitSharingDbContext> optionsBuilderWriter = new DbContextOptionsBuilder<ProfitSharingDbContext>().UseOracle(connectionString);
        if (debug)
        {
            // Dumps sql
            optionsBuilderWriter.EnableSensitiveDataLogging().LogTo(s => Debug.WriteLine(s));
        }

        optionsBuilderWriter.ReplaceService<IModelCustomizer, GlobalBoolToNumberModelCustomizer>();
        ProfitSharingDbContext ctx = new(optionsBuilderWriter.Options);

        return ctx;
    }

    private static ProfitSharingReadOnlyDbContext setUpReadOnlyCtx(string connectionString, bool debug)
    {
        DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext> optionsBuilder = new DbContextOptionsBuilder<ProfitSharingReadOnlyDbContext>().UseOracle(connectionString);
        if (debug)
        {
            optionsBuilder.EnableSensitiveDataLogging().LogTo(s => Debug.WriteLine(s));
        }

        optionsBuilder.ReplaceService<IModelCustomizer, GlobalBoolToNumberModelCustomizer>();

        ProfitSharingReadOnlyDbContext readOnlyCtx = new(optionsBuilder.Options);

        return readOnlyCtx;
    }


    // This number <--> bool must be happening somewhere in the commons code?   I could not find it. 
    // This allow the Entity to have "bool isActive
    private sealed class GlobalBoolToNumberModelCustomizer : RelationalModelCustomizer
    {
        public GlobalBoolToNumberModelCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies)
        {
        }

        public override void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            base.Customize(modelBuilder, context);

            ValueConverter<bool, int> boolConverter = new(
                v => v ? 1 : 0, // Convert bool → int
                v => v == 1 // Convert int → bool
            );

            // Apply to ALL bool properties across ALL entities
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool) || property.ClrType == typeof(bool?))
                    {
                        property.SetValueConverter(boolConverter);
                        property.SetColumnType("NUMBER(1,0)");
                    }
                }
            }
        }
    }
}
