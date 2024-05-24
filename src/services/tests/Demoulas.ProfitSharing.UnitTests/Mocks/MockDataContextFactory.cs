using System.Diagnostics;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Demoulas.ProfitSharing.IntegrationTests.Mocks;

public sealed class MockDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    private MockDataContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static MockDataContextFactory InitializeForTesting(IServiceCollection serviceCollection, string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        _ = serviceCollection.AddDbContextPool<ProfitSharingDbContext>(builder =>
        {
            _ = builder.UseOracle(connectionString);
            _ = builder.EnableSensitiveDataLogging(Debugger.IsAttached);
        });
        _ = serviceCollection.AddDbContextPool<ProfitSharingReadOnlyDbContext>(builder =>
        {
            _ = builder.EnableSensitiveDataLogging(Debugger.IsAttached);
            _ = builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            _ = builder.UseOracle(connectionString, optionsBuilder =>
            {
                _ = optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });


        return new MockDataContextFactory(serviceCollection.BuildServiceProvider());
    }

    /// <summary>
    /// For Read/Write workloads where all operations will execute inside a single transaction
    /// </summary>
    public async Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ProfitSharingDbContext>();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await func(context);

            // Commit the transaction when all operations are done
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Roll back the transaction if any operation fails
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
