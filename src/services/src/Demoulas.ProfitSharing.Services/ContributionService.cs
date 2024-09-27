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

    internal async Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(short profitYear, ISet<int> badgeNumber, CancellationToken cancellationToken)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var pdQuery = await ctx.ProfitDetails
                .Where(details => details.ProfitYear <= profitYear)
                .Select(details => new 
                {
                    details.Ssn,
                    details.Contribution,
                    details.Earnings,
                    ProfitFort = details.ProfitCode == 0 ? details.Forfeiture : 0,
                    ProfitPymt = details.ProfitCode != 0 ? details.Forfeiture : 0,
                    details.FederalTaxes,
                    details.StateTaxes
                })
                .GroupBy(d => d.Ssn)
                .Select(g => new
                {
                    Ssn = g.Key,
                    TotalContributions = g.Sum(x => x.Contribution),
                    TotalEarnings = g.Sum(x => x.Earnings),
                    TotalForfeitures = g.Sum(x => x.ProfitFort),
                    TotalPayments = g.Sum(x => x.ProfitPymt),
                    TotalFedTaxes = g.Sum(x => x.FederalTaxes),
                    TotalStateTaxes = g.Sum(x => x.StateTaxes)
                }).ToListAsync(cancellationToken);

            var demoQuery = await ctx.Demographics
                .Where(d => badgeNumber.Contains(d.BadgeNumber))
                .Select(d => new { d.OracleHcmId, d.BadgeNumber, d.Ssn })
                .ToListAsync(cancellationToken);
            
            var query = from d in demoQuery
                join r in pdQuery on d.Ssn equals r.Ssn
                select new InternalProfitDetailDto
                {
                    OracleHcmId = d.OracleHcmId,
                    BadgeNumber = d.BadgeNumber,
                    TotalContributions = r.TotalContributions,
                    TotalEarnings = r.TotalEarnings,
                    TotalForfeitures = r.TotalForfeitures,
                    TotalPayments = r.TotalPayments,
                    TotalFederalTaxes = r.TotalFedTaxes,
                    TotalStateTaxes = r.TotalStateTaxes
                };

            return query.ToDictionary(d=> d.BadgeNumber);
        });
    }
}
