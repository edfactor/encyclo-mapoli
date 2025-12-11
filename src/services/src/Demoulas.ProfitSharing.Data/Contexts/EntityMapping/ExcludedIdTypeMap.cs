using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class ExcludedIdTypeMap : IEntityTypeConfiguration<ExcludedIdType>
{
    public void Configure(EntityTypeBuilder<ExcludedIdType> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("EXCLUDED_ID_TYPE");

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("ID");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(128)
            .HasColumnName("NAME");

        builder.HasData(GetPredefinedExcludedIdTypes());
    }

    public static List<ExcludedIdType> GetPredefinedExcludedIdTypes()
    {
        return new List<ExcludedIdType>
        {
            ExcludedIdType.Constants.QPay066TAExclusions,
            ExcludedIdType.Constants.QPay066IExclusions
        };
    }
}
