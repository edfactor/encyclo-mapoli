using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.Breakdown;

public class BreakdownReportService : IBreakdownService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;

    public BreakdownReportService(IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
    }

    private sealed class ActiveMemberDto
    {
        public int BadgeNumber { get; init; }
        public short StoreNumber { get; init; }
        public string? FullName { get; init; } = string.Empty;
        public int Ssn { get; init; }
        public string DepartmentName { get; init; } = string.Empty;
        public string PayClassificationName { get; init; } = string.Empty;
        public byte PayClassificationId { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public char EmploymentStatusId { get; init; }
        public byte DepartmentId { get; init; }
        public byte PayFrequencyId { get; init; }
    }

    private IQueryable<ActiveMemberDto> GetStoreQueryBase(IProfitSharingDbContext context)
    {
        return context.Demographics
            .Include(d => d.PayClassification)
            .Include(d=> d.Department)
            .Select(d => new ActiveMemberDto
            {
                BadgeNumber = d.BadgeNumber,
                StoreNumber = d.StoreNumber,
                FullName = d.ContactInfo.FullName,
                Ssn = d.Ssn,
                DateOfBirth = d.DateOfBirth,
                PayClassificationId = d.PayClassificationId,
                EmploymentStatusId = d.EmploymentStatusId,
                DepartmentId = d.DepartmentId,
                PayFrequencyId = d.PayFrequencyId,
                DepartmentName = d.Department!.Name,
                PayClassificationName = d.PayClassification!.Name,
            });
    }

    private static IQueryable<ActiveMemberDto> FilterForStoreManagement(IQueryable<ActiveMemberDto> queryable)
    {
        return queryable.Where(d =>
            ((d.EmploymentStatusId == EmploymentStatus.Constants.Active &&
              d.PayFrequencyId != PayFrequency.Constants.Monthly) ||
             (d.EmploymentStatusId == EmploymentStatus.Constants.Inactive && d.StoreNumber == 986)) &&
            (
                (d.DepartmentId == Department.Constants.Grocery && new[]
                {
                PayClassification.Constants.Manager, PayClassification.Constants.AssistantManager,
                PayClassification.Constants.FrontEndManager, PayClassification.Constants.Merchandiser,
                PayClassification.Constants.GroceryManager
                }.Contains(d.PayClassificationId)) ||
                (d.DepartmentId == Department.Constants.Meat && new[]
                {
                PayClassification.Constants.Manager, PayClassification.Constants.AssistantManager
                }.Contains(d.PayClassificationId)) ||
                (d.DepartmentId == Department.Constants.Produce && d.PayClassificationId == PayClassification.Constants.Manager) ||
                (d.DepartmentId == Department.Constants.Deli && d.PayClassificationId == PayClassification.Constants.Manager) ||
                (d.DepartmentId == Department.Constants.Dairy && d.PayClassificationId == PayClassification.Constants.Manager) ||
                (d.DepartmentId == Department.Constants.BeerAndWine && d.PayClassificationId == PayClassification.Constants.Manager) ||
                (d.DepartmentId == Department.Constants.Bakery && d.PayClassificationId == PayClassification.Constants.Manager)
            )
        );
    }

    private static IQueryable<ActiveMemberDto> FilterForNonStoreManagement(IQueryable<ActiveMemberDto> queryable)
    {
        return queryable.Where(d =>
            !(
                ((d.EmploymentStatusId == EmploymentStatus.Constants.Active &&
                  d.PayFrequencyId != PayFrequency.Constants.Monthly) ||
                 (d.EmploymentStatusId == EmploymentStatus.Constants.Inactive && d.StoreNumber == 986)) &&
                (
                    (d.DepartmentId == Department.Constants.Grocery && new[]
                    {
                    PayClassification.Constants.Manager, PayClassification.Constants.AssistantManager,
                    PayClassification.Constants.FrontEndManager, PayClassification.Constants.Merchandiser,
                    PayClassification.Constants.GroceryManager
                    }.Contains(d.PayClassificationId)) ||
                    (d.DepartmentId == Department.Constants.Meat && new[]
                    {
                    PayClassification.Constants.Manager, PayClassification.Constants.AssistantManager
                    }.Contains(d.PayClassificationId)) ||
                    (d.DepartmentId == Department.Constants.Produce && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.Deli && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.Dairy && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.BeerAndWine && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.Bakery && d.PayClassificationId == PayClassification.Constants.Manager)
                )
            )
        );
    }

    public Task<BreakdownByStoreTotals> GetTotalsByStore(BreakdownByStoreRequest breakdownByStoreRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(BreakdownByStoreRequest breakdownByStoreRequest, CancellationToken cancellationToken)
    {
       return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            if (breakdownByStoreRequest.StoreNumber <= 0)
            {
                throw new InvalidOperationException($"Invalid {nameof(breakdownByStoreRequest.StoreNumber)} {breakdownByStoreRequest.StoreNumber}.");
            }

            var employeesBase = GetStoreQueryBase(ctx);

            employeesBase = breakdownByStoreRequest.StoreManagement ? FilterForStoreManagement(employeesBase) : FilterForNonStoreManagement(employeesBase);
            employeesBase = employeesBase.Where(d => d.StoreNumber == breakdownByStoreRequest.StoreNumber);

            HashSet<int> employeeSsns = await employeesBase.Select(pp => pp!.Ssn).ToHashSetAsync(cancellationToken);

            if (employeeSsns.Contains(0))
            {
                throw new InvalidOperationException("Unexpected 0 SSN encountered.");
            }

            CalendarResponseDto calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(breakdownByStoreRequest.ProfitYear, cancellationToken);
            short priorYear = (short)(breakdownByStoreRequest.ProfitYear - 1);
            Dictionary<int, decimal?> employeeVestingRatios = await _totalService
                .GetVestingRatio(ctx, breakdownByStoreRequest.ProfitYear, calInfo.FiscalEndDate)
                .Where(vr => employeeSsns.Contains(vr.Ssn ?? 0))
                .ToDictionaryAsync(vr => vr.Ssn ?? 0, vr => vr.Ratio, cancellationToken);

            Dictionary<int, decimal> endingBalanceLastYearBySsn = await _totalService.GetTotalBalanceSet(ctx, priorYear)
                .Where(tbs => employeeSsns.Contains(tbs.Ssn))
                .ToDictionaryAsync(tbs => tbs.Ssn, tbs => tbs.Total ?? 0, cancellationToken);

            Dictionary<int, InternalProfitDetailDto> txnsForProfitYear = await TotalService.GetTransactionsBySsnForProfitYearForOracle(ctx, breakdownByStoreRequest.ProfitYear)
                .Where(txns => employeeSsns.Contains(txns.Ssn))
                .ToDictionaryAsync(txns => txns.Ssn, txns => txns, cancellationToken);


            var paginatedResults = await employeesBase.ToPaginationResultsAsync(breakdownByStoreRequest, cancellationToken);

            var combined = paginatedResults.Results
                .Select(d =>
                {
                    decimal vestingRatio = employeeVestingRatios.GetValueOrDefault(d.Ssn) ?? 0;
                    decimal beginningBalance = endingBalanceLastYearBySsn.GetValueOrDefault(d.Ssn);
                    InternalProfitDetailDto txns = txnsForProfitYear.GetValueOrDefault(d.Ssn) ?? new InternalProfitDetailDto();

                    return new MemberYearSummaryDto
                    {
                        BadgeNumber = d.BadgeNumber,
                        FullName = d.FullName!,
                        StoreNumber = d.StoreNumber,
                        BeginningBalance = beginningBalance,
                        Earnings = txns.TotalEarnings,
                        Contributions = txns.TotalContributions,
                        Distributions = txns.Distribution,
                        Forfeitures = txns.TotalForfeitures,
                        EndingBalance = beginningBalance + txns.TotalContributions + txns.TotalEarnings + txns.TotalForfeitures +
                                        txns.Distribution + txns.BeneficiaryAllocation,
                        VestedAmount = (beginningBalance + txns.TotalContributions + txns.TotalEarnings + txns.TotalForfeitures +
                                        txns.Distribution + txns.BeneficiaryAllocation) * vestingRatio,
                        VestedPercent = (byte)(vestingRatio * 100),
                        PayClassificationId = d.PayClassificationId,
                        PayClassificationName = d.PayClassificationName
                    };
                })
                .OrderBy(s => s.StoreNumber)
                .ThenBy(s => s.FullName, StringComparer.Ordinal)
                .ToList();

            PaginatedResponseDto<MemberYearSummaryDto> paginatedResponseDto = new() { Results = combined, Total = paginatedResults.Total };

            return new ReportResponseBase<MemberYearSummaryDto>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = $"Breakdown Report for {breakdownByStoreRequest.ProfitYear}",
                Response = paginatedResponseDto
            };
        });
    }
}

