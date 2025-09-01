using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class TerminationAndRehireService : ITerminationAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;

    public TerminationAndRehireService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
    }

    /// <summary>
    /// Generates a report of employees who are on military leave.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave.</returns>
    public async Task<ReportResponseBase<EmployeesOnMilitaryLeaveResponse>> GetEmployeesOnMilitaryLeaveAsync(PaginationRequestDto req, CancellationToken cancellationToken)
    {
        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(context);
            var inactiveMilitaryMembers = await demographics
                .Where(d => d.TerminationCodeId == TerminationCode.Constants.Military
                                     && d.EmploymentStatusId == EmploymentStatus.Constants.Inactive)
                .OrderBy(d => d.ContactInfo.FullName)
                .Select(d => new EmployeesOnMilitaryLeaveResponse
                {
                    DepartmentId = d.DepartmentId,
                    BadgeNumber = d.BadgeNumber,
                    Ssn = d.Ssn.MaskSsn(),
                    FullName = d.ContactInfo.FullName,
                    DateOfBirth = d.DateOfBirth,
                    TerminationDate = d.TerminationDate,
                    IsExecutive = d.PayFrequencyId == PayFrequency.Constants.Monthly,
                })
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return inactiveMilitaryMembers;
        });

        return new ReportResponseBase<EmployeesOnMilitaryLeaveResponse>
        {
            ReportName = "EMPLOYEES ON MILITARY LEAVE",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = ReferenceData.DsmMinValue,
            EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
            Response = militaryMembers
        };
    }
}
