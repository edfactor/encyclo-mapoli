using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicMap : IEntityTypeConfiguration<Demographic>
{
    public void Configure(EntityTypeBuilder<Demographic> builder)
    {
        //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887785/DEMOGRAPHICS
        //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31909725/Employee+Hiring+data+fields
        //https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables

        _ = builder.ToTable("Demographics");
        _ = builder.HasKey(e => e.OracleHcmId);

        _ = builder.HasIndex(e => e.SSN, "IX_SSN");
        _ = builder.Property(e => e.SSN)
            .HasPrecision(9)
            .IsRequired()
            .ValueGeneratedNever();

        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7);

        _ = builder.Property(e => e.OracleHcmId)
            .HasPrecision(15)
            .ValueGeneratedNever();

        _ = builder.Property(e => e.FullName)
            .HasMaxLength(60)
            .HasComment("FullName")
            .IsRequired();

        _ = builder.Property(e => e.LastName)
            .HasMaxLength(30)
            .HasComment("LastName")
            .IsRequired();

        _ = builder.Property(e => e.FirstName)
            .HasMaxLength(30)
            .HasComment("FirstName")
            .IsRequired();

        _ = builder.Property(e => e.MiddleName)
            .HasMaxLength(25)
            .HasComment("MiddleName");

        _ = builder.Property(e => e.StoreNumber)
            .HasPrecision(3)
            .HasComment("StoreNumber");

        _ = builder.Property(e => e.DepartmentId)
            .HasPrecision(1)
            .HasComment("Department");

        builder.Property(e => e.PayClassificationId)
            .HasComment("PayClassification")
            .HasPrecision(2);

        _ = builder.Property(e => e.DateOfBirth)
            .HasComment("DateOfBirth")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.HireDate)
            .HasComment("HireDate")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.ReHireDate)
            .HasComment("ReHireDate")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.TerminationCodeId)
            .HasMaxLength(1)
            .HasComment("TerminationCode");


        _ = builder.Property(e => e.TerminationDate)
            .HasComment("TerminationDate")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.FullTimeDate)
            .HasComment("FullTimeDate")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.EmploymentTypeId)
            .HasMaxLength(2)
            .HasComment("EmploymentType");

        _ = builder.Property(e => e.PayFrequencyId)
            .HasMaxLength(1)
            .HasComment("PayFrequency");

        _ = builder.Property(e => e.GenderId)
            .HasMaxLength(1)
            .HasComment("Gender");


        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("STREET").HasComment("Street").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("STREET2").HasComment("Street2");
            address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("STREET3").HasComment("Street3");
            address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("STREET4").HasComment("Street4");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("CITY").HasComment("City").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE").HasComment("State").IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE").HasComment("Postal Code").IsRequired();
            address.Property(a => a.CountryISO).HasMaxLength(2).HasColumnName("COUNTRY_ISO").HasDefaultValue(Constants.US);

            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryISO);
        });

        builder.OwnsOne(e => e.ContactInfo, contact =>
        {
            contact.Property(a => a.PhoneNumber).HasMaxLength(15).HasColumnName("PHONE_NUMBER");
            contact.Property(a => a.MobileNumber).HasMaxLength(15).HasColumnName("MOBILE_NUMBER");
            contact.Property(a => a.EmailAddress).HasMaxLength(50).HasColumnName("EMAIL_ADDRESS");
        });


        builder.HasOne(e => e.PayClassification)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.PayClassificationId);

        builder.HasOne(d => d.Department)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d => d.DepartmentId);

        builder.HasOne(d => d.EmploymentType)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d => d.EmploymentTypeId);

        builder.HasOne(d => d.Gender)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d => d.GenderId);

        builder.HasOne(d => d.PayFrequency)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d => d.PayFrequencyId);

        builder.HasOne(d => d.TerminationCode)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d => d.TerminationCodeId);

        builder.HasMany(d => d.PayProfit)
            .WithOne(p => p.Demographic);

        builder.HasOne(d => d.EmploymentStatus)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d=> d.EmploymentStatusId);
    }
}
