using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
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
using Microsoft.Extensions.Hosting;

namespace Demoulas.ProfitSharing.Services.Reports;

public sealed class ProfitSharingSummaryReportService : IProfitSharingSummaryReportService
{
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ICalendarService _calendarService;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;

    private sealed record EmployeeProjection
    {
        public int BadgeNumber { get; init; }
        public decimal Hours { get; init; }
        public decimal Wages { get; init; }
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
    }

    private sealed record EmployeeWithBalance
    {
        public EmployeeProjection Employee { get; init; } = null!;
        public decimal Balance { get; init; }
    }


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

    public async Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(
        FrozenProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var reportReq = new YearEndProfitSharingReportRequest
        {
            ProfitYear = req.ProfitYear,
            IncludeDetails = true,
            IncludeTotals = false,
            IncludeActiveEmployees = true,
            IncludeTerminatedEmployees = true,
            IncludeBeneficiaries = false,
            IsYearEnd = true,
            Take = int.MaxValue
        };
        var report = await GetYearEndProfitSharingReportAsync(reportReq, cancellationToken);
        var details = report.Response.Results.ToList();

        var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
        var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
        var nonTerminatedStatuses = new List<char> { EmploymentStatus.Constants.Active, EmploymentStatus.Constants.Inactive };

        // Use local functions for status checks
        static bool IsActiveOrInactive(char? status, DateOnly? termDate, List<char> nonTerminatedStatuses, DateOnly fiscalEnd) =>
            (status != null && nonTerminatedStatuses.Contains(status.Value)) || (termDate != null && termDate > fiscalEnd);
        static bool IsTerminated(char? status, DateOnly? termDate, DateOnly fiscalEnd) =>
            status == EmploymentStatus.Constants.Terminated && termDate != null && termDate > fiscalEnd;
        static bool IsTerminatedWithinFiscal(char? status, DateOnly? termDate, DateOnly fiscalBegin, DateOnly fiscalEnd) =>
            status == EmploymentStatus.Constants.Terminated && termDate != null && termDate <= fiscalEnd && termDate >= fiscalBegin;

        // Helper to create a summary line
        YearEndProfitSharingReportSummaryLineItem? CreateLine(string subgroup, string prefix, string title, Func<YearEndProfitSharingReportDetail, bool> filter)
        {
            var group = details.Where(filter).ToList();
            if (!group.Any()) { return null; }
            return new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = subgroup,
                LineItemPrefix = prefix,
                LineItemTitle = title,
                NumberOfMembers = group.Count,
                TotalWages = group.Sum(y => y.Wages),
                TotalBalance = group.Sum(y => y.Balance)
            };
        }

        var lineItems = new List<YearEndProfitSharingReportSummaryLineItem?>
        {
            CreateLine("Active and Inactive", "1", "AGE 18-20 WITH >= 1000 PS HOURS", x =>
                IsActiveOrInactive(x.EmployeeStatus, x.TerminationDate, nonTerminatedStatuses, calInfo.FiscalEndDate) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21),
            CreateLine("Active and Inactive", "2", ">= AGE 21 WITH >= 1000 PS HOURS", x =>
                IsActiveOrInactive(x.EmployeeStatus, x.TerminationDate, nonTerminatedStatuses, calInfo.FiscalEndDate) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday21),
            CreateLine("Active and Inactive", "3", "<  AGE 18", x =>
                IsActiveOrInactive(x.EmployeeStatus, x.TerminationDate, nonTerminatedStatuses, calInfo.FiscalEndDate) &&
                x.DateOfBirth > birthday18),
            CreateLine("Active and Inactive", "4", ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT", x =>
                IsActiveOrInactive(x.EmployeeStatus, x.TerminationDate, nonTerminatedStatuses, calInfo.FiscalEndDate) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.Balance > 0),
            CreateLine("Active and Inactive", "5", ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT", x =>
                IsActiveOrInactive(x.EmployeeStatus, x.TerminationDate, nonTerminatedStatuses, calInfo.FiscalEndDate) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.Balance == 0),
            CreateLine("TERMINATED", "6", ">= AGE 18 WITH >= 1000 PS HOURS", x =>
                IsTerminated(x.EmployeeStatus, x.TerminationDate, calInfo.FiscalEndDate) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday18),
            CreateLine("TERMINATED", "7", ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT", x =>
                IsTerminatedWithinFiscal(x.EmployeeStatus, x.TerminationDate, calInfo.FiscalBeginDate, calInfo.FiscalEndDate) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.Balance == 0),
            CreateLine("TERMINATED", "8", ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT", x =>
                IsTerminatedWithinFiscal(x.EmployeeStatus, x.TerminationDate, calInfo.FiscalBeginDate, calInfo.FiscalEndDate) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.Balance != 0),
            CreateLine("TERMINATED", "X", "<  AGE 18           NO WAGES :   0", x =>
                IsTerminatedWithinFiscal(x.EmployeeStatus, x.TerminationDate, calInfo.FiscalBeginDate, calInfo.FiscalEndDate) &&
                x.Wages == 0 && x.DateOfBirth > birthday18)
        };

        return new YearEndProfitSharingReportSummaryResponse
        {
            LineItems = lineItems.Where(li => li != null).ToList()!
        };
    }

    public async Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(
        YearEndProfitSharingReportRequest req,
        CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var employees = await _dataContextFactory.UseReadOnlyContext(ctx =>
            BuildFilteredEmployeeSetAsync(ctx, req, calInfo)
        );

        // Details
        PaginatedResponseDto<YearEndProfitSharingReportDetail>? details = null;
        if (req.IncludeDetails)
        {
            details = await employees.Select(x => new YearEndProfitSharingReportDetail
            {
                BadgeNumber = x.Employee.BadgeNumber,
                EmployeeName = x.Employee.FullName!,
                StoreNumber = x.Employee.StoreNumber,
                EmployeeTypeCode = x.Employee.EmploymentTypeId,
                EmployeeTypeName = x.Employee.EmploymentTypeName,
                DateOfBirth = x.Employee.DateOfBirth,
                Age = (byte)x.Employee.DateOfBirth.Age(DateTime.UtcNow),
                Ssn = x.Employee.Ssn.MaskSsn(),
                Wages = x.Employee.Wages,
                Hours = x.Employee.Hours,
                Points = Convert.ToInt16(x.Employee.PointsEarned),
                IsNew = x.Balance == 0 && x.Employee.Hours > ReferenceData.MinimumHoursForContribution(),
                IsUnder21 = (DateTime.UtcNow.Year - x.Employee.DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < x.Employee.DateOfBirth.DayOfYear ? 1 : 0)) < 21,
                EmployeeStatus = x.Employee.EmploymentStatusId,
                Balance = x.Balance,
                YearsInPlan = x.Employee.Years ?? 0,
                TerminationDate = x.Employee.TerminationDate
            }).ToPaginationResultsAsync(req, cancellationToken);
        }

        // Totals
        ProfitShareTotal? totals = null;
        if (req.IncludeTotals)
        {
            var terminatedStatus = EmploymentStatus.Constants.Terminated;
            // Batch all DB-compatible totals
            var totalsResult = await employees.GroupBy(e => 1).Select(g => new
            {
                WagesTotal = g.Sum(e => e.Employee.Wages),
                HoursTotal = g.Sum(e => e.Employee.Hours),
                PointsTotal = g.Sum(e => e.Employee.PointsEarned ?? 0),
                TerminatedWagesTotal = g.Sum(e => e.Employee.EmploymentStatusId == terminatedStatus ? e.Employee.Wages : 0),
                TerminatedHoursTotal = g.Sum(e => e.Employee.EmploymentStatusId == terminatedStatus ? e.Employee.Hours : 0),
                TerminatedPointsTotal = g.Sum(e => e.Employee.EmploymentStatusId == terminatedStatus ? (e.Employee.PointsEarned ?? 0) : 0),
                NumberOfEmployees = g.Count(),
                NumberOfNewEmployees = g.Sum(e => e.Employee.Years == 0 ? 1 : 0)
            }).FirstOrDefaultAsync(cancellationToken);

            // Do under-21 count in memory
            var birthdates = await employees.Select(e => e.Employee.DateOfBirth).ToListAsync(cancellationToken);
            var numberOfEmployeesUnder21 = birthdates.Count(dob => dob.Age(calInfo.FiscalEndDate.ToDateTime(TimeOnly.MinValue)) < 21);

            totals = totalsResult != null
                ? new ProfitShareTotal
                {
                    WagesTotal = totalsResult.WagesTotal,
                    HoursTotal = totalsResult.HoursTotal,
                    PointsTotal = totalsResult.PointsTotal,
                    TerminatedWagesTotal = totalsResult.TerminatedWagesTotal,
                    TerminatedHoursTotal = totalsResult.TerminatedHoursTotal,
                    TerminatedPointsTotal = totalsResult.TerminatedPointsTotal,
                    NumberOfEmployees = totalsResult.NumberOfEmployees,
                    NumberOfNewEmployees = totalsResult.NumberOfNewEmployees,
                    NumberOfEmployeesUnder21 = numberOfEmployeesUnder21
                }
                : new ProfitShareTotal();
        }

        // Build response
        var response = new YearEndProfitSharingReportResponse
        {
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            ReportName = $"PROFIT SHARE REPORT (PAY426) - {req.ProfitYear}",
            Response = details ?? new PaginatedResponseDto<YearEndProfitSharingReportDetail>(req),
            WagesTotal = totals?.WagesTotal ?? 0,
            HoursTotal = totals?.HoursTotal ?? 0,
            PointsTotal = totals?.PointsTotal ?? 0,
            TerminatedWagesTotal = totals?.TerminatedWagesTotal ?? 0,
            TerminatedHoursTotal = totals?.TerminatedHoursTotal ?? 0,
            TerminatedPointsTotal = totals?.TerminatedPointsTotal ?? 0,
            NumberOfEmployees = totals?.NumberOfEmployees ?? 0,
            NumberOfNewEmployees = totals?.NumberOfNewEmployees ?? 0,
            NumberOfEmployeesInPlan = totals != null ? (totals.NumberOfEmployees - totals.NumberOfEmployeesUnder21 - totals.NumberOfNewEmployees) : 0,
            NumberOfEmployeesUnder21 = totals?.NumberOfEmployeesUnder21 ?? 0
        };
        return response;
    }

    private static IQueryable<EmployeeProjection> ApplyRequestFilters(
        IQueryable<EmployeeProjection> qry,
        YearEndProfitSharingReportRequest req,
        CalendarResponseDto calInfo)
    {

        if (req.MinimumHoursInclusive.HasValue)
        {
            var minHours = req.MinimumHoursInclusive.Value;
            qry = qry.Where(p => p.Hours >= minHours);
        }


        if (req.MaximumHoursInclusive.HasValue)
        {
            qry = qry.Where(p => p.Hours <= req.MaximumHoursInclusive.Value);
        }

        if (req.MinimumAgeInclusive.HasValue)
        {
            var minBirth = calInfo.FiscalEndDate
                .AddYears(-req.MinimumAgeInclusive.Value);
            qry = qry.Where(p => p.DateOfBirth <= minBirth);
        }

        if (req.MaximumAgeInclusive.HasValue)
        {
            var maxBirth = calInfo.FiscalEndDate
                .AddYears(-(req.MaximumAgeInclusive.Value + 1))
                .AddDays(1);
            qry = qry.Where(p => p.DateOfBirth >= maxBirth);
        }

        // employment‑status permutations
        if (req.IncludeActiveEmployees)
        {
            qry = qry.Where(p =>
                (p.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                 p.EmploymentStatusId == EmploymentStatus.Constants.Inactive ||
                 (p.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                  p.TerminationDate > calInfo.FiscalEndDate)));
        }

        else if (req.IncludeTerminatedEmployees)
        {
            qry = qry.Where(p =>
                p.EmploymentStatusId == EmploymentStatus.Constants.Terminated && p.TerminationDate >= calInfo.FiscalEndDate);
        }

        return qry;
    }

    // Unified method to build filtered employee set with balances and years
    private async Task<IQueryable<EmployeeWithBalance>> BuildFilteredEmployeeSetAsync(
        ProfitSharingReadOnlyDbContext ctx,
        YearEndProfitSharingReportRequest req,
        CalendarResponseDto calInfo)
    {
        IQueryable<PayProfit> basePayProfits = ctx.PayProfits
            .Where(p => p.ProfitYear == req.ProfitYear)
            .Include(p => p.Demographic)!
            .ThenInclude(d => d!.ContactInfo);

        var demographicQuery = await _demographicReaderService.BuildDemographicQuery(ctx, true);
        if (req.IsYearEnd)
        {
            basePayProfits = basePayProfits
                .Join(demographicQuery, p => p.DemographicId, d => d.Id, (p, _) => p);
        }

        IQueryable<Beneficiary> beneficiaryQuery = Enumerable.Empty<Beneficiary>().AsQueryable();
        if (req.IncludeBeneficiaries)
        {
            beneficiaryQuery = ctx.Beneficiaries
                .Include(b => b.Contact)!
                .ThenInclude(c => c!.ContactInfo)
                .Where(b => !demographicQuery.Any(x => x.Ssn == b.Contact!.Ssn));
        }

        var yearsOfService = _totalService.GetYearsOfService(ctx, req.ProfitYear);
        var balances = _totalService.GetTotalBalanceSet(ctx, req.ProfitYear);

        var employeeQry =
            from pp in basePayProfits
            join et in ctx.EmploymentTypes on pp.Demographic!.EmploymentTypeId equals et.Id
            join yip in yearsOfService on pp.Demographic!.Ssn equals yip.Ssn into yipTmp
            from yip in yipTmp.DefaultIfEmpty()
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
                Years = yip.Years
            };

        if (req.IncludeBeneficiaries)
        {
            employeeQry = employeeQry.Union(
                from b in beneficiaryQuery
                select new EmployeeProjection
                {
                    BadgeNumber = 0,
                    Hours = 0m,
                    Wages = 0m,
                    DateOfBirth = b.Contact!.DateOfBirth,
                    EmploymentStatusId = EmploymentStatus.Constants.Terminated,
                    TerminationDate = null,
                    Ssn = b.Contact!.Ssn,
                    FullName = b.Contact!.ContactInfo.FullName,
                    StoreNumber = 0,
                    EmploymentTypeId = EmploymentType.Constants.PartTime,
                    EmploymentTypeName = "",
                    PointsEarned = null,
                    Years = 0
                });
        }

        employeeQry = ApplyRequestFilters(employeeQry, req, calInfo);

        var employeeWithBalanceQry =
            from e in employeeQry
            join bal in balances on e.Ssn equals bal.Ssn into balTmp
            from bal in balTmp.DefaultIfEmpty()
            select new EmployeeWithBalance { Employee = e, Balance = (decimal)(bal != null && bal.Total != null ? bal.Total : 0) };

        return employeeWithBalanceQry;
    }
}
