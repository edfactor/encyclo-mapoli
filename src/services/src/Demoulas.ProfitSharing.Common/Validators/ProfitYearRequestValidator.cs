using Demoulas.ProfitSharing.Common.Interfaces;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class ProfitYearRequestValidator : AbstractValidator<IProfitYearRequest>
{
    public ProfitYearRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .NotEmpty()
            .WithMessage("Profit year is required.")
            .GreaterThanOrEqualTo((short)2020)
            .WithMessage("Profit years less than 2020 is not supported in this application");
    }
}
