using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services;
public static class EligibilityService
{
    public static IQueryable<Demographic> GetEligibleEmployeesForPoints(IProfitSharingDbContext ctx, short profitYear, DateOnly beginDate, DateOnly endDate)
    {
        var under21 = endDate.AddYears(-ReferenceData.MinimumAgeForContribution());
        var over64 = endDate.AddYears(-64);

        return from pp in ctx.PayProfits.Include(x => x.Demographic)
               where pp.ProfitYear == profitYear &&
                     (
                        (pp.Demographic!.DateOfBirth >= under21 && (pp.HoursExecutive + pp.CurrentHoursYear) >= ReferenceData.MinimumHoursForContribution()) ||
                        (pp.Demographic!.DateOfBirth >= over64 && (pp.HoursExecutive + pp.CurrentHoursYear) >= 0)
                     ) &&
                     (
                        pp.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                        (pp.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated && pp.Demographic!.TerminationDate >= beginDate && pp.Demographic!.TerminationDate < endDate)
                     )
               select pp.Demographic;
    }
}
