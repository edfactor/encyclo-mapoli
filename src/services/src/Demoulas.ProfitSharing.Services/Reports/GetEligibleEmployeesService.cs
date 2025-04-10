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
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public GetEligibleEmployeesService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    public async Task<GetEligibleEmployeesResponse> GetEligibleEmployeesAsync(ProfitYearRequest request,
        CancellationToken cancellationToken)
    {
        CalendarResponseDto response =
            await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        DateOnly birthDateOfExactly21YearsOld = response.FiscalEndDate.AddYears(-21);
        short hoursWorkedRequirement = ContributionService.MinimumHoursForContribution();

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var baseQuery = ctx.PayProfits.Where(p => p.ProfitYear == request.ProfitYear)
                .Join(
                    FrozenService.GetDemographicSnapshot(ctx, request.ProfitYear),
                    pp => pp.DemographicId,
                    d => d.Id,
                    (pp, d) => new
                    {
                        d.OracleHcmId,
                        Hours = pp.CurrentHoursYear + pp.HoursExecutive,
                        d.DateOfBirth,
                        d.EmploymentStatusId,
                        d.BadgeNumber,
                        d.ContactInfo.FullName,
                        d.DepartmentId,
                        DepartmentName = d.Department!.Name,
                        d.StoreNumber
                    });

            int numberReadOnFrozen = await baseQuery.CountAsync(cancellationToken);

            int numberNotSelected = await baseQuery.Where(e =>
                e.DateOfBirth > birthDateOfExactly21YearsOld /*too young*/
                || e.Hours < hoursWorkedRequirement
                || e.EmploymentStatusId == EmploymentStatus.Constants.Terminated).CountAsync(cancellationToken);

            PaginatedResponseDto<EligibleEmployee> result = await baseQuery
                .Where(e => e.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/ &&
                            e.Hours >= hoursWorkedRequirement && e.EmploymentStatusId !=
                            EmploymentStatus.Constants.Terminated)
                .Select(e => new EligibleEmployee
                {
                    OracleHcmId = e.OracleHcmId,
                    BadgeNumber = e.BadgeNumber,
                    FullName = e.FullName!,
                    DepartmentId = e.DepartmentId,
                    Department = e.DepartmentName,
                    StoreNumber = e.StoreNumber
                }).ToPaginationResultsAsync(request, cancellationToken);

            return new GetEligibleEmployeesResponse
            {
                ReportName = $"Get Eligible Employees for Year {request.ProfitYear}",
                ReportDate = DateTimeOffset.Now,
                Response = result,
                NumberReadOnFrozen = numberReadOnFrozen,
                NumberNotSelected = numberNotSelected,
                NumberWritten = result.Results.Count()
            };
        });
    }
}
