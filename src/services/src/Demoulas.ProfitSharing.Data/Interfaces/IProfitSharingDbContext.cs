using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDbContext
{
    DbSet<Bank> Banks { get; set; }
    DbSet<BankAccount> BankAccounts { get; set; }
    DbSet<AnnuityRate> AnnuityRates { get; set; }
    DbSet<Demographic> Demographics { get; set; }
    DbSet<DemographicHistory> DemographicHistories { get; set; }
    DbSet<EmploymentType> EmploymentTypes { get; set; }
    DbSet<Department> Departments { get; set; }
    DbSet<FrozenState> FrozenStates { get; set; }
    DbSet<Country> Countries { get; set; }
    DbSet<Beneficiary> Beneficiaries { get; set; }
    DbSet<BeneficiaryArchive> BeneficiaryArchives { get; set; }
    DbSet<BeneficiaryContactArchive> BeneficiaryContactArchives { get; set; }
    DbSet<PayProfit> PayProfits { get; set; }
    DbSet<ProfitDetail> ProfitDetails { get; set; }
    DbSet<Distribution> Distributions { get; set; }
    DbSet<DistributionFrequency> DistributionFrequencies { get; set; }
    DbSet<DistributionPayee> DistributionPayees { get; set; }
    DbSet<DistributionStatus> DistributionStatuses { get; set; }
    DbSet<ExcludedId> ExcludedIds { get; set; }
    DbSet<FakeSsn> FakeSsns { get; set; }
    DbSet<Missive> Missives { get; set; }
    DbSet<ParticipantTotal> ParticipantTotals { get; set; }
    DbSet<ParticipantTotal> ParticipantEvtaTotals { get; set; }
    DbSet<ParticipantTotalRatio> ParticipantTotalRatios { get; set; }
    DbSet<ParticipantTotalYear> ParticipantTotalYears { get; set; }
    DbSet<ParticipantTotalVestingBalance> ParticipantTotalVestingBalances { get; set; }
    DbSet<ProfitDetailRollup> ProfitDetailRollups { get; set; }
    DbSet<ProfitShareTotal> ProfitShareTotals { get; set; }
    DbSet<StateTax> StateTaxes { get; set; }
    DatabaseFacade Database { get; }
}
