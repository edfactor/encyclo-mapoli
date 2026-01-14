using Demoulas.ProfitSharing.Common.Contracts.Request.ItOperations;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public sealed class UpdateStateTaxRateRequestValidator : AbstractValidator<UpdateStateTaxRateRequest>
{
    public UpdateStateTaxRateRequestValidator()
    {
        RuleFor(x => x.Abbreviation)
            .NotEmpty()
            .WithMessage("Abbreviation is required.")
            .Must(a => a != null && a.Trim().Length == 2)
            .WithMessage("Abbreviation must be two letters (A-Z).")
            .Matches("^[a-zA-Z]{2}$")
            .WithMessage("Abbreviation must be two letters (A-Z).");

        RuleFor(x => x.Rate)
            .InclusiveBetween(0m, 100m)
            .WithMessage("Rate must be between 0 and 100.")
            .Must(rate => Math.Round(rate, 2, MidpointRounding.AwayFromZero) == rate)
            .WithMessage("Rate must have up to 2 decimal places.");
    }
}
