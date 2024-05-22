using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts;

public class ProfitSharingDbContext : OracleDbContext<ProfitSharingDbContext>
{
    public ProfitSharingDbContext(DbContextOptions<ProfitSharingDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DemographicsMap());
    }
}
