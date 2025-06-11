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
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;
public sealed class UnForfeitService : IUnForfeitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public UnForfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
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
            return await query.AsEnumerable()
                .GroupBy(m => new
                {
                    m.BadgeNumber,
                    m.FullName,
                    m.Ssn,
                    m.HireDate,
                    m.TerminationDate,
                    m.ReHiredDate,
                    m.NetBalanceLastYear,
                    m.VestedBalanceLastYear,
                    m.CompanyContributionYears,
                    m.StoreNumber,
                    m.EmploymentStatus
                })
                .AsEnumerable() // Switch to client-s
                .Select(group => new RehireForfeituresResponse
                {
                    BadgeNumber = group.Key.BadgeNumber,
                    Ssn = group.Key.Ssn.MaskSsn(),
                    FullName = group.Key.FullName,
                    HireDate = group.Key.HireDate,
                    TerminationDate = group.Key.TerminationDate,
                    ReHiredDate = group.Key.ReHiredDate,
                    VestedBalanceLastYear = group.Key.VestedBalanceLastYear,
                    NetBalanceLastYear = group.Key.NetBalanceLastYear,
                    StoreNumber = group.Key.StoreNumber,
                    EmploymentStatus = group.Key.EmploymentStatus,
                    CompanyContributionYears = group.Key.CompanyContributionYears,
                    Details = group.SelectMany(x => x.Details)
                        .OrderByDescending(d => d.ProfitYear)
                        .ThenBy(d => d.Remark)
                        .Select(pd => new MilitaryRehireProfitSharingDetailResponse
                        {
                            Forfeiture = pd.Forfeiture,
                            Remark = pd.Remark,
                            ProfitYear = pd.ProfitYear,
                            HoursCurrentYear = pd.HoursCurrentYear,
                            EnrollmentId = pd.EnrollmentId,
                            EnrollmentName = pd.EnrollmentName,
                            ProfitCodeId = pd.ProfitCodeId
                        })
                        .ToList()
                }).AsQueryable()
                .ToPaginationResultsAsync(req, cancellationToken);
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
                context.PayProfits.Include(e => e.Enrollment)
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
                    demographics.EmploymentStatusId,
                    EmploymentStatus = demographics.EmploymentStatus!.Name,
                    PayProfit = new
                    {
                        payProfit.DemographicId,
                        payProfit.EnrollmentId,
                        payProfit.Enrollment,
                        payProfit.CurrentHoursYear,
                        payProfit.ProfitYear
                    }
                }
            )
            .Where(m =>
                m.EmploymentStatusId == EmploymentStatus.Constants.Active
                && m.ReHireDate != null
                && m.ReHireDate >= beginning
                && m.ReHireDate <= ending)
            .Join(
                context.ProfitDetails, // Table to join with (ProfitDetail)
                combined => new { combined.Ssn, combined.PayProfit.ProfitYear }, // Key selector from the result of the first join
                profitDetail => new { profitDetail.Ssn, profitDetail.ProfitYear }, // Foreign key selector from ProfitDetail
                (member, profitDetail) => new // Result selector after joining ProfitDetail
                {
                    member.BadgeNumber,
                    member.FullName,
                    member.Ssn,
                    member.HireDate,
                    member.TerminationDate,
                    member.ReHireDate,
                    member.StoreNumber,
                    member.EmploymentStatusId,
                    member.EmploymentStatus,
                    Details = new
                    {
                        EnrollmentName = member.PayProfit.Enrollment!.Name,
                        member.PayProfit.EnrollmentId,
                        member.PayProfit.CurrentHoursYear,
                        profitDetail.Forfeiture,
                        profitDetail.Remark,
                        profitDetail.ProfitYear,
                        profitDetail.ProfitCodeId
                    }
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
                (temp, yip) => new { temp.member, yip }
            )
            .GroupJoin(
                _totalService.TotalVestingBalance(context, (short)(beginning.Year - 1), ending),
                member => member.member.Ssn,
                tot => tot.Ssn,
                (member, tot) => new { member, tot }
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
                    ReHiredDate = temp.member.member.ReHireDate ?? ReferenceData.DsmMinValue,
                    StoreNumber = temp.member.member.StoreNumber,
                    CompanyContributionYears = temp.member.yip!.Years,
                    NetBalanceLastYear = tot != null ? tot.CurrentBalance ?? 0 : 0m,
                    VestedBalanceLastYear = tot != null ? tot.VestedBalance ?? 0 : 0m,
                    EmploymentStatusId = temp.member.member.EmploymentStatusId,
                    EmploymentStatus = temp.member.member.EmploymentStatus,
                    Details = new List<RehireProfitSummaryQueryDetails>
                    {
                        new RehireProfitSummaryQueryDetails
                        {
                            EnrollmentId = temp.member.member.Details.EnrollmentId,
                            EnrollmentName = temp.member.member.Details.EnrollmentName,
                            Forfeiture = temp.member.member.Details.Forfeiture,
                            HoursCurrentYear = temp.member.member.Details.CurrentHoursYear,
                            ProfitYear = temp.member.member.Details.ProfitYear,
                            Remark = temp.member.member.Details.Remark,
                            ProfitCodeId = temp.member.member.Details.ProfitCodeId
                        }
                    }
                }
            );

        return query;
    }

}
