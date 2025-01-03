using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
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

    public GetEligibleEmployeesService(IProfitSharingDataContextFactory dataContextFactory, ICalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    public async Task<GetEligibleEmployeesResponse> GetEligibleEmployeesAsync(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var response = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        var birthDateOfExactly21YearsOld = response.FiscalEndDate.AddYears(-21);
        var hoursWorkedRequirement = ReferenceData.MinimumHoursForContribution();

        return  await _dataContextFactory.UseReadOnlyContext(async c =>
        {
            int numberReadOnFrozen = await c.PayProfits.Where(p => p.ProfitYear == request.ProfitYear).CountAsync(cancellationToken);

            int numberNotSelected = await c.PayProfits
            .Include(p => p.Demographic)
            .Where(p => p.ProfitYear == request.ProfitYear)
            .Where(p => p.Demographic!.DateOfBirth > birthDateOfExactly21YearsOld /*too young*/ || p.CurrentHoursYear < hoursWorkedRequirement || p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
            .CountAsync(cancellationToken: cancellationToken);

            var totalEligible = await c.PayProfits
           .Include(p => p.Demographic)
           .Where(p => p.ProfitYear == request.ProfitYear)
           .Where(p => p.Demographic!.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/  && p.CurrentHoursYear >= hoursWorkedRequirement && p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated).CountAsync(cancellationToken);

            var result = await c.PayProfits
                .Include(p => p.Demographic)
                .Where(p =>  p.ProfitYear == request.ProfitYear)
                .Where(p => p.Demographic!.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/  && p.CurrentHoursYear >= hoursWorkedRequirement && p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated)
                .Select(p => new GetEligibleEmployeesResponseDto()
                {
                    OracleHcmId = p.Demographic!.OracleHcmId,
                    BadgeNumber = p.Demographic!.EmployeeId,
                    FullName = p.Demographic!.ContactInfo.FullName!,
                })
                .OrderBy(p => p.FullName)
                .ToPaginationResultsAsync(request, cancellationToken);

            return new GetEligibleEmployeesResponse
            {
                ReportName = $"Get Eligible Employees for Year {request.ProfitYear}",
                ReportDate = DateTimeOffset.Now,
                Response = result,
                NumberReadOnFrozen = numberReadOnFrozen,
                NumberNotSelected = numberNotSelected,
                NumberWritten = totalEligible
            };

        });


    }

}
