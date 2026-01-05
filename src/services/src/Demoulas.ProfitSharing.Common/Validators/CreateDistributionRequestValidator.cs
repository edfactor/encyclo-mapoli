using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public sealed class CreateDistributionRequestValidator : AbstractValidator<CreateDistributionRequest>
{
    public CreateDistributionRequestValidator()
    {
        // Include badge number validation
        Include(new BadgeNumberRequestValidator<CreateDistributionRequest>());

        // StatusId: Required char
        RuleFor(x => x.StatusId)
            .NotEmpty()
            .WithMessage("StatusId is required.");

        // FrequencyId: Required char
        RuleFor(x => x.FrequencyId)
            .NotEmpty()
            .WithMessage("FrequencyId is required.");

        // TaxCodeId: Required char
        RuleFor(x => x.TaxCodeId)
            .NotEmpty()
            .WithMessage("TaxCodeId is required.");

        // GrossAmount: Must be greater than zero
        RuleFor(x => x.GrossAmount)
            .GreaterThan(0)
            .WithMessage("Gross amount must be greater than zero.")
            .Must(amount => amount >= 0)
            .WithMessage("Gross amount cannot be negative.");

        // FederalTaxPercentage: 0-100%
        RuleFor(x => x.FederalTaxPercentage)
            .InclusiveBetween(0m, 100m)
            .WithMessage("Federal tax percentage must be between 0 and 100.")
            .Must(rate => Math.Round(rate, 2, MidpointRounding.AwayFromZero) == rate)
            .WithMessage("Federal tax percentage must have up to 2 decimal places.");

        // StateTaxPercentage: 0-100%
        RuleFor(x => x.StateTaxPercentage)
            .InclusiveBetween(0m, 100m)
            .WithMessage("State tax percentage must be between 0 and 100.")
            .Must(rate => Math.Round(rate, 2, MidpointRounding.AwayFromZero) == rate)
            .WithMessage("State tax percentage must have up to 2 decimal places.");

        // FederalTaxAmount: Cannot be negative
        RuleFor(x => x.FederalTaxAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Federal tax amount cannot be negative.");

        // StateTaxAmount: Cannot be negative
        RuleFor(x => x.StateTaxAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("State tax amount cannot be negative.");

        // CheckAmount: Cannot be negative
        RuleFor(x => x.CheckAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Check amount cannot be negative.");

        // Business rule: CheckAmount should equal GrossAmount - FederalTax - StateTax
        RuleFor(x => x)
            .Must(x =>
            {
                var expectedCheckAmount = x.GrossAmount - x.FederalTaxAmount - x.StateTaxAmount;
                var difference = Math.Abs(x.CheckAmount - expectedCheckAmount);
                return difference < 0.01m; // Allow for rounding differences
            })
            .WithMessage("Check amount must equal gross amount minus federal tax minus state tax.");

        // Business rule: FederalTaxAmount should match FederalTaxPercentage of GrossAmount
        RuleFor(x => x)
            .Must(x =>
            {
                if (x.GrossAmount == 0) return true;
                var expectedFederalTax = Math.Round(x.GrossAmount * x.FederalTaxPercentage / 100m, 2, MidpointRounding.AwayFromZero);
                var difference = Math.Abs(x.FederalTaxAmount - expectedFederalTax);
                return difference < 0.01m; // Allow for rounding differences
            })
            .WithMessage("Federal tax amount does not match the federal tax percentage of gross amount.");

        // Business rule: StateTaxAmount should match StateTaxPercentage of GrossAmount
        RuleFor(x => x)
            .Must(x =>
            {
                if (x.GrossAmount == 0) return true;
                var expectedStateTax = Math.Round(x.GrossAmount * x.StateTaxPercentage / 100m, 2, MidpointRounding.AwayFromZero);
                var difference = Math.Abs(x.StateTaxAmount - expectedStateTax);
                return difference < 0.01m; // Allow for rounding differences
            })
            .WithMessage("State tax amount does not match the state tax percentage of gross amount.");

        // Optional fields validation
        RuleFor(x => x.Memo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Memo))
            .WithMessage("Memo cannot exceed 500 characters.");

        RuleFor(x => x.ForTheBenefitOfPayee)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.ForTheBenefitOfPayee))
            .WithMessage("For the benefit of payee cannot exceed 100 characters.");

        RuleFor(x => x.ForTheBenefitOfAccountType)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.ForTheBenefitOfAccountType))
            .WithMessage("For the benefit of account type cannot exceed 50 characters.");
    }
}
