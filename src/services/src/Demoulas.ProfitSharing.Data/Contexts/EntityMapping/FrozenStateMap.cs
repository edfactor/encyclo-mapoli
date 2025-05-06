using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class FrozenStateMap : IEntityTypeConfiguration<FrozenState>
{
    public void Configure(EntityTypeBuilder<FrozenState> builder)
    {
        _ = builder.ToTable("FROZEN_STATE");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        _ = builder.Property(x => x.ProfitYear)
            .HasColumnName("PROFIT_YEAR")
            .IsRequired();

        _ = builder.Property(x => x.FrozenBy)
            .HasColumnName("FROZEN_BY")
            .HasMaxLength(64)
            .IsRequired();

        _ = builder.Property(x => x.AsOfDateTime)
            .HasColumnName("AS_OF_DATETIME")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .IsRequired();

        _ = builder.Property(x => x.CreatedDateTime)
            .HasColumnName("CREATED_DATETIME")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .HasDefaultValueSql("SYSTIMESTAMP")
            .ValueGeneratedOnAdd();

        _ = builder.Property(x => x.IsActive)
            .HasColumnName("IS_ACTIVE");
    }
}
