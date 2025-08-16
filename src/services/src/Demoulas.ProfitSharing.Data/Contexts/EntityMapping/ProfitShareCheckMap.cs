using Demoulas.Common.Data.Contexts.ValueConverters;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;
internal sealed class ProfitShareCheckMap : IEntityTypeConfiguration<ProfitShareCheck>
{
    public void Configure(EntityTypeBuilder<ProfitShareCheck> builder)
    {
        _ = builder.ToTable("PROFIT_SHARE_CHECK");

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .HasPrecision(15)
            .ValueGeneratedOnAdd()
            .HasColumnName("Id");

        _ = builder.HasIndex(e => new {e.CheckNumber, e.IsVoided}, "IX_CheckNumber_IsVoided");
        
        _ = builder.Property(e => e.CheckNumber)
            .HasPrecision(15)
            .ValueGeneratedNever()
            .HasColumnName("CHECK_NUMBER");

        _ = builder.HasIndex(e => e.Ssn, "IX_SSN");
        _ = builder.Property(e => e.Ssn)
            .HasPrecision(9)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnName("SSN");

        _ = builder.Property(e => e.DemographicId)
            .HasPrecision(9)
            .ValueGeneratedNever()
            .HasColumnName("DEMOGRAPHIC_ID");

        _ = builder.HasIndex(e => e.PscCheckId, "IX_PSC_CHECKID").IsUnique();
        _ = builder.Property(e => e.PscCheckId)
            .HasPrecision(15)
            .ValueGeneratedNever()
            .HasColumnName("PSC_CHECK_ID");

        _ = builder.Property(e => e.FloatDays)
            .HasPrecision(6)
            .ValueGeneratedNever()
            .HasColumnName("FLOAT_DAYS");

        _ = builder.Property(e => e.CheckDate)
            .HasColumnType("DATE")
            .HasColumnName("CHECK_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.VoidDate)
            .HasColumnType("DATE")
            .HasColumnName("VOID_CHECK_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.ClearDate)
            .HasColumnType("DATE")
            .HasColumnName("CLEAR_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.ClearDateLoaded)
            .HasColumnType("DATE")
            .HasColumnName("CLEAR_DATE_LOADED")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.VoidReconDate)
            .HasColumnType("DATE")
            .HasColumnName("VOID_RECON_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.CheckRunDate)
            .HasColumnType("DATE")
            .HasColumnName("CHECK_RUN_DATE")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.DateLoaded)
            .HasColumnType("DATE")
            .HasColumnName("DATE_LOADED")
            .HasConversion<DateOnlyConverter>();

        _ = builder.Property(e => e.PayableName)
            .HasMaxLength(84)
            .HasColumnName("PAYABLE_NAME")
            .IsRequired();

        _ = builder.Property(e => e.RefNumber)
            .HasMaxLength(36)
            .HasColumnName("REF_NUMBER");

        _ = builder.Property(e => e.ReplaceCheck)
            .HasMaxLength(24)
            .HasColumnName("REPLACE_CHECK");


        _ = builder.Property(e => e.CheckAmount)
            .HasPrecision(9, 2)
            .HasColumnName("CHECK_AMOUNT")
            .IsRequired();

        _ = builder.Property(e => e.IsVoided)
            .HasColumnName("VOID_FLAG");

        _ = builder.Property(e => e.OtherBeneficiary)
            .HasColumnName("OTHER_BENEFICIARY");


        _ = builder.Property(e => e.IsManualCheck)
            .HasColumnName("MANUAL_CHECK");

        _ = builder.Property(e => e.TaxCodeId)
            .HasColumnName("TAX_CODE_ID");


        _ = builder.HasOne(x => x.TaxCode)
            .WithMany()
            .HasForeignKey(t => t.TaxCodeId);
    }
}
