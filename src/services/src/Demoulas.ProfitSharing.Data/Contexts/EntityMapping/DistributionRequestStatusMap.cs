using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionRequestStatusMap : IEntityTypeConfiguration<DistributionRequestStatus>
{
    public void Configure(EntityTypeBuilder<DistributionRequestStatus> builder)
    {
        builder.ToTable("DISTRIBUTION_REQUEST_STATUS");
        builder.HasKey(c => c.Id);
        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasColumnName("ID");
        builder.Property(c => c.Name).HasMaxLength(20);
        builder.HasData(
                new DistributionRequestStatus { Id = DistributionRequestStatus.Constants.NEW_ENTRY, Name = "NEW_ENTRY" },
                new DistributionRequestStatus { Id = DistributionRequestStatus.Constants.READY_FOR_REVIEW, Name = "READY_FOR_REVIEW" },
                new DistributionRequestStatus { Id = DistributionRequestStatus.Constants.IN_COMMITTEE_REVIEW, Name = "IN_COMMITTEE_REVIEW" },
                new DistributionRequestStatus { Id = DistributionRequestStatus.Constants.APPROVED, Name = "APPROVED" },
                new DistributionRequestStatus { Id = DistributionRequestStatus.Constants.DECLINED, Name = "DECLINED" },
                new DistributionRequestStatus { Id = DistributionRequestStatus.Constants.PROCESSED, Name = "PROCESSED" }
            );
    }


}
