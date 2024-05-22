using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts;

public class ProfitSharingReadOnlyDbContext : ReadOnlyOracleDbContext<ProfitSharingReadOnlyDbContext>
{
    public ProfitSharingReadOnlyDbContext(DbContextOptions<ProfitSharingReadOnlyDbContext> options)
        : base(options)
    {

    }

    public DbSet<Demographics> Demographics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DemographicsMap());
    }
}
