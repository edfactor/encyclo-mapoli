using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionStatusMap : IEntityTypeConfiguration<DistributionStatus>
{
    public void Configure(EntityTypeBuilder<DistributionStatus> builder)
    {
        builder.ToTable("DISTRIBUTION_STATUS");
        builder.HasKey(c => c.Id).HasName("PK_DISTRIBUTION_STATUS");
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.HasData(
            new DistributionStatus { Id = DistributionStatus.Constants.ManualCheck, Name = "Manual Check" },
            new DistributionStatus { Id = DistributionStatus.Constants.PurgeRecord, Name = "Purge this Record" },
            new DistributionStatus { Id = DistributionStatus.Constants.RequestOnHold, Name = "Request is on hold" },
            new DistributionStatus { Id = DistributionStatus.Constants.Override, Name = "Override vested amount in check (death or > 64 and 5 years vested" },
            new DistributionStatus { Id = DistributionStatus.Constants.PaymentMade, Name = "Payment as been made" },
            new DistributionStatus { Id = DistributionStatus.Constants.OkayToPay, Name = "Request is OK to pay" },
            new DistributionStatus { Id = DistributionStatus.Constants.PurgeAllRecordsForSsn, Name = "Purge all records for the SSN" },
            new DistributionStatus { Id = DistributionStatus.Constants.PurgeAllRecordsForSsn2, Name = "Purge all records for the SSN" }
        );
    }
}
