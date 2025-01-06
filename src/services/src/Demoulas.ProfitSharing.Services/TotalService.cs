using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.ServiceDto;
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

    public TotalService(IProfitSharingDataContextFactory profitSharingDataContextFactory, ICalendarService calendarService)
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
    public IQueryable<ParticipantTotalDto> GetTotalBalanceSet(IProfitSharingDbContext ctx, short profitYear)
    {
        var sumAllFieldProfitCodeTypes = new[] {
                                       ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                                       ProfitCode.Constants.OutgoingForfeitures.Id,
                                       ProfitCode.Constants.OutgoingDirectPayments.Id,
                                       ProfitCode.Constants.OutgoingXferBeneficiary.Id
        };

#pragma warning disable S3358 // Ternary operators should not be nested
        return (from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
            group pd by pd.Ssn
            into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id ? x.Forfeiture * -1 : //Just look at forfeiture
                    sumAllFieldProfitCodeTypes.Contains(x.ProfitCodeId) ? -x.Forfeiture + x.Contribution + x.Earnings : //Invert forfeiture, and add columns
                    x.Contribution + x.Earnings + x.Forfeiture) //Just add the columns
            });
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    /// <summary>
    /// Retrieves the total profit-sharing amounts for participants up to a specified profit year.
    /// </summary>
    /// <param name="ctx">
    /// The database context used to access profit-sharing data.
    /// </param>
    /// <param name="profitYear">
    /// The profit year up to which the totals should be calculated.
    /// </param>
    /// <returns>
    /// A queryable collection of <see cref="ParticipantTotalDto"/> objects, each containing the SSN and total profit-sharing amount for a participant.
    /// </returns>
    public IQueryable<ParticipantTotalDto> GetTotalEtva(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
            group pd by pd.Ssn into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id).Sum(x => x.Contribution) +
                       pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id).Sum(x => x.Earnings) +
                       pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id).Sum(x => x.Forfeiture)
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
    public IQueryable<ParticipantTotalDto> GetTotalDistributions(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
            group pd by pd.Ssn into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => new[] {
                        ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id,
                        ProfitCode.Constants.OutgoingForfeitures.Id,
                        ProfitCode.Constants.OutgoingDirectPayments.Id,
                        ProfitCode.Constants.OutgoingXferBeneficiary.Id,
                        ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
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
    /// The year for which the total years of service are to be calculated.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalYearsDto"/> containing the SSN and total years of service for each participant.
    /// </returns>
    public IQueryable<ParticipantTotalYearsDto> GetYearsOfService(IProfitSharingDbContext ctx, short profitYear)
    {
        return (from pp in ctx.PayProfits.Include(p => p.Demographic)
            where pp.ProfitYear == profitYear
            select new ParticipantTotalYearsDto { Ssn = pp.Demographic!.Ssn, Years = pp.YearsInPlan });
    }

    /// <summary>
    /// Calculates the vesting ratio for participants based on their demographic and beneficiary information,
    /// years of service, hours worked, and other criteria.
    /// </summary>
    /// <param name="ctx">
    /// The database context used to access demographic, pay profit, and beneficiary data.
    /// </param>
    /// <param name="profitYear">
    /// The profit-sharing year for which the vesting ratio is being calculated.
    /// </param>
    /// <param name="asOfDate">
    /// The date as of which the vesting ratio is being determined.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> of <see cref="ParticipantTotalRatioDto"/> containing the calculated vesting ratios
    /// for each participant.
    /// </returns>
    public IQueryable<ParticipantTotalRatioDto> GetVestingRatio(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate)
    {

        var birthDate65 = asOfDate.AddYears(-65);
        var beginningOfYear = asOfDate.AddYears(-1).AddDays(1);

        var demoInfo = (
            from d in ctx.Demographics
            join pp in ctx.PayProfits on new { d.Id, ProfitYear = profitYear } equals new { Id = pp.DemographicId, pp.ProfitYear }
            join cy in GetYearsOfService(ctx, profitYear) on d.Ssn equals cy.Ssn
            select new
            {
                d.Ssn,
                pp.EnrollmentId,
                d.TerminationCodeId,
                d.TerminationDate,
                pp.ZeroContributionReasonId,
                d.DateOfBirth,
                FromBeneficiary = (short)0,
                Years = cy.Years,
                Hours = pp.CurrentHoursYear,
            }
        );

        var beneficiaryInfo = (
            from b in ctx.Beneficiaries
            join dt in ctx.Demographics on b.Contact!.Ssn equals dt.Ssn into d_join
            where !d_join.Any()
            select new
            {
                b.Contact!.Ssn,
                EnrollmentId = (byte)0,
                TerminationCodeId = (char?)null,
                TerminationDate = (DateOnly?)null,
                ZeroContributionReasonId = (byte?)null,
                b.Contact.DateOfBirth,
                FromBeneficiary = (short)1,
                Years = (short)0,
                Hours = (decimal)0
            }
        );

        var demoOrBeneficiary = demoInfo.Union(beneficiaryInfo);
        var hoursWorkedRequirement = ContributionService.MinimumHoursForContribution();

#pragma warning disable S1244 // Floating point numbers should not be tested for equality
#pragma warning disable S3358 // Ternary operators should not be nested
        return (
            from db in demoOrBeneficiary
            select new ParticipantTotalRatioDto()
            {
                Ssn = db.Ssn,
                Ratio = db.FromBeneficiary == 1 ? 1.0m :
                        db.DateOfBirth <= birthDate65 && (db.TerminationDate == null || db.TerminationDate < beginningOfYear) ? 1m :
                        db.EnrollmentId == 3 || db.EnrollmentId == 4 ? 1m :
                        db.TerminationCodeId == 'Z' ? 1m :
                        db.ZeroContributionReasonId == 6 ? 1m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= hoursWorkedRequirement ? 1 : 0) + db.Years < 3 ? 0m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= hoursWorkedRequirement ? 1 : 0) + db.Years == 3 ? .2m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= hoursWorkedRequirement ? 1 : 0) + db.Years == 4 ? .4m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= hoursWorkedRequirement ? 1 : 0) + db.Years == 5 ? .6m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= hoursWorkedRequirement ? 1 : 0) + db.Years == 6 ? .8m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= hoursWorkedRequirement ? 1 : 0) + db.Years > 6 ? 1m : 0
            }
        );
#pragma warning restore S3358 // Ternary operators should not be nested
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
    public IQueryable<ParticipantTotalVestingBalanceDto> TotalVestingBalance(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate)
    {
        return (from b in GetTotalBalanceSet(ctx, profitYear)
                join e in GetTotalEtva(ctx, profitYear) on b.Ssn equals e.Ssn
                join d in GetTotalDistributions(ctx, profitYear) on b.Ssn equals d.Ssn
                join v in GetVestingRatio(ctx, profitYear, asOfDate) on e.Ssn equals v.Ssn
                select new ParticipantTotalVestingBalanceDto
                {
                    Ssn = e.Ssn,
                    CurrentBalance = b.Total,
                    Etva = e.Total,
                    TotalDistributions = d.Total,
                    VestingPercent = v.Ratio,
                    VestedBalance = ((b.Total + d.Total - e.Total) * v.Ratio) + e.Total - d.Total
                }
        );
    }

    /// <summary>
    /// Retrieves the vesting balance for a single member based on the specified search criteria.
    /// </summary>
    /// <param name="searchBy">
    /// Specifies the search criteria, either by Social Security Number (SSN) or Employee ID.
    /// </param>
    /// <param name="employeeIdOrSsn">
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
    public async Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMemberAsync(SearchBy searchBy, int employeeIdOrSsn, short profitYear, CancellationToken cancellationToken)
    {
        var calendarInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, cancellationToken);
        switch (searchBy)
        {
            case SearchBy.EmployeeId:
                return await _profitSharingDataContextFactory.UseReadOnlyContext(ctx =>
                {
                    var rslt = (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                                      join d in ctx.Demographics on t.Ssn equals d.Ssn
                                      where d.EmployeeId == employeeIdOrSsn
                                      select new BalanceEndpointResponse { Id = employeeIdOrSsn, Ssn = t.Ssn.MaskSsn(), CurrentBalance = t.CurrentBalance, Etva = t.Etva, TotalDistributions = t.TotalDistributions, VestedBalance = t.VestedBalance, VestingPercent = t.VestingPercent }).FirstOrDefaultAsync(cancellationToken);
                    return rslt;
                });

            default: //SSN
                return await _profitSharingDataContextFactory.UseReadOnlyContext(ctx =>
                {
                    var rslt = (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate) where t.Ssn == employeeIdOrSsn
                                      select new BalanceEndpointResponse { Id = employeeIdOrSsn, Ssn = t.Ssn.MaskSsn(), CurrentBalance = t.CurrentBalance, Etva = t.Etva, TotalDistributions = t.TotalDistributions, VestedBalance =  t.VestedBalance, VestingPercent = t.VestingPercent}).FirstOrDefaultAsync(cancellationToken);
                    return rslt;
                });
                
        }
    }
}
