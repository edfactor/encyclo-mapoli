using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for unmasking sensitive demographic data (SSN) for compliance and verification purposes.
/// Restricted to users with SSN-Unmasking role.
/// </summary>
public interface IUnmaskingService
{
    /// <summary>
    /// Gets the unmasked, formatted SSN for a demographic record.
    /// Returns formatted SSN (e.g., "123-45-6789") or failure if not found.
    /// </summary>
    /// <param name="demographicId">The demographic ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Formatted SSN or error result</returns>
    Task<Result<string>> GetUnmaskedSsnAsync(int demographicId, CancellationToken cancellationToken = default);
}
