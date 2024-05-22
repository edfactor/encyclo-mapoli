using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.ProfitSharing.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

//When running migrations or using design-time tools, Entity Framework will need to be able to create an instance of this DbContext without going through the normal startup process
namespace Demoulas.ProfitSharing.Data.Factories;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OracleDbContext>
{
    public OracleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OracleDbContext>();
        optionsBuilder.UseOracle();

        return new ProfitSharingDbContext(optionsBuilder.Options);
    }
}
