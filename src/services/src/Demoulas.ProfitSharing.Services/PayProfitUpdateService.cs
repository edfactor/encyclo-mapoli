using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;

public sealed class PayProfitUpdateService : IPayProfitUpdateService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ILogger _logger;
    private readonly TotalService _totalService;


    public PayProfitUpdateService(IProfitSharingDataContextFactory dataContextFactory, ILoggerFactory loggerFactory, ITotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _logger = loggerFactory.CreateLogger<PayProfitUpdateService>();
        _totalService = (TotalService)totalService;
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
            await _dataContextFactory.UseWritableContext(async ctx =>
            {
                Dictionary<int, PayProfit> allPayProfitBySsn =
                    await ctx.PayProfits.Where(pp => pp.ProfitYear == profitYear).Include(p => p.Demographic).ToDictionaryAsync(k => k.Demographic!.Ssn, v => v, ct);
                Dictionary<int, byte> yearsBySsn = await _totalService.GetYearsOfService(ctx, profitYear).ToDictionaryAsync(t => t.Ssn, t => t.Years, ct);

                // Is this likely to be too big for production?  Perhaps we should batch update?
                List<ProfitDetail> allProfitDetail = await ctx.ProfitDetails.Where(pd => pd.ProfitYear <= profitYear).ToListAsync(ct);
                Dictionary<int, List<ProfitDetail>> profitDetailBySsn = allProfitDetail.GroupBy(pd => pd.Ssn).ToDictionary(g => g.Key, g => g.ToList());

                foreach (PayProfit pp in allPayProfitBySsn.Values)
                {
                    int ssn = pp.Demographic!.Ssn;
                    List<ProfitDetail> pds = profitDetailBySsn.ContainsKey(ssn)
                        ? profitDetailBySsn[ssn].OrderBy(pd => pd.ProfitYear).ThenBy(pd => pd.ProfitYearIteration).ThenBy(pd => pd.ProfitCodeId).ToList()
                        : new List<ProfitDetail>();
                    byte years = yearsBySsn.ContainsKey(ssn) ? yearsBySsn[ssn] : (byte)0;
                    byte newEnrollmentId = 0;
                    if (profitDetailBySsn.ContainsKey(ssn))
                    {
                        EnrollmentSummarizer enrollmentSummarizer = new();
                        newEnrollmentId = enrollmentSummarizer.ComputeEnrollment(pp, years, pds);
                    }

                    pp.EnrollmentId = newEnrollmentId;
                }

                await ctx.SaveChangesAsync(ct);
            }, ct);
        }
    }
}
