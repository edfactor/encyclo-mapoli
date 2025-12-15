using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation;

public sealed class GetProfitSharingAdjustmentsRequestValidator : AbstractValidator<GetProfitSharingAdjustmentsRequest>
{
    public GetProfitSharingAdjustmentsRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .Must(y => y is >= 1900 and <= 2100)
            .WithMessage("ProfitYear must be between 1900 and 2100.");

        RuleFor(x => x.DemographicId)
            .GreaterThan(0)
            .WithMessage("DemographicId must be greater than zero.");

        RuleFor(x => x.SequenceNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SequenceNumber must be zero or greater.");
    }
}
