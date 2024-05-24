using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Extensions;
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

        modelBuilder.ApplyModelConfiguration();
    }
}
