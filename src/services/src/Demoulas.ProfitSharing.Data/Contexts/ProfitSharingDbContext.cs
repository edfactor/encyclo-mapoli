using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts;

public class ProfitSharingDbContext : OracleDbContext<ProfitSharingDbContext>, IProfitSharingDbContext
{
    public ProfitSharingDbContext()
    {
        // Used for Unit testing/Mocking only
    }
    public ProfitSharingDbContext(DbContextOptions<ProfitSharingDbContext> options)
    : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }
    
    public virtual DbSet<Demographic> Demographics { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<PayClassification> PayClassifications { get; set; }
    public virtual DbSet<ProfitDetail> ProfitDetails { get; set; }
    public virtual DbSet<ProfitCode> ProfitCodes { get; set; }
    public virtual DbSet<TaxCode> TaxCodes { get; set; }
    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
    public virtual DbSet<PayProfit> PayProfits { get; set; }
    public virtual DbSet<Distribution> Distributions { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }
    public virtual DbSet<DemographicSyncAudit> DemographicSyncAudit { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
