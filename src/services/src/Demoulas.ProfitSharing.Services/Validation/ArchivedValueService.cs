using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Validation;

/// <summary>
/// Service for retrieving archived report values from stored checksums.
/// Extracted from ChecksumValidationService to improve separation of concerns (PS-1721).
/// </summary>
/// <remarks>
/// This service provides access to historical report field values that were archived
/// when reports were finalized. These archived values are used for cross-reference validation
/// and audit trail purposes.
/// </remarks>
public class ArchivedValueService : IArchivedValueService
{
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;
    private readonly ILogger<ArchivedValueService> _logger;

    public ArchivedValueService(
        ICrossReferenceValidationService crossReferenceValidationService,
        ILogger<ArchivedValueService> logger)
    {
        _crossReferenceValidationService = crossReferenceValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves archived values for all Master Update cross-reference fields for a given year.
    /// </summary>
    /// <param name="profitYear">The profit year to retrieve archived values for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary of archived values keyed by "ReportCode.FieldName", or null on failure.</returns>
    /// <remarks>
    /// This method leverages the cross-reference validation logic with an empty currentValues dictionary
    /// to extract the ExpectedValue (archived value) for each field without performing actual validation.
    /// </remarks>
    public async Task<Result<Dictionary<string, decimal>?>> GetMasterUpdateArchivedValuesAsync(
        short profitYear,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving archived values for Master Update year {ProfitYear}", profitYear);

            // Call cross-reference validation with empty current values to get archived values
            var validationResult = await _crossReferenceValidationService.ValidateMasterUpdateCrossReferencesAsync(
                profitYear,
                new Dictionary<string, decimal>(), // Empty dictionary - we only want archived values
                cancellationToken);

            if (!validationResult.IsSuccess)
            {
                _logger.LogError(
                    "Failed to retrieve archived values for year {ProfitYear}: {Error}",
                    profitYear,
                    validationResult.Error?.Description);
                return Result<Dictionary<string, decimal>?>.Failure(validationResult.Error!);
            }

            var validationResponse = validationResult.Value;
            if (validationResponse == null)
            {
                _logger.LogWarning("Validation response is null for year {ProfitYear}", profitYear);
                return Result<Dictionary<string, decimal>?>.Success(null);
            }

            // Extract archived (expected) values from validation groups
            var archivedValues = new Dictionary<string, decimal>();

            foreach (var group in validationResponse.ValidationGroups)
            {
                foreach (var validation in group.Validations)
                {
                    if (validation.ExpectedValue.HasValue)
                    {
                        string key = $"{validation.ReportCode}.{validation.FieldName}";
                        archivedValues[key] = validation.ExpectedValue.Value;
                    }
                }
            }

            _logger.LogInformation(
                "Retrieved {Count} archived values for year {ProfitYear}",
                archivedValues.Count,
                profitYear);

            return Result<Dictionary<string, decimal>?>.Success(archivedValues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving archived values for year {ProfitYear}", profitYear);
            return Result<Dictionary<string, decimal>?>.Failure(
                Error.Unexpected($"Failed to retrieve archived values: {ex.Message}"));
        }
    }
}
