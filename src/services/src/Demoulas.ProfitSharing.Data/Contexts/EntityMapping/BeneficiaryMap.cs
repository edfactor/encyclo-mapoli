using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BeneficiaryMap : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        _ = builder.HasKey(c => c.PSN);
        _ = builder.ToTable("BENEFICIARY");

        _ = builder.Property(c => c.PSN).IsRequired().HasPrecision(11).ValueGeneratedNever();

        _ = builder.HasIndex(c => c.SSN, "IX_SSN");
        _ = builder.Property(c => c.SSN).IsRequired().HasPrecision(9);

        _ = builder.Property(b => b.FirstName).IsRequired().HasMaxLength(30).HasColumnName("FIRST_NAME");
        _ = builder.Property(b => b.MiddleName).HasMaxLength(30).HasColumnName("MIDDLE_NAME");
        _ = builder.Property(b => b.LastName).IsRequired().HasMaxLength(30).HasColumnName("LAST_NAME");

        _ = builder.Property(b => b.DateOfBirth).HasColumnType("DATE").HasConversion<DateOnlyConverter>().HasColumnName("DATE_OF_BIRTH");

        _ = builder.Property(b => b.KindId).IsRequired().HasColumnName("KIND_ID");

        _ = builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("STREET").HasComment("Street").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("STREET2").HasComment("Street2");
            address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("STREET3").HasComment("Street3");
            address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("STREET4").HasComment("Street4");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("CITY").HasComment("City").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE").HasComment("State").IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE").HasComment("Postal Code").IsRequired();
            address.Property(a => a.CountryISO).HasMaxLength(2).HasColumnName("COUNTRY_ISO").HasDefaultValue(Country.Constants.US);

            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryISO);
        });

        _ = builder.OwnsOne(e => e.ContactInfo, contact =>
        {
            contact.Property(a => a.PhoneNumber).HasMaxLength(15).HasColumnName("PHONE_NUMBER");
            contact.Property(a => a.MobileNumber).HasMaxLength(15).HasColumnName("MOBILE_NUMBER");
            contact.Property(a => a.EmailAddress).HasMaxLength(50).HasColumnName("EMAIL_ADDRESS");
        });

        _ = builder.HasOne(d => d.Kind)
            .WithMany(p => p.Beneficiaries)
            .HasForeignKey(d => d.KindId);

       _ = builder.Property(e => e.Distribution).HasPrecision(9, 2).HasColumnName("DISTRIBUTION");
       _ = builder.Property(e => e.Amount).HasPrecision(9, 2).HasColumnName("AMOUNT");
       _ = builder.Property(e => e.Earnings).HasPrecision(9, 2).HasColumnName("EARNINGS");
       _ = builder.Property(e => e.SecondaryEarnings).HasPrecision(9, 2).HasColumnName("SECONDARY_EARNINGS");

    }
}
