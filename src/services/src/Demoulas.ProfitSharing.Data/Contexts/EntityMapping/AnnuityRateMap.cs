using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class AnnuityRateMap : ModifiedBaseMap<AnnuityRate>
{
    public override void Configure(EntityTypeBuilder<AnnuityRate> builder)
    {
        base.Configure(builder);
        _ = builder.ToTable("ANNUITY_RATE");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.Property(c => c.Year).IsRequired().HasColumnName("YEAR").HasPrecision(4);
        _ = builder.Property(c => c.Age).IsRequired().HasColumnName("AGE").HasPrecision(3);
        _ = builder.Property(c => c.SingleRate).IsRequired().HasColumnName("SINGLE_RATE").HasPrecision(6, 4);
        _ = builder.Property(c => c.JointRate).IsRequired().HasColumnName("JOINT_RATE").HasPrecision(6, 4);

        _ = builder.HasIndex(c => new { c.Year, c.Age }, "IX_YEAR_AGE")
            .IsUnique()
            .HasDatabaseName("IX_ANNUITY_RATE_YEAR_AGE");
    }
}
