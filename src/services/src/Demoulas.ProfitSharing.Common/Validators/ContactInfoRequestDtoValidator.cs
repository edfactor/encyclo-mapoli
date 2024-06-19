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
            .MaximumLength(50).WithMessage("EmailAddress cannot exceed 50 characters.");
    }
}
