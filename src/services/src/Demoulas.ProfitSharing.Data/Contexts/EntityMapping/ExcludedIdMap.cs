using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class ExcludedIdMap : IEntityTypeConfiguration<ExcludedId>
{
    public void Configure(EntityTypeBuilder<ExcludedId> builder)
    {
        _ = builder.ToTable("EXCLUDED_ID");

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(x => x.Id).HasColumnName("ID").IsRequired();
        _ = builder.Property(x => x.ExcludedIdTypeId).HasColumnName("EXCLUDED_ID_TYPE_ID")
            .IsRequired();
        _ = builder.Property(x => x.ExcludedIdValue).HasColumnName("EXCLUDED_ID_VALUE")
            .IsRequired();

        _ = builder.HasOne(e => e.ExcludedType)
            .WithMany()
            .HasForeignKey(x => x.ExcludedIdTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
