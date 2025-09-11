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
            .HasColumnName("TOTAL_CONTRIBUTIONS")
            .HasPrecision(18, 2);
        builder.Property(x => x.TotalEarnings)
            .HasColumnName("TOTAL_EARNINGS")
            .HasPrecision(18, 2);
        builder.Property(x => x.TotalForfeitures)
            .HasColumnName("TOTAL_FORFEITURES")
            .HasPrecision(18, 2);
        builder.Property(x => x.TotalPayments)
            .HasColumnName("TOTAL_PAYMENTS")
            .HasPrecision(18, 2);
        builder.Property(x => x.Distribution)
            .HasColumnName("DISTRIBUTION")
            .HasPrecision(18, 2);
        builder.Property(x => x.BeneficiaryAllocation)
            .HasColumnName("BENEFICIARY_ALLOCATION")
            .HasPrecision(18, 2);
        builder.Property(x => x.CurrentBalance)
            .HasColumnName("CURRENT_BALANCE")
            .HasPrecision(18, 2);
        builder.Property(x => x.MilitaryTotal)
            .HasColumnName("MILITARY_TOTAL")
            .HasPrecision(18, 2);
        builder.Property(x => x.ClassActionFundTotal)
            .HasColumnName("CLASS_ACTION_FUND_TOTAL")
            .HasPrecision(18, 2);
        builder.Property(x => x.PaidAllocationsTotal)
            .HasColumnName("PAID_ALLOCATIONS_TOTAL")
            .HasPrecision(18, 2);
        builder.Property(x => x.DistributionsTotal)
            .HasColumnName("DISTRIBUTIONS_TOTAL")
            .HasPrecision(18, 2);
        builder.Property(x => x.AllocationsTotal)
            .HasColumnName("ALLOCATIONS_TOTAL")
            .HasPrecision(18, 2);
        builder.Property(x => x.ForfeitsTotal)
            .HasColumnName("FORFEITS_TOTAL")
            .HasPrecision(18, 2);
    }
}
