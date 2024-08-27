using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BeneficiaryRelPercentMap : IEntityTypeConfiguration<BeneficiaryRelPercent>
{
    public void Configure(EntityTypeBuilder<BeneficiaryRelPercent> builder)
    {
        builder.ToTable("BENEFICIARY_REL_PERCENT");
        builder.HasKey(c => c.PSN);
        builder.Property(c => c.PSN).IsRequired().HasPrecision(11).ValueGeneratedNever();
        builder.HasIndex(c => c.SSN, "IX_SSN");
        builder.Property(c => c.SSN).IsRequired().HasPrecision(9);
        builder.Property(b => b.KindId).IsRequired().HasColumnName("KIND_ID");
        builder.Property(e => e.Percent).IsRequired().HasColumnType("numeric(3,0)").HasPrecision(3);
        builder.Property(e => e.Relationship).HasMaxLength(10);

        builder.HasOne(d => d.Kind).WithMany(p => p.BeneficiaryRelPercents).HasForeignKey(d => d.KindId);

    }
}
