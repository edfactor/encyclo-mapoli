using System.Runtime.CompilerServices;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Extensions;
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
/// </remarks>
public sealed class TotalService : ITotalService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly ICalendarService _calendarService;

    public TotalService(IProfitSharingDataContextFactory profitSharingDataContextFactory,
        ICalendarService calendarService)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendarService = calendarService;
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
    internal IQueryable<ParticipantTotalDto> GetTotalBalanceSetEmployeePortion(IProfitSharingDbContext ctx, short profitYear)
    {
        byte[] sumAllFieldProfitCodeTypes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

        return (from pd in ctx.ProfitDetails
                where pd.ProfitYear <= profitYear && pd.CommentRelatedOracleHcmId == null
                group pd by pd.Ssn
            into pd_g
                select new ParticipantTotalDto
                {
                    Ssn = pd_g.Key,
                    Total = pd_g.Sum(x =>
                        sumAllFieldProfitCodeTypes.Contains(x.ProfitCodeId)
                            ? (-x.Forfeiture + x.Contribution + x.Earnings)
                            : (x.Contribution + x.Earnings + x.Forfeiture)) //Just add the columns
                });
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
    internal IQueryable<ParticipantTotalDto> GetTotalBalanceSet(IProfitSharingDbContext ctx, short profitYear)
    {
        byte[] sumAllFieldProfitCodeTypes = ProfitDetailExtensions.GetProfitCodesForBalanceCalc();

        return (from pd in ctx.ProfitDetails
                where pd.ProfitYear <= profitYear
                group pd by pd.Ssn
            into pd_g
                select new ParticipantTotalDto
                {
                    Ssn = pd_g.Key,
                    Total = pd_g.Sum(x =>
                        sumAllFieldProfitCodeTypes.Contains(x.ProfitCodeId)
                            ? (-x.Forfeiture + x.Contribution + x.Earnings)
                            : (x.Contribution + x.Earnings + x.Forfeiture)) //Just add the columns
                });
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
    internal IQueryable<ParticipantTotalDto> GetTotalComputedEtva(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
            group pd by pd.Ssn
            into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id /*6*/)
                            .Sum(x => x.Contribution)
                        + pd_g.Where(
                                x => x.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id /*8*/)
                            .Sum(x => x.Earnings)
                        - pd_g.Where(
                                x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id /*9*/)
                            .Sum(x => x.Forfeiture)
            }
        );
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
                Total = pd_g.Where(x => new[]
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
    /// Retrieves the total years of service for participants in the profit-sharing plan for a specified year.
    /// </summary>
    /// <param name="ctx">
    /// The database context used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The employee year for which the total years of service are to be returned.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalYearsDto"/> containing the SSN and total years of service for each participant.
    /// </returns>
    internal IQueryable<ParticipantTotalYearsDto> GetYearsOfService(IProfitSharingDbContext ctx, short profitYear)
    {
        return 
                (from pdx in 
                     (from pd in ctx.ProfitDetails
                      where pd.ProfitYear <= profitYear
                      group pd by new { pd.Ssn, pd.ProfitYear } into pdGrp
                      select new { pdGrp.Key.Ssn, pdGrp.Key.ProfitYear, YearsOfServiceCredit = pdGrp.Max(x => x.YearsOfServiceCredit) }
                     ) // Get the max value per year, and use that.  This is so that if a year has more than one row, we're only counting the max value for that year.
                group pdx by pdx.Ssn into pdxGrp
                select new ParticipantTotalYearsDto() { Ssn = pdxGrp.Key, Years = (byte)pdxGrp.Sum(x=>x.YearsOfServiceCredit)}
                );
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
            select new ParticipantTotalDto()
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => validProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture)
            });
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
            select new ParticipantTotalDto()
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => validProfitCodes.Contains(x.ProfitCodeId)).Sum(x => x.Forfeiture)
            });
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
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalRatioDto"/> containing the calculated vesting ratios
    /// for each participant.
    /// </returns>
    internal IQueryable<ParticipantTotalRatioDto> GetVestingRatio(IProfitSharingDbContext ctx, short profitYear,
        DateOnly asOfDate)
    {

        var birthDate65 = asOfDate.AddYears(-65);
        var beginningOfYear = asOfDate.AddYears(-1).AddDays(1);

        var demoInfo = (
            from d in ctx.Demographics
            select new
            {
                d.Ssn,
                d.DateOfBirth,
                FromBeneficiary = (short)0,
            }
        );

        var beneficiaryInfo = (
            from b in ctx.Beneficiaries
            join dt in ctx.Demographics on b.Contact!.Ssn equals dt.Ssn into d_join
            where !d_join.Any()
            select new
            {
                b.Contact!.Ssn,
                b.Contact.DateOfBirth,
                FromBeneficiary = (short)1,
            }
        );

        var demoOrBeneficiary = demoInfo.Union(beneficiaryInfo);
        var hoursWorkedRequirement = ContributionService.MinimumHoursForContribution();

#pragma warning disable S1244 // Floating point numbers should not be tested for equality
#pragma warning disable S125 // Sections of code should not be commented out
        return (
            from db in demoOrBeneficiary
            join dTbl in ctx.Demographics on db.Ssn equals dTbl.Ssn into dTmp
            from d in dTmp.DefaultIfEmpty()
            join ppTbl in ctx.PayProfits on new { Id = (d != null ? d.Id : 0), ProfitYear = profitYear } equals new
            {
                Id = ppTbl.DemographicId,
                ppTbl.ProfitYear
            } into ppTmp
            from pp in ppTmp.DefaultIfEmpty()
            join cyTbl in GetYearsOfService(ctx, profitYear) on db.Ssn equals cyTbl.Ssn into cyTmp
            from cy in cyTmp.DefaultIfEmpty()
            select new ParticipantTotalRatioDto
            {
                Ssn = db.Ssn,
                Ratio = 
                    //Beneficiaries are always 100% vested
                    db.FromBeneficiary == 1 ? 1.0m :

                    //Otherwise, If over 65, and not terminated this year, 100% vested
                    db.DateOfBirth <= birthDate65 &&
                        ((d != null && d.TerminationDate == null) || (d!= null && d.TerminationDate < beginningOfYear)) ? 1m :

                    //Otherwise, If enrollment has forfeitures, 100%
                    (pp != null && pp.EnrollmentId == 
                        Enrollment.Constants.OldVestingPlanHasForfeitureRecords) || (pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords) ? 1m :

                    //Otherwise, If deceased, mark for 100% vested
                    (d != null && d.TerminationCodeId == TerminationCode.Constants.Deceased) ? 1m :

                    //Otherwise, If zero contribution reason is as below, 100% vested 
                    (pp != null && pp.ZeroContributionReasonId == ZeroContributionReason.Constants
                        .SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested) ? 1m :

                    //Otherwise, If total years (including the present one) is 0, 1, or 2, 0% Vested
                    ((pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions) ? 1 : 0) +
                        ((pp != null && pp.CurrentHoursYear + pp.HoursExecutive>= hoursWorkedRequirement) ? 1 : 0) + (cy != null ? cy.Years : 0) < 3 ? 0m :

                    //Otherwise, If total years (including the present one) is 3, 20% Vested
                    ((pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions) ? 1 : 0) +
                        ((pp != null && pp.CurrentHoursYear + pp.HoursExecutive >= hoursWorkedRequirement) ? 1 : 0) + (cy != null ? cy.Years : 0) == 3 ? .2m :

                    //Otherwise, If total years (including the present one) is 4, 40% Vested
                    ((pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions) ? 1 : 0) +
                        ((pp != null && pp.CurrentHoursYear + pp.HoursExecutive >= hoursWorkedRequirement) ? 1 : 0) + (cy != null ? cy.Years : 0) == 4 ? .4m :

                    //Otherwise, If total years (including the present one) is 5, 60% Vested
                    ((pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions) ? 1 : 0) +
                        ((pp != null && pp.CurrentHoursYear + pp.HoursExecutive >= hoursWorkedRequirement) ? 1 : 0) + (cy != null ? cy.Years : 0) == 5 ? .6m :

                    //Otherwise, If total years (including the present one) is 6, 80% Vested
                    ((pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions) ? 1 : 0) +
                        ((pp != null && pp.CurrentHoursYear + pp.HoursExecutive >= hoursWorkedRequirement) ? 1 : 0) + (cy != null ? cy.Years : 0) == 6 ? .8m :

                    //Otherwise, If total years (including the present one) is more than 6, 100% Vested
                    ((pp != null && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions) ? 1 : 0) +
                        ((pp != null && pp.CurrentHoursYear + pp.HoursExecutive >= hoursWorkedRequirement) ? 1 : 0) + (cy != null ? cy.Years : 0) > 6 ? 1m : 

                    //Otherwise, 0% vested
                    0
            }
        );
#pragma warning restore S125 // Sections of code should not be commented out
#pragma warning restore S1244 // Floating point numbers should not be tested for equality
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
    internal IQueryable<ParticipantTotalVestingBalanceDto> TotalVestingBalance(IProfitSharingDbContext ctx,
        short profitYear, DateOnly asOfDate)
    {
        return TotalVestingBalance(ctx, profitYear, profitYear, asOfDate);
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
    internal IQueryable<ParticipantTotalVestingBalanceDto> TotalVestingBalance(IProfitSharingDbContext ctx,
        short employeeYear, short profitYear, DateOnly asOfDate)
    {
        return (from b in GetTotalBalanceSet(ctx, profitYear)
                join etvaTbl in GetTotalComputedEtva(ctx, employeeYear) on b.Ssn equals etvaTbl.Ssn into etvaTmp
                from e in etvaTmp.DefaultIfEmpty()
                join distTbl in GetTotalDistributions(ctx, profitYear) on b.Ssn equals distTbl.Ssn into distTmp
                from d in distTmp.DefaultIfEmpty()
                join vestTbl in GetVestingRatio(ctx, employeeYear, asOfDate) on e.Ssn equals vestTbl.Ssn into vestTmp
                from v in vestTmp.DefaultIfEmpty()
                join yipTbl in GetYearsOfService(ctx, profitYear) on b.Ssn equals yipTbl.Ssn into yipTmp
                from yip in yipTmp.DefaultIfEmpty()
                select new ParticipantTotalVestingBalanceDto
                {
                    Ssn = b.Ssn,
                    CurrentBalance = b.Total ?? 0,
                    Etva = e.Total ?? 0,
                    TotalDistributions = d.Total ?? 0,
                    VestingPercent = v.Ratio ?? 0,
                    YearsInPlan = yip.Years ?? 0,
                    VestedBalance = (((b.Total ?? 0) + (d.Total ?? 0) - (e.Total ?? 0)) * (v.Ratio ?? 0)) + (e.Total ?? 0) - (d.Total ?? 0)
                }
            );
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
        var calendarInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, cancellationToken);
        switch (searchBy)
        {
            case SearchBy.BadgeNumber:
                return await _profitSharingDataContextFactory.UseReadOnlyContext(ctx =>
                {
                    var rslt = (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                        join d in ctx.Demographics on t.Ssn equals d.Ssn
                        where d.BadgeNumber == badgeNumberOrSsn
                        select new BalanceEndpointResponse
                        {
                            Id = badgeNumberOrSsn,
                            Ssn = t.Ssn.MaskSsn(),
                            CurrentBalance = (t.CurrentBalance ?? 0),
                            Etva = (t.Etva ?? 0),
                            TotalDistributions = (t.TotalDistributions ?? 0),
                            VestedBalance = (t.VestedBalance ?? 0),
                            VestingPercent = (t.VestingPercent ?? 0),
                            YearsInPlan = (t.YearsInPlan ?? 0)
                        }).FirstOrDefaultAsync(cancellationToken);
                    return rslt;
                });

            default: //SSN
                return await _profitSharingDataContextFactory.UseReadOnlyContext(ctx =>
                {
                    var rslt = (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                        where t.Ssn == badgeNumberOrSsn
                        select new BalanceEndpointResponse
                        {
                            Id = badgeNumberOrSsn,
                            Ssn = t.Ssn.MaskSsn(),
                            CurrentBalance = (t.CurrentBalance ?? 0),
                            Etva = (t.Etva ?? 0),
                            TotalDistributions = (t.TotalDistributions ?? 0),
                            VestedBalance = (t.VestedBalance ?? 0),
                            VestingPercent = (t.VestingPercent ?? 0),
                            YearsInPlan = (t.YearsInPlan ?? 0)
                        }).FirstOrDefaultAsync(cancellationToken);
                    return rslt;
                });

        }
    }

    public static IQueryable<InternalProfitDetailDto> GetTransactionsBySsnForProfitYear(IProfitSharingDbContext ctx, short profitYear)
    {
        return ctx.ProfitDetails
            .Where(pd=>pd.ProfitYear == profitYear)
            .GroupBy(details => details.Ssn)
            .Select(g => new
            {
                Ssn = g.Key,
                TotalContributions = g.Sum(x => x.CalculateContribution()),
                TotalEarnings = g.Sum(x => x.CalculateEarnings()),
                TotalForfeitures = g.Sum(x => x.CalculateForfeiture()),
                TotalPayments = g.Sum(x => x.CalculatePayment()),
                Distribution = g.Sum(x => x.CalculateDistribution()),
                BeneficiaryAllocation = g.Sum(x => x.CalculateBeneficiaryAllocation()),
                CurrentBalance = g.Sum(x => x.CalculateRowBalance())
            })
            .Select(r => new InternalProfitDetailDto
            {
                Ssn = r.Ssn,
                TotalContributions = r.TotalContributions,
                TotalEarnings = r.TotalEarnings,
                TotalForfeitures = r.TotalForfeitures,
                TotalPayments = r.TotalPayments,
                CurrentAmount = r.CurrentBalance,
                Distribution = r.Distribution,
                BeneficiaryAllocation = r.BeneficiaryAllocation
            });
    }
    
    /// <summary>
    /// Extracts a single year of profit_detail transactions.
    /// Ignores any 0 records
    /// includes special handling for ClassActionFund and Military.
    /// </summary>
    internal static IQueryable<InternalProfitDetailTotalsBySsn> GetProfitDetailTotalsForASingleYear( IProfitSharingDbContext ctx,short profitYear, CancellationToken cancellationToken)
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
    internal static Task<Dictionary<int, ProfitDetailTotals>> GetProfitDetailTotalsForASingleYear(IProfitSharingDataContextFactory dbFactory, short profitYear, HashSet<int> ssns,
        CancellationToken cancellationToken)
    {
        return dbFactory.UseReadOnlyContext(ctx =>
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

            return query.ToDictionaryAsync(k => k.Ssn, v => new ProfitDetailTotals
                (v.DistributionsTotal,
                    v.ForfeitsTotal,
                    v.AllocationsTotal,
                    v.PaidAllocationsTotal,
                    v.MilitaryTotal,
                    v.ClassActionFundTotal
                )
                , cancellationToken);
        });
    }
}
