using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts;

public sealed class ProfitSharingDbContext : OracleDbContext<ProfitSharingDbContext>, IProfitSharingDbContext
{
    public ProfitSharingDbContext(DbContextOptions<ProfitSharingDbContext> options)
        : base(options)
    {

    }

    public DbSet<Demographic> Demographics { get; set; }
    public DbSet<Definition> Definitions { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<PayClassification> PayClassifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
