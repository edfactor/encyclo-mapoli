using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping.Data;
using Demoulas.Common.Data.Services.Entities.Entities;
using System.Reflection;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;

namespace Demoulas.ProfitSharing.UnitTests.Common.Mocks;

/// <summary>
/// This factory is used to configure a ScenarioDataContextFactory for use in tests.
/// Specifically it helps configure Employees or Beneficiaries in a standard way or helps configure the mock database for a specific test scenario.
/// </summary>
public sealed class ScenarioFactory
{
    private readonly ScenarioDataContextFactory _sdb = new();

    public short ThisYear { get; set; } = 2024;
    public short LastYear => (short)(ThisYear - 1);
    public List<Demographic> Demographics { get; set; } = [];
    public List<PayProfit> PayProfits { get; set; } = [];
    public List<Beneficiary> Beneficiaries { get; set; } = [];
    public List<ProfitDetail> ProfitDetails { get; set; } = [];

    // populate ProfitCode dictionary object
    public List<ProfitCode> ProfitCodes { get; set; } = typeof(ProfitCode.Constants)
        .GetProperties(BindingFlags.Public | BindingFlags.Static)
        .Select(p => p.GetValue(null))
        .OfType<ProfitCode>().ToList();

    // collects ctx.ProfitDetail.AddRange() rows for inspection by test
    public List<ProfitDetail> addedProfitDetails = [];

    // collects ctx.ProfitDetail.RemoveRange() rows for inspection by test
    public List<ProfitDetail> removedProftitDetails = [];

    public ScenarioFactory CreateOneEmployeeWithProfitDetails()
    {
        var (demographic, payprofits) = StockFactory.CreateEmployee(ThisYear);
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
                ProfitYear = LastYear,
                Contribution = 1000m
            },
            // This is a NOP record that tests can manipulate as approprate.  
            new ProfitDetail
            {
                ProfitCode = /*8*/ ProfitCode.Constants.Incoming100PercentVestedEarnings,
                ProfitCodeId = /*0*/ ProfitCode.Constants.Incoming100PercentVestedEarnings.Id,
                Ssn = demographic.Ssn,
                ProfitYear = LastYear
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
                ProfitYear = LastYear,
                Contribution = 1000m
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

        Mock<DbSet<AccountingPeriod>>? mockCalendar = CaldarRecordSeeder.Records.AsQueryable().BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.AccountingPeriods).Returns(mockCalendar.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.AccountingPeriods).Returns(mockCalendar.Object);

        // Take care of Beneficiaries and child table BeneficiaryContacts
        Mock<DbSet<Beneficiary>> mockBeneficiaries = Beneficiaries.AsQueryable().BuildMockDbSet();
        Mock<DbSet<BeneficiaryContact>> mockBeneficiaryContacts = Beneficiaries.Where(b => b.Contact != null).Select(b => b.Contact!).AsQueryable().BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _sdb.ProfitSharingDbContext.Setup(m => m.BeneficiaryContacts).Returns(mockBeneficiaryContacts.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.Beneficiaries).Returns(mockBeneficiaries.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.BeneficiaryContacts).Returns(mockBeneficiaryContacts.Object);

        // Demographics
        Mock<DbSet<Demographic>> mockDemograhpics = Demographics.AsQueryable().BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.Demographics).Returns(mockDemograhpics.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.Demographics).Returns(mockDemograhpics.Object);

        // PayProfits
        Mock<DbSet<PayProfit>> mockPayProfits = PayProfits.AsQueryable().BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.PayProfits).Returns(mockPayProfits.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.PayProfits).Returns(mockPayProfits.Object);

        // ProfitDetails
        Mock<DbSet<ProfitDetail>> mockProfitDetails = ProfitDetails.AsQueryable().BuildMockDbSet();
        mockProfitDetails
            .Setup(m => m.AddRange(It.IsAny<IEnumerable<ProfitDetail>>()))
            .Callback<IEnumerable<ProfitDetail>>((profitDetails) => addedProfitDetails.AddRange(profitDetails));
        mockProfitDetails
            .Setup(m => m.RemoveRange(It.IsAny<IEnumerable<ProfitDetail>>()))
            .Callback<IEnumerable<ProfitDetail>>((profitDetails) => removedProftitDetails.AddRange(profitDetails));

        _sdb.ProfitSharingDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.ProfitDetails).Returns(mockProfitDetails.Object);

        Mock<DbSet<ProfitCode>> mockProfitCodes = ProfitCodes.AsQueryable().BuildMockDbSet();
        _sdb.ProfitSharingDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);
        _sdb.ProfitSharingReadOnlyDbContext.Setup(m => m.ProfitCodes).Returns(mockProfitCodes.Object);

        return _sdb;
    }
}
