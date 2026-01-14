using Demoulas.ProfitSharing.Data.Entities.CheckRun;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

/// <summary>
/// Entity Framework Core configuration for CheckRunWorkflow entity.
/// Maps workflow state tracking for check printing runs including step progression and reprint limits.
/// </summary>
internal sealed class CheckRunWorkflowMap : IEntityTypeConfiguration<CheckRunWorkflow>
{
    public void Configure(EntityTypeBuilder<CheckRunWorkflow> builder)
    {
        _ = builder.ToTable("CHECK_RUN_WORKFLOW");

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        _ = builder.Property(e => e.ProfitYear)
            .IsRequired();

        _ = builder.Property(e => e.CheckRunDate)
            .IsRequired();

        _ = builder.Property(e => e.StepNumber)
            .IsRequired();

        _ = builder.Property(e => e.StepStatus)
            .IsRequired()
            .HasConversion<int>();

        _ = builder.Property(e => e.CheckNumber);

        _ = builder.Property(e => e.ReprintCount)
            .IsRequired()
            .HasDefaultValue(0);

        _ = builder.Property(e => e.MaxReprintCount)
            .IsRequired()
            .HasDefaultValue(2);

        _ = builder.Property(e => e.CreatedByUserName)
            .HasMaxLength(50);

        _ = builder.Property(e => e.CreatedDate)
            .IsRequired();

        _ = builder.Property(e => e.ModifiedByUserName)
            .HasMaxLength(50);

        _ = builder.Property(e => e.ModifiedDate);

        // Index for profit year lookups
        _ = builder.HasIndex(e => e.ProfitYear)
            .HasDatabaseName("IX_CHECK_RUN_WORKFLOW_PROFIT_YEAR");

        // Index for date-based queries (reprint constraints)
        _ = builder.HasIndex(e => e.CheckRunDate)
            .HasDatabaseName("IX_CHECK_RUN_WORKFLOW_RUN_DATE");

        // Composite index for status queries
        _ = builder.HasIndex(e => new { e.ProfitYear, e.StepStatus })
            .HasDatabaseName("IX_CHECK_RUN_WORKFLOW_YEAR_STATUS");
    }
}
