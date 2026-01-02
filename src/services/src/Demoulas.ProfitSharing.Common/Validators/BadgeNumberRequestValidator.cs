using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validator for badge numbers. Badge numbers must be between 2 and 11 digits (10 to 99,999,999,999).
/// </summary>
/// <typeparam name="T">The request type that implements IBadgeNumberRequest</typeparam>
public sealed class BadgeNumberRequestValidator<T> : AbstractValidator<T> where T : IBadgeNumberRequest
{
    public BadgeNumberRequestValidator()
    {
        // Badge Number: 2-11 digits (10 to 2,147,483,647)
        RuleFor(x => x.BadgeNumber)
            .InclusiveBetween(10, int.MaxValue)
            .WithMessage("BadgeNumber must be between {0} and {1}.");
    }
}
