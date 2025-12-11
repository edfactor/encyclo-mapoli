using System.Reflection;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

public class ScenarioDataContextFactory : IProfitSharingDataContextFactory
{
    public Mock<ProfitSharingDbContext> ProfitSharingDbContext { get; }
    public Mock<ProfitSharingReadOnlyDbContext> ProfitSharingReadOnlyDbContext { get; }
    public Mock<DemoulasCommonWarehouseContext> StoreInfoDbContext { get; }


    public ScenarioDataContextFactory()
    {
        ProfitSharingDbContext = new Mock<ProfitSharingDbContext>();
        ProfitSharingReadOnlyDbContext = new Mock<ProfitSharingReadOnlyDbContext>();
        StoreInfoDbContext = new Mock<DemoulasCommonWarehouseContext>();
    }

    /// <summary>
    /// Creates a factory with mocked DbSets populated with test data.
    /// </summary>
    /// <param name="demographics">Demographics to seed</param>
    /// <param name="histories">Demographic histories to seed</param>
    /// <param name="audits">Demographic sync audits to seed</param>
    /// <param name="beneficiaryContacts">Beneficiary contacts to seed</param>
    /// <param name="profitDetails">Profit details to seed</param>
    /// <returns>Configured factory ready for testing</returns>
    public static ScenarioDataContextFactory Create(
        List<Demographic>? demographics = null,
        List<DemographicHistory>? histories = null,
        List<DemographicSyncAudit>? audits = null,
        List<BeneficiaryContact>? beneficiaryContacts = null,
        List<ProfitDetail>? profitDetails = null)
    {
        var factory = new ScenarioDataContextFactory();

        // Setup Demographics DbSet
        var demographicsList = demographics ?? new List<Demographic>();
        var demographicsMock = demographicsList.BuildMockDbSet();
        factory.ProfitSharingDbContext
            .Setup(x => x.Demographics)
            .Returns(demographicsMock.Object);
        factory.ProfitSharingReadOnlyDbContext
            .Setup(x => x.Demographics)
            .Returns(demographicsMock.Object);

        // Setup DemographicHistories DbSet
        var historiesList = histories ?? new List<DemographicHistory>();
        var historiesMock = historiesList.BuildMockDbSet();
        factory.ProfitSharingDbContext
            .Setup(x => x.DemographicHistories)
            .Returns(historiesMock.Object);

        // Setup DemographicSyncAudit DbSet
        var auditsList = audits ?? new List<DemographicSyncAudit>();
        var auditsMock = auditsList.BuildMockDbSet();
        factory.ProfitSharingDbContext
            .Setup(x => x.DemographicSyncAudit)
            .Returns(auditsMock.Object);

        // Setup BeneficiaryContacts DbSet
        var beneficiaryContactsList = beneficiaryContacts ?? new List<BeneficiaryContact>();
        var beneficiaryContactsMock = beneficiaryContactsList.BuildMockDbSet();
        factory.ProfitSharingDbContext
            .Setup(x => x.BeneficiaryContacts)
            .Returns(beneficiaryContactsMock.Object);

        // Setup ProfitDetails DbSet
        var profitDetailsList = profitDetails ?? new List<ProfitDetail>();
        var profitDetailsMock = profitDetailsList.BuildMockDbSet();
        factory.ProfitSharingDbContext
            .Setup(x => x.ProfitDetails)
            .Returns(profitDetailsMock.Object);

        // Setup SaveChangesAsync to return success
        factory.ProfitSharingDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken ct) => demographicsList.Count + historiesList.Count);

        return factory;
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

#pragma warning disable RCS1229
    public Task UseReadOnlyContext(Func<ProfitSharingReadOnlyDbContext, Task> func, CancellationToken cancellationToken = new CancellationToken())
#pragma warning restore RCS1229
    {
        try
        {
            return func.Invoke(ProfitSharingReadOnlyDbContext.Object);
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

    public async Task<T> UseWarehouseContext<T>(Func<DemoulasCommonWarehouseContext, Task<T>> func)
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
