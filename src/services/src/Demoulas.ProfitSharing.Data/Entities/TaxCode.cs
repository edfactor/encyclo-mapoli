using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class TaxCode : ILookupTable<char>
{
    public static class Constants
    {
        public static readonly TaxCode Unknown = new TaxCode
        {
            Id = '0',
            Name = "Unknown - not legal tax code, yet 24 records in the obfuscated set have this value.",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false,
            IsProtected = true
        };

        public static readonly TaxCode EarlyDistributionNoException = new TaxCode
        {
            Id = '1',
            Name = "Early (Premature) dist no known exception",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true
        };

        public static readonly TaxCode EarlyDistributionExceptionApplies = new TaxCode
        {
            Id = '2',
            Name = "Early (Premature) dist exception applies",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true
        };

        public static readonly TaxCode Disability = new TaxCode
        {
            Id = '3',
            Name = "Disability",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode Death = new TaxCode
        {
            Id = '4',
            Name = "Death",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true
        };

        public static readonly TaxCode ProhibitedTransaction = new TaxCode
        {
            Id = '5',
            Name = "Prohibited transaction",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode Section1035Exchange = new TaxCode
        {
            Id = '6',
            Name = "Section 1035 exchange",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode NormalDistribution = new TaxCode
        {
            Id = '7',
            Name = "Normal distribution",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true,
            IsProtected = true
        };

        public static readonly TaxCode ExcessContributionsEarningsDeferrals8 = new TaxCode
        {
            Id = '8',
            Name = "Excess contributions + earnings/deferrals",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode PS58Cost = new TaxCode
        {
            Id = '9',
            Name = "PS 58 cost",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode QualifiesFor5Or10YearAveraging = new TaxCode
        {
            Id = 'A',
            Name = "Qualifies for 5- or 10-year averaging",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode QualifiesForDeathBenefitExclusion = new TaxCode
        {
            Id = 'B',
            Name = "Qualifies for death benefit exclusion",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode QualifiesForBothAandB = new TaxCode
        {
            Id = 'C',
            Name = "Qualifies for both A and B",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode ExcessContributionsEarningsDeferralsD = new TaxCode
        {
            Id = 'D',
            Name = "Excess contributions + earnings deferrals",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode ExcessAnnualAdditionsSection415 = new TaxCode
        {
            Id = 'E',
            Name = "Excess annual additions under section 415",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode CharitableGiftAnnuity = new TaxCode
        {
            Id = 'F',
            Name = "Charitable gift annuity",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode DirectRolloverToIRA = new TaxCode
        {
            Id = 'G',
            Name = "Direct rollover to IRA",
            IsAvailableForDistribution = true,
            IsAvailableForForfeiture = true
        };

        public static readonly TaxCode DirectRolloverToPlanOrAnnuity = new TaxCode
        {
            Id = 'H',
            Name = "Direct rollover to plan/tax sheltered annuity",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };

        public static readonly TaxCode ExcessContributionsEarningsDeferralsP = new TaxCode
        {
            Id = 'P',
            Name = "Excess contributions + earnings/deferrals ",
            IsAvailableForDistribution = false,
            IsAvailableForForfeiture = false
        };
    }

    public char Id { get; set; }
    public required string Name { get; set; }

    public bool IsAvailableForDistribution { get; set; }

    public bool IsAvailableForForfeiture { get; set; }

    /// <summary>
    /// Indicates whether this tax code is used in business logic and must not be changed.
    /// </summary>
    public bool IsProtected { get; set; }
}
