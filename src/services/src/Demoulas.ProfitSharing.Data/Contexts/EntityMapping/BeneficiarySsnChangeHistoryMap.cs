using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class BeneficiarySsnChangeHistoryMap : IEntityTypeConfiguration<BeneficiarySsnChangeHistory>
{
    public void Configure(EntityTypeBuilder<BeneficiarySsnChangeHistory> builder)
    {
        _ = builder.ToTable("BENEFICIARY_SSN_CHANGE_HISTORY");
        _ = builder.HasKey(x => x.Id);

        _ = builder.HasIndex(x => x.BeneficiaryId, "IX_BENEFICIARY");

        _ = builder.Property(x => x.Id)
            .HasPrecision(18)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        _ = builder.Property(x => x.BeneficiaryId)
            .HasPrecision(9)
            .HasColumnName("BENEFICIARY_ID")
            .IsRequired();

        _ = builder.Property(x => x.OldSsn)
            .HasPrecision(9)
            .HasColumnName("OLD_SSN")
            .IsRequired();

        _ = builder.Property(x => x.NewSsn)
            .HasPrecision(9)
            .HasColumnName("NEW_SSN")
            .IsRequired();

        _ = builder.Property(e => e.ChangeDateUtc)
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP")
            .HasColumnName("CREATED_UTC");
    }
}
