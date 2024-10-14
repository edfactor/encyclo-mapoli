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


    internal Task<Dictionary<int, byte>> GetContributionYears(short profitYear, ISet<int> badgeNumberSet, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(context => GetContributionYears(context, profitYear, badgeNumberSet, cancellationToken));
    }

    internal static Task<Dictionary<int, byte>> GetContributionYears(IProfitSharingDbContext context, short profitYear, ISet<int> badgeNumberSet, CancellationToken cancellationToken)
    {
        return GetContributionYearsQuery(context, profitYear, badgeNumberSet)
            .ToDictionaryAsync(arg => arg.BadgeNumber, arg => arg.YearsInPlan, cancellationToken);
    }

    internal static IQueryable<ContributionYears> GetContributionYearsQuery(IProfitSharingDbContext context, short profitYear, IEnumerable<int> badgeNumberSet)
    {
        return context.PayProfits
            .Include(p => p.Demographic)
            .Where(p => p.ProfitYear <= profitYear && badgeNumberSet.Contains(p.Demographic!.BadgeNumber))
            .GroupBy(p => p.Demographic!.BadgeNumber)
            .Select(p => new ContributionYears { BadgeNumber = p.Key, YearsInPlan = (byte)p.Count() });
    }

    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(short profitYear, IEnumerable<int> badgeNumbers, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx => GetNetBalance(ctx, profitYear, badgeNumbers, cancellationToken));
    }

    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(IProfitSharingDbContext context, short profitYear, IEnumerable<int> badgeNumbers,
        CancellationToken cancellationToken)
    {
        var badgeHash = badgeNumbers.ToHashSet();
        var pdQuery = context.ProfitDetails
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



        var demoQuery = context.Demographics
            .Where(d => badgeHash.Contains(d.BadgeNumber))
            .Select(d => new { d.OracleHcmId, d.BadgeNumber, d.Ssn });

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

        return query.ToDictionaryAsync(d => d.BadgeNumber, cancellationToken);
    }
}
