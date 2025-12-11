using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for validating the complete PAY444 balance equation per the Balance Reports Cross-Reference Matrix.
/// Validates: Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures
/// </summary>
public interface IBalanceEquationValidationService
{
    /// <summary>
    /// Validates the complete balance equation for PAY444 (Master Update) for a given profit year.
    /// This validates Rule 5 from the Balance Reports Cross-Reference Matrix:
    /// Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Earnings - Forfeitures
    /// </summary>
    /// <param name="profitYear">The profit year to validate</param>
    /// <param name="currentValues">Dictionary of current field values keyed by "ReportCode.FieldName" (e.g., "PAY444.BeginningBalance")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Result containing validation group with:
    /// - Individual component validations (Beginning Balance, Contributions, etc.)
    /// - Calculated ending balance from equation
    /// - Expected ending balance (if available from report)
    /// - Variance and detailed breakdown
    /// - IsValid: true if calculated ending balance matches expected
    /// </returns>
    /// <remarks>
    /// Balance Matrix Rule 5: The ending balance must equal the sum of all components.
    /// This is the most comprehensive validation as it ensures all PAY444 fields reconcile correctly.
    /// Components:
    /// - Beginning Balance (from prior year PAY443)
    /// - Contributions (employer + employee, excluding ALLOC transfers)
    /// - ALLOC (internal transfers IN)
    /// - Distributions (withdrawals, excluding PAID ALLOC)
    /// - PAID ALLOC (internal transfers OUT)
    /// - Earnings (investment gains/losses)
    /// - Forfeitures (forfeited amounts redistributed)
    /// </remarks>
    Task<Result<CrossReferenceValidationGroup>> ValidateBalanceEquationAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken = default);
}
