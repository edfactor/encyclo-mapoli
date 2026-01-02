using Demoulas.ProfitSharing.Common.Contracts.Request;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validator for employee badge numbers. Employee badge numbers must be between 2 and 11 digits (10 to 99,999,999,999).
/// </summary>
/// <typeparam name="T">The request type that implements IEmployeeBadgeNumberRequest</typeparam>
public sealed class EmployeeBadgeNumberRequestValidator<T> : AbstractValidator<T> where T : IEmployeeBadgeNumberRequest
{
    public EmployeeBadgeNumberRequestValidator()
    {
        // Employee Badge Number: 2-11 digits (10 to 99,999,999,999)
        RuleFor(x => x.EmployeeBadgeNumber)
            .InclusiveBetween(10, 99_999_999_999)
            .WithMessage("EmployeeBadgeNumber must be between 2 and 11 digits.");
    }
}
