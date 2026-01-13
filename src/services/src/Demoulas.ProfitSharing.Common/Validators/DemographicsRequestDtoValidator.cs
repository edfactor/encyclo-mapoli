using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class DemographicsRequestDtoValidator : Validator<DemographicsRequest>
{
    public DemographicsRequestDtoValidator()
    {
        RuleFor(x => x.Ssn)
            .NotEmpty()
            .InclusiveBetween(1000000, 999_99_9999).WithMessage("Must be a valid SSN number.");

        RuleFor(x => x.BadgeNumber)
            .InclusiveBetween(1, 9_999_999).WithMessage("BadgeNumber must be a 7-digit number.");

        RuleFor(x => x.OracleHcmId)
            .NotEmpty()
            .InclusiveBetween(1, 999_999_999_999_999).WithMessage("OracleHcmId must be a 15-digit number.");

        RuleFor(x => x.StoreNumber)
            .InclusiveBetween((short)1, (short)999)
            .WithMessage("StoreNumber must be a 3-digit number.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required.");

        RuleFor(x => x.PayClassificationId)
            .NotEmpty()
            .MaximumLength(4)
            .Matches("^[A-Z0-9]{1,4}$").WithMessage("PayClassificationId must be 1-4 chars alphanumeric (uppercase).");

        RuleFor(x => x.ContactInfo)
            .SetValidator(new ContactInfoRequestDtoValidator());

        RuleFor(x => x.Address)
            .SetValidator(new AddressRequestDtoValidator());

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("DateOfBirth is required.");

        RuleFor(x => x.HireDate)
            .NotEmpty().WithMessage("HireDate is required.");

        RuleFor(x => x.ReHireDate)
            .NotEmpty().WithMessage("ReHireDate is required.");
    }
}
