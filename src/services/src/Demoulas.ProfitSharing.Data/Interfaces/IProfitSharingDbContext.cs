using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDbContext
{
    DbSet<Demographic> Demographics { get; set; }
    DbSet<DemographicHistory> DemographicHistories { get; set; }
    DbSet<EmploymentType> EmploymentTypes { get; set; }
    DbSet<Department> Departments { get; set; }
    DbSet<FrozenState> FrozenStates { get; set; }
    DbSet<Country> Countries { get; set; }
    DbSet<Beneficiary> Beneficiaries { get; set; }
    DbSet<PayProfit> PayProfits { get; set; }
    DbSet<ProfitDetail> ProfitDetails { get; set; }
    DbSet<Distribution> Distributions { get; set; }
    DbSet<FakeSsn> FakeSsns { get; set; }
    DbSet<Missive> Missives { get; set; }
    DbSet<ParticipantTotal> ParticipantTotals { get; set; }
    DbSet<ParticipantTotalRatio> ParticipantTotalRatios { get; set; }
    DbSet<ParticipantTotalYear> ParticipantTotalYears { get; set; }
    DbSet<ParticipantTotalVestingBalance> ParticipantTotalVestingBalances { get; set; }
    DatabaseFacade Database { get; }
}
