using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class DemographicsRequestDtoValidator : Validator<DemographicsRequestDto>
{
    public DemographicsRequestDtoValidator()
    {
        RuleFor(x => x.SSN)
            .NotEmpty()
            .InclusiveBetween(100000000, 999999999).WithMessage("SSN must be a 9-digit number.");

        RuleFor(x => x.BadgeNumber)
            .InclusiveBetween(1, 9999999).WithMessage("BadgeNumber must be a 7-digit number.");

        RuleFor(x => x.OracleHcmId)
            .NotEmpty()
            .InclusiveBetween(1, 999999999999999).WithMessage("OracleHcmId must be a 15-digit number.");

        RuleFor(x => x.FullName)
            .MaximumLength(60).WithMessage("FullName cannot exceed 60 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(30).WithMessage("LastName cannot exceed 30 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(30).WithMessage("FirstName cannot exceed 30 characters.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(25).WithMessage("MiddleName cannot exceed 25 characters.");

        RuleFor(x => x.StoreNumber)
            .InclusiveBetween((short)1, (short)999)
            .WithMessage("StoreNumber must be a 3-digit number.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.");

        RuleFor(x => x.PayClassificationId)
            .InclusiveBetween((byte)1, (byte)99)
            .WithMessage("PayClassificationId must be a 2-digit number.");

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

        RuleFor(x => x.TerminationCode)
            .IsInEnum().WithMessage("TerminationCode must be a valid enum value.");

        RuleFor(x => x.EmploymentType)
            .IsInEnum().WithMessage("EmploymentType must be a valid enum value.");

        RuleFor(x => x.PayFrequency)
            .IsInEnum().WithMessage("PayFrequency must be a valid enum value.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender must be a valid enum value.");
    }
}
