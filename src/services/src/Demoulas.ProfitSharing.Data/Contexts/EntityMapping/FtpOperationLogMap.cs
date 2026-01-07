using Demoulas.ProfitSharing.Data.Entities.FileTransfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

/// <summary>
/// Entity Framework Core configuration for FtpOperationLog entity.
/// Maps audit log for FTP operations performed during check run workflows.
/// </summary>
internal sealed class FtpOperationLogMap : IEntityTypeConfiguration<FtpOperationLog>
{
    public void Configure(EntityTypeBuilder<FtpOperationLog> builder)
    {
        _ = builder.ToTable("FTP_OPERATION_LOG");

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        _ = builder.Property(e => e.CheckRunWorkflowId)
            .IsRequired();

        _ = builder.Property(e => e.OperationType)
            .IsRequired()
            .HasConversion<int>();

        _ = builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        _ = builder.Property(e => e.Destination)
            .IsRequired()
            .HasMaxLength(500);

        _ = builder.Property(e => e.IsSuccess)
            .IsRequired();

        _ = builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        _ = builder.Property(e => e.DurationMs)
            .IsRequired();

        _ = builder.Property(e => e.Timestamp)
            .IsRequired();

        _ = builder.Property(e => e.UserName)
            .HasMaxLength(50);

        // Index for FK queries
        _ = builder.HasIndex(e => e.CheckRunWorkflowId)
            .HasDatabaseName("IX_FTP_OPERATION_LOG_WORKFLOW_ID");

        // Index for time-based queries
        _ = builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_FTP_OPERATION_LOG_TIMESTAMP");
    }
}
