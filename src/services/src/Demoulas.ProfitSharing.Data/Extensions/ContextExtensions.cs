using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Extensions;
internal static class ContextExtensions
{
    public static ModelBuilder ApplyModelConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CountryMap());
        modelBuilder.ApplyConfiguration(new DefinitionMap());
        modelBuilder.ApplyConfiguration(new DemographicMap());
        modelBuilder.ApplyConfiguration(new PayClassificationMap());

        return modelBuilder;
    }
}
