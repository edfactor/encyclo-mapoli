using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class TaxCodeMap : IEntityTypeConfiguration<TaxCode>
{
    public void Configure(EntityTypeBuilder<TaxCode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("TAX_CODE");

        builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasColumnName("ID");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(128).HasColumnName("NAME");
        builder.HasData(GetPredefinedTaxCodes());
    }

    private static List<TaxCode> GetPredefinedTaxCodes()
    {
        return
        [
            TaxCode.Constants.Unknown,
            TaxCode.Constants.EarlyDistributionNoException,
            TaxCode.Constants.EarlyDistributionExceptionApplies,
            TaxCode.Constants.Disability,
            TaxCode.Constants.Death,
            TaxCode.Constants.ProhibitedTransaction,
            TaxCode.Constants.Section1035Exchange,
            TaxCode.Constants.NormalDistribution,
            TaxCode.Constants.ExcessContributionsEarningsDeferrals8,
            TaxCode.Constants.PS58Cost,
            TaxCode.Constants.QualifiesFor5Or10YearAveraging,
            TaxCode.Constants.QualifiesForDeathBenefitExclusion,
            TaxCode.Constants.QualifiesForBothAandB,
            TaxCode.Constants.ExcessContributionsEarningsDeferralsD,
            TaxCode.Constants.ExcessAnnualAdditionsSection415,
            TaxCode.Constants.CharitableGiftAnnuity,
            TaxCode.Constants.DirectRolloverToIRA,
            TaxCode.Constants.DirectRolloverToPlanOrAnnuity,
            TaxCode.Constants.ExcessContributionsEarningsDeferralsP
        ];
    }
}
