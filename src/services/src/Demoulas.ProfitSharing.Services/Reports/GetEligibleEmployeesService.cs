using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using static FastEndpoints.Ep;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class GetEligibleEmployeesService : IGetEligibleEmployeesService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly CalendarService _calendarService;

    public GetEligibleEmployeesService(IProfitSharingDataContextFactory dataContextFactory, CalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
    }

    public async Task<GetEligibleEmployeesResponse> GetEligibleEmployees(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        int NumberReadOnFrozen = 0;
        var yearEndDate = (await _calendarService.GetYearStartAndEndAccountingDates(request.ProfitYear, cancellationToken)).YearEndDate;
        var birthDateOfExactly21YearsOld = yearEndDate.AddYears(-21);

        return  await _dataContextFactory.UseReadOnlyContext(async c =>
        {
            NumberReadOnFrozen = await c.PayProfits.Where(p => p.ProfitYear == request.ProfitYear).CountAsync(cancellationToken);

            int numberNotSelected = await c.PayProfits
            .Include(p => p.Demographic)
            .Where(p => p.ProfitYear == request.ProfitYear)
            .Where(p => p.Demographic!.DateOfBirth > birthDateOfExactly21YearsOld /*too young*/ || p.CurrentHoursYear < 1000 || p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated)
            .CountAsync();

            var totalEligible = await c.PayProfits
           .Include(p => p.Demographic)
           .Where(p => p.ProfitYear == request.ProfitYear)
           .Where(p => p.Demographic!.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/  && p.CurrentHoursYear >= 1000 && p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated).CountAsync(cancellationToken);

            var result = c.PayProfits
                .Include(p => p.Demographic)
                .Where(p =>  p.ProfitYear == request.ProfitYear)
                .Where(p => p.Demographic!.DateOfBirth <= birthDateOfExactly21YearsOld /*over 21*/  && p.CurrentHoursYear >= 1000 && p.Demographic!.EmploymentStatusId != EmploymentStatus.Constants.Terminated)
                .Select(p => new GetEligibleEmployeesResponseDto()
                {
                    OracleHcmId = p.OracleHcmId,
                    BadgeNumber = p.Demographic!.BadgeNumber,
                    FullName = p.Demographic!.ContactInfo.FullName!,
                })
                .OrderBy(p => p.FullName)
                .ToPaginationResultsAsync(request, cancellationToken);

            return new GetEligibleEmployeesResponse
            {
                ReportName = $"Get Eligible Employees for Year {request.ProfitYear}",
                ReportDate = DateTimeOffset.Now,
                Response = await result,
                NumberReadOnFrozen = NumberReadOnFrozen,
                NumberNotSelected = numberNotSelected,
                NumberWritten = totalEligible
            };

        });


    }

}
