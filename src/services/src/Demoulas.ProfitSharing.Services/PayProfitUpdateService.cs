using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class PayProfitUpdateService : IPayProfitUpdateService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;

    public PayProfitUpdateService(IProfitSharingDataContextFactory dataContextFactory, ILoggerFactory loggerFactory)
    {
        _dataContextFactory = dataContextFactory;
        _logger = loggerFactory.CreateLogger<PayProfitUpdateService>();
    }
    public async Task SetZeroContributionReason(IQueryable<PayProfit> records, byte zeroContributionReasonId, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("Beginning Set Zero Contribution Reason to {0}", zeroContributionReasonId))
        {
            await records.ExecuteUpdateAsync(x => x.SetProperty(pp => pp.ZeroContributionReasonId, zeroContributionReasonId), cancellationToken);
        }
    }

    public async Task SetEnrollmentId(short profitYear, CancellationToken ct)
    {
        using (_logger.BeginScope("Setting EnrollmentId for ProfitYear {0}", profitYear))
        {
            _ = await _dataContextFactory.UseWritableContext(async ctx => {
                
                //Update to 2 if there is an enrollment record for the year
                await ctx.PayProfits.Include(x=>x.Demographic).Where(
                    x => x.ProfitYear == profitYear && ctx.ProfitDetails.Any(pd=>pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions && x.Demographic!.Ssn == pd.Ssn && pd.ProfitYear == profitYear)
                ).ExecuteUpdateAsync(x => x.SetProperty(pp => pp.EnrollmentId, Enrollment.Constants.NewVestingPlanHasContributions), ct);


                //Update to 4 if there is forfeiture, and no enrollment for the year.
                await ctx.PayProfits.Include(x => x.Demographic).Where(
                    x => x.ProfitYear == profitYear && 
                    ctx.ProfitDetails.Any(pd =>
                        pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures && 
                        x.Demographic!.Ssn == pd.Ssn &&
                        pd.ProfitYear == profitYear &&
                        pd.Forfeiture > 0
                    )  &&
                    !ctx.ProfitDetails.Any(pd =>
                        pd.ProfitYear == profitYear &&
                        pd.Ssn == x.Demographic!.Ssn && 
                        pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions
                    )
                ).ExecuteUpdateAsync(x => x.SetProperty(pp => pp.EnrollmentId, Enrollment.Constants.NewVestingPlanHasForfeitureRecords), ct);
                return true;
            }, ct);
        }
    }
}
