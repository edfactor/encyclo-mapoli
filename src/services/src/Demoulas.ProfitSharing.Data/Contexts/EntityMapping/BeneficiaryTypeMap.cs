using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class BeneficiaryTypeMap : IEntityTypeConfiguration<BeneficiaryType>
{
    public void Configure(EntityTypeBuilder<BeneficiaryType> builder)
    {
        builder.ToTable("BENEFICIARY_TYPE");

        builder.HasKey(c => c.Id).HasName("ID");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(20).HasColumnName("NAME");

        builder.HasData(
            new BeneficiaryType { Id = BeneficiaryType.Constants.Employee, Name = "Employee" },
            new BeneficiaryType { Id = BeneficiaryType.Constants.Beneficiary, Name = "Beneficiary" }
        );
    }
}
