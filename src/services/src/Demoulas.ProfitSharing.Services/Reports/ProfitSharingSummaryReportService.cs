using System.ComponentModel;
using System.Linq.Expressions;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports;

/// <summary>
/// Service for generating year-end profit sharing summary and detail reports.
/// </summary>
public sealed class ProfitSharingSummaryReportService : IProfitSharingSummaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private static readonly short _hoursThreshold;

    private sealed record EmployeeProjection
    {
        public required int BadgeNumber { get; init; }
        public required decimal Hours { get; init; }
        public required decimal Wages { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public char EmploymentStatusId { get; init; }
        public DateOnly? TerminationDate { get; init; }
        public int Ssn { get; init; }
        public string? FullName { get; init; } = null!;
        public short StoreNumber { get; init; }
        public char EmploymentTypeId { get; init; }
        public string EmploymentTypeName { get; init; } = null!;
        public decimal? PointsEarned { get; init; }
        public byte? Years { get; init; }
        public short? FirstContributionYear { get; init; }
        public bool IsExecutive { get; init; }
        public byte EnrollmentId { get; init; }
    }

    private sealed record EmployeeWithBalance
    {
        public required EmployeeProjection Employee { get; init; } = null!;
        public required short ProfitYear { get; init; }
        public required decimal Balance { get; init; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfitSharingSummaryReportService"/> class.
    /// </summary>
    public ProfitSharingSummaryReportService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
    }

    static ProfitSharingSummaryReportService()
    {
        // Set the hours threshold for profit sharing eligibility
        _hoursThreshold = ReferenceData.MinimumHoursForContribution();
    }

    /// <summary>
    /// Generates the year-end profit sharing summary report, grouped by eligibility and status.
    /// OPTIMIZED: Single query with in-memory aggregation to eliminate multiple database roundtrips.
    /// </summary>
    /// <param name="req">The profit year request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Summary response with line items for each group.</returns>
    public async Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(
        BadgeNumberRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
        var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
        var fiscalEndDate = calInfo.FiscalEndDate;
        var fiscalBeginDate = calInfo.FiscalBeginDate;

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // OPTIMIZATION: Fetch minimal employee data once with a lightweight DTO
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, req.UseFrozenData);

            // Get balance data once (instead of via ActiveSummary which builds full EmployeeWithBalance objects)
            var beginningBalanceYear = (short)(req.ProfitYear - 1);
            var balances = _totalService.GetTotalBalanceSet(ctx, beginningBalanceYear);
            var priorBalances = _totalService.GetTotalBalanceSet(ctx, (short)(beginningBalanceYear - 1));

            // Build lightweight summary DTO - fetch everything once
            var summaryData = await (
                from pp in ctx.PayProfits.Where(p => p.ProfitYear == req.ProfitYear)
                join d in demographicQuery on pp.DemographicId equals d.Id
                join bal in balances on d.Ssn equals bal.Ssn into balTmp
                from bal in balTmp.DefaultIfEmpty()
                join priorBal in priorBalances on d.Ssn equals priorBal.Ssn into priorBalTmp
                from priorBal in priorBalTmp.DefaultIfEmpty()
                select new
                {
                    Hours = pp.CurrentHoursYear + pp.HoursExecutive,
                    Wages = pp.CurrentIncomeYear + pp.IncomeExecutive,
                    Points = (int)Math.Round((pp.CurrentIncomeYear + pp.IncomeExecutive) / 100, MidpointRounding.AwayFromZero),
                    DateOfBirth = d.DateOfBirth,
                    EmploymentStatus = d.EmploymentStatusId,
                    TerminationDate = d.TerminationDate,
                    Balance = (decimal)(bal != null && bal.TotalAmount != null ? bal.TotalAmount : 0),
                    PriorBalance = (decimal)(priorBal != null && priorBal.TotalAmount != null ? priorBal.TotalAmount : 0)
                })
                .ToListAsync(cancellationToken);

            // Helper to create line items from in-memory data
            YearEndProfitSharingReportSummaryLineItem CreateLineFromData(
                string subgroup,
                string prefix,
                string title,
                Func<dynamic, bool> mainFilter,
                Func<dynamic, bool>? totalsFilter = null,
                bool includeHoursAndPoints = false)
            {
                var mainGroup = summaryData.Where(mainFilter).ToList();

                var lineItem = new YearEndProfitSharingReportSummaryLineItem
                {
                    Subgroup = subgroup,
                    LineItemPrefix = prefix,
                    LineItemTitle = title,
                    NumberOfMembers = mainGroup.Count,
                    TotalWages = mainGroup.Sum(x => (decimal)x.Wages),
                    TotalHours = includeHoursAndPoints ? mainGroup.Sum(x => (decimal)x.Hours) : null,
                    TotalPoints = includeHoursAndPoints ? mainGroup.Sum(x => (int)x.Points) : null,
                    TotalBalance = mainGroup.Sum(x => (decimal)x.Balance),
                    TotalPriorBalance = mainGroup.Sum(x => (decimal)x.PriorBalance)
                };

                // If totalsFilter provided, override NumberOfMembers
                if (totalsFilter != null)
                {
                    lineItem.NumberOfMembers = summaryData.Where(totalsFilter).Count();
                }

                return lineItem;
            }

            // Helper predicates matching GetReportDetail logic
            bool IsActiveOrInactive(dynamic x) =>
                x.EmploymentStatus == EmploymentStatus.Constants.Active ||
                x.EmploymentStatus == EmploymentStatus.Constants.Inactive;

            bool IsTerminatedAfterFiscalEnd(dynamic x) =>
                x.EmploymentStatus == EmploymentStatus.Constants.Terminated &&
                x.TerminationDate != null && x.TerminationDate > fiscalEndDate;

            bool IsTerminatedBeforeFiscalEnd(dynamic x) =>
                x.EmploymentStatus == EmploymentStatus.Constants.Terminated &&
                x.TerminationDate != null &&
                x.TerminationDate < fiscalEndDate;

            // OPTIMIZATION: Build all line items in memory (no more database roundtrips)
            var lineItems = new List<YearEndProfitSharingReportSummaryLineItem>
            {
                // Line 1: Age 18-20 with 1000+ hours (Active/Inactive or terminated after fiscal end)
                CreateLineFromData(
                    "Active and Inactive",
                    ((int)YearEndProfitSharingReportId.Age18To20With1000Hours).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age18To20With1000Hours),
                    x => (IsActiveOrInactive(x) || IsTerminatedAfterFiscalEnd(x)) &&
                         x.Hours >= _hoursThreshold &&
                         x.DateOfBirth <= birthday18 &&
                         x.DateOfBirth > birthday21 &&
                         x.Wages > 0),

                // Line 2: Age 21+ with 1000+ hours (ONLY line with hours/points populated)
                CreateLineFromData(
                    "Active and Inactive",
                    ((int)YearEndProfitSharingReportId.Age21OrOlderWith1000Hours).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age21OrOlderWith1000Hours),
                    x => (IsActiveOrInactive(x) || IsTerminatedAfterFiscalEnd(x)) &&
                         x.Hours >= _hoursThreshold &&
                         x.DateOfBirth <= birthday21 &&
                         x.Wages > 0,
                    totalsFilter: null,
                    includeHoursAndPoints: true),

                // Line 3: Under 18
                CreateLineFromData(
                    "Active and Inactive",
                    ((int)YearEndProfitSharingReportId.Under18).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Under18),
                    x => (IsActiveOrInactive(x) || IsTerminatedAfterFiscalEnd(x)) &&
                         x.DateOfBirth > birthday18 &&
                         x.Wages > 0),

                // Line 4: Age 18+ with <1000 hours and prior balance
                CreateLineFromData(
                    "Active and Inactive",
                    ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount),
                    x => (IsActiveOrInactive(x) || IsTerminatedAfterFiscalEnd(x)) &&
                         x.Hours < _hoursThreshold &&
                         x.DateOfBirth <= birthday18 &&
                         x.Balance > 0),

                // Line 5: Age 18+ with <1000 hours and no prior balance
                CreateLineFromData(
                    "Active and Inactive",
                    ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                    x => (IsActiveOrInactive(x) || IsTerminatedAfterFiscalEnd(x)) &&
                         x.Hours < _hoursThreshold &&
                         x.DateOfBirth <= birthday18 &&
                         x.Balance == 0 &&
                         x.Wages > 0),

                // Line 6: Terminated age 18+ with 1000+ hours
                CreateLineFromData(
                    "TERMINATED",
                    ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours),
                    x => IsTerminatedBeforeFiscalEnd(x) &&
                         x.Hours >= _hoursThreshold &&
                         x.DateOfBirth <= birthday18 &&
                         x.Wages > 0),

                // Line 7: Terminated age 18+ with <1000 hours and no prior balance
                // COBOL Logic: Counts ALL terminated employees classified as Report 7, not just those terminated in fiscal year
                CreateLineFromData(
                    "TERMINATED",
                    ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                    x => x.EmploymentStatus == EmploymentStatus.Constants.Terminated &&
                         x.TerminationDate != null &&
                         x.TerminationDate < fiscalEndDate &&
                         x.Hours < _hoursThreshold &&
                         x.DateOfBirth <= birthday18 &&
                         x.Balance == 0 &&
                         x.Wages > 0),

                // Line 8: Terminated age 18+ with <1000 hours and prior balance
                // COBOL Logic: Counts ALL terminated employees classified as Report 8, not just those terminated in fiscal year
                CreateLineFromData(
                    "TERMINATED",
                    ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount),
                    x => x.EmploymentStatus == EmploymentStatus.Constants.Terminated &&
                         x.TerminationDate != null &&
                         x.TerminationDate < fiscalEndDate &&
                         x.Hours < _hoursThreshold &&
                         x.DateOfBirth <= birthday18 &&
                         x.Balance > 0)
            };

            // Line N: Non-employee beneficiaries (from BeneficiaryContacts, NOT in Demographics)
            // Matches COBOL Report 10 logic - LEFT JOIN anti-join pattern
            var demographicQueryForBeneficiaries = await _demographicReaderService.BuildDemographicQuery(ctx, false);
            var beneData = await (
                from bc in ctx.BeneficiaryContacts
                join d in demographicQueryForBeneficiaries on bc.Ssn equals d.Ssn into demoJoin
                from d in demoJoin.DefaultIfEmpty()
                where d == null
                join tot in _totalService.GetTotalBalanceSet(ctx, beginningBalanceYear) on bc.Ssn equals tot.Ssn into totJoin
                from tot in totJoin.DefaultIfEmpty()
                select new { bc, tot }
            ).ToListAsync(cancellationToken);

            if (beneData.Any())
            {
                var beneficiaryLineItem = new YearEndProfitSharingReportSummaryLineItem
                {
                    Subgroup = "TERMINATED",
                    LineItemPrefix = "N",
                    LineItemTitle = "NON-EMPLOYEE BENEFICIARIES",
                    NumberOfMembers = beneData.Count,
                    TotalWages = 0,
                    TotalHours = null,  // Null - beneficiaries don't have hours
                    TotalPoints = null, // Null - beneficiaries don't get contributions/points
                    TotalBalance = beneData.Sum(x => x.tot?.TotalAmount ?? 0),
                    TotalPriorBalance = 0
                };
                lineItems.Add(beneficiaryLineItem);
            }

            return new YearEndProfitSharingReportSummaryResponse { LineItems = lineItems };
        }, cancellationToken);
    }

    /// <summary>
    /// Generates the year-end profit sharing detail report for a given request.
    /// </summary>
    /// <param name="req">The report request, including filters and report ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed report response with totals and pagination.</returns>
    public async Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(
        YearEndProfitSharingReportRequest req,
        CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            IQueryable<YearEndProfitSharingReportDetail> allDetails;
            
            // Special handling for Report 10: Non-Employee Beneficiaries
            if (req.ReportId == YearEndProfitSharingReportId.NonEmployeeBeneficiaries)
            {
                allDetails = await GetNonEmployeeBeneficiariesAsync(ctx, req.ProfitYear, cancellationToken);
            }
            else
            {
                // Standard employee reports (1-8): fetch from Demographics/PayProfit
                allDetails = await ActiveSummary(ctx, req, calInfo.FiscalEndDate);
            }

            // Apply report-specific filtering for ReportId 1-8, 10
            IQueryable<YearEndProfitSharingReportDetail> filteredDetails = allDetails;
            var reportIdInt = (int)req.ReportId;
            if (reportIdInt is >= 1 and <= 8 or 10)
            {
                var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
                var birthday21 = calInfo.FiscalEndDate.AddYears(-21);

                var filter = GetReportDetailFilterExpression(reportIdInt, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21);
                filteredDetails = allDetails.Where(filter);
            }

            // READY excludes employees with zero wages (COBOL behavior) - BUT NOT for reports that track prior amounts
            // Reports 4 and 8 specifically include employees with prior balances regardless of current-year wages
            // Report 10 (NonEmployeeBeneficiaries) has zero wages by design - they're non-employees with inherited balances
            // Match this behavior to ensure SMART reports match READY reports
            if (req.ReportId != YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount &&
                req.ReportId != YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount &&
                req.ReportId != YearEndProfitSharingReportId.NonEmployeeBeneficiaries)
            {
                filteredDetails = filteredDetails.Where(d => d.Wages > 0);
            }

            var sortReq = req;
            // Sorting exceptions brought on by oracle provide limitations.
            if (req.SortBy != null && req.SortBy.Equals("age", StringComparison.InvariantCultureIgnoreCase))
            {
                sortReq = req with { SortBy = "DateOfBirth" };
            }

            if (req.SortBy != null && req.SortBy.Equals("lastName", StringComparison.InvariantCultureIgnoreCase))
            {
                sortReq = req with { SortBy = "EmployeeName" };
            }

            var details = await filteredDetails.ToPaginationResultsAsync(sortReq, cancellationToken);

            // Post-materialization processing
            foreach (var detail in details.Results)
            {
                // Convert database status (lowercase) to uppercase
                // Database: 'a'/'i'/'t' → Report format: 'A'/'I'/'T'
                if (detail.EmployeeStatus.HasValue)
                {
                    detail.EmployeeStatus = char.ToUpper(detail.EmployeeStatus.Value);
                }

                // For terminated employee reports (6, 7, 8), zero out Points per READY PAY426N COBOL logic
                if (req.ReportId == YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours ||
                    req.ReportId == YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount ||
                    req.ReportId == YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount)
                {
                    detail.Points = 0;
                }
            }

            // Totals calculation - handle in-memory queryables (Report 10) vs EF queryables (Reports 1-8)
            ProfitShareTotal totals;
            
            if (req.ReportId == YearEndProfitSharingReportId.NonEmployeeBeneficiaries)
            {
                // Report 10: filteredDetails is in-memory, use synchronous LINQ
                var filteredList = filteredDetails.ToList();
                totals = new ProfitShareTotal
                {
                    WagesTotal = filteredList.Sum(x => x.Wages),
                    HoursTotal = filteredList.Sum(x => x.Hours),
                    PointsTotal = filteredList.Sum(x => x.Points),
                    BalanceTotal = filteredList.Sum(x => x.Balance),
                    NumberOfEmployees = filteredList.Count,
                    NumberOfNewEmployees = filteredList.Count(x => x.YearsInPlan == 0),
                };
            }
            else
            {
                // Reports 1-8: filteredDetails is EF queryable, use async operations
                var filteredTotals = await (
                    from a in filteredDetails
                    group a by true
                    into g
                    select new
                    {
                        NumberOfEmployees = g.Count(),
                        NumberOfNewEmployees = g.Count(x => x.YearsInPlan == 0),
                        WagesTotal = g.Sum(x => x.Wages),
                        HoursTotal = g.Sum(x => x.Hours),
                        PointsTotal = g.Sum(x => x.Points),
                        BalanceTotal = g.Sum(x => x.Balance),
                    }
                ).FirstOrDefaultAsync(cancellationToken);

                totals = new ProfitShareTotal
                {
                    WagesTotal = filteredTotals?.WagesTotal ?? 0,
                    HoursTotal = filteredTotals?.HoursTotal ?? 0,
                    PointsTotal = filteredTotals?.PointsTotal ?? 0,
                    BalanceTotal = filteredTotals?.BalanceTotal ?? 0,
                    NumberOfEmployees = filteredTotals?.NumberOfEmployees ?? 0,
                    NumberOfNewEmployees = filteredTotals?.NumberOfEmployees ?? 0,
                };
            }

            var response = new YearEndProfitSharingReportResponse
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = calInfo.FiscalBeginDate,
                EndDate = calInfo.FiscalEndDate,
                ReportName = $"PROFIT SHARE REPORT (PAY426) - {req.ProfitYear}",
                Response = details,
                WagesTotal = totals.WagesTotal,
                HoursTotal = totals.HoursTotal,
                PointsTotal = totals.PointsTotal,
                BalanceTotal = totals.BalanceTotal,
                NumberOfEmployees = totals.NumberOfEmployees,
                NumberOfNewEmployees = totals.NumberOfNewEmployees,
                NumberOfEmployeesInPlan = totals.NumberOfEmployees - totals.NumberOfEmployeesUnder21 - totals.NumberOfNewEmployees,
                NumberOfEmployeesUnder21 = totals.NumberOfEmployeesUnder21
            };
            return response;
        }, cancellationToken);
    }

    public async Task<YearEndProfitSharingReportTotals> GetYearEndProfitSharingTotalsAsync(
        BadgeNumberRequest req,
        CancellationToken cancellationToken = default
    )
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
        var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
        var birthday64 = calInfo.FiscalEndDate.AddYears(-64);
        var fiscalEndDate = calInfo.FiscalEndDate;

        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Build optimized query that aggregates at database level without materializing individual records
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, req.UseFrozenData);

            // Get first contribution year lookup
            var firstContributionYearQuery = TotalService.GetFirstContributionYear(ctx, req.ProfitYear);

            // Build aggregation query directly from PayProfit + Demographics
            // Note: We materialize to a simple DTO first to avoid complex EF translation issues
            var totalsData = await (
                from pp in ctx.PayProfits
                    .Where(p => p.ProfitYear == req.ProfitYear)
                join d in demographicQuery on pp.DemographicId equals d.Id
                join fc in firstContributionYearQuery on d.Ssn equals fc.Ssn into fcTmp
                from fc in fcTmp.DefaultIfEmpty()
                where ((d.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                        d.EmploymentStatusId == EmploymentStatus.Constants.Inactive) ||
                       (d.TerminationDate.HasValue && d.TerminationDate.Value > fiscalEndDate)) &&
                      (((pp.CurrentHoursYear + pp.HoursExecutive) >= _hoursThreshold && d.DateOfBirth <= birthday18) ||
                       (d.DateOfBirth <= birthday64))
                select new
                {
                    Hours = pp.CurrentHoursYear + pp.HoursExecutive,
                    Wages = pp.CurrentIncomeYear + pp.IncomeExecutive,
                    DateOfBirth = d.DateOfBirth,
                    FirstContributionYear = fc != null ? fc.FirstContributionYear : (short?)null,
                    Ssn = d.Ssn
                })
                .ToListAsync(cancellationToken);

            // Aggregate in memory (much faster than old approach since we skip balance calculations)
            var numberOfEmployees = totalsData.Count(x => x.DateOfBirth > birthday21 || x.Hours >= _hoursThreshold);
            var numberOfNewEmployees = totalsData.Count(x => !x.FirstContributionYear.HasValue && x.DateOfBirth <= birthday21);
            var numberOfEmployeesUnder21 = totalsData.Count(x => x.DateOfBirth > birthday21);

            // Exclude age < 21 and age 64+ with < 1000 hours from wage/hour totals
            var eligibleForTotals = totalsData.Where(x =>
                x.DateOfBirth <= birthday21 &&
                !(x.DateOfBirth <= birthday64 && x.Hours < _hoursThreshold)).ToList();

            var wagesTotal = eligibleForTotals.Sum(x => x.Wages);
            // Truncate individual hours BEFORE summing to match READY PAY426.cbl behavior
            // READY: ADD S-HRS (truncated int) TO WS-SECT-HOURS (no decimals)
            var hoursTotal = eligibleForTotals.Sum(x => Math.Truncate(x.Hours));
            // Calculate points per employee BEFORE summing to match READY PAY426.cbl behavior
            // READY: Rounds each employee's wages/100, then sums rounded points
            var pointsTotal = eligibleForTotals.Sum(x => Math.Round(x.Wages / 100, MidpointRounding.AwayFromZero));

            return new YearEndProfitSharingReportTotals
            {
                NumberOfEmployees = numberOfEmployees,
                NumberOfNewEmployees = numberOfNewEmployees,
                NumberOfEmployeesUnder21 = numberOfEmployeesUnder21,
                WagesTotal = wagesTotal,
                HoursTotal = hoursTotal,
                PointsTotal = pointsTotal,
            };
        }, cancellationToken);
    }

    /// <summary>
    /// Builds a filtered set of employees with balances and years of service for a given year and optional badge number.
    /// </summary>
    private async Task<IQueryable<EmployeeWithBalance>> BuildFilteredEmployeeSetAsync(
        ProfitSharingReadOnlyDbContext ctx,
        FrozenProfitYearRequest req,
        int? badgeNumber,
        DateOnly asOfDate)
    {
        IQueryable<PayProfit> basePayProfits = ctx.PayProfits
            .Where(p => p.ProfitYear == req.ProfitYear)
            .Include(p => p.Demographic)
            .ThenInclude(d => d!.ContactInfo);

        var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, req.UseFrozenData);
        basePayProfits = basePayProfits
            .Join(demographicQuery, p => p.DemographicId, d => d.Id, (p, _) => p);

        var yearsOfService = _totalService.GetYearsOfService(ctx, req.ProfitYear, asOfDate);
        var employeeQry =
            from pp in basePayProfits
            join et in ctx.EmploymentTypes on pp.Demographic!.EmploymentTypeId equals et.Id
            join yip in yearsOfService on pp.Demographic!.Ssn equals yip.Ssn into yipTmp
            from yip in yipTmp.DefaultIfEmpty()
            join fc in TotalService.GetFirstContributionYear(ctx, req.ProfitYear) on pp.Demographic!.Ssn equals fc.Ssn into fcTmp
            from fc in fcTmp.DefaultIfEmpty()
            select new EmployeeProjection
            {
                BadgeNumber = pp.Demographic!.BadgeNumber,
                // Truncate hours to match READY PAY426.cbl behavior (uses S-HRS integer part only)
                Hours = Math.Truncate(pp.CurrentHoursYear + pp.HoursExecutive),
                Wages = pp.CurrentIncomeYear + pp.IncomeExecutive,
                DateOfBirth = pp.Demographic!.DateOfBirth,
                EmploymentStatusId = pp.Demographic!.EmploymentStatusId,
                TerminationDate = pp.Demographic!.TerminationDate,
                Ssn = pp.Demographic!.Ssn,
                FullName = pp.Demographic!.ContactInfo.FullName,
                StoreNumber = pp.Demographic!.StoreNumber,
                EmploymentTypeId = (char)pp.EmployeeTypeId,
                EmploymentTypeName = et.Name,
                PointsEarned = pp.PointsEarned,
                Years = yip.Years,
                FirstContributionYear = fc.FirstContributionYear,
                IsExecutive = pp.Demographic!.PayFrequencyId == PayFrequency.Constants.Monthly,
                EnrollmentId = pp.EnrollmentId
            };


        if (badgeNumber.HasValue)
        {
            employeeQry = employeeQry.Where(e => e.BadgeNumber == badgeNumber);
        }

        var beginningBalanceYear = (short)(req.ProfitYear - 1);
        var balances = _totalService.GetTotalBalanceSet(ctx, beginningBalanceYear);
        var employeeWithBalanceQry =
            from e in employeeQry
            join bal in balances on e.Ssn equals bal.Ssn into balTmp
            from bal in balTmp.DefaultIfEmpty()
            select new EmployeeWithBalance
            {
                Employee = e,
                ProfitYear = req.ProfitYear,
                Balance = (decimal)(bal != null && bal.TotalAmount != null ? bal.TotalAmount : 0),
            };
        return employeeWithBalanceQry;
    }

    /// <summary>
    /// Returns non-employee beneficiaries (people with balances who are NOT in Demographics/PayProfit).
    /// Matches COBOL PAY426N Report 10 logic: reads PAYBEN, excludes anyone in PAYPROFIT.
    /// </summary>
    private async Task<IQueryable<YearEndProfitSharingReportDetail>> GetNonEmployeeBeneficiariesAsync(
        ProfitSharingReadOnlyDbContext ctx,
        short profitYear,
        CancellationToken cancellationToken = default)
    {
        // Get beneficiaries who are NOT in Demographics (non-employees)
        // Matches COBOL PAY426N Report 10: reads PAYBEN, checks if SSN exists in PAYPROFIT
        var beginningBalanceYear = (short)(profitYear - 1);
        var balances = _totalService.GetTotalBalanceSet(ctx, beginningBalanceYear);

        // Build demographic query using the reader service (frozen data not needed for anti-join check)
        var demographicQueryForAntiJoin = await _demographicReaderService.BuildDemographicQuery(ctx, useFrozenData: false);

        // We cannot call MaskSsn() inside an EF Core-translated query. Materialize the minimal data first
        // then apply MaskSsn() in-memory. Use LEFT JOIN with null check for anti-join pattern.
        var rawBeneficiaries = await (
            from bc in ctx.BeneficiaryContacts
            join d in demographicQueryForAntiJoin on bc.Ssn equals d.Ssn into demoJoin
            from d in demoJoin.DefaultIfEmpty()
            where d == null  // Anti-join: only beneficiaries NOT in Demographics (COBOL PAYPROFIT equivalent)
            join bal in balances on bc.Ssn equals bal.Ssn into balJoin
            from bal in balJoin.DefaultIfEmpty()  // LEFT JOIN: include beneficiaries even if they have no balance
            select new { Beneficiary = bc, Balance = bal }
        ).ToListAsync(cancellationToken);

        var details = rawBeneficiaries.Select(x => new YearEndProfitSharingReportDetail
        {
            BadgeNumber = 0,  // Non-employees have badge 0
            ProfitYear = profitYear,
            EmployeeName = x.Beneficiary.ContactInfo.FullName ?? string.Empty,
            StoreNumber = 0,
            EmployeeTypeCode = '0',  // READY reports beneficiaries with type code '0' (non-employee indicator)
            EmployeeTypeName = "Hourly",  // Will be normalized to "Hourly" for comparison
            DateOfBirth = x.Beneficiary.DateOfBirth,
            Age = 99,  // READY COBOL sets WS-AGE = 99 for all beneficiaries (PAY426N.cbl line 1610)
            // Use the MaskSsn extension in-memory (bc.Ssn is an int)
            Ssn = x.Beneficiary.Ssn.MaskSsn(),
            Wages = 0,  // Non-employees have no wages
            Hours = 0,  // Non-employees have no hours
            Points = 0,  // No points earned
            IsNew = false,
            IsUnder21 = false,
            EmployeeStatus = EmploymentStatus.Constants.Active,  // READY marks beneficiaries as Active (space in report)
            Balance = (decimal)(x.Balance?.TotalAmount ?? 0),  // Balance may be null if no prior year balance exists
            YearsInPlan = 0,  // Fixed value per COBOL (WS-PYRS = 99)
            TerminationDate = null,
            FirstContributionYear = null,
            IsExecutive = false
        }).AsQueryable();

        return details;
    }

    /// <summary>
    /// Returns a queryable of year-end profit sharing report details for the given request.
    /// </summary>
    private Task<IQueryable<YearEndProfitSharingReportDetail>> ActiveSummary(ProfitSharingReadOnlyDbContext ctx, BadgeNumberRequest req, DateOnly ageAsOfDate)
    {
        return ActiveSummary(ctx, req, ageAsOfDate, req.BadgeNumber);
    }

    /// <summary>
    /// Returns a queryable of year-end profit sharing report details for the given year and optional badge number.
    /// </summary>
    private async Task<IQueryable<YearEndProfitSharingReportDetail>> ActiveSummary(ProfitSharingReadOnlyDbContext ctx, FrozenProfitYearRequest req, DateOnly ageAsOfDate,
        int? badgeNumber = null)
    {
        var employees = await BuildFilteredEmployeeSetAsync(ctx, req, badgeNumber, ageAsOfDate);

        // Calculate age once to avoid multiple EF translations
        var ageCalcDate = ageAsOfDate.ToDateTime(TimeOnly.MaxValue);

        // Calculate age cutoff for 21 years old (anyone born on or before this date is >= 21)
        var age21CutoffDate = ageAsOfDate.AddYears(-21);

        // Always fetch all details for the year
        IQueryable<YearEndProfitSharingReportDetail> allDetails = employees.Select(x => new YearEndProfitSharingReportDetail
        {
            BadgeNumber = x.Employee.BadgeNumber,
            ProfitYear = x.ProfitYear,
            EmployeeName = x.Employee.FullName!,
            StoreNumber = x.Employee.StoreNumber,
            EmployeeTypeCode = x.Employee.EmploymentTypeId,
            EmployeeTypeName = x.Employee.EmploymentTypeName,
            DateOfBirth = x.Employee.DateOfBirth,
            Age = (byte)x.Employee.DateOfBirth.Age(ageCalcDate),
            Ssn = x.Employee.Ssn.MaskSsn(),
            Wages = x.Employee.Wages,
            Hours = x.Employee.Hours,
            // Calculate points per READY PAY426N formula (COBOL lines 2620-2633):
            // - Age >= 21 AND Hours >= 1000: DIVIDE wages BY 100, round up if remainder >= 50
            // - Otherwise: points = 0
            // Note: Points will be zeroed for terminated employee reports (6, 7, 8) after materialization
            Points = x.Employee.DateOfBirth <= age21CutoffDate && x.Employee.Hours >= 1000
                ? (short)(Math.Truncate(x.Employee.Wages / 100) + (x.Employee.Wages % 100 >= 50 ? 1 : 0))
                : (short)0,
            // Calculate IsNew dynamically like READY report does
            // READY PAY426N displays "NEW" only when ALL conditions met:
            // 1. Never enrolled (PY-PS-ENROLLED = 0 / EnrollmentId = 0)
            // 2. No prior contributions (NO-PROFIT-SHARE / FirstContributionYear == null)
            // 3. Meets minimum hours (Hours >= 1000)
            // 4. At least 18 years old (Report 3 shows "<21>" for all under-18, never "NEW")
            IsNew = x.Employee.EnrollmentId == 0
                && x.Employee.FirstContributionYear == null
                && x.Employee.Hours >= ReferenceData.MinimumHoursForContribution()
                && x.Employee.DateOfBirth.Age(ageCalcDate) >= 18,
            IsUnder21 = x.Employee.DateOfBirth.Age(ageCalcDate) < 21,
            EmployeeStatus = x.Employee.EmploymentStatusId,
            Balance = x.Balance,
            YearsInPlan = x.Employee.Years ?? 0,
            TerminationDate = x.Employee.TerminationDate,
            FirstContributionYear = x.Employee.FirstContributionYear,
            IsExecutive = x.Employee.IsExecutive
        });

        return allDetails;
    }

    /// <summary>
    /// Returns a filter expression for individual employee detail records based on PAY426N report ID.
    /// </summary>
    /// <param name="reportId">The report ID or line number.</param>
    /// <param name="fiscalBeginDate">Fiscal year begin date.</param>
    /// <param name="fiscalEndDate">Fiscal year end date.</param>
    /// <param name="birthday18">Date representing 18 years before fiscal end.</param>
    /// <param name="birthday21">Date representing 21 years before fiscal end.</param>
    /// <returns>Filter expression for the report detail records.</returns>
    private static Expression<Func<YearEndProfitSharingReportDetail, bool>> GetReportDetailFilterExpression(
        int reportId,
        DateOnly fiscalBeginDate,
        DateOnly fiscalEndDate,
        DateOnly birthday18,
        DateOnly birthday21)
    {
        return reportId switch
        {
            1 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21,
            2 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday21,
            3 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.DateOfBirth > birthday18,
            4 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.Balance > 0,
            5 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.Balance == 0,
            6 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate < fiscalEndDate &&
                x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday18,
            7 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate != null && x.TerminationDate < fiscalEndDate &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.Balance == 0,
            8 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate != null && x.TerminationDate < fiscalEndDate &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.Balance > 0,
            10 => x =>
                x.BadgeNumber == 0, // Non-employee beneficiaries (not in Demographics/PayProfit)
            11 => x => x.BadgeNumber == 0,
            _ => x => true
        };
    }

    // Helper to get Description from enum
    private static string GetEnumDescription(YearEndProfitSharingReportId value)
    {
        var field = typeof(YearEndProfitSharingReportId).GetField(value.ToString());
        var attr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
        return attr?.Description ?? value.ToString();
    }
}
