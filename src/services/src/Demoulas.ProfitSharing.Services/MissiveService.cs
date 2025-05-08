using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response.Lookup;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services;
internal sealed class MissiveService : IMissiveService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ITotalService _totalService;
    private readonly ICalendarService _calendarService;
    private readonly ILogger<MissiveService> _logger;

    public MissiveService(
        IProfitSharingDataContextFactory dataContextFactory,
        ITotalService totalService,
        ILoggerFactory loggerFactory,
        ICalendarService calendarService
    ) 
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
        _logger = loggerFactory.CreateLogger<MissiveService>();
    }
    public async Task<List<int>> DetermineMissivesForSsn(int ssn, short profitYear, CancellationToken cancellation)
    {
        using (_logger.BeginScope("Searching for missives for ssn {0}", ssn.MaskSsn()))
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, cancellation);

            var rslt = new List<int>();
            _ = await _dataContextFactory.UseReadOnlyContext(async ctx =>
            {

                if (await HasNewVestingPlanHasContributions(ctx, ssn, profitYear, cancellation))
                {
                    rslt.Add(Missive.Constants.VestingIncreasedOnCurrentBalance);
                }

                if (await VestingIsNow100Percent(ctx, ssn, calInfo, cancellation))
                {
                    rslt.Add(Missive.Constants.VestingIsNow100Percent);
                }

                if (await EmployeeIsAlsoABeneficiary(ctx, ssn, cancellation))
                {
                    rslt.Add(Missive.Constants.EmployeeIsAlsoABeneficiary);
                }

                if (await EmployeeMayBe100Percent(ctx, ssn, profitYear, cancellation))
                {
                    rslt.Add(Missive.Constants.EmployeeMayBe100Percent);
                }

                return Task.FromResult(true);
            }).Unwrap();

            return rslt;
        }
    }

    public Task<List<MissiveResponse>> GetAllMissives(CancellationToken token)
    {
        return _dataContextFactory.UseReadOnlyContext(ctx =>
        {
            return ctx.Missives.Select(x => new MissiveResponse() { Id = x.Id, Message = x.Message, Description = x.Description, Severity = x.Severity}).ToListAsync(token);
        });
    }

    private async Task<bool> HasNewVestingPlanHasContributions(ProfitSharingReadOnlyDbContext ctx,int ssn, short profitYear, CancellationToken cancellation)
    {
        var memberBalance = await _totalService.GetVestingBalanceForSingleMemberAsync(Common.Contracts.Request.SearchBy.Ssn, ssn, profitYear, cancellation);
        if (memberBalance != null && memberBalance.YearsInPlan >= 2 && memberBalance.YearsInPlan <= 7)
        {
            var vestingIncreased = await (
                from d in ctx.Demographics
                join pp in ctx.PayProfits.Where(x => x.ProfitYear == profitYear) on d.Id equals pp.DemographicId
                where d.Ssn == ssn
                  && pp.CurrentHoursYear + pp.HoursExecutive > ReferenceData.MinimumHoursForContribution()
                  && pp.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                select d.Id

            ).AnyAsync(cancellation);
            return true;
        }

        return false;
    }

    private static Task<bool> VestingIsNow100Percent(ProfitSharingReadOnlyDbContext ctx, int ssn, CalendarResponseDto calInfo, CancellationToken cancellation)
    {
        var sixtyFiveBirthDate = DateOnly.FromDateTime(DateTime.Today).AddYears(-65);
        return (from d in ctx.Demographics
                where d.Ssn == ssn && 
                      d.DateOfBirth <= sixtyFiveBirthDate && 
                      (!d.TerminationDate.HasValue || d.TerminationDate > calInfo.FiscalEndDate) &&
                      d.TerminationCodeId != TerminationCode.Constants.Deceased
                select d.Id).AnyAsync(cancellation);
    }

    private static Task<bool> EmployeeIsAlsoABeneficiary(ProfitSharingReadOnlyDbContext ctx, int ssn, CancellationToken token)
    {
        return (from bc in ctx.BeneficiaryContacts where bc.Ssn == ssn select bc.Id).AnyAsync(token);
    }

    private static Task<bool> EmployeeMayBe100Percent(ProfitSharingReadOnlyDbContext ctx, int ssn, short profitYear, CancellationToken token) 
    {
        return (from pp in ctx.PayProfits.Where(x => x.ProfitYear == profitYear)
                join d in ctx.Demographics on pp.DemographicId equals d.Id
                where pp.ZeroContributionReasonId == ZeroContributionReason.Constants.SixtyFourFirstContributionMoreThan5YearsAgo100PercentVestedOnBirthDay
                    && d.Ssn == ssn
                select d.Id).AnyAsync(token);
    }
}
