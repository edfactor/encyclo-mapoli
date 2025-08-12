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

    // Lean row shape for summary aggregation to keep SQL minimal
    private sealed record SummaryRow
    {
        public required decimal Wages { get; init; }
        public required decimal Hours { get; init; }
        public required int Points { get; init; }
        public required decimal Balance { get; init; }
        public required decimal PriorBalance { get; init; }
        public required char EmployeeStatus { get; init; }
        public DateOnly? TerminationDate { get; init; }
        public required DateOnly DateOfBirth { get; init; }
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
        
    // Build a lean source query for the year (only fields needed for aggregation)
    IQueryable<SummaryRow> summarySource = await GetSummarySourceAsync(req);

        // 1) Active at year-end set (Active/Inactive or Terminated after end)
        var activeAtEnd = summarySource.Where(x =>
            (x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive)
            || (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate > calInfo.FiscalEndDate));

        var flaggedActive = activeAtEnd.Select(x => new
        {
            x.Wages,
            x.Hours,
            x.Points,
            x.Balance,
            x.PriorBalance,
            x.DateOfBirth,
            C1 = (x.Hours >= hoursThreshold && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21),
            C2 = (x.Hours >= hoursThreshold && x.DateOfBirth <= birthday21),
            C3 = (x.DateOfBirth > birthday18),
            C4 = (x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0),
            C5 = (x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance == 0),
        });

        var aggActive = await flaggedActive
            .GroupBy(_ => 1)
            .Select(g => new
            {
                L1_Count = g.Sum(y => y.C1 ? 1 : 0),
                L1_Wages = g.Sum(y => y.C1 ? y.Wages : 0m),
                L1_Hours = g.Sum(y => y.C1 ? y.Hours : 0m),
                L1_Points = g.Sum(y => y.C1 ? y.Points : 0),
                L1_Balance = g.Sum(y => y.C1 ? y.Balance : 0m),
                L1_PriorBalance = g.Sum(y => y.C1 ? y.PriorBalance : 0m),

                L2_Count = g.Sum(y => y.C2 ? 1 : 0),
                L2_Wages = g.Sum(y => y.C2 ? y.Wages : 0m),
                L2_Hours = g.Sum(y => y.C2 ? y.Hours : 0m),
                L2_Points = g.Sum(y => y.C2 ? y.Points : 0),
                L2_Balance = g.Sum(y => y.C2 ? y.Balance : 0m),
                L2_PriorBalance = g.Sum(y => y.C2 ? y.PriorBalance : 0m),

                L3_Count = g.Sum(y => y.C3 ? 1 : 0),
                L3_Wages = g.Sum(y => y.C3 ? y.Wages : 0m),
                L3_Hours = g.Sum(y => y.C3 ? y.Hours : 0m),
                L3_Points = g.Sum(y => y.C3 ? y.Points : 0),
                L3_Balance = g.Sum(y => y.C3 ? y.Balance : 0m),
                L3_PriorBalance = g.Sum(y => y.C3 ? y.PriorBalance : 0m),

                L4_Count = g.Sum(y => y.C4 ? 1 : 0),
                L4_Wages = g.Sum(y => y.C4 ? y.Wages : 0m),
                L4_Hours = g.Sum(y => y.C4 ? y.Hours : 0m),
                L4_Points = g.Sum(y => y.C4 ? y.Points : 0),
                L4_Balance = g.Sum(y => y.C4 ? y.Balance : 0m),
                L4_PriorBalance = g.Sum(y => y.C4 ? y.PriorBalance : 0m),

                L5_Count = g.Sum(y => y.C5 ? 1 : 0),
                L5_Wages = g.Sum(y => y.C5 ? y.Wages : 0m),
                L5_Hours = g.Sum(y => y.C5 ? y.Hours : 0m),
                L5_Points = g.Sum(y => y.C5 ? y.Points : 0),
                L5_Balance = g.Sum(y => y.C5 ? y.Balance : 0m),
                L5_PriorBalance = g.Sum(y => y.C5 ? y.PriorBalance : 0m),
            })
            .FirstOrDefaultAsync(cancellationToken);

        // 2) Terminated within the window
        var terminatedWindow = summarySource.Where(x =>
            x.EmployeeStatus == EmploymentStatus.Constants.Terminated
            && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate);

        var flaggedTerminated = terminatedWindow.Select(x => new
        {
            x.Wages,
            x.Hours,
            x.Points,
            x.Balance,
            x.PriorBalance,
            x.DateOfBirth,
            C6 = (x.Hours >= hoursThreshold && x.DateOfBirth <= birthday18),
            C7 = (x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance == 0),
            C8 = (x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0),
            C10 = (x.Wages == 0 && x.DateOfBirth > birthday18)
        });

        var aggTerm = await flaggedTerminated
            .GroupBy(_ => 1)
            .Select(g => new
            {
                L6_Count = g.Sum(y => y.C6 ? 1 : 0),
                L6_Wages = g.Sum(y => y.C6 ? y.Wages : 0m),
                L6_Hours = g.Sum(y => y.C6 ? y.Hours : 0m),
                L6_Points = g.Sum(y => y.C6 ? y.Points : 0),
                L6_Balance = g.Sum(y => y.C6 ? y.Balance : 0m),
                L6_PriorBalance = g.Sum(y => y.C6 ? y.PriorBalance : 0m),

                L7_Count = g.Sum(y => y.C7 ? 1 : 0),
                L7_Wages = g.Sum(y => y.C7 ? y.Wages : 0m),
                L7_Hours = g.Sum(y => y.C7 ? y.Hours : 0m),
                L7_Points = g.Sum(y => y.C7 ? y.Points : 0),
                L7_Balance = g.Sum(y => y.C7 ? y.Balance : 0m),
                L7_PriorBalance = g.Sum(y => y.C7 ? y.PriorBalance : 0m),

                L8_Count = g.Sum(y => y.C8 ? 1 : 0),
                L8_Wages = g.Sum(y => y.C8 ? y.Wages : 0m),
                L8_Hours = g.Sum(y => y.C8 ? y.Hours : 0m),
                L8_Points = g.Sum(y => y.C8 ? y.Points : 0),
                L8_Balance = g.Sum(y => y.C8 ? y.Balance : 0m),
                L8_PriorBalance = g.Sum(y => y.C8 ? y.PriorBalance : 0m),

                L10_Count = g.Sum(y => y.C10 ? 1 : 0),
                L10_Wages = g.Sum(y => y.C10 ? y.Wages : 0m),
                L10_Hours = g.Sum(y => y.C10 ? y.Hours : 0m),
                L10_Points = g.Sum(y => y.C10 ? y.Points : 0),
                L10_Balance = g.Sum(y => y.C10 ? y.Balance : 0m),
                L10_PriorBalance = g.Sum(y => y.C10 ? y.PriorBalance : 0m)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // 3) Special totals-only count for line 8 (ignores termination window)
        var l8TotalsOnly = await summarySource
            .Where(x => x.Hours < hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0)
            .GroupBy(_ => 1)
            .Select(g => g.Sum(_ => 1))
            .FirstOrDefaultAsync(cancellationToken);

        var lineItems = new List<YearEndProfitSharingReportSummaryLineItem?>();

        // Helper to map aggregation to line item objects
        if (aggActive != null)
        {
            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age18To20With1000Hours).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age18To20With1000Hours),
                NumberOfMembers = aggActive.L1_Count,
                TotalWages = aggActive.L1_Wages,
                TotalHours = aggActive.L1_Hours,
                TotalPoints = aggActive.L1_Points,
                TotalBalance = aggActive.L1_Balance,
                TotalPriorBalance = aggActive.L1_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age21OrOlderWith1000Hours).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age21OrOlderWith1000Hours),
                NumberOfMembers = aggActive.L2_Count,
                TotalWages = aggActive.L2_Wages,
                TotalHours = aggActive.L2_Hours,
                TotalPoints = aggActive.L2_Points,
                TotalBalance = aggActive.L2_Balance,
                TotalPriorBalance = aggActive.L2_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Under18).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Under18),
                NumberOfMembers = aggActive.L3_Count,
                TotalWages = aggActive.L3_Wages,
                TotalHours = aggActive.L3_Hours,
                TotalPoints = aggActive.L3_Points,
                TotalBalance = aggActive.L3_Balance,
                TotalPriorBalance = aggActive.L3_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount),
                NumberOfMembers = aggActive.L4_Count,
                TotalWages = aggActive.L4_Wages,
                TotalHours = aggActive.L4_Hours,
                TotalPoints = aggActive.L4_Points,
                TotalBalance = aggActive.L4_Balance,
                TotalPriorBalance = aggActive.L4_PriorBalance
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                NumberOfMembers = aggActive.L5_Count,
                TotalWages = aggActive.L5_Wages,
                TotalHours = aggActive.L5_Hours,
                TotalPoints = aggActive.L5_Points,
                TotalBalance = aggActive.L5_Balance,
                TotalPriorBalance = aggActive.L5_PriorBalance
            });

            // Terminated lines (from aggTerm)
            var term = aggTerm;
            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours),
                NumberOfMembers = term?.L6_Count ?? 0,
                TotalWages = term?.L6_Wages ?? 0m,
                TotalHours = term?.L6_Hours ?? 0m,
                TotalPoints = term?.L6_Points ?? 0,
                TotalBalance = term?.L6_Balance ?? 0m,
                TotalPriorBalance = term?.L6_PriorBalance ?? 0m
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                NumberOfMembers = term?.L7_Count ?? 0,
                TotalWages = term?.L7_Wages ?? 0m,
                TotalHours = term?.L7_Hours ?? 0m,
                TotalPoints = term?.L7_Points ?? 0,
                TotalBalance = term?.L7_Balance ?? 0m,
                TotalPriorBalance = term?.L7_PriorBalance ?? 0m
            });

            // For this line, NumberOfMembers uses the broader totals-only condition (matches prior behavior)
            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount),
                NumberOfMembers = l8TotalsOnly,
                TotalWages = term?.L8_Wages ?? 0m,
                TotalHours = term?.L8_Hours ?? 0m,
                TotalPoints = term?.L8_Points ?? 0,
                TotalBalance = term?.L8_Balance ?? 0m,
                TotalPriorBalance = term?.L8_PriorBalance ?? 0m
            });

            lineItems.Add(new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = ((int)YearEndProfitSharingReportId.TerminatedUnder18NoWages).ToString(),
                LineItemTitle = GetEnumDescription(YearEndProfitSharingReportId.TerminatedUnder18NoWages),
                NumberOfMembers = term?.L10_Count ?? 0,
                TotalWages = term?.L10_Wages ?? 0m,
                TotalHours = term?.L10_Hours ?? 0m,
                TotalPoints = term?.L10_Points ?? 0,
                TotalBalance = term?.L10_Balance ?? 0m,
                TotalPriorBalance = term?.L10_PriorBalance ?? 0m
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

    // Minimal source for summary aggregation, avoids heavy per-row computations (age, masking, strings)
    private async Task<IQueryable<SummaryRow>> GetSummarySourceAsync(BadgeNumberRequest req)
    {
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Base pay profits for the target year
            IQueryable<PayProfit> basePayProfits = ctx.PayProfits
                .Where(p => p.ProfitYear == req.ProfitYear);

            // Restrict to the demographics we include elsewhere
            var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);
            basePayProfits = basePayProfits
                .Join(demographicQuery, p => p.DemographicId, d => d.Id, (p, _) => p);

            if (req.BadgeNumber.HasValue)
            {
                basePayProfits = basePayProfits.Where(p => p.Demographic!.BadgeNumber == req.BadgeNumber);
            }

            // Balances used by summary
            var beginningBalance = (short)(req.ProfitYear - 1);
            var balances = _totalService.GetTotalBalanceSet(ctx, beginningBalance);
            var priorBalances = _totalService.GetTotalBalanceSet(ctx, (short)(beginningBalance - 1));

            var query = from pp in basePayProfits
                        join bal in balances on pp.Demographic!.Ssn equals bal.Ssn into balTmp
                        from bal in balTmp.DefaultIfEmpty()
                        join pbal in priorBalances on pp.Demographic!.Ssn equals pbal.Ssn into pbalTmp
                        from pbal in pbalTmp.DefaultIfEmpty()
                        select new SummaryRow
                        {
                            Wages = pp.CurrentIncomeYear + pp.IncomeExecutive,
                            Hours = pp.CurrentHoursYear + pp.HoursExecutive,
                            Points = (int)(pp.PointsEarned ?? 0),
                            Balance = bal != null && bal.Total != null ? bal.Total!.Value : 0m,
                            PriorBalance = pbal != null && pbal.Total != null ? pbal.Total!.Value : 0m,
                            EmployeeStatus = pp.Demographic!.EmploymentStatusId,
                            TerminationDate = pp.Demographic!.TerminationDate,
                            DateOfBirth = pp.Demographic!.DateOfBirth
                        };

            // Only statuses relevant to summary
            query = query.Where(x => x.EmployeeStatus == EmploymentStatus.Constants.Active
                                  || x.EmployeeStatus == EmploymentStatus.Constants.Inactive
                                  || x.EmployeeStatus == EmploymentStatus.Constants.Terminated);

            return query;
        });
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
