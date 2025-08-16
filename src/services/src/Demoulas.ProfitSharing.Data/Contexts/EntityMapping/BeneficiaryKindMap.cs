using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class BeneficiaryKindMap : IEntityTypeConfiguration<BeneficiaryKind>
{
    public void Configure(EntityTypeBuilder<BeneficiaryKind> builder)
    {
        builder.ToTable("BENEFICIARY_KIND");

        builder.HasKey(c => c.Id).HasName("PK_BENEFICIARY_KIND");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(20).HasColumnName("NAME");

        builder.HasMany(b => b.Beneficiaries).WithOne(p => p.Kind);

        builder.HasData(
            new BeneficiaryKind { Id = BeneficiaryKind.Constants.Primary, Name = "Primary" },
            new BeneficiaryKind { Id = BeneficiaryKind.Constants.Secondary, Name = "Secondary" }
        );
    }
}
