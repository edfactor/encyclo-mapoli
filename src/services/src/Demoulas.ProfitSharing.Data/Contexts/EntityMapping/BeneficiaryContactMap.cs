using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class BeneficiaryContactMap : ModifiedBaseMap<BeneficiaryContact>
{
    public override void Configure(EntityTypeBuilder<BeneficiaryContact> builder)
    {
        base.Configure(builder);

        _ = builder.ToTable("BENEFICIARY_CONTACT");
        _ = builder.HasKey(c => c.Id);

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();

        _ = builder.HasIndex(d => d.Ssn, "IX_SSN");
        _ = builder.Property(c => c.Ssn).IsRequired().HasPrecision(9).HasColumnName("SSN");

        _ = builder.Property(b => b.DateOfBirth).HasColumnType("DATE").HasConversion<DateOnlyConverter>().HasColumnName("DATE_OF_BIRTH");
        _ = builder.Property(b => b.CreatedDate)
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>()
            .HasColumnName("CREATED_DATE")
            .HasDefaultValueSql("SYSDATE")
            .ValueGeneratedOnAdd();


        _ = builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(56).HasColumnName("STREET").HasComment("Street").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(56).HasColumnName("STREET2").HasComment("Street2");
            address.Property(a => a.Street3).HasMaxLength(56).HasColumnName("STREET3").HasComment("Street3");
            address.Property(a => a.Street4).HasMaxLength(56).HasColumnName("STREET4").HasComment("Street4");
            address.Property(a => a.City).HasMaxLength(36).HasColumnName("CITY").HasComment("City").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE").HasComment("State").IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE").HasComment("Postal Code").IsRequired();
            address.Property(a => a.CountryIso).HasMaxLength(2).HasColumnName("COUNTRY_ISO").HasDefaultValue(Country.Constants.Us);

            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryIso);
        });

        builder.OwnsOne(e => e.ContactInfo, contact =>
        {
            _ = contact.Property(e => e.FullName)
                .HasMaxLength(84)
                .HasComment("FullName")
                .HasColumnName("FULL_NAME")
                .IsRequired();

            _ = contact.Property(e => e.LastName)
                .HasMaxLength(30)
                .HasComment("LastName")
                .HasColumnName("LAST_NAME")
                .IsRequired();

            _ = contact.Property(e => e.FirstName)
                .HasMaxLength(30)
                .HasComment("FirstName")
                .HasColumnName("FIRST_NAME")
                .IsRequired();

            _ = contact.Property(e => e.MiddleName)
                .HasMaxLength(25)
                .HasColumnName("MIDDLE_NAME")
                .HasComment("MiddleName");

            contact.Property(a => a.PhoneNumber).HasMaxLength(16).HasColumnName("PHONE_NUMBER");
            contact.Property(a => a.MobileNumber).HasMaxLength(16).HasColumnName("MOBILE_NUMBER");
            contact.Property(a => a.EmailAddress).HasMaxLength(84).HasColumnName("EMAIL_ADDRESS");
        });



        _ = builder.HasMany(d => d.Beneficiaries)
            .WithOne(p => p.Contact)
            .HasForeignKey(d => d.BeneficiaryContactId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = builder.HasMany(d => d.BeneficiarySsnChangeHistories)
            .WithOne(p => p.BeneficiaryContact)
            .HasForeignKey(d => d.BeneficiaryContactId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
