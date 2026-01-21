using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

/// <summary>
/// Provides services for calculating and retrieving various profit-sharing totals and related data.
/// </summary>
/// <remarks>
/// This class implements the <see cref="ITotalService"/> interface and offers methods to compute
/// participant totals, distributions, vesting balances, and other profit-sharing-related metrics.
/// It relies on dependencies such as <see cref="IProfitSharingDataContextFactory"/> and <see cref="ICalendarService"/>
/// to interact with the data context and calendar-related operations.
///
/// Design notes, executive summary and SQL snippets are documented in Confluence:
/// <see href="https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/517898377/Totals+Service+Executive+Summary+and+SQL+snippets">Totals Service — Executive Summary and SQL snippets</see>
/// </remarks>
/// <seealso href="https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/517898377/Totals+Service+Executive+Summary+and+SQL+snippets">Confluence: Totals Service</seealso>
public sealed class TotalService : ITotalService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly IEmbeddedSqlService _embeddedSqlService;
    private readonly IDemographicReaderService _demographicReaderService;

    public TotalService(IProfitSharingDataContextFactory profitSharingDataContextFactory,
        ICalendarService calendarService,
        IEmbeddedSqlService embeddedSqlService,
        IDemographicReaderService demographicReaderService)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendarService = calendarService;
        _embeddedSqlService = embeddedSqlService;
        _demographicReaderService = demographicReaderService;
    }

    /// <summary>
    /// Retrieves a queryable set of total balance data for participants based on the specified profit year.
    /// </summary>
    /// <param name="ctx">
    /// The database context implementing <see cref="IProfitSharingDbContext"/> used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The profit year (as a <see cref="short"/>) up to which the total balances are calculated.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalDto"/> containing the total balance data for participants.
    /// </returns>
    internal IQueryable<ParticipantTotal> GetTotalBalanceSet(IProfitSharingDbContext ctx, short profitYear)
    {
        return _embeddedSqlService.GetTotalBalanceAlt(ctx, profitYear);
    }

    /// <summary>
    /// Retrieves the total profit-sharing amounts for participants up to a specified profit year.
    /// </summary>
    /// <param name="ctx">
    /// The database context used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The employee year for which the totals should be returned.
    /// </param>
    /// <returns>
    /// A queryable collection of <see cref="ParticipantTotalDto"/> objects, each containing the SSN and total profit-sharing amount for a participant.
    /// </returns>
    internal IQueryable<ParticipantTotal> GetTotalComputedEtva(IProfitSharingDbContext ctx, short profitYear)
    {
        return _embeddedSqlService.GetTotalComputedEtvaAlt(ctx, profitYear);
    }


    /// <summary>
    /// Retrieves the total distributions for participants up to a specified profit year.
    /// </summary>
    /// <param name="ctx">
    /// The database context implementing <see cref="IProfitSharingDbContext"/> used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The profit year up to which distributions are calculated.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalDto"/> representing the total distributions
    /// for each participant, grouped by their SSN.
    /// </returns>
    internal IQueryable<ParticipantTotalDto> GetTotalDistributions(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
            group pd by pd.Ssn
            into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                TotalAmount = pd_g.Where(x => new[]
                {
                    /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                    /*2*/ ProfitCode.Constants.OutgoingForfeitures.Id,
                    /*3*/ ProfitCode.Constants.OutgoingDirectPayments.Id,
                    /*5*/ ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                    /*9*/ ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
                }.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture)
            }
        );
    }

    /// <summary>
    /// Retrieves the yearly distributions for participants for a specific profit year only.
    /// This is different from <see cref="GetTotalDistributions"/> which returns cumulative distributions.
    /// </summary>
    /// <param name="ctx">
    /// The database context implementing <see cref="IProfitSharingDbContext"/> used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The specific profit year for which distributions are calculated (not cumulative).
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalDto"/> representing the yearly distributions
    /// for each participant, grouped by their SSN. Only includes distributions for the specified year.
    /// </returns>
    /// <remarks>
    /// This method calculates withdrawals for a single profit year using profit codes:
    /// - PC 1: Outgoing Payments (Partial Withdrawal)
    /// - PC 3: Outgoing Direct Payments / Rollover Payments
    /// - PC 9: Outgoing Payment from 100% Vesting Amount (ETVA funds)
    ///
    /// Created to fix PS-2424: Account History Report was showing cumulative instead of yearly withdrawals.
    /// </remarks>
    internal IQueryable<ParticipantTotalDto> GetYearlyDistributions(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear == profitYear  // Note: == not <= (yearly, not cumulative)
            group pd by pd.Ssn
            into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                TotalAmount = pd_g.Where(x => new[]
                {
                    /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                    /*3*/ ProfitCode.Constants.OutgoingDirectPayments.Id,
                    /*5*/ ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                    /*9*/ ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
                }.Contains(x.ProfitCodeId)).Sum(x => Math.Abs(x.Contribution + x.Earnings + x.Forfeiture))
            }
        );
    }

    /// <summary>
    /// Retrieves the total years of service for participants in the profit-sharing plan for a specified year.
    /// </summary>
    /// <param name="ctx">
    /// The database context used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The employee year for which the total years of service are to be returned.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalYear"/> containing the SSN and total years of service for each participant.
    /// </returns>
    internal IQueryable<ParticipantTotalYear> GetYearsOfService(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate)
    {
        return _embeddedSqlService.GetYearsOfServiceAlt(ctx, profitYear, asOfDate);
    }

    /// <summary>
    /// Gets the sum total of forfeitures by SSN over the course of history with an employee
    /// </summary>
    /// <param name="ctx">The database context</param>
    /// <param name="employeeYear">Maximum profit year that will be searched through</param>
    /// <returns></returns>
    internal IQueryable<ParticipantTotalDto> GetForfeitures(IProfitSharingDbContext ctx, short employeeYear)
    {
        int[] validProfitCodes =
            [ProfitCode.Constants.OutgoingForfeitures.Id, ProfitCode.Constants.Outgoing100PercentVestedPayment.Id];
        return (from pd in ctx.ProfitDetails
                where pd.ProfitYear <= employeeYear
                group pd by pd.Ssn
            into pd_g
                select new ParticipantTotalDto() { Ssn = pd_g.Key, TotalAmount = pd_g.Where(x => validProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture) });
    }

    /// <summary>
    /// Gets the sum total of loans by SSN over the course of history with an employee
    /// </summary>
    /// <param name="ctx">The database context</param>
    /// <param name="employeeYear">Maximum Profit year that will be searched through</param>
    /// <returns></returns>
    internal IQueryable<ParticipantTotalDto> GetQuoteLoansUnQuote(IProfitSharingDbContext ctx, short employeeYear)
    {
        int[] validProfitCodes =
            [ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id, ProfitCode.Constants.OutgoingDirectPayments.Id];
        return (from pd in ctx.ProfitDetails
                where pd.ProfitYear <= employeeYear
                group pd by pd.Ssn
            into pd_g
                select new ParticipantTotalDto() { Ssn = pd_g.Key, TotalAmount = pd_g.Where(x => validProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture) });
    }

    /// <summary>
    /// Calculates the vesting ratio for participants based on their demographic and beneficiary information,
    /// years of service, hours worked, and other criteria.
    /// </summary>
    /// <param name="ctx">
    /// The database context used to access demographic, pay profit, and beneficiary data.
    /// </param>
    /// <param name="profitYear">
    /// The Profit year (aka selector of PayProfit) year for which the vesting ratio is being calculated.
    /// </param>
    /// <param name="asOfDate">
    /// The date as of which the vesting ratio is being determined.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalRatio"/> containing the calculated vesting ratios
    /// for each participant.
    /// </returns>
    internal IQueryable<ParticipantTotalRatio> GetVestingRatio(ProfitSharingReadOnlyDbContext ctx, short profitYear,
        DateOnly asOfDate)
    {
        return _embeddedSqlService.GetVestingRatioAlt(ctx, profitYear, asOfDate);
    }

    /// <summary>
    /// Retrieves the total vesting balance for participants based on the provided profit year and date.
    /// </summary>
    /// <param name="ctx">The database context used to access profit-sharing data.</param>
    /// <param name="profitYear">The profit year for which the vesting balance is calculated.</param>
    /// <param name="asOfDate">The date as of which the vesting balance is calculated.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalVestingBalanceDto"/> containing the total vesting balance
    /// details for each participant, including current balance, ETVA, total distributions, vesting percentage, and vested balance.
    /// </returns>
    internal IQueryable<ParticipantTotalVestingBalance> TotalVestingBalance(IProfitSharingDbContext ctx,
        short profitYear, DateOnly asOfDate)
    {
        return _embeddedSqlService.TotalVestingBalanceAlt(ctx, profitYear, profitYear, asOfDate);
    }


    /// <summary>
    /// Retrieves the total vesting balance for participants (using employee Year) using profit detail rows based upon the profitYear.
    /// The asOfDate is used for age at a particular moment in time.
    /// </summary>
    /// <param name="ctx">The database context used to access profit-sharing data.</param>
    /// <param name="employeeYear">Selects the employees to be used (current year, or prior year.)</param>
    /// <param name="profitYear">The profit year for which the vesting balance is calculated.  Can be this year or any prior year.</param>
    /// <param name="asOfDate">The date as of which the vesting balance is calculated. In particular for age calculation of vesting.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalVestingBalanceDto"/> containing the total vesting balance
    /// details for each participant, including current balance, ETVA, total distributions, vesting percentage, and vested balance.
    /// </returns>
    internal IQueryable<ParticipantTotalVestingBalance> TotalVestingBalance(IProfitSharingDbContext ctx,
        short employeeYear, short profitYear, DateOnly asOfDate)
    {
        return _embeddedSqlService.TotalVestingBalanceAlt(ctx, employeeYear, profitYear, asOfDate);
    }

    /// <summary>
    /// Retrieves the vesting balance for a single member based on the specified search criteria.
    /// </summary>
    /// <param name="searchBy">
    /// Specifies the search criteria, either by Social Security Number (SSN) or Employee ID.
    /// </param>
    /// <param name="badgeNumberOrSsn">
    /// The identifier used for the search, which can be either an Employee ID or an SSN, depending on the <paramref name="searchBy"/> value.
    /// </param>
    /// <param name="profitYear">
    /// The profit year for which the vesting balance is being retrieved.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the vesting balance details
    /// as a <see cref="BalanceEndpointResponse"/> object, or <c>null</c> if no matching record is found.
    /// </returns>
    public async Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMemberAsync(SearchBy searchBy,
        int badgeNumberOrSsn, short profitYear, CancellationToken cancellationToken)
    {
        var list = await GetVestingBalanceForMembersAsync(searchBy, new HashSet<int> { badgeNumberOrSsn }, profitYear, cancellationToken);
        return list?.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the first contribution year for each participant based on their SSN and a specified profit year.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="profitYear"></param>
    /// <returns></returns>
    public static IQueryable<SsnAndFirstYear> GetFirstContributionYear(IProfitSharingDbContext ctx, short profitYear)
    {
        return from pd in ctx.ProfitDetails
               where pd.ProfitYear < profitYear
                  && pd.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                  && pd.Contribution != 0
               group pd by pd.Ssn into g
               select new SsnAndFirstYear()
               {
                   Ssn = g.Key,
                   FirstContributionYear = g.Min(x => x.ProfitYear),
               };
    }


    public async Task<List<BalanceEndpointResponse>> GetVestingBalanceForMembersAsync(SearchBy searchBy,
        ISet<int> badgeNumberOrSsnCollection, short profitYear, CancellationToken cancellationToken)
    {
        var calendarInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, cancellationToken);

        switch (searchBy)
        {
            case SearchBy.BadgeNumber:
                return await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
                {
                    var demographics = await _demographicReaderService.BuildDemographicQueryAsync(ctx);
                    var rslt = await (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                                      join d in demographics on t.Ssn equals d.Ssn
                                      where badgeNumberOrSsnCollection.Contains(d.BadgeNumber)
                                      select new BalanceEndpointResponse
                                      {
                                          Id = t.Ssn,
                                          Ssn = t.Ssn.MaskSsn(),
                                          CurrentBalance = (t.CurrentBalance ?? 0),
                                          VestedBalance = (t.VestedBalance ?? 0),
                                          VestingPercent = (t.VestingPercent ?? 0),
                                          YearsInPlan = (t.YearsInPlan ?? 0),
                                          AllocationsToBeneficiary = (t.AllocationsToBeneficiary ?? 0),
                                          AllocationsFromBeneficiary = (t.AllocationsFromBeneficiary ?? 0)
                                      }).ToListAsync(cancellationToken);
                    return rslt;
                }, cancellationToken);

            default: //SSN
                return await _profitSharingDataContextFactory.UseReadOnlyContext(ctx =>
                {
                    var rslt = (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                                where badgeNumberOrSsnCollection.Contains(t.Ssn)
                                select new BalanceEndpointResponse
                                {
                                    Id = t.Ssn,
                                    Ssn = t.Ssn.MaskSsn(),
                                    CurrentBalance = (t.CurrentBalance ?? 0),
                                    VestedBalance = (t.VestedBalance ?? 0),
                                    VestingPercent = (t.VestingPercent ?? 0),
                                    YearsInPlan = (t.YearsInPlan ?? 0),
                                    AllocationsToBeneficiary = (t.AllocationsToBeneficiary ?? 0),
                                    AllocationsFromBeneficiary = (t.AllocationsFromBeneficiary ?? 0)
                                }).ToListAsync(cancellationToken);
                    return rslt;
                }, cancellationToken);
        }
    }

    /// <summary>
    ///  Retrieves the transactions by SSN for a specific profit year.   The ORACLE driver likes the long winded version of the query (not mixing in any C# methods)
    /// </summary>
    public IQueryable<ProfitDetailRollup> GetTransactionsBySsnForProfitYearForOracle(IProfitSharingDbContext ctx, short profitYear)
    {
        return _embeddedSqlService.GetTransactionsBySsnForProfitYearForOracle(ctx, profitYear);
    }


    /// <summary>
    /// Extracts a single year of profit_detail transactions.
    /// Ignores any 0 records
    /// includes special handling for ClassActionFund and Military.
    /// </summary>
    internal static IQueryable<InternalProfitDetailTotalsBySsn> GetProfitDetailTotalsForASingleYear(IProfitSharingDbContext ctx, short profitYear)
    {
        return ctx.ProfitDetails
            .Where(pd => pd.ProfitYear == profitYear)
            .GroupBy(pd => pd.Ssn) // Grouping by Ssn
            .Select(g => new InternalProfitDetailTotalsBySsn
            {
                Ssn = g.Key,
                DistributionsTotal = g.Where(pd =>
                        pd.ProfitCodeId == /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal ||
                        pd.ProfitCodeId == /*3*/ ProfitCode.Constants.OutgoingDirectPayments ||
                        (pd.ProfitCodeId == /*9*/ProfitCode.Constants.Outgoing100PercentVestedPayment &&
                         !(pd.CommentType == CommentType.Constants.TransferOut ||
                           pd.CommentType == CommentType.Constants.QdroOut)))
                    .Sum(pd => pd.Forfeiture),
                PaidAllocationsTotal = g.Where(pd =>
                        (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment &&
                         (pd.CommentType == CommentType.Constants.TransferOut ||
                          pd.CommentType == CommentType.Constants.QdroOut)) ||
                        pd.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
                    .Sum(pd => pd.Forfeiture),
                ForfeitsTotal = g.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                    .Sum(pd => pd.Forfeiture),
                AllocationsTotal = g.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary)
                    .Sum(pd => pd.Contribution),
                MilitaryTotal = g.Where(pd => pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationMilitary)
                    .Sum(pd => pd.Contribution),
                ClassActionFundTotal = g.Where(pd => pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationClassActionFund)
                    .Sum(pd => pd.Earnings)
            });
    }

    /// <summary>
    /// Extracts a single year of profit_detail transactions.
    /// Ignores any 0 records
    /// includes special handling for ClassActionFund and Military.
    /// </summary>
    internal static Task<ILookup<int, ProfitDetailTotals>> GetProfitDetailTotalsForASingleYear(
        IProfitSharingDataContextFactory dbFactory,
        short profitYear,
        HashSet<int> ssns,
        CancellationToken cancellationToken)
    {
        return dbFactory.UseReadOnlyContext(async ctx =>
        {
            var query = ctx.ProfitDetails
                .Where(pd => ssns.Contains(pd.Ssn))
                .Where(pd => pd.ProfitYear == profitYear)
                .GroupBy(pd => pd.Ssn) // Grouping by Ssn
                .Select(g => new
                {
                    Ssn = g.Key,
                    DistributionsTotal = g.Where(pd =>
                            pd.ProfitCodeId == /*1*/ ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal ||
                            pd.ProfitCodeId == /*3*/ ProfitCode.Constants.OutgoingDirectPayments ||
                            (pd.ProfitCodeId == /*9*/ProfitCode.Constants.Outgoing100PercentVestedPayment &&
                             !(pd.CommentType == CommentType.Constants.TransferOut ||
                               pd.CommentType == CommentType.Constants.QdroOut)))
                        .Sum(pd => pd.Forfeiture),
                    PaidAllocationsTotal = g.Where(pd =>
                            (pd.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment &&
                             (pd.CommentType == CommentType.Constants.TransferOut ||
                              pd.CommentType == CommentType.Constants.QdroOut)) ||
                            pd.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
                        .Sum(pd => pd.Forfeiture),
                    ForfeitsTotal = g.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                        .Sum(pd => pd.Forfeiture),
                    AllocationsTotal = g.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary)
                        .Sum(pd => pd.Contribution),
                    MilitaryTotal = g.Where(pd => pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationMilitary)
                        .Sum(pd => pd.Contribution),
                    ClassActionFundTotal = g.Where(pd => pd.ProfitYearIteration == ProfitDetail.Constants.ProfitYearIterationClassActionFund)
                        .Sum(pd => pd.Earnings)
                });

            var results = await query.ToListAsync(cancellationToken);
            return results.ToLookup(
                k => k.Ssn,
                v => new ProfitDetailTotals(
                    v.DistributionsTotal,
                    v.ForfeitsTotal,
                    v.AllocationsTotal,
                    v.PaidAllocationsTotal,
                    v.MilitaryTotal,
                    v.ClassActionFundTotal));
        }, cancellationToken);
    }
}
