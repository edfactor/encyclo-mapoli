using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Represents a vesting schedule defining how participant balances vest over time.
/// </summary>
/// <remarks>
/// Vesting schedules determine the percentage of a participant's balance they are entitled to
/// based on years of service. Two primary schedules exist:
/// - Old Plan (pre-2007): 7-year vesting schedule
/// - New Plan (2007+): 6-year vesting schedule
/// 
/// See: https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31886463/Profit+Sharing+for+the+non-programmer
/// </remarks>
public sealed class VestingSchedule : ILookupTable<int>
{
    public static class Constants
    {
        /// <summary>
        /// Old vesting plan (pre-2007): 7-year schedule with vesting starting at year 3.
        /// Schedule: Year 0=0%, 1=0%, 2=0%, 3=20%, 4=40%, 5=60%, 6=80%, 7+=100%
        /// </summary>
        public const int OldPlan = 1;

        /// <summary>
        /// New vesting plan (2007+): 6-year schedule with vesting starting at year 2.
        /// Schedule: Year 0=0%, 1=0%, 2=20%, 3=40%, 4=60%, 5=80%, 6+=100%
        /// </summary>
        public const int NewPlan = 2;
    }

    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The effective date when this vesting schedule became active.
    /// Used to determine which vesting plan applies based on contribution dates.
    /// </summary>
    /// <remarks>
    /// Old Plan: 1917-01-01 (Market Basket founding date)
    /// New Plan: 2007-01-01 (new vesting rules effective date)
    /// </remarks>
    public DateOnly EffectiveDate { get; set; }

    /// <summary>
    /// Collection of vesting percentages for each year of service.
    /// </summary>
    public ICollection<VestingScheduleDetail> Details { get; set; } = new List<VestingScheduleDetail>();
}
