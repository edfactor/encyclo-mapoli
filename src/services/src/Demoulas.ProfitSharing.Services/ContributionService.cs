using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
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

    /// <summary>
    /// Retrieves a queryable collection of net balance details for employees based on the specified profit year.
    /// </summary>
    /// <param name="profitYear">
    /// The profit year up to which the net balance details should be calculated.
    /// </param>
    /// <param name="ctx">
    /// The database context used to access profit sharing data.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="InternalProfitDetailDto"/> containing net balance details,
    /// including contributions, earnings, forfeitures, payments, and taxes for each employee.
    /// </returns>
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
            .Select(d => new { d.OracleHcmId, BadgeNumber = d.BadgeNumber, d.Ssn });

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
        return query;
    }
    
    /// <summary>
    /// Retrieves the net balance details for a specified profit year and a set of badge numbers.
    /// </summary>
    /// <param name="profitYear">
    /// The profit year for which the net balance is to be calculated.
    /// </param>
    /// <param name="badgeNumbers">
    /// A set of badge numbers identifying the employees whose net balance details are to be retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a dictionary where the key is the badge number
    /// and the value is an <see cref="InternalProfitDetailDto"/> representing the net balance details for the corresponding badge number.
    /// </returns>
    internal Task<Dictionary<int, InternalProfitDetailDto>> GetNetBalance(short profitYear, ISet<int> badgeNumbers, CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            var query = from d in GetNetBalanceQuery(profitYear, ctx).Where(x => badgeNumbers.Contains(x.BadgeNumber)) select d;

            return query.ToDictionaryAsync(d => d.BadgeNumber, cancellationToken);
        });
    }

    #pragma warning disable S3400
    internal static short MinimumHoursForContribution()
    {
        return 1000;
    }
    #pragma warning restore S3400
}
