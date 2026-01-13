using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class PayFrequencyMap : IEntityTypeConfiguration<PayFrequency>
{
    public void Configure(EntityTypeBuilder<PayFrequency> builder)
    {
        builder.ToTable("PAY_FREQUENCY");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(64)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasData(
            new PayFrequency { Id = PayFrequency.Constants.Weekly, Name = "Weekly" },
            new PayFrequency { Id = PayFrequency.Constants.Monthly, Name = "Monthly" }
        );
    }
}
