using System.Data.SqlTypes;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class TerminationAndRehireService : ITerminationAndRehireService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;

    public TerminationAndRehireService(
        IProfitSharingDataContextFactory dataContextFactory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _totalService = totalService;
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
                    TerminationDate = d.TerminationDate
                })
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);

            return inactiveMilitaryMembers;
        });

        return new ReportResponseBase<EmployeesOnMilitaryLeaveResponse>
        {
            ReportName = "EMPLOYEES ON MILITARY LEAVE",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = ReferenceData.DsmMinValue(),
            EndDate = DateTimeOffset.UtcNow.ToDateOnly(),
            Response = militaryMembers
        };
    }

    /// <summary>
    /// Finds rehires who may be entitled to forfeitures taken out in prior years.
    /// </summary>
    /// <param name="req">The pagination request containing the necessary parameters for the search.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a report response with the rehire profit sharing data.</returns>
    public async Task<ReportResponseBase<RehireForfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        var validator = new StartAndEndDateRequestValidator();
        await validator.ValidateAndThrowAsync(req, cancellationToken);
        

        var militaryMembers = await _dataContextFactory.UseReadOnlyContext(async context =>
        {
            var query = await GetRehireProfitQueryBase(context, req);
            return await query.Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
                .GroupBy(m => new
                {
                    m.BadgeNumber,
                    m.FullName,
                    m.Ssn,
                    m.HireDate,
                    m.TerminationDate,
                    m.ReHiredDate,
                    m.CompanyContributionYears,
                    m.HoursCurrentYear,
                    m.StoreNumber,
                    m.NetBalanceLastYear,
                    m.VestedBalanceLastYear,
                    m.EnrollmentId,
                    m.EnrollmentName,
                    m.EmploymentStatus
                }).Select(group =>
                    new RehireForfeituresResponse
                    {
                        BadgeNumber = group.Key.BadgeNumber,
                        Ssn = group.Key.Ssn.MaskSsn(),
                        FullName = group.Key.FullName,
                        HireDate = group.Key.HireDate,
                        TerminationDate = group.Key.TerminationDate,
                        HoursCurrentYear = group.Key.HoursCurrentYear,
                        ReHiredDate = group.Key.ReHiredDate,
                        CompanyContributionYears = group.Key.CompanyContributionYears,
                        EnrollmentId = group.Key.EnrollmentId,
                        EnrollmentName = group.Key.EnrollmentName,
                        EmploymentStatus = group.Key.EmploymentStatus,
                        StoreNumber = group.Key.StoreNumber,
                        VestedBalanceLastYear = group.Key.VestedBalanceLastYear,
                        NetBalanceLastYear =  group.Key.NetBalanceLastYear,
                        Details = group.Select(pd => new MilitaryRehireProfitSharingDetailResponse
                        {
                            Forfeiture = pd.Forfeiture,
                            Remark = pd.Remark,
                            ProfitYear = pd.ProfitYear
                        })
                    })
                .OrderBy(x=> x.BadgeNumber)
                .ThenByDescending(x=> x.ReHiredDate)
                .ToPaginationResultsAsync(req, cancellationToken: cancellationToken);
        });

        return new ReportResponseBase<RehireForfeituresResponse>
        {
            ReportName = "REHIRE'S PROFIT SHARING DATA",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = req.BeginningDate,
            EndDate = req.EndingDate,
            Response = militaryMembers
        };
    }

    private async Task<IQueryable<RehireProfitSummaryQuery>> GetRehireProfitQueryBase(ProfitSharingReadOnlyDbContext context,
        StartAndEndDateRequest req)
    {
        var beginning = req.BeginningDate;
        var ending = req.EndingDate;

        var yearsOfServiceQuery = _totalService.GetYearsOfService(context, (short)req.EndingDate.Year);
        var demo = await _demographicReaderService.BuildDemographicQuery(context);
        
        var query = demo
            .Join(
                context.PayProfits.Include(e=> e.Enrollment)
                    .Where(x => x.ProfitYear >= beginning.Year && x.ProfitYear <= ending.Year), // Table to join with (PayProfit)
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
                    payProfit.Enrollment,
                    payProfit.CurrentHoursYear,
                    demographics.EmploymentStatusId,
                    EmploymentStatus = demographics.EmploymentStatus!.Name
                }
            )
            .Where(m =>
                m.EmploymentStatusId == EmploymentStatus.Constants.Active
                && m.ReHireDate != null
                && m.ReHireDate >= beginning
                && m.ReHireDate <= ending)
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
                    member.EnrollmentId,
                    member.CurrentHoursYear,
                    profitDetail.Forfeiture,
                    profitDetail.Remark,
                    profitDetail.ProfitYear,
                    profitDetail.ProfitCodeId,
                    member.Enrollment,
                    member.EmploymentStatusId,
                    member.EmploymentStatus
                }
            )
            .GroupJoin(
                yearsOfServiceQuery,
                member => member.Ssn,
                yip => yip.Ssn,
                (member, yipGroup) => new { member, yipGroup }
            )
            .SelectMany(
                temp => temp.yipGroup.DefaultIfEmpty(),
                (temp, yip) => new 
                {
                    temp.member,
                    yip
                }
            )
            .GroupJoin(
                _totalService.TotalVestingBalance(context, (short)(beginning.Year -1), ending),
                member => member.member.Ssn,
                tot => tot.Ssn,
                (member, tot) => new { member, tot}
            )
            .SelectMany(
                temp => temp.tot.DefaultIfEmpty(),
                (temp, tot) => new RehireProfitSummaryQuery
                {
                    BadgeNumber = temp.member.member.BadgeNumber,
                    FullName = temp.member.member.FullName,
                    Ssn = temp.member.member.Ssn,
                    HireDate = temp.member.member.HireDate,
                    TerminationDate = temp.member.member.TerminationDate,
                    ReHiredDate = temp.member.member.ReHireDate ?? ReferenceData.DsmMinValue(),
                    StoreNumber = temp.member.member.StoreNumber,
                    CompanyContributionYears = temp.member.yip!.Years ?? 0,
                    EnrollmentId = temp.member.member.EnrollmentId,
                    EnrollmentName = temp.member.member.Enrollment!.Name,
                    HoursCurrentYear = temp.member.member.CurrentHoursYear,
                    NetBalanceLastYear = tot != null ? tot.CurrentBalance ?? 0 : 0m,
                    VestedBalanceLastYear = tot != null ? tot.VestedBalance ?? 0 : 0m,
                    EmploymentStatusId = temp.member.member.EmploymentStatusId,
                    EmploymentStatus = temp.member.member.EmploymentStatus,
                    Forfeiture = temp.member.member.Forfeiture,
                    Remark = temp.member.member.Remark,
                    ProfitYear = temp.member.member.ProfitYear,
                    ProfitCodeId = temp.member.member.ProfitCodeId
                }
            )
            .OrderBy(m => m.BadgeNumber)
            .ThenBy(m => m.FullName);

        return query;
    }

}
