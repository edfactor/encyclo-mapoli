using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicHistoryMap : IEntityTypeConfiguration<DemographicHistory>
{
    public void Configure(EntityTypeBuilder<DemographicHistory> builder)
    {
        _ = builder.ToTable("DEMOGRAPHIC_HISTORY");
        _ = builder.HasKey(x => x.Id);

        _ = builder.HasIndex(x => x.DemographicId, "IX_DEMOGRAPHIC");
        _ = builder.HasIndex(x => new { x.ValidFrom, x.ValidTo, x.DemographicId }, "IX_VALID_FROM_TO_DEMOGRAPHIC_ID");

        _ = builder.Property(x => x.Id)
            .HasPrecision(18)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        _ = builder.Property(x => x.DemographicId)
            .HasPrecision(9)
            .HasColumnName("DEMOGRAPHIC_ID")
            .IsRequired();

        _ = builder.Property(x => x.ValidFrom)
            .HasColumnName("VALID_FROM")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .IsRequired();

        _ = builder.Property(x => x.ValidTo)
            .HasColumnName("VALID_TO")
            .HasColumnType("TIMESTAMP WITH TIME ZONE")
            .IsRequired();

        _ = builder.HasIndex(e => e.BadgeNumber, "IX_BadgeNumber");
        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .HasColumnName("BADGE_NUMBER");

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
            .HasPrecision(2);

        _ = builder.Property(e => e.DateOfBirth)
            .HasComment("DateOfBirth")
            .HasColumnName("DATE_OF_BIRTH")
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

        _ = builder.Property(e => e.EmploymentTypeId)
            .HasMaxLength(1)
            .HasColumnName("EMPLOYMENT_TYPE_ID")
            .HasComment("EmploymentType");

        _ = builder.Property(e => e.PayFrequencyId)
            .HasMaxLength(1)
            .HasColumnName("PAY_FREQUENCY_ID")
            .HasComment("PayFrequency");

        _ = builder.Property(e => e.EmploymentStatusId)
            .HasColumnName("EMPLOYMENT_STATUS_ID");

        _ = builder.Property(e => e.CreatedDateTime)
            .IsRequired()
            .HasColumnName("CREATED_DATETIME");
    }
}
