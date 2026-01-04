using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// Configuration for annuity rate age ranges per year.
/// Defines the minimum and maximum ages for which annuity rates must exist.
/// </summary>
public sealed class AnnuityRateConfig : ModifiedBase
{
    /// <summary>
    /// Gets or sets the profit year for this configuration.
    /// </summary>
    public short Year { get; set; }

    /// <summary>
    /// Gets or sets the minimum age for which annuity rates must exist.
    /// Typically 67 (minimum retirement age).
    /// </summary>
    public byte MinimumAge { get; set; }

    /// <summary>
    /// Gets or sets the maximum age for which annuity rates must exist.
    /// Typically 120 (actuarial maximum age).
    /// </summary>
    public byte MaximumAge { get; set; }
}
