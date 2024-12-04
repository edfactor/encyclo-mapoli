using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Provides services related to profit sharing contributions.
/// </summary>
public sealed class ContributionService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public ContributionService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }


    internal Task<Dictionary<int, byte>> GetContributionYears(ISet<int> employeeId)
    {
        return _dataContextFactory.UseReadOnlyContext(context =>
        {
            return GetContributionYears(context, employeeId)
                .ToDictionaryAsync(arg => arg.EmployeeId, arg => arg.YearsInPlan);
        });
    }

    /// <summary>
    /// Retrieves the contribution years for a set of employees.
    /// </summary>
    /// <param name="context">
    /// The database context used to access profit sharing data.
    /// </param>
    /// <param name="employeeId">
    /// A set of employee IDs for which to retrieve contribution years.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ContributionYears"/> representing the contribution years
    /// for each employee in the specified set.
    /// </returns>
    internal IQueryable<ContributionYears> GetContributionYears(IProfitSharingDbContext context, ISet<int> employeeId)
    {

        return context.PayProfits
            .Include(p => p.Demographic)
            .Where(p => employeeId.Contains(p.Demographic!.EmployeeId))
            .GroupBy(p => p.Demographic!.EmployeeId)
            .Select(p => new ContributionYears { EmployeeId = p.Key,YearsInPlan = (byte)p.Count() });
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
