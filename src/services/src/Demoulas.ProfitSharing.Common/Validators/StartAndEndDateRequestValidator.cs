using Demoulas.Common.Contracts.Validators;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class StartAndEndDateRequestValidator : PaginationValidatorBase<StartAndEndDateRequest>
{
    public StartAndEndDateRequestValidator()
    {
        RuleFor(x => x.BeginningDate)
            .NotEmpty().WithMessage("Beginning date is required.");
  

        RuleFor(x => x.EndingDate)
            .NotEmpty().WithMessage("Ending date is required.")
            .GreaterThanOrEqualTo(x => x.BeginningDate)
            .WithMessage("Ending date must be greater than or equal to beginning date.");
    }
}
