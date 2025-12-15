using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using FluentValidation;

namespace Demoulas.ProfitSharing.Endpoints.Validation;

public sealed class GetProfitSharingAdjustmentsRequestValidator : AbstractValidator<GetProfitSharingAdjustmentsRequest>
{
    public GetProfitSharingAdjustmentsRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .InclusiveBetween(1900, 2500)
            .WithMessage("ProfitYear must be between 1900 and 2500.");

        RuleFor(x => x.OracleHcmId)
            .GreaterThan(0)
            .WithMessage("OracleHcmId must be greater than zero.");

        RuleFor(x => x.SequenceNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SequenceNumber must be zero or greater.");
    }
}
