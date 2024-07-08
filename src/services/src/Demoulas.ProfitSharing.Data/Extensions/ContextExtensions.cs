using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Extensions;
internal static class ContextExtensions
{
    public static ModelBuilder ApplyModelConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CountryMap());
        modelBuilder.ApplyConfiguration(new DemographicMap());
        modelBuilder.ApplyConfiguration(new PayClassificationMap());

        modelBuilder.ApplyConfiguration(new GenderMap());
        modelBuilder.ApplyConfiguration(new PayFrequencyMap());
        modelBuilder.ApplyConfiguration(new TerminationCodeMap());
        modelBuilder.ApplyConfiguration(new EmploymentTypeMap());
        modelBuilder.ApplyConfiguration(new DepartmentMap());
        modelBuilder.ApplyConfiguration(new EnrollmentMap());
        modelBuilder.ApplyConfiguration(new PayProfitMap());

        return modelBuilder;
    }
}
