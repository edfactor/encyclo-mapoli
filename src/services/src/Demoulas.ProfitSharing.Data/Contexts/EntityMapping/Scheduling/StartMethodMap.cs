using Demoulas.ProfitSharing.Data.Entities.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Scheduling;

internal sealed class StartMethodMap : IEntityTypeConfiguration<StartMethod>
{
    public void Configure(EntityTypeBuilder<StartMethod> builder)
    {
        builder.ToTable("JobStartMethod");

        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id).HasPrecision(3);
        builder.Property(b => b.Name).HasMaxLength(30);
        builder.HasMany(b => b.Jobs).WithOne(j => j.StartMethod).HasForeignKey(j => j.StartMethodId);


        builder.HasData(
            new StartMethod { Id = StartMethod.Constants.System, Name = "System" },
            new StartMethod { Id = StartMethod.Constants.OnDemand, Name = "OnDemand" }
        );
    }
}
