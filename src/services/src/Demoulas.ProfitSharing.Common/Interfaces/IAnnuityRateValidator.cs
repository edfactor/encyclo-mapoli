using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Validates annuity rate data completeness for profit sharing year-end processing.
/// </summary>
public interface IAnnuityRateValidator
{
    /// <summary>
    /// Validates that all required annuity rates exist for the specified year.
    /// Checks for completeness (all ages from MinimumAge to MaximumAge) and detects gaps.
    /// </summary>
    /// <param name="year">The profit year to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Result indicating success if all required ages have rates,
    /// or failure with validation error listing missing age ranges.
    /// </returns>
    Task<Result<bool>> ValidateYearCompletenessAsync(short year, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the minimum age required for annuity rates for the specified year.
    /// </summary>
    /// <param name="year">The profit year.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The minimum age from configuration, or null if no configuration exists.</returns>
    Task<byte?> GetMinimumAgeForYearAsync(short year, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the maximum age required for annuity rates for the specified year.
    /// </summary>
    /// <param name="year">The profit year.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The maximum age from configuration, or null if no configuration exists.</returns>
    Task<byte?> GetMaximumAgeForYearAsync(short year, CancellationToken cancellationToken = default);
}
