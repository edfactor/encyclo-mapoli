using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Virtual;

internal sealed class ParticipantTotalVestingBalanceMap : IEntityTypeConfiguration<ParticipantTotalVestingBalance>
{
    //This table is virtual in nature.  It uses the FromSql method to access data.
    public void Configure(EntityTypeBuilder<ParticipantTotalVestingBalance> builder)
    {
        builder.Metadata.SetIsTableExcludedFromMigrations(true);

        builder.HasKey(x => new { x.Id, x.Ssn });

        builder.Property(x => x.Ssn)
            .HasColumnName("SSN")
            .IsRequired();

        builder.Property(x => x.Id)
            .HasColumnName("ID");

        builder.Property(x => x.VestedBalance)
            .HasColumnName("VESTEDBALANCE")
            .HasPrecision(18, 2);
        builder.Property(x => x.CurrentBalance)
            .HasColumnName("CURRENTBALANCE")
            .HasPrecision(18, 2);
        builder.Property(x => x.VestingPercent)
            .HasColumnName("RATIO")
            .HasPrecision(18, 6);

        builder.Property(x => x.YearsInPlan)
            .HasColumnName("YEARS");

        builder.Property(x => x.AllocationsToBeneficiary)
            .HasColumnName("ALLOCTOBENE")
            .HasPrecision(18, 2);
        builder.Property(x => x.AllocationsFromBeneficiary)
            .HasColumnName("ALLOCFROMBENE")
            .HasPrecision(18, 2);
    }
}
