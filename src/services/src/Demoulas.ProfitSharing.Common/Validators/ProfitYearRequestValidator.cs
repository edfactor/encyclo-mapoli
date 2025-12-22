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
            .WithMessage("Profit years less than 2020 are not supported in this application")
            .LessThanOrEqualTo((short)(DateTime.UtcNow.Year + 1))
            .WithMessage($"Profit year cannot be greater than {DateTime.UtcNow.Year + 1}.");
    }
}
