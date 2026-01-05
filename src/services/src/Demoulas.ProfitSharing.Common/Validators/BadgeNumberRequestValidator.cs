using Demoulas.ProfitSharing.Common.Interfaces;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validator for badge numbers. Badge numbers must be between 1 and 10 digits (10 to 2,147,483,647).
/// </summary>
/// <typeparam name="T">The request type that implements IBadgeNumberRequest</typeparam>
public sealed class BadgeNumberRequestValidator<T> : AbstractValidator<T> where T : IBadgeNumberRequest
{
    public BadgeNumberRequestValidator()
    {
        // Badge Number: 1-10 digits (10 to 2,147,483,647)
        RuleFor(x => x.BadgeNumber)
            .InclusiveBetween(1, int.MaxValue)
            .WithMessage("BadgeNumber must be between {0} and {1}.");
    }
}
