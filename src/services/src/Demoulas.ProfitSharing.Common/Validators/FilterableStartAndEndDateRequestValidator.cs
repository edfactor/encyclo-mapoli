using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validates FilterableStartAndEndDateRequest including date ranges, pagination, and optional vested balance filters.
/// Enforces that vested balance value and operator are provided together.
/// </summary>
public class FilterableStartAndEndDateRequestValidator : AbstractValidator<FilterableStartAndEndDateRequest>
{
    public FilterableStartAndEndDateRequestValidator()
    {
        // Include base date and pagination validation
        Include(new StartAndEndDateRequestValidator());

        // Vested balance value validation
        RuleFor(x => x.VestedBalanceValue)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.VestedBalanceValue.HasValue)
            .WithMessage("Vested balance value must be greater than or equal to zero.");

        // Vested balance operator validation
        RuleFor(x => x.VestedBalanceOperator)
            .IsInEnum()
            .When(x => x.VestedBalanceOperator.HasValue)
            .WithMessage("Vested balance operator must be a valid comparison operator (Equals, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual).");

        // Require both value and operator together
        RuleFor(x => x.VestedBalanceValue)
            .NotNull()
            .When(x => x.VestedBalanceOperator.HasValue)
            .WithMessage("Vested balance value is required when vested balance operator is provided.");

        RuleFor(x => x.VestedBalanceOperator)
            .NotNull()
            .When(x => x.VestedBalanceValue.HasValue)
            .WithMessage("Vested balance operator is required when vested balance value is provided.");
    }
}
