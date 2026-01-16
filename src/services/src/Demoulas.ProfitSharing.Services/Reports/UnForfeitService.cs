using System.Linq.Dynamic.Core;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Common.Telemetry;
using Demoulas.ProfitSharing.Common.Validators;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class UnforfeitService : IUnforfeitService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly TotalService _totalService;

    public UnforfeitService(
        IProfitSharingDataContextFactory dataContextFactory,
        IDemographicReaderService demographicReaderService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _demographicReaderService = demographicReaderService;
        _totalService = totalService;
    }

    public async Task<ReportResponseBase<UnforfeituresResponse>> FindRehiresWhoMayBeEntitledToForfeituresTakenOutInPriorYearsAsync(FilterableStartAndEndDateRequest req,
        CancellationToken cancellationToken)
    {
        StartAndEndDateRequestValidator? validator = new();
        await validator.ValidateAndThrowAsync(req, cancellationToken);

        PaginatedResponseDto<UnforfeituresResponse>? rehiredEmployees;

        // Time the database query execution
        using (DatabaseTelemetryScope.StartQuery("GetRehireUnforfeitures", nameof(UnforfeitService), "read"))
        {
            rehiredEmployees = await _dataContextFactory.UseReadOnlyContext(async context =>
            {
                // Unforfeit is always for the current year.  Ths page is used in December flow - so we should be using member financials (pay_profit row)
                // from the wall clock year.
                // If someone is using this service in Jan/Feb/March - then we may have trouble in the Suggested Forfeit, because those transactions will be in the current year
                // and not the "openProfitYear" (aka wall clock year - 1)
                var today = DateTime.Today.ToDateOnly();
                short profitYear = (short)today.Year;

                IQueryable<ParticipantTotalYear>? yearsOfServiceQuery = _totalService.GetYearsOfService(context, profitYear, today);
                IQueryable<ParticipantTotalVestingBalance>? vestingServiceQuery = _totalService.TotalVestingBalance(context, profitYear, today);
                IQueryable<Demographic>? demo = await _demographicReaderService.BuildDemographicQuery(context);

                // PERFORMANCE: Pre-filter demographics to reduce join volume
                IQueryable<Demographic>? activeDemographics = demo
                    .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Active)
                    .Where(d => d.ReHireDate >= req.BeginningDate && d.ReHireDate <= req.EndingDate);

                // PERFORMANCE: Pre-filter ProfitDetails to only forfeitures before joining
                IQueryable<ProfitDetail>? forfeitureTransactions = context.ProfitDetails
                    .Where(pd => pd.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id);

                // PERFORMANCE: Phase 1 - Get main employee data WITHOUT Details (much faster)
                var mainQuery =
                    from pd in forfeitureTransactions
                    join d in activeDemographics on pd.Ssn equals d.Ssn
                    join ppYE in context.PayProfits
                        on new { d.Id, ProfitYear = profitYear } equals new { Id = ppYE.DemographicId, ppYE.ProfitYear }
                    join yos in yearsOfServiceQuery on d.Ssn equals yos.Ssn into yosTmp
                    from yos in yosTmp.DefaultIfEmpty()
                    join vest in vestingServiceQuery on d.Ssn equals vest.Ssn into vestTmp
                    from vest in vestTmp.DefaultIfEmpty()
                    where !req.ExcludeZeroBalance || vest != null && (vest.CurrentBalance != 0 || vest.VestedBalance != 0)
                    select new
                    {
                        d.BadgeNumber,
                        d.ContactInfo.FullName,
                        d.Ssn,
                        d.HireDate,
                        d.ReHireDate,
                        d.StoreNumber,
                        d.Id,
                        YearsOfService = yos != null ? yos.Years : (byte)0,
                        NetBalanceLastYear = vest != null ? vest.CurrentBalance ?? 0 : 0,
                        VestedBalanceLastYear = vest != null ? vest.VestedBalance ?? 0 : 0,
                        d.PayFrequencyId,
                        EnrollmentId = ppYE.VestingScheduleId == 0
                            ? EnrollmentConstants.NotEnrolled
                            : ppYE.HasForfeited
                                ? ppYE.VestingScheduleId == VestingSchedule.Constants.OldPlan
                                    ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                                    : EnrollmentConstants.NewVestingPlanHasForfeitureRecords
                                : ppYE.VestingScheduleId == VestingSchedule.Constants.OldPlan
                                    ? EnrollmentConstants.OldVestingPlanHasContributions
                                    : EnrollmentConstants.NewVestingPlanHasContributions,
                        EnrollmentName = EnrollmentConstants.GetDescription(ppYE.VestingScheduleId == 0
                            ? EnrollmentConstants.NotEnrolled
                            : ppYE.HasForfeited
                                ? ppYE.VestingScheduleId == VestingSchedule.Constants.OldPlan
                                    ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                                    : EnrollmentConstants.NewVestingPlanHasForfeitureRecords
                                : ppYE.VestingScheduleId == VestingSchedule.Constants.OldPlan
                                    ? EnrollmentConstants.OldVestingPlanHasContributions
                                    : EnrollmentConstants.NewVestingPlanHasContributions),
                        HoursProfitYear = ppYE.TotalHours,
                        WagesProfitYear = ppYE.TotalIncome
                    };

                // Apply pagination to main query
                var sortBy = (req.SortBy ?? "badgenumber").ToLowerInvariant() switch
                {
                    "rehireddate" => "RehireDate",
                    "companycontributionyears" => "YearsOfService",
                    "" => "badgenumber",
                    _ => (req.SortBy ?? "badgenumber")
                };

                if (req.IsSortDescending ?? false)
                {
                    sortBy += " DESC";
                }


                var paginatedMain = await mainQuery
                    .Distinct() // Remove duplicates from multiple ProfitDetail rows
                    .OrderBy(sortBy)
                    .Skip(req.Skip ?? 0)
                    .Take(req.Take ?? 10)
                    .ToListAsync(cancellationToken);

                if (!paginatedMain.Any())
                {
                    return new PaginatedResponseDto<UnforfeituresResponse>(req)
                    {
                        Results = new List<UnforfeituresResponse>(),
                        Total = 0
                    };
                }

                // Get total count for pagination
                var totalCount = await mainQuery.Select(x => x.Ssn).Distinct().CountAsync(cancellationToken);

                // PERFORMANCE: Phase 2 - Get Details for only the SSNs in the result set
                var ssnList = paginatedMain.Select(x => x.Ssn).Distinct().ToList();

                var detailsQuery =
                    from pd in forfeitureTransactions
                    where ssnList.Contains(pd.Ssn)
                    join d in activeDemographics on pd.Ssn equals d.Ssn
                    join pp in context.PayProfits
                        on new { d.Id, pd.ProfitYear } equals new { Id = pp.DemographicId, pp.ProfitYear } into ppTmp
                    from pp in ppTmp.DefaultIfEmpty()
                    select new
                    {
                        pd.Ssn,
                        pd.Id,
                        pd.ProfitYear,
                        pd.Forfeiture,
                        pd.Remark,
                        pd.ProfitCodeId,
                        pd.CommentType,
                        HoursTransactionYear = pp != null ? (decimal?)pp.CurrentHoursYear : null,
                        WagesTransactionYear = pp != null ? (decimal?)(pp.TotalIncome) : null
                    };

                var allDetails = await detailsQuery.ToListAsync(cancellationToken);

                // Group details by SSN for efficient lookup
                var detailsBySsn = allDetails
                    .GroupBy(x => x.Ssn)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // PERFORMANCE: Phase 3 - Combine in memory (fast for small result sets)
                var responses = paginatedMain.Select(main =>
                {
                    detailsBySsn.TryGetValue(main.Ssn, out var details);
                    var maxProfitDetailId = (details?.Any() ?? false) ? details.Max(d => d.Id) : 0;

                    return new UnforfeituresResponse
                    {
                        BadgeNumber = main.BadgeNumber,
                        FullName = main.FullName,
                        Ssn = main.Ssn.MaskSsn(),
                        HireDate = main.HireDate,
                        ReHiredDate = main.ReHireDate ?? default,
                        StoreNumber = main.StoreNumber,
                        CompanyContributionYears = main.YearsOfService,
                        NetBalanceLastYear = main.NetBalanceLastYear,
                        VestedBalanceLastYear = main.VestedBalanceLastYear,
                        EnrollmentName = main.EnrollmentName,
                        EnrollmentId = main.EnrollmentId,
                        HoursProfitYear = main.HoursProfitYear,
                        WagesProfitYear = main.WagesProfitYear,
                        IsExecutive = main.PayFrequencyId == PayFrequency.Constants.Monthly,
                        Details = (details ?? Enumerable.Empty<dynamic>())
                            .Select(d => new RehireTransactionDetailResponse
                            {
                                ProfitYear = d.ProfitYear,
                                HoursTransactionYear = d.HoursTransactionYear,
                                Forfeiture = d.Forfeiture,
                                Remark = d.Remark,
                                ProfitCodeId = d.ProfitCodeId,
                                WagesTransactionYear = d.WagesTransactionYear,
                                SuggestedUnforfeiture =
                                    d.Id == maxProfitDetailId &&
                                    d.CommentType != null && d.CommentType == CommentType.Constants.Forfeit &&
                                    (main.YearsOfService == 1 && main.EnrollmentId == EnrollmentConstants.NewVestingPlanHasContributions
                                     || main.EnrollmentId == EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                                     || main.EnrollmentId == EnrollmentConstants.NewVestingPlanHasForfeitureRecords)
                                        ? d.Forfeiture
                                        : null,
                                ProfitDetailId = d.Id
                            })
                            .OrderByDescending(x => x.ProfitDetailId)
                            .ToList()
                    };
                }).ToList();

                var results = new PaginatedResponseDto<UnforfeituresResponse>(req)
                {
                    Results = responses,
                    Total = totalCount
                };

                // Record result count metric
                EndpointTelemetry.RecordCountsProcessed.Record(results.Results.Count(),
                    new("record_type", "unforfeiture"),
                    new("service", nameof(UnforfeitService)));

                return results;
            }, cancellationToken);
        }

        // Business logic: Build response wrapper (minimal processing)
        ReportResponseBase<UnforfeituresResponse> response;
        using (BusinessLogicTelemetryScope.Start("BuildReportResponse", nameof(UnforfeitService)))
        {
            response = new ReportResponseBase<UnforfeituresResponse>
            {
                ReportName = "REHIRE'S PROFIT SHARING DATA",
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = req.BeginningDate,
                EndDate = req.EndingDate,
                Response = rehiredEmployees
            };
        }

        return response;
    }
}
