using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
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
    private readonly IEmbeddedSqlService _embeddedSqlService;
    private readonly IHostEnvironment _hostEnvironment;
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
        IEmbeddedSqlService embeddedSqlService,
        IHostEnvironment hostEnvironment,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _embeddedSqlService = embeddedSqlService;
        _hostEnvironment = hostEnvironment;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<YearEndProfitSharingReportSummaryResponse> GetYearEndProfitSharingSummaryReportAsync(
        FrozenProfitYearRequest req, CancellationToken cancellationToken = default)
    {
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        return await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var birthday18 = calInfo.FiscalEndDate.AddYears(-18);
            var birthday21 = calInfo.FiscalEndDate.AddYears(-21);
            var nonTerminatedStatuses =
                new List<char> { EmploymentStatus.Constants.Active, EmploymentStatus.Constants.Inactive };
            var response = new YearEndProfitSharingReportSummaryResponse
            {
                LineItems = new List<YearEndProfitSharingReportSummaryLineItem>()
            };

            // AGE 18-20 WITH >= 1000 PS HOURS
            var qry = ctx.PayProfits.Include(x => x.Demographic)
                .Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) ||
                            (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 &&
                            x.Demographic!.DateOfBirth <= birthday18 && x.Demographic!.DateOfBirth > birthday21)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            var lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "1",
                LineItemTitle = "AGE 18-20 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            // >= AGE 21 WITH >= 1000 PS HOURS
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) ||
                            (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday21)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "2",
                LineItemTitle = ">= AGE 21 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            // <  AGE 18
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) ||
                            (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                .Where(x => x.Demographic!.DateOfBirth > birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "Active and Inactive",
                LineItemPrefix = "3",
                LineItemTitle = "<  AGE 18",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //>= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) ||
                            (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth < birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot })
                .Where(x => x.tot.Total > 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
                {
                    Subgroup = "Active and Inactive",
                    LineItemPrefix = "4",
                    LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                    NumberOfMembers = x.Count(),
                    TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                    TotalBalance = x.Sum(y => y.tot.Total ?? 0)
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //>= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => nonTerminatedStatuses.Contains(p.Demographic!.EmploymentStatusId) ||
                            (p.Demographic!.TerminationDate > calInfo.FiscalEndDate))
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth < birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot })
                .Where(x => x.tot.Total == 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
                {
                    Subgroup = "Active and Inactive",
                    LineItemPrefix = "5",
                    LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                    NumberOfMembers = x.Count(),
                    TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                    TotalBalance = x.Sum(y => y.tot.Total ?? 0)
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated >= AGE 18 WITH >= 1000 PS HOURS 
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                            p.Demographic!.TerminationDate > calInfo.FiscalEndDate)
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) >= 1000 && x.Demographic!.DateOfBirth <= birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "6",
                LineItemTitle = ">= AGE 18 WITH >= 1000 PS HOURS",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated >= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                            p.Demographic!.TerminationDate <= calInfo.FiscalEndDate &&
                            p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) < 1000 && x.Demographic!.DateOfBirth <= birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot })
                .Where(x => x.tot.Total == 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "7",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND NO PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated >= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                            p.Demographic!.TerminationDate <= calInfo.FiscalEndDate &&
                            p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                .Where(x => (x.CurrentHoursYear + x.HoursExecutive) < 1000 && x.Demographic!.DateOfBirth <= birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot })
                .Where(x => x.tot.Total != 0);

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "8",
                LineItemTitle = ">= AGE 18 WITH < 1000 PS HOURS AND PRIOR PS AMOUNT",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            //Terminated <  AGE 18           NO WAGES :   0
            qry = ctx.PayProfits.Include(x => x.Demographic).Where(p => p.ProfitYear == req.ProfitYear)
                .Where(p => p.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                            p.Demographic!.TerminationDate <= calInfo.FiscalEndDate &&
                            p.Demographic!.TerminationDate >= calInfo.FiscalBeginDate)
                .Where(x => (x.CurrentIncomeYear + x.IncomeExecutive) == 0 && x.Demographic!.DateOfBirth > birthday18)
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Demographic!.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            lineItem = await qry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "X",
                LineItemTitle = "<  AGE 18           NO WAGES :   0",
                NumberOfMembers = x.Count(),
                TotalWages = x.Sum(y => y.pp.IncomeExecutive + y.pp.CurrentIncomeYear),
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }


            var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
            var beneQry = ctx.BeneficiaryContacts
                .Where(bc => !demographics.Any(x => x.Ssn == bc.Ssn))
                .Join(_totalService.GetTotalBalanceSet(ctx, req.ProfitYear), x => x.Ssn, x => x.Ssn,
                    (pp, tot) => new { pp, tot });

            lineItem = await beneQry.GroupBy(x => true).Select(x => new YearEndProfitSharingReportSummaryLineItem()
            {
                Subgroup = "TERMINATED",
                LineItemPrefix = "N",
                LineItemTitle = "NON-EMPLOYEE BENEFICIARIES",
                NumberOfMembers = x.Count(),
                TotalWages = 0,
                TotalBalance = x.Sum(y => y.tot.Total ?? 0)
            }).FirstOrDefaultAsync(cancellationToken);
            if (lineItem != null)
            {
                response.LineItems.Add(lineItem);
            }

            return response;
        });
    }

    public async Task<YearEndProfitSharingReportResponse> GetYearEndProfitSharingReportAsync(
        YearEndProfitSharingReportRequest req,
        CancellationToken cancellationToken = default)
    {
        // ──────────────────────────────────────────────────────────────────────────
        //  Calendar helpers
        // ──────────────────────────────────────────────────────────────────────────
        var calInfo = await _calendarService
            .GetYearStartAndEndAccountingDatesAsync(req.ProfitYear, cancellationToken);
        var birthDate21 = calInfo.FiscalEndDate.AddYears(-21);


        var totTask = _dataContextFactory.UseReadOnlyContext<ProfitShareTotal>( ctx =>
        {
            
            if (req.IncludeTotals)
            {
                var queryable = _embeddedSqlService.GetProfitShareTotals(ctx, req.ProfitYear, 
                    calInfo.FiscalEndDate,
                    ReferenceData.MinimumHoursForContribution(),
                    birthDate21, cancellationToken);

#pragma warning disable AsyncFixer02
                return (_hostEnvironment.IsTestEnvironment() ? Task.FromResult(queryable.First()) : queryable.FirstAsync(cancellationToken))!;
#pragma warning restore AsyncFixer02
            }

            return Task.FromResult(new ProfitShareTotal());
        });


        Task<PaginatedResponseDto<YearEndProfitSharingReportDetail>> responseTask = _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            if (!req.IncludeDetails)
            {
                return new PaginatedResponseDto<YearEndProfitSharingReportDetail>(req);
            }

            // ──────────────────────────────────────────────────────────────────────────
            //  Base PayProfit query
            // ──────────────────────────────────────────────────────────────────────────
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

            // ──────────────────────────────────────────────────────────────────────────
            //  Beneficiaries (optional)
            // ──────────────────────────────────────────────────────────────────────────
            IQueryable<Beneficiary> beneficiaryQuery = Enumerable
                .Empty<Beneficiary>()
                .AsQueryable();

            if (req.IncludeBeneficiaries)
            {
                beneficiaryQuery = ctx.Beneficiaries
                    .Include(b => b.Contact)!
                    .ThenInclude(c => c!.ContactInfo)
                    .Where(b => !demographicQuery.Any(x => x.Ssn == b.Contact!.Ssn));
            }

            // ──────────────────────────────────────────────────────────────────────────
            //  Years‑of‑service, balances, first‑contribution CTEs
            // ──────────────────────────────────────────────────────────────────────────
            var yearsOfService = _totalService.GetYearsOfService(ctx, req.ProfitYear);
            var balances = _totalService.GetTotalBalanceSet(ctx, req.ProfitYear);


            // ──────────────────────────────────────────────────────────────────────────
            //  Main employee projection
            // ──────────────────────────────────────────────────────────────────────────
            var employeeQry =
                from pp in basePayProfits
                join et in ctx.EmploymentTypes
                    on pp.Demographic!.EmploymentTypeId equals et.Id
                join yip in yearsOfService
                    on pp.Demographic!.Ssn equals yip.Ssn into yipTmp
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

            //  beneficiaries shaped like employees
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

            // ──────────────────────────────────────────────────────────────────────────
            //  Filters
            // ──────────────────────────────────────────────────────────────────────────
            employeeQry = ApplyRequestFilters(employeeQry, req, calInfo);

            // ──────────────────────────────────────────────────────────────────────────
            //  Balance join (only if needed)
            // ──────────────────────────────────────────────────────────────────────────
            bool needsBalance =
                req.IncludeDetails ||
                !req.IncludeEmployeesWithNoPriorProfitSharingAmounts ||
                !req.IncludeEmployeesWithPriorProfitSharingAmounts;

            IQueryable<EmployeeWithBalance> employeeWithBalanceQry =
                needsBalance
                    ? from e in employeeQry
                    join bal in balances
                        on e.Ssn equals bal.Ssn into balTmp
                    from bal in balTmp.DefaultIfEmpty()
                    select new EmployeeWithBalance
                    {
                        Employee = e, Balance = (decimal)(bal != null && bal.Total != null ? bal.Total : 0)
                    }
                    : employeeQry.Select(e => new EmployeeWithBalance { Employee = e, Balance = 0m });

            //  optional balance‑based include/exclude
            if (needsBalance &&
                req.IncludeEmployeesWithPriorProfitSharingAmounts ^
                req.IncludeEmployeesWithNoPriorProfitSharingAmounts)
            {
                bool wantPrior = req.IncludeEmployeesWithPriorProfitSharingAmounts;
                employeeWithBalanceQry = employeeWithBalanceQry
                    .Where(x => (x.Balance > 0) == wantPrior);
            }

            // ──────────────────────────────────────────────────────────────────────────
            //  Details (paged)
            // ──────────────────────────────────────────────────────────────────────────

            return await employeeWithBalanceQry
                .Select(x => new YearEndProfitSharingReportDetail
                {
                    BadgeNumber = x.Employee.BadgeNumber,
                    EmployeeName = x.Employee.FullName!,
                    StoreNumber = x.Employee.StoreNumber,
                    EmployeeTypeCode = x.Employee.EmploymentTypeId,
                    EmployeeTypeName = x.Employee.EmploymentTypeName,
                    DateOfBirth = x.Employee.DateOfBirth,
                    Age = 0, // back‑filled later
                    Ssn = x.Employee.Ssn.MaskSsn(),
                    Wages = x.Employee.Wages,
                    Hours = x.Employee.Hours,
                    Points = Convert.ToInt16(x.Employee.PointsEarned),
                    IsNew = x.Balance == 0 &&
                            x.Employee.Hours >
                            ReferenceData.MinimumHoursForContribution(),
                    IsUnder21 = false, // back‑filled later
                    EmployeeStatus = x.Employee.EmploymentStatusId,
                    Balance = x.Balance,
                    YearsInPlan = x.Employee.Years ?? 0,
                    TerminationDate = x.Employee.TerminationDate
                })
                .ToPaginationResultsAsync(req, cancellationToken);

        });

        await Task.WhenAll(totTask, responseTask);

        var response = new YearEndProfitSharingReportResponse
        {
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            ReportName = $"PROFIT SHARE REPORT (PAY426) - {req.ProfitYear}",
            Response = await responseTask
        };

        var tot = await totTask;
        response.WagesTotal = tot.WagesTotal;
        response.HoursTotal = tot.HoursTotal;
        response.PointsTotal = tot.PointsTotal;
        response.TerminatedWagesTotal = tot.TerminatedWagesTotal;
        response.TerminatedHoursTotal = tot.TerminatedHoursTotal;
        response.TerminatedPointsTotal = tot.TerminatedPointsTotal;
        response.NumberOfEmployees = tot.NumberOfEmployees;
        response.NumberOfNewEmployees = tot.NumberOfNewEmployees;
        response.NumberOfEmployeesInPlan =
            tot.NumberOfEmployees - tot.NumberOfEmployeesUnder21 -
            tot.NumberOfNewEmployees;
        response.NumberOfEmployeesUnder21 = tot.NumberOfEmployeesUnder21;

        if (response.Response.Results.Any())
        {
            foreach (var d in response.Response.Results)
            {
                d.Age = (byte)d.DateOfBirth.Age(response.ReportDate.DateTime);
                if (d.Age < 21)
                {
                    d.IsUnder21 = true;
                    d.Points = 0;
                }
            }
        }

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
        qry = qry.Where(p =>
            (p.EmploymentStatusId == EmploymentStatus.Constants.Active ||
             p.EmploymentStatusId == EmploymentStatus.Constants.Inactive ||
             (p.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
              p.TerminationDate > calInfo.FiscalEndDate)));


        return qry;
    }

   
}
