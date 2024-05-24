using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts;

public class ProfitSharingReadOnlyDbContext : ReadOnlyOracleDbContext<ProfitSharingReadOnlyDbContext>
{
    public ProfitSharingReadOnlyDbContext(DbContextOptions<ProfitSharingReadOnlyDbContext> options)
        : base(options)
    {

    }

    public DbSet<Demographic> Demographics { get; set; }
    public DbSet<Definition> Definitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
