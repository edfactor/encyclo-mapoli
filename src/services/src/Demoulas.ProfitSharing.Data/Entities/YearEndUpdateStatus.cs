using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// This entity represents the year-end update status for the plan.
/// When the year-end process is run, this entity is saved to reflect the status of the process.
/// This is helpful for showing the Admin what updates have been applied to the plan.
/// </summary>
public class YearEndUpdateStatus : ModifiedBase
{
    public int Id { get; set; }
    public required short ProfitYear { get; set; }
    
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
}
