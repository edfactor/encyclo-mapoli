using Demoulas.ProfitSharing.Data.Entities.FileTransfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Configurations.FileTransfer;

/// <summary>
/// Entity Framework Core configuration for FtpOperationLog entity.
/// </summary>
public sealed class FtpOperationLogConfiguration : IEntityTypeConfiguration<FtpOperationLog>
{
    public void Configure(EntityTypeBuilder<FtpOperationLog> builder)
    {
        builder.ToTable("FtpOperationLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CheckRunWorkflowId)
            .IsRequired();

        builder.Property(e => e.OperationType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Destination)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.IsSuccess)
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.DurationMs)
            .IsRequired();

        builder.Property(e => e.Timestamp)
            .IsRequired();

        builder.Property(e => e.UserId)
            .IsRequired();

        // Index for FK queries
        builder.HasIndex(e => e.CheckRunWorkflowId)
            .HasDatabaseName("IX_FtpOperationLogs_CheckRunWorkflowId");

        // Index for time-based queries
        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_FtpOperationLogs_Timestamp");
    }
}
