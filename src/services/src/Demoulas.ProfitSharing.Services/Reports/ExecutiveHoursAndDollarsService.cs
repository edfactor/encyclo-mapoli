using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class ExecutiveHoursAndDollarsService : IExecutiveHoursAndDollarsService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public ExecutiveHoursAndDollarsService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Generates a list of executives who have hours and dollars for the provided year.
    /// </summary>
    /// <param name="request">The year and pagination details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of executive hours/dollars</returns>
    public async Task<ReportResponseBase<ExecutiveHoursAndDollarsResponse>> GetExecutiveHoursAndDollarsReport(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var result = _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.PayProfits
                .Where(p => (p.HoursExecutive > 0 || p.IncomeExecutive > 0) && p.ProfitYear == request.ProfitYear)
                .Include(p=>p.Demographic)
                .Select(p => new ExecutiveHoursAndDollarsResponse
                {
                    BadgeNumber = p.Demographic!.BadgeNumber,
                    FullName = p.Demographic.FullName,
                    StoreNumber = p.Demographic.StoreNumber,
                    HoursExecutive = p.HoursExecutive,
                    IncomeExecutive = p.IncomeExecutive,
                    CurrentHoursYear = p.CurrentHoursYear ?? 0,
                    CurrentIncomeYear = p.CurrentIncomeYear ?? 0,
                    PayFrequencyId = p.Demographic.PayFrequencyId,
                    EmploymentStatusId = p.Demographic.EmploymentStatusId
                })
                .OrderBy(p => p.StoreNumber)
                .ThenBy(p => p.BadgeNumber)
                .ToPaginationResultsAsync(request, cancellationToken);
        });

        return new ReportResponseBase<ExecutiveHoursAndDollarsResponse>
        {
            ReportName = $"Executive Hours and Dollars for Year {request.ProfitYear}",
            ReportDate = DateTimeOffset.Now,
            Response = await result
        };

    }

}
