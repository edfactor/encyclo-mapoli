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
public sealed class ProfitDetailReversalsService: IProfitDetailReversalsService
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
            return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
            {
                [nameof(profitDetailIds)] = ["Profit detail IDs cannot be null or empty"]
            }));
        }

        // Guard against degenerate queries - limit batch size
        if (profitDetailIds.Length > 1000)
        {
            _logger.LogWarning("Attempted to reverse {Count} profit details, exceeding maximum batch size of 1000", profitDetailIds.Length);
            return Result<bool>.Failure(Error.Validation(new Dictionary<string, string[]>
            {
                [nameof(profitDetailIds)] = ["Cannot reverse more than 1000 profit details in a single operation"]
            }));
        }

        try
        {
            var result = await _dbContextFactory.UseWritableContext(async (ctx) =>
            {
                var profitDetails = await ctx.ProfitDetails
                    .Where(pd => profitDetailIds.Contains(pd.Id))
                    .ToListAsync(cancellationToken);

                // Validate that we found some profit details to reverse
                if (profitDetails.Count == 0)
                {
                    _logger.LogWarning("No profit details found for provided IDs: [{Ids}]", string.Join(", ", profitDetailIds));
                    return Result<bool>.Failure(Error.EntityNotFound("Profit details"));
                }

                var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, false);
                var employeeSsns = await demographicQuery
                    .Where(d => profitDetails.Select(pd => pd.Ssn).Contains(d.Ssn))
                    .Select(d => d.Ssn)
                    .ToListAsync(cancellationToken);
                
                var currentDate = DateTime.Now;
                var reversedCount = 0;
                var skippedCount = 0;
                
                foreach (var pd in profitDetails)
                {
                    // Skip non-reversible profit codes (0, 2, 8)
                    if (pd.ProfitCodeId is 0 or 2 or 8) {
                        skippedCount++;
                        continue;
                    }

                    var reverseProfitDetail = new Data.Entities.ProfitDetail
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
                        Remark = pd.Remark?.StartsWith("REV") == true 
                            ? $"UN-REV {currentDate:MM/yy}" 
                            : $"REV {currentDate:MM/yy}",
                        
                        ZeroContributionReasonId = null
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
                
                _logger.LogInformation("Successfully reversed {ReversedCount} profit details, skipped {SkippedCount} non-reversible entries", 
                    reversedCount, skippedCount);
                
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
