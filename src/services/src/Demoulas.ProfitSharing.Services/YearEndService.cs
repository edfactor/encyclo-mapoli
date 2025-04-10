using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services;
public sealed class YearEndService: IYearEndService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly ICalendarService _calendar;
    private readonly IPayProfitUpdateService _payProfitUpdateService;

    public YearEndService(IProfitSharingDataContextFactory profitSharingDataContextFactory, ICalendarService calendar, IPayProfitUpdateService payProfitUpdateService)
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _calendar = calendar;
        _payProfitUpdateService = payProfitUpdateService;
    }
    public async Task RunFinalYearEndUpdates(short profitYear, CancellationToken ct)
    {
        var calendarInfo = await _calendar.GetYearStartAndEndAccountingDatesAsync(profitYear, ct);
        await _profitSharingDataContextFactory.UseWritableContext(async ctx =>
        {
            var under21Cutoff = calendarInfo.FiscalEndDate.AddYears(- ReferenceData.MinimumAgeForContribution());
            var over65 = calendarInfo.FiscalEndDate.AddYears(- 65);

            //Set zero contribution reason to 1 for employees under 21
            var empsUnder21 = ctx.PayProfits.Where(x => x.ProfitYear == profitYear && x.Demographic!.DateOfBirth < under21Cutoff);

            await _payProfitUpdateService.SetZeroContributionReason(empsUnder21, ZeroContributionReason.Constants.Under21WithOver1Khours, ct);

            //Set zero contribution reason to 0 for employees over 65 who formally had alternate zero contributions
            var empsOver65WithNonNormalZeroReason = ctx.PayProfits.Where(x =>
                x.ProfitYear == profitYear &&
                x.Demographic!.DateOfBirth >= over65 &&
                x.ZeroContributionReasonId > ZeroContributionReason.Constants.TerminatedEmployeeOver1000HoursWorkedGetsYearVested &&
                x.ZeroContributionReasonId != ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested);

            await _payProfitUpdateService.SetZeroContributionReason(empsOver65WithNonNormalZeroReason, ZeroContributionReason.Constants.Normal, ct);

            // Calculate points for eligible employees
            var eligibleEmployees = from e in EligibilityService.GetEligibleEmployeesForPoints(ctx, profitYear, calendarInfo.FiscalBeginDate, calendarInfo.FiscalEndDate)
                                    join pp in ctx.PayProfits on new { DemographicId = e.Id, ProfitYear = profitYear } equals new { pp.DemographicId, pp.ProfitYear }
                                    select pp;

            foreach (var pp in eligibleEmployees) //Doing this via materialized list so we can control the rounding.
            {
                pp.PointsEarned = Math.Round((pp.IncomeExecutive + pp.CurrentIncomeYear) / 100.0m, MidpointRounding.AwayFromZero);
            }

            _ = await ctx.SaveChangesAsync(ct);
        }, ct);
    }

    public Task UpdateEnrollmentId(short profitYear, CancellationToken ct)
    {
        return _payProfitUpdateService.SetEnrollmentId(profitYear, ct);
    }
}
