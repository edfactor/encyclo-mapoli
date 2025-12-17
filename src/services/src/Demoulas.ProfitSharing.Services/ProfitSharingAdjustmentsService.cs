using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Contracts.Request.ProfitDetails;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Request.Audit;
using Demoulas.ProfitSharing.Common.Contracts.Response.ProfitDetails;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Interfaces.Audit;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Audit;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class ProfitSharingAdjustmentsService : IProfitSharingAdjustmentsService
{
    private const int MaxRows = 18;
    private const string AdministrativeComment = "ADMINISTRATIVE";

    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ITotalService _totalService;
    private readonly IAuditService _auditService;
    private readonly ILogger<ProfitSharingAdjustmentsService> _logger;

    public ProfitSharingAdjustmentsService(
        IProfitSharingDataContextFactory dbContextFactory,
        IDemographicReaderService demographicReaderService,
        ITotalService totalService,
        IAuditService auditService,
        ILogger<ProfitSharingAdjustmentsService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
        _auditService = auditService;
        _logger = logger;
    }

    public Task<Result<GetProfitSharingAdjustmentsResponse>> GetAsync(GetProfitSharingAdjustmentsRequest request, CancellationToken ct)
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
            var canEditYearExtension = IsUnderAgeAtContributionYearEnd(demographic.DateOfBirth, profitYear, underAgeThreshold: 18);
            var isOver21AtInitialHire = IsOverAgeAtDate(demographic.DateOfBirth, demographic.HireDate, ageThreshold: 21);

            var balances = await _totalService.GetVestingBalanceForMembersAsync(SearchBy.Ssn, new HashSet<int> { ssn }, profitYear, ct);
            var balance = balances.FirstOrDefault();
            var currentBalance = balance?.CurrentBalance ?? 0m;
            var vestedBalance = balance?.VestedBalance ?? 0m;

            var includeRowsForYear = includeAllRows || IsUnderAgeAtContributionYearEnd(demographic.DateOfBirth, profitYear, underAgeThreshold: 22);

            var profitDetails = includeRowsForYear
                ? await ctx.ProfitDetails
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
                            : null,
                        Comment = pd.Remark != null ? pd.Remark : string.Empty,
                        // For 008-22 parity: only the EXT (ProfitYearIteration) field is editable, and only
                        // when the account holder is under 18 as of Dec 31 of the contribution year.
                        IsEditable = canEditYearExtension
                    })
                    .ToListAsync(ct)
                : new List<ProfitSharingAdjustmentRowResponse>();

            for (var i = 0; i < profitDetails.Count; i++)
            {
                profitDetails[i] = profitDetails[i] with { RowNumber = i + 1 };
            }

            var responseRows = new List<ProfitSharingAdjustmentRowResponse>(MaxRows);
            responseRows.AddRange(profitDetails);

            while (responseRows.Count < MaxRows)
            {
                var rowNumber = responseRows.Count + 1;

                // For 008-22 parity, we expose exactly one "new row" for inserts.
                // Only amount fields are intended to be editable on that row; the UI enforces that.
                // We keep additional filler rows non-editable and blank.
                bool isInsertRow = rowNumber == profitDetails.Count + 1;

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
                    ActivityDate = isInsertRow ? DateOnly.FromDateTime(DateTime.Today) : null,
                    Comment = isInsertRow ? AdministrativeComment : string.Empty,
                    // For the insert row: EXT is not editable.
                    IsEditable = false
                });
            }

            return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
            {
                ProfitYear = request.ProfitYear,
                DemographicId = demographicId,
                IsOver21AtInitialHire = isOver21AtInitialHire,
                CurrentBalance = currentBalance,
                VestedBalance = vestedBalance,
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
                    .Select(d => new { d.Id, d.Ssn, d.DateOfBirth, d.HireDate })
                    .FirstOrDefaultAsync(ct);

                if (demographic is null)
                {
                    return Result<GetProfitSharingAdjustmentsResponse>.Failure(Error.EmployeeNotFound);
                }

                var ssn = demographic.Ssn;
                var demographicId = demographic.Id;
                var profitYear = request.ProfitYear;

                // New requirement: block save if member is over 22 as of Dec 31 of the adjusted year.
                // Defense-in-depth: enforced here even though request validation also checks.
                var ageAtYearEnd = profitYear - (short)demographic.DateOfBirth.Year;
                if (ageAtYearEnd > 22)
                {
                    return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                    {
                        [nameof(request.ProfitYear)] = ["Member age must be 22 or younger for the selected ProfitYear."]
                    });
                }

                var canEditYearExtension = IsUnderAgeAtContributionYearEnd(demographic.DateOfBirth, profitYear, underAgeThreshold: 18);
                var isOver21AtInitialHire = IsOverAgeAtDate(demographic.DateOfBirth, demographic.HireDate, ageThreshold: 21);

                var balances = await _totalService.GetVestingBalanceForMembersAsync(SearchBy.Ssn, new HashSet<int> { ssn }, profitYear, ct);
                var balance = balances.FirstOrDefault();
                var currentBalance = balance?.CurrentBalance ?? 0m;
                var vestedBalance = balance?.VestedBalance ?? 0m;

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

                var nowUtc = DateTimeOffset.UtcNow;
                var extUpdates = new List<(int ProfitDetailId, byte OriginalValue, byte NewValue)>();

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

                    // 008-22 parity: For existing rows, only EXT (ProfitYearIteration) may be updated.
                    if (row.Contribution != pd.Contribution || row.Earnings != pd.Earnings || row.Forfeiture != pd.Forfeiture)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                                $"Row {row.RowNumber} can only update EXT (ProfitYearIteration). Amount fields are read-only for existing rows."
                            ]
                        });
                    }

                    var existingActivityDate = GetActivityDateOrNull(pd);
                    if (row.ActivityDate != existingActivityDate)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                                $"Row {row.RowNumber} can only update EXT (ProfitYearIteration). ActivityDate is read-only for existing rows."
                            ]
                        });
                    }

                    var existingComment = pd.Remark != null ? pd.Remark : string.Empty;
                    if (!string.Equals(row.Comment ?? string.Empty, existingComment, StringComparison.Ordinal))
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.RowNumber)] = [
                                $"Row {row.RowNumber} can only update EXT (ProfitYearIteration). Comment is read-only for existing rows."
                            ]
                        });
                    }

                    if (!canEditYearExtension && row.ProfitYearIteration != pd.ProfitYearIteration)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitYearIteration)] = [
                                "Year extension (EXT) is only editable when the account holder is under 18 as of Dec 31 of the contribution year."
                            ]
                        });
                    }

                    if (row.ProfitYearIteration != pd.ProfitYearIteration)
                    {
                        extUpdates.Add((pd.Id, pd.ProfitYearIteration, row.ProfitYearIteration));
                    }

                    pd.ProfitYearIteration = row.ProfitYearIteration;

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

                ProfitDetail? insertedProfitDetail = null;

                foreach (var row in insertCandidates)
                {
                    if (row.ProfitYearIteration != 3)
                    {
                        return Result<GetProfitSharingAdjustmentsResponse>.ValidationFailure(new Dictionary<string, string[]>
                        {
                            [nameof(ProfitSharingAdjustmentRowRequest.ProfitYearIteration)] = ["New rows must have EXT (ProfitYearIteration) = 3."]
                        });
                    }

                    var activityDate = DateOnly.FromDateTime(DateTime.Today);

                    var newProfitDetail = new ProfitDetail
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
                        Remark = AdministrativeComment,
                        FederalTaxes = 0,
                        StateTaxes = 0,
                        YearsOfServiceCredit = 0
                    };

                    ctx.ProfitDetails.Add(newProfitDetail);

                    // At most one insert is allowed; keep reference for auditing after SaveChanges.
                    insertedProfitDetail = newProfitDetail;
                }

                await ctx.SaveChangesAsync(ct);

                // Audit updates (EXT changes) and optional insert.
                foreach (var update in extUpdates)
                {
                    await _auditService.LogDataChangeAsync(
                        operationName: "Update Profit Sharing Adjustment",
                        tableName: "PROFIT_DETAIL",
                        auditOperation: AuditEvent.AuditOperations.Update,
                        primaryKey: $"Id:{update.ProfitDetailId}",
                        changes:
                        [
                            new AuditChangeEntryInput
                            {
                                ColumnName = "PROFIT_YEAR_ITERATION",
                                OriginalValue = update.OriginalValue.ToString(),
                                NewValue = update.NewValue.ToString(),
                            },
                        ],
                        cancellationToken: ct);
                }

                if (insertedProfitDetail is not null)
                {
                    await _auditService.LogDataChangeAsync(
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
                        ],
                        cancellationToken: ct);
                }

                return Result<GetProfitSharingAdjustmentsResponse>.Success(new GetProfitSharingAdjustmentsResponse
                {
                    ProfitYear = request.ProfitYear,
                    DemographicId = demographicId,
                    IsOver21AtInitialHire = isOver21AtInitialHire,
                    CurrentBalance = currentBalance,
                    VestedBalance = vestedBalance,
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

    private static bool IsUnderAgeAtContributionYearEnd(DateOnly dateOfBirth, short profitYear, int underAgeThreshold)
    {
        var asOf = new DateOnly(profitYear, 12, 31);

        var age = asOf.Year - dateOfBirth.Year;
        if (dateOfBirth > asOf.AddYears(-age))
        {
            age--;
        }

        return age < underAgeThreshold;
    }

    private static bool IsOverAgeAtDate(DateOnly dateOfBirth, DateOnly asOf, int ageThreshold)
    {
        var age = asOf.Year - dateOfBirth.Year;
        if (dateOfBirth > asOf.AddYears(-age))
        {
            age--;
        }

        return age > ageThreshold;
    }

    private static DateOnly? GetActivityDateOrNull(ProfitDetail pd)
    {
        return pd.MonthToDate > 0 && pd.YearToDate > 0
            ? new DateOnly(pd.YearToDate, pd.MonthToDate, 1)
            : null;
    }
}
