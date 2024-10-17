using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Interfaces;

public interface IProfitSharingDbContext
{
    DbSet<Demographic> Demographics { get; set; }
    DbSet<Country> Countries { get; set; }
    DbSet<ProfitDetail> ProfitDetails { get; set; }

    DbSet<Beneficiary> Beneficiaries { get; set; }
    DbSet<PayProfit> PayProfits { get; set; }
    DbSet<ProfitDetail> ProfitDetails { get; set; }
    DbSet<Distribution> Distributions { get; set; }

    DbSet<Job> Jobs { get; set; }
}
