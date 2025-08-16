using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionFrequencyMap : IEntityTypeConfiguration<DistributionFrequency>
{
    public void Configure(EntityTypeBuilder<DistributionFrequency> builder)
    {
        builder.ToTable("DISTRIBUTION_FREQUENCY");
        builder.HasKey(c => c.Id).HasName("PK_DISTRIBUTION_FREQUENCY");
        builder.Property(c => c.Name).HasMaxLength(20);
        builder.HasData(
            new DistributionFrequency { Id = DistributionFrequency.Constants.Monthly, Name = "Monthly" },
            new DistributionFrequency { Id = DistributionFrequency.Constants.Annually, Name = "Annually" },
            new DistributionFrequency { Id = DistributionFrequency.Constants.Quarterly, Name = "Quarterly" },
            new DistributionFrequency { Id = DistributionFrequency.Constants.Hardship, Name = "Hardship" },
            new DistributionFrequency { Id = DistributionFrequency.Constants.PayDirect, Name = "Pay Direct" },
            new DistributionFrequency { Id = DistributionFrequency.Constants.RolloverDirect, Name = "Rollover Direct" }
        );
    }
}
