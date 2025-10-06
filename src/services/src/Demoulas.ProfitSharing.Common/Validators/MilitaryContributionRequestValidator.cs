using System.Diagnostics.Metrics;
using Demoulas.ProfitSharing.Common.Contracts.Request.Military;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.Util.Extensions;
using FastEndpoints;
using FluentValidation;

namespace Demoulas.ProfitSharing.Common.Validators;

/// <summary>
/// Validates requests to create Military Contributions according to business rules.
/// </summary>
/// <remarks>
/// <para><strong>Business Rules Summary (from TPR008-13.cbl):</strong></para>
/// <list type="number">
/// <item><description><strong>5-Year Lookback Window</strong>: Contributions allowed for current year OR up to 5 years prior (Proj 19567, expanded from 3 years on 12/31/2013)</description></item>
/// <item><description><strong>Age Requirement</strong>: Employee must be at least 21 years old on contribution date (eligibility rule)</description></item>
/// <item><description><strong>Hire Date Validation</strong>: Contribution year must be on or after employee's earliest hire year</description></item>
/// <item><description><strong>Payroll Frequency Restriction (COBOL only)</strong>: Monthly employees (PY-FREQ=2) required special user authorization in COBOL (line 735-743). C# implementation simplifies this to general employment status validation.</description></item>
/// <item><description><strong>Duplicate Prevention</strong>: No duplicate regular contributions allowed for same year (line 1530-1547). Supplemental contributions bypass this check.</description></item>
/// <item><description><strong>Year/Date Consistency</strong>: When profit year differs from contribution date year, must be marked supplemental (no YOS credit)</description></item>
/// <item><description><strong>Amount Validation</strong>: Contribution amount must be greater than zero</description></item>
/// <item><description><strong>Date Format Validation</strong>: Month must be 1-12 or 20 (special code for year-only entry), year validated per lookback rule (line 1240-1253)</description></item>
/// <item><description><strong>Screen Duplicate Check</strong>: Same date cannot appear multiple times on entry screen (line 1270-1398)</description></item>
/// <item><description><strong>Original Entry Requirement</strong>: For month-specific entries (MMYY format), the year.0 record must exist first (line 1460-1489)</description></item>
/// </list>
/// <para><strong>COBOL Source References:</strong></para>
/// <list type="bullet">
/// <item><description>TPR008-13.cbl - Main military contribution entry program (MAIN-1286/MAIN-1280 Oracle HCM Integration)</description></item>
/// <item><description>Line 37-38: Proj 19567 - 5-year lookback expansion</description></item>
/// <item><description>Line 1256-1265: Year validation logic (410-CHECK-EACH)</description></item>
/// <item><description>Line 1270-1398: Duplicate date detection (420-CHECK-SCREEN-DATE, 432-CHECK-SCREEN-YR)</description></item>
/// <item><description>Line 1410-1557: Database duplicate checks (440-CHECK-FOR-DUP-PROFIT-DATES, 450/460-PROCESS-MONTH-CHECK)</description></item>
/// <item><description>Line 735-743: PY-FREQ check for monthly associates (PY-FREQ=2 = monthly paid, restricted to authorized users in COBOL; PY-FREQ=1 = weekly paid, no restriction)</description></item>
/// </list>
/// <para><strong>Documentation:</strong></para>
/// <para>Comprehensive validation rules, temporal semantics, and QA plan: https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/537919492</para>
/// <para>See also: MILITARY_CONTRIBUTION_VALIDATION.md in src/ui/public/docs/</para>
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
            .WithMessage($"{nameof(CreateMilitaryContributionRequest.ProfitYear)} must not be greater than this year.")
            .Must((request, profitYear) =>
            {
                // COBOL Rule: Contribution year must be current year OR within 5 years prior
                // Source: TPR008-13.cbl line 1256-1265 (Proj 19567 - expanded from 3 to 5 years)
                var currentYear = DateTime.Today.Year;
                var contributionYear = request.ContributionDate.Year;
                var isValid = contributionYear == currentYear ||
                             (contributionYear >= currentYear - 5 && contributionYear < currentYear);
                return isValid || TrackFailure("ContributionYearOutsideLookbackWindow");
            })
            .WithMessage(request => $"Contribution year {request.ContributionDate.Year} must be current year or within 5 years prior (allowed: {DateTime.Today.Year - 5} to {DateTime.Today.Year}).");

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
        var records = results.Value!.Results.Where(x => x.ProfitYear == (short)req.ContributionDate.Year);

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
