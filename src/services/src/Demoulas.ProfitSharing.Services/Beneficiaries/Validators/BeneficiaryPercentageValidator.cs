using Demoulas.ProfitSharing.Data.Contexts;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Beneficiaries.Validators;

public class BeneficiaryPercentageValidator
{
    /// <summary>
    /// Validates that the sum of all beneficiary percentages for a badge number does not exceed 100%.
    /// This method should be called after adding or updating a beneficiary to ensure the total is valid.
    /// </summary>
    /// <param name="badgeNumber">The employee badge number to validate</param>
    /// <param name="ctx">Database context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public async Task<ValidationResult> ValidateBeneficiaryPercentageSumAsync(
        int badgeNumber,
        ProfitSharingDbContext ctx,
        CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        // Get all beneficiaries for this badge number from database
        var beneficiariesFromDb = await ctx.Beneficiaries
            .Where(x => x.BadgeNumber == badgeNumber)
            .ToListAsync(cancellationToken);

        // Also get any beneficiaries in the change tracker that haven't been saved yet
        var beneficiariesFromLocal = ctx.Beneficiaries.Local
            .Where(x => x.BadgeNumber == badgeNumber)
            .ToList();

        // Combine both lists and remove duplicates (by Id)
        var allBeneficiaries = beneficiariesFromDb
            .Concat(beneficiariesFromLocal)
            .GroupBy(b => b.Id)
            .Select(g => g.First())
            .ToList();

        // Only validate if there are beneficiaries
        if (allBeneficiaries.Count == 0)
        {
            return result; // No validation error if no beneficiaries
        }

        // Calculate the total percentage
        var totalPercentage = allBeneficiaries.Sum(b => b.Percent);

        // Check if total exceeds 100
        if (totalPercentage > 100m)
        {
            result.Errors.Add(new ValidationFailure(
                "Percentage",
                $"The sum of all beneficiary percentages for badge number {badgeNumber} cannot exceed 100%. Current total: {totalPercentage}%"));
        }

        return result;
    }

    /// <summary>
    /// Validates that adding or updating a beneficiary would result in a valid percentage sum.
    /// This method checks the hypothetical state before committing changes.
    /// </summary>
    /// <param name="badgeNumber">The employee badge number to validate</param>
    /// <param name="newPercentage">The percentage being added or updated to</param>
    /// <param name="beneficiaryIdToExclude">Optional: Beneficiary ID to exclude from calculation (for updates)</param>
    /// <param name="ctx">Database context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ValidationResult indicating success or failure</returns>
    public async Task<ValidationResult> ValidateBeneficiaryPercentageWithNewValueAsync(
        int badgeNumber,
        decimal newPercentage,
        int? beneficiaryIdToExclude,
        ProfitSharingDbContext ctx,
        CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        // Get all existing beneficiaries for this badge number, excluding the one being updated
        var query = ctx.Beneficiaries
            .Where(x => x.BadgeNumber == badgeNumber);

        if (beneficiaryIdToExclude.HasValue)
        {
            query = query.Where(x => x.Id != beneficiaryIdToExclude.Value);
        }

        var existingBeneficiaries = await query.ToListAsync(cancellationToken);

        // Calculate total percentage including the new/updated value
        var existingTotal = existingBeneficiaries.Sum(b => b.Percent);
        var proposedTotal = existingTotal + newPercentage;

        // Check if the proposed total would exceed 100%
        if (proposedTotal > 100m)
        {
            var operation = beneficiaryIdToExclude.HasValue ? "updating" : "creating";
            result.Errors.Add(new ValidationFailure(
                "Percentage",
                $"After {operation} this beneficiary, the sum of all beneficiary percentages for badge number {badgeNumber} would be {proposedTotal}%, which exceeds the maximum of 100%. Current total: {existingTotal}%"));
        }

        return result;
    }
}
