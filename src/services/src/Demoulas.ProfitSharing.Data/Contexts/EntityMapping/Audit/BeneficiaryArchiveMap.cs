using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Audit;

public sealed class BeneficiaryArchiveMap : IEntityTypeConfiguration<BeneficiaryArchive>
{
    public void Configure(EntityTypeBuilder<BeneficiaryArchive> builder)
    {
        _ = builder.ToTable("BENEFICIARY_ARCHIVE");
        _ = builder.HasKey(c => c.ArchiveId);

        _ = builder.Property(d => d.ArchiveId).HasColumnName("ARCHIVE_ID").ValueGeneratedOnAdd();

        _ = builder.Property(d => d.Id).HasColumnName("ID").HasPrecision(9);

        _ = builder.Property(e => e.PsnSuffix)
            .HasPrecision(5)
            .HasColumnName("PSN_SUFFIX");

        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("BADGE_NUMBER");

        _ = builder.Property(e => e.DemographicId)
            .HasPrecision(9)
            .ValueGeneratedNever()
            .HasColumnName("DEMOGRAPHIC_ID");

        _ = builder.Property(b => b.BeneficiaryContactId).HasColumnName("BENEFICIARY_CONTACT_ID");

        _ = builder.Property(e => e.Percent).HasColumnName("PERCENT")
            .HasColumnType("numeric(3,0)")
            .HasPrecision(3)
            .IsRequired();

        _ = builder.Property(e => e.Relationship).HasColumnName("RELATIONSHIP").HasMaxLength(10);

        _ = builder.Property(e => e.DeleteDate).HasColumnName("DELETE_DATE");
        _ = builder.Property(e => e.DeletedBy)
            .HasMaxLength(24)
            .HasColumnName("DELETED_BY")
            .HasDefaultValueSql("SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");
    }
}
