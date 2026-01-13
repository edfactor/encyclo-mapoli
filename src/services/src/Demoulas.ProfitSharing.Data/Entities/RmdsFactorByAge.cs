namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Required Minimum Distribution (RMD) life expectancy divisors by age.
/// Used for calculating RMD distributions from profit sharing accounts based on IRS Publication 590-B.
/// Formula: RMD Amount = Account Balance ÷ Factor
/// </summary>
public sealed class RmdsFactorByAge
{
    /// <summary>
    /// Age in years (73-99)
    /// </summary>
    public byte Age { get; set; }

    /// <summary>
    /// IRS life expectancy divisor for this age (e.g., 26.5 years for age 73).
    /// This represents the expected remaining years of life, not a percentage.
    /// </summary>
    public decimal Factor { get; set; }
}
