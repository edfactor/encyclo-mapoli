using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.PositivePay;

/// <summary>
/// Service for generating Positive Pay CSV files for Payroll reconciliation.
/// Per Anna Visi comment on PS-1790: "After 2025 changes no longer send this file to bank, 
/// instead sent to Payroll, and Finance sends direct to bank."
/// 
/// This file is sent to Christina/Payroll team for Finance reconciliation.
/// Replaces QPROF025 reconciliation step commented out in PROFDIST-FTP.ksh script.
/// </summary>
public sealed class PositivePayFileService : IPositivePayFileService
{
    private const string NewtekAccountNumber = "0375495656";
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly ILogger<PositivePayFileService> _logger;

    public PositivePayFileService(
        IProfitSharingDataContextFactory factory,
        ILogger<PositivePayFileService> logger)
    {
        _factory = factory;
        _logger = logger;
    }

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
    public async Task<Result<Stream>> GeneratePositivePayCsvAsync(int profitYear, CancellationToken cancellationToken)
    {
        try
        {
#pragma warning disable CA2016 // Forward CancellationToken - UseReadOnlyContext doesn't accept cancellation token
            return await _factory.UseReadOnlyContext(async context =>
#pragma warning restore CA2016
            {
                // Query ProfitShareCheck with filters
                var checks = await context.ProfitShareChecks
                    .Where(c => c.CheckRunDate != null
                        && c.CheckRunDate.Value.Year == profitYear
                        && (c.IsVoided == null || c.IsVoided == false))
                    .TagWith($"PositivePayCsv-{profitYear}")
                    .Select(c => new PositivePayRecord
                    {
                        CheckNumber = c.CheckNumber,
                        Amount = c.CheckAmount,
                        IssueDate = c.CheckDate ?? DateOnly.MinValue,
                        AccountNumber = NewtekAccountNumber,
                        BadgeNumber = c.Ssn // Note: Using SSN field which stores badge number context per entity design
                    })
                    .OrderBy(c => c.CheckNumber)
                    .ToListAsync(cancellationToken);

                if (checks.Count == 0)
                {
                    _logger.LogWarning(
                        "No checks found for profit year {ProfitYear} to generate Positive Pay CSV",
                        profitYear);
                    return Result<Stream>.Failure(Error.CalendarYearNotFound);
                }

                // Generate CSV using CsvHelper
                var memoryStream = new MemoryStream();
                await using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
                await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                });

                // Write records (CsvHelper auto-maps properties)
                await csv.WriteRecordsAsync(checks, cancellationToken);
                await writer.FlushAsync(cancellationToken);

                // Reset stream position for reading
                memoryStream.Position = 0;

                _logger.LogInformation(
                    "Generated Positive Pay CSV for profit year {ProfitYear} with {CheckCount} checks, file size {FileSize} bytes",
                    profitYear, checks.Count, memoryStream.Length);

                return Result<Stream>.Success(memoryStream);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error generating Positive Pay CSV for profit year {ProfitYear}: {ErrorMessage}",
                profitYear, ex.Message);
            return Result<Stream>.Failure(Error.Unexpected(ex.Message));
        }
    }
}
