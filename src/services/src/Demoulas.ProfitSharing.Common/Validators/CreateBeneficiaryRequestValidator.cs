using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class CreateBeneficiaryRequestValidator : AbstractValidator<CreateBeneficiaryRequest>
{
    public CreateBeneficiaryRequestValidator(IBeneficiaryService beneficiaryService)
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

        // Validate that the sum of all beneficiary percentages doesn't exceed 100%
        RuleFor(x => x)
            .MustAsync(async (request, cancellationToken) =>
            {
                var existingPercentageSum = await beneficiaryService.GetBeneficiaryPercentageSumAsync(
                    request.EmployeeBadgeNumber,
                    null,
                    cancellationToken);

                // If we got an error (-1), allow validation to pass (the error will be caught elsewhere)
                if (existingPercentageSum < 0)
                {
                    return true;
                }

                return (existingPercentageSum + request.Percentage) <= 100m;
            })
            .WithMessage("The sum of all beneficiary percentages would exceed 100%.");
    }
}
