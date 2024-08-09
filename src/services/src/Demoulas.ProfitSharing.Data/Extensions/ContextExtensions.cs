using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Extensions;
internal static class ContextExtensions
{
    public static ModelBuilder ApplyModelConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CountryMap());
        modelBuilder.ApplyConfiguration(new DemographicMap());
        modelBuilder.ApplyConfiguration(new PayClassificationMap());
        modelBuilder.ApplyConfiguration(new ProfitDetailMap());
        modelBuilder.ApplyConfiguration(new ProfitCodeMap());
        modelBuilder.ApplyConfiguration(new TaxCodeMap());
        modelBuilder.ApplyConfiguration(new GenderMap());
        modelBuilder.ApplyConfiguration(new PayFrequencyMap());
        modelBuilder.ApplyConfiguration(new TerminationCodeMap());
        modelBuilder.ApplyConfiguration(new EmploymentTypeMap());
        modelBuilder.ApplyConfiguration(new DepartmentMap());
        modelBuilder.ApplyConfiguration(new EnrollmentMap());
        modelBuilder.ApplyConfiguration(new PayProfitMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryTypeMap());
        modelBuilder.ApplyConfiguration(new EmployeeTypeMap());
        modelBuilder.ApplyConfiguration(new ZeroContributionReasonMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryMap());
        modelBuilder.ApplyConfiguration(new EmploymentStatusMap());
        modelBuilder.ApplyConfiguration(new JobMap());
        modelBuilder.ApplyConfiguration(new JobStatusMap());
        modelBuilder.ApplyConfiguration(new StartMethodMap());
        modelBuilder.ApplyConfiguration(new JobTypeMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryKindMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryRelPercentMap());

        // Force table names to be upper case for consistency with all existing DSM projects
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Set table name to upper case
            entity.SetTableName(entity.GetTableName()?.ToUpper());
        }

        return modelBuilder;
    }
}
