using Demoulas.ProfitSharing.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

//When running migrations or using design-time tools, Entity Framework will need to be able to create an instance of this DbContext without going through the normal startup process
namespace Demoulas.ProfitSharing.Data.Factories;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProfitSharingDbContext>
{
    public ProfitSharingDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ProfitSharingDbContext> optionsBuilder = new DbContextOptionsBuilder<ProfitSharingDbContext>();
        _ = optionsBuilder.UseOracle(builder =>
        {
            builder.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
        });
#if DEBUG
        _ = optionsBuilder.EnableDetailedErrors();
#endif
        _ = optionsBuilder.UseUpperCaseNamingConvention();

        return new ProfitSharingDbContext(optionsBuilder.Options);
    }
}
