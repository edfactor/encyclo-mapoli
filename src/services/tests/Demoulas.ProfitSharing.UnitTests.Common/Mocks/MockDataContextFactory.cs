using System.Reflection;
using Demoulas.Common.Data.Services.Entities.Contexts;
using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping.Data;
using Demoulas.Common.Data.Services.Entities.Entities;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.UnitTests.Common.Common;
using Demoulas.ProfitSharing.UnitTests.Common.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
using Department = Demoulas.ProfitSharing.Data.Entities.Department;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

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

        // Assuming you have a DbContext type called ProfitSharingReadOnlyDbContext
        var mockContext = new Mock<ProfitSharingReadOnlyDbContext>( /* options if needed */ );
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockContext.Object);

        // Setup the ProviderName property to return the in-memory provider value
        mockDatabaseFacade.SetupGet(db => db.ProviderName)
            .Returns("Microsoft.EntityFrameworkCore.InMemory");

        // Make sure that when your context.Database is accessed, it returns our mocked DatabaseFacade
        mockContext.SetupGet(ctx => ctx.Database)
            .Returns(mockDatabaseFacade.Object);


        List<Country>? countries = new CountryFaker().Generate(10);
        Mock<DbSet<Country>> mockCountry = countries.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Countries).Returns(mockCountry.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Countries).Returns(mockCountry.Object);

        List<Navigation>? navigations = new NavigationFaker().DummyNavigationData();
        Mock<DbSet<Navigation>> mockNavigation = navigations.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Navigations).Returns(mockNavigation.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Navigations).Returns(mockNavigation.Object);

        List<NavigationStatus>? navigationStatus = new NavigationStatusFaker().DummyNavigationStatus();
        Mock<DbSet<NavigationStatus>> mockNavigationStatus = navigationStatus.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.NavigationStatuses).Returns(mockNavigationStatus.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.NavigationStatuses).Returns(mockNavigationStatus.Object);


       


        List<PayClassification>? payClassifications = new PayClassificationFaker().Generate(500);
        Mock<DbSet<PayClassification>> mockPayClassifications = payClassifications.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.PayClassifications).Returns(mockPayClassifications.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.PayClassifications).Returns(mockPayClassifications.Object);

        var profitCodes = new ProfitCodeFaker().Generate(10);
        var mockProfitCodes = profitCodes.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);

        var taxCodes = new TaxCodeFaker().Generate(10);
        var mockTaxCodes = taxCodes.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.TaxCodes).Returns(mockTaxCodes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.TaxCodes).Returns(mockTaxCodes.Object);

        var employmentTypes = new List<EmploymentType>()
        {
            new EmploymentType() {Id=EmploymentType.Constants.FullTimeAccruedPaidHolidays,Name=EmploymentType.Constants.FullTimeAccruedPaidHolidays.ToString() },
            new EmploymentType() {Id=EmploymentType.Constants.FullTimeEightPaidHolidays,Name=EmploymentType.Constants.FullTimeEightPaidHolidays.ToString() },
            new EmploymentType() {Id=EmploymentType.Constants.FullTimeStraightSalary,Name=EmploymentType.Constants.FullTimeStraightSalary.ToString() },
            new EmploymentType() {Id=EmploymentType.Constants.PartTime,Name=EmploymentType.Constants.PartTime.ToString() }
        };
        var mockEmploymentTypes = employmentTypes.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.EmploymentTypes).Returns(mockEmploymentTypes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.EmploymentTypes).Returns(mockEmploymentTypes.Object);

        List<Demographic>? demographics = new DemographicFaker().Generate(500);
        List<DemographicHistory>? demographicHistories = new DemographicHistoryFaker(demographics).Generate(demographics.Count);

        var profitDetails = new ProfitDetailFaker(demographics).Generate(demographics.Count * 5);
        var mockProfitDetails = profitDetails.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);

        List<Beneficiary>? beneficiaries = new BeneficiaryFaker(demographics).Generate(demographics.Count * 5);
        List<PayProfit>? profits = new PayProfitFaker(demographics).Generate(demographics.Count * 3);

        foreach (PayProfit payProfit in profits)
        {
            demographics.Find(d => d.Id == payProfit.DemographicId)?.PayProfits.Add(payProfit);
        }

        List<ParticipantTotal> participantTotals = new ParticipantTotalFaker(demographics,beneficiaries).Generate(demographics.Count + beneficiaries.Count);
        Constants.FakeParticipantTotals = participantTotals.BuildMockDbSet();

        List<ParticipantTotalVestingBalance> participantTotalVestingBalances = new ParticipantTotalVestingBalanceFaker(demographics, beneficiaries).Generate(demographics.Count + beneficiaries.Count);
        Constants.FakeParticipantTotalVestingBalances = participantTotalVestingBalances.BuildMockDbSet();

        List<ParticipantTotal> etvaBalances = new ParticipantEtvaTotalFaker(profitDetails).Generate(profitDetails.Count);
        Constants.FakeEtvaTotals = etvaBalances.BuildMockDbSet();

        var profitShareTotal = new ProfitShareTotalFaker().Generate();
        Constants.ProfitShareTotals = (new List<ProfitShareTotal>() { profitShareTotal }).BuildMockDbSet();
       


        List<FrozenState>? frozenStates = new FrozenStateFaker().Generate(1);
        List<NavigationTracking>? navigationTrackings = new NavigationTrackingFaker().Generate(1);

        Mock<DbSet<Beneficiary>> mockBeneficiaries = beneficiaries.BuildMockDbSet();
        Mock<DbSet<BeneficiaryContact>> mockBeneficiaryContacts =
            beneficiaries.Where(b => b.Contact != null).Select(b => b.Contact!).ToList().BuildMockDbSet();
        
        _profitSharingDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _profitSharingDbContext.Setup(m => m.BeneficiaryContacts).Returns(mockBeneficiaryContacts.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.BeneficiaryContacts).Returns(mockBeneficiaryContacts.Object);

        List<Beneficiary>? beneficiaryList = new BeneficiaryListFaker().GetDummyBeneficiary();
        Mock<DbSet<Beneficiary>> mockBeneficiaryList = beneficiaryList.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaryList.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaryList.Object);

        Mock<DbSet<PayProfit>> mockProfits = profits.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.PayProfits).Returns(mockProfits.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.PayProfits).Returns(mockProfits.Object);

        Mock<DbSet<Demographic>> mockDemographic = demographics.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Demographics).Returns(mockDemographic.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Demographics).Returns(mockDemographic.Object);

        Mock<DbSet<DemographicHistory>> mockDemographicHistories = demographicHistories.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.DemographicHistories).Returns(mockDemographicHistories.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.DemographicHistories).Returns(mockDemographicHistories.Object);

        Mock<DbSet<AccountingPeriod>>? mockCalendar = CaldarRecordSeeder.Records.ToList().BuildMockDbSet();
        _profitSharingReadOnlyDbContext.Setup(m => m.AccountingPeriods).Returns(mockCalendar.Object);

        Mock<DbSet<FrozenState>> mockFrozenStates = frozenStates.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.FrozenStates).Returns(mockFrozenStates.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.FrozenStates).Returns(mockFrozenStates.Object);
        Mock<DbSet<NavigationTracking>> mockNavigationTrackings = navigationTrackings.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.NavigationTrackings).Returns(mockNavigationTrackings.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.NavigationTrackings).Returns(mockNavigationTrackings.Object);

        _profitSharingDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _profitSharingDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(token =>
            {
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(token);
                }

                return Task.FromResult(1); // Return some result for non-canceled token
            });

        // create departments from Constants
        List<Department> departments = typeof(Department.Constants)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(f => f.FieldType == typeof(byte))
        .Select(f => new Department { Id = (byte)f.GetValue(null)!, Name = f.Name })
        .ToList();

        Mock<DbSet<Department>> mockDepartments = departments.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Departments).Returns(mockDepartments.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Departments).Returns(mockDepartments.Object);

        var distributions = new DistributionFaker().Generate(500);
        var mockDistributions = distributions.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Distributions).Returns(mockDistributions.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Distributions).Returns(mockDistributions.Object);

        var distributionFrequencies = new List<DistributionFrequency>()
        {
            new DistributionFrequency() {Id=DistributionFrequency.Constants.Hardship,Name=DistributionFrequency.Constants.Hardship.ToString() },
            new DistributionFrequency() {Id=DistributionFrequency.Constants.PayDirect,Name=DistributionFrequency.Constants.PayDirect.ToString() },
            new DistributionFrequency() {Id=DistributionFrequency.Constants.RolloverDirect,Name=DistributionFrequency.Constants.RolloverDirect.ToString() },
            new DistributionFrequency() {Id=DistributionFrequency.Constants.Monthly,Name=DistributionFrequency.Constants.Monthly.ToString() },
            new DistributionFrequency() {Id=DistributionFrequency.Constants.Quarterly,Name=DistributionFrequency.Constants.Quarterly.ToString() },
            new DistributionFrequency() {Id=DistributionFrequency.Constants.Annually,Name=DistributionFrequency.Constants.Annually.ToString() }
        };
        var mockDistributionFrequencies = distributionFrequencies.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.DistributionFrequencies).Returns(mockDistributionFrequencies.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.DistributionFrequencies).Returns(mockDistributionFrequencies.Object);

        var distributionStatuses = new List<DistributionStatus>()
        {
            new DistributionStatus() {Id=DistributionStatus.Constants.ManualCheck,Name=DistributionStatus.Constants.ManualCheck.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.PurgeRecord,Name=DistributionStatus.Constants.PurgeRecord.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.RequestOnHold,Name=DistributionStatus.Constants.RequestOnHold.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.Override,Name=DistributionStatus.Constants.Override.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.PaymentMade,Name=DistributionStatus.Constants.PaymentMade.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.OkayToPay,Name=DistributionStatus.Constants.OkayToPay.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.PurgeAllRecordsForSsn,Name=DistributionStatus.Constants.PurgeAllRecordsForSsn.ToString() },
            new DistributionStatus() {Id=DistributionStatus.Constants.PurgeAllRecordsForSsn2,Name=DistributionStatus.Constants.PurgeAllRecordsForSsn2.ToString() },
        };
        var mockDistributionStatuses = distributionStatuses.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.DistributionStatuses).Returns(mockDistributionStatuses.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.DistributionStatuses).Returns(mockDistributionStatuses.Object);

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
            return await action.Invoke(_profitSharingDbContext.Object, null!).ConfigureAwait(false);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
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
