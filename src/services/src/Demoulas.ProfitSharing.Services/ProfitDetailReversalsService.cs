using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Common.Contracts;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class ProfitDetailReversalsService : IProfitDetailReversalsService
{
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<ProfitDetailReversalsService> _logger;

    public ProfitDetailReversalsService(
        IProfitSharingDataContextFactory dbContextFactory,
        IDemographicReaderService demographicReaderService,
        ILogger<ProfitDetailReversalsService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
    }

    public async Task<Result<bool>> ReverseProfitDetailsAsync(int[] profitDetailIds, CancellationToken cancellationToken)
    {
        // Input validation
        if (profitDetailIds == null || profitDetailIds.Length == 0)
        {
            _logger.LogWarning("Attempted to reverse profit details with null or empty ID array");
            return Result<bool>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(profitDetailIds)] = ["Profit detail IDs cannot be null or empty"]
            });
        }

        // Guard against degenerate queries - limit batch size
        if (profitDetailIds.Length > 1000)
        {
            _logger.LogWarning("Attempted to reverse {Count} profit details, exceeding maximum batch size of 1000", profitDetailIds.Length);
            return Result<bool>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(profitDetailIds)] = ["Cannot reverse more than 1000 profit details in a single operation"]
            });
        }

        try
        {
            var result = await _dbContextFactory.UseWritableContext(async (ctx) =>
            {
                // Phase 1: Load all profit details and validate
                var profitDetails = await ctx.ProfitDetails
                    .Where(pd => profitDetailIds.Contains(pd.Id))
                    .ToListAsync(cancellationToken);

                // Check if we found all requested profit details
                var foundIds = profitDetails.Select(pd => pd.Id).ToHashSet();
                var missingIds = profitDetailIds.Where(id => !foundIds.Contains(id)).ToArray();

                if (missingIds.Length > 0)
                {
                    _logger.LogWarning("Profit details not found for IDs: [{MissingIds}]", string.Join(", ", missingIds));
                    return Result<bool>.ValidationFailure(new Dictionary<string, string[]>
                    {
                        ["profitDetailIds"] = [$"Profit details not found for IDs: {string.Join(", ", missingIds)}"]
                    });
                }

                // Get frozen year for validation
                var maxFrozenYear = await ctx.FrozenStates.MaxAsync(fy => (int?)fy.ProfitYear, cancellationToken) ?? 0;
                var currentDate = DateTime.Now;

                // Phase 2: Validate ALL records and collect issues
                var validationErrors = new Dictionary<string, List<string>>();

                foreach (var pd in profitDetails)
                {
                    var pdErrors = new List<string>();

                    // Check for non-reversible profit codes
                    if (pd.ProfitCodeId is 0 or 2 or 8)
                    {
                        pdErrors.Add($"Profit code {pd.ProfitCodeId} is not reversible");
                    }

                    // Check if profit year is frozen
                    if (pd.ProfitYear <= maxFrozenYear)
                    {
                        pdErrors.Add($"Cannot reverse profit detail for frozen year {pd.ProfitYear}");
                    }

                    // Check January month restrictions
                    if (currentDate.Month == 1 && pd.MonthToDate > 1 && pd.MonthToDate < 12)
                    {
                        pdErrors.Add($"In January, can only reverse months 1 or 12, not month {pd.MonthToDate}");
                    }

                    // Check if MonthToDate is too old
                    if (pd.MonthToDate < currentDate.Month - 2)
                    {
                        pdErrors.Add($"Cannot reverse profit detail from month {pd.MonthToDate} as it is more than 2 months old");
                    }

                    if (pdErrors.Count > 0)
                    {
                        validationErrors[$"profitDetail_{pd.Id}"] = pdErrors;
                    }
                }

                // If any validation errors, return them all without processing
                if (validationErrors.Count > 0)
                {
                    var flattenedErrors = validationErrors.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.ToArray()
                    );

                    _logger.LogWarning("Validation failed for {ErrorCount} profit details out of {TotalCount} requested",
                        validationErrors.Count, profitDetails.Count);

                    return Result<bool>.ValidationFailure(flattenedErrors);
                }

                // Phase 3: All validations passed, now process all records
                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
                var employeeSsns = await demographicQuery
                    .Where(d => profitDetails.Select(pd => pd.Ssn).Contains(d.Ssn))
                    .Select(d => d.Ssn)
                    .ToListAsync(cancellationToken);

                var reversedCount = 0;

                foreach (var pd in profitDetails)
                {
                    var state = pd.Remark?.Length >= 17 ? pd.Remark.Substring(15, 2) : "";

                    var reverseProfitDetail = new ProfitDetail
                    {
                        Ssn = pd.Ssn,
                        ProfitYear = pd.ProfitYear,
                        ProfitYearIteration = pd.ProfitYearIteration,
                        DistributionSequence = pd.DistributionSequence,
                        ProfitCodeId = pd.ProfitCodeId,
                        Contribution = pd.ProfitCodeId == 6 ? -pd.Contribution : 0,
                        Forfeiture = pd.ProfitCodeId is 1 or 3 or 5 or 9 ? -pd.Forfeiture : 0,
                        Earnings = 0,
                        FederalTaxes = -pd.FederalTaxes,
                        StateTaxes = -pd.StateTaxes,
                        MonthToDate = (byte)currentDate.Month,
                        YearToDate = (short)currentDate.Year,
                        YearsOfServiceCredit = (sbyte)-pd.YearsOfServiceCredit,
                        TaxCodeId = pd.TaxCodeId,
                        CommentTypeId = pd.Remark?.StartsWith("REV") == true ? CommentType.Constants.UndoReversal : CommentType.Constants.Reversal,
                        Remark = ((pd.Remark?.StartsWith("REV") == true
                            ? $"UN-REV {currentDate:MM/yy}  "
                            : $"REV    {currentDate:MM/yy}  ") + state).Trim(),

                        ZeroContributionReasonId = null,
                        CommentRelatedState = pd.CommentRelatedState
                    };

                    ctx.ProfitDetails.Add(reverseProfitDetail);
                    reversedCount++;

                    // Handle ETVA updates for profit codes 6 and 9
                    var employeeMember = employeeSsns.FirstOrDefault(x => x == pd.Ssn);
                    if (pd.ProfitCodeId is 6 or 9 && employeeMember != default)
                    {
                        var currentPayProfit = await ctx.PayProfits
                            .FirstOrDefaultAsync(pp => pp.DemographicId == employeeMember, cancellationToken);
                        if (currentPayProfit != null)
                        {
                            currentPayProfit.Etva += reverseProfitDetail.Contribution + reverseProfitDetail.Forfeiture;
                        }
                    }
                }

                await ctx.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully reversed {ReversedCount} profit details", reversedCount);

                return Result<bool>.Success(true);
            }, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reverse profit details for IDs: [{Ids}]", string.Join(", ", profitDetailIds));
            return Result<bool>.Failure(Error.Unexpected($"Failed to reverse profit details: {ex.Message}"));
        }
    }
}
