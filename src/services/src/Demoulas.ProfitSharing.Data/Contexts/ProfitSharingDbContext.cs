using Demoulas.Common.Data.Contexts.Contexts;
using Demoulas.Common.Data.Services.Entities.Entities;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Entities.CheckRun;
using Demoulas.ProfitSharing.Data.Entities.FileTransfer;
using Demoulas.ProfitSharing.Data.Entities.Navigations;
using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Department = Demoulas.ProfitSharing.Data.Entities.Department;

namespace Demoulas.ProfitSharing.Data.Contexts;

public class ProfitSharingDbContext : OracleDbContext<ProfitSharingDbContext>, IProfitSharingDbContext
{
    public ProfitSharingDbContext()
    {
        // Used for Unit testing/Mocking only
    }
    public ProfitSharingDbContext(DbContextOptions<ProfitSharingDbContext> options)
    : base(options)
    {
        ChangeTracker.LazyLoadingEnabled = false;
    }

    public virtual DbSet<AnnuityRate> AnnuityRates { get; set; }
    public virtual DbSet<AuditEvent> AuditEvents { get; set; }
    public virtual DbSet<FileTransferAudit> FileTransferAudits { get; set; }
    public virtual DbSet<FtpOperationLog> FtpOperationLogs { get; set; }
    public virtual DbSet<ReportChecksum> ReportChecksums { get; set; }
    public virtual DbSet<Demographic> Demographics { get; set; }
    public virtual DbSet<DemographicHistory> DemographicHistories { get; set; }
    public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }
    public virtual DbSet<ExcludedId> ExcludedIds { get; set; }
    public virtual DbSet<FrozenState> FrozenStates { get; set; }
    public virtual DbSet<Country> Countries { get; set; }
    public virtual DbSet<PayClassification> PayClassifications { get; set; }
    public virtual DbSet<ProfitDetail> ProfitDetails { get; set; }
    public virtual DbSet<ProfitCode> ProfitCodes { get; set; }
    public virtual DbSet<TaxCode> TaxCodes { get; set; }
    public virtual DbSet<TerminationCode> TerminationCodes { get; set; }
    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
    public virtual DbSet<BeneficiaryArchive> BeneficiaryArchives { get; set; }
    public virtual DbSet<BeneficiaryContact> BeneficiaryContacts { get; set; }
    public virtual DbSet<BeneficiaryContactArchive> BeneficiaryContactArchives { get; set; }
    public virtual DbSet<BeneficiaryType> BeneficiaryTypes { get; set; }
    public virtual DbSet<CheckRunWorkflow> CheckRunWorkflows { get; set; }
    public virtual DbSet<CommentType> CommentTypes { get; set; }
    public virtual DbSet<PayProfit> PayProfits { get; set; }
    public virtual DbSet<Distribution> Distributions { get; set; }
    public virtual DbSet<DistributionFrequency> DistributionFrequencies { get; set; }
    public virtual DbSet<DistributionPayee> DistributionPayees { get; set; }
    public virtual DbSet<DistributionStatus> DistributionStatuses { get; set; }
    public virtual DbSet<Job> Jobs { get; set; }
    public virtual DbSet<DemographicSyncAudit> DemographicSyncAudit { get; set; }

    public virtual DbSet<DataImportRecord> DataImportRecords { get; set; }
    public virtual DbSet<FakeSsn> FakeSsns { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Missive> Missives { get; set; }
    public virtual DbSet<YearEndUpdateStatus> YearEndUpdateStatuses { get; set; }
    public virtual DbSet<ParticipantTotal> ParticipantTotals { get; set; }
    public virtual DbSet<ParticipantTotal> ParticipantEvtaTotals { get; set; }
    public virtual DbSet<ParticipantTotalRatio> ParticipantTotalRatios { get; set; }
    public virtual DbSet<ParticipantTotalYear> ParticipantTotalYears { get; set; }
    public virtual DbSet<ParticipantTotalVestingBalance> ParticipantTotalVestingBalances { get; set; }
    public virtual DbSet<ProfitDetailRollup> ProfitDetailRollups { get; set; }
    public virtual DbSet<Navigation> Navigations { get; set; }
    public virtual DbSet<NavigationRole> NavigationRoles { get; set; }
    public virtual DbSet<NavigationStatus> NavigationStatuses { get; set; }
    public virtual DbSet<NavigationTracking> NavigationTrackings { get; set; }

    public virtual DbSet<ProfitShareCheck> ProfitShareChecks { get; set; }
    public virtual DbSet<ProfitShareTotal> ProfitShareTotals { get; set; }
    public virtual DbSet<StateTax> StateTaxes { get; set; }
    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<HealthCheckStatusHistory> HealthCheckStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyModelConfiguration();
    }
}
