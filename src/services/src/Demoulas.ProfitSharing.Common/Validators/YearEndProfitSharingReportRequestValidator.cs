using Demoulas.ProfitSharing.Common.Contracts.Request;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class YearEndProfitSharingReportRequestValidator : Validator<YearEndProfitSharingReportRequest>
{
    public YearEndProfitSharingReportRequestValidator()
    {
        RuleFor(x => x.ProfitYear)
            .GreaterThan((short)ReferenceData.DsmMinValue.Year).WithMessage($"ProfitYear must be after {ReferenceData.DsmMinValue.Year}.");

        RuleFor(x => x.ReportId)
            .IsInEnum().WithMessage("ReportId must be a valid report type.");

        RuleFor(x => x.BadgeNumber)
            .GreaterThan(0).When(x => x.BadgeNumber.HasValue)
            .WithMessage("BadgeNumber must be greater than 0 if provided.");
    }
}
