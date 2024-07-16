using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Common;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BeneficiaryMap : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        builder.HasKey(c => c.PSN);
        builder.ToTable("Beneficiary");

        builder.Property(c => c.PSN).IsRequired().HasPrecision(9);

        builder.HasIndex(c => c.SSN, "IX_SSN");
        builder.Property(c => c.SSN).IsRequired().HasPrecision(9);

        builder.Property(b => b.FirstName).IsRequired().HasMaxLength(30);
        builder.Property(b => b.MiddleName).HasMaxLength(30);
        builder.Property(b => b.LastName).IsRequired().HasMaxLength(30);

        builder.Property(b => b.DateOfBirth).HasColumnType("DATE").HasConversion<DateOnlyConverter>();

        
        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasComment("Street").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(30).HasComment("Street2");
            address.Property(a => a.City).HasMaxLength(25).HasComment("City").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasComment("State").IsRequired();
            address.Property(a => a.PostalCode).HasPrecision(9).HasComment("Postal Code").IsRequired().HasConversion<PostalCodeConverter>();
            address.Property(a => a.CountryISO).HasMaxLength(2).HasColumnName("CountryISO").HasDefaultValue(Constants.US);

            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryISO);
        });

        builder.OwnsOne(e => e.ContactInfo, contact =>
        {
            contact.Property(a => a.PhoneNumber).HasMaxLength(15).HasComment("PhoneNumber").IsRequired();
            contact.Property(a => a.MobileNumber).HasMaxLength(15).HasColumnName("MobileNumber");
            contact.Property(a => a.EmailAddress).HasMaxLength(50).HasColumnName("EmailAddress");
        });
    }
}
