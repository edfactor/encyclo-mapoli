using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public sealed class ContributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public ContributionService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    public Task<Dictionary<int, int>> GetContributionYears(ISet<int> badgeNumber)
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

    public Task<Dictionary<int, decimal>> GetNetBalance(short fiscalYear)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var query = from pd in ctx.ProfitDetails
                join dem in ctx.Demographics on pd.Ssn equals dem.Ssn into tmpPayProfit
                //from pp in tmpPayProfit.DefaultIfEmpty()
                //join pdLj in ctx.ProfitDetails on dem.Ssn equals pdLj.Ssn into tmpProfitDetails
                //from pd in tmpProfitDetails.DefaultIfEmpty()
                //where pp.FiscalYear == req.ReportingYear && dupNameSlashDateOfBirth.Contains(dem.FullName)
        });
    }
}
