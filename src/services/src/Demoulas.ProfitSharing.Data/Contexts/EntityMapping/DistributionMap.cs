using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class DistributionMap : IEntityTypeConfiguration<Distribution>
{
    public void Configure(EntityTypeBuilder<Distribution> builder)
    {
        builder.ToTable("DISTRIBUTION");
        builder.HasKey(d => new { d.SSN, d.SequenceNumber });
        builder.Property(d => d.SSN).HasPrecision(9);
        builder.Property(d => d.SequenceNumber).HasColumnName("SEQUENCE_NUMBER");
        builder.Property(d => d.EmployeeName).HasMaxLength(25).HasColumnName("EMPLOYEE_NAME");
        builder.Property(d => d.FrequencyId).HasColumnName("FREQUENCY_ID");
        builder.Property(d => d.StatusId).HasColumnName("STATUS_ID");
        builder.Property(d => d.PayeeSSN).HasPrecision(9).HasColumnName("PAYEE_SSN");
        builder.Property(d => d.PayeeName).HasMaxLength(30).HasColumnName("PAYEE_NAME");
        builder.Property(d => d.PayeeSSN).HasColumnName("PAYEE_SSN");
        builder.OwnsOne(d => d.PayeeAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("STREET");
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("STREET2");
            address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("STREET3");
            address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("STREET4");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("CITY");
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("STATE");
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("POSTAL_CODE");
            address.Property(a => a.CountryISO).HasMaxLength(2).HasColumnName("COUNTRY_ISO")
                .HasDefaultValue(Country.Constants.US);
            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryISO);
        });
        builder.Property(d => d.ThirdPartyPayee).HasMaxLength(30).HasColumnName("THIRD_PARTY_PAYEEE");
        builder.Property(d => d.ThirdPartyName).HasMaxLength(30).HasColumnName("THIRD_PARTY_NAME");
        builder.Property(d => d.ThirdPartyAccount).HasMaxLength(30).HasColumnName("THIRD_PARTY_ACCOUNT");
        builder.OwnsOne(d => d.ThirdPartyAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(30).HasColumnName("THIRD_PARTY_STREET");
            address.Property(a => a.Street2).HasMaxLength(30).HasColumnName("THIRD_PARTY_STREET2");
            address.Property(a => a.Street3).HasMaxLength(30).HasColumnName("THIRD_PARTY_STREET3");
            address.Property(a => a.Street4).HasMaxLength(30).HasColumnName("THIRD_PARTY_STREET4");
            address.Property(a => a.City).HasMaxLength(25).HasColumnName("THIRD_PARTY_CITY");
            address.Property(a => a.State).HasMaxLength(3).HasColumnName("THIRD_PARTY_STATE");
            address.Property(a => a.PostalCode).HasMaxLength(9).HasColumnName("THIRD_PARTY_POSTAL_CODE");
            address.Property(a => a.CountryISO).HasMaxLength(2).HasColumnName("THIRD_PARTY_COUNTRY_ISO")
                .HasDefaultValue(Country.Constants.US);
            address.HasOne<Country>()
                .WithMany()
                .HasForeignKey(o => o.CountryISO);
        });
        builder.Property(d => d.ForTheBenefitOfPayee).HasMaxLength(30).HasColumnName("FORTHEBENEFITOF_PAYEE");
        builder.Property(d => d.ForTheBenefitOfAccountType).HasMaxLength(30).HasColumnName("FORTHEBENEFITOF_ACCOUNT_TYPE");

        builder.Property(d => d.Tax1099ForEmployee).HasColumnType("NUMBER(1)").HasColumnName("TAX1099_FOR_EMPLOYEE");
        builder.Property(d => d.Tax1099ForBeneficiary).HasColumnType("NUMBER(1)").HasColumnName("TAX1099_FOR_BENEFICIARY");
        builder.Property(d => d.FederalTaxPercentage).HasPrecision(9, 2).HasColumnName("FEDERAL_TAX_PERCENTAGE");
        builder.Property(d => d.StateTaxPercentage).HasPrecision(9, 2).HasColumnName("STATE_TAX_PERCENTAGE");
        builder.Property(d => d.GrossAmount).HasPrecision(9, 2).HasColumnName("GROSS_AMOUNT");
        builder.Property(d => d.FederalTaxAmount).HasPrecision(9, 2).HasColumnName("FEDERAL_TAX_AMOUNT");
        builder.Property(d => d.StateTaxAmount).HasPrecision(9, 2).HasColumnName("STATE_TAX_AMOUNT");
        builder.Property(d => d.CheckAmount).HasPrecision(9, 2).HasColumnName("CHECK_AMOUNT");
        builder.Property(d => d.TaxCodeId).HasColumnName("TAX_CODE_ID");
        builder.Property(d => d.Deceased).HasColumnType("NUMBER(1)");
        builder.Property(d => d.GenderId).HasColumnName("GENDER_ID");
        builder.Property(d => d.QualifiedDomesticRelationsOrder).HasColumnType("NUMBER(1)").HasColumnName("QDRO");
        builder.Property(d => d.RothIRA).HasColumnType("NUMBER(1)").HasColumnName("ROTH_IRA");

        builder.HasOne(d => d.Gender).WithMany().HasForeignKey(d => d.GenderId);
        builder.HasOne(d => d.Frequency).WithMany().HasForeignKey(d => d.FrequencyId);
        builder.HasOne(d => d.Status).WithMany().HasForeignKey(d => d.StatusId);
        builder.HasOne(x => x.TaxCode).WithMany().HasForeignKey(t => t.TaxCodeId);
    }
}
