using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Extensions;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.Util.Extensions;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class MilitaryAndRehireService : IMilitaryAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly CalendarService _calendarService;

    public MilitaryAndRehireService(IProfitSharingDataContextFactory dataContextFactory, CalendarService calendarService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
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
    public async Task<ReportResponseBase<MilitaryAndRehireForfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYears(FiscalYearRequest req, CancellationToken cancellationToken)
    {
        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = await GetMilitaryAndRehireProfitQueryBase(context, req, cancellationToken);
            return await query.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                .GroupBy(m => new
                {
                    m.BadgeNumber,
                    m.FullName,
                    m.Ssn,
                    m.ReHiredDate,
                    m.CompanyContributionYears,
                    m.HoursCurrentYear
                }).Select(group =>
                    new MilitaryAndRehireForfeituresResponse
                    {
                        BadgeNumber = group.Key.BadgeNumber,
                        Ssn = group.Key.Ssn.MaskSsn(),
                        FullName = group.Key.FullName,
                        HoursCurrentYear = group.Key.HoursCurrentYear,
                        ReHiredDate = group.Key.ReHiredDate,
                        CompanyContributionYears = group.Key.CompanyContributionYears,
                        Details = group.Select(pd => new MilitaryRehireProfitSharingDetailResponse
                        {
                            Forfeiture = pd.Forfeiture, Remark = pd.Remark, ProfitYear = pd.ProfitYear
                        })
                    })
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
        });
        return new ReportResponseBase<MilitaryAndRehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = militaryMembers
        };
    }

    public async Task<ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>> GetMilitaryAndRehireProfitSummaryReport(FiscalYearRequest req, CancellationToken cancellationToken)
    {
        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = await GetMilitaryAndRehireProfitQueryBase(context, req, cancellationToken);
            return await query
                .Select(d => new MilitaryAndRehireProfitSummaryResponse
                {
                    BadgeNumber = d.BadgeNumber,
                    FullName = d.FullName,
                    Ssn = d.Ssn.MaskSsn(),
                    HireDate = d.HireDate,
                    TerminationDate = d.TerminationDate,
                    ReHiredDate = d.ReHiredDate,
                    StoreNumber = d.StoreNumber,
                    CompanyContributionYears = d.CompanyContributionYears,
                    EnrollmentId = d.EnrollmentId,
                    HoursCurrentYear = d.HoursCurrentYear,
                    NetBalanceLastYear = d.NetBalanceLastYear,
                    VestedBalanceLastYear = d.VestedBalanceLastYear,
                    EmploymentStatusId = d.EmploymentStatusId,
                    Forfeiture = d.Forfeiture,
                    Remark = d.Remark,
                    ProfitYear = d.ProfitYear,
                    ProfitCodeId = d.ProfitCodeId
                }).ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
        });

        return new ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>
        {
            ReportName = "MILITARY TERM-REHIRE",
            ReportDate = DateTimeOffset.Now,
            Response = militaryMembers
        };
    }

    private async Task<IQueryable<MilitaryAndRehireProfitSummaryQueryResponse>> GetMilitaryAndRehireProfitQueryBase(ProfitSharingReadOnlyDbContext context,
        FiscalYearRequest req, CancellationToken cancellationToken)
    {
        var bracket = await _calendarService.GetYearStartAndEndAccountingDates(req.ReportingYear, cancellationToken);

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
                    demographics.HireDate,
                    demographics.TerminationDate,
                    demographics.ReHireDate,
                    demographics.StoreNumber,
                    payProfit.CompanyContributionYears,
                    payProfit.EnrollmentId,
                    payProfit.HoursCurrentYear,
                    payProfit.NetBalanceLastYear,
                    payProfit.VestedBalanceLastYear,
                    demographics.EmploymentStatusId
                }
            )
            .Where(m =>
                m.EmploymentStatusId == EmploymentStatus.Constants.Active
                && m.ReHireDate != null
                && m.ReHireDate >= bracket.BeginDate
                && m.ReHireDate <= bracket.YearEndDate)
            .Join(
                context.ProfitDetails, // Table to join with (ProfitDetail)
                combined => combined.Ssn, // Key selector from the result of the first join
                profitDetail => profitDetail.Ssn, // Foreign key selector from ProfitDetail
                (member, profitDetail) => new // Result selector after joining ProfitDetail
                {
                    member.BadgeNumber,
                    member.FullName,
                    member.Ssn,
                    member.HireDate,
                    member.TerminationDate,
                    member.ReHireDate,
                    member.StoreNumber,
                    member.CompanyContributionYears,
                    member.EnrollmentId,
                    member.HoursCurrentYear,
                    member.NetBalanceLastYear,
                    member.VestedBalanceLastYear,
                    member.EmploymentStatusId,
                    profitDetail.Forfeiture,
                    profitDetail.Remark,
                    profitDetail.ProfitYear,
                    profitDetail.ProfitCodeId
                }
            )
            .OrderBy(m => m.BadgeNumber)
            .ThenBy(m => m.FullName)
            .Select(d => new MilitaryAndRehireProfitSummaryQueryResponse
            {
                BadgeNumber = d.BadgeNumber,
                FullName = d.FullName,
                Ssn = d.Ssn,
                HireDate = d.HireDate,
                TerminationDate = d.TerminationDate,
                ReHiredDate = d.ReHireDate ?? SqlDateTime.MinValue.Value.ToDateOnly(),
                StoreNumber = d.StoreNumber,
                CompanyContributionYears = d.CompanyContributionYears,
                EnrollmentId = d.EnrollmentId,
                HoursCurrentYear = d.HoursCurrentYear ?? 0,
                NetBalanceLastYear = d.NetBalanceLastYear,
                VestedBalanceLastYear = d.VestedBalanceLastYear,
                EmploymentStatusId = d.EmploymentStatusId,
                Forfeiture = d.Forfeiture,
                Remark = d.Remark,
                ProfitYear = d.ProfitYear,
                ProfitCodeId = d.ProfitCodeId
            });
        return query;
    }
}
