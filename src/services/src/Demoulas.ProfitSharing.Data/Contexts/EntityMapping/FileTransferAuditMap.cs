using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

/// <summary>
/// Entity Framework Core configuration for FileTransferAudit entity.
/// Maps audit records for file transfer operations to external systems.
/// </summary>
internal sealed class FileTransferAuditMap : IEntityTypeConfiguration<FileTransferAudit>
{
    public void Configure(EntityTypeBuilder<FileTransferAudit> builder)
    {
        _ = builder.ToTable("FILE_TRANSFER_AUDIT");

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        _ = builder.Property(e => e.Timestamp)
            .IsRequired();

        _ = builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        _ = builder.Property(e => e.Destination)
            .IsRequired()
            .HasMaxLength(500);

        _ = builder.Property(e => e.FileSize)
            .IsRequired();

        _ = builder.Property(e => e.TransferDurationMs)
            .IsRequired();

        _ = builder.Property(e => e.IsSuccess)
            .IsRequired();

        _ = builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        _ = builder.Property(e => e.UserName)
            .HasMaxLength(50);

        _ = builder.Property(e => e.CsvContent);

        _ = builder.Property(e => e.CheckRunWorkflowId);

        // Index for timestamp-based queries
        _ = builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_FILE_TRANSFER_AUDIT_TIMESTAMP");

        // Index for FK queries
        _ = builder.HasIndex(e => e.CheckRunWorkflowId)
            .HasDatabaseName("IX_FILE_TRANSFER_AUDIT_WORKFLOW_ID");

        // Index for filename searches
        _ = builder.HasIndex(e => e.FileName)
            .HasDatabaseName("IX_FILE_TRANSFER_AUDIT_FILENAME");
    }
}
