using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class MilitaryAndRehireService : IMilitaryAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;

    public MilitaryAndRehireService(IProfitSharingDataContextFactory dataContextFactory)
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

    /// <summary>
    /// Finds rehires who may be entitled to forfeitures taken out in prior years.
    /// </summary>
    /// <param name="req">The pagination request containing the necessary parameters for the search.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a report response with the rehire profit sharing data.</returns>
    public async Task<ReportResponseBase<MilitaryAndRehireForfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYears(PaginationRequestDto req, CancellationToken cancellationToken)
    {
        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(context =>
        {
            var query = context.Demographics
                .Join(
                    context.PayProfits, // Table to join with (PayProfit)
                    demographics => demographics.Ssn, // Primary key selector from Demographics
                    payProfit => payProfit.Ssn, // Foreign key selector from PayProfit
                    (demographics, payProfit) => new // Result selector after joining
                    {
                        demographics.BadgeNumber,
                        demographics.FullName,
                        demographics.Ssn,
                        demographics.ReHireDate,
                        payProfit.CompanyContributionYears,
                        payProfit.EnrollmentId,
                        payProfit.HoursCurrentYear,
                        demographics.EmploymentStatusId
                    }
                )
                .Where(m =>
                    m.EmploymentStatusId == EmploymentStatus.Constants.Active
                    && m.ReHireDate != null
                    && m.ReHireDate > new DateOnly(2000, 01, 01))
                .Join(
                    context.ProfitDetails, // Table to join with (ProfitDetail)
                    combined => combined.Ssn, // Key selector from the result of the first join
                    profitDetail => profitDetail.Ssn, // Foreign key selector from ProfitDetail
                    (member, profitDetail) => new // Result selector after joining ProfitDetail
                    {
                        member.BadgeNumber,
                        member.FullName,
                        member.Ssn,
                        member.ReHireDate,
                        member.CompanyContributionYears,
                        member.EnrollmentId,
                        member.HoursCurrentYear,
                        profitDetail.Forfeiture,
                        profitDetail.Remark,
                        profitDetail.ProfitYear,
                        profitDetail.ProfitCodeId
                    }
                )
                .Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id)
                .OrderBy(m => m.BadgeNumber)
                .GroupBy(m => new
                {
                    m.BadgeNumber,
                    m.FullName,
                    m.Ssn,
                    m.ReHireDate,
                    m.CompanyContributionYears,
                    m.HoursCurrentYear
                }) // Group by employee details
                .Select(group =>
                    new MilitaryAndRehireForfeituresResponse
                    {
                        BadgeNumber = group.Key.BadgeNumber,
                        Ssn = group.Key.Ssn.MaskSsn(),
                        FullName = group.Key.FullName,
                        HoursCurrentYear = group.Key.HoursCurrentYear ?? 0,
                        ReHiredDate = group.Key.ReHireDate ?? SqlDateTime.MinValue.Value.ToDateOnly(),
                        CompanyContributionYears = group.Key.CompanyContributionYears,
                        Details = group.Select(pd => new MilitaryRehireProfitSharingDetailResponse
                        {
                            Forfeiture = pd.Forfeiture, Remark = pd.Remark, ProfitYear = pd.ProfitYear
                        }).ToList()
                    });
            
            return query.ToPaginationResultsAsync(req, cancellationToken);
        });

        return new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = militaryMembers
        };
    }
}
