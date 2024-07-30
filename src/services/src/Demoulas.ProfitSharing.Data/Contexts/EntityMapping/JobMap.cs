using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities.MassTransit;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class JobMap : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("Job");

        builder.Property(b => b.RequestedBy).HasMaxLength(30);
        builder.Property(b => b.StartMethod).IsRequired().HasPrecision(2);
        builder.Property(b => b.JobType).IsRequired().HasPrecision(2);
        builder.Property(b => b.StatusEnum).IsRequired().HasPrecision(2);
        builder.Property(b => b.Started).IsRequired().HasColumnType("DATE");
        builder.Property(b => b.Completed).HasColumnType("DATE");
    }
}
