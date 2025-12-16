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
    [Description("PS-0000 : GetAsync pads to 18 rows and includes insert row defaults")]
    public async Task GetAsync_ShouldReturn18Rows_WithInsertRowDefaults()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();

        var result = await _service.GetAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            SequenceNumber = candidate.SequenceNumber
        }, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value!.Rows.Count.ShouldBe(18);

        var insertRow = result.Value.Rows.FirstOrDefault(r => r.ProfitDetailId is null && r.ActivityDate is not null);
        insertRow.ShouldNotBeNull();

        insertRow!.ProfitYearIteration.ShouldBe((byte)3);
        insertRow.ProfitCodeId.ShouldBe(ProfitCode.Constants.IncomingContributions.Id);
        insertRow.Comment.ShouldBe("ADMINISTRATIVE");
        insertRow.ActivityDate.ShouldBe(DateOnly.FromDateTime(DateTime.Today));
        insertRow.IsEditable.ShouldBeFalse();

        foreach (var fillerRow in result.Value.Rows
            .Where(r => r.ProfitDetailId is null)
            .Where(r => r.RowNumber != insertRow.RowNumber))
        {
            fillerRow.ActivityDate.ShouldBeNull();
            fillerRow.Comment.ShouldBe(string.Empty);
            fillerRow.Contribution.ShouldBe(0);
            fillerRow.Earnings.ShouldBe(0);
            fillerRow.Forfeiture.ShouldBe(0);
        }
    }

    [Fact]
    [Description("PS-0000 : SaveAsync rejects amount edits on existing rows")]
    public async Task SaveAsync_WhenExistingRowAmountChanged_ShouldReturnValidationFailure()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();

        var getResult = await _service.GetAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            SequenceNumber = candidate.SequenceNumber
        }, CancellationToken.None);

        getResult.IsSuccess.ShouldBeTrue();
        getResult.Value.ShouldNotBeNull();

        var existingRow = getResult.Value!.Rows
            .Where(r => r.ProfitDetailId is not null)
            .FirstOrDefault(r => r.ProfitYearIteration is 0 or 3);

        existingRow.ShouldNotBeNull("Test data did not contain an existing row with ProfitYearIteration (EXT) 0 or 3.");

        var saveResult = await _service.SaveAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            SequenceNumber = candidate.SequenceNumber,
            Rows = new[]
            {
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = existingRow!.ProfitDetailId,
                    RowNumber = existingRow.RowNumber,
                    ProfitYearIteration = existingRow.ProfitYearIteration,
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
            .ShouldContain("Amount fields are read-only");
    }

    [Fact]
    [Description("PS-0000 : SaveAsync rejects multiple insert rows in one save")]
    public async Task SaveAsync_WhenMultipleInsertRows_ShouldReturnValidationFailure()
    {
        var candidate = await FindCandidateWithLessThanMaxRowsAsync();

        var getResult = await _service.GetAsync(new GetProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            SequenceNumber = candidate.SequenceNumber
        }, CancellationToken.None);

        getResult.IsSuccess.ShouldBeTrue();
        getResult.Value.ShouldNotBeNull();

        var insertRow = getResult.Value!.Rows.First(r => r.ProfitDetailId is null && r.ActivityDate is not null);
        var fillerRow = getResult.Value.Rows.First(r => r.ProfitDetailId is null && r.ActivityDate is null);

        var saveResult = await _service.SaveAsync(new SaveProfitSharingAdjustmentsRequest
        {
            ProfitYear = candidate.ProfitYear,
            BadgeNumber = candidate.BadgeNumber,
            SequenceNumber = candidate.SequenceNumber,
            Rows = new[]
            {
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null,
                    RowNumber = insertRow.RowNumber,
                    ProfitYearIteration = 3,
                    ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                    Contribution = 1,
                    Earnings = 0,
                    Forfeiture = 0,
                    ActivityDate = insertRow.ActivityDate,
                    Comment = insertRow.Comment
                },
                new ProfitSharingAdjustmentRowRequest
                {
                    ProfitDetailId = null,
                    RowNumber = fillerRow.RowNumber,
                    ProfitYearIteration = 3,
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

    private Task<(short ProfitYear, int BadgeNumber, int SequenceNumber)> FindCandidateWithLessThanMaxRowsAsync()
    {
        return MockDbContextFactory.UseReadOnlyContext(async ctx =>
        {
            var profitCodeId = ProfitCode.Constants.IncomingContributions.Id;

            var candidates = await (
                from pd in ctx.ProfitDetails
                join d in ctx.Demographics on pd.Ssn equals d.Ssn
                where pd.ProfitCodeId == profitCodeId
                where pd.ProfitYearIteration == 0 || pd.ProfitYearIteration == 3
                where d.BadgeNumber > 0
                group pd by new { pd.ProfitYear, d.BadgeNumber, pd.DistributionSequence } into g
                select new
                {
                    g.Key.ProfitYear,
                    g.Key.BadgeNumber,
                    SequenceNumber = g.Key.DistributionSequence,
                    Count = g.Count()
                })
                .Where(x => x.Count > 0 && x.Count < 18)
                .OrderBy(x => x.Count)
                .ToListAsync();

            var candidate = candidates.FirstOrDefault();
            candidate.ShouldNotBeNull("Test data did not contain a ProfitDetail group with < 18 rows for ProfitCodeId 0, ProfitYearIteration 0/3, and a matching Demographic.");

            return (candidate!.ProfitYear, candidate.BadgeNumber, candidate.SequenceNumber);
        }, CancellationToken.None);
    }
}
