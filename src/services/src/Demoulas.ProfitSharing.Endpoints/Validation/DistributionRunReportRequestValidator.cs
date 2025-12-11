using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using Demoulas.ProfitSharing.Data.Entities;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation;

/// <summary>
/// Validator for DistributionRunReportRequest to enforce pagination limits and valid filter criteria.
/// Guards against degenerate queries and excessive batch sizes.
/// </summary>
public sealed class DistributionRunReportRequestValidator : AbstractValidator<DistributionRunReportRequest>
{
    public DistributionRunReportRequestValidator()
    {
        // Distribution frequency validation
        When(x => x.DistributionFrequencies != null && x.DistributionFrequencies.Length > 0, () =>
        {
            RuleFor(x => x.DistributionFrequencies)
                .Must(frequencies => frequencies!.All(f => IsValidDistributionFrequency(f)))
                .WithMessage("All distribution frequencies must be valid single characters (A, M, Q, S, P, etc.).");

            RuleFor(x => x.DistributionFrequencies)
                .Must(frequencies => frequencies!.Length <= 10)
                .WithMessage("Cannot filter by more than 10 distribution frequencies.");

            RuleFor(x => x.DistributionFrequencies)
                .Must(frequencies => frequencies!.Distinct().Count() == frequencies!.Length)
                .WithMessage("Duplicate distribution frequencies are not allowed.");
        });
    }

    private static bool IsValidDistributionFrequency(char frequency)
    {
        // Valid distribution frequency codes - adjust based on your business rules
        var validFrequencies = new[] { DistributionFrequency.Constants.Annually, DistributionFrequency.Constants.Monthly, DistributionFrequency.Constants.Hardship, DistributionFrequency.Constants.PayDirect, DistributionFrequency.Constants.Quarterly, DistributionFrequency.Constants.RolloverDirect };
        return validFrequencies.Contains(char.ToUpperInvariant(frequency));
    }
}
