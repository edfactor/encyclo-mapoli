using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class CreateBeneficiaryContactRequestValidator : AbstractValidator<CreateBeneficiaryContactRequest>
{
    public CreateBeneficiaryContactRequestValidator()
    {
        RuleFor(x => x.ContactSsn)
            .InclusiveBetween(100000000, 999999999)
            .WithMessage("Contact SSN must be a 9-digit number.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First Name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last Name is required.");
    }
}
