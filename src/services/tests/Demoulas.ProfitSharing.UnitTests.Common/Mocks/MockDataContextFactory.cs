using System.Diagnostics;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

    /// <summary>
    /// Creates a mock DbSet that uses a backing list to persist Add/Remove operations
    /// </summary>
    private static Mock<DbSet<T>> BuildMockDbSetWithBackingList<T>(List<T> data) where T : class
    {
        // Start with IQueryable-enabled DbSet backed by the list
        var mockSet = data.BuildMockDbSet();

        // Auto-increment counter for ID generation (for entities with Id property)
        var nextId = data.Count > 0 ? GetMaxId(data) + 1 : 1;

        // Ensure Add/Remove operations mutate the backing list
        mockSet.Setup(s => s.Add(It.IsAny<T>()))
            .Callback<T>(e =>
            {
                SetIdIfNeeded(e, nextId++);
                data.Add(e);
            });
        mockSet.Setup(s => s.AddRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(range =>
            {
                foreach (var e in range)
                {
                    SetIdIfNeeded(e, nextId++);
                    data.Add(e);
                }
            });
        mockSet.Setup(s => s.Remove(It.IsAny<T>()))
            .Callback<T>(e => data.Remove(e));
        mockSet.Setup(s => s.RemoveRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(range =>
            {
                foreach (var e in range.ToList())
                {
                    data.Remove(e);
                }
            });

        // Async adds used by service (e.g., DemographicSyncAudit.AddAsync)
        mockSet.Setup(s => s.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback<T, CancellationToken>((e, _) =>
            {
                SetIdIfNeeded(e, nextId++);
                data.Add(e);
            })
            .Returns<T, CancellationToken>((e, _) =>
                ValueTask.FromResult((EntityEntry<T>)null!));

        return mockSet;
    }

    /// <summary>
    /// Helper method to set ID property for entities that have one
    /// </summary>
    private static void SetIdIfNeeded<T>(T entity, long id) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.CanWrite)
        {
            // Convert the id to the appropriate type
            var targetType = idProperty.PropertyType;
            if (targetType == typeof(long))
            {
                idProperty.SetValue(entity, id);
            }
            else if (targetType == typeof(int))
            {
                idProperty.SetValue(entity, (int)id);
            }
            else if (targetType == typeof(short))
            {
                idProperty.SetValue(entity, (short)id);
            }
        }
    }

    /// <summary>
    /// Helper method to get the maximum ID from existing entities
    /// </summary>
    private static long GetMaxId<T>(List<T> data) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            return 0;
        }

        long maxId = 0;
        foreach (var item in data)
        {
            var value = idProperty.GetValue(item);
            if (value != null)
            {
                if (value is long longVal && longVal > maxId)
                {
                    maxId = longVal;
                }
                else if (value is int intVal && intVal > maxId)
                {
                    maxId = intVal;
                }
                else if (value is short shortVal && shortVal > maxId)
                {
                    maxId = shortVal;
                }
            }
        }
        return maxId;
    }

    internal MockDataContextFactory()
    {
        _profitSharingDbContext = new Mock<ProfitSharingDbContext>();
        _profitSharingDbContext.Setup(ctx => ctx.SaveChangesAsync(true, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _profitSharingReadOnlyDbContext = new Mock<ProfitSharingReadOnlyDbContext>();
        _storeInfoDbContext = new Mock<DemoulasCommonDataContext>();

        // Setup Database facade for the read-only context
        var mockDatabaseFacade = new Mock<DatabaseFacade>(_profitSharingReadOnlyDbContext.Object);

        // Setup the ProviderName property to return the in-memory provider value
        mockDatabaseFacade.SetupGet(db => db.ProviderName)
            .Returns("Microsoft.EntityFrameworkCore.InMemory");

        // Make sure that when your context.Database is accessed, it returns our mocked DatabaseFacade
        _profitSharingReadOnlyDbContext.SetupGet(ctx => ctx.Database)
            .Returns(mockDatabaseFacade.Object);


        var dataGenTimer = Stopwatch.StartNew();
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

        List<NavigationRole>? navigationRoles = new NavigationFaker().GetAllNavigationRoles();
        Mock<DbSet<NavigationRole>> mockNavigationRoles = navigationRoles.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.NavigationRoles).Returns(mockNavigationRoles.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.NavigationRoles).Returns(mockNavigationRoles.Object);

        List<PayClassification>? payClassifications = new PayClassificationFaker().Generate(250);
        Mock<DbSet<PayClassification>> mockPayClassifications = payClassifications.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.PayClassifications).Returns(mockPayClassifications.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.PayClassifications).Returns(mockPayClassifications.Object);

        var profitCodes = new ProfitCodeFaker().Generate(10);
        var mockProfitCodes = profitCodes.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);

        // Use actual TaxCode constants instead of faker for predictable test data
        var taxCodesList = new List<TaxCode>()
        {
            TaxCode.Constants.Unknown,
            TaxCode.Constants.EarlyDistributionNoException,
            TaxCode.Constants.EarlyDistributionExceptionApplies,
            TaxCode.Constants.Disability,
            TaxCode.Constants.Death,
            TaxCode.Constants.ProhibitedTransaction,
            TaxCode.Constants.Section1035Exchange,
            TaxCode.Constants.NormalDistribution,
            TaxCode.Constants.ExcessContributionsEarningsDeferrals8,
            TaxCode.Constants.PS58Cost,
            TaxCode.Constants.QualifiesFor5Or10YearAveraging,
            TaxCode.Constants.QualifiesForDeathBenefitExclusion,
            TaxCode.Constants.QualifiesForBothAandB,
            TaxCode.Constants.ExcessContributionsEarningsDeferralsD,
            TaxCode.Constants.ExcessAnnualAdditionsSection415,
            TaxCode.Constants.CharitableGiftAnnuity,
            TaxCode.Constants.DirectRolloverToIRA,
            TaxCode.Constants.DirectRolloverToPlanOrAnnuity,
            TaxCode.Constants.ExcessContributionsEarningsDeferralsP
        };
        var mockTaxCodesList = taxCodesList.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.TaxCodes).Returns(mockTaxCodesList.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.TaxCodes).Returns(mockTaxCodesList.Object);

        var stateTaxes = new List<StateTax>()
        {
            new StateTax() { Abbreviation = "NH", Rate = 0.00m, UserModified = "TestUser", DateModified = DateOnly.FromDateTime(DateTime.Today) },
            new StateTax() { Abbreviation = "CA", Rate = 13.30m, UserModified = "TestUser", DateModified = DateOnly.FromDateTime(DateTime.Today) },
            new StateTax() { Abbreviation = "TX", Rate = 0.00m, UserModified = "TestUser", DateModified = DateOnly.FromDateTime(DateTime.Today) },
            new StateTax() { Abbreviation = "NY", Rate = 8.82m, UserModified = "TestUser", DateModified = DateOnly.FromDateTime(DateTime.Today) }
        };
        var mockStateTaxes = stateTaxes.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.StateTaxes).Returns(mockStateTaxes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.StateTaxes).Returns(mockStateTaxes.Object);

        var states = new List<State>()
        {
            new State() { Abbreviation = "MA", Name = "Massachusetts" },
            new State() { Abbreviation = "NH", Name = "New Hampshire" },
            new State() { Abbreviation = "ME", Name = "Maine" },
            new State() { Abbreviation = "CT", Name = "Connecticut" },
            new State() { Abbreviation = "RI", Name = "Rhode Island" },
            new State() { Abbreviation = "VT", Name = "Vermont" },
            new State() { Abbreviation = "NY", Name = "New York" },
            new State() { Abbreviation = "CA", Name = "California" },
            new State() { Abbreviation = "TX", Name = "Texas" }
        };
        var mockStates = states.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.States).Returns(mockStates.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.States).Returns(mockStates.Object);

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

        List<Demographic>? demographics = new DemographicFaker().Generate(250);
        List<DemographicHistory>? demographicHistories = new DemographicHistoryFaker(demographics).Generate(demographics.Count);

        var profitDetails = new ProfitDetailFaker(demographics).Generate(demographics.Count * 4);

        // Add COMMENT_RELATED_STATE values to some profit details for state lookup testing
        var statesToAssign = new[] { "MA", "NH", "ME", "CT", "RI", "VT", "NY", "CA", "TX" };
        for (int i = 0; i < profitDetails.Count; i++)
        {
            if (i % 7 == 0) // Assign state to approximately 1 in 7 records
            {
                profitDetails[i].CommentRelatedState = statesToAssign[i % statesToAssign.Length];
            }
        }

        var mockProfitDetails = BuildMockDbSetWithBackingList(profitDetails);
        _profitSharingDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);

        List<Beneficiary>? beneficiaries = new BeneficiaryFaker(demographics).Generate(demographics.Count * 4);
        List<PayProfit>? profits = new PayProfitFaker(demographics).Generate(demographics.Count * 2);

        foreach (PayProfit payProfit in profits)
        {
            demographics.Find(d => d.Id == payProfit.DemographicId)?.PayProfits.Add(payProfit);
        }

        List<ParticipantTotal> participantTotals = new ParticipantTotalFaker(demographics, beneficiaries).Generate(demographics.Count + beneficiaries.Count);
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

        Mock<DbSet<PayProfit>> mockProfits = BuildMockDbSetWithBackingList(profits);
        _profitSharingDbContext.Setup(m => m.PayProfits).Returns(mockProfits.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.PayProfits).Returns(mockProfits.Object);

        Mock<DbSet<Demographic>> mockDemographic = demographics.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.Demographics).Returns(mockDemographic.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Demographics).Returns(mockDemographic.Object);

        Mock<DbSet<DemographicHistory>> mockDemographicHistories = demographicHistories.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.DemographicHistories).Returns(mockDemographicHistories.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.DemographicHistories).Returns(mockDemographicHistories.Object);

        // Setup FakeSsns - used to exclude fake SSNs from duplicate detection
        List<FakeSsn>? fakeSsns = new List<FakeSsn>();
        Mock<DbSet<FakeSsn>> mockFakeSsns = fakeSsns.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.FakeSsns).Returns(mockFakeSsns.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.FakeSsns).Returns(mockFakeSsns.Object);

        Mock<DbSet<AccountingPeriod>>? mockCalendar = CaldarRecordSeeder.Records.ToList().BuildMockDbSet();
        _storeInfoDbContext.Setup(m => m.AccountingPeriods).Returns(mockCalendar.Object);

        Mock<DbSet<FrozenState>> mockFrozenStates = frozenStates.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.FrozenStates).Returns(mockFrozenStates.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.FrozenStates).Returns(mockFrozenStates.Object);
        Mock<DbSet<NavigationTracking>> mockNavigationTrackings = navigationTrackings.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.NavigationTrackings).Returns(mockNavigationTrackings.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.NavigationTrackings).Returns(mockNavigationTrackings.Object);

        // Setup Enrollments with all constant values
        var enrollments = new List<Enrollment>
        {
            new() { Id = Enrollment.Constants.NotEnrolled, Name = "Not Enrolled" },
            new() { Id = Enrollment.Constants.OldVestingPlanHasContributions, Name = "Old vesting plan has Contributions (7 years to full vesting)" },
            new() { Id = Enrollment.Constants.NewVestingPlanHasContributions, Name = "New vesting plan has Contributions (6 years to full vesting)" },
            new() { Id = Enrollment.Constants.OldVestingPlanHasForfeitureRecords, Name = "Old vesting plan has Forfeiture records" },
            new() { Id = Enrollment.Constants.NewVestingPlanHasForfeitureRecords, Name = "New vesting plan has Forfeiture records" },
            new() { Id = Enrollment.Constants.Import_Status_Unknown, Name = "Previous years enrollment is unknown. (History not previously tracked)" }
        };
        Mock<DbSet<Enrollment>> mockEnrollments = enrollments.BuildMockDbSet();
        _profitSharingReadOnlyDbContext.Setup(m => m.Enrollments).Returns(mockEnrollments.Object);

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

        // Create distribution payees first
        var distributionPayees = new List<DistributionPayee>
        {
            new DistributionPayee
            {
                Id = 1,
                Ssn = 123456789,
                Name = "John Payee",
                Address = new Address { Street = "123 Main St", City = "Boston", State = "MA", PostalCode = "02101" },
                Memo = "Primary Payee"
            },
            new DistributionPayee
            {
                Id = 2,
                Ssn = 987654321,
                Name = "Jane Payee",
                Address = new Address { Street = "456 Oak Ave", City = "Cambridge", State = "MA", PostalCode = "02142" },
                Memo = "Alternate Payee"
            },
            new DistributionPayee
            {
                Id = 3,
                Ssn = 555123456,
                Name = "Bob Payee",
                Address = new Address { Street = "789 Pine Rd", City = "Somerville", State = "MA", PostalCode = "02145" },
                Memo = "Backup Payee"
            }
        };
        var mockDistributionPayees = distributionPayees.BuildMockDbSet();
        _profitSharingDbContext.Setup(m => m.DistributionPayees).Returns(mockDistributionPayees.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.DistributionPayees).Returns(mockDistributionPayees.Object);

        // Create distribution frequencies
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

        // Create distribution statuses
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

        // Now create distributions with all related entities provided
        var distributions = new DistributionFaker(distributionFrequencies, distributionStatuses, taxCodesList, distributionPayees).Generate(250);
        var mockDistributions = BuildMockDbSetWithBackingList(distributions);
        _profitSharingDbContext.Setup(m => m.Distributions).Returns(mockDistributions.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Distributions).Returns(mockDistributions.Object);

        var commentTypes = new List<CommentType>()
        {
            CommentType.Constants.TransferOut,
            CommentType.Constants.TransferIn,
            CommentType.Constants.QdroOut,
            CommentType.Constants.QdroIn,
            CommentType.Constants.VOnly,
            CommentType.Constants.Forfeit,
            CommentType.Constants.Unforfeit,
            CommentType.Constants.ClassAction,
            CommentType.Constants.Voided,
            CommentType.Constants.Hardship,
            CommentType.Constants.Distribution,
            CommentType.Constants.Payoff,
            CommentType.Constants.Dirpay,
            CommentType.Constants.Rollover,
            CommentType.Constants.RothIra,
            CommentType.Constants.Over64OneYearVested,
            CommentType.Constants.Over64TwoYearsVested,
            CommentType.Constants.Over64ThreeYearsVested,
            CommentType.Constants.Military,
            CommentType.Constants.Other,
            CommentType.Constants.Reversal,
            CommentType.Constants.UndoReversal,
            CommentType.Constants.OneHundredPercentEarnings,
            CommentType.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
            CommentType.Constants.ForfeitClassAction
        };
        var mockCommentTypes = commentTypes.BuildMockDbSet();
        // CommentTypes is read-only, only on ReadOnlyDbContext
        _profitSharingReadOnlyDbContext.Setup(m => m.CommentTypes).Returns(mockCommentTypes.Object);

        // Setup generic Set<T>() method for LookupCache support
        _profitSharingReadOnlyDbContext.Setup(m => m.Set<StateTax>()).Returns(mockStateTaxes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Set<DistributionFrequency>()).Returns(mockDistributionFrequencies.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Set<DistributionStatus>()).Returns(mockDistributionStatuses.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Set<CommentType>()).Returns(mockCommentTypes.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Set<TaxCode>()).Returns(mockTaxCodesList.Object);
        _profitSharingReadOnlyDbContext.Setup(m => m.Set<State>()).Returns(mockStates.Object);
    }

    /// <summary>
    /// For backward compatibility, returns a fresh factory instance per call.
    /// Each test that needs isolation gets its own factory with 6,500+ fresh fake records.
    /// Use this for all test scenarios to prevent state pollution.
    /// </summary>
    public static IProfitSharingDataContextFactory InitializeForTesting()
    {
        return new MockDataContextFactory();
    }

    /// <summary>
    /// INTERNAL: For future optimization only.
    /// Allows returning a cached instance if needed, but not used by default to prevent test pollution.
    /// </summary>
    internal static IProfitSharingDataContextFactory InitializeForTestingCached()
    {
        return MockDbContextFactoryCache.GetOrCreateInstance();
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
    public async Task<T> UseReadOnlyContext<T>(Func<ProfitSharingReadOnlyDbContext, Task<T>> func, CancellationToken cancellationToken = default)
    {
        if (_profitSharingReadOnlyDbContext == null)
        {
            throw new InvalidOperationException("_profitSharingReadOnlyDbContext is null. Mock initialization failed.");
        }

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

    public Task UseReadOnlyContext(Func<ProfitSharingReadOnlyDbContext, Task> func, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }


    public async Task<T> UseWarehouseContext<T>(Func<DemoulasCommonDataContext, Task<T>> func)
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

/// <summary>
/// Static cache holder for MockDataContextFactory to avoid expensive factory recreation.
/// The factory instantiation is expensive (~60-80s) due to 6,500+ fake records generation.
/// Reusing a single instance across all tests provides ~50% test performance improvement.
/// 
/// Thread-safe using double-checked locking pattern.
/// </summary>
internal static class MockDbContextFactoryCache
{
    private static volatile MockDataContextFactory? _cachedInstance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Gets the cached factory instance, creating it if needed.
    /// Subsequent calls return the same instance without recreating it.
    /// </summary>
    public static IProfitSharingDataContextFactory GetOrCreateInstance()
    {
        if (_cachedInstance != null)
        {
            return _cachedInstance;
        }

        lock (_lock)
        {
            if (_cachedInstance == null)
            {
                _cachedInstance = new MockDataContextFactory();
            }
        }

        return _cachedInstance;
    }
}
