using System.ComponentModel;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.UnitTests.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Demoulas.ProfitSharing.UnitTests.Services;

[Collection("SharedGlobalState")]
[Description("PS-0000 : Unit tests for ProfitSharingAdjustmentsService (008-22 parity rules)")]
public sealed class ProfitSharingAdjustmentsServiceTests : ApiTestBase<Api.Program>
{
    private readonly IProfitSharingAdjustmentsService _service;

    public ProfitSharingAdjustmentsServiceTests()
    {
        _service = ServiceProvider?.GetRequiredService<IProfitSharingAdjustmentsService>()!;
    }

    [Fact]
    [Description("PS-0000 : GetAsync returns real rows only (no padding), up to 18")]
    public async Task GetAsync_ShouldReturnRealRowsOnly_UpTo18()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();
        // Default GetAsync now returns rows only when the member is under 21 as of today.
        await SetDemographicDobForAgeAsOfTodayAsync(candidate.Ssn, yearsOldAsOfToday: 20);

        var result = await _service.GetAdjustmentsAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
        }, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        result.Value!.Rows.Count.ShouldBeGreaterThan(0);
        result.Value.Rows.Count.ShouldBeLessThanOrEqualTo(18);

        result.Value.Rows.ShouldAllBe(r => r.ProfitDetailId != null);
        result.Value.Rows.ShouldAllBe(r => r.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id);
        result.Value.Rows.Select(r => r.RowNumber).ShouldBe(Enumerable.Range(1, result.Value.Rows.Count));
    }

    [Fact]
    [Description("PS-0000 : SaveAsync rejects amount edits on existing rows")]
    public async Task SaveAsync_WhenExistingRowAmountChanged_ShouldReturnValidationFailure()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();
        // Default GetAsync now returns rows only when the member is under 21 as of today.
        await SetDemographicDobForAgeAsOfTodayAsync(candidate.Ssn, yearsOldAsOfToday: 20);

        var getResult = await _service.GetAdjustmentsAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
        }, CancellationToken.None);

        getResult.IsSuccess.ShouldBeTrue();
        getResult.Value.ShouldNotBeNull();

        var existingRow = getResult.Value!.Rows
            .FirstOrDefault(r => r.ProfitDetailId != null);

        existingRow.ShouldNotBeNull("Test data did not contain an existing row.");

        var saveResult = await _service.SaveAdjustmentsAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            Rows = new[]
            {
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = existingRow!.ProfitDetailId,
                    RowNumber = existingRow.RowNumber,
                    ProfitCodeId = existingRow.ProfitCodeId,
                    Contribution = existingRow.Contribution + 1,
                    Earnings = existingRow.Earnings,
                    Forfeiture = existingRow.Forfeiture,
                    ActivityDate = existingRow.ActivityDate,
                    Comment = existingRow.Comment
                }
            }
        }, CancellationToken.None);

        saveResult.IsSuccess.ShouldBeFalse();
        saveResult.Error.ShouldNotBeNull();
        saveResult.Error!.ValidationErrors.ShouldNotBeNull();
        saveResult.Error.ValidationErrors!.ShouldContainKey(nameof(ProfitSharingAdjustmentRowRequest.RowNumber));
        saveResult.Error.ValidationErrors[nameof(ProfitSharingAdjustmentRowRequest.RowNumber)][0]
            .ShouldContain("Amount fields cannot be changed");
    }

    [Fact]
    [Description("PS-0000 : SaveAsync rejects multiple insert rows in one save")]
    public async Task SaveAsync_WhenMultipleInsertRows_ShouldReturnValidationFailure()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();
        // Ensure age-based guard does not trigger first; focus this test on insert-row validation.
        await SetDemographicDobForAgeAsOfTodayAsync(candidate.Ssn, yearsOldAsOfToday: 20);

        var saveResult = await _service.SaveAdjustmentsAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            Rows = new[]
            {
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null,
                    RowNumber = 1,
                    ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                    Contribution = 1,
                    Earnings = 0,
                    Forfeiture = 0,
                    ActivityDate = DateOnly.FromDateTime(DateTime.Today),
                    Comment = "ADMINISTRATIVE"
                },
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null,
                    RowNumber = 2,
                    ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                    Contribution = 2,
                    Earnings = 0,
                    Forfeiture = 0,
                    ActivityDate = null,
                    Comment = string.Empty
                }
            }
        }, CancellationToken.None);

        saveResult.IsSuccess.ShouldBeFalse();
        saveResult.Error.ShouldNotBeNull();
        saveResult.Error!.ValidationErrors.ShouldNotBeNull();
        saveResult.Error.ValidationErrors!.ShouldContainKey(nameof(SaveProfitSharingAdjustmentsRequest.Rows));
        saveResult.Error.ValidationErrors[nameof(SaveProfitSharingAdjustmentsRequest.Rows)][0]
            .ShouldContain("Only one new adjustment row");
    }

    [Fact]
    [Description("PS-0000 : SaveAsync rejects save when member is not under 21 as of today")]
    public async Task SaveAsync_WhenNotUnder21AsOfToday_ShouldReturnValidationFailure()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();
        await SetDemographicDobForAgeAsOfTodayAsync(candidate.Ssn, yearsOldAsOfToday: 22);

        var saveResult = await _service.SaveAdjustmentsAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            Rows =
            [
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null,
                    RowNumber = 1,
                    ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                    Contribution = 0,
                    Earnings = 0,
                    Forfeiture = 0,
                    ActivityDate = null,
                    Comment = string.Empty
                }
            ]
        }, CancellationToken.None);

        saveResult.IsSuccess.ShouldBeFalse();
        saveResult.Error.ShouldNotBeNull();
        saveResult.Error!.ValidationErrors.ShouldNotBeNull();
        saveResult.Error.ValidationErrors!.ShouldContainKey(nameof(SaveProfitSharingAdjustmentsRequest.ProfitYear));
        saveResult.Error.ValidationErrors[nameof(SaveProfitSharingAdjustmentsRequest.ProfitYear)][0]
            .ShouldContain("under 21 as of today");
    }

    [Fact]
    [Description("PS-0000 : GetAsync returns zero rows by default when member is not under 21 as of today")]
    public async Task GetAsync_WhenNotUnder21AsOfTodayAndGetAllRowsFalse_ShouldReturnZeroRows()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();
        await SetDemographicDobForAgeAsOfTodayAsync(candidate.Ssn, yearsOldAsOfToday: 22);

        var result = await _service.GetAdjustmentsAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            GetAllRows = false
        }, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Rows.Count.ShouldBe(0);
    }

    [Fact]
    [Description("PS-2266 : SaveAsync rejects adjustment when source profit detail has already been reversed")]
    public async Task SaveAsync_WhenSourceAlreadyReversed_ShouldReturnValidationFailure()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();
        await SetDemographicDobForAgeAsOfTodayAsync(candidate.Ssn, yearsOldAsOfToday: 20);

        // Get an existing row to use as the "source" for reversal
        var getResult = await _service.GetAdjustmentsAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
        }, CancellationToken.None);

        getResult.IsSuccess.ShouldBeTrue();
        var sourceRow = getResult.Value!.Rows.FirstOrDefault(r => r.ProfitDetailId != null);
        sourceRow.ShouldNotBeNull("Test data did not contain an existing row to reverse.");

        // First adjustment: should succeed
        var firstSave = await _service.SaveAdjustmentsAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            Rows =
            [
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null, // Insert row
                    RowNumber = 1,
                    ProfitCodeId = sourceRow.ProfitCodeId,
                    Contribution = -sourceRow.Contribution,
                    Earnings = -sourceRow.Earnings,
                    Forfeiture = -sourceRow.Forfeiture,
                    ActivityDate = DateOnly.FromDateTime(DateTime.Today),
                    Comment = "ADMINISTRATIVE",
                    ReversedFromProfitDetailId = sourceRow.ProfitDetailId // Link to source
                }
            ]
        }, CancellationToken.None);

        firstSave.IsSuccess.ShouldBeTrue("First adjustment should succeed.");

        // Second adjustment: should fail with double-reversal error
        var secondSave = await _service.SaveAdjustmentsAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            Rows =
            [
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null, // Insert row
                    RowNumber = 1,
                    ProfitCodeId = sourceRow.ProfitCodeId,
                    Contribution = -sourceRow.Contribution,
                    Earnings = -sourceRow.Earnings,
                    Forfeiture = -sourceRow.Forfeiture,
                    ActivityDate = DateOnly.FromDateTime(DateTime.Today),
                    Comment = "ADMINISTRATIVE",
                    ReversedFromProfitDetailId = sourceRow.ProfitDetailId // Same source as first
                }
            ]
        }, CancellationToken.None);

        secondSave.IsSuccess.ShouldBeFalse("Second adjustment to same source should fail.");
        secondSave.Error.ShouldNotBeNull();
        secondSave.Error!.ValidationErrors.ShouldNotBeNull();
        secondSave.Error.ValidationErrors!.ShouldContainKey(nameof(ProfitSharingAdjustmentRowRequest.ReversedFromProfitDetailId));
        secondSave.Error.ValidationErrors[nameof(ProfitSharingAdjustmentRowRequest.ReversedFromProfitDetailId)][0]
            .ShouldContain("already been reversed");
    }

    private Task<(short ProfitYear, int BadgeNumber, int Ssn)> FindCandidateWithLessThanMaxRowsAsync()
    {
        return MockDbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var profitCodeId = ProfitCode.Constants.IncomingContributions.Id;

            var candidates = await (
                from pd in ctx.ProfitDetails
                join d in ctx.Demographics on pd.Ssn equals d.Ssn
                where pd.ProfitCodeId == profitCodeId
                where pd.ProfitYearIteration == 0
                where d.BadgeNumber > 0
                group pd by new { pd.ProfitYear, d.BadgeNumber, pd.Ssn } into g
                select new
                {
                    g.Key.ProfitYear,
                    g.Key.BadgeNumber,
                    g.Key.Ssn,
                    Count = g.Count()
                })
                .Where(x => x.Count > 0 && x.Count < 18)
                .OrderBy(x => x.Count)
                .ToListAsync();

            var candidate = candidates.FirstOrDefault();
            candidate.ShouldNotBeNull("Test data did not contain a ProfitDetail group with < 18 rows for ProfitCodeId 0 and a matching Demographic.");

            return (candidate!.ProfitYear, candidate.BadgeNumber, candidate.Ssn);
        }, CancellationToken.None);
    }

    private Task SetDemographicDobForAgeAsOfTodayAsync(int ssn, int yearsOldAsOfToday)
    {
        return MockDbContextFactory.UseWritableContext(async ctx =>
        {
            var demographic = await ctx.Demographics.FirstOrDefaultAsync(d => d.Ssn == ssn);
            demographic.ShouldNotBeNull("Test data did not contain a Demographic row for the selected SSN.");

            // Use Jan 1 to avoid birthday edge cases.
            var birthYear = DateTime.Today.Year - yearsOldAsOfToday;
            demographic!.DateOfBirth = new DateOnly(birthYear, 1, 1);
        }, CancellationToken.None);
    }
}
