using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BeneficiaryMap : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        _ = builder.ToTable("BENEFICIARY");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(e => e.PsnSuffix, "IX_PsnSuffix");
        _ = builder.Property(e => e.PsnSuffix)
            .HasPrecision(5)
            .HasColumnName("PSN_SUFFIX");

        _ = builder.HasIndex(e => e.BadgeNumber, "IX_BadgeNumber");
        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("BADGE_NUMBER");

        _ = builder.Property(e => e.DemographicId)
            .HasPrecision(11)
            .ValueGeneratedNever()
            .HasColumnName("DEMOGRAPHIC_ID");

        _ = builder.Property(b => b.KindId).HasColumnName("KIND_ID");
        _ = builder.Property(b => b.BeneficiaryContactId).HasColumnName("BENEFICIARY_CONTACT_ID");

        _ = builder.Property(e => e.Distribution).HasPrecision(9, 2).HasColumnName("DISTRIBUTION");
        _ = builder.Property(e => e.Amount).HasPrecision(9, 2).HasColumnName("AMOUNT");
        _ = builder.Property(e => e.Earnings).HasPrecision(9, 2).HasColumnName("EARNINGS");
        _ = builder.Property(e => e.SecondaryEarnings).HasPrecision(9, 2).HasColumnName("SECONDARY_EARNINGS");

        _ = builder.Property(e => e.Percent).HasColumnName("PERCENT")
            .HasColumnType("numeric(3,0)")
            .HasPrecision(3)
            .IsRequired();

        _ = builder.Property(e => e.Relationship).HasColumnName("RELATIONSHIP").HasMaxLength(10);


        _ = builder.HasOne(d => d.Kind)
            .WithMany(p => p.Beneficiaries)
            .HasForeignKey(d => d.KindId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = builder.HasOne(d => d.Contact)
            .WithMany(p => p.Beneficiaries)
            .HasForeignKey(d => d.BeneficiaryContactId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
