using System.Diagnostics.Metrics;
using Demoulas.ProfitSharing.Common.Interfaces;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validates beneficiary percentage sum constraints as an async FluentValidation validator.
/// Follows the same pattern as MilitaryContributionRequestValidator by injecting services 
/// and making async database calls during validation.
/// </summary>
/// <remarks>
/// This validator ensures that the sum of all beneficiary percentages for an employee 
/// does not exceed 100%. Unlike the original implementation which was called after saving,
/// this validator integrates directly into the FluentValidation pipeline and validates 
/// during request processing before changes are persisted.
/// </remarks>
public class BeneficiaryPercentageValidator : Validator<dynamic>
{
    private readonly IBeneficiaryService _beneficiaryService;
    private readonly ILogger<BeneficiaryPercentageValidator> _logger;
    private static readonly Meter s_meter = new("Demoulas.ProfitSharing.Validators");
    private static readonly Counter<long> s_validationFailures = s_meter.CreateCounter<long>(
        "ps_validation_failures_total",
        description: "Counts of validation failures by validator and rule");

    public BeneficiaryPercentageValidator(IBeneficiaryService beneficiaryService, ILogger<BeneficiaryPercentageValidator> logger)
    {
        _beneficiaryService = beneficiaryService;
        _logger = logger;
    }

    /// <summary>
    /// Validates that the sum of all beneficiary percentages for a badge number does not exceed 100%.
    /// This method validates the hypothetical state after adding or updating a beneficiary.
    /// </summary>
    /// <param name="badgeNumber">The employee badge number to validate</param>
    /// <param name="newPercentage">The percentage being added or updated to</param>
    /// <param name="beneficiaryIdToExclude">Optional: Beneficiary ID to exclude from calculation (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if valid, false otherwise with failure tracking</returns>
    public async Task<bool> ValidateBeneficiaryPercentageWithNewValueAsync(
        int badgeNumber,
        decimal newPercentage,
        int? beneficiaryIdToExclude,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get existing beneficiary percentage sum for this badge, excluding the one being updated
            var existingTotal = await _beneficiaryService.GetBeneficiaryPercentageSumAsync(
                badgeNumber,
                beneficiaryIdToExclude,
                cancellationToken);

            // If badge not found (returns -1), allow validation to pass
            if (existingTotal < 0)
            {
                return true;
            }

            var proposedTotal = existingTotal + newPercentage;

            // Check if the proposed total would exceed 100%
            if (proposedTotal > 100m)
            {
                return TrackFailure("BeneficiaryPercentageExceedsMaximum");
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log the error for debugging but allow validation to pass (database error will be caught separately)
            _logger.LogError(ex, "Error validating beneficiary percentage for badge {BadgeNumber} with percentage {NewPercentage}", 
                badgeNumber, newPercentage);
            return true;
        }
    }

    private static bool TrackFailure(string rule)
    {
        s_validationFailures.Add(1,
            new KeyValuePair<string, object?>("validator", "BeneficiaryPercentage"),
            new KeyValuePair<string, object?>("rule", rule));
        return false;
    }
}
