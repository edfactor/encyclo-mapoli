
using System.Reflection;
using Demoulas.Common.Data.Services.Contexts.EntityMapping.Data;
using Demoulas.Common.Data.Services.Entities.Entities;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
// Common also has a Department.cs file, so we need to specify the one we want to use
using Department = Demoulas.ProfitSharing.Data.Entities.Department;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

/// <summary>
/// This factory is used to configure a ScenarioDataContextFactory for use in tests.
/// Specifically it helps configure Employees or Beneficiaries in a standard way or helps configure the mock database for a specific test scenario.
/// </summary>
public sealed class ScenarioFactory
{
    private readonly ScenarioDataContextFactory _sdb = new();

    public short ProfitYear { get; set; } = 2024;
    public short ProfitYearPriorYear => (short)(ProfitYear - 1);
    public List<Demographic> Demographics { get; set; } = [];
    public List<PayProfit> PayProfits { get; set; } = [];
    public List<Beneficiary> Beneficiaries { get; set; } = [];
    public List<ProfitDetail> ProfitDetails { get; set; } = [];
    public List<FrozenState> FrozenStates { get; set; } = [];
    public List<DemographicHistory> DemographicHistories { get; set; } = [];
    public List<YearEndUpdateStatus> YearEndUpdateStatuses { get; set; } = [];
    public List<AuditEvent> AuditEvents { get; set; } = [];
    public List<Bank> Banks { get; set; } = [];
    public List<BankAccount> BankAccounts { get; set; } = [];

    // populate ProfitCode dictionary object from the Constants
    public List<ProfitCode> ProfitCodes { get; set; } = typeof(ProfitCode.Constants)
        .GetProperties(BindingFlags.Public | BindingFlags.Static)
        .Select(p => p.GetValue(null))
        .OfType<ProfitCode>().ToList();

    // populate the departments from Constants
    public List<Department> Departments { get; set; } = typeof(Department.Constants)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(f => f.FieldType == typeof(byte))
        .Select(f => new Department { Id = (byte)f.GetValue(null)!, Name = f.Name })
        .ToList();

    // populate TerminationCodes from Constants
    public List<TerminationCode> TerminationCodes { get; set; } = typeof(TerminationCode.Constants)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(f => f.FieldType == typeof(char))
        .Select(f => new TerminationCode { Id = (char)f.GetValue(null)!, Name = f.Name })
        .ToList();

    // RMD factors (IRS Publication 590-B Uniform Lifetime Table) - seed all ages 73-99
    public List<RmdsFactorByAge> RmdsFactorsByAge { get; set; } =
    [
        new RmdsFactorByAge { Age = 73, Factor = 26.5m },
        new RmdsFactorByAge { Age = 74, Factor = 25.5m },
        new RmdsFactorByAge { Age = 75, Factor = 24.6m },
        new RmdsFactorByAge { Age = 76, Factor = 23.7m },
        new RmdsFactorByAge { Age = 77, Factor = 22.9m },
        new RmdsFactorByAge { Age = 78, Factor = 22.0m },
        new RmdsFactorByAge { Age = 79, Factor = 21.1m },
        new RmdsFactorByAge { Age = 80, Factor = 20.2m },
        new RmdsFactorByAge { Age = 81, Factor = 19.4m },
        new RmdsFactorByAge { Age = 82, Factor = 18.5m },
        new RmdsFactorByAge { Age = 83, Factor = 17.7m },
        new RmdsFactorByAge { Age = 84, Factor = 16.8m },
        new RmdsFactorByAge { Age = 85, Factor = 16.0m },
        new RmdsFactorByAge { Age = 86, Factor = 15.2m },
        new RmdsFactorByAge { Age = 87, Factor = 14.4m },
        new RmdsFactorByAge { Age = 88, Factor = 13.7m },
        new RmdsFactorByAge { Age = 89, Factor = 12.9m },
        new RmdsFactorByAge { Age = 90, Factor = 12.2m },
        new RmdsFactorByAge { Age = 91, Factor = 11.5m },
        new RmdsFactorByAge { Age = 92, Factor = 10.8m },
        new RmdsFactorByAge { Age = 93, Factor = 10.1m },
        new RmdsFactorByAge { Age = 94, Factor = 9.5m },
        new RmdsFactorByAge { Age = 95, Factor = 8.9m },
        new RmdsFactorByAge { Age = 96, Factor = 8.4m },
        new RmdsFactorByAge { Age = 97, Factor = 7.8m },
        new RmdsFactorByAge { Age = 98, Factor = 7.3m },
        new RmdsFactorByAge { Age = 99, Factor = 6.8m }
    ];

    // collects ctx.ProfitDetail.AddRange() rows for inspection by test
    public List<ProfitDetail> addedProfitDetails = [];

    // collects ctx.ProfitDetail.RemoveRange() rows for inspection by test
    public List<ProfitDetail> removedProftitDetails = [];

    public ScenarioFactory CreateOneEmployeeWithProfitDetails()
    {
        var (demographic, payprofits) = StockFactory.CreateEmployee(ProfitYear);
        PayProfits = payprofits;
        Demographics = [demographic];
        ProfitDetails =
        [
            // A default contribution record which gives the employee some initial money
            new ProfitDetail
            {
                ProfitCode = /*0*/ ProfitCode.Constants.IncomingContributions,
                ProfitCodeId = /*0*/ ProfitCode.Constants.IncomingContributions.Id,
                Ssn = demographic.Ssn,
                ProfitYear = ProfitYearPriorYear,
                Contribution = 1000m
            },
            // This is a NOP record that tests can manipulate as needed.  
            new ProfitDetail
            {
                ProfitCode = /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings,
                ProfitCodeId = /*0*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id,
                Ssn = demographic.Ssn,
                ProfitYear = ProfitYearPriorYear
            }
        ];
        return this;
    }

    public ScenarioFactory CreateOneBeneWithProfitDetail()
    {
        Beneficiaries = [StockFactory.CreateBeneficiary()];
        ProfitDetails =
        [
            new ProfitDetail
            {
                ProfitCode = /*0*/ ProfitCode.Constants.IncomingContributions,
                ProfitCodeId = /*0*/ ProfitCode.Constants.IncomingContributions.Id,
                Ssn = Beneficiaries[0].Contact!.Ssn,
                ProfitYear = ProfitYearPriorYear,
                Contribution = 1000m
            }
        ];
        return this;
    }

    public ScenarioFactory EmployeeWithHistory()
    {
        FrozenStates =
        [
            new FrozenState
            {
                AsOfDateTime = DateTimeOffset.UtcNow,
                CreatedDateTime = DateTimeOffset.UtcNow,
                FrozenBy = "a pal",
                ProfitYear = ProfitYear,
                IsActive = true
            }
        ];
        PayProfits =
        [
            new PayProfit
            {
                ProfitYear = ProfitYear,
                DemographicId = 21,
                Etva = 0,
                VestingScheduleId = VestingSchedule.Constants.NewPlan,
                HasForfeited = false,
                CurrentHoursYear = 1200,
                HoursExecutive = 0
            }
        ];
        Demographics =
        [
            new Demographic
            {
                Id = 21,
                OracleHcmId = 44,
                Ssn = 0,
                BadgeNumber = 721,
                StoreNumber = 0,
                PayClassificationId = PayClassification.Constants.Manager,
                ContactInfo = new ContactInfo
                {
                    FullName = "Lasty, Firsty",
                    LastName = "Lasty",
                    FirstName = "Firsty",
                    MiddleName = null,
                    PhoneNumber = null,
                    MobileNumber = null,
                    EmailAddress = null
                },
                Address = new Address
                {
                    Street = "22 Main",
                    Street2 = null,
                    Street3 = null,
                    Street4 = null,
                    City = null,
                    State = null,
                    PostalCode = null,
                    CountryIso = null
                },
                DateOfBirth = default,
                HireDate = default,
                DepartmentId = 2,
                Department = new Department { Id = 2, Name = "Two Dept" },
            }
        ];
        PayProfits[0].Demographic = Demographics[0];
        DemographicHistories =
        [
            new DemographicHistory
            {
                BadgeNumber = 445,
                DemographicId = 21,
                OracleHcmId = 44,
                DepartmentId = 5,
                TerminationCodeId = 'a',
                DateOfBirth = DateTime.Now.AddYears(-40).ToDateOnly(),
                // Contact/Address fields copied from Demographics[0]
                FirstName = Demographics[0].ContactInfo?.FirstName,
                LastName = Demographics[0].ContactInfo?.LastName,
                MiddleName = Demographics[0].ContactInfo?.MiddleName,
                PhoneNumber = Demographics[0].ContactInfo?.PhoneNumber,
                MobileNumber = Demographics[0].ContactInfo?.MobileNumber,
                EmailAddress = Demographics[0].ContactInfo?.EmailAddress,
                Street = Demographics[0].Address?.Street,
                Street2 = Demographics[0].Address?.Street2,
                City = Demographics[0].Address?.City,
                State = Demographics[0].Address?.State,
                PostalCode = Demographics[0].Address?.PostalCode,
                HireDate = Demographics[0].HireDate,
                ReHireDate = Demographics[0].ReHireDate,
                TerminationDate = Demographics[0].TerminationDate,
                EmploymentTypeId = Demographics[0].EmploymentTypeId,
                PayFrequencyId = Demographics[0].PayFrequencyId,
                EmploymentStatusId = Demographics[0].EmploymentStatusId,
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddYears(1)
            }
        ];
        return this;
    }

    public IProfitSharingDataContextFactory BuildMocks()
    {
        //  Transaction handling 
        Mock<DatabaseFacade> mockDatabase = new();
        var databaseFacadeMock = new Mock<DatabaseFacade>(_sdb.ProfitSharingDbContext.Object);

        // Mock a transaction
        var dbTransactionMock = new Mock<IDbContextTransaction>();

        // Setup BeginTransactionAsync on DatabaseFacade
        databaseFacadeMock
            .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbTransactionMock.Object);

        // Link the mocked DatabaseFacade to the DbContext mock
        _sdb.ProfitSharingDbContext.Setup(m => m.Database).Returns(databaseFacadeMock.Object);

        Mock<DbSet<AccountingPeriod>>? mockCalendar = CaldarRecordSeeder.Records.ToList().BuildMockDbSet();
        _sdb.StoreInfoDbContext.Setup(m => m.AccountingPeriods).Returns(mockCalendar.Object);

        // Take care of Beneficiaries and child table BeneficiaryContacts
        Mock<DbSet<Beneficiary>> mockBeneficiaries = Beneficiaries.BuildMockDbSet();
        Mock<DbSet<BeneficiaryContact>> mockBeneficiaryContacts = Beneficiaries.Where(b => b.Contact != null).Select(b => b.Contact!).ToList().BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _sdb.ProfitSharingDbContext.Setup(m => m.BeneficiaryContacts).Returns(mockBeneficiaryContacts.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.BeneficiaryContacts).Returns(mockBeneficiaryContacts.Object);

        // Demographics
        Mock<DbSet<Demographic>> mockDemograhpics = Demographics.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.Demographics).Returns(mockDemograhpics.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.Demographics).Returns(mockDemograhpics.Object);

        // TerminationCodes
        Mock<DbSet<TerminationCode>> mockTerminationCodes = TerminationCodes.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.TerminationCodes).Returns(mockTerminationCodes.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.TerminationCodes).Returns(mockTerminationCodes.Object);

        // PayProfits
        Mock<DbSet<PayProfit>> mockPayProfits = PayProfits.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.PayProfits).Returns(mockPayProfits.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.PayProfits).Returns(mockPayProfits.Object);

        // ProfitDetails
        Mock<DbSet<ProfitDetail>> mockProfitDetails = ProfitDetails.BuildMockDbSet();
        mockProfitDetails
            .Setup(m => m.AddRange(It.IsAny<IEnumerable<ProfitDetail>>()))
            .Callback<IEnumerable<ProfitDetail>>((profitDetails) => addedProfitDetails.AddRange(profitDetails));
        mockProfitDetails
            .Setup(m => m.RemoveRange(It.IsAny<IEnumerable<ProfitDetail>>()))
            .Callback<IEnumerable<ProfitDetail>>((profitDetails) => removedProftitDetails.AddRange(profitDetails));

        _sdb.ProfitSharingDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);

        Mock<DbSet<ProfitCode>> mockProfitCodes = ProfitCodes.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);

        Mock<DbSet<Department>> mockDepartments = Departments.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.Departments).Returns(mockDepartments.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.Departments).Returns(mockDepartments.Object);

        Mock<DbSet<FrozenState>> mockFrozenStates = FrozenStates.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.FrozenStates).Returns(mockFrozenStates.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.FrozenStates).Returns(mockFrozenStates.Object);

        Mock<DbSet<DemographicHistory>> mockDemographicHistories = DemographicHistories.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.DemographicHistories).Returns(mockDemographicHistories.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.DemographicHistories).Returns(mockDemographicHistories.Object);

        Mock<DbSet<YearEndUpdateStatus>> mockYearEndUpdateStatuses = YearEndUpdateStatuses.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.YearEndUpdateStatuses).Returns(mockYearEndUpdateStatuses.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.YearEndUpdateStatuses).Returns(mockYearEndUpdateStatuses.Object);

        // AuditEvents
        Mock<DbSet<AuditEvent>> mockAuditEvents = AuditEvents.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.AuditEvents).Returns(mockAuditEvents.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.AuditEvents).Returns(mockAuditEvents.Object);

        // RmdsFactorsByAge - IRS RMD factors for ages 73-99
        Mock<DbSet<RmdsFactorByAge>> mockRmdsFactorsByAge = RmdsFactorsByAge.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.RmdsFactorsByAge).Returns(mockRmdsFactorsByAge.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.RmdsFactorsByAge).Returns(mockRmdsFactorsByAge.Object);

        // Banks and BankAccounts for administration
        Mock<DbSet<Bank>> mockBanks = Banks.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.Banks).Returns(mockBanks.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.Banks).Returns(mockBanks.Object);

        Mock<DbSet<BankAccount>> mockBankAccounts = BankAccounts.BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.BankAccounts).Returns(mockBankAccounts.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.BankAccounts).Returns(mockBankAccounts.Object);

        return _sdb;
    }

    public ScenarioFactory WithYearEndStatuses()
    {
        YearEndUpdateStatuses = StockFactory.CreateYearEndUpdateStatuses(ProfitYear);
        return this;
    }
}
