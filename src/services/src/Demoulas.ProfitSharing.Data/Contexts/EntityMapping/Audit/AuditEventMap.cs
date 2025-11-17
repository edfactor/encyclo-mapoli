using System.Text.Json;
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

        _ = builder.Property(d => d.Id).HasColumnName("AUDIT_EVENT_ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(e => e.TableName, "IX_TableName");
        _ = builder.Property(e => e.TableName)
            .HasMaxLength(128)
            .HasColumnName("TABLE_NAME");

        _ = builder.Property(e => e.Operation)
            .HasMaxLength(24)
            .HasColumnName("OPERATION");

        _ = builder.Property(e => e.PrimaryKey)
            .HasMaxLength(512)
            .HasColumnName("PRIMARY_KEY");


        _ = builder.Property(e => e.UserName)
            .HasMaxLength(96)
            .HasColumnName("USER_NAME")
            .HasDefaultValueSql("SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");


        _ = builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("CREATED_AT")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP");


        builder.Property(x => x.ChangesJson)
            .HasColumnName("CHANGES_JSON")
            .HasColumnType("CLOB")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, JsonSerializerOptions.Web),
                v => v == null ? null : JsonSerializer.Deserialize<List<AuditChangeEntry>>(v, JsonSerializerOptions.Web)
            )
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<AuditChangeEntry>?>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? null : c.ToList()));

        _ = builder.Property(e => e.ChangesHash)
            .HasMaxLength(64)
            .HasColumnName("CHANGES_HASH");
    }
}
