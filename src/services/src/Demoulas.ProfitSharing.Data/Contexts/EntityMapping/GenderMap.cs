using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class GenderMap : IEntityTypeConfiguration<Gender>
{
    public void Configure(EntityTypeBuilder<Gender> builder)
    {
        builder.ToTable("GENDER");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(32)
            .HasColumnName("NAME")
            .IsRequired();

        builder.HasData(
            new Gender { Id = Gender.Constants.Male, Name = "Male" },
            new Gender { Id = Gender.Constants.Female, Name = "Female" },
            new Gender { Id = Gender.Constants.Nonbinary, Name = "Nonbinary" },
            new Gender { Id = Gender.Constants.Unknown, Name = "Unknown" }
        );
    }
}
