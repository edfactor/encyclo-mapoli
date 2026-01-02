using Demoulas.ProfitSharing.Common.Contracts.Request.Lookups;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public sealed class StateTaxLookupRequestValidator : AbstractValidator<StateTaxLookupRequest>
{
    public StateTaxLookupRequestValidator()
    {
        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .Must(s => s != null && s.Trim().Length == 2)
            .WithMessage("State must be a two-letter abbreviation (A-Z).")
            .Matches("^[a-zA-Z]{2}$")
            .WithMessage("State must be a two-letter abbreviation (A-Z).");
    }
}
