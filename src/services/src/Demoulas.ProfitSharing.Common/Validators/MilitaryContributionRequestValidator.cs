using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.Util.Extensions;
using FastEndpoints;
using FluentValidation;
using System.Diagnostics.Metrics;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validates requests to create Military Contributions according to business rules.
/// </summary>
/// <remarks>
/// Documentation: Military Contributions validation rules, temporal semantics, and QA plan are documented here:
/// https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/537919492/Military+Contributions+Validation+Behavior+and+Operations
/// </remarks>
public class MilitaryContributionRequestValidator : Validator<CreateMilitaryContributionRequest>
{
    private readonly IEmployeeLookupService _employeeLookup;
    private readonly IMilitaryService _militaryService;
    private static readonly Meter s_meter = new("Demoulas.ProfitSharing.Validators");
    private static readonly Counter<long> s_validationFailures = s_meter.CreateCounter<long>(
        "ps_validation_failures_total",
        description: "Counts of validation failures by validator and rule");

    public MilitaryContributionRequestValidator(IEmployeeLookupService employeeLookup, IMilitaryService militaryService)
    {
        _employeeLookup = employeeLookup;
        _militaryService = militaryService;
        RuleFor(r => r.ContributionAmount)
            .GreaterThan(0)
            .WithMessage($"The {nameof(CreateMilitaryContributionRequest.ContributionAmount)} must be greater than zero.");

        RuleFor(r => r.ProfitYear)
            .GreaterThanOrEqualTo((short)2020)
            .WithMessage($"{nameof(CreateMilitaryContributionRequest.ProfitYear)} must not less than 2020.")
            .LessThanOrEqualTo((short)DateTime.Today.Year)
            .WithMessage($"{nameof(CreateMilitaryContributionRequest.ProfitYear)} must not be greater than this year.");

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
            .MustAsync((request, _, ct) => ValidateAtLeast21OnContribution(request, ct))
            .WithMessage(request => $"Employee must be at least 21 years old on the contribution date {request.ContributionDate:yyyy-MM-dd}.")
            .MustAsync((request, _, ct) => ValidateContributionDate(request, ct))
            .WithMessage(request => $"Regular Contribution already recorded for year {request.ContributionDate.Year}. Duplicates are not allowed.");

        // YOS requires supplemental when posting for a different profit year than the contribution date year
        RuleFor(r => r.IsSupplementalContribution)
            .Must((request, isSupp) =>
            {
                if (request.ProfitYear == (short)request.ContributionDate.Year)
                {
                    return true; // same year, either regular or supplemental allowed subject to other rules
                }
                // Different posting year than contribution date year must be supplemental (no YOS credit)
                return isSupp || TrackFailure("YosRequiresSupplementalWhenYearMismatch");
            })
            .WithMessage(r => $"When profit year ({r.ProfitYear}) differs from contribution year ({r.ContributionDate.Year}), the contribution must be marked Supplemental.");

        // Employment status eligibility: only Active as-of contribution date
        RuleFor(r => r.ContributionDate)
            .MustAsync(async (request, _, ct) =>
            {
                // If badge is invalid, let the BadgeNumber rule handle it; avoid compounding errors
                if (!await _employeeLookup.BadgeExistsAsync(request.BadgeNumber, ct))
                {
                    return true;
                }
                var isActive = await _employeeLookup.IsActiveAsOfAsync(request.BadgeNumber, DateOnly.FromDateTime(request.ContributionDate), ct);
                if (!isActive.HasValue)
                {
                    return TrackFailure("EmploymentStatusMissing");
                }
                if (!isActive.Value)
                {
                    return TrackFailure("EmploymentStatusNotActive");
                }
                return true; // Active only
            })
            .WithMessage(request => $"Employee employment status is not eligible for contributions as of {DateOnly.FromDateTime(request.ContributionDate):yyyy-MM-dd}.");
    }

    private async Task<bool> ValidateContributionDate(CreateMilitaryContributionRequest req, CancellationToken token)
    {
        if (req.IsSupplementalContribution)
        {
            return true;
        }

        // Query by the contribution date year rather than the selected ProfitYear so we detect
        // existing records for the actual contribution year (fixes PS-1721 where users may select
        // a different profit year in the UI than the contribution date year).
        var results = await _militaryService.GetMilitaryServiceRecordAsync(new GetMilitaryContributionRequest { BadgeNumber = req.BadgeNumber, Take = short.MaxValue },
            isArchiveRequest: false,
            cancellationToken: token);

        if (!results.IsSuccess)
        {
            return TrackFailure("ServiceError");
        }
        var records = results.Value!.Results.Where(x=> x.ProfitYear == (short)req.ContributionDate.Year);

        var ok = records.All(x => x.IsSupplementalContribution || x.ContributionDate.Year != req.ContributionDate.Year);
        return ok || TrackFailure("DuplicateRegularContribution");
    }

    private async Task<bool> ValidateNotBeforeHire(CreateMilitaryContributionRequest req, CancellationToken token)
    {
        // If badge is invalid, let the BadgeNumber rule handle it; return false to fail validation here if hire date cannot be determined.
        var earliest = await _employeeLookup.GetEarliestHireDateAsync(req.BadgeNumber, token);
        if (!earliest.HasValue)
        {
            return TrackFailure("HireDateMissing");
        }

        // Contribution year must be >= earliest hire year
        return req.ContributionDate.Year >= earliest.Value.Year || TrackFailure("BeforeHireYear");
    }

    private async Task<bool> ValidateAtLeast21OnContribution(CreateMilitaryContributionRequest req, CancellationToken token)
    {
        var dob = await _employeeLookup.GetDateOfBirthAsync(req.BadgeNumber, token);
        if (!dob.HasValue)
        {
            // If DOB is not available, fail validation so the issue can be surfaced.
            return TrackFailure("DobMissing");
        }

        return dob.Value.Age(req.ContributionDate) >= 21 || TrackFailure("AgeUnder21");
    }

    private static bool TrackFailure(string rule)
    {
        s_validationFailures.Add(1,
            new KeyValuePair<string, object?>("validator", "MilitaryContribution"),
            new KeyValuePair<string, object?>("rule", rule));
        return false;
    }
}
