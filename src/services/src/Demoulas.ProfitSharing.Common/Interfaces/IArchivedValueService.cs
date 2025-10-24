using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for retrieving archived report values from stored checksums.
/// Provides access to historical report field values for audit and comparison purposes.
/// </summary>
/// <remarks>
/// PS-1721: Extracted from ChecksumValidationService to improve separation of concerns.
/// This service specializes in retrieving archived values without performing validation.
/// </remarks>
public interface IArchivedValueService
{
    /// <summary>
    /// Retrieves archived checksum values for Master Update validation without performing comparison.
    /// Returns the expected values from archived reports (PAY443, QPAY129, QPAY066TA, etc.) so the
    /// UI can perform its own comparison against current values.
    /// </summary>
    /// <param name="profitYear">The profit year to retrieve archived values for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing dictionary of archived values keyed by "ReportCode.FieldName", 
    /// or null if no archived values found for the specified year.
    /// </returns>
    Task<Result<Dictionary<string, decimal>?>> GetMasterUpdateArchivedValuesAsync(
        short profitYear,
        CancellationToken cancellationToken = default);
}
