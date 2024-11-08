using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.OracleHcm.Validators;

public class AddressItemValidator : Validator<AddressItem>
{
    public AddressItemValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(30).WithMessage("Street cannot exceed 30 characters.");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(30).WithMessage("Street2 cannot exceed 30 characters.");

        RuleFor(x => x.TownOrCity)
            .NotEmpty()
            .MaximumLength(25).WithMessage("City cannot exceed 25 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(3).WithMessage("State cannot exceed 3 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("PostalCode must be in a valid format.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(2)
            .WithMessage("CountryISO cannot exceed 2 characters.");
    }
}
