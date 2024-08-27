using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class AddressRequestDtoValidator : Validator<AddressRequestDto>
{
    public AddressRequestDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(30).WithMessage("Street cannot exceed 30 characters.");

        RuleFor(x => x.Street2)
            .MaximumLength(30).WithMessage("Street2 cannot exceed 30 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(25).WithMessage("City cannot exceed 25 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(3).WithMessage("State cannot exceed 3 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("PostalCode must be in a valid format.");

        RuleFor(x => x.CountryIso)
            .NotEmpty()
            .MaximumLength(2)
            .WithMessage("CountryISO cannot exceed 2 characters.");
    }
}
