using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionRequestTypeMap : IEntityTypeConfiguration<DistributionRequestType>
{

    public void Configure(EntityTypeBuilder<DistributionRequestType> builder)
    {
        builder.ToTable("DISTRIBUTION_REQUEST_TYPE");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever().HasColumnName("ID").IsRequired();
        builder.Property(e => e.Name).HasMaxLength(32).HasColumnName("NAME").IsRequired();

        builder.HasData(DistributionRequestType.Types.Select((reason, dex) => new DistributionRequestType { Id = (byte)dex, Name = reason }));
    }

}
