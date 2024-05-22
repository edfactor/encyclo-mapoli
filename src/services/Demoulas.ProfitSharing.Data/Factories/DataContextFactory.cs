using System.Diagnostics;
using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using EntityFramework.Exceptions.Oracle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Data.Factories;

public sealed class DataContextFactory : IProfitSharingDataContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    private DataContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static DataContextFactory Initialize(IHostApplicationBuilder builder, IEnumerable<ContextFactoryRequest> contextFactoryRequests)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(contextFactoryRequests);

        List<ContextFactoryRequest> factoryRequests = contextFactoryRequests.ToList();
        if (factoryRequests.Count == 0)
        {
            throw new ArgumentException($"{nameof(contextFactoryRequests)} has no database contexts to configure");
        }

        void CommonDbBuilderSettings(DbContextOptionsBuilder dbBuilder)
        {
            _ = dbBuilder.UseExceptionProcessor();
            _ = dbBuilder.EnableSensitiveDataLogging(Debugger.IsAttached);
            _ = dbBuilder.EnableDetailedErrors(Debugger.IsAttached);
        }

        var addOracleDatabaseDbContext = typeof(AspireOracleEFCoreExtensions)
           .GetMethods()
           .First(m => m.Name == nameof(AspireOracleEFCoreExtensions.AddOracleDatabaseDbContext));

        var enrichOracleDatabaseDbContext = typeof(AspireOracleEFCoreExtensions)
           .GetMethods()
           .First(m => m.Name == nameof(AspireOracleEFCoreExtensions.EnrichOracleDatabaseDbContext));


        foreach (ContextFactoryRequest contextFactoryRequest in factoryRequests)
        {
            if (contextFactoryRequest.ContextType.BaseType?.Name == "OracleDbContext`1")
            {
                contextFactoryRequest.ConfigureDbContextOptions ??= CommonDbBuilderSettings;
            }
            else if (contextFactoryRequest.ContextType.BaseType?.Name == "ReadOnlyOracleDbContext`1")
            {
                contextFactoryRequest.ConfigureDbContextOptions ??= dbBuilder =>
                {
                    CommonDbBuilderSettings(dbBuilder);
                    _ = dbBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                    _ = dbBuilder.UseOracle(optionsBuilder =>
                 {
                       _ = optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                   });
                };
            }

            var addContext = addOracleDatabaseDbContext.MakeGenericMethod(contextFactoryRequest.ContextType);
            addContext.Invoke(null,
            [
               builder, contextFactoryRequest.ConnectionName, contextFactoryRequest.ConfigureSettings, contextFactoryRequest.ConfigureDbContextOptions
            ]);
        }

        return new DataContextFactory(builder.Services.BuildServiceProvider());
    }

    /// <summary>
    /// For Read/Write workloads where all operations will execute inside a single transaction
    /// </summary>
    public async Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProfitSharingDbContext>();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await func(context);

            // Commit the transaction when all operations are done
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (Exception)
        {
            // Roll back the transaction if any operation fails
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// For read only workloads. This should not be mixed with Read/Write workloads in the same method as a matter of best
    /// practice.
    /// More information can be found here: https://docs.microsoft.com/en-us/azure/azure-sql/database/read-scale-out
    /// </summary>
    public async Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProfitSharingReadOnlyDbContext>();
        return await func(dbContext);
    }
}
