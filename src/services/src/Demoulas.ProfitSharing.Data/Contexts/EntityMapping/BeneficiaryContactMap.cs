using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.Common.Data.Contexts.ValueConverters;
using System.Reflection.Emit;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

public sealed class BeneficiaryContactMap : IEntityTypeConfiguration<BeneficiaryContact>
{
    public void Configure(EntityTypeBuilder<BeneficiaryContact> builder)
    {
        _ = builder.ToTable("BENEFICIARY_CONTACT");

        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(d => d.Ssn, "IX_SSN");
        _ = builder.Property(c => c.Ssn).IsRequired().HasPrecision(9).HasColumnName("SSN");

        _ = builder.Property(b => b.DateOfBirth)
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>()
            .HasColumnName("DATE_OF_BIRTH");
        _ = builder.Property(b => b.CreatedDate)
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>()
            .HasColumnName("CREATED_DATE")
            .HasDefaultValueSql("SYSDATE")
            .ValueGeneratedOnAdd();

        _ = builder.Property(e => e.FullName)
            .HasMaxLength(84)
            .HasComment("FullName")
            .HasColumnName("FULL_NAME")
            .IsRequired();

        _ = builder.Property(e => e.LastName)
            .HasMaxLength(30)
            .HasComment("LastName")
            .HasColumnName("LAST_NAME")
            .IsRequired();

        _ = builder.Property(e => e.FirstName)
            .HasMaxLength(30)
            .HasComment("FirstName")
            .HasColumnName("FIRST_NAME")
            .IsRequired();

        _ = builder.Property(e => e.MiddleName)
            .HasMaxLength(25)
            .HasColumnName("MIDDLE_NAME")
            .HasComment("MiddleName");

        builder.Property(a => a.PhoneNumber).HasMaxLength(16).HasColumnName("PHONE_NUMBER");
        builder.Property(a => a.MobileNumber).HasMaxLength(16).HasColumnName("MOBILE_NUMBER");
        builder.Property(a => a.EmailAddress).HasMaxLength(84).HasColumnName("EMAIL_ADDRESS");


        _ = builder.OwnsOne(e => e.Address, address =>
        {
            _ = address.Property(a => a.Street).HasMaxLength(30).HasColumnName("STREET").HasComment("Street").IsRequired();
            _ = address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("STREET2").HasComment("Street2");
            _ = address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("STREET3").HasComment("Street3");
            _ = address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("STREET4").HasComment("Street4");
            _ = address.Property(a => a.City).HasMaxLength(25).HasColumnName("CITY").HasComment("City").IsRequired();
            _ = address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE").HasComment("State").IsRequired();
            _ = address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE").HasComment("Postal Code").IsRequired();
            _ = address.Property(a => a.CountryIso).HasMaxLength(2).HasColumnName("COUNTRY_ISO").HasDefaultValue(Country.Constants.Us);

            _ = address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryIso);
        });

        _ = builder.HasMany(d => d.Beneficiaries)
            .WithOne(p => p.Contact)
            .HasForeignKey(d => d.BeneficiaryContactId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
