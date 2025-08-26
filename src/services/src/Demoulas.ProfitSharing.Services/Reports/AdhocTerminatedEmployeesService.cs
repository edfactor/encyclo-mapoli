using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;

namespace Demoulas.ProfitSharing.Services.Reports;
public class AdhocTerminatedEmployeesService : IAdhocTerminatedEmployeesService
{
    private readonly IProfitSharingDataContextFactory _profitSharingDataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ICalendarService _calendarService;

    public AdhocTerminatedEmployeesService(
        IProfitSharingDataContextFactory profitSharingDataContextFactory,
        IDemographicReaderService demographicReaderService,
        ICalendarService calendarService
    )
    {
        _profitSharingDataContextFactory = profitSharingDataContextFactory;
        _demographicReaderService = demographicReaderService;
        _calendarService = calendarService;
    }

    public async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployees(FrozenProfitYearRequest req, CancellationToken cancellationToken)
    {
        var startDate = DateOnly.MinValue;
        var endDate = DateOnly.MaxValue;

        var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, req.UseFrozenData);
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear);
            var query = (from d in demographic
                         where d.TerminationDate != null
                            && d.TerminationDate.Value <= calInfo.FiscalEndDate
                            && d.TerminationCodeId != TerminationCode.Constants.Retired
                         select new AdhocTerminatedEmployeeResponse
                         {
                             BadgeNumber = d.BadgeNumber,
                             FullName = d.ContactInfo.FullName != null ? d.ContactInfo.FullName : string.Empty,
                             Ssn = d.Ssn.MaskSsn(),
                             TerminationDate = d.TerminationDate!.Value,
                             TerminationCodeId = d.TerminationCodeId,
                             IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly
                         }).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            startDate = calInfo.FiscalBeginDate;
            endDate = calInfo.FiscalEndDate;
            return await query;
        });

        return new ReportResponseBase<AdhocTerminatedEmployeeResponse>()
        {
            ReportName = "Adhoc Terminated Employee Report",
            ReportDate = DateTimeOffset.Now,
            StartDate = startDate,
            EndDate = endDate,
            Response = rslt
        };
    }
}
