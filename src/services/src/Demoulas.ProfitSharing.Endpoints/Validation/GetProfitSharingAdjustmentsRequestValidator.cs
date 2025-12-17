using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation.Legacy;

public sealed class LegacyGetProfitSharingAdjustmentsRequestValidator : AbstractValidator<GetProfitSharingAdjustmentsRequest>
{
    public LegacyGetProfitSharingAdjustmentsRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .Must(y => y is >= 1900 and <= 2100)
            .WithMessage("ProfitYear must be between 1900 and 2100.");

        RuleFor(x => x.BadgeNumber)
            .GreaterThan(0)
            .WithMessage("BadgeNumber must be greater than zero.");

        RuleFor(x => x.SequenceNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SequenceNumber must be zero or greater.");
    }
}
