using Demoulas.ProfitSharing.Data.Contexts.EntityMapping.Base;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionMap : ModifiedBaseMap<Distribution>
{
    public override void Configure(EntityTypeBuilder<Distribution> builder)
    {
        base.Configure(builder);

        _ = builder.ToTable("DISTRIBUTION");
        _ = builder.HasKey(d => d.Id);

        _ = builder.HasIndex(d => new { d.Ssn, d.PaymentSequence }, "IX_SSN_PAYMENT_SEQUENCE").IsUnique();

        _ = builder.Property(d => d.Id).HasColumnName("ID").ValueGeneratedOnAdd();
        _ = builder.Property(d => d.Ssn).HasColumnName("SSN").HasPrecision(9);
        _ = builder.Property(d => d.PaymentSequence).HasColumnName("PAYMENT_SEQUENCE");
        _ = builder.Property(d => d.EmployeeName).HasMaxLength(84).HasColumnName("EMPLOYEE_NAME");
        _ = builder.Property(d => d.FrequencyId).HasColumnName("FREQUENCY_ID");
        _ = builder.Property(d => d.StatusId).HasColumnName("STATUS_ID");
        _ = builder.Property(d => d.PayeeId).HasColumnName("PAYEE_ID");
        _ = builder.Property(d => d.ThirdPartyPayeeId).HasColumnName("THIRD_PARTY_PAYEE_ID");

        _ = builder.Property(d => d.ForTheBenefitOfPayee).HasMaxLength(30).HasColumnName("FORTHEBENEFITOF_PAYEE");
        _ = builder.Property(d => d.ForTheBenefitOfAccountType).HasMaxLength(30).HasColumnName("FORTHEBENEFITOF_ACCOUNT_TYPE");

        _ = builder.Property(d => d.Tax1099ForEmployee).HasColumnType("NUMBER(1)").HasColumnName("TAX1099_FOR_EMPLOYEE");
        _ = builder.Property(d => d.Tax1099ForBeneficiary).HasColumnType("NUMBER(1)").HasColumnName("TAX1099_FOR_BENEFICIARY");
        _ = builder.Ignore(d => d.FederalTaxPercentage);
        _ = builder.Ignore(d => d.StateTaxPercentage);
        _ = builder.Property(d => d.GrossAmount).HasPrecision(9, 2).HasColumnName("GROSS_AMOUNT");
        _ = builder.Property(d => d.FederalTaxAmount).HasPrecision(9, 2).HasColumnName("FEDERAL_TAX_AMOUNT");
        _ = builder.Property(d => d.StateTaxAmount).HasPrecision(9, 2).HasColumnName("STATE_TAX_AMOUNT");
        _ = builder.Ignore(d => d.CheckAmount);
        _ = builder.Property(d => d.TaxCodeId).HasColumnName("TAX_CODE_ID");
        _ = builder.Property(d => d.IsDeceased).HasColumnName("DECEASED").HasColumnType("NUMBER(1)");
        _ = builder.Property(d => d.GenderId).HasColumnName("GENDER_ID");
        _ = builder.Property(d => d.QualifiedDomesticRelationsOrder)
            .HasColumnType("NUMBER(1)")
            .HasColumnName("QDRO")
            .HasComment("Qualified Domestic Relations Order");
        _ = builder.Property(d => d.RothIra).HasColumnType("NUMBER(1)").HasColumnName("ROTH_IRA");
        _ = builder.Property(d => d.Memo).HasColumnName("MEMO").HasMaxLength(128);
        _ = builder.Property(d => d.ThirdPartyPayeeAccount).HasMaxLength(30).HasColumnName("THIRD_PARTY_PAYEE_ACCOUNT");


        _ = builder.HasOne(d => d.Gender).WithMany().HasForeignKey(d => d.GenderId);
        _ = builder.HasOne(d => d.Frequency).WithMany().HasForeignKey(d => d.FrequencyId);
        _ = builder.HasOne(d => d.Status).WithMany().HasForeignKey(d => d.StatusId);
        _ = builder.HasOne(x => x.TaxCode).WithMany().HasForeignKey(t => t.TaxCodeId);

        _ = builder.HasOne(x => x.Payee).WithMany(p => p.Distributions).HasForeignKey(t => t.PayeeId);
        _ = builder.HasOne(x => x.ThirdPartyPayee).WithMany(p => p.Distributions).HasForeignKey(t => t.ThirdPartyPayeeId);
    }
}
