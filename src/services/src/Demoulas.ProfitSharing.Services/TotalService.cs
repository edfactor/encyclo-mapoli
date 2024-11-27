using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Services;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;

public sealed class TotalService : ITotalService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly ICalendarService _calendarService;

    public TotalService(IProfitSharingDataContextFactory profitSharingDataContextFactory, ICalendarService calendarService)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendarService = calendarService;
    }
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
                group pd by pd.Ssn into pd_g
                select new ParticipantTotalDto
                {
                    Ssn = pd_g.Key,
                    Total = pd_g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment ? x.Forfeiture * -1 : //Just look at forfeiture
                                          sumAllFieldProfitCodeTypes.Contains(x.ProfitCodeId) ? -x.Forfeiture + x.Contribution + x.Earnings : //Invert forfeiture, and add columns
                                          x.Contribution + x.Earnings + x.Forfeiture) //Just add the columns
                });
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    public IQueryable<ParticipantTotalDto> GetTotalEtva(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from pd in ctx.ProfitDetails
            where pd.ProfitYear <= profitYear
            group pd by pd.Ssn into pd_g
            select new ParticipantTotalDto
            {
                Ssn = pd_g.Key,
                Total = pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary).Sum(x => x.Contribution) +
                       pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Incoming100PercentVestedEarnings.Id).Sum(x => x.Earnings) +
                       pd_g.Where(x => x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id).Sum(x => x.Forfeiture)
            }
        );
    }

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

    public IQueryable<ParticipantTotalDto> GetGrossAmount(IProfitSharingDbContext ctx, short profitYear)
    {
        return (
            from ds in ctx.Distributions
            group ds by ds.Ssn into ds_g
            select new ParticipantTotalDto()
            {
                Ssn = ds_g.Key,
                Total = ds_g.Where(x => x.StatusId != DistributionStatus.Constants.PurgeRecord && x.StatusId != DistributionStatus.Constants.PaymentMade).Sum(x => x.GrossAmount)
            }
        );
    }

    public IQueryable<ParticipantTotalYearsDto> GetYearsOfService(IProfitSharingDbContext ctx, short profitYear)
    {
        return (from pp in ctx.PayProfits.Include(p=>p.Demographic)
                where pp.ProfitYear == profitYear
                select new ParticipantTotalYearsDto { Ssn = pp.Demographic!.Ssn, Years = pp.YearsInPlan }); //Need to verify logic here
    }

    public IQueryable<ParticipantTotalRatioDto> GetVestingRatio(IProfitSharingDbContext ctx, short profitYear, DateOnly asOfDate)
    {

        var BirthDate65 = asOfDate.AddYears(-65);
        var BeginningOfYear = asOfDate.AddYears(-1).AddDays(1);

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

#pragma warning disable S1244 // Floating point numbers should not be tested for equality
#pragma warning disable S3358 // Ternary operators should not be nested
        return (
            from db in demoOrBeneficiary
            select new ParticipantTotalRatioDto()
            {
                Ssn = db.Ssn,
                Ratio = db.FromBeneficiary == 1 ? 1.0m :
                        db.DateOfBirth <= BirthDate65 && (db.TerminationDate == null || db.TerminationDate < BeginningOfYear) ? 1m :
                        db.EnrollmentId == 3 || db.EnrollmentId == 4 ? 1m :
                        db.TerminationCodeId == 'Z' ? 1m :
                        db.ZeroContributionReasonId == 6 ? 1m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years < 3 ? 0m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 3 ? .2m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 4 ? .4m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 5 ? .6m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years == 6 ? .8m :
                        (db.EnrollmentId == 2 ? 1 : 0) + (db.Hours >= 1000 ? 1 : 0) + db.Years > 6 ? 1m : 0
            }
        );
#pragma warning restore S3358 // Ternary operators should not be nested
#pragma warning restore S1244 // Floating point numbers should not be tested for equality
    }

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

    public async Task<BalanceEndpointResponse?> GetVestingBalanceForSingleMember(SearchBy searchBy, string id, short profitYear)
    {
        var calendarInfo = await _calendarService.GetYearStartAndEndAccountingDates(profitYear);
        switch (searchBy)
        {
            case SearchBy.EmployeeId:
                var employeeId = Convert.ToInt32(id);
                return await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
                {
                    var rslt = await (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate)
                                      join d in ctx.Demographics on t.Ssn equals d.Ssn
                                      where d.EmployeeId == employeeId
                                      select new BalanceEndpointResponse { Id = id, Ssn = t.Ssn.MaskSsn(), CurrentBalance = t.CurrentBalance, Etva = t.Etva, TotalDistributions = t.TotalDistributions, VestedBalance = t.VestedBalance, VestingPercent = t.VestingPercent }).FirstOrDefaultAsync();
                    return rslt;
                });

            default: //SSN
                var ssn = Convert.ToInt32(id);
                return await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
                {
                    var rslt = await (from t in TotalVestingBalance(ctx, profitYear, calendarInfo.FiscalEndDate) where t.Ssn == ssn 
                                      select new BalanceEndpointResponse { Id = id, Ssn = t.Ssn.MaskSsn(), CurrentBalance = t.CurrentBalance, Etva = t.Etva, TotalDistributions = t.TotalDistributions, VestedBalance =  t.VestedBalance, VestingPercent = t.VestingPercent}).FirstOrDefaultAsync();
                    return rslt;
                });
                
        }
    }
}
