
using System.Reflection;
using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping.Data;
using Demoulas.Common.Data.Services.Entities.Entities;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
// Common also has a Department.cs file, so we need to specify the one we want to use
using Department = Demoulas.ProfitSharing.Data.Entities.Department;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

/// <summary>
/// This factory is used to configure a ScenarioDataContextFactory for use in tests.
/// Specifically it helps configure Employees or Beneficiaries in a standard way or helps configure the InMemory database for a specific test scenario.
/// </summary>
public sealed class ScenarioFactory
{
    public short ProfitYear { get; set; } = 2024;
    public short ProfitYearPriorYear => (short)(ProfitYear - 1);
    public List<Demographic> Demographics { get; set; } = [];
    public List<PayProfit> PayProfits { get; set; } = [];
    public List<Beneficiary> Beneficiaries { get; set; } = [];
    public List<ProfitDetail> ProfitDetails { get; set; } = [];
    public List<FrozenState> FrozenStates { get; set; } = [];
    public List<DemographicHistory> DemographicHistories { get; set; } = [];
    public List<YearEndUpdateStatus> YearEndUpdateStatuses { get; set; } = [];

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
                DateOfBirth = DateTime.Now.AddYears(-40).ToDateOnly()
            }
        ];
        return this;
    }

    /// <summary>
    /// Creates a ScenarioDataContextFactory configured with mock DbSets for the scenario data.
    /// Uses MockQueryable to provide IQueryable support for mock DbSets.
    /// 
    /// LIMITATION: Mock-based IQueryable does not implement IAsyncQueryProvider, so EF Core
    /// async methods (.ToListAsync(), .FirstOrDefaultAsync(), etc.) will fail with:
    /// "The source IQueryable doesn't implement IAsyncEnumerable"
    /// 
    /// For tests requiring async query support, consider using a real database provider
    /// (e.g., EF Core InMemory, SQLite InMemory, or test containers).
    /// </summary>
    public IProfitSharingDataContextFactory BuildMocks()
    {
        var demographics = Demographics.Any() ? Demographics : [];
        var beneficiaryContacts = Beneficiaries
            .Where(b => b.Contact is not null)
            .Select(b => b.Contact!)
            .ToList();

        var sdb = new ScenarioDataContextFactory();

        // Setup Demographics DbSet with MockQueryable
        Mock<DbSet<Demographic>> mockDemographicSet = demographics.BuildMockDbSet();
        sdb.ProfitSharingDbContext
            .Setup(c => c.Demographics)
            .Returns(mockDemographicSet.Object);
        sdb.ProfitSharingReadOnlyDbContext
            .Setup(c => c.Demographics)
            .Returns(mockDemographicSet.Object);

        // Setup DemographicHistories DbSet
        Mock<DbSet<DemographicHistory>> mockHistoriesSet = DemographicHistories.BuildMockDbSet();
        sdb.ProfitSharingDbContext
            .Setup(c => c.DemographicHistories)
            .Returns(mockHistoriesSet.Object);
        sdb.ProfitSharingReadOnlyDbContext
            .Setup(c => c.DemographicHistories)
            .Returns(mockHistoriesSet.Object);

        // Setup BeneficiaryContacts DbSet
        Mock<DbSet<BeneficiaryContact>> mockBeneficiaryContactsSet = beneficiaryContacts.BuildMockDbSet();
        sdb.ProfitSharingDbContext
            .Setup(c => c.BeneficiaryContacts)
            .Returns(mockBeneficiaryContactsSet.Object);
        sdb.ProfitSharingReadOnlyDbContext
            .Setup(c => c.BeneficiaryContacts)
            .Returns(mockBeneficiaryContactsSet.Object);

        // Setup ProfitDetails DbSet
        Mock<DbSet<ProfitDetail>> mockProfitDetailsSet = ProfitDetails.BuildMockDbSet();
        sdb.ProfitSharingDbContext
            .Setup(c => c.ProfitDetails)
            .Returns(mockProfitDetailsSet.Object);
        sdb.ProfitSharingReadOnlyDbContext
            .Setup(c => c.ProfitDetails)
            .Returns(mockProfitDetailsSet.Object);

        return sdb;
    }

    public ScenarioFactory WithYearEndStatuses()
    {
        YearEndUpdateStatuses = StockFactory.CreateYearEndUpdateStatuses(ProfitYear);
        return this;
    }
}
