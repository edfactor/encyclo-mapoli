using System.Reflection;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

public class ScenarioDataContextFactory : IProfitSharingDataContextFactory
{
    public Mock<ProfitSharingDbContext> ProfitSharingDbContext { get; }
    public Mock<ProfitSharingReadOnlyDbContext> ProfitSharingReadOnlyDbContext { get; }
    public Mock<DemoulasCommonDataContext> StoreInfoDbContext { get; }


    public ScenarioDataContextFactory()
    {
        ProfitSharingDbContext = new Mock<ProfitSharingDbContext>();
        ProfitSharingReadOnlyDbContext = new Mock<ProfitSharingReadOnlyDbContext>();
        StoreInfoDbContext = new Mock<DemoulasCommonDataContext>();
    }

    /// <summary>
    /// For Read/Write workloads where all operations will execute inside a single transaction
    /// </summary>
    public async Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await func.Invoke(ProfitSharingDbContext.Object);
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }

    /// <summary>
    /// For Read/Write workloads where all operations will execute inside a single transaction
    /// </summary>
    public async Task<T> UseWritableContext<T>(Func<ProfitSharingDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await func.Invoke(ProfitSharingDbContext.Object);
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }

    public async Task<T> UseWritableContextAsync<T>(
        Func<ProfitSharingDbContext, IDbContextTransaction, Task<T>> action,
        CancellationToken cancellationToken)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await action.Invoke(ProfitSharingDbContext.Object, null!).ConfigureAwait(false);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    public async Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func, CancellationToken cancellationToken = default)   
    {
        try
        {
            return await func.Invoke(ProfitSharingReadOnlyDbContext.Object);
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }


    public async Task<T> UseStoreInfoContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
    {
        try
        {
            return await func.Invoke(StoreInfoDbContext.Object);
        }
        catch (TargetInvocationException ex)
        {
            switch (ex.InnerException)
            {
                case null:
                    throw;
                default:
                    throw ex.InnerException;
            }
        }
    }
}
