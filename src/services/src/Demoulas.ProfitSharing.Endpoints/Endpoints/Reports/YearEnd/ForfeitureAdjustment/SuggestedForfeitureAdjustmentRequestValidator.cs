using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.ForfeitureAdjustment;

public sealed class SuggestedForfeitureAdjustmentRequestValidator : AbstractValidator<SuggestedForfeitureAdjustmentRequest>
{
    public SuggestedForfeitureAdjustmentRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => (x.Ssn.HasValue && !x.Badge.HasValue) || (!x.Ssn.HasValue && x.Badge.HasValue))
            .WithMessage("Provide either SSN or badge number (but not both).");

        When(x => x.Ssn.HasValue, () =>
        {
            RuleFor(x => x.Ssn!.Value)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("SSN must be a positive 9-digit number.");
        });

        When(x => x.Badge.HasValue, () =>
        {
            RuleFor(x => x.Badge!.Value)
                .GreaterThan(0)
                .WithMessage("Badge number must be greater than 0.");
        });
    }
}
