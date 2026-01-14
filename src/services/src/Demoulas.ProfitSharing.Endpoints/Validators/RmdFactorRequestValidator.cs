using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validators;

/// <summary>
/// Validator for RMD factor requests.
/// </summary>
public sealed class RmdsFactorRequestValidator : AbstractValidator<RmdsFactorRequest>
{
    public RmdsFactorRequestValidator()
    {
        RuleFor(x => x.Age)
            .InclusiveBetween((byte)73, (byte)120)
            .WithMessage("Age must be between 73 and 120.");

        RuleFor(x => x.Factor)
            .GreaterThan(0)
            .WithMessage("Factor must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Factor must be less than or equal to 100 (IRS factors are typically under 30).");
    }
}
