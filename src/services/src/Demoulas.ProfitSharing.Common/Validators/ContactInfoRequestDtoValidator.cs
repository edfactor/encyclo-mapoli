using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class ContactInfoRequestDtoValidator : Validator<ContactInfoRequestDto>
{
    public ContactInfoRequestDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(15).WithMessage("PhoneNumber cannot exceed 15 characters.");

        RuleFor(x => x.MobileNumber)
            .MaximumLength(15).WithMessage("MobileNumber cannot exceed 15 characters.");

        RuleFor(x => x.EmailAddress)
            .MaximumLength(84).WithMessage("EmailAddress cannot exceed 84 characters.");

        RuleFor(x => x.FullName)
            .MaximumLength(84).WithMessage("FullName cannot exceed 84 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(30).WithMessage("LastName cannot exceed 30 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(30).WithMessage("FirstName cannot exceed 30 characters.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(25).WithMessage("MiddleName cannot exceed 25 characters.");
    }
}
