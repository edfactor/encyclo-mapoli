using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class MilitaryContributionRequestValidator : Validator<CreateMilitaryContributionRequest>
{
    public MilitaryContributionRequestValidator(IEmployeeLookupService employeeLookup)
    {
        RuleFor(r => r.ContributionAmount)
            .GreaterThan(0)
            .WithMessage($"The {nameof(CreateMilitaryContributionRequest.ContributionAmount)} must be greater than zero.");

        RuleFor(r => r.ProfitYear)
            .GreaterThanOrEqualTo((short)2020)
            .WithMessage($"{nameof(MilitaryContributionRequest.ProfitYear)} must not less than 2020.")
            .LessThanOrEqualTo((short)DateTime.Today.Year)
            .WithMessage($"{nameof(MilitaryContributionRequest.ProfitYear)} must not be greater than this year.");

        RuleFor(r => r.BadgeNumber)
            .GreaterThan(0)
            .WithMessage($"{nameof(CreateMilitaryContributionRequest.BadgeNumber)} must be greater than zero.")
            .MustAsync(async (badge, ct) => await employeeLookup.BadgeExistsAsync(badge, ct))
            .WithMessage("Employee with the given badge number was not found.");

        RuleFor(r => r.ContributionDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage($"{nameof(CreateMilitaryContributionRequest.ContributionDate)} cannot be in the future.")
            .MustAsync((request, _, ct) => ValidateContributionDate(request, ct))
            .WithMessage(request => $"Regular Contribution already recorded for year {request.ContributionDate.Year}. Duplicates are not allowed.");
    }

    private async Task<bool> ValidateContributionDate(CreateMilitaryContributionRequest req, CancellationToken token)
    {
        if (req.IsSupplementalContribution)
        {
            return true;
        }

        var ms = Resolve<IMilitaryService>();
        var results = await ms.GetMilitaryServiceRecordAsync(new MilitaryContributionRequest { ProfitYear = req.ProfitYear, BadgeNumber = req.BadgeNumber, Take = short.MaxValue },
            isArchiveRequest: false,
            cancellationToken: token);

        if (!results.IsSuccess) { return false; }
        var records = results.Value!.Results;

        return records.All(x => x.IsSupplementalContribution || x.ContributionDate.Year != req.ContributionDate.Year);
    }
}
