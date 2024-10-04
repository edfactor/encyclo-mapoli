using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class ProfitShareCheckMap : IEntityTypeConfiguration<ProfitShareCheck>
{
    public void Configure(EntityTypeBuilder<ProfitShareCheck> builder)
    {
        _ = builder.ToTable("PROFIT_SHARE_CHECK");

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .HasPrecision(15)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        _ = builder.HasIndex(e => e.CheckNumber, "IX_CheckNumber").IsUnique();
        
        _ = builder.Property(e => e.CheckNumber)
            .HasPrecision(15)
            .ValueGeneratedNever()
            .HasColumnName("CHECK_NUMBER");

       
    }
}
