using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public class FakeSsnMap : IEntityTypeConfiguration<FakeSsn>
{
    public void Configure(EntityTypeBuilder<FakeSsn> builder)
    {
        // Map to the table "FakeSsns"
        builder.ToTable("FAKE_SSNS");

        // Define the primary key
        builder.HasKey(f => f.Id);

        // Configure the Ssn property as required and unique
        builder.Property(f => f.Ssn).HasPrecision(9)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("SSN");

        builder.HasIndex(x => x.Ssn, "IX_FAKE_SSN").IsUnique();
    }
}
