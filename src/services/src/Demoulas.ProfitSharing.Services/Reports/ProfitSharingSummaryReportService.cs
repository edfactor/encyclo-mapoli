using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Report;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Text.Json;
using System;
using System.Linq;

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
    }

    private sealed record EmployeeWithBalance
    {
        public required EmployeeProjection Employee { get; init; } = null!;
        public required short ProfitYear { get; init; }
        public required decimal Balance { get; init; }

        public required short PriorProfitYear { get; init; }
        public required decimal PriorBalance { get; init; }
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

    /// <summary>
    /// Generates the year-end profit sharing summary report, grouped by eligibility and status.
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
      var hoursThreshold = ReferenceData.MinimumHoursForContribution();
        
        // Build the base query for the year once
        IQueryable<YearEndProfitSharingReportDetail> activeDetails = await ActiveSummary(req, calInfo.FiscalEndDate);

        // Pre-flag each row with the report line conditions, then aggregate once in the DB
        var flagged = activeDetails.Select(x => new
        {
            x.Wages,
            x.Hours,
            x.Points,
            x.Balance,
            x.PriorBalance,
            x.EmployeeStatus,
            x.TerminationDate,
            x.DateOfBirth,

            // Active + Inactive
        C1 = (((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate > calInfo.FiscalEndDate))
            && x.Hours >= hoursThreshold && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21),
        C2 = (((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate))
            && x.Hours >= hoursThreshold && x.DateOfBirth <= birthday21),
        C3 = (((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate))
                  && x.DateOfBirth > birthday18),
        C4 = (((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate))
            && x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0),
        C5 = (((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate))
            && x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance == 0),

            // Terminated
            C6 = (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate < calInfo.FiscalEndDate
            && x.Hours >= hoursThreshold && x.DateOfBirth <= birthday18),
            C7 = (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate
            && x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance == 0),
            C8 = (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate
            && x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0),
            // Special totals count for line 8 (ignores termination window)
        C8TotalsOnly = (x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0),

            C10 = (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate
                  && x.Wages == 0 && x.DateOfBirth > birthday18)
        });

        var agg = await flagged
            .GroupBy(_ => 1)
            .Select(g => new
            {
                // Line 1
                L1_Count = g.Sum(y => y.C1 ? 1 : 0),
                L1_Wages = g.Sum(y => y.C1 ? y.Wages : 0m),
                L1_Hours = g.Sum(y => y.C1 ? y.Hours : 0m),
                L1_Points = g.Sum(y => y.C1 ? y.Points : 0),
                L1_Balance = g.Sum(y => y.C1 ? y.Balance : 0m),
                L1_PriorBalance = g.Sum(y => y.C1 ? y.PriorBalance : 0m),

                // Line 2
                L2_Count = g.Sum(y => y.C2 ? 1 : 0),
                L2_Wages = g.Sum(y => y.C2 ? y.Wages : 0m),
                L2_Hours = g.Sum(y => y.C2 ? y.Hours : 0m),
                L2_Points = g.Sum(y => y.C2 ? y.Points : 0),
                L2_Balance = g.Sum(y => y.C2 ? y.Balance : 0m),
                L2_PriorBalance = g.Sum(y => y.C2 ? y.PriorBalance : 0m),

                // Line 3
                L3_Count = g.Sum(y => y.C3 ? 1 : 0),
                L3_Wages = g.Sum(y => y.C3 ? y.Wages : 0m),
                L3_Hours = g.Sum(y => y.C3 ? y.Hours : 0m),
                L3_Points = g.Sum(y => y.C3 ? y.Points : 0),
                L3_Balance = g.Sum(y => y.C3 ? y.Balance : 0m),
                L3_PriorBalance = g.Sum(y => y.C3 ? y.PriorBalance : 0m),

                // Line 4
                L4_Count = g.Sum(y => y.C4 ? 1 : 0),
                L4_Wages = g.Sum(y => y.C4 ? y.Wages : 0m),
                L4_Hours = g.Sum(y => y.C4 ? y.Hours : 0m),
                L4_Points = g.Sum(y => y.C4 ? y.Points : 0),
                L4_Balance = g.Sum(y => y.C4 ? y.Balance : 0m),
                L4_PriorBalance = g.Sum(y => y.C4 ? y.PriorBalance : 0m),

                // Line 5
                L5_Count = g.Sum(y => y.C5 ? 1 : 0),
                L5_Wages = g.Sum(y => y.C5 ? y.Wages : 0m),
                L5_Hours = g.Sum(y => y.C5 ? y.Hours : 0m),
                L5_Points = g.Sum(y => y.C5 ? y.Points : 0),
                L5_Balance = g.Sum(y => y.C5 ? y.Balance : 0m),
                L5_PriorBalance = g.Sum(y => y.C5 ? y.PriorBalance : 0m),

                // Line 6 (Terminated 18+ with 1000+ hours)
                L6_Count = g.Sum(y => y.C6 ? 1 : 0),
                L6_Wages = g.Sum(y => y.C6 ? y.Wages : 0m),
                L6_Hours = g.Sum(y => y.C6 ? y.Hours : 0m),
                L6_Points = g.Sum(y => y.C6 ? y.Points : 0),
                L6_Balance = g.Sum(y => y.C6 ? y.Balance : 0m),
                L6_PriorBalance = g.Sum(y => y.C6 ? y.PriorBalance : 0m),

                // Line 7 (Terminated 18+ <1000 no prior)
                L7_Count = g.Sum(y => y.C7 ? 1 : 0),
                L7_Wages = g.Sum(y => y.C7 ? y.Wages : 0m),
                L7_Hours = g.Sum(y => y.C7 ? y.Hours : 0m),
                L7_Points = g.Sum(y => y.C7 ? y.Points : 0),
                L7_Balance = g.Sum(y => y.C7 ? y.Balance : 0m),
                L7_PriorBalance = g.Sum(y => y.C7 ? y.PriorBalance : 0m),

                // Line 8 (Terminated 18+ <1000 with prior) + special totals-only count
                L8_Count = g.Sum(y => y.C8 ? 1 : 0),
                L8_CountTotalsOnly = g.Sum(y => y.C8TotalsOnly ? 1 : 0),
                L8_Wages = g.Sum(y => y.C8 ? y.Wages : 0m),
                L8_Hours = g.Sum(y => y.C8 ? y.Hours : 0m),
                L8_Points = g.Sum(y => y.C8 ? y.Points : 0),
                L8_Balance = g.Sum(y => y.C8 ? y.Balance : 0m),
                L8_PriorBalance = g.Sum(y => y.C8 ? y.PriorBalance : 0m),

                // Line 10 (Terminated under 18 with no wages)
                L10_Count = g.Sum(y => y.C10 ? 1 : 0),
                L10_Wages = g.Sum(y => y.C10 ? y.Wages : 0m),
                L10_Hours = g.Sum(y => y.C10 ? y.Hours : 0m),
                L10_Points = g.Sum(y => y.C10 ? y.Points : 0),
                L10_Balance = g.Sum(y => y.C10 ? y.Balance : 0m),
                L10_PriorBalance = g.Sum(y => y.C10 ? y.PriorBalance : 0m)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var lineItems = new List<YearEndProfitSharingReportSummaryLineItem?>();

        // Helper to map aggregation to line item objects
        if (agg != null)
        {
            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age18To20With1000Hours).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age18To20With1000Hours),
                NumberOfMembers = agg.L1_Count,
                TotalWages = agg.L1_Wages,
                TotalHours = agg.L1_Hours,
                TotalPoints = agg.L1_Points,
                TotalBalance = agg.L1_Balance,
                TotalPriorBalance = agg.L1_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age21OrOlderWith1000Hours).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age21OrOlderWith1000Hours),
                NumberOfMembers = agg.L2_Count,
                TotalWages = agg.L2_Wages,
                TotalHours = agg.L2_Hours,
                TotalPoints = agg.L2_Points,
                TotalBalance = agg.L2_Balance,
                TotalPriorBalance = agg.L2_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Under18).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Under18),
                NumberOfMembers = agg.L3_Count,
                TotalWages = agg.L3_Wages,
                TotalHours = agg.L3_Hours,
                TotalPoints = agg.L3_Points,
                TotalBalance = agg.L3_Balance,
                TotalPriorBalance = agg.L3_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount),
                NumberOfMembers = agg.L4_Count,
                TotalWages = agg.L4_Wages,
                TotalHours = agg.L4_Hours,
                TotalPoints = agg.L4_Points,
                TotalBalance = agg.L4_Balance,
                TotalPriorBalance = agg.L4_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                NumberOfMembers = agg.L5_Count,
                TotalWages = agg.L5_Wages,
                TotalHours = agg.L5_Hours,
                TotalPoints = agg.L5_Points,
                TotalBalance = agg.L5_Balance,
                TotalPriorBalance = agg.L5_PriorBalance
            });

            // Terminated lines
            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours),
                NumberOfMembers = agg.L6_Count,
                TotalWages = agg.L6_Wages,
                TotalHours = agg.L6_Hours,
                TotalPoints = agg.L6_Points,
                TotalBalance = agg.L6_Balance,
                TotalPriorBalance = agg.L6_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                NumberOfMembers = agg.L7_Count,
                TotalWages = agg.L7_Wages,
                TotalHours = agg.L7_Hours,
                TotalPoints = agg.L7_Points,
                TotalBalance = agg.L7_Balance,
                TotalPriorBalance = agg.L7_PriorBalance
            });

            // For this line, NumberOfMembers uses the broader totals-only condition (matches prior behavior)
            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount),
                NumberOfMembers = agg.L8_CountTotalsOnly,
                TotalWages = agg.L8_Wages,
                TotalHours = agg.L8_Hours,
                TotalPoints = agg.L8_Points,
                TotalBalance = agg.L8_Balance,
                TotalPriorBalance = agg.L8_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedUnder18NoWages).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedUnder18NoWages),
                NumberOfMembers = agg.L10_Count,
                TotalWages = agg.L10_Wages,
                TotalHours = agg.L10_Hours,
                TotalPoints = agg.L10_Points,
                TotalBalance = agg.L10_Balance,
                TotalPriorBalance = agg.L10_PriorBalance
            });
        }

        // Special query for non-employee beneficiaries (line N)
        YearEndProfitSharingReportSummaryLineItem? beneficiaryLineItem = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var beneQry = from bc in ctx.BeneficiaryContacts
                          join d in demographics on bc.Ssn equals d.Ssn into gj
                          from d in gj.DefaultIfEmpty()
                          where d == null
                          join tot in _totalService.GetTotalBalanceSet(ctx, req.ProfitYear) on bc.Ssn equals tot.Ssn
                          select new { bc, tot };

            return await beneQry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "N",
                LineItemTitle = "NON-EMPLOYEE BENEFICIARIES",
                NumberOfMembers = x.Count(),
                TotalWages = 0m,
                TotalHours = 0m,
                TotalPoints = 0,
                TotalBalance = x.Sum(y => y.tot.Total ?? 0m),
                TotalPriorBalance = 0m
            }).FirstOrDefaultAsync(cancellationToken);
        });
        
        if (beneficiaryLineItem != null)
        {
            lineItems.Add(beneficiaryLineItem);
        }

        return new YearEndProfitSharingReportSummaryResponse { LineItems = lineItems.Where(li => li != null).ToList()! };
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

        // Always fetch all details for the year
        IQueryable<YearEndProfitSharingReportDetail> allDetails = await ActiveSummary(req, calInfo.FiscalEndDate);

        // Apply report-specific filtering for ReportId 1-8, 10
        IQueryable<YearEndProfitSharingReportDetail> filteredDetails = allDetails;
        var reportIdInt = (int)req.ReportId;
        if (reportIdInt is >= 1 and <= 8 or 10)
        {
            var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
            var birthday21 = calInfo.FiscalEndDate.AddYears(-21);

            var filter = GetReportFilter(reportIdInt, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21);
            filteredDetails = allDetails.Where(filter);
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

        // Totals (use filteredDetails for counts)
        ProfitShareTotal totals = new ProfitShareTotal
        {
            WagesTotal = filteredDetails.Sum(x => x.Wages),
            HoursTotal = filteredDetails.Sum(x => x.Hours),
            PointsTotal = filteredDetails.Sum(x => x.Points),
            BalanceTotal = filteredDetails.Sum(x => x.Balance),
            NumberOfEmployees = details.Total,
            NumberOfNewEmployees = filteredDetails.Count(x => x.YearsInPlan == 0),
        };

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
            TerminatedWagesTotal = totals.TerminatedWagesTotal,
            TerminatedHoursTotal = totals.TerminatedHoursTotal,
            TerminatedPointsTotal = totals.TerminatedPointsTotal,
            TerminatedBalanceTotal = totals.TerminatedBalanceTotal,
            NumberOfEmployees = totals.NumberOfEmployees,
            NumberOfNewEmployees = totals.NumberOfNewEmployees,
            NumberOfEmployeesInPlan = totals.NumberOfEmployees - totals.NumberOfEmployeesUnder21 - totals.NumberOfNewEmployees,
            NumberOfEmployeesUnder21 = totals.NumberOfEmployeesUnder21
        };
        return response;
    }

    public async Task<YearEndProfitSharingReportTotals> GetYearEndProfitSharingTotalsAsync(
        BadgeNumberRequest req,
        CancellationToken cancellationToken = default
    )
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
        var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
        var birthday65 = calInfo.FiscalEndDate.AddYears(-65);
        var lastYear = req.ProfitYear - 1;
        var fiscalBeginDate = calInfo.FiscalBeginDate;
        var fiscalEndDate = calInfo.FiscalEndDate;
    var hoursThreshold = ReferenceData.MinimumHoursForContribution();
        // Always fetch all details for the year
        IQueryable<YearEndProfitSharingReportDetail> allDetails = await ActiveSummary(req, calInfo.FiscalEndDate);
        
    allDetails = allDetails.Where(x => ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
        x.Hours >= hoursThreshold && x.DateOfBirth <= birthday18);

        var totals = await (
            from a in allDetails
            group a by true into g
            select new
            {
                NumberOfEmployees = g.Count(),
                NumberOfNewEmployees = g.Count(x => ((x.FirstContributionYear == null) && x.DateOfBirth <= birthday21)),
                NumberOfEmployeesUnder21 = g.Count(x => x.DateOfBirth > birthday21),
                WagesTotal = g.Where(x => x.DateOfBirth < birthday21).Sum(x => x.Wages),
                HoursTotal = g.Where(x => x.DateOfBirth < birthday21).Sum(x => x.Hours),
                PointsTotal = g.Where(x => x.DateOfBirth <= birthday21 && x.DateOfBirth > birthday65).Sum(x => x.Wages / 100m),
            }
            ).FirstOrDefaultAsync(cancellationToken);

        if (totals == null)
        {
            return new YearEndProfitSharingReportTotals();
        }

        IQueryable<YearEndProfitSharingReportDetail> terminatedDetails = (await ActiveSummary(req, calInfo.FiscalEndDate))
            .Where(x => x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate >= fiscalBeginDate && x.TerminationDate <= fiscalEndDate);

        var terminatedTotals = await (
            from t in terminatedDetails
            group t by true into g
            select new
            {
                TerminatedWagesTotal = g.Sum(x => x.Wages),
                TerminatedHoursTotal = g.Sum(x => x.Hours),
                TerminatedPointsTotal = g.Where(x => x.DateOfBirth <= birthday21 && x.DateOfBirth > birthday65).Sum(x => x.Wages / 100m),
            }
        ).FirstOrDefaultAsync(cancellationToken);

        var rslt = new YearEndProfitSharingReportTotals
        {
            NumberOfEmployees = totals.NumberOfEmployees,
            NumberOfNewEmployees = totals.NumberOfNewEmployees,
            NumberOfEmployeesUnder21 = totals.NumberOfEmployeesUnder21,
            WagesTotal = totals.WagesTotal,
            HoursTotal = totals.HoursTotal,
            PointsTotal = totals.PointsTotal,
            TerminatedHoursTotal = terminatedTotals?.TerminatedHoursTotal ?? 0,
            TerminatedWagesTotal = terminatedTotals?.TerminatedWagesTotal ?? 0,
            TerminatedPointsTotal = terminatedTotals?.TerminatedPointsTotal ?? 0,
        };

        return rslt;

    }

    /// <summary>
    /// Builds a filtered set of employees with balances and years of service for a given year and optional badge number.
    /// </summary>
    private async Task<IQueryable<EmployeeWithBalance>> BuildFilteredEmployeeSetAsync(
        ProfitSharingReadOnlyDbContext ctx,
        ProfitYearRequest req,
        int? badgeNumber)
    {
        IQueryable<PayProfit> basePayProfits = ctx.PayProfits
            .Where(p => p.ProfitYear == req.ProfitYear);

        var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);
        // No IsYearEnd check, always join
        basePayProfits = basePayProfits
            .Join(demographicQuery, p => p.DemographicId, d => d.Id, (p, _) => p);

    var yearsOfService = _totalService.GetYearsOfService(ctx, req.ProfitYear);
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
                Hours = pp.CurrentHoursYear + pp.HoursExecutive,
                Wages = pp.CurrentIncomeYear + pp.IncomeExecutive,
                DateOfBirth = pp.Demographic!.DateOfBirth,
                EmploymentStatusId = pp.Demographic!.EmploymentStatusId,
                TerminationDate = pp.Demographic!.TerminationDate,
                Ssn = pp.Demographic!.Ssn,
                FullName = pp.Demographic!.ContactInfo.FullName,
                StoreNumber = pp.Demographic!.StoreNumber,
                EmploymentTypeId = pp.Demographic!.EmploymentTypeId,
                EmploymentTypeName = et.Name,
                PointsEarned = pp.PointsEarned,
                Years = yip.Years,
                FirstContributionYear = fc.FirstContributionYear
            };
      

        if (badgeNumber.HasValue)
        {
            employeeQry = employeeQry.Where(e => e.BadgeNumber == badgeNumber);
        }

    var beginningBalance = (short)(req.ProfitYear - 1);
        var balances = _totalService.GetTotalBalanceSet(ctx, beginningBalance);
        var priorBalances = _totalService.GetTotalBalanceSet(ctx, (short)(beginningBalance - 1));
        var employeeWithBalanceQry =
            from e in employeeQry
            join bal in balances on e.Ssn equals bal.Ssn into balTmp
            from bal in balTmp.DefaultIfEmpty()
            join priorBal in priorBalances on e.Ssn equals priorBal.Ssn into priorBalTmp
            from priorBal in priorBalTmp.DefaultIfEmpty()
            select new EmployeeWithBalance
            {
                Employee = e,
                ProfitYear = req.ProfitYear,
        Balance = (decimal)(bal != null && bal.Total != null ? bal.Total : 0m),
                PriorProfitYear = (short)(req.ProfitYear - 1),
        PriorBalance = (decimal)(priorBal != null && priorBal.Total != null ? priorBal.Total : 0m)
            };
        return employeeWithBalanceQry;
    }

    /// <summary>
    /// Returns a queryable of year-end profit sharing report details for the given request.
    /// </summary>
    private Task<IQueryable<YearEndProfitSharingReportDetail>> ActiveSummary(BadgeNumberRequest req, DateOnly ageAsOfDate)
    {
        return ActiveSummary(req, ageAsOfDate, req.BadgeNumber);
    }

    /// <summary>
    /// Returns a queryable of year-end profit sharing report details for the given year and optional badge number.
    /// </summary>
    private async Task<IQueryable<YearEndProfitSharingReportDetail>> ActiveSummary(ProfitYearRequest req, DateOnly ageAsOfDate, int? badgeNumber = null)
    {
        var employees = await _dataContextFactory.UseReadOnlyContext(ctx => BuildFilteredEmployeeSetAsync(ctx, req, badgeNumber));

      

    // Always fetch all details for the year
    IQueryable<YearEndProfitSharingReportDetail> allDetails = employees.Select(x => new YearEndProfitSharingReportDetail
        {
            BadgeNumber = x.Employee.BadgeNumber,
            ProfitYear = x.ProfitYear,
            PriorProfitYear = x.PriorProfitYear,
            EmployeeName = x.Employee.FullName!,
            StoreNumber = x.Employee.StoreNumber,
            EmployeeTypeCode = x.Employee.EmploymentTypeId,
            EmployeeTypeName = x.Employee.EmploymentTypeName,
            DateOfBirth = x.Employee.DateOfBirth,
            Age = (byte)x.Employee.DateOfBirth.Age(ageAsOfDate.ToDateTime(TimeOnly.MaxValue)),
            Ssn = x.Employee.Ssn.MaskSsn(),
            Wages = x.Employee.Wages,
            Hours = x.Employee.Hours,
            Points = Convert.ToInt16(x.Employee.PointsEarned),
            IsNew = x.Balance == 0m && x.Employee.Hours > ReferenceData.MinimumHoursForContribution() && (x.Employee.Years == null || x.Employee.Years == 0),
            // Use fiscal end-date derived cutoff rather than DateTime.UtcNow for correctness and translation
            IsUnder21 = x.Employee.DateOfBirth > ageAsOfDate.AddYears(-21),
            EmployeeStatus = x.Employee.EmploymentStatusId,
            Balance = x.Balance,
            PriorBalance = x.PriorBalance,
            YearsInPlan = x.Employee.Years ?? 0,
            TerminationDate = x.Employee.TerminationDate,
            FirstContributionYear = x.Employee.FirstContributionYear
        });

        return allDetails;
    }

    /// <summary>
    /// Returns the filter expression for a given report ID or line number.
    /// </summary>
    /// <param name="reportId">The report ID or line number.</param>
    /// <param name="fiscalBeginDate">Fiscal year begin date.</param>
    /// <param name="fiscalEndDate">Fiscal year end date.</param>
    /// <param name="birthday18">Date representing 18 years before fiscal end.</param>
    /// <param name="birthday21">Date representing 21 years before fiscal end.</param>
    /// <returns>Filter expression for the report line.</returns>
    private static Expression<Func<YearEndProfitSharingReportDetail, bool>> GetReportFilter(
        int reportId,
        DateOnly fiscalBeginDate,
        DateOnly fiscalEndDate,
        DateOnly birthday18,
        DateOnly birthday21)
    {
        return reportId switch
        {
            1 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate > fiscalEndDate)) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21,
            2 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday21,
            3 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.DateOfBirth > birthday18,
            4 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
            5 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance == 0,
            6 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate < fiscalEndDate &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday18,
            7 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= fiscalEndDate && x.TerminationDate >= fiscalBeginDate &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance == 0,
            8 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= fiscalEndDate && x.TerminationDate >= fiscalBeginDate &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
            10 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= fiscalEndDate && x.TerminationDate >= fiscalBeginDate &&
                x.Wages == 0 && x.DateOfBirth > birthday18,
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
