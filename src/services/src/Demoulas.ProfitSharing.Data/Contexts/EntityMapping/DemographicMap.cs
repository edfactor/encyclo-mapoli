using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.Common.Data.Contexts.ValueConverters;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DemographicMap : IEntityTypeConfiguration<Demographic>
{
    public void Configure(EntityTypeBuilder<Demographic> builder)
    {
        //https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31909725/Employee+Hiring+data+fields
        //https://demoulas.atlassian.net/wiki/spaces/~bherrmann/pages/39944312/Quick+Guide+to+Profit+Sharing+Tables

        _ = builder.HasKey(e => e.BadgeNumber);
        _ = builder.ToTable("DEMOGRAPHICS");

        _ = builder.Property(e => e.BadgeNumber)
            .HasPrecision(7)
            .ValueGeneratedNever()
            .HasColumnName("DEM_BADGE");

        _ = builder.Property(e => e.FullName)
            .HasMaxLength(60)
            .HasColumnName("PY_NAM")
            .IsRequired();

        _ = builder.Property(e => e.LastName)
            .HasMaxLength(30)
            .HasColumnName("PY_LNAME")
            .IsRequired();

        _ = builder.Property(e => e.FirstName)
            .HasMaxLength(30)
            .HasColumnName("PY_FNAME")
            .IsRequired();

        _ = builder.Property(e => e.MiddleName)
            .HasMaxLength(25)
            .HasColumnName("PY_MNAME");

        _ = builder.Property(e => e.StoreNumber)
            .HasPrecision(3)
            .HasColumnName("PY_STOR");

        _ = builder.Property(e => e.Department)
            .HasPrecision(1)
            .HasColumnName("PY_DP");

        _ = builder.Property(e => e.PayClass)
            .HasPrecision(3)
            .HasColumnName("PY_CLA");

        _ = builder.Property(e => e.DateOfBirth)
            .HasPrecision(8)
            .HasColumnName("PY_DOB")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.HireDate)
            .HasPrecision(8)
            .HasColumnName("PY_HIRE_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.ReHireDate)
            .HasPrecision(8)
            .HasColumnName("PY_REHIRE_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.TerminationDate)
            .HasPrecision(8)
            .HasColumnName("PY_TERM_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.FullTimeDate)
            .HasPrecision(8)
            .HasColumnName("PY_FULL_DT")
            .HasConversion<IntegerToDateOnlyConverterYyyyMMdd>();

        _ = builder.Property(e => e.EmploymentType)
            .HasMaxLength(2)
            .HasColumnName("PY_FUL")
            .HasConversion<EmploymentTypeConverter>();

        //builder.HasOne<Definition>()
        //    .WithMany()
        //    .HasForeignKey(o => o.EmploymentType);


        _ = builder.Property(e => e.PayFrequency)
            .HasMaxLength(1)
            .HasColumnName("PY_FREQ")
            .HasConversion<PayFrequencyConverter>();

        //"PY_TYPE" CHAR(1),
        //"PY_SCOD" CHAR(1),

        //"PY_TERM" CHAR(1),
        //"PY_ASSIGN_ID" NUMBER(15, 0),
        //"PY_ASSIGN_DESC" CHAR(15),
        //"PY_NEW_EMP" CHAR(1),
        //"PY_GENDER" CHAR(1),
        //"PY_EMP_TELNO" NUMBER(10, 0),
        //"PY_SHOUR" NUMBER(5, 2),
        //"PY_SET_PWD" CHAR(1),
        //"PY_SET_PWD_DT" DATE,
        //"PY_CLASS_DT" NUMBER(8, 0),
        //"PY_GUID" VARCHAR2(256),

        builder.OwnsOne(e => e.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("PY_ADD").IsRequired();
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("PY_ADD2");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("PY_CITY").IsRequired();
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("PY_STATE").IsRequired();
            address.Property(a => a.PostalCode).HasPrecision(9).HasColumnName("PY_ZIP").IsRequired().HasConversion<PostalCodeConverter>();
        });
    }
}
