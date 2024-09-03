using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class CalendarService : IMilitaryAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public CalendarService(IProfitSharingDataContextFactory dataContextFactory)
    {
        _dataContextFactory = dataContextFactory;
    }

    /// <summary>
    /// Generates a report of employees who are on military leave and have been rehired.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave.</returns>
    public async Task<ReportResponseBase<MilitaryAndRehireReportResponse>> GetMilitaryAndRehireReport(PaginationRequestDto req, CancellationToken cancellationToken)
    {
        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var inactiveMilitaryMembers = await context.Demographics.Where(d => d.TerminationCodeId == TerminationCode.Constants.Military
                                                                                && d.EmploymentStatusId == EmploymentStatus.Constants.Inactive)
                .OrderBy(d => d.FullName)
                .Select(d => new MilitaryAndRehireReportResponse
                {
                    DepartmentId = d.DepartmentId,
                    BadgeNumber = d.BadgeNumber,
                    Ssn = d.Ssn.MaskSsn(),
                    FullName = d.FullName,
                    DateOfBirth = d.DateOfBirth,
                    TerminationDate = d.TerminationDate
                })
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return inactiveMilitaryMembers;
        });

        return new ReportResponseBase<MilitaryAndRehireReportResponse>
        {
            ReportName = "EMPLOYEES ON MILITARY LEAVE",
            ReportDate = DateTimeOffset.Now,
            Response = militaryMembers
        };
    }
}
