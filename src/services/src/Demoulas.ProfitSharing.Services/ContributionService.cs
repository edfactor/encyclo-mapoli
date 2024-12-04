using Demoulas.ProfitSharing.Data.Entities;
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
                .Where(p => badgeNumber.Contains(p.Demographic!.EmployeeId))
                .GroupBy(p => p.Demographic!.EmployeeId)
                .Select(p => new { BadgeNumber = p.Key, ContributionYears = p.Count() })
                .ToDictionaryAsync(arg => arg.BadgeNumber, arg => arg.ContributionYears);
        });
    }

    internal IQueryable<InternalProfitDetailDto> GetNetBalanceQuery(short profitYear, IProfitSharingDbContext ctx)
    {
        var pdQuery = ctx.ProfitDetails
                .Where(details => details.ProfitYear <= profitYear)
                .GroupBy(details => details.Ssn)
                .Select(g => new
                {
                    Ssn = g.Key,
                    TotalContributions = g.Sum(x => x.Contribution),
                    TotalEarnings = g.Sum(x => x.Earnings),
                    TotalForfeitures = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
                    TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
                    TotalFedTaxes = g.Sum(x => x.FederalTaxes),
                    TotalStateTaxes = g.Sum(x => x.StateTaxes)
                });



        var demoQuery = ctx.Demographics
            .Select(d => new { d.OracleHcmId, d.EmployeeId, d.Ssn });

        var query = from d in demoQuery
                    join r in pdQuery on d.Ssn equals r.Ssn
                    select new InternalProfitDetailDto
                    {
                        OracleHcmId = d.OracleHcmId,
                        BadgeNumber = d.EmployeeId,
                        TotalContributions = r.TotalContributions,
                        TotalEarnings = r.TotalEarnings,
                        TotalForfeitures = r.TotalForfeitures,
                        TotalPayments = r.TotalPayments,
                        TotalFederalTaxes = r.TotalFedTaxes,
                        TotalStateTaxes = r.TotalStateTaxes
                    };
        return query;
    }
    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(short profitYear, ISet<int> badgeNumbers, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = from d in GetNetBalanceQuery(profitYear, ctx).Where(x => badgeNumbers.Contains(x.BadgeNumber)) select d;

            return query.ToDictionaryAsync(d => d.BadgeNumber, cancellationToken);
        });
    }
}
