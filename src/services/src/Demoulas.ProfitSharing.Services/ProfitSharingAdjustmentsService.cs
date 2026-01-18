using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class ProfitSharingAdjustmentsService : IProfitSharingAdjustmentsService
{
    private const int MaxRows = 18;
    private const string AdministrativeComment = "ADMINISTRATIVE";
    private const byte AdministrativeProfitYearIteration = 3;
    private const int AdministrativeDistributionSequence = 0;

    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IProfitSharingAuditService _profitSharingAuditService;
    private readonly ILogger<ProfitSharingAdjustmentsService> _logger;
    private readonly TimeProvider _timeProvider;

    public ProfitSharingAdjustmentsService(
        IProfitSharingDataContextFactory dbContextFactory,
        IDemographicReaderService demographicReaderService,
        IProfitSharingAuditService profitSharingAuditService,
        ILogger<ProfitSharingAdjustmentsService> logger,
        TimeProvider? timeProvider = null)
    {
        _dbContextFactory = dbContextFactory;
        _demographicReaderService = demographicReaderService;
        _profitSharingAuditService = profitSharingAuditService;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Task<Result<GetProfitSharingAdjustmentsResponse>> GetAdjustmentsAsync(GetProfitSharingAdjustmentsRequest request, CancellationToken ct)
    {
        return _dbContextFactory.UseReadOnlyContext(async ctx =>
        {
            if (request.ProfitYear is < 1900 or > 2100)
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

            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: false);

            var demographic = await demographicQuery
                .TagWith($"ProfitSharingAdjustments-Get-Demographic-{request.BadgeNumber}")
                .Where(d => d.BadgeNumber == request.BadgeNumber)
                .Select(d => new { d.Id, d.Ssn, d.DateOfBirth, d.HireDate })
                .FirstOrDefaultAsync(ct);

            if (demographic is null)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.EmployeeNotFound);
            }

            var ssn = demographic.Ssn;
            var demographicId = demographic.Id;
            var profitYear = request.ProfitYear;
            var includeAllRows = request.GetAllRows;
            var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

            // Confluence rule: default to only show rows for accounts under 21 as of today.
            // Users can override via GetAllRows.
            var includeRowsForYear = includeAllRows || IsUnderAgeAtDate(demographic.DateOfBirth, today, underAgeThreshold: 21);

            byte[] paymentProfitCodes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

            if (!includeRowsForYear)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
                {
                    ProfitYear = request.ProfitYear,
                    DemographicId = demographicId,
                    BadgeNumber = request.BadgeNumber,
                    Rows = new List<ProfitSharingAdjustmentRowResponse>()
                });
            }

            var profitDetails = includeRowsForYear
                ? await ctx.ProfitDetails
                    .TagWith($"ProfitSharingAdjustments-Get-ProfitDetails-AllYears")
                    .Where(pd =>
                        pd.Ssn == ssn &&
                        pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id)
                    .OrderByDescending(pd => pd.ProfitYear)
                    .ThenByDescending(pd => pd.CreatedAtUtc)
                    .Take(MaxRows)
                    .Select(pd => new ProfitSharingAdjustmentRowResponse
                    {
                        ProfitDetailId = pd.Id,
                        HasBeenReversed = false, // Will be updated after lookup
                        RowNumber = 0, // assigned after materialization
                        ProfitYear = pd.ProfitYear,
                        ProfitYearIteration = pd.ProfitYearIteration,
                        ProfitCodeId = pd.ProfitCodeId,
                        ProfitCodeName = pd.ProfitCode != null ? pd.ProfitCode.Name : string.Empty,
                        Contribution = pd.Contribution,
                        Earnings = pd.Earnings,
                        Forfeiture = !paymentProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0,
                        Payment = paymentProfitCodes.Contains(pd.ProfitCodeId) ? pd.Forfeiture : 0,
                        FederalTaxes = pd.FederalTaxes,
                        StateTaxes = pd.StateTaxes,
                        TaxCodeId = pd.TaxCodeId != null ? pd.TaxCodeId.Value : TaxCode.Constants.Unknown.Id,
                        ActivityDate = pd.MonthToDate > 0 && pd.YearToDate > 0
                            ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1)
                            : null,
                        Comment = pd.Remark != null ? pd.Remark : string.Empty,
                        IsEditable = false
                    })
                    .ToListAsync(ct)
                : new List<ProfitSharingAdjustmentRowResponse>();

            // Determine which profit details have already been reversed
            // (i.e., another profit detail has ReversedFromProfitDetailId pointing to them)
            var profitDetailIds = profitDetails
                .Where(pd => pd.ProfitDetailId.HasValue)
                .Select(pd => pd.ProfitDetailId!.Value)
                .ToList();

            var alreadyReversedIds = profitDetailIds.Count > 0
                ? await ctx.ProfitDetails
                    .TagWith($"ProfitSharingAdjustments-CheckReversed-{ssn}")
                    .Where(pd => pd.ReversedFromProfitDetailId != null && profitDetailIds.Contains(pd.ReversedFromProfitDetailId.Value))
                    .Select(pd => pd.ReversedFromProfitDetailId!.Value)
                    .Distinct()
                    .ToListAsync(ct)
                : [];

            var alreadyReversedSet = alreadyReversedIds.ToHashSet();

            for (var i = 0; i < profitDetails.Count; i++)
            {
                var hasBeenReversed = profitDetails[i].ProfitDetailId.HasValue
                    && alreadyReversedSet.Contains(profitDetails[i].ProfitDetailId!.Value);

                profitDetails[i] = profitDetails[i] with
                {
                    RowNumber = i + 1,
                    HasBeenReversed = hasBeenReversed
                };
            }

            return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
            {
                ProfitYear = request.ProfitYear,
                DemographicId = demographicId,
                BadgeNumber = request.BadgeNumber,
                Rows = profitDetails
            });
        }, ct);
    }

    public async Task<Result<GetProfitSharingAdjustmentsResponse>> SaveAdjustmentsAsync(SaveProfitSharingAdjustmentsRequest request, CancellationToken ct)
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
            if (row.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id)
            {
                return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                {
                    [nameof(ProfitSharingAdjustmentRowRequest.ProfitCodeId)] = ["Only ProfitCodeId 0 (Incoming contributions) is supported on this screen."]
                });
            }

            if (row is { Contribution: 0, Earnings: 0, Forfeiture: 0, ProfitDetailId: null })
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
                    .Select(d => new { d.Id, d.Ssn, d.DateOfBirth, d.HireDate })
                    .FirstOrDefaultAsync(ct);

                if (demographic is null)
                {
                    return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.EmployeeNotFound);
                }

                var ssn = demographic.Ssn;
                var demographicId = demographic.Id;
                var profitYear = request.ProfitYear;

                var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

                // Confluence rule: under-21 adjustment eligibility is based on age as of today.
                // Defense-in-depth: enforce here even if the UI filters/labels.
                if (!IsUnderAgeAtDate(demographic.DateOfBirth, today, underAgeThreshold: 21))
                {
                    return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                    {
                        [nameof(request.ProfitYear)] = ["Member must be under 21 as of today to use this screen."]
                    });
                }

                var idsToUpdate = request.Rows
                    .Where(r => r.ProfitDetailId.HasValue)
                    .Select(r => r.ProfitDetailId!.Value)
                    .Distinct()
                    .ToArray();

                List<ProfitDetail> existing = idsToUpdate.Length == 0
                    ? new List<ProfitDetail>()
                    : await ctx.ProfitDetails
                        .TagWith($"ProfitSharingAdjustments-Save-LoadExisting-{profitYear}")
                        .Where(pd => idsToUpdate.Contains(pd.Id))
                        .ToListAsync(ct);

                var existingById = existing.ToDictionary(pd => pd.Id);

                var nowUtc = DateTimeOffset.UtcNow;

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

                    if (pd.Ssn != ssn || pd.ProfitYear != profitYear)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitDetailId)] = [$"ProfitDetailId {id} does not belong to the requested employee/year."]
                        });
                    }

                    if (pd.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitCodeId)] = [$"ProfitDetailId {id} is not ProfitCodeId 0."]
                        });
                    }

                    // Existing rows are read-only in this workflow.
                    if (row.Contribution != pd.Contribution || row.Earnings != pd.Earnings || row.Forfeiture != pd.Forfeiture)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                                $"Row {row.RowNumber} is read-only. Amount fields cannot be changed for existing rows."
                            ]
                        });
                    }

                    var existingActivityDate = GetActivityDateOrNull(pd);
                    if (row.ActivityDate != existingActivityDate)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                                $"Row {row.RowNumber} is read-only. ActivityDate cannot be changed for existing rows."
                            ]
                        });
                    }

                    var existingComment = pd.Remark != null ? pd.Remark : string.Empty;
                    if (!string.Equals(row.Comment ?? string.Empty, existingComment, StringComparison.Ordinal))
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                                $"Row {row.RowNumber} is read-only. Comment cannot be changed for existing rows."
                            ]
                        });
                    }

                    pd.ModifiedAtUtc = nowUtc;
                }

                var insertCandidates = request.Rows
                    .Where(r => r.ProfitDetailId is null)
                    .Where(r => r.Contribution != 0 || r.Earnings != 0 || r.Forfeiture != 0)
                    .ToList();

                // 008-22 parity: only one "new row" insert is allowed per save.
                if (insertCandidates.Count > 1)
                {
                    return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                    {
                        [nameof(request.Rows)] = ["Only one new adjustment row may be inserted per save."]
                    });
                }

                // Double-reversal protection: check if any source profit details have already been reversed.
                var sourceIdsToReverse = insertCandidates
                    .Where(r => r.ReversedFromProfitDetailId.HasValue)
                    .Select(r => r.ReversedFromProfitDetailId!.Value)
                    .Distinct()
                    .ToList();

                if (sourceIdsToReverse.Count > 0)
                {
                    var alreadyReversedIds = await ctx.ProfitDetails
                        .TagWith($"ProfitSharingAdjustments-CheckDoubleReversal-{profitYear}")
                        .Where(pd => pd.ReversedFromProfitDetailId != null && sourceIdsToReverse.Contains(pd.ReversedFromProfitDetailId.Value))
                        .Select(pd => pd.ReversedFromProfitDetailId!.Value)
                        .Distinct()
                        .ToListAsync(ct);

                    if (alreadyReversedIds.Count > 0)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ReversedFromProfitDetailId)] = [
                                $"The following profit detail IDs have already been reversed and cannot be reversed again: {string.Join(", ", alreadyReversedIds)}"
                            ]
                        });
                    }
                }

                ProfitDetail? insertedProfitDetail = null;

                foreach (var row in insertCandidates)
                {
                    var activityDate = today;

                    var newProfitDetail = new ProfitDetail
                    {
                        Ssn = ssn,
                        ProfitYear = profitYear,
                        ProfitYearIteration = AdministrativeProfitYearIteration,
                        DistributionSequence = AdministrativeDistributionSequence,
                        ProfitCodeId = ProfitCode.Constants.IncomingContributions.Id,
                        Contribution = row.Contribution,
                        Earnings = row.Earnings,
                        Forfeiture = row.Forfeiture,
                        MonthToDate = (byte)activityDate.Month,
                        YearToDate = (short)activityDate.Year,
                        Remark = AdministrativeComment,
                        FederalTaxes = 0,
                        StateTaxes = 0,
                        YearsOfServiceCredit = 0,
                        ReversedFromProfitDetailId = row.ReversedFromProfitDetailId // Track which record was reversed
                    };

                    ctx.ProfitDetails.Add(newProfitDetail);

                    // At most one insert is allowed; keep reference for auditing after SaveChanges.
                    insertedProfitDetail = newProfitDetail;
                }

                await ctx.SaveChangesAsync(ct);

                // Audit optional insert.

                if (insertedProfitDetail is not null)
                {
                    await _profitSharingAuditService.LogDataChangeAsync(
                        operationName: "Create Profit Sharing Adjustment",
                        tableName: "PROFIT_DETAIL",
                        auditOperation: AuditEvent.AuditOperations.Create,
                        primaryKey: $"Id:{insertedProfitDetail.Id}",
                        changes:
                        [
                            new AuditChangeEntryInput { ColumnName = "PROFIT_YEAR", NewValue = insertedProfitDetail.ProfitYear.ToString() },
                            new AuditChangeEntryInput { ColumnName = "DISTRIBUTION_SEQUENCE", NewValue = insertedProfitDetail.DistributionSequence.ToString() },
                            new AuditChangeEntryInput { ColumnName = "PROFIT_CODE_ID", NewValue = insertedProfitDetail.ProfitCodeId.ToString() },
                            new AuditChangeEntryInput { ColumnName = "PROFIT_YEAR_ITERATION", NewValue = insertedProfitDetail.ProfitYearIteration.ToString() },
                            new AuditChangeEntryInput { ColumnName = "CONTRIBUTION", NewValue = insertedProfitDetail.Contribution.ToString() },
                            new AuditChangeEntryInput { ColumnName = "EARNINGS", NewValue = insertedProfitDetail.Earnings.ToString() },
                            new AuditChangeEntryInput { ColumnName = "FORFEITURE", NewValue = insertedProfitDetail.Forfeiture.ToString() },
                            new AuditChangeEntryInput { ColumnName = "MONTH_TO_DATE", NewValue = insertedProfitDetail.MonthToDate.ToString() },
                            new AuditChangeEntryInput { ColumnName = "YEAR_TO_DATE", NewValue = insertedProfitDetail.YearToDate.ToString() },
                            new AuditChangeEntryInput { ColumnName = "REMARK", NewValue = insertedProfitDetail.Remark },
                            new AuditChangeEntryInput { ColumnName = "REVERSED_FROM_PROFIT_DETAIL_ID", NewValue = insertedProfitDetail.ReversedFromProfitDetailId?.ToString() },
                        ],
                        cancellationToken: ct);
                }

                return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
                {
                    ProfitYear = request.ProfitYear,
                    DemographicId = demographicId,
                    BadgeNumber = request.BadgeNumber,
                    Rows = []
                });
            }, ct);

            if (!result.IsSuccess)
            {
                return result;
            }

            return await GetAdjustmentsAsync(new GetProfitSharingAdjustmentsRequest
            {
                ProfitYear = request.ProfitYear,
                BadgeNumber = request.BadgeNumber
            }, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save profit sharing adjustments for BadgeNumber {BadgeNumber} year {ProfitYear}",
                request.BadgeNumber, request.ProfitYear);

            return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.Unexpected("Failed to save profit sharing adjustments."));
        }
    }

    private static bool IsUnderAgeAtDate(DateOnly dateOfBirth, DateOnly asOf, int underAgeThreshold)
    {
        var age = asOf.Year - dateOfBirth.Year;
        if (dateOfBirth > asOf.AddYears(-age))
        {
            age--;
        }

        return age < underAgeThreshold;
    }

    private static DateOnly? GetActivityDateOrNull(ProfitDetail pd)
    {
        return pd.MonthToDate > 0 && pd.YearToDate > 0
            ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1)
            : null;
    }
}
