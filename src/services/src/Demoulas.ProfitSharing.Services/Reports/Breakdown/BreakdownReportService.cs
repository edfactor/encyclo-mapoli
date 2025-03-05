
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.Breakdown;

// Cope with LINQ / Oracle -- Ternary optimzation issue
#pragma warning disable S3358

public class BreakdownReportService : IBreakdownService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly ITotalService _totalService;

    public BreakdownReportService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        ITotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
    }

    public async Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(ProfitYearRequest request, CancellationToken cancellationToken)
    {
        var response = _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            CalendarResponseDto dates = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            short priorYear = (short)(request.ProfitYear - 1);
            TotalService totalService = (TotalService)_totalService; // slight cheat, so we can access its internal methods 
            
            var employeesQuery = ctx.PayProfits.Include(p => p.Demographic)
                .Where(pp => pp.ProfitYear == request.ProfitYear)
                .Join(totalService.GetVestingRatio(ctx, request.ProfitYear, dates.FiscalEndDate),
                    pp => pp.Demographic!.Ssn,
                    vr => vr.Ssn,
                    (pp, vr) => new { pp, VestingRatio = vr.Ratio })
                .Join(totalService.GetTotalBalanceSet(ctx, priorYear),
                    ppAndVestingRatio => ppAndVestingRatio.pp.Demographic!.Ssn,
                    tbs => tbs.Ssn,
                    (ppAndRatio, tbs) => new { ppAndRatio.pp, ppAndRatio.VestingRatio, BeginningBalance = tbs.Total })
                .Join(TotalService.GetTransactionsBySsnForProfitYear(ctx, request.ProfitYear),
                    collected => collected.pp.Demographic!.Ssn,
                    transactionSums => transactionSums.Ssn,
                    (collected, transactionSums) => new
                    {
                        collected.pp,
                        d = collected.pp.Demographic,
                        collected.VestingRatio,
                        collected.BeginningBalance,
                        transactionSums,
                        EndingBalance = collected.BeginningBalance + transactionSums.TotalContributions + transactionSums.TotalEarnings + transactionSums.TotalForfeitures +
                                        transactionSums.Distribution + transactionSums.BeneficiaryAllocation
                    }
                )
                .OrderBy(s=>s.d!.StoreNumber).ThenBy(s =>
                        // This ranks the managers above the employees.  Search for " 120 " in,
                        // https://bitbucket.org/demoulas/hpux/raw/fcd54cd50e1660f050b23a1f5ae44799458b51c0/iqs-source/QPAY066TA.pco
                        s.d!.DepartmentId == Department.Constants.Grocery && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 10
                        : s.d!.DepartmentId == Department.Constants.Grocery && s.d!.PayClassificationId == PayClassification.Constants.AssistantManager ? 20
                        : s.d!.DepartmentId == Department.Constants.Grocery && s.d!.PayClassificationId == PayClassification.Constants.Merchandiser ? 30
                        : s.d!.DepartmentId == Department.Constants.Grocery && s.d!.PayClassificationId == PayClassification.Constants.FrontEndManager ? 40
                        : s.d!.DepartmentId == Department.Constants.Grocery && s.d!.PayClassificationId == PayClassification.Constants.GroceryManager ? 50
                        : s.d!.DepartmentId == Department.Constants.Meat && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 60
                        : s.d!.DepartmentId == Department.Constants.Meat && s.d!.PayClassificationId == PayClassification.Constants.AssistantManager ? 70
                        : s.d!.DepartmentId == Department.Constants.Deli && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 80
                        : s.d!.DepartmentId == Department.Constants.Produce && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 90
                        : s.d!.DepartmentId == Department.Constants.Dairy && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 100
                        : s.d!.DepartmentId == Department.Constants.Bakery && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 110
                        : s.d!.DepartmentId == Department.Constants.BeerAndWine && s.d!.PayClassificationId == PayClassification.Constants.Manager ? 120
                        : 1999) // Default value
                    .ThenBy(s => s.d!.ContactInfo.FullName)
                .Select(coll => new MemberYearSummaryDto
                {
                    BadgeNumber = coll.d!.BadgeNumber,
                    FullName = coll.d.ContactInfo.FullName!,
                    Ssn = coll.d.Ssn.MaskSsn(),
                    PayFrequencyId = coll.d.PayFrequencyId,
                    EnrollmentId = coll.pp.EnrollmentId,
                    StoreNumber = coll.d.StoreNumber,
                    DepartmentId = coll.d.DepartmentId,
                    PayClassificationId = coll.d.PayClassificationId,
                    BeginningBalance = coll.BeginningBalance,
                    Earnings = coll.transactionSums.TotalEarnings,
                    Contributions = coll.transactionSums.TotalContributions,
                    Forfeiture = coll.transactionSums.TotalForfeitures,
                    Distributions = coll.transactionSums.Distribution,
                    EndingBalance = coll.EndingBalance,
                    VestedAmount = coll.EndingBalance * coll.VestingRatio,
                    VestedPercentage = coll.VestingRatio * 100,
                    EmploymentStatusId = coll.d.EmploymentStatusId
                })
                .ToPaginationResultsAsync(request, cancellationToken);

            return await employeesQuery;
        });

        return new ReportResponseBase<MemberYearSummaryDto> { ReportDate = DateTimeOffset.Now, ReportName = $"Breakdown Report for {request.ProfitYear}", Response = await response };
    }
}
