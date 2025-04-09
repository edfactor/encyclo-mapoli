using System.Diagnostics.CodeAnalysis;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.AspNetCore.Http;
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
        List<MemberYearSummaryDto> employees = await _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            CalendarResponseDto calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(breakdownByStoreRequest.ProfitYear, cancellationToken);
            short priorYear = (short)(breakdownByStoreRequest.ProfitYear - 1);
            DateOnly birthDate21 = calInfo.FiscalEndDate.AddYears(-21);

            IQueryable<PayProfit> employeesBase = ctx.PayProfits
                .Include(p => p.Demographic)
                .ThenInclude(demographic => demographic!.ContactInfo)
                .Where(pp => pp.ProfitYear == breakdownByStoreRequest.ProfitYear);
            // .Where(pp => pp.Demographic!.StoreNumber < 3)

            // This branching based on report requested is going to get more complex as more report types are added,
            // it will likely graduate to a switch statement, or some other more sophisticated form of dispatch.
            if (breakdownByStoreRequest.StoreNumber == null)
            {
                employeesBase = employeesBase.Where(pp => pp.Demographic!.EmploymentStatusId == EmploymentStatus.Constants.Active
                                                          && pp.Demographic.StoreNumber < StoreTypes.PsPensionRetired
                                                          && pp.EnrollmentId > 0);
                if (breakdownByStoreRequest.Under21Only)
                {
                    employeesBase = employeesBase.Where(pp => pp.Demographic!.DateOfBirth > birthDate21);
                }
            }
            else if (breakdownByStoreRequest.StoreNumber == StoreTypes.PsPensionRetired)
            {
                employeesBase = employeesBase.Where(pp => pp.Demographic!.TerminationCodeId == TerminationCode.Constants.RetiredReceivingPension);
            }
            else
            {
                throw new BadHttpRequestException("Unexpected store number encountered.");
            }

            List<PayProfit> employees = await employeesBase
                .ToListAsync(cancellationToken);

            HashSet<int> employeeSsns = employees.Select(pp => pp.Demographic!.Ssn).ToHashSet();

            Dictionary<int, decimal?> employeeVestingRatios = await _totalService
                .GetVestingRatio(ctx, breakdownByStoreRequest.ProfitYear, calInfo.FiscalEndDate)
                .Where(vr => employeeSsns.Contains(vr.Ssn ?? 0))
                .ToDictionaryAsync(vr => vr.Ssn ?? 0, vr => vr.Ratio, cancellationToken);

            if (employeeVestingRatios.ContainsKey(0))
            {
                throw new InvalidOperationException("Unexpected 0 SSN encountered.");
            }

            Dictionary<int, decimal> endingBalanceLastYearBySsn = await _totalService.GetTotalBalanceSet(ctx, priorYear)
                .Where(tbs => employeeSsns.Contains(tbs.Ssn!.Value))
                .ToDictionaryAsync(tbs => tbs.Ssn!.Value, tbs => tbs.Total ?? 0, cancellationToken);

            Dictionary<int, InternalProfitDetailDto> txnsForProfitYear = await TotalService.GetTransactionsBySsnForProfitYear(ctx, breakdownByStoreRequest.ProfitYear)
                .Where(txns => employeeSsns.Contains(txns.Ssn))
                .ToDictionaryAsync(txns => txns.Ssn, txns => txns, cancellationToken);

            return employees
                .Select(employee =>
                {
                    decimal vestingRatio = employeeVestingRatios.GetValueOrDefault(employee.Demographic!.Ssn) ?? 0;
                    decimal beginningBalance = endingBalanceLastYearBySsn.GetValueOrDefault(employee.Demographic!.Ssn);
                    Demographic d = employee.Demographic!;
                    InternalProfitDetailDto txns = txnsForProfitYear.GetValueOrDefault(employee.Demographic!.Ssn) ?? new InternalProfitDetailDto();
                    short employeeRank = EmployeeSortRank(breakdownByStoreRequest.StoreNumber, d.DepartmentId, d.PayClassificationId);

                    return new MemberYearSummaryDto
                    {
                        BadgeNumber = d.BadgeNumber,
                        FullName = d.ContactInfo.FullName!,
                        Ssn = d.Ssn.MaskSsn(),
                        PayFrequencyId = d.PayFrequencyId,
                        EnrollmentId = employee.EnrollmentId,
                        StoreNumber = breakdownByStoreRequest.StoreNumber == null ? d.StoreNumber : breakdownByStoreRequest.StoreNumber ?? 0,
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
                        EmployeeCategory = EmployeeCategory(breakdownByStoreRequest.StoreNumber, employeeRank),
                        EmployeeSortRank = employeeRank // This may not need to go the client, but I think its presence helps explain that ordering has intention
                    };
                })
                .OrderBy(s => s.StoreNumber)
                .ThenBy(s => s.EmployeeSortRank)
                // NOTE: Sort using same character handling that READY uses (ie "Mc" sorts after "ME") aka the Ordinal sort.
                // Failure to use this sort, causes READY and SMART reports to not match - tears.
                .ThenBy(s => s.FullName, StringComparer.Ordinal)
                .ToList();
        });

        // This post filtering is slightly odd, but the idea is to keep the main member code working for all stores.   As more breakdown reports are added, this will evolve.
        if (!breakdownByStoreRequest.Under21Only)
        {
            employees = employees.Where(e => !(e is { BeginningBalance: 0, EndingBalance: 0 })).ToList();
        }

        // This report is broken down by store, so pagination is TBD (as you do not want to split up a store.)
        // We currently have no pagination for this report, so we are just returning the full list.
        // This strategy will be revisited
        PaginatedResponseDto<MemberYearSummaryDto> paginatedResponseDto = new() { Results = employees, Total = employees.Count };
        
        return new ReportResponseBase<MemberYearSummaryDto>
        {
            ReportDate = DateTimeOffset.Now, ReportName = $"Breakdown Report for {breakdownByStoreRequest.ProfitYear}", Response = paginatedResponseDto
        };
    }

    private static string EmployeeCategory(short? requestedStoreNumber, short employeeRank)
    {
        if (requestedStoreNumber == StoreTypes.PsPensionRetired)
        {
            return "RETIRED - DRAWING PENSION";
        }

        if (employeeRank == ASSOCIATE_SORT_RANK_1999)
        {
            return "ASSOCIATES";
        }

        return "STORE MANAGEMENT";
    }

    public static readonly short ASSOCIATE_SORT_RANK_1999 = 1999;

    private static short EmployeeSortRank(short? requestedStoreNumber, byte departmentId, byte payClassificationId)
    {
        if (requestedStoreNumber != null)
        {
            return 1; // Specific "Virtual Stores" dont do categories. right?
        }
        

        // the managers have a sort order used in the reports which is reflected in this expression.
        // The source for this ranking can be found in the cobol link below.  Search for " 120 " in,
        // https://bitbucket.org/demoulas/hpux/raw/fcd54cd50e1660f050b23a1f5ae44799458b51c0/iqs-source/QPAY066TA.pco
        return (short)(departmentId == Department.Constants.Grocery && payClassificationId == PayClassification.Constants.Manager ? 10
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
            : ASSOCIATE_SORT_RANK_1999);
    }
}
