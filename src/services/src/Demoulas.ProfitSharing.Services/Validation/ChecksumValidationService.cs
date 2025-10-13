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
/// Service for validating archived report checksums against caller-provided field values.
/// Detects data drift by comparing stored checksums with current values from the caller.
/// Orchestrates cross-reference validations including balance integrity checks.
/// </summary>
public sealed class ChecksumValidationService : IChecksumValidationService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IBalanceValidationService _balanceValidationService;
    private readonly ILogger<ChecksumValidationService> _logger;

    public ChecksumValidationService(
        IProfitSharingDataContextFactory dataContextFactory,
        IBalanceValidationService balanceValidationService,
        ILogger<ChecksumValidationService> logger)
    {
        _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        _balanceValidationService = balanceValidationService ?? throw new ArgumentNullException(nameof(balanceValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result<ChecksumValidationResponse>> ValidateReportFieldsAsync(
        short profitYear,
        string reportType,
        Dictionary<string, decimal> fieldsToValidate,
        CancellationToken cancellationToken)
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

    /// <summary>
    /// OPTIMIZATION: Load all archived checksums for a profit year in a single query.
    /// This eliminates N+1 query problems where each field validation makes a separate DB call.
    /// </summary>
    /// <returns>Dictionary keyed by "ReportCode.FieldName" containing archived values and metadata</returns>
    private async Task<Dictionary<string, ArchivedFieldData>> LoadAllArchivedChecksumsAsync(
        short profitYear,
        CancellationToken cancellationToken)
    {
        var cache = new Dictionary<string, ArchivedFieldData>(StringComparer.OrdinalIgnoreCase);

        await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Single query to fetch all archived checksums for the year
            var allArchivedReports = await ctx.ReportChecksums
                .TagWith($"LoadAllArchivedChecksums-Year{profitYear}")
                .Where(r => r.ProfitYear == profitYear)
                .ToListAsync(cancellationToken);

            // Group by report type and take most recent for each
            var latestByReportType = allArchivedReports
                .GroupBy(r => r.ReportType)
                .Select(g => g.OrderByDescending(r => r.CreatedAtUtc).First());

            // Build cache dictionary
            foreach (var report in latestByReportType)
            {
                foreach (var field in report.KeyFieldsChecksumJson)
                {
                    string cacheKey = $"{report.ReportType}.{field.Key}";
                    cache[cacheKey] = new ArchivedFieldData
                    {
                        Value = field.Value.Key, // The actual archived value
                        ChecksumHash = field.Value.Value, // The hash bytes
                        ArchivedAt = report.CreatedAtUtc.DateTime,
                        ReportType = report.ReportType
                    };
                }
            }

            return Task.CompletedTask;
        }, cancellationToken);

        return cache;
    }

    /// <summary>
    /// Helper class for caching archived field data to eliminate N+1 queries.
    /// </summary>
    private sealed class ArchivedFieldData
    {
        public decimal Value { get; init; }
        public byte[] ChecksumHash { get; init; } = Array.Empty<byte>();
        public DateTime ArchivedAt { get; init; }
        public string ReportType { get; init; } = string.Empty;
    }

    /// <inheritdoc />
    public async Task<Result<MasterUpdateCrossReferenceValidationResponse>> ValidateMasterUpdateCrossReferencesAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationGroups = new List<CrossReferenceValidationGroup>();
            var validatedReports = new HashSet<string>();
            var criticalIssues = new List<string>();
            var warnings = new List<string>();
            int totalValidations = 0;
            int passedValidations = 0;
            int failedValidations = 0;

            // Group 0: Beginning Balance (PAY444 vs archived PAY443 for SAME year)
            var beginningBalanceGroup = await ValidateBeginningBalanceGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);
            validationGroups.Add(beginningBalanceGroup);
            totalValidations += beginningBalanceGroup.Validations.Count;
            passedValidations += beginningBalanceGroup.Validations.Count(v => v.IsValid);
            failedValidations += beginningBalanceGroup.Validations.Count(v => !v.IsValid);
            if (!beginningBalanceGroup.IsValid)
            {
                criticalIssues.Add($"CRITICAL: Beginning balance mismatch - PAY444 does not match archived PAY443 for year {profitYear}");
            }

            // Group 1: Total Distributions (4-way match)
            var distributionsGroup = await ValidateDistributionsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);
            validationGroups.Add(distributionsGroup);
            totalValidations += distributionsGroup.Validations.Count;
            passedValidations += distributionsGroup.Validations.Count(v => v.IsValid);
            failedValidations += distributionsGroup.Validations.Count(v => !v.IsValid);
            if (!distributionsGroup.IsValid)
            {
                criticalIssues.Add($"Distribution totals mismatch detected across reports");
            }

            // Group 2: Total Forfeitures (3-way match)
            var forfeituresGroup = await ValidateForfeituresGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);
            validationGroups.Add(forfeituresGroup);
            totalValidations += forfeituresGroup.Validations.Count;
            passedValidations += forfeituresGroup.Validations.Count(v => v.IsValid);
            failedValidations += forfeituresGroup.Validations.Count(v => !v.IsValid);
            if (!forfeituresGroup.IsValid)
            {
                criticalIssues.Add($"Forfeiture totals mismatch detected across reports");
            }

            // Group 3: Total Contributions (2-way match)
            var contributionsGroup = await ValidateContributionsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);
            validationGroups.Add(contributionsGroup);
            totalValidations += contributionsGroup.Validations.Count;
            passedValidations += contributionsGroup.Validations.Count(v => v.IsValid);
            failedValidations += contributionsGroup.Validations.Count(v => !v.IsValid);
            if (!contributionsGroup.IsValid)
            {
                warnings.Add($"Contribution totals mismatch - review PAY443 and PAY444 values");
            }

            // Group 4: Total Earnings (2-way match)
            var earningsGroup = await ValidateEarningsGroupAsync(
                profitYear, currentValues, validatedReports, cancellationToken);
            validationGroups.Add(earningsGroup);
            totalValidations += earningsGroup.Validations.Count;
            passedValidations += earningsGroup.Validations.Count(v => v.IsValid);
            failedValidations += earningsGroup.Validations.Count(v => !v.IsValid);
            if (!earningsGroup.IsValid)
            {
                warnings.Add($"Earnings totals mismatch - review PAY443 and PAY444 values");
            }

            // Group 5: ALLOC/PAID ALLOC Transfers (Balance Matrix Rule 2)
            var allocTransfersGroup = await ValidateAllocTransfersGroupAsync(
                profitYear, cancellationToken);
            validationGroups.Add(allocTransfersGroup);
            totalValidations += allocTransfersGroup.Validations.Count;
            passedValidations += allocTransfersGroup.Validations.Count(v => v.IsValid);
            failedValidations += allocTransfersGroup.Validations.Count(v => !v.IsValid);
            if (!allocTransfersGroup.IsValid)
            {
                criticalIssues.Add($"CRITICAL: ALLOC/PAID ALLOC transfers do not balance to zero for year {profitYear} - Balance integrity violation");
            }

            // Determine overall validation status
            bool isValid = failedValidations == 0;
            bool blockMasterUpdate = criticalIssues.Any(); // Block on critical issues only

            string message = isValid
                ? $"All {totalValidations} cross-reference validations passed successfully."
                : $"{failedValidations} of {totalValidations} validations failed. Review details below.";

            var response = new MasterUpdateCrossReferenceValidationResponse
            {
                ProfitYear = profitYear,
                IsValid = isValid,
                Message = message,
                ValidationGroups = validationGroups,
                TotalValidations = totalValidations,
                PassedValidations = passedValidations,
                FailedValidations = failedValidations,
                ValidatedReports = validatedReports.OrderBy(r => r).ToList(),
                BlockMasterUpdate = blockMasterUpdate,
                CriticalIssues = criticalIssues,
                Warnings = warnings
            };

            _logger.LogInformation(
                "Master Update cross-reference validation completed for year {ProfitYear}: {PassedValidations}/{TotalValidations} passed, Block={Block}",
                profitYear, passedValidations, totalValidations, blockMasterUpdate);

            return Result<MasterUpdateCrossReferenceValidationResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error performing Master Update cross-reference validation for year {ProfitYear}",
                profitYear);

            return Result<MasterUpdateCrossReferenceValidationResponse>.Failure(
                Error.Unexpected($"Failed to validate Master Update cross-references: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public async Task<Result<MasterUpdateCrossReferenceValidationResponse>> GetMasterUpdateArchivedValuesAsync(
        short profitYear,
        CancellationToken cancellationToken)
    {
        try
        {
            // Simply call the existing validation method with empty dictionary
            // The validation groups will contain expectedValue for each field (the archived values)
            // The UI can use these expectedValue fields to compare against its current values
            var emptyCurrentValues = new Dictionary<string, decimal>();

            return await ValidateMasterUpdateCrossReferencesAsync(
                profitYear,
                emptyCurrentValues,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving Master Update archived values for year {ProfitYear}",
                profitYear);

            return Result<MasterUpdateCrossReferenceValidationResponse>.Failure(
                Error.Unexpected($"Failed to retrieve Master Update archived values: {ex.Message}"));
        }
    }

    /// <summary>
    /// Group 0: Validates beginning balance against archived PAY443 for SAME year.
    /// Validation Rule: PAY444.BeginningBalance (year N) should match PAY443.TotalProfitSharingBalance (archived year N)
    /// This validates that the current Master Update matches what was finalized at year-end.
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateBeginningBalanceGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // Get archived PAY443.TotalProfitSharingBalance for SAME year (profitYear)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear,
            "PAY443",
            "TotalProfitSharingBalance",
            currentValues,
            cancellationToken);

        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");
        validatedReports.Add("PAY444");

        bool allValid = validations.All(v => v.IsValid);

        string summary = allValid
            ? $"✅ Beginning balance matches archived PAY443 for year {profitYear}"
            : $"⚠️ Beginning balance does NOT match archived PAY443 for year {profitYear}";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Beginning Balance",
            Description = $"Validates PAY444.BeginningBalance matches archived PAY443.TotalProfitSharingBalance for year {profitYear}",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = $"PAY444.BeginningBalance should equal archived PAY443.TotalProfitSharingBalance"
        };
    }

    /// <summary>
    /// Validates the Total Distributions cross-references (PAY443, QPAY129, QPAY066TA).
    /// Validation Rule: PAY444.DISTRIB = PAY443.TotalDistributions = QPAY129.Distributions = QPAY066TA.TotalDisbursements
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateDistributionsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.DistributionTotals
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "DistributionTotals", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        // QPAY129.Distributions
        var qpay129Validation = await ValidateSingleFieldAsync(
            profitYear, "QPAY129", "Distributions", currentValues, cancellationToken);
        validations.Add(qpay129Validation);
        validatedReports.Add("QPAY129");

        // QPAY066TA.TotalDisbursements
        var qpay066taValidation = await ValidateSingleFieldAsync(
            profitYear, "QPAY066TA", "TotalDisbursements", currentValues, cancellationToken);
        validations.Add(qpay066taValidation);
        validatedReports.Add("QPAY066TA");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "All distribution totals are in sync across PAY443, QPAY129, and QPAY066TA."
            : "Distribution totals are OUT OF SYNC. This is a critical financial discrepancy.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Distributions",
            Description = "Cross-validation of distribution totals across year-end reports",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = "PAY444.DISTRIB = PAY443.DistributionTotals = QPAY129.Distributions = QPAY066TA.TotalDisbursements"
        };
    }

    /// <summary>
    /// Validates the Total Forfeitures cross-references (PAY443, QPAY129).
    /// Validation Rule: PAY444.FORFEITS = PAY443.TotalForfeitures = QPAY129.ForfeitedAmount
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateForfeituresGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalForfeitures
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalForfeitures", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        // QPAY129.ForfeitedAmount
        var qpay129Validation = await ValidateSingleFieldAsync(
            profitYear, "QPAY129", "ForfeitedAmount", currentValues, cancellationToken);
        validations.Add(qpay129Validation);
        validatedReports.Add("QPAY129");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "All forfeiture totals are in sync across PAY443 and QPAY129."
            : "Forfeiture totals are OUT OF SYNC. Review PAY443 and QPAY129 data.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Forfeitures",
            Description = "Cross-validation of forfeiture totals across year-end reports",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "Critical",
            ValidationRule = "PAY444.FORFEITS = PAY443.TotalForfeitures = QPAY129.ForfeitedAmount"
        };
    }

    /// <summary>
    /// Validates the Total Contributions cross-references (PAY443).
    /// Validation Rule: PAY444.CONTRIB = PAY443.TotalContributions
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateContributionsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalContributions (if it exists - need to add to response DTO)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalContributions", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "Contribution totals are in sync."
            : "Contribution totals mismatch detected.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Contributions",
            Description = "Cross-validation of contribution totals",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "High",
            ValidationRule = "PAY444.CONTRIB = PAY443.TotalContributions"
        };
    }

    /// <summary>
    /// Validates the Total Earnings cross-references (PAY443).
    /// Validation Rule: PAY444.EARNINGS = PAY443.TotalEarnings
    /// </summary>
    private async Task<CrossReferenceValidationGroup> ValidateEarningsGroupAsync(
        short profitYear,
        Dictionary<string, decimal> currentValues,
        HashSet<string> validatedReports,
        CancellationToken cancellationToken)
    {
        var validations = new List<CrossReferenceValidation>();

        // PAY443.TotalEarnings (if it exists - need to add to response DTO)
        var pay443Validation = await ValidateSingleFieldAsync(
            profitYear, "PAY443", "TotalEarnings", currentValues, cancellationToken);
        validations.Add(pay443Validation);
        validatedReports.Add("PAY443");

        bool allValid = validations.All(v => v.IsValid);
        string summary = allValid
            ? "Earnings totals are in sync."
            : "Earnings totals mismatch detected.";

        return new CrossReferenceValidationGroup
        {
            GroupName = "Total Earnings",
            Description = "Cross-validation of earnings totals",
            IsValid = allValid,
            Validations = validations,
            Summary = summary,
            Priority = "High",
            ValidationRule = "PAY444.EARNINGS = PAY443.TotalEarnings"
        };
    }

    /// <summary>
    /// Validates that ALLOC (Incoming QDRO Beneficiary) and PAID ALLOC (Outgoing XFER Beneficiary) 
    /// transactions sum to zero, per Balance Matrix Rule 2.
    /// Delegates to IBalanceValidationService for the actual validation logic.
    /// </summary>
    /// <remarks>
    /// ALLOC: ProfitCodeId = 6 (IncomingQdroBeneficiary), stored in Contribution field.
    /// PAID ALLOC: ProfitCodeId = 5 (OutgoingXferBeneficiary), stored in Forfeiture field.
    /// Rule: Sum(ALLOC) + Sum(PAID ALLOC) = 0
    /// Reference: BALANCE_REPORTS_CROSS_REFERENCE_MATRIX.md - Rule 2
    /// </remarks>
    private async Task<CrossReferenceValidationGroup> ValidateAllocTransfersGroupAsync(
        short profitYear,
        CancellationToken cancellationToken)
    {
        // Delegate to dedicated balance validation service
        var result = await _balanceValidationService.ValidateAllocTransfersAsync(profitYear, cancellationToken);

        // Return the validation group, or a default error group if the service call failed
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        _logger.LogError("Balance validation service returned failure: {Error}", result.Error?.Description);

        // Return error validation group
        return new CrossReferenceValidationGroup
        {
            GroupName = "ALLOC/PAID ALLOC Transfers",
            Description = $"Error validating ALLOC transfers for year {profitYear}",
            IsValid = false,
            Validations = new List<CrossReferenceValidation>
            {
                new CrossReferenceValidation
                {
                    FieldName = "NetAllocTransfer",
                    ReportCode = "PAY444",
                    CurrentValue = null,
                    ExpectedValue = 0m,
                    IsValid = false,
                    Message = $"Service error: {result.Error?.Description ?? "Unknown error"}",
                    Notes = "Balance validation service failed"
                }
            },
            Summary = $"❌ Error validating ALLOC transfers: {result.Error?.Description ?? "Unknown error"}",
            Priority = "Critical",
            ValidationRule = "Sum(ALLOC) + Sum(PAID ALLOC) must equal 0 (Balance Matrix Rule 2)"
        };
    }

    /// <summary>
    /// Validates a single field against its archived checksum.
    /// </summary>
    private async Task<CrossReferenceValidation> ValidateSingleFieldAsync(
        short profitYear,
        string reportCode,
        string fieldName,
        Dictionary<string, decimal> currentValues,
        CancellationToken cancellationToken)
    {
        try
        {
            // Look up current value using "ReportCode.FieldName" key
            string lookupKey = $"{reportCode}.{fieldName}";
            decimal? currentValue = currentValues.TryGetValue(lookupKey, out decimal value)
                ? value
                : null;

            // Always fetch the archived/expected value, even if no current value provided
            // This allows the UI to perform its own comparison
            decimal? expectedValue = null;
            DateTime? archivedAt = null;

            var archived = await _dataContextFactory.UseReadOnlyContext(async ctx =>
                await ctx.ReportChecksums
                    .TagWith($"GetArchivedValue-{reportCode}-{fieldName}-Year{profitYear}")
                    .Where(r => r.ProfitYear == profitYear && r.ReportType == reportCode)
                    .OrderByDescending(r => r.CreatedAtUtc)
                    .FirstOrDefaultAsync(cancellationToken), cancellationToken);

            if (archived != null)
            {
                archivedAt = archived.CreatedAtUtc.DateTime;
                var archivedFieldData = archived.KeyFieldsChecksumJson
                    .FirstOrDefault(kvp => kvp.Key == fieldName);

                if (!string.IsNullOrEmpty(archivedFieldData.Key))
                {
                    expectedValue = archivedFieldData.Value.Key; // The actual archived value
                }
            }

            if (currentValue == null)
            {
                // Return archived value even though no current value to compare against
                // UI will use expectedValue for its own comparison
                return new CrossReferenceValidation
                {
                    FieldName = fieldName,
                    ReportCode = reportCode,
                    IsValid = false, // Cannot validate without current value
                    CurrentValue = null,
                    ExpectedValue = expectedValue,
                    ArchivedAt = archivedAt,
                    Message = $"Current value not provided for {reportCode}.{fieldName}",
                    Notes = expectedValue.HasValue
                        ? $"Archived value available: {expectedValue.Value:N2}"
                        : "No archived value found"
                };
            }

            // Validate against archived checksum
            var validationResult = await ValidateReportFieldsAsync(
                profitYear,
                reportCode,
                new Dictionary<string, decimal> { [fieldName] = currentValue.Value },
                cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return new CrossReferenceValidation
                {
                    FieldName = fieldName,
                    ReportCode = reportCode,
                    IsValid = false,
                    CurrentValue = currentValue,
                    Message = $"Validation failed: {validationResult.Error?.Description ?? "Unknown error"}",
                    Notes = "Error during validation"
                };
            }

            var checksumResponse = validationResult.Value;
            bool fieldIsValid = checksumResponse?.IsValid ?? false;

            // expectedValue and archivedAt already fetched above

            decimal? variance = expectedValue.HasValue && currentValue.HasValue
                ? currentValue - expectedValue
                : null;

            string message = fieldIsValid
                ? $"{reportCode}.{fieldName} matches archived value"
                : $"{reportCode}.{fieldName} does NOT match archived value";

            return new CrossReferenceValidation
            {
                FieldName = fieldName,
                ReportCode = reportCode,
                IsValid = fieldIsValid,
                CurrentValue = currentValue,
                ExpectedValue = expectedValue,
                Variance = variance,
                Message = message,
                ArchivedAt = checksumResponse?.ArchivedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error validating {ReportCode}.{FieldName} for year {ProfitYear}",
                reportCode, fieldName, profitYear);

            return new CrossReferenceValidation
            {
                FieldName = fieldName,
                ReportCode = reportCode,
                IsValid = false,
                Message = $"Validation error: {ex.Message}",
                Notes = "Exception during validation"
            };
        }
    }
}
