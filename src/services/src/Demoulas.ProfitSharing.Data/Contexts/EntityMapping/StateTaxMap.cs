using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class StateTaxMap : IEntityTypeConfiguration<StateTax>
{
    public void Configure(EntityTypeBuilder<StateTax> builder)
    {
        builder.ToTable("STATE_TAX");
        builder.HasKey(x => x.Abbreviation);
        builder.Property(x => x.Abbreviation).HasMaxLength(2);
        builder.Property(x => x.Rate).HasPrecision(9, 2);
        builder.Property(x => x.UserModified).HasColumnName("USER_MODIFIED").HasMaxLength(20);
        builder.Property(x => x.DateModified).HasColumnName("DATE_MODIFIED").HasColumnType("DATE").HasConversion<DateOnlyConverter>();

    }
}
