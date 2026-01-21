using Demoulas.ProfitSharing.Common.Contracts.Request.Administration;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public sealed class CreateTaxCodeRequestValidator : AbstractValidator<CreateTaxCodeRequest>
{
    public CreateTaxCodeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(id => id != null && id.Trim().Length == 1)
            .WithMessage("Id must be a single character.")
            .Matches("^[a-zA-Z0-9]$")
            .WithMessage("Id must be a single letter or number.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(128)
            .WithMessage("Name must be 128 characters or less.");
    }
}
