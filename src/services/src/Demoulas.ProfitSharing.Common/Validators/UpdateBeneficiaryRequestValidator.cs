using Demoulas.ProfitSharing.Common.Contracts.Request.Beneficiaries;
using Demoulas.ProfitSharing.Common.Interfaces;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class UpdateBeneficiaryRequestValidator : AbstractValidator<UpdateBeneficiaryRequest>
{
    public UpdateBeneficiaryRequestValidator(IBeneficiaryService? beneficiaryService = null, int? badgeNumber = null, int? beneficiaryId = null)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Beneficiary ID must be greater than 0.");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(0.01m, 100m)
            .When(x => x.Percentage.HasValue)
            .WithMessage("Percentage must be between 0 and 100.");

        // Validate that the sum of all beneficiary percentages doesn't exceed 100%
        if (beneficiaryService != null && badgeNumber.HasValue && beneficiaryId.HasValue)
        {
            RuleFor(x => x.Percentage)
                .MustAsync(async (percentage, cancellationToken) =>
                {
                    if (!percentage.HasValue)
                    {
                        return true;
                    }

                    var existingPercentageSum = await beneficiaryService.GetBeneficiaryPercentageSumAsync(
                        badgeNumber.Value,
                        beneficiaryId.Value,
                        cancellationToken);

                    // If we got an error (-1), allow validation to pass (the error will be caught elsewhere)
                    if (existingPercentageSum < 0)
                    {
                        return true;
                    }

                    return (existingPercentageSum + percentage.Value) <= 100m;
                })
                .When(x => x.Percentage.HasValue)
                .WithMessage("The sum of all beneficiary percentages would exceed 100%.");
        }
    }
}
