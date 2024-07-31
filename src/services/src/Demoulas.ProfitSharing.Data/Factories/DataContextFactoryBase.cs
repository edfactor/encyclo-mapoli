using System.Diagnostics;
using System.Reflection;
using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.Common.Data.Contexts.DTOs.Context;
using EntityFramework.Exceptions.Oracle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Data.Factories;

public abstract class DataContextFactoryBase<TDbContext, TReadOnlyDbContext> where TDbContext : OracleDbContext<TDbContext> where TReadOnlyDbContext : ReadOnlyOracleDbContext<TReadOnlyDbContext>
{
    protected IServiceProvider ServiceProvider { get; }

    protected ILogger Logger { get; }

    protected DataContextFactoryBase(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ILoggerFactory factory = serviceProvider.GetRequiredService<ILoggerFactory>();
        Logger = factory.CreateLogger<DataContextFactoryBase<TDbContext, TReadOnlyDbContext>>();
    }

    protected static IServiceCollection InitializeContexts(IHostApplicationBuilder builder, IEnumerable<ContextFactoryRequest> contextFactoryRequests) 
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
            _ = dbBuilder.UseUpperCaseNamingConvention();
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
            _ = addContext.Invoke(null,
            [
                builder, contextFactoryRequest.ConnectionName, contextFactoryRequest.ConfigureSettings, contextFactoryRequest.ConfigureDbContextOptions
            ]);

            MethodInfo genericMethodInfo = enrichOracleDatabaseDbContext.MakeGenericMethod(contextFactoryRequest.ContextType);
            genericMethodInfo.Invoke(null, new object[] { builder, null! });
        }

        return builder.Services;
    }

    #region Use Writable Context

    public virtual Task UseWritableContext(Func<TDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        return UseWritableContextInternal(async context =>
        {
            await func(context);
            return Task.CompletedTask;
        }, cancellationToken);
    }

    public virtual Task<T> UseWritableContext<T>(Func<TDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        return UseWritableContextInternal(func, cancellationToken);
    }

    protected virtual async Task<T> UseWritableContextInternal<T>(Func<TDbContext, Task<T>> func, CancellationToken cancellationToken)
    {
        using (Logger.BeginScope("Writable DB Operation"))
        {
            await using AsyncServiceScope scope = ServiceProvider.CreateAsyncScope();
            TDbContext context = scope.ServiceProvider.GetRequiredService<TDbContext>();
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
                    Logger.LogError(ex, "Failed to execute operation, rolling back : {message}", ex.Message);
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
    public async Task<T> UseReadOnlyContext<T>(Func<TReadOnlyDbContext, Task<T>> func)
    {
        using (Logger.BeginScope("ReadOnly DB Operation"))
        {
            try
            {
                await using AsyncServiceScope scope = ServiceProvider.CreateAsyncScope();
                TReadOnlyDbContext dbContext = scope.ServiceProvider.GetRequiredService<TReadOnlyDbContext>();
                return await func(dbContext);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute readonly query");
                throw;
            }

        }
    }
}
