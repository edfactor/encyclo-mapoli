using Demoulas.ProfitSharing.Common.Contracts.Request.CheckRun;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation;

/// <summary>
/// Validates CheckRunStartRequest to ensure all required fields are present and valid.
/// </summary>
public sealed class CheckRunStartRequestValidator : AbstractValidator<CheckRunStartRequest>
{
    public CheckRunStartRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .InclusiveBetween(2000, 2100)
            .WithMessage("ProfitYear must be between 2000 and 2100.");

        RuleFor(x => x.CheckRunDate)
            .NotEmpty()
            .WithMessage("CheckRunDate is required.")
            .Must(date => date >= new DateOnly(2000, 1, 1) && date <= new DateOnly(2100, 12, 31))
            .WithMessage("CheckRunDate must be between 2000-01-01 and 2100-12-31.");

        RuleFor(x => x.CheckNumber)
            .GreaterThan(0)
            .WithMessage("CheckNumber must be a positive integer.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}
