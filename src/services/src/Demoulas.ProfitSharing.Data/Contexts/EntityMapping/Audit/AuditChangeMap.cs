using Demoulas.ProfitSharing.Data.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Audit;

public sealed class AuditChangeMap : IEntityTypeConfiguration<AuditChange>
{
    public void Configure(EntityTypeBuilder<AuditChange> builder)
    {
        _ = builder.ToTable("AUDIT_CHANGE");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.Property(e => e.ColumnName)
            .HasMaxLength(50)
            .HasColumnName("COLUMN_NAME");

        _ = builder.Property(e => e.OriginalValue)
            .HasMaxLength(512)
            .HasColumnName("ORIGINAL_VALUE");

        _ = builder.Property(e => e.NewValue)
            .HasMaxLength(512)
            .HasColumnName("NEW_VALUE");

        _ = builder.Property(e => e.UserName)
            .HasMaxLength(24)
            .HasColumnName("USER_NAME")
            .HasDefaultValueSql("SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");


        _ = builder.Property(e => e.ChangeDate)
            .HasColumnName("CHANGE_DATE")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP");
    }
}
