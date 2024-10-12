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
        builder.Property(e => e.Id).ValueGeneratedNever().HasColumnName("ID").IsRequired();
        builder.Property(e => e.Name).HasMaxLength(64).HasColumnName("NAME").IsRequired();

        builder.HasData(DistributionRequestReason.Reasons.Select((reason, dex) => new DistributionRequestReason { Id = (byte)dex, Name = reason }));
    }
}
