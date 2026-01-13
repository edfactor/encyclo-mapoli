using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public class DistributionRequestReasonMap : IEntityTypeConfiguration<DistributionRequestReason>
{
    public void Configure(EntityTypeBuilder<DistributionRequestReason> builder)
    {
        builder.ToTable("DISTRIBUTION_REQUEST_REASON");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasPrecision(2)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();
        builder.Property(e => e.Name).HasMaxLength(32).HasColumnName("NAME").IsRequired();


        builder.HasData(
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.CAR, Name = "CAR" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.EDUCATION_EXP, Name = "EDUCATION_EXP" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.EVICTION_OR_FORECLOSE, Name = "EVICTION_OR_FORECLOSE" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.FUNERAL_EXP, Name = "FUNERAL_EXP" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.HOME_PURCHASE, Name = "HOME_PURCHASE" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.HOME_REPAIR, Name = "HOME_REPAIR" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.MEDICAL_DENTAL, Name = "MEDICAL_DENTAL" },
            new DistributionRequestReason { Id = DistributionRequestReason.Constants.OTHER, Name = "OTHER" }
        );

    }
}
