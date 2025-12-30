using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class CreateBeneficiaryRequestValidator : AbstractValidator<CreateBeneficiaryRequest>
{
    public CreateBeneficiaryRequestValidator()
    {
        RuleFor(x => x.BeneficiaryContactId)
            .GreaterThan(0)
            .WithMessage("Beneficiary Contact ID must be greater than 0.");

        RuleFor(x => x.EmployeeBadgeNumber)
            .GreaterThan(0)
            .WithMessage("Employee Badge Number must be greater than 0.");

        RuleFor(x => x.FirstLevelBeneficiaryNumber)
            .InclusiveBetween((byte)1, (byte)9)
            .When(x => x.FirstLevelBeneficiaryNumber.HasValue)
            .WithMessage("FirstLevelBeneficiaryNumber must be between 1 and 9.");

        RuleFor(x => x.SecondLevelBeneficiaryNumber)
            .InclusiveBetween((byte)1, (byte)9)
            .When(x => x.SecondLevelBeneficiaryNumber.HasValue)
            .WithMessage("SecondLevelBeneficiaryNumber must be between 1 and 9.");

        RuleFor(x => x.ThirdLevelBeneficiaryNumber)
            .InclusiveBetween((byte)1, (byte)9)
            .When(x => x.ThirdLevelBeneficiaryNumber.HasValue)
            .WithMessage("ThirdLevelBeneficiaryNumber must be between 1 and 9.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Percentage must be between 0 and 100.");

        RuleFor(x => x.Relationship)
            .NotEmpty()
            .WithMessage("Relationship is required.");
    }
}
