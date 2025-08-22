using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                             TerminationCodeId = d.TerminationCodeId
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

    public async Task<ReportResponseBase<AdhocTerminatedEmployeeResponse>> GetTerminatedEmployeesNeedingFormLetter(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        
        var rslt = await _profitSharingDataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographic = await _demographicReaderService.BuildDemographicQuery(ctx, false /*Want letter to be sent to the most current address*/);
            var query = (from d in demographic.Include(x=>x.Address)
                         where d.TerminationDate != null
                            && d.TerminationDate.Value >= req.BeginningDate && d.TerminationDate.Value <= req.EndingDate
                            && d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                            && d.TerminationCodeId != TerminationCode.Constants.Retired
                         /*TODO : Exclude employees who have already been sent a letter?*/
                         /*Filter for employees who are not fully vested, and probably have a balance */
                         select new AdhocTerminatedEmployeeResponse
                         {
                             BadgeNumber = d.BadgeNumber,
                             FullName = d.ContactInfo.FullName != null ? d.ContactInfo.FullName : string.Empty,
                             Ssn = d.Ssn.MaskSsn(),
                             Address = d.Address.Street,
                             Address2 = !string.IsNullOrEmpty(d.Address.Street2) ? d.Address.Street2 : string.Empty,
                             City = !string.IsNullOrEmpty(d.Address.City) ? d.Address.City : string.Empty,
                             State = !string.IsNullOrEmpty(d.Address.State) ? d.Address.State : string.Empty,
                             PostalCode = !string.IsNullOrEmpty(d.Address.PostalCode) ? d.Address.PostalCode : string.Empty,
                             TerminationDate = d.TerminationDate!.Value,
                             TerminationCodeId = d.TerminationCodeId
                         }).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return await query;
        });

        return new ReportResponseBase<AdhocTerminatedEmployeeResponse>()
        {
            ReportName = "Adhoc Terminated Employee Report",
            ReportDate = DateTimeOffset.Now,
            StartDate = req.BeginningDate,
            EndDate = req.EndingDate,
            Response = rslt
        };
    }
}
