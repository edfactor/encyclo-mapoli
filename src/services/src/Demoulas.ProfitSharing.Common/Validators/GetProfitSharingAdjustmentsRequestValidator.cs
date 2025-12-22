using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// FluentValidation validator for GetProfitSharingAdjustmentsRequest.
/// </summary>
public sealed class GetProfitSharingAdjustmentsRequestValidator : Validator<GetProfitSharingAdjustmentsRequest>
{
    public GetProfitSharingAdjustmentsRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .Must(y => y is >= 1900 and <= 2100)
            .WithMessage("ProfitYear must be between 1900 and 2100.");

        RuleFor(x => x.BadgeNumber)
            .GreaterThan(0)
            .WithMessage("BadgeNumber must be greater than zero.");
    }
}
