using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;
using Demoulas.ProfitSharing.Data.Entities.NotOwned;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts;

public class ProfitSharingReadOnlyDbContext : ReadOnlyOracleDbContext<ProfitSharingReadOnlyDbContext>, IProfitSharingDbContext
{
    public ProfitSharingReadOnlyDbContext()
    {
        // Used for Unit testing/Mocking only
    }

    public ProfitSharingReadOnlyDbContext(DbContextOptions<ProfitSharingReadOnlyDbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Demographic> Demographics { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<PayClassification> PayClassifications { get; set; }
    public virtual DbSet<ProfitDetail> ProfitDetails { get; set; }
    public virtual DbSet<ProfitCode> ProfitCodes { get; set; }
    public virtual DbSet<TaxCode> TaxCodes { get; set; }
    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
    public virtual DbSet<PayProfit> PayProfits { get; set; }

    public DbSet<Job> Jobs { get; set; }

    public virtual DbSet<CaldarRecord> CaldarRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
