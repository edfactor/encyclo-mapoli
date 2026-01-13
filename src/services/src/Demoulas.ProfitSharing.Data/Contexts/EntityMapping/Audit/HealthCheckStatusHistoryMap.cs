using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Audit;

public sealed class HealthCheckStatusHistoryMap : IEntityTypeConfiguration<HealthCheckStatusHistory>
{
    public void Configure(EntityTypeBuilder<HealthCheckStatusHistory> builder)
    {
        _ = builder.ToTable("_HEALTH_CHECK_STATUS_HISTORY");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        _ = builder.Property(d => d.Key).HasColumnName("KEY").HasMaxLength(128).IsRequired();
        _ = builder.Property(d => d.Status).HasColumnName("STATUS").HasMaxLength(24).IsRequired();
        _ = builder.Property(d => d.Description).HasColumnName("DESCRIPTION").IsRequired(false);
        _ = builder.Property(d => d.Exception).HasColumnName("EXCEPTION").IsRequired(false);
        _ = builder.Property(d => d.Duration).HasColumnName("DURATION").IsRequired();
        _ = builder.Property(d => d.CreatedAt).HasColumnName("CREATED_AT").IsRequired();

    }
}
