using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;
public class WagesService : IWagesService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
 
    public WagesService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public async Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesReport(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var result = _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.PayProfits
                .Include(p => p.Demographic)
                .Where(p => p.CurrentIncomeYear != 0 && p.ProfitYear == request.ProfitYear)
                .Select(p => new WagesCurrentYearResponse
                {
                    BadgeNumber = p.Demographic!.BadgeNumber, HoursCurrentYear = p.CurrentHoursYear, IncomeCurrentYear = p.CurrentIncomeYear
                })
                .ToPaginationResultsAsync(request, cancellationToken);

        });

        return new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = $"EJR PROF-DOLLAR-EXTRACT YEAR={request.ProfitYear}",
            ReportDate = DateTimeOffset.Now,
            Response = await result
        };
    }
}
