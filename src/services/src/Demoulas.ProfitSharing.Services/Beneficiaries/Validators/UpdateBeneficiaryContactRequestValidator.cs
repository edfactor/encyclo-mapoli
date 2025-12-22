using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using FluentValidation;

namespace Demoulas.ProfitSharing.Services.Beneficiaries.Validators;

public class UpdateBeneficiaryContactRequestValidator : AbstractValidator<UpdateBeneficiaryContactRequest>
{
    public UpdateBeneficiaryContactRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Beneficiary Contact ID must be greater than 0.");

        RuleFor(x => x.ContactSsn)
            .InclusiveBetween(100000000, 999999999)
            .When(x => x.ContactSsn.HasValue)
            .WithMessage("Contact SSN must be a 9-digit number.");
    }
}
