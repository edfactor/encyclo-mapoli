using Demoulas.Common.Contracts.Validators;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class StartAndEndDateRequestValidator : PaginationValidatorBase<StartAndEndDateRequest>
{
    public StartAndEndDateRequestValidator()
    {
        // Date validation
        RuleFor(x => x.BeginningDate)
            .NotEmpty().WithMessage("Beginning date is required.");

        RuleFor(x => x.EndingDate)
            .NotEmpty().WithMessage("Ending date is required.")
            .GreaterThanOrEqualTo(x => x.BeginningDate)
            .WithMessage("Ending date must be greater than or equal to beginning date.");

        // Pagination validation (explicit rules since base class doesn't provide them)
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Skip must be greater than or equal to 0.");

        RuleFor(x => x.Take)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Take must be greater than or equal to 1.")
            .Must(take => take <= 10000 || take == int.MaxValue)
            .WithMessage("Take must be less than or equal to 10000 (or int.MaxValue for CSV exports).");
    }
}
