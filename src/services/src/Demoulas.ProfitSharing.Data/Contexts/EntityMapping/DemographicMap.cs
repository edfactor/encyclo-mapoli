using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicMap : ModifiedBaseMap<Demographic>
{
    public override void Configure(EntityTypeBuilder<Demographic> builder)
    {
        //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887785/DEMOGRAPHICS
        //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31909725/Employee+Hiring+data+fields
        //https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables

        base.Configure(builder);

        _ = builder.ToTable("DEMOGRAPHIC");
        _ = builder.HasKey(e => e.Id);

        _ = builder.HasIndex(e => e.Ssn, "IX_SSN");
        _ = builder.HasIndex(e => e.DateOfBirth, "IX_DOB");
        _ = builder.HasIndex(e => e.TerminationDate, "IX_TERMINATION_DATE");
        _ = builder.HasIndex(e => e.HireDate, "IX_HIRE_DATE"); // Performance: year-end service date calculations
        // Supports report filters on employment status and termination window
        _ = builder.HasIndex(e => e.EmploymentStatusId, "IX_EMPLOYMENT_STATUS");
        _ = builder.HasIndex(e => new { e.EmploymentStatusId, e.TerminationDate }, "IX_STATUS_TERMINATION_DATE");
        // Performance: Composite index for year-end report filtering (employment status + hire/termination date ranges)
        _ = builder.HasIndex(e => new { e.EmploymentStatusId, e.HireDate, e.TerminationDate }, "IX_EMPLOYMENT_STATUS_HIRE_TERMINATION");
        _ = builder.HasIndex(e => new { e.Ssn, e.OracleHcmId }, "IX_SSN_ORACLE_HCM_ID");
        _ = builder.HasIndex(e => new { e.Ssn, e.BadgeNumber }, "IX_SSN_BADGE_NUMBER");

        _ = builder.Property(e => e.Id)
            .HasPrecision(9)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        _ = builder.Property(e => e.Ssn)
            .HasPrecision(9)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("SSN");

        _ = builder.HasIndex(e => e.BadgeNumber, "IX_BadgeNumber").IsUnique();
        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("BADGE_NUMBER");

        _ = builder.HasIndex(e => e.OracleHcmId, "IX_ORACLE_HCM_ID").IsUnique();
        _ = builder.Property(e => e.OracleHcmId)
            .HasPrecision(15)
            .ValueGeneratedNever()
            .HasColumnName("ORACLE_HCM_ID");

        _ = builder.Property(e => e.StoreNumber)
            .HasPrecision(4)
            .HasColumnName("STORE_NUMBER")
            .HasComment("StoreNumber");

        _ = builder.Property(e => e.DepartmentId)
            .HasPrecision(1)
            .HasColumnName("DEPARTMENT")
            .HasComment("Department");

        builder.Property(e => e.PayClassificationId)
            .HasComment("PayClassification")
            .HasColumnName("PAY_CLASSIFICATION_ID")
            .HasMaxLength(4);

        _ = builder.Property(e => e.DateOfBirth)
            .HasComment("DateOfBirth")
            .HasColumnName("DATE_OF_BIRTH")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.DateOfDeath)
            .HasComment("DateOfDeath")
            .HasColumnName("DATE_OF_DEATH")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.HireDate)
            .HasComment("HireDate")
            .HasColumnType("DATE")
            .HasColumnName("HIRE_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.ReHireDate)
            .HasComment("ReHireDate")
            .HasColumnName("REHIRE_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.TerminationCodeId)
            .HasMaxLength(1)
            .HasColumnName("TERMINATION_CODE_ID")
            .HasComment("TerminationCode");


        _ = builder.Property(e => e.TerminationDate)
            .HasComment("TerminationDate")
            .HasColumnName("TERMINATION_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.FullTimeDate)
            .HasComment("FullTimeDate")
            .HasColumnName("FULL_TIME_DATE")
            .HasColumnType("DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.EmploymentTypeId)
            .HasMaxLength(1)
            .HasColumnName("EMPLOYMENT_TYPE_ID")
            .HasComment("EmploymentType");

        _ = builder.Property(e => e.PayFrequencyId)
            .HasMaxLength(1)
            .HasColumnName("PAY_FREQUENCY_ID")
            .HasComment("PayFrequency");

        _ = builder.Property(e => e.GenderId)
            .HasMaxLength(1)
            .HasColumnName("GENDER_ID")
            .HasComment("Gender");

        _ = builder.Property(e => e.EmploymentStatusId)
            .HasColumnName("EMPLOYMENT_STATUS_ID");

        _ = builder.Property(e => e.VestingScheduleId)
            .HasColumnName("VESTING_SCHEDULE_ID");

        _ = builder.Property(e => e.HasForfeited)
            .HasColumnName("HAS_FORFEITED");

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
            contact.HasIndex(e => e.FullName, "IX_FULL_NAME");
            _ = contact.Property(e => e.FullName)
                .HasColumnType("NVARCHAR2(128)")
                .HasComputedColumnSql(
                    "LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END",
                    stored: true)
                .HasMaxLength(128)
                .HasComment("Automatically computed from LastName, FirstName, and MiddleName with middle initial")
                .HasColumnName("FULL_NAME");

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

        builder.HasOne(d => d.EmploymentStatus)
            .WithMany(p => p.Demographics)
            .HasForeignKey(d => d.EmploymentStatusId);

        builder.HasOne(d => d.VestingSchedule)
            .WithMany()
            .HasForeignKey(d => d.VestingScheduleId);

        _ = builder.HasMany(d => d.Beneficiaries)
            .WithOne(p => p.Demographic)
            .HasForeignKey(p => p.DemographicId);

        _ = builder.HasMany(d => d.PayProfits)
            .WithOne(p => p.Demographic)
            .HasForeignKey(d => d.DemographicId);

        _ = builder.HasMany(d => d.Checks)
            .WithOne()
            .HasForeignKey(p => p.DemographicId);

        builder.HasMany(d => d.DistributionRequests).WithOne()
            .HasForeignKey(p => p.DemographicId);

        builder.HasMany(d => d.DemographicSsnChangeHistories)
            .WithOne(d => d.Demographic)
            .HasForeignKey(p => p.DemographicId);
    }
}
