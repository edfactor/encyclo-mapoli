using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Response.Validation;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Validation;

/// <summary>
/// Service for SHA256 checksum-based validation of archived report data.
/// Compares caller-provided current values against archived checksums to detect data drift.
/// </summary>
/// <remarks>
/// PS-1721: Refactored to focus solely on checksum validation.
/// This service now has a single, focused responsibility: validating report fields via SHA256 checksums.
///
/// Related Services:
/// - <see cref="ICrossReferenceValidationService"/>: Orchestrates cross-reference validation across multiple reports
/// - <see cref="IArchivedValueService"/>: Retrieves archived report values from stored checksums
///
/// Responsibilities:
/// 1. SHA256 checksum calculation and comparison
/// 2. Per-field validation against archived checksums
/// 3. Archived value retrieval for individual fields
/// </remarks>
public sealed class ChecksumValidationService : IChecksumValidationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger<ChecksumValidationService> _logger;

    public ChecksumValidationService(
        IProfitSharingDataContextFactory dataContextFactory,
        ILogger<ChecksumValidationService> logger)
    {
        _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Checksum Validation

    /// <inheritdoc />
    public async Task<Result<ChecksumValidationResponse>> ValidateReportFieldsAsync(
        short profitYear,
        string reportType,
        Dictionary<string, decimal> fieldsToValidate,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(reportType))
        {
            return Result<ChecksumValidationResponse>.Failure(
                Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(reportType)] = new[] { "Report type cannot be null or empty." }
                }));
        }

        if (fieldsToValidate == null || !fieldsToValidate.Any())
        {
            return Result<ChecksumValidationResponse>.Failure(
                Error.Validation(new Dictionary<string, string[]>
                {
                    [nameof(fieldsToValidate)] = new[] { "At least one field must be provided for validation." }
                }));
        }

        _logger.LogInformation(
            "Validating {FieldCount} fields for report {ReportType} year {ProfitYear}",
            fieldsToValidate.Count,
            reportType,
            profitYear);

        try
        {
            // 1. Query for most recent archived checksum
            var archived = await _dataContextFactory.UseReadOnlyContext(async ctx =>
                await ctx.ReportChecksums
                    .Where(r => r.ProfitYear == profitYear && r.ReportType == reportType)
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .FirstOrDefaultAsync(cancellationToken), cancellationToken);

            if (archived == null)
            {
                _logger.LogWarning(
                    "No archived report found for {ReportType} year {ProfitYear}",
                    reportType,
                    profitYear);

                return Result<ChecksumValidationResponse>.Failure(
                    Error.EntityNotFound($"No archived report found for {reportType} year {profitYear}"));
            }

            // 2. Build dictionaries of archived checksums AND actual values for quick lookup
            var archivedChecksums = archived.KeyFieldsChecksumJson.ToDictionary(
                kvp => kvp.Key,
                kvp => Convert.ToBase64String(kvp.Value.Value) // The hash bytes
            );

            // 3. Validate each provided field
            var fieldResults = new Dictionary<string, FieldValidationResult>();
            var mismatchedFields = new List<string>();

            foreach (var (fieldName, fieldValue) in fieldsToValidate)
            {
                // Calculate checksum for the provided value
                var providedHash = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(fieldValue));
                var providedChecksum = Convert.ToBase64String(providedHash);

                // Check if this field exists in archived checksums
                if (!archivedChecksums.TryGetValue(fieldName, out var archivedChecksum))
                {
                    // Field not found in archive
                    fieldResults[fieldName] = new FieldValidationResult
                    {
                        Matches = false,
                        ProvidedValue = fieldValue,
                        ProvidedChecksum = providedChecksum,
                        ArchivedChecksum = null,
                        Message = $"Field '{fieldName}' not found in archived report"
                    };
                    mismatchedFields.Add(fieldName);
                    continue;
                }

                // Compare checksums
                bool matches = string.Equals(providedChecksum, archivedChecksum, StringComparison.Ordinal);

                fieldResults[fieldName] = new FieldValidationResult
                {
                    Matches = matches,
                    ProvidedValue = fieldValue,
                    ProvidedChecksum = providedChecksum,
                    ArchivedChecksum = archivedChecksum,
                    Message = matches
                        ? "Value matches archived checksum"
                        : "Value does not match archived checksum - data drift detected"
                };

                if (!matches)
                {
                    mismatchedFields.Add(fieldName);
                }
            }

            // 4. Build response
            bool isValid = mismatchedFields.Count == 0;
            string message = isValid
                ? $"All {fieldsToValidate.Count} field(s) match archived checksums"
                : $"{mismatchedFields.Count} of {fieldsToValidate.Count} field(s) do not match: {string.Join(", ", mismatchedFields)}";

            var response = new ChecksumValidationResponse
            {
                ProfitYear = profitYear,
                ReportType = reportType,
                IsValid = isValid,
                FieldResults = fieldResults,
                MismatchedFields = mismatchedFields,
                Message = message,
                ArchivedAt = archived.CreatedAtUtc,
                ValidatedAt = DateTimeOffset.UtcNow
            };

            if (!isValid)
            {
                _logger.LogWarning(
                    "Checksum validation failed for {ReportType} year {ProfitYear}: {MismatchedCount} field(s) mismatched: {MismatchedFields}",
                    reportType,
                    profitYear,
                    mismatchedFields.Count,
                    string.Join(", ", mismatchedFields));
            }
            else
            {
                _logger.LogInformation(
                    "Checksum validation passed for {ReportType} year {ProfitYear}: all {FieldCount} field(s) matched",
                    reportType,
                    profitYear,
                    fieldsToValidate.Count);
            }

            return Result<ChecksumValidationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating checksums for {ReportType} year {ProfitYear}",
                reportType,
                profitYear);

            return Result<ChecksumValidationResponse>.Failure(
                Error.Unexpected($"Failed to validate checksums: {ex.Message}"));
        }
    }

    #endregion
}
