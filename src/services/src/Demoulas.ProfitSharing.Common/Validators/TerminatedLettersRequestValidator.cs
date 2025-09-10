using Demoulas.Common.Contracts.Validators;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class TerminatedLettersRequestValidator : PaginationValidatorBase<TerminatedLettersRequest>
{
    public TerminatedLettersRequestValidator()
    {
        RuleFor(x => x.BeginningDate)
            .LessThanOrEqualTo(x => x.EndingDate)
            .When(x => x.BeginningDate.HasValue && x.EndingDate.HasValue)
            .WithMessage("Beginning date must be less than or equal to ending date.");

        RuleFor(x => x.EndingDate)
            .GreaterThanOrEqualTo(x => x.BeginningDate)
            .When(x => x.BeginningDate.HasValue && x.EndingDate.HasValue)
            .WithMessage("Ending date must be greater than or equal to beginning date.");

        RuleForEach(x => x.BadgeNumbers)
            .InclusiveBetween(10000, 9999999)
            .WithMessage("Badge numbers must be 5-7-digit positive numbers (10000-9999999).");
    }
}