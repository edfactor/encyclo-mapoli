using System.Diagnostics.CodeAnalysis;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

/**
 * This class rebuilds the min PAY_PROFIT year's enrollment and zerocontribution flags.
 * This class is a service which is started at application start up time.
 * It checks to see if the lowest payprofit year has no enrollments, which indicates it is missing data.
 * if it is missing this data, it initiates a process to recreate the ZeroContribution and Enrollment data for the year.
 */

public class RebuildEnrollmentAndZeroContService : BackgroundService
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Add a small delay to allow the application to fully start
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken).ConfigureAwait(false);

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
    
    private async Task<short> IsRebuildNeededAsync(CancellationToken cancellationToken)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var enrollmentSum = await ctx.PayProfits
                .Where(p => p.ProfitYear == ctx.PayProfits.Min(x => x.ProfitYear))
                .SumAsync(p => p.EnrollmentId, cancellationToken);

            // Return the year if sum indicates rebuild needed, otherwise 0
            return enrollmentSum == 0 ? ctx.PayProfits.Min(x => x.ProfitYear) : (short)0;
        }).ConfigureAwait(false);
    }

}
