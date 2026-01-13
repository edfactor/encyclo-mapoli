using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record PayProfitResponseDto : IProfitYearRequest
{
    /// <summary>
    /// Gets or sets the Oracle HCM (Human Capital Management) identifier.
    /// This identifier is used to uniquely associate the PayProfit entity with a Demographic/Oracle HCM record.
    /// </summary>
    public required int DemographicId { get; set; }

    /// <summary>
    /// Gets or sets the year for which the profit is being calculated.
    /// </summary>
    /// <value>
    /// The year represented as a short integer.
    /// </value>
    public short ProfitYear { get; set; }

    /// <summary>
    /// Hours towards Profit Sharing in the current year (updated weekly)
    /// </summary>
    public decimal CurrentHoursYear { get; set; }


    /// <summary>
    /// Income (Wage) accumulated so far in the current year (updated weekly)
    /// </summary>
    public decimal CurrentIncomeYear { get; set; }

    /// <summary>
    /// Number of weeks worked in the current year
    /// </summary>
    public byte WeeksWorkedYear { get; set; }

    public DateTimeOffset LastUpdate { get; set; }

    /// <summary>
    /// Points Earned (for the ProfitYear).
    /// </summary>
    public decimal? PointsEarned { get; set; }

    /// <summary>
    /// Total number of years a member was in the plan.
    /// </summary>
    public byte YearsInPlan { get; set; }

    public static PayProfitResponseDto ResponseExample() => new()
    {
        DemographicId = 1,
        ProfitYear = 2024,
        CurrentHoursYear = 1000.00m,
        CurrentIncomeYear = 75000.00m,
        WeeksWorkedYear = 52,
        LastUpdate = DateTimeOffset.UtcNow,
        PointsEarned = 1000.00m,
        YearsInPlan = 10
    };
}
