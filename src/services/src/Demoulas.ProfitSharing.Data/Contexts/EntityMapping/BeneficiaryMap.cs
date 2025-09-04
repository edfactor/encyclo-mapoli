using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class BeneficiaryMap : ModifiedBaseMap<Beneficiary>
{
    public override void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        base.Configure(builder);
        
        _ = builder.ToTable("BENEFICIARY");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(e => e.PsnSuffix, "IX_PsnSuffix");
        _ = builder.Property(e => e.PsnSuffix)
            .HasPrecision(5)
            .HasColumnName("PSN_SUFFIX");

        _ = builder.HasIndex(e => e.BadgeNumber, "IX_BADGE_NUMBER");
        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("BADGE_NUMBER");

        _ = builder.Property(e => e.DemographicId)
            .HasPrecision(9)
            .ValueGeneratedNever()
            .HasColumnName("DEMOGRAPHIC_ID");

        _ = builder.Property(b => b.KindId).HasColumnName("KIND_ID");
        _ = builder.Property(b => b.BeneficiaryContactId).HasColumnName("BENEFICIARY_CONTACT_ID");
        
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
