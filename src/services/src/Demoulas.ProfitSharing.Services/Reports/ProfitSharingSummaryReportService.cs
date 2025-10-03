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

    static ProfitSharingSummaryReportService()
    {
        // Set the hours threshold for profit sharing eligibility
        _hoursThreshold = ReferenceData.MinimumHoursForContribution();
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

        // Helper to aggregate a list of details into summary values
        static (int Count, decimal TotalWages, decimal TotalHours, int TotalPoints, decimal TotalBalance, decimal TotalPriorBalance) AggregateDetails(
            List<YearEndProfitSharingReportDetail> details)
        {
            return (
                details.Count,
                details.Sum(y => y.Wages),
                details.Sum(y => y.Hours),
                details.Sum(y => y.Points),
                details.Sum(y => y.Balance),
                details.Sum(y => y.PriorBalance)
            );
        }

        /// <summary>
        /// Creates a summary line item for a specific group of employees.
        /// </summary>
        /// <param name="subgroup">The subgroup label.</param>
        /// <param name="prefix">The line item prefix.</param>
        /// <param name="title">The line item title.</param>
        /// <param name="details">Queryable of report details.</param>
        /// <param name="mainFilter">Main filter for the group.</param>
        /// <param name="totalsFilter">Optional filter for totals.</param>
        /// <returns>Summary line item for the group.</returns>
        async Task<YearEndProfitSharingReportSummaryLineItem?> CreateLine(
            string subgroup, string prefix, string title,
            IQueryable<YearEndProfitSharingReportDetail> details,
            Expression<Func<YearEndProfitSharingReportDetail, bool>> mainFilter,
            Expression<Func<YearEndProfitSharingReportDetail, bool>>? totalsFilter = null // optional
        )
        {
            var mainGroup = await details.Where(mainFilter).ToListAsync(cancellationToken);

            var mainAgg = AggregateDetails(mainGroup);
            var response = new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = subgroup,
                LineItemPrefix = prefix,
                LineItemTitle = title,
                NumberOfMembers = mainAgg.Count,
                TotalWages = mainAgg.TotalWages,
                TotalHours = mainAgg.TotalHours,
                TotalPoints = mainAgg.TotalPoints,
                TotalBalance = mainAgg.TotalBalance,
                TotalPriorBalance = mainAgg.TotalPriorBalance
            };

            if (totalsFilter != null)
            {
                List<YearEndProfitSharingReportDetail> totalsGroup = await details.Where(totalsFilter).ToListAsync(cancellationToken);
                var totalsAgg = AggregateDetails(totalsGroup);
                response.NumberOfMembers = totalsAgg.Count;
            }

            return response;
        }



        // Special query for non-employee beneficiaries (line N)
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var activeDetails = await ActiveSummary(ctx, req, calInfo.FiscalEndDate);

            var lineItems = new List<YearEndProfitSharingReportSummaryLineItem?>
            {
                await CreateLine("Active and Inactive", ((int)YearEndProfitSharingReportId.Age18To20With1000Hours).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age18To20With1000Hours), activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.Age18To20With1000Hours, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21)),
                await CreateLine("Active and Inactive", ((int)YearEndProfitSharingReportId.Age21OrOlderWith1000Hours).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age21OrOlderWith1000Hours), activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.Age21OrOlderWith1000Hours, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21)),
                await CreateLine("Active and Inactive", ((int)YearEndProfitSharingReportId.Under18).ToString(), GetEnumDescription(YearEndProfitSharingReportId.Under18),
                    activeDetails, GetReportFilter((int)YearEndProfitSharingReportId.Under18, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21)),
                await CreateLine("Active and Inactive", ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount), activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndPriorAmount, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18,
                        birthday21)),
                await CreateLine("Active and Inactive", ((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount), activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.Age18OrOlderWithLessThan1000HoursAndNoPriorAmount, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18,
                        birthday21)),

                // Terminated lines
                await CreateLine("TERMINATED", ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours), activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWith1000Hours, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21)),
                await CreateLine(
                    "TERMINATED",
                    ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount),
                    activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndNoPriorAmount, calInfo.FiscalBeginDate, calInfo.FiscalEndDate,
                        birthday18, birthday21)
                ),
                await CreateLine(
                    "TERMINATED",
                    ((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount),
                    activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.TerminatedAge18OrOlderWithLessThan1000HoursAndPriorAmount, calInfo.FiscalBeginDate, calInfo.FiscalEndDate,
                        birthday18, birthday21),
                    x => x.Hours >= 0 && x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0
                ),
                await CreateLine("TERMINATED", ((int)YearEndProfitSharingReportId.TerminatedUnder18NoWages).ToString(),
                    GetEnumDescription(YearEndProfitSharingReportId.TerminatedUnder18NoWages), activeDetails,
                    GetReportFilter((int)YearEndProfitSharingReportId.TerminatedUnder18NoWages, calInfo.FiscalBeginDate, calInfo.FiscalEndDate, birthday18, birthday21))
            };

            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx, req.UseFrozenData);
            var beneQry = ctx.BeneficiaryContacts
                .Where(bc => !demographics.Any(x => x.Ssn == bc.Ssn))
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            var beneficiaryLineItem = await beneQry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "N",
                LineItemTitle = "NON-EMPLOYEE BENEFICIARIES",
                NumberOfMembers = x.Count(),
                TotalWages = 0,
                TotalHours = 0,
                TotalPoints = 0,
                TotalBalance = x.Sum(y => y.tot.TotalAmount ?? 0),
                TotalPriorBalance = 0
            }).FirstOrDefaultAsync(cancellationToken);


            if (beneficiaryLineItem != null)
            {
                lineItems.Add(beneficiaryLineItem);
            }

            return new YearEndProfitSharingReportSummaryResponse { LineItems = lineItems.Where(li => li != null).ToList()! };
        });
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

            // Always fetch all details for the year
            IQueryable<YearEndProfitSharingReportDetail> allDetails = await ActiveSummary(ctx, req, calInfo.FiscalEndDate);

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

            ProfitShareTotal totals = new ProfitShareTotal
            {
                WagesTotal = filteredTotals?.WagesTotal ?? 0,
                HoursTotal = filteredTotals?.HoursTotal ?? 0,
                PointsTotal = filteredTotals?.PointsTotal ?? 0,
                BalanceTotal = filteredTotals?.BalanceTotal ?? 0,
                NumberOfEmployees = filteredTotals?.NumberOfEmployees ?? 0,
                NumberOfNewEmployees = filteredTotals?.NumberOfNewEmployees ?? 0,
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
                NumberOfEmployees = totals.NumberOfEmployees,
                NumberOfNewEmployees = totals.NumberOfNewEmployees,
                NumberOfEmployeesInPlan = totals.NumberOfEmployees - totals.NumberOfEmployeesUnder21 - totals.NumberOfNewEmployees,
                NumberOfEmployeesUnder21 = totals.NumberOfEmployeesUnder21
            };
            return response;
        });
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
        var fiscalBeginDate = calInfo.FiscalBeginDate;
        var fiscalEndDate = calInfo.FiscalEndDate;


        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            // Always fetch all details for the year
            IQueryable<YearEndProfitSharingReportDetail> allDetails = await ActiveSummary(ctx, req, calInfo.FiscalEndDate);

            // Include employees with >= 1000 hours and age >= 18, OR age >= 64 (regardless of hours)
            allDetails = allDetails.Where(x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                ((x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday18) || (x.DateOfBirth <= birthday64)));

            var totals = await (
                from a in allDetails
                group a by true
                into g
                select new
                {
                    NumberOfEmployees = g.Count(x => x.DateOfBirth > birthday21 || x.Hours >= _hoursThreshold),
                    NumberOfNewEmployees = g.Count(x => ((x.FirstContributionYear == null) && x.DateOfBirth <= birthday21)),
                    NumberOfEmployeesUnder21 = g.Count(x => x.DateOfBirth > birthday21),
                    // Exclude age < 21 and age 64+ with < 1000 hours from totals
                    WagesTotal = g.Where(x => x.DateOfBirth <= birthday21 && !(x.DateOfBirth <= birthday64 && x.Hours < _hoursThreshold)).Sum(x => x.Wages),
                    HoursTotal = g.Where(x => x.DateOfBirth <= birthday21 && !(x.DateOfBirth <= birthday64 && x.Hours < _hoursThreshold)).Sum(x => Math.Truncate(x.Hours)),
                    PointsTotal = g.Where(x => x.DateOfBirth <= birthday21 && !(x.DateOfBirth <= birthday64 && x.Hours < _hoursThreshold)).Sum(x => Math.Round(x.Wages / 100)),
                }
            ).FirstOrDefaultAsync(cancellationToken);

            if (totals == null)
            {
                return new YearEndProfitSharingReportTotals();
            }

            var rslt = new YearEndProfitSharingReportTotals
            {
                NumberOfEmployees = totals.NumberOfEmployees,
                NumberOfNewEmployees = totals.NumberOfNewEmployees,
                NumberOfEmployeesUnder21 = totals.NumberOfEmployeesUnder21,
                WagesTotal = totals.WagesTotal,
                HoursTotal = totals.HoursTotal,
                PointsTotal = totals.PointsTotal,
            };

            return rslt;
        });

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
        // No IsYearEnd check, always join
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
                FirstContributionYear = fc.FirstContributionYear,
                IsExecutive = pp.Demographic!.PayFrequencyId == PayFrequency.Constants.Monthly
            };


        if (badgeNumber.HasValue)
        {
            employeeQry = employeeQry.Where(e => e.BadgeNumber == badgeNumber);
        }

        var beginningBalanceYear = (short)(req.ProfitYear - 1);
        var balances = _totalService.GetTotalBalanceSet(ctx, beginningBalanceYear);
        var priorBalances = _totalService.GetTotalBalanceSet(ctx, (short)(beginningBalanceYear - 1));
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
                Balance = (decimal)(bal != null && bal.TotalAmount != null ? bal.TotalAmount : 0),
                PriorProfitYear = beginningBalanceYear,
                PriorBalance = (decimal)(priorBal != null && priorBal.TotalAmount != null ? priorBal.TotalAmount : 0)
            };
        return employeeWithBalanceQry;
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
            IsNew = x.Balance == 0 && x.Employee.Hours > ReferenceData.MinimumHoursForContribution() && (x.Employee.Years == null || x.Employee.Years == 0),
            IsUnder21 = (DateTime.UtcNow.Year - x.Employee.DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < x.Employee.DateOfBirth.DayOfYear ? 1 : 0)) < 21,
            EmployeeStatus = x.Employee.EmploymentStatusId,
            Balance = x.Balance,
            PriorBalance = x.PriorBalance,
            YearsInPlan = x.Employee.Years ?? 0,
            TerminationDate = x.Employee.TerminationDate,
            FirstContributionYear = x.Employee.FirstContributionYear,
            IsExecutive = x.Employee.IsExecutive
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
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) ||
                 (x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate > fiscalEndDate)) &&
                x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21,
            2 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday21,
            3 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.DateOfBirth > birthday18,
            4 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
            5 => x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > fiscalEndDate)) &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance == 0,
            6 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate < fiscalEndDate &&
                x.Hours >= _hoursThreshold && x.DateOfBirth <= birthday18,
            7 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= fiscalEndDate && x.TerminationDate >= fiscalBeginDate &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance == 0,
            8 => x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= fiscalEndDate && x.TerminationDate >= fiscalBeginDate &&
                x.Hours < _hoursThreshold && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
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
