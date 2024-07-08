using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PayFrequencyMap : IEntityTypeConfiguration<PayFrequency>
{
    public void Configure(EntityTypeBuilder<PayFrequency> builder)
    {
        builder.ToTable("PayFrequency");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasData(
            new PayFrequency { Id = PayFrequency.Constants.Weekly, Name = "Weekly" },
            new PayFrequency { Id = PayFrequency.Constants.Monthly, Name = "Monthly" }
        );
    }
}
