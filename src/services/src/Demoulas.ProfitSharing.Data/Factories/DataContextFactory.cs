using System.Diagnostics;
using System.Reflection;
using Aspire.Oracle.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.StoreInfo.Entities.Contexts;
using EntityFramework.Exceptions.Oracle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Data.Factories;

public sealed class DataContextFactory : IProfitSharingDataContextFactory
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    private DataContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ILoggerFactory factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = factory.CreateLogger<DataContextFactory>();
    }

    public static IProfitSharingDataContextFactory Initialize(IHostApplicationBuilder builder, IEnumerable<ContextFactoryRequest> contextFactoryRequests)
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
            if (Debugger.IsAttached)
            {
                _ = dbBuilder.LogTo(s => Debug.WriteLine(s));
            }

            _ = dbBuilder.UseExceptionProcessor();
            _ = dbBuilder.EnableSensitiveDataLogging(Debugger.IsAttached);
            _ = dbBuilder.EnableDetailedErrors(Debugger.IsAttached);
            _ = dbBuilder.UseOracle(optionsBuilder => _ = optionsBuilder.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21));
        }

        MethodInfo addOracleDatabaseDbContext = typeof(AspireOracleEFCoreExtensions)
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
                        _ = optionsBuilder.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21);
                        _ = optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
                };
            }

            MethodInfo addContext = addOracleDatabaseDbContext.MakeGenericMethod(contextFactoryRequest.ContextType);
            addContext.Invoke(null,
            [
                builder, contextFactoryRequest.ConnectionName, contextFactoryRequest.ConfigureSettings, contextFactoryRequest.ConfigureDbContextOptions
            ]);

            MethodInfo genericMethodInfo = enrichOracleDatabaseDbContext.MakeGenericMethod(contextFactoryRequest.ContextType);
            genericMethodInfo.Invoke(null, new object[] { builder, null! });
        }


        return new DataContextFactory(builder.Services.BuildServiceProvider());
    }

    #region Use Writable Context

    public Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        return UseWritableContextInternal(async context =>
        {
            await func(context);
            return Task.CompletedTask;
        }, cancellationToken);
    }

    public Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        return UseWritableContextInternal(func, cancellationToken);
    }

    private async Task<T> UseWritableContextInternal<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("Writable DB Operation"))
        {
            await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
            ProfitSharingDbContext context = scope.ServiceProvider.GetRequiredService<ProfitSharingDbContext>();
            var executionStrategy = context.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    T result = await func(context);

                    // Commit the transaction when all operations are done
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch (Exception ex)
                {
                    // Roll back the transaction if any operation fails
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Failed to execute operation, rolling back");
                    throw;
                }
            });
        }
    }

    #endregion


    /// <summary>
    /// For read only workloads. This should not be mixed with Read/Write workloads in the same method as a matter of best
    /// practice.
    /// More information can be found here: https://docs.microsoft.com/en-us/azure/azure-sql/database/read-scale-out
    /// </summary>
    public async Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        using (_logger.BeginScope("ReadOnly DB Operation"))
        {
            try
            {
                await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                ProfitSharingReadOnlyDbContext dbContext = scope.ServiceProvider.GetRequiredService<ProfitSharingReadOnlyDbContext>();
                return await func(dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute readonly query");
                throw;
            }

        }
    }

    /// <summary>
    /// Context to access Store related data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public async Task<T> UseStoreInfoContext<T>(Func<StoreInfoDbContext, Task<T>> func)
    {
        using (_logger.BeginScope("Store Info DB Operation"))
        {
            try
            {
                await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
                StoreInfoDbContext dbContext = scope.ServiceProvider.GetRequiredService<StoreInfoDbContext>();
                return await func(dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute readonly Store Info query");
                throw;
            }
        }
    }
}
