using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public sealed class ContributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public ContributionService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    internal Task<Dictionary<int, int>> GetContributionYears(ISet<int> badgeNumber)
    {
        return _dataContextFactory.UseReadOnlyContext(context =>
        {
            return context.PayProfits
                .Include(p => p.Demographic)
                .Where(p => badgeNumber.Contains(p.Demographic!.BadgeNumber))
                .GroupBy(p => p.Demographic!.BadgeNumber)
                .Select(p => new { BadgeNumber = p.Key, ContributionYears = p.Count() })
                .ToDictionaryAsync(arg => arg.BadgeNumber, arg => arg.ContributionYears);
        });
    }

    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(short profitYear, ISet<int> badgeNumber, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = from dem in ctx.Demographics
                join pd in ctx.ProfitDetails on dem.Ssn equals pd.Ssn into tmpPd
                where badgeNumber.Contains(dem.BadgeNumber)
                group new { dem, tmpPd }
                    by new InternalProfitDetailDto
                    {
                      OracleHcmId  = dem.OracleHcmId,
                      BadgeNumber = dem.BadgeNumber,
                      NetBalance = tmpPd.Where(t => t.ProfitYear <= profitYear)
                          .Sum(t => t.Earnings + t.Contribution),
                    }
                into grp
                select new InternalProfitDetailDto { BadgeNumber = grp.Key.BadgeNumber, NetBalance = grp.Key.NetBalance };

            return query.ToDictionaryAsync(d=> d.BadgeNumber, cancellationToken);
        });
    }
}
