using System.Linq.Dynamic.Core;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class GetEligibleEmployeesService : IGetEligibleEmployeesService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;

    public GetEligibleEmployeesService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    public async Task<GetEligibleEmployeesResponse> GetEligibleEmployeesAsync(ProfitYearRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        var birthDateOfExactly21YearsOld = response.FiscalEndDate.AddYears(-21);
        var hoursWorkedRequirement = ContributionService.MinimumHoursForContribution();

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = ctx.PayProfits.AsNoTracking().Where(p => p.ProfitYear == request.ProfitYear)
                .Join(
                    FrozenService.GetDemographicSnapshot(ctx, request.ProfitYear),
                    pp => pp.DemographicId,
                    d => d.Id,
                    (pp, d) => new
                    {
                        d.OracleHcmId,
                        Hours = (pp.CurrentHoursYear + pp.HoursExecutive),
                        d.DateOfBirth,
                        d.EmploymentStatusId,
                        d.BadgeNumber,
                        d.ContactInfo.FullName,
                        d.DepartmentId,
                        DepartmentName = d.Department!.Name
                    });

            var allRecords = await baseQuery.ToListAsync(cancellationToken);

            int numberReadOnFrozen = allRecords.Count;

            int numberNotSelected = allRecords
                .Count(e =>
                    e.DateOfBirth > birthDateOfExactly21YearsOld /*too young*/
                    || e.Hours < hoursWorkedRequirement
                    || e.EmploymentStatusId == EmploymentStatus.Constants.Terminated);

            var result = allRecords
                .Where(e => e.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/ &&
                            e.Hours >= hoursWorkedRequirement && e.EmploymentStatusId !=
                            EmploymentStatus.Constants.Terminated)
                .Select(e => new EligibleEmployee
                {
                    OracleHcmId = e.OracleHcmId,
                    BadgeNumber = e.BadgeNumber,
                    FullName = e.FullName!,
                    DepartmentId = e.DepartmentId,
                    Department = e.DepartmentName
                })
                .OrderBy(p => p.FullName);

            var paginatedResults = new PaginatedResponseDto<EligibleEmployee>(request) { Results = result };

            return new GetEligibleEmployeesResponse
            {
                ReportName = $"Get Eligible Employees for Year {request.ProfitYear}",
                ReportDate = DateTimeOffset.Now,
                Response = paginatedResults,
                NumberReadOnFrozen = numberReadOnFrozen,
                NumberNotSelected = numberNotSelected,
                NumberWritten = result.Count()
            };
        });
    }
}
