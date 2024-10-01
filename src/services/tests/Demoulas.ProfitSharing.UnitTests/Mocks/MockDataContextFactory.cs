using System.Reflection;
using System.Threading;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.NotOwned;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.NotOwned;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Fakes;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Mocks;

public sealed class MockDataContextFactory : IProfitSharingDataContextFactory
{
    private readonly Mock<ProfitSharingDbContext> _profitSharingDbContext;
    private readonly Mock<ProfitSharingReadOnlyDbContext> _profitSharingReadOnlyDbContext;
    private readonly Mock<DemoulasCommonDataContext> _storeInfoDbContext;

    private MockDataContextFactory()
    {
        _profitSharingDbContext = new Mock<ProfitSharingDbContext>();
        _profitSharingDbContext.Setup(ctx => ctx.SaveChangesAsync(true, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _profitSharingReadOnlyDbContext = new Mock<ProfitSharingReadOnlyDbContext>();
        _storeInfoDbContext = new Mock<DemoulasCommonDataContext>();

       
        List<Country>? countries = new CountryFaker().Generate(10);
        Mock<DbSet<Country>> mockCountry = countries.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Countries).Returns(mockCountry.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Countries).Returns(mockCountry.Object);

        List<PayClassification>? payClassifications = new PayClassificationFaker().Generate(500);
        Mock<DbSet<PayClassification>> mockPayClassifications = payClassifications.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.PayClassifications).Returns(mockPayClassifications.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.PayClassifications).Returns(mockPayClassifications.Object);

        var profitCodes = new ProfitCodeFaker().Generate(10);
        var mockProfitCodes = profitCodes.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);

        var taxCodes = new TaxCodeFaker().Generate(10);
        var mockTaxCodes = taxCodes.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.TaxCodes).Returns(mockTaxCodes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.TaxCodes).Returns(mockTaxCodes.Object);

        List<Demographic>? demographics = new DemographicFaker().Generate(500);
        

        var profitDetails = new ProfitDetailFaker(demographics).Generate(demographics.Count * 5);
        var mockProfitDetails = profitDetails.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);

        List<Beneficiary>? beneficiaries = new BeneficiaryFaker().Generate(5_000);

        List<PayProfit>? profits = new PayProfitFaker(demographics).Generate(demographics.Count);
        
        foreach (PayProfit payProfit in profits)
        {
            demographics.Find(d=> d.OracleHcmId == payProfit.OracleHcmId)?.PayProfits.Add(payProfit);
        }


        Mock<DbSet<Beneficiary>> mockBeneficiaries = beneficiaries.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);

        Mock<DbSet<PayProfit>> mockProfits = profits.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.PayProfits).Returns(mockProfits.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.PayProfits).Returns(mockProfits.Object);

        Mock<DbSet<Demographic>> mockDemographic = demographics.AsQueryable().BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Demographics).Returns(mockDemographic.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Demographics).Returns(mockDemographic.Object);

        Mock<DbSet<CaldarRecord>>? mockCalendar = CaldarRecordSeeder.Records.AsQueryable().BuildMockDbSet();
        _profitSharingReadOnlyDbContext.Setup(m => m.CaldarRecords).Returns(mockCalendar.Object);


        _profitSharingReadOnlyDbContext.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new NotImplementedException("This is a read only context, saving changes is not allowed."));
        _profitSharingDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(token =>
            {
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }
                return Task.FromResult(1); // Return some result for non-canceled token
            });
    }

    public static IProfitSharingDataContextFactory InitializeForTesting()
    {
        return new MockDataContextFactory();
    }

    /// <summary>
    /// For Read/Write workloads where all operations will execute inside a single transaction
    /// </summary>
    public async Task UseWritableContext(Func<ProfitSharingDbContext, Task> func, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await func.Invoke(_profitSharingDbContext.Object);
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
            return await func.Invoke(_profitSharingDbContext.Object);
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
    /// For read only workloads. This should not be mixed with Read/Write workloads in the same method as a matter of best
    /// practice.
    /// More information can be found here: https://docs.microsoft.com/en-us/azure/azure-sql/database/read-scale-out
    /// </summary>
    public async Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func)
    {
        try
        {
            return await func.Invoke(_profitSharingReadOnlyDbContext.Object);
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
            return await func.Invoke(_storeInfoDbContext.Object);
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
