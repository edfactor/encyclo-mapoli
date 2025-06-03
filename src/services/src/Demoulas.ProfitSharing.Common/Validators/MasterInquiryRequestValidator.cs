using Demoulas.ProfitSharing.Common.Contracts.Request.MasterInquiry;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class MasterInquiryRequestValidator : AbstractValidator<MasterInquiryRequest>
{
    public MasterInquiryRequestValidator()
    {
        short minYear = (short)ReferenceData.DsmMinValue.Year;
        RuleFor(x => x)
            .Must(x => (x.ProfitYear > minYear) || x.EndProfitYear > minYear)
            .WithMessage($"Either ProfitYear or EndProfitYear must be set and greater than {minYear}.");
    }
}
