using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class TerminationAndRehireService : ITerminationAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly TotalService _totalService;
    private readonly ILoggerFactory _factory;
    private readonly ILogger<TerminationAndRehireService> _logger;

    public TerminationAndRehireService(
        IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService,
        ILoggerFactory factory)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _factory = factory;
        _logger = factory.CreateLogger<TerminationAndRehireService>();
    }

    /// <summary>
    /// Generates a report of employees who are on military leave.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the report response with details of employees on military leave.</returns>
    public async Task<ReportResponseBase<EmployeesOnMilitaryLeaveResponse>> GetMilitaryAndRehireReportAsync(PaginationRequestDto req, CancellationToken cancellationToken)
    {
        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var inactiveMilitaryMembers = await context.Demographics.Where(d => d.TerminationCodeId == TerminationCode.Constants.Military
                                                                                && d.EmploymentStatusId == EmploymentStatus.Constants.Inactive)
                .OrderBy(d => d.ContactInfo.FullName)
                .Select(d => new EmployeesOnMilitaryLeaveResponse
                {
                    DepartmentId = d.DepartmentId,
                    BadgeNumber = d.BadgeNumber,
                    Ssn = d.Ssn.MaskSsn(),
                    FullName = d.ContactInfo.FullName,
                    DateOfBirth = d.DateOfBirth,
                    TerminationDate = d.TerminationDate
                })
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return inactiveMilitaryMembers;
        });

        return new ReportResponseBase<EmployeesOnMilitaryLeaveResponse>
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
    public async Task<ReportResponseBase<RehireForfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(RehireForfeituresRequest req, CancellationToken cancellationToken)
    {
        var validator = new RehireForfeituresRequestValidator(_calendarService, _factory);
        await validator.ValidateAndThrowAsync(req, cancellationToken);
        _logger.LogInformation("Finding rehires with forfeitures for profit year {ProfitYear}", req.ProfitYear);

        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = await GetRehireProfitQueryBase(context, req, cancellationToken);
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
                    new RehireForfeituresResponse
                    {
                        BadgeNumber = group.Key.BadgeNumber,
                        Ssn = group.Key.Ssn.MaskSsn(),
                        FullName = group.Key.FullName,
                        HoursCurrentYear = group.Key.HoursCurrentYear,
                        ReHiredDate = group.Key.ReHiredDate,
                        CompanyContributionYears = group.Key.CompanyContributionYears,
                        Details = group.Select(pd => new MilitaryRehireProfitSharingDetailResponse
                        {
                            Forfeiture = pd.Forfeiture,
                            Remark = pd.Remark,
                            ProfitYear = pd.ProfitYear
                        })
                    })
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
        });
        return new ReportResponseBase<RehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.Now,
            Response = militaryMembers
        };
    }

    public async Task<ReportResponseBase<MilitaryAndRehireProfitSummaryResponse>> GetMilitaryAndRehireProfitSummaryReportAsync(RehireForfeituresRequest req, CancellationToken cancellationToken)
    {
        var validator = new RehireForfeituresRequestValidator(_calendarService, _factory);
        await validator.ValidateAndThrowAsync(req, cancellationToken);

        _logger.LogInformation("Generating military and rehire profit summary report for profit year {ProfitYear}", req.ProfitYear);

        var bracket = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            short lastYear = (short)(req.ProfitYear - 1);
            var query = (
                from b in await GetRehireProfitQueryBase(context, req, cancellationToken)
                join yipTbl in _totalService.GetYearsOfService(context, (short)req.EndingDate.Year) on b.Ssn equals yipTbl.Ssn into yipTmp
                from yip in yipTmp.DefaultIfEmpty()
                join lyBalTbl in _totalService.TotalVestingBalance(context, lastYear, bracket.FiscalEndDate) on b.Ssn equals lyBalTbl.Ssn into lyBalTmp
                from lyBal in lyBalTmp.DefaultIfEmpty()
                select new
                {
                    b.BadgeNumber,
                    b.FullName,
                    b.Ssn,
                    b.HireDate,
                    b.TerminationDate,
                    b.ReHiredDate,
                    b.StoreNumber,
                    CompanyContributionYears = yip.Years,
                    b.EnrollmentId,
                    b.HoursCurrentYear,
                    NetBalanceLastYear = lyBal.VestedBalance,
                    VestedBalanceLastYear = lyBal.CurrentBalance,
                    b.EmploymentStatusId,
                    b.Forfeiture,
                    b.Remark,
                    b.ProfitYear,
                    b.ProfitCodeId
                }
            );
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
            ReportName = "TERM-REHIRE",
            ReportDate = DateTimeOffset.Now,
            Response = militaryMembers
        };
    }

    private async Task<IQueryable<MilitaryAndRehireProfitSummaryQueryResponse>> GetRehireProfitQueryBase(ProfitSharingReadOnlyDbContext context,
        RehireForfeituresRequest req, CancellationToken cancellationToken)
    {
        var bracket = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        var beginning = req.BeginningDate.ToDateOnly(DateTimeKind.Local) > bracket.FiscalBeginDate ? bracket.FiscalBeginDate : req.BeginningDate.ToDateOnly(DateTimeKind.Local);
        var ending = req.EndingDate.ToDateOnly(DateTimeKind.Local) > bracket.FiscalEndDate ? bracket.FiscalEndDate : req.EndingDate.ToDateOnly(DateTimeKind.Local);

        var query = context.Demographics
            .Join(
                context.PayProfits.Where(x => x.ProfitYear == req.ProfitYear), // Table to join with (PayProfit)
                demographics => demographics.Id, // Primary key selector from Demographics
                payProfit => payProfit.DemographicId, // Foreign key selector from PayProfit
                (demographics, payProfit) => new // Result selector after joining
                {
                    demographics.BadgeNumber,
                    demographics.ContactInfo.FullName,
                    demographics.Ssn,
                    demographics.HireDate,
                    demographics.TerminationDate,
                    demographics.ReHireDate,
                    demographics.StoreNumber,
                    payProfit.EnrollmentId,
                    payProfit.CurrentHoursYear,
                    demographics.EmploymentStatusId
                }
            )
            .Where(m =>
                m.EmploymentStatusId == EmploymentStatus.Constants.Active
                && m.ReHireDate != null
                && m.ReHireDate >= beginning
                && m.ReHireDate <= ending)
            .Join(
                context.ProfitDetails.Where(x => x.ProfitYear == req.ProfitYear), // Table to join with (ProfitDetail)
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
                    member.EnrollmentId,
                    member.CurrentHoursYear,
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
                ReHiredDate = d.ReHireDate ?? SqlDateTime.MinValue.Value.ToDateOnly(DateTimeKind.Local),
                StoreNumber = d.StoreNumber,
                CompanyContributionYears = 0, //Filled out in detail report
                EnrollmentId = d.EnrollmentId,
                HoursCurrentYear = d.CurrentHoursYear,
                NetBalanceLastYear = 0, //Filled out in detail report
                VestedBalanceLastYear = 0, //Filled out in detail report
                EmploymentStatusId = d.EmploymentStatusId,
                Forfeiture = d.Forfeiture,
                Remark = d.Remark,
                ProfitYear = d.ProfitYear,
                ProfitCodeId = d.ProfitCodeId
            });
        return query;
    }
}
