using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

public class MilitaryContributionRequestValidator : Validator<CreateMilitaryContributionRequest>
{
    private readonly IEmployeeLookupService _employeeLookup;
    private readonly IMilitaryService _militaryService;

    public MilitaryContributionRequestValidator(IEmployeeLookupService employeeLookup, IMilitaryService militaryService)
    {
        _employeeLookup = employeeLookup;
        _militaryService = militaryService;
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
            .MustAsync((request, _, ct) => ValidateNotBeforeHire(request, ct))
            .WithMessage(request => $"Contribution year {request.ContributionDate.Year} is before the employee's earliest known hire year.")
            .MustAsync((request, _, ct) => ValidateContributionDate(request, ct))
            .WithMessage(request => $"Regular Contribution already recorded for year {request.ContributionDate.Year}. Duplicates are not allowed.");
    }

    private async Task<bool> ValidateContributionDate(CreateMilitaryContributionRequest req, CancellationToken token)
    {
        if (req.IsSupplementalContribution)
        {
            return true;
        }

        var results = await _militaryService.GetMilitaryServiceRecordAsync(new MilitaryContributionRequest { ProfitYear = req.ProfitYear, BadgeNumber = req.BadgeNumber, Take = short.MaxValue },
            isArchiveRequest: false,
            cancellationToken: token);

        if (!results.IsSuccess) { return false; }
        var records = results.Value!.Results;

        return records.All(x => x.IsSupplementalContribution || x.ContributionDate.Year != req.ContributionDate.Year);
    }

    private async Task<bool> ValidateNotBeforeHire(CreateMilitaryContributionRequest req, CancellationToken token)
    {
        // If badge is invalid, let the BadgeNumber rule handle it; return false to fail validation here if hire date cannot be determined.
        var earliest = await _employeeLookup.GetEarliestHireDateAsync(req.BadgeNumber, token);
        if (!earliest.HasValue)
        {
            return false;
        }

        // Contribution year must be >= earliest hire year
        return req.ContributionDate.Year >= earliest.Value.Year;
    }
}
