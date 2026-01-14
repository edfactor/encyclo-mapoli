using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.OracleHcm.Validators;

public class NameItemValidator : Validator<NameItem>
{
    public NameItemValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(84).WithMessage("FullName cannot exceed 84 characters.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(84).WithMessage("DisplayName cannot exceed 84 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName cannot be empty")
            .MaximumLength(30)
            .WithMessage("LastName cannot exceed 30 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("FirstName cannot be empty")
            .MaximumLength(30)
            .WithMessage("FirstName cannot exceed 30 characters.");
    }
}
