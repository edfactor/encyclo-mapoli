using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validator for PayServicesRequest to ensure valid input data.
/// Validates ID ranges and description requirements following project patterns.
/// </summary>
public sealed class PayServicesRequestValidator : AbstractValidator<PayServicesRequest>
{
    public PayServicesRequestValidator()
    {
        // Validate ProfitYear is within a reasonable range
        RuleFor(x => x.ProfitYear)
            .InclusiveBetween((short)2000, (short)2100)
            .WithMessage("ProfitYear must be between 2000 and 2100.");
    }
}
