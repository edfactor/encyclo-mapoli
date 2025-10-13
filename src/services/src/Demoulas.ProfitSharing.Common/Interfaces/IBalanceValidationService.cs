using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for validating balance integrity rules per the Balance Reports Cross-Reference Matrix.
/// Focuses on Rule 2 and related balance equation validations.
/// </summary>
public interface IBalanceValidationService
{
    /// <summary>
    /// Validates that ALLOC (Incoming QDRO Beneficiary) and PAID ALLOC (Outgoing XFER Beneficiary)
    /// transactions sum to zero for a given profit year (Balance Matrix Rule 2).
    /// </summary>
    /// <param name="profitYear">The profit year to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing validation group with:
    /// - IncomingAllocations: Total ALLOC (code 6) from Contribution field
    /// - OutgoingAllocations: Total PAID ALLOC (code 5) from Forfeiture field
    /// - NetAllocTransfer: Net transfer amount (should be 0)
    /// - IsValid: true if net transfer equals zero
    /// </returns>
    /// <remarks>
    /// Balance Matrix Rule 2: The sum of ALLOC and PAID ALLOC must equal zero.
    /// ALLOC represents money transferred IN (ProfitCodeId=6, Contribution field).
    /// PAID ALLOC represents money transferred OUT (ProfitCodeId=5, Forfeiture field).
    /// An imbalance indicates a data integrity issue requiring investigation.
    /// </remarks>
    Task<Result<CrossReferenceValidationGroup>> ValidateAllocTransfersAsync(
        short profitYear,
        CancellationToken cancellationToken = default);
}
