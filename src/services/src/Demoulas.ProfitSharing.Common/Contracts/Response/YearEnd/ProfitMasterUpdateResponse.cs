using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

/// <summary>
/// This response describes an update applied to a profit year.   It says who did it and when.   It says how many members were effected, and what parameters were used.
/// This gives the administrator context about the state of the YE contributions.  In particular lets us know if the admin can revert and if so what they are
/// reverting.
/// </summary>
[YearEndArchiveProperty]
[NoMemberDataExposed]
public sealed record ProfitMasterUpdateResponse
{
    public required DateTimeOffset UpdatedTime { get; set; }
    public required string UpdatedBy { get; set; }

    public required int BeneficiariesEffected { get; set; }
    public required int EmployeesEffected { get; set; }
    public required int EtvasEffected { get; set; }

    public required decimal ContributionPercent { get; set; }
    public required decimal IncomingForfeitPercent { get; set; }
    public required decimal EarningsPercent { get; set; }
    public required decimal SecondaryEarningsPercent { get; set; }
    public required long MaxAllowedContributions { get; set; }
    public required long BadgeAdjusted { get; set; }
    public required long BadgeAdjusted2 { get; set; }
    public required decimal AdjustContributionAmount { get; set; }
    public required decimal AdjustEarningsAmount { get; set; }
    public required decimal AdjustIncomingForfeitAmount { get; set; }
    public required decimal AdjustEarningsSecondaryAmount { get; set; }
    public int TransactionsCreated { get; set; }


    public static ProfitMasterUpdateResponse Example()
    {
        return new ProfitMasterUpdateResponse
        {
            UpdatedTime = DateTimeOffset.UtcNow,
            UpdatedBy = "John Doe",
            BeneficiariesEffected = 4,
            EmployeesEffected = 721,
            EtvasEffected = 400,
            ContributionPercent = 15,
            IncomingForfeitPercent = 4,
            EarningsPercent = 2,
            SecondaryEarningsPercent = 0,
            MaxAllowedContributions = 30_000,
            BadgeAdjusted = 7773838,
            BadgeAdjusted2 = 0,
            AdjustContributionAmount = 11,
            AdjustEarningsAmount = 12,
            AdjustIncomingForfeitAmount = 15,
            AdjustEarningsSecondaryAmount = 0
        };
    }
}
