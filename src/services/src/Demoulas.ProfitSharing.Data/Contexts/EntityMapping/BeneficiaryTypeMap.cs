using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class BeneficiaryTypeMap : IEntityTypeConfiguration<BeneficiaryType>
{
    public void Configure(EntityTypeBuilder<BeneficiaryType> builder)
    {
        builder.ToTable("BeneficiaryType");

        builder.HasKey(c => c.Id).HasName("ID");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(20).HasColumnName("NAME");

        builder.HasMany(b => b.Profits).WithOne(p => p.BeneficiaryType);

        builder.HasData(
            new PayFrequency { Id = BeneficiaryType.Constants.Employee, Name = "Employee" },
            new PayFrequency { Id = BeneficiaryType.Constants.Beneficiary, Name = "Beneficiary" }
        );
    }
}
