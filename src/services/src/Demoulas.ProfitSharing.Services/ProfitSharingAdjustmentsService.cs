using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class ProfitSharingAdjustmentsService : IProfitSharingAdjustmentsService
{
    private const int MaxRows = 18;

    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<ProfitSharingAdjustmentsService> _logger;

    public ProfitSharingAdjustmentsService(
        IProfitSharingDataContextFactory dbContextFactory,
        IDemographicReaderService demographicReaderService,
        ILogger<ProfitSharingAdjustmentsService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
    }

    public Task<Result<GetProfitSharingAdjustmentsResponse>> GetAsync(GetProfitSharingAdjustmentsRequest request, CancellationToken ct)
    {
        return _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            if (request.ProfitYear < 1900 || request.ProfitYear > 2500)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(request.ProfitYear)] = ["ProfitYear must be a valid year."]
                });
            }

            if (request.BadgeNumber <= 0)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(request.BadgeNumber)] = ["BadgeNumber must be greater than zero."]
                });
            }

            if (request.SequenceNumber < 0)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(request.SequenceNumber)] = ["SequenceNumber must be zero or greater."]
                });
            }

            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: false);

            var demographic = await demographicQuery
                .TagWith($"ProfitSharingAdjustments-Get-Demographic-{request.BadgeNumber}")
                .Where(d => d.BadgeNumber == request.BadgeNumber)
                .Select(d => new { d.Ssn })
                .FirstOrDefaultAsync(ct);

            if (demographic is null)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.EmployeeNotFound);
            }

            var ssn = demographic.Ssn;
            var profitYear = request.ProfitYear;

            var profitDetails = await ctx.ProfitDetails
                .TagWith($"ProfitSharingAdjustments-Get-ProfitDetails-{profitYear}-{request.SequenceNumber}")
                .Where(pd =>
                    pd.Ssn == ssn &&
                    pd.ProfitYear == profitYear &&
                    pd.DistributionSequence == request.SequenceNumber &&
                    pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id)
                .OrderBy(pd => pd.ProfitYearIteration)
                .ThenBy(pd => pd.CreatedAtUtc)
                .Take(MaxRows)
                .Select(pd => new ProfitSharingAdjustmentRowResponse
                {
                    ProfitDetailId = pd.Id,
                    RowNumber = 0, // assigned after materialization
                    ProfitYear = pd.ProfitYear,
                    ProfitYearIteration = pd.ProfitYearIteration,
                    ProfitCodeId = pd.ProfitCodeId,
                    Contribution = pd.Contribution,
                    Earnings = pd.Earnings,
                    Forfeiture = pd.Forfeiture,
                    ActivityDate = pd.MonthToDate > 0 && pd.YearToDate > 0
                        ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1)
                        : (DateOnly?)null,
                    Comment = pd.Remark != null ? pd.Remark : string.Empty,
                    IsEditable = true
                })
                .ToListAsync(ct);

            for (var i = 0; i < profitDetails.Count; i++)
            {
                profitDetails[i] = profitDetails[i] with { RowNumber = i + 1 };
            }

            var responseRows = new List<ProfitSharingAdjustmentRowResponse>(MaxRows);
            responseRows.AddRange(profitDetails);

            while (responseRows.Count < MaxRows)
            {
                var rowNumber = responseRows.Count + 1;

                responseRows.Add(new ProfitSharingAdjustmentRowResponse
                {
                    ProfitDetailId = null,
                    RowNumber = rowNumber,
                    ProfitYear = request.ProfitYear,
                    ProfitYearIteration = 3,
                    ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                    Contribution = 0,
                    Earnings = 0,
                    Forfeiture = 0,
                    ActivityDate = DateOnly.FromDateTime(DateTime.Today),
                    Comment = "ADMINISTRATIVE",
                    IsEditable = true
                });
            }

            return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
            {
                ProfitYear = request.ProfitYear,
                BadgeNumber = request.BadgeNumber,
                SequenceNumber = request.SequenceNumber,
                Rows = responseRows
            });
        }, ct);
    }

    public async Task<Result<GetProfitSharingAdjustmentsResponse>> SaveAsync(SaveProfitSharingAdjustmentsRequest request, CancellationToken ct)
    {
        if (request.Rows is null || request.Rows.Count == 0)
        {
            return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(request.Rows)] = ["Rows are required."]
            });
        }

        if (request.Rows.Count > MaxRows)
        {
            return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(request.Rows)] = [$"No more than {MaxRows} rows are allowed."]
            });
        }

        var duplicateRowNumbers = request.Rows
            .GroupBy(r => r.RowNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (duplicateRowNumbers.Length > 0)
        {
            return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                    $"Duplicate RowNumber values are not allowed: {string.Join(", ", duplicateRowNumbers)}"
                ]
            });
        }

        foreach (var row in request.Rows)
        {
            if (row.ProfitYearIteration is not (0 or 3))
            {
                return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(ProfitSharingAdjustmentRowRequest.ProfitYearIteration)] = ["ProfitYearIteration (EXT) must be 0 or 3."]
                });
            }

            if (row.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(ProfitSharingAdjustmentRowRequest.ProfitCodeId)] = ["Only ProfitCodeId 0 (Incoming contributions) is supported on this screen."]
                });
            }

            if (row.Contribution == 0 && row.Earnings == 0 && row.Forfeiture == 0 && row.ProfitDetailId is null)
            {
                // Allow empty insert rows.
                continue;
            }
        }

        try
        {
            var result = await _dbContextFactory.UseWritableContext(async ctx =>
            {
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: false);

                var demographic = await demographicQuery
                    .TagWith($"ProfitSharingAdjustments-Save-Demographic-{request.BadgeNumber}")
                    .Where(d => d.BadgeNumber == request.BadgeNumber)
                    .Select(d => new { d.Ssn })
                    .FirstOrDefaultAsync(ct);

                if (demographic is null)
                {
                    return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.EmployeeNotFound);
                }

                var ssn = demographic.Ssn;
                var profitYear = request.ProfitYear;

                var idsToUpdate = request.Rows
                    .Where(r => r.ProfitDetailId.HasValue)
                    .Select(r => r.ProfitDetailId!.Value)
                    .Distinct()
                    .ToArray();

                List<ProfitDetail> existing = idsToUpdate.Length == 0
                    ? new List<ProfitDetail>()
                    : await ctx.ProfitDetails
                        .TagWith($"ProfitSharingAdjustments-Save-LoadExisting-{profitYear}-{request.SequenceNumber}")
                        .Where(pd => idsToUpdate.Contains(pd.Id))
                        .ToListAsync(ct);

                var existingById = existing.ToDictionary(pd => pd.Id);

                foreach (var row in request.Rows.Where(r => r.ProfitDetailId.HasValue))
                {
                    var id = row.ProfitDetailId!.Value;

                    if (!existingById.TryGetValue(id, out var pd))
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitDetailId)] = [$"ProfitDetailId {id} was not found."]
                        });
                    }

                    if (pd.Ssn != ssn || pd.ProfitYear != profitYear || pd.DistributionSequence != request.SequenceNumber)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitDetailId)] = [$"ProfitDetailId {id} does not belong to the requested employee/year/sequence."]
                        });
                    }

                    if (pd.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitCodeId)] = [$"ProfitDetailId {id} is not ProfitCodeId 0."]
                        });
                    }

                    pd.ProfitYearIteration = row.ProfitYearIteration;
                    pd.Contribution = row.Contribution;
                    pd.Earnings = row.Earnings;
                    pd.Forfeiture = row.Forfeiture;
                    pd.Remark = row.Comment;

                    if (row.ActivityDate.HasValue)
                    {
                        pd.MonthToDate = (byte)row.ActivityDate.Value.Month;
                        pd.YearToDate = (short)row.ActivityDate.Value.Year;
                    }

                    pd.ModifiedAtUtc = DateTimeOffset.UtcNow;
                }

                var insertCandidates = request.Rows
                    .Where(r => r.ProfitDetailId is null)
                    .Where(r => r.Contribution != 0 || r.Earnings != 0 || r.Forfeiture != 0)
                    .ToList();

                foreach (var row in insertCandidates)
                {
                    var activityDate = row.ActivityDate ?? DateOnly.FromDateTime(DateTime.Today);

                    ctx.ProfitDetails.Add(new ProfitDetail
                    {
                        Ssn = ssn,
                        ProfitYear = profitYear,
                        ProfitYearIteration = row.ProfitYearIteration,
                        DistributionSequence = request.SequenceNumber,
                        ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                        Contribution = row.Contribution,
                        Earnings = row.Earnings,
                        Forfeiture = row.Forfeiture,
                        MonthToDate = (byte)activityDate.Month,
                        YearToDate = (short)activityDate.Year,
                        Remark = row.Comment,
                        FederalTaxes = 0,
                        StateTaxes = 0,
                        YearsOfServiceCredit = 0
                    });
                }

                await ctx.SaveChangesAsync(ct);

                return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
                {
                    ProfitYear = request.ProfitYear,
                    BadgeNumber = request.BadgeNumber,
                    SequenceNumber = request.SequenceNumber,
                    Rows = []
                });
            }, ct);

            if (!result.IsSuccess)
            {
                return result;
            }

            return await GetAsync(new GetProfitSharingAdjustmentsRequest
            {
                ProfitYear = request.ProfitYear,
                BadgeNumber = request.BadgeNumber,
                SequenceNumber = request.SequenceNumber
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save profit sharing adjustments for BadgeNumber {BadgeNumber} year {ProfitYear} seq {SequenceNumber}",
                request.BadgeNumber, request.ProfitYear, request.SequenceNumber);

            return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.Unexpected("Failed to save profit sharing adjustments."));
        }
    }
}
