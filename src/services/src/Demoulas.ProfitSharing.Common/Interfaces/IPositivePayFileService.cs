using Demoulas.ProfitSharing.Common.Contracts;

namespace Demoulas.ProfitSharing.Common.Interfaces;

/// <summary>
/// Service for generating Positive Pay CSV files for Payroll reconciliation.
/// Per Anna Visi comment on PS-1790: "After 2025 changes no longer send this file to bank, 
/// instead sent to Payroll, and Finance sends direct to bank."
/// This file is sent to Christina/Payroll team for Finance reconciliation.
/// Replaces QPROF025 reconciliation step commented out in PROFDIST-FTP.ksh script.
/// </summary>
public interface IPositivePayFileService
{
    /// <summary>
    /// Generates a Positive Pay CSV file containing check data for the specified profit year.
    /// CSV includes columns: CheckNumber, Amount, IssueDate, AccountNumber, BadgeNumber.
    /// </summary>
    /// <param name="profitYear">The profit year to generate the report for (e.g., 2024).</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a Stream with the CSV content on success,
    /// or an Error on failure.
    /// </returns>
    Task<Result<Stream>> GeneratePositivePayCsvAsync(int profitYear, CancellationToken cancellationToken);
}
