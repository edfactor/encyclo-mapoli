using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Virtual;
internal sealed class ProfitDetailRollupMap : IEntityTypeConfiguration<ProfitDetailRollup>
{
    // This table is virtual in nature. It uses the FromSql method to access data.
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProfitDetailRollup> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);
        builder.HasKey(x => x.Ssn);
        builder.Property(x => x.Ssn)
            .HasColumnName("SSN")
            .IsRequired();
        builder.Property(x => x.TotalContributions)
            .HasColumnName("TOTAL_CONTRIBUTIONS");
        builder.Property(x => x.TotalEarnings)
            .HasColumnName("TOTAL_EARNINGS");
        builder.Property(x => x.TotalForfeitures)
            .HasColumnName("TOTAL_FORFEITURES");
        builder.Property(x => x.TotalPayments)
            .HasColumnName("TOTAL_PAYMENTS");
        builder.Property(x => x.Distribution)
            .HasColumnName("DISTRIBUTION");
        builder.Property(x => x.BeneficiaryAllocation)
            .HasColumnName("BENEFICIARY_ALLOCATION");
        builder.Property(x => x.CurrentBalance)
            .HasColumnName("CURRENT_BALANCE");
        builder.Property(x => x.MilitaryTotal)
            .HasColumnName("MILITARY_TOTAL");
        builder.Property(x => x.ClassActionFundTotal)
            .HasColumnName("CLASS_ACTION_FUND_TOTAL");
        builder.Property(x => x.PaidAllocationsTotal)
            .HasColumnName("PAID_ALLOCATIONS_TOTAL");
        builder.Property(x => x.DistributionsTotal)
            .HasColumnName("DISTRIBUTIONS_TOTAL");
        builder.Property(x => x.AllocationsTotal)
            .HasColumnName("ALLOCATIONS_TOTAL");
        builder.Property(x => x.ForfeitsTotal)
            .HasColumnName("FORFEITS_TOTAL");
    }
}
