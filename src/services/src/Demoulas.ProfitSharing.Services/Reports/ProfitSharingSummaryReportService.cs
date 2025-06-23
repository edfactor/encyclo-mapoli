using System.Diagnostics;
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
using System.Linq.Expressions;

namespace Demoulas.ProfitSharing.Services.Reports;

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
    }

    private sealed record EmployeeWithBalance
    {
        public required EmployeeProjection Employee { get; init; } = null!;
        public required short ProfitYear { get; init; }
        public required decimal Balance { get; init; }

        public required short PriorProfitYear { get; init; }
        public required decimal PriorBalance { get; init; }
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
        ProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
        var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
        

        async Task<YearEndProfitSharingReportSummaryLineItem?> CreateLine(
            string subgroup, string prefix, string title,
            IQueryable<YearEndProfitSharingReportDetail> details,
            Expression<Func<YearEndProfitSharingReportDetail, bool>> mainFilter,
            Expression<Func<YearEndProfitSharingReportDetail, bool>>? totalsFilter = null // optional
        )
        {
            // All unique employees matching main filter
            var mainGroup = await details
                .Where(mainFilter)
                .ToListAsync(cancellationToken);

            // Only those with >= 100 hours (for totals)
            var totalsGroup = mainGroup;
            if (totalsFilter != null)
            {
                totalsGroup = await details.Where(totalsFilter).ToListAsync(cancellationToken);
            }

            return new YearEndProfitSharingReportSummaryLineItem
            {
                Subgroup = subgroup,
                LineItemPrefix = prefix,
                LineItemTitle = title,
                NumberOfMembers = totalsGroup.Count,
                BadgeNumbers = mainGroup.Select(g => g.BadgeNumber).ToHashSet(),
                TotalWages = mainGroup.Sum(y => y.Wages),
                TotalHours = mainGroup.Sum(y => y.Hours),
                TotalPoints = mainGroup.Sum(y => y.Points),
                TotalBalance = mainGroup.Sum(y => y.Balance),
                TotalPriorBalance = mainGroup.Sum(y => y.PriorBalance)
            };
        }

        var activeDetails = await ActiveSummary(req);

        var lineItems = new List<YearEndProfitSharingReportSummaryLineItem?>
        {
            // Active/Inactive lines
            await CreateLine("Active and Inactive", "1", "AGE 18-20 WITH >= 1000 PS HOURS", activeDetails, x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate)) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21),
            await CreateLine("Active and Inactive", "2", ">= AGE 21 WITH >= 1000 PS HOURS", activeDetails, x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate)) &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday21),
            await CreateLine("Active and Inactive", "3", "<  AGE 18", activeDetails, x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate)) &&
                x.DateOfBirth > birthday18),
            await CreateLine("Active and Inactive", "4", ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT", activeDetails, x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate)) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0),
            await CreateLine("Active and Inactive", "5", ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT", activeDetails, x =>
                ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) || (x.TerminationDate > calInfo.FiscalEndDate)) &&
                x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance == 0),

            // Terminated lines
            await CreateLine("TERMINATED", "6", ">= AGE 18 WITH >= 1000 PS HOURS", activeDetails, x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate < calInfo.FiscalEndDate &&
                x.Hours >= 1000 && x.DateOfBirth <= birthday18),
            await CreateLine(
                "TERMINATED",
                "7",
                ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                activeDetails,
                x =>
                    x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate &&
                    x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance == 0
            ),
            await CreateLine(
                "TERMINATED",
                "8",
                ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                activeDetails,
                x =>
                    x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate &&
                    x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
                x => x.Hours >= 0 && x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0
            ),
            await CreateLine("TERMINATED", "X", "<  AGE 18           NO WAGES :   0", activeDetails, x =>
                x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate &&
                x.Wages == 0 && x.DateOfBirth > birthday18)
        };

        return new YearEndProfitSharingReportSummaryResponse { LineItems = lineItems.Where(li => li != null).ToList()! };
    }

    public async Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(
        YearEndProfitSharingReportRequest req,
        CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);

        // Always fetch all details for the year
        IQueryable<YearEndProfitSharingReportDetail> allDetails = await ActiveSummary(req);

        // Apply report-specific filtering for ReportId 1-8, 10
        IQueryable<YearEndProfitSharingReportDetail> filteredDetails = allDetails;
        if (req.ReportId is >= 1 and <= 8 or 10)
        {
            var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
            var birthday21 = calInfo.FiscalEndDate.AddYears(-21);

            Expression<Func<YearEndProfitSharingReportDetail, bool>> filter = req.ReportId switch
            {
                1 => x =>
                    ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) ||
                     (x.TerminationDate > calInfo.FiscalEndDate))
                    && x.Hours >= 1000 && x.DateOfBirth <= birthday18 && x.DateOfBirth > birthday21,
                2 => x =>
                    ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) ||
                     (x.TerminationDate > calInfo.FiscalEndDate))
                    && x.Hours >= 1000 && x.DateOfBirth <= birthday21,
                3 => x =>
                    ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) ||
                     (x.TerminationDate > calInfo.FiscalEndDate))
                    && x.DateOfBirth > birthday18,
                4 => x =>
                    ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) ||
                     (x.TerminationDate > calInfo.FiscalEndDate))
                    && x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
                5 => x =>
                    ((x.EmployeeStatus == EmploymentStatus.Constants.Active || x.EmployeeStatus == EmploymentStatus.Constants.Inactive) ||
                     (x.TerminationDate > calInfo.FiscalEndDate))
                    && x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance == 0,
                6 => x =>
                    x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate < calInfo.FiscalEndDate
                                                                              && x.Hours >= 1000 && x.DateOfBirth <= birthday18,
                7 => x =>
                    x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate
                    && x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance == 0,
                8 => x =>
                    x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate
                    && x.Hours < 1000 && x.DateOfBirth <= birthday18 && x.PriorBalance > 0,
                10 => x =>
                    x.EmployeeStatus == EmploymentStatus.Constants.Terminated && x.TerminationDate <= calInfo.FiscalEndDate && x.TerminationDate >= calInfo.FiscalBeginDate
                    && x.Wages == 0 && x.DateOfBirth > birthday18,
                _ => x => true
            };
            filteredDetails = allDetails.Where(filter);
        }

        var details = await filteredDetails.ToPaginationResultsAsync(req, cancellationToken);

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


    // Unified method to build filtered employee set with balances and years
    private async Task<IQueryable<EmployeeWithBalance>> BuildFilteredEmployeeSetAsync(
        ProfitSharingReadOnlyDbContext ctx,
        ProfitYearRequest req,
        int? badgeNumber)
    {
        IQueryable<PayProfit> basePayProfits = ctx.PayProfits
            .Where(p => p.ProfitYear == req.ProfitYear)
            .Include(p => p.Demographic)!
            .ThenInclude(d => d!.ContactInfo);

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

        if (badgeNumber.HasValue)
        {
            employeeQry = employeeQry.Where(e => e.BadgeNumber == badgeNumber);
        }

        var balances = _totalService.GetTotalBalanceSet(ctx, req.ProfitYear);
        var priorBalances = _totalService.GetTotalBalanceSet(ctx, (short)(req.ProfitYear - 1));
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
                Balance = (decimal)(bal != null && bal.Total != null ? bal.Total : 0),
                PriorProfitYear = (short)(req.ProfitYear - 1),
                PriorBalance = (decimal)(priorBal != null && priorBal.Total != null ? priorBal.Total : 0)
            };
        return employeeWithBalanceQry;
    }

    private Task<IQueryable<YearEndProfitSharingReportDetail>> ActiveSummary(YearEndProfitSharingReportRequest req)
    {
        return ActiveSummary(req, req.BadgeNumber);
    }

    private async Task<IQueryable<YearEndProfitSharingReportDetail>> ActiveSummary(ProfitYearRequest req, int? badgeNumber = null)
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
            Age = (byte)x.Employee.DateOfBirth.Age(DateTime.UtcNow),
            Ssn = x.Employee.Ssn.MaskSsn(),
            Wages = x.Employee.Wages,
            Hours = x.Employee.Hours,
            Points = Convert.ToInt16(x.Employee.PointsEarned),
            IsNew = x.Balance == 0 && x.Employee.Hours > ReferenceData.MinimumHoursForContribution(),
            IsUnder21 = (DateTime.UtcNow.Year - x.Employee.DateOfBirth.Year - (DateTime.UtcNow.DayOfYear < x.Employee.DateOfBirth.DayOfYear ? 1 : 0)) < 21,
            EmployeeStatus = x.Employee.EmploymentStatusId,
            Balance = x.Balance,
            PriorBalance = x.PriorBalance,
            YearsInPlan = x.Employee.Years ?? 0,
            TerminationDate = x.Employee.TerminationDate
        });

        return allDetails;
    }
}
