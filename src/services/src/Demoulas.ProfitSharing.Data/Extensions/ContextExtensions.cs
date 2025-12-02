using Demoulas.Common.Data.Services.Entities.Contexts.EntityMapping;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Audit;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Navigations;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Scheduling;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Virtual;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Extensions;
internal static class ContextExtensions
{
    public static ModelBuilder ApplyModelConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AnnuityRateMap());
        modelBuilder.ApplyConfiguration(new AuditEventMap());
        modelBuilder.ApplyConfiguration(new FakeSsnMap());
        modelBuilder.ApplyConfiguration(new DataImportRecordMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryContactMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryContactArchiveMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryArchiveMap());
        modelBuilder.ApplyConfiguration(new BeneficiaryTypeMap());
        modelBuilder.ApplyConfiguration(new BeneficiarySsnChangeHistoryMap());
        modelBuilder.ApplyConfiguration(new AccountingPeriodConfiguration { ExcludeFromMigrations = false, SeedDataAfterMigrations = true });
        modelBuilder.ApplyConfiguration(new CommentTypeMap());
        modelBuilder.ApplyConfiguration(new CountryMap());
        modelBuilder.ApplyConfiguration(new DepartmentMap());
        modelBuilder.ApplyConfiguration(new DemographicMap());
        modelBuilder.ApplyConfiguration(new DemographicHistoryMap());
        modelBuilder.ApplyConfiguration(new DemographicSyncAuditMap());
        modelBuilder.ApplyConfiguration(new DistributionFrequencyMap());
        modelBuilder.ApplyConfiguration(new DemographicSsnChangeHistoryMap());
        modelBuilder.ApplyConfiguration(new DistributionMap());
        modelBuilder.ApplyConfiguration(new DistributionPayeeMap());
        modelBuilder.ApplyConfiguration(new DistributionRequestMap());
        modelBuilder.ApplyConfiguration(new DistributionRequestTypeMap());
        modelBuilder.ApplyConfiguration(new DistributionRequestReasonMap());
        modelBuilder.ApplyConfiguration(new DistributionRequestStatusMap());
        modelBuilder.ApplyConfiguration(new DistributionStatusMap());
        modelBuilder.ApplyConfiguration(new DistributionThirdPartyPayeeMap());
        modelBuilder.ApplyConfiguration(new EmployeeTypeMap());
        modelBuilder.ApplyConfiguration(new EmploymentStatusMap());
        modelBuilder.ApplyConfiguration(new EmploymentTypeMap());
        modelBuilder.ApplyConfiguration(new ExcludedIdMap());
        modelBuilder.ApplyConfiguration(new ExcludedIdTypeMap());
        modelBuilder.ApplyConfiguration(new EnrollmentMap());
        modelBuilder.ApplyConfiguration(new FrozenStateMap());
        modelBuilder.ApplyConfiguration(new GenderMap());
        modelBuilder.ApplyConfiguration(new HealthCheckStatusHistoryMap());
        modelBuilder.ApplyConfiguration(new JobMap());
        modelBuilder.ApplyConfiguration(new JobStatusMap());
        modelBuilder.ApplyConfiguration(new JobTypeMap());
        modelBuilder.ApplyConfiguration(new MissiveMap());
        modelBuilder.ApplyConfiguration(new ParticipantTotalMap());
        modelBuilder.ApplyConfiguration(new ParticipantTotalRatioMap());
        modelBuilder.ApplyConfiguration(new ParticipantTotalVestingBalanceMap());
        modelBuilder.ApplyConfiguration(new ProfitShareTotalsMap());
        modelBuilder.ApplyConfiguration(new ParticipantTotalYearMap());
        modelBuilder.ApplyConfiguration(new ProfitDetailRollupMap());
        modelBuilder.ApplyConfiguration(new PayClassificationMap());
        modelBuilder.ApplyConfiguration(new PayFrequencyMap());
        modelBuilder.ApplyConfiguration(new PayProfitMap());
        modelBuilder.ApplyConfiguration(new ProfitCodeMap());
        modelBuilder.ApplyConfiguration(new ProfitDetailMap());
        modelBuilder.ApplyConfiguration(new ProfitShareCheckMap());
        modelBuilder.ApplyConfiguration(new ReportChecksumMap());
        modelBuilder.ApplyConfiguration(new StartMethodMap());
        modelBuilder.ApplyConfiguration(new StateTaxMap());
        modelBuilder.ApplyConfiguration(new TaxCodeMap());
        modelBuilder.ApplyConfiguration(new TerminationCodeMap());
        modelBuilder.ApplyConfiguration(new ZeroContributionReasonMap());
        modelBuilder.ApplyConfiguration(new YearEndUpdateStatusMapping());
        modelBuilder.ApplyConfiguration(new NavigationMap());
        modelBuilder.ApplyConfiguration(new NavigationStatusMap());
        modelBuilder.ApplyConfiguration(new NavigationTrackingMap());
        modelBuilder.ApplyConfiguration(new NavigationRoleMap());
        modelBuilder.ApplyConfiguration(new StateMap());

        modelBuilder.HasSequence<int>("FAKE_SSN_SEQ").StartsAt(666000000)
            .IncrementsBy(1)
            .HasMin(666000000)
            .HasMax(666999999)
            .IsCyclic(false);

        // Force table names to be upper case for consistency with all existing DSM projects
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Set table name to upper case
            entity.SetTableName(entity.GetTableName()?.ToUpper());
        }

        // Set the global delete behavior to NoAction for all relationships
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        return modelBuilder;
    }
}
