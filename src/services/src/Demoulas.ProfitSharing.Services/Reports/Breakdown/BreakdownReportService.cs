using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.Breakdown;

public class BreakdownReportService : IBreakdownService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;

    public BreakdownReportService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService
    )
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
    }

    public async Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(BreakdownByStoreRequest breakdownByStoreRequest, CancellationToken cancellationToken)
    {
        List<MemberYearSummaryDto> response = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            CalendarResponseDto calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(breakdownByStoreRequest.ProfitYear, cancellationToken);
            short priorYear = (short)(breakdownByStoreRequest.ProfitYear - 1);
            DateOnly birthDate21 = calInfo.FiscalEndDate.AddYears(-21);

            List<PayProfit> employees = await ctx.PayProfits
                .Include(p => p.Demographic)
                .ThenInclude(demographic => demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == breakdownByStoreRequest.ProfitYear)
                .Where(pp => !breakdownByStoreRequest.Under21Only || pp.Demographic!.DateOfBirth > birthDate21)
                .ToListAsync(cancellationToken);
            HashSet<int> employeeSsns = employees.Select(pp => pp.Demographic!.Ssn).ToHashSet();

            Dictionary<int, int?> employeeVestingRatios = await _totalService
                .GetVestingRatio(ctx, breakdownByStoreRequest.ProfitYear, calInfo.FiscalEndDate)
                .Where(vr => employeeSsns.Contains(vr.Ssn ?? 0))
                .ToDictionaryAsync(vr => vr.Ssn ?? 0, vr => vr.Ssn, cancellationToken);

            if (employeeVestingRatios.ContainsKey(0))
            {
                throw new InvalidOperationException("Unexpected 0 SSN encountered.");
            }

            Dictionary<int, decimal?> endingBalanceLastYearBySsn = await _totalService.GetTotalBalanceSet(ctx, priorYear)
                .Where(tbs => employeeSsns.Contains(tbs.Ssn))
                .ToDictionaryAsync(tbs => tbs.Ssn, tbs => tbs.Total, cancellationToken);

            Dictionary<int, InternalProfitDetailDto> txnsForProfitYear = await TotalService.GetTransactionsBySsnForProfitYear(ctx, breakdownByStoreRequest.ProfitYear)
                .Where(txns => employeeSsns.Contains(txns.Ssn))
                .ToDictionaryAsync(txns => txns.Ssn, txns => txns, cancellationToken);

            return employees
                .Select(employee =>
                {
                    int vestingRatio = employeeVestingRatios.GetValueOrDefault(employee.Demographic!.Ssn) ?? 0;
                    decimal beginningBalance = endingBalanceLastYearBySsn.GetValueOrDefault(employee.Demographic!.Ssn) ?? 0;
                    Demographic d = employee.Demographic!;
                    InternalProfitDetailDto txns = txnsForProfitYear.GetValueOrDefault(employee.Demographic!.Ssn) ?? new InternalProfitDetailDto();

                    return new MemberYearSummaryDto
                    {
                        BadgeNumber = d.BadgeNumber,
                        FullName = d.ContactInfo.FullName!,
                        Ssn = d.Ssn.MaskSsn(),
                        PayFrequencyId = d.PayFrequencyId,
                        EnrollmentId = employee.EnrollmentId,
                        StoreNumber = d.StoreNumber,
                        DepartmentId = d.DepartmentId,
                        PayClassificationId = d.PayClassificationId,
                        BeginningBalance = beginningBalance,
                        Earnings = txns.TotalEarnings,
                        Contributions = txns.TotalContributions,
                        Forfeiture = txns.TotalForfeitures,
                        Distributions = txns.Distribution,
                        EndingBalance = beginningBalance + txns.TotalContributions + txns.TotalEarnings + txns.TotalForfeitures +
                                        txns.Distribution + txns.BeneficiaryAllocation,
                        VestedAmount = (beginningBalance + txns.TotalContributions + txns.TotalEarnings + txns.TotalForfeitures +
                                        txns.Distribution + txns.BeneficiaryAllocation) * vestingRatio,
                        VestedPercentage = vestingRatio * 100,
                        EmploymentStatusId = d.EmploymentStatusId,
                        EmployeeRank = (short)EmployeeRank(employee.Demographic.DepartmentId, employee.Demographic.PayClassificationId)
                    };
                })
                .OrderBy(s => s.StoreNumber).ThenBy(s => s.EmployeeRank).ThenBy(s => s.FullName)
                .ToList();
        });

        // This report is broken down by store, so pagination is TBD (as you do not want to split up a store.)
        // We currently have no pagination for this report, so we are just returning the full list.
        PaginatedResponseDto<MemberYearSummaryDto> paginatedResponseDto = new() { Results = response, Total = response.Count };
        return new ReportResponseBase<MemberYearSummaryDto>
        {
            ReportDate = DateTimeOffset.Now, ReportName = $"Breakdown Report for {breakdownByStoreRequest.ProfitYear}", Response = paginatedResponseDto
        };
    }

    private static int EmployeeRank(byte departmentId, byte payClassificationId)
    {
        // Not sure if we need anything other than the 1999 rank for the regular associates, and then another number for managers,
        // but for now we are holding on to this until we finish the breakdown report work, at which point it should be clear if
        // this amount of ordering specificity is required.
        //
        // This ranks the managers above the employees.  Search for " 120 " in,
        // https://bitbucket.org/demoulas/hpux/raw/fcd54cd50e1660f050b23a1f5ae44799458b51c0/iqs-source/QPAY066TA.pco
        return departmentId == Department.Constants.Grocery && payClassificationId == PayClassification.Constants.Manager ? 10
            : departmentId == Department.Constants.Grocery && payClassificationId == PayClassification.Constants.AssistantManager ? 20
            : departmentId == Department.Constants.Grocery && payClassificationId == PayClassification.Constants.Merchandiser ? 30
            : departmentId == Department.Constants.Grocery && payClassificationId == PayClassification.Constants.FrontEndManager ? 40
            : departmentId == Department.Constants.Grocery && payClassificationId == PayClassification.Constants.GroceryManager ? 50
            : departmentId == Department.Constants.Meat && payClassificationId == PayClassification.Constants.Manager ? 60
            : departmentId == Department.Constants.Meat && payClassificationId == PayClassification.Constants.AssistantManager ? 70
            : departmentId == Department.Constants.Deli && payClassificationId == PayClassification.Constants.Manager ? 80
            : departmentId == Department.Constants.Produce && payClassificationId == PayClassification.Constants.Manager ? 90
            : departmentId == Department.Constants.Dairy && payClassificationId == PayClassification.Constants.Manager ? 100
            : departmentId == Department.Constants.Bakery && payClassificationId == PayClassification.Constants.Manager ? 110
            : departmentId == Department.Constants.BeerAndWine && payClassificationId == PayClassification.Constants.Manager ? 120
            : 1999 /* Regular Associate */;
    }
}
