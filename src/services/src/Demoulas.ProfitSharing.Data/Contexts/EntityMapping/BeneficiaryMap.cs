using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BeneficiaryMap : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        _ = builder.ToTable("BENEFICIARY");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(d => d.Psn, "IX_PSN");
        _ = builder.Property(c => c.Psn).IsRequired().HasPrecision(11).ValueGeneratedNever().HasColumnName("PSN");

        _ = builder.HasIndex(d => d.Ssn, "IX_SSN");
        _ = builder.Property(c => c.Ssn).IsRequired().HasPrecision(9).HasColumnName("SSN");

        _ = builder.Property(b => b.FirstName).IsRequired().HasMaxLength(30).HasColumnName("FIRST_NAME");
        _ = builder.Property(b => b.MiddleName).HasMaxLength(30).HasColumnName("MIDDLE_NAME");
        _ = builder.Property(b => b.LastName).IsRequired().HasMaxLength(30).HasColumnName("LAST_NAME");

        _ = builder.Property(b => b.DateOfBirth).HasColumnType("DATE").HasConversion<DateOnlyConverter>().HasColumnName("DATE_OF_BIRTH");

        _ = builder.Property(b => b.KindId).HasColumnName("KIND_ID");

        _ = builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("STREET").HasComment("Street").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("STREET2").HasComment("Street2");
            address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("STREET3").HasComment("Street3");
            address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("STREET4").HasComment("Street4");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("CITY").HasComment("City").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE").HasComment("State").IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE").HasComment("Postal Code").IsRequired();
            address.Property(a => a.CountryIso).HasMaxLength(2).HasColumnName("COUNTRY_ISO").HasDefaultValue(Country.Constants.Us);

            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryIso);
        });

        _ = builder.OwnsOne(e => e.ContactInfo, contact =>
        {
            contact.Property(a => a.PhoneNumber).HasMaxLength(16).HasColumnName("PHONE_NUMBER");
            contact.Property(a => a.MobileNumber).HasMaxLength(16).HasColumnName("MOBILE_NUMBER");
            contact.Property(a => a.EmailAddress).HasMaxLength(64).HasColumnName("EMAIL_ADDRESS");
        });

       _ = builder.Property(e => e.Distribution).HasPrecision(9, 2).HasColumnName("DISTRIBUTION");
       _ = builder.Property(e => e.Amount).HasPrecision(9, 2).HasColumnName("AMOUNT");
       _ = builder.Property(e => e.Earnings).HasPrecision(9, 2).HasColumnName("EARNINGS");
       _ = builder.Property(e => e.SecondaryEarnings).HasPrecision(9, 2).HasColumnName("SECONDARY_EARNINGS");

       _ = builder.Property(e => e.Percent).HasColumnName("PERCENT")
           .HasColumnType("numeric(3,0)")
           .HasPrecision(3)
           .IsRequired();

       _ = builder.Property(e => e.Relationship).HasColumnName("RELATIONSHIP").HasMaxLength(10);


       _ = builder.HasOne(d => d.Kind)
           .WithMany(p => p.Beneficiaries)
           .HasForeignKey(d => d.KindId);
    }
}
