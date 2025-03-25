using FluentValidation;
using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Services.Validation;

public class RehireForfeituresRequestValidator : AbstractValidator<RehireForfeituresRequest>
{
    public RehireForfeituresRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .GreaterThan((short)2000)
            .LessThanOrEqualTo((short)(DateTime.Now.Year + 1))
            .WithMessage("Profit year must be between 2000 and next year.");

        RuleFor(x => x.BeginningDate)
            .NotEmpty()
            .WithMessage("Beginning date is required.");

        RuleFor(x => x.EndingDate)
            .NotEmpty()
            .WithMessage("Ending date is required.");

        RuleFor(x => x.EndingDate)
            .GreaterThanOrEqualTo(x => x.BeginningDate)
            .WithMessage("Ending date must be greater than or equal to beginning date.");
    }
}
