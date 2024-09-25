using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Services.Reports;
public class WagesService : IWagesService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
 
    public WagesService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    public async Task<ReportResponseBase<WagesCurrentYearResponse>> GetWagesCurrentYearReport(PaginationRequestDto request, CancellationToken cancellationToken)
    {
       var result = _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.PayProfits
                .Where(p => p.IncomeCurrentYear != 0)
                .Select(p => new WagesCurrentYearResponse { BadgeNumber = p.BadgeNumber, HoursCurrentYear = p.HoursCurrentYear ?? 0,  IncomeCurrentYear = p.IncomeCurrentYear ?? 0 })
                .ToPaginationResultsAsync(request, cancellationToken);

        });

        return new ReportResponseBase<WagesCurrentYearResponse>
        {
            ReportName = "EJR PROF-DOLLAR-EXTRACT YEAR=THIS",
            ReportDate = DateTimeOffset.Now,
            Response = await result
        };
    }

    public async Task<ReportResponseBase<WagesPreviousYearResponse>> GetWagesPreviousYearReport(PaginationRequestDto request, CancellationToken cancellationToken)
    {
        var result = _dataContextFactory.UseReadOnlyContext(c =>
        {
            return c.PayProfits
                .Where(p => p.IncomeCurrentYear != 0)
                .Select(p => new WagesPreviousYearResponse { BadgeNumber = p.BadgeNumber, HoursLastYear = p.HoursLastYear, IncomeLastYear = p.IncomeLastYear })
                .ToPaginationResultsAsync(request, cancellationToken);

        });

        return new ReportResponseBase<WagesPreviousYearResponse>
        {
            ReportName = "EJR PROF-DOLLAR-EXTRACT YEAR=LAST",
            ReportDate = DateTimeOffset.Now,
            Response = await result
        };
    }
}
