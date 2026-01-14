using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Data.Cli;

/**
 * This class rebuilds the min PAY_PROFIT year's enrollment and zerocontribution flags.
 * This class is a service which is started at application start up time.
 * It checks to see if the lowest payprofit year has no enrollments, which indicates it is missing data.
 * if it is missing this data, it initiates a process to recreate the ZeroContribution and Enrollment data for the year.
 */
internal sealed class RebuildEnrollmentAndZeroContService
{
    private readonly ILogger<RebuildEnrollmentAndZeroContService> _logger;
    private readonly IPayProfitUpdateService _payProfitUpdateService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IYearEndService _yearEndService;

    public RebuildEnrollmentAndZeroContService(
        ILogger<RebuildEnrollmentAndZeroContService> logger,
        IPayProfitUpdateService payProfitUpdateService,
        IYearEndService yearEndService,
        IProfitSharingDataContextFactory dataContextFactory
    )
    {
        _logger = logger;
        _payProfitUpdateService = payProfitUpdateService;
        _dataContextFactory = dataContextFactory;
        _yearEndService = yearEndService;
    }

    internal async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Check if rebuild is actually needed
            short profitYear = await IsRebuildNeededAsync(stoppingToken).ConfigureAwait(false);

            if (profitYear == 0)
            {
                _logger.LogInformation("Rebuild is not needed - this is typical.");
                return;
            }

            _logger.LogInformation("Starting RebuildEnrollmentAndZeroCont service for profit year {ProfitYear}", profitYear);

            // Rebuilds the ZeroContr
            await _yearEndService.RunFinalYearEndUpdates(profitYear, true, stoppingToken).ConfigureAwait(false);
            // Rebuilds the Enrollment Ids
            await _payProfitUpdateService.SetEnrollmentId(profitYear, stoppingToken).ConfigureAwait(false);

            _logger.LogInformation("RebuildEnrollmentAndZeroCont completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RebuildEnrollmentAndZeroCont execution");
            // Re-throw with context
            throw new InvalidOperationException("RebuildEnrollmentAndZeroCont failed during execution", ex);
        }
    }

    private Task<short> IsRebuildNeededAsync(CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var minProfitYear = await ctx.PayProfits.MinAsync(x => x.ProfitYear, cancellationToken);

            // Count how many Demographics have VestingScheduleId set for members with PayProfit records in this year
            var vestingScheduleIdCount = await ctx.PayProfits
                .Where(pp => pp.ProfitYear == minProfitYear)
                .CountAsync(pp => pp.Demographic != null && pp.Demographic.VestingScheduleId.HasValue, cancellationToken);

            // Return the year if rebuild needed (no VestingScheduleId set), otherwise 0
            return vestingScheduleIdCount == 0 ? minProfitYear : (short)0;
        }, cancellationToken);
    }
}
