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

        _ = builder.HasKey(e => e.SSN);
        _ = builder.ToTable("Demographics");

        _ = builder.Property(e => e.SSN)
            .HasPrecision(9)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("DEM_SSN");

        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("DEM_BADGE");

        _ = builder.Property(e => e.OracleHcmId)
            .HasPrecision(15)
            .HasColumnName("PY_ASSIGN_ID");

        _ = builder.Property(e => e.FullName)
            .HasMaxLength(60)
            .HasComment("FullName")
            .HasColumnName("PY_NAM")
            .IsRequired();

        _ = builder.Property(e => e.LastName)
            .HasMaxLength(30)
            .HasComment("LastName")
            .HasColumnName("PY_LNAME")
            .IsRequired();

        _ = builder.Property(e => e.FirstName)
            .HasMaxLength(30)
            .HasComment("FirstName")
            .HasColumnName("PY_FNAME")
            .IsRequired();

        _ = builder.Property(e => e.MiddleName)
            .HasMaxLength(25)
            .HasComment("MiddleName")
            .HasColumnName("PY_MNAME");

        _ = builder.Property(e => e.StoreNumber)
            .HasPrecision(3)
            .HasComment("StoreNumber")
            .HasColumnName("PY_STOR");

        _ = builder.Property(e => e.DepartmentId)
            .HasPrecision(1)
            .HasComment("Department")
            .HasColumnName("PY_DP");

        builder.Property(e => e.PayClassificationId)
            .HasColumnName("PY_CLA")
            .HasComment("PayClassification")
            .HasPrecision(2);

        _ = builder.Property(e => e.DateOfBirth)
            .HasPrecision(8)
            .HasComment("DateOfBirth")
            .HasColumnName("PY_DOB")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.HireDate)
            .HasPrecision(8)
            .HasComment("HireDate")
            .HasColumnName("PY_HIRE_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.ReHireDate)
            .HasPrecision(8)
            .HasComment("ReHireDate")
            .HasColumnName("PY_REHIRE_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.TerminationCodeId)
            .HasMaxLength(1)
            .HasComment("TerminationCode")
            .HasColumnName("PY_TERM");


        _ = builder.Property(e => e.TerminationDate)
            .HasPrecision(8)
            .HasComment("TerminationDate")
            .HasColumnName("PY_TERM_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.FullTimeDate)
            .HasPrecision(8)
            .HasComment("FullTimeDate")
            .HasColumnName("PY_FULL_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.EmploymentTypeId)
            .HasMaxLength(2)
            .HasComment("EmploymentType")
            .HasColumnName("PY_FUL");

        _ = builder.Property(e => e.PayFrequencyId)
            .HasMaxLength(1)
            .HasComment("PayFrequency")
            .HasColumnName("PY_FREQ");

        _ = builder.Property(e => e.GenderId)
            .HasMaxLength(1)
            .HasComment("Gender")
            .HasColumnName("PY_GENDER");


        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("PY_ADD").HasComment("Street").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("PY_ADD2").HasComment("Street2");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("PY_CITY").HasComment("City").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("PY_STATE").HasComment("State").IsRequired();
            address.Property(a => a.PostalCode).HasPrecision(9).HasColumnName("PY_ZIP").HasComment("Postal Code").IsRequired().HasConversion<PostalCodeConverter>();
            address.Property(a => a.CountryISO).HasMaxLength(2).HasColumnName("CountryISO").HasDefaultValue(Constants.US);

            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryISO);
        });

        builder.OwnsOne(e => e.ContactInfo, contact =>
        {
            contact.Property(a => a.PhoneNumber).HasMaxLength(15).HasColumnName("PY_EMP_TELNO").HasComment("PhoneNumber").IsRequired();
            contact.Property(a => a.MobileNumber).HasMaxLength(15).HasColumnName("MobileNumber");
            contact.Property(a => a.EmailAddress).HasMaxLength(50).HasColumnName("EmailAddress");
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
    }
}
