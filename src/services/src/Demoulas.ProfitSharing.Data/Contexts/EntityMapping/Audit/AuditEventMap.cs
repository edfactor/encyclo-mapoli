using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Audit;

public sealed class AuditEventMap : IEntityTypeConfiguration<AuditEvent>
{
    public void Configure(EntityTypeBuilder<AuditEvent> builder)
    {
        _ = builder.ToTable("AUDIT_EVENT");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(e => e.TableName, "IX_TableName");
        _ = builder.Property(e => e.TableName)
            .HasMaxLength(30)
            .HasColumnName("TABLE_NAME");

        _ = builder.Property(e => e.Operation)
            .HasMaxLength(12)
            .HasColumnName("OPERATION");

        _ = builder.Property(e => e.PrimaryKey)
            .HasMaxLength(32)
            .HasColumnName("PRIMARY_KEY");


        _ = builder.HasMany(d => d.Changes)
            .WithMany()
            .UsingEntity(e =>
            {
                e.ToTable("AUDIT_CHANGE__AUDIT_EVENT");
            });
    }
}
