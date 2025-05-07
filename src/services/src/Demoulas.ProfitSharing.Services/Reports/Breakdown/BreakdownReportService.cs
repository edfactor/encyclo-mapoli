using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.Breakdown;

public sealed class BreakdownReportService : IBreakdownService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;

    public BreakdownReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
    }

    #region ── Helper DTOs ────────────────────────────────────────────────────────────────

    private sealed record ActiveMemberDto
    {
        public int BadgeNumber { get; init; }
        public short StoreNumber { get; init; }
        public string FullName { get; init; } = string.Empty;
        public int Ssn { get; init; }
        public string DepartmentName { get; init; } = string.Empty;
        public string PayClassificationName { get; init; } = string.Empty;
        public byte PayClassificationId { get; init; }
        public DateOnly DateOfBirth { get; init; }
        public char EmploymentStatusId { get; init; }
        public byte DepartmentId { get; init; }
        public byte PayFrequencyId { get; init; }
    }

    private sealed record EmployeeFinancialSnapshot(
        int Ssn,
        decimal BeginningBalance,
        InternalProfitDetailDto Txn,
        decimal VestingRatio);

    #endregion

    public Task<BreakdownByStoreTotals> GetTotalsByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ValidateStoreNumber(request);

            // ── Query ------------------------------------------------------------------
            var employeesBase = BuildEmployeesBaseQuery(ctx, request);
            var employeeSsns = await employeesBase.Select(e => e.Ssn).ToHashSetAsync(cancellationToken);

            ThrowIfInvalidSsns(employeeSsns);

            var snapshots = await GetEmployeeFinancialSnapshotsAsync(
                ctx, request.ProfitYear, employeeSsns, cancellationToken);

            // ── Aggregate --------------------------------------------------------------
            var totals = new BreakdownByStoreTotals
            {
                TotalNumberEmployees = (short)snapshots.Count,
                TotalBeginningBalances = snapshots.Sum(x => x.BeginningBalance),
                TotalEarnings = snapshots.Sum(x => x.Txn.TotalEarnings),
                TotalContributions = snapshots.Sum(x => x.Txn.TotalContributions),
                TotalForfeitures = snapshots.Sum(x => x.Txn.TotalForfeitures),
                TotalDisbursements = snapshots.Sum(x => x.Txn.Distribution)
            };

            totals.TotalEndBalances = totals.TotalBeginningBalances
                                     + totals.TotalEarnings
                                     + totals.TotalContributions
                                     + totals.TotalForfeitures
                                     + totals.TotalDisbursements
                                     + snapshots.Sum(x => x.Txn.BeneficiaryAllocation);

            totals.TotalVestedBalance = snapshots.Sum(x =>
            {
                var endBal = x.BeginningBalance
                           + x.Txn.TotalContributions
                           + x.Txn.TotalEarnings
                           + x.Txn.TotalForfeitures
                           + x.Txn.Distribution
                           + x.Txn.BeneficiaryAllocation;
                return endBal * x.VestingRatio;
            });

            return totals;
        });
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ValidateStoreNumber(request);

            var employeesBase = BuildEmployeesBaseQuery(ctx, request);
            var paginated = await employeesBase.ToPaginationResultsAsync(request, cancellationToken);
            var employeeSsns = paginated.Results.Select(r => r.Ssn).ToHashSet();

            ThrowIfInvalidSsns(employeeSsns);

            var snapshots = await GetEmployeeFinancialSnapshotsAsync(
                ctx, request.ProfitYear, employeeSsns, cancellationToken);
            var snapshotBySsn = snapshots.ToDictionary(s => s.Ssn);

            var members = paginated.Results
                .Select(d => BuildMemberYearSummary(d, snapshotBySsn.GetValueOrDefault(d.Ssn)))
                .OrderBy(m => m.StoreNumber)
                .ThenBy(m => m.FullName, StringComparer.Ordinal)
                .ToList();

            return new ReportResponseBase<MemberYearSummaryDto>
            {
                ReportDate = DateTimeOffset.Now,
                ReportName = $"Breakdown Report for {request.ProfitYear}",
                Response = new PaginatedResponseDto<MemberYearSummaryDto>
                {
                    Results = members,
                    Total = paginated.Total
                }
            };
        });
    }

    #region ── Private: common building blocks ───────────────────────────────────────────

    private static void ValidateStoreNumber(BreakdownByStoreRequest request)
    {
        if (request.StoreNumber <= 0)
        {
            throw new InvalidOperationException(
                $"Invalid {nameof(request.StoreNumber)} {request.StoreNumber}.");
        }
    }

    private static void ThrowIfInvalidSsns(ISet<int> ssns)
    {
        if (ssns.Contains(0))
        {
            throw new InvalidOperationException("Unexpected 0 SSN encountered.");
        }
    }

    private IQueryable<ActiveMemberDto> BuildEmployeesBaseQuery(
        IProfitSharingDbContext ctx,
        BreakdownByStoreRequest request)
    {
        var query = ctx.Demographics
            .Include(d => d.PayClassification)
            .Include(d => d.Department)
            .Select(d => new ActiveMemberDto
            {
                BadgeNumber = d.BadgeNumber,
                StoreNumber = d.StoreNumber,
                FullName = d.ContactInfo.FullName!,
                Ssn = d.Ssn,
                DateOfBirth = d.DateOfBirth,
                PayClassificationId = d.PayClassificationId,
                EmploymentStatusId = d.EmploymentStatusId,
                DepartmentId = d.DepartmentId,
                PayFrequencyId = d.PayFrequencyId,
                DepartmentName = d.Department!.Name,
                PayClassificationName = d.PayClassification!.Name,
            });

        // Store‑level + management filter
        query = request.StoreManagement ? ApplyStoreManagementFilter(query)
                                        : ApplyNonStoreManagementFilter(query);

        return query.Where(e => e.StoreNumber == request.StoreNumber);
    }

    private async Task<List<EmployeeFinancialSnapshot>> GetEmployeeFinancialSnapshotsAsync(
        IProfitSharingDbContext ctx,
        short profitYear,
        HashSet<int> employeeSsns,
        CancellationToken ct)
    {
        // Calendar & prior year --------------------------------------------------------
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, ct);
        var priorYear = (short)(profitYear - 1);

        // Dictionaries ----------------------------------------------------------------
        var vestingBySsn = await _totalService
            .GetVestingRatio(ctx, profitYear, calInfo.FiscalEndDate)
            .Where(vr => employeeSsns.Contains(vr.Ssn ?? 0))
            .ToDictionaryAsync(vr => vr.Ssn ?? 0, vr => vr.Ratio, ct);

        var balanceBySsnLastYear = await _totalService
            .GetTotalBalanceSet(ctx, priorYear)
            .Where(tbs => employeeSsns.Contains(tbs.Ssn))
            .ToDictionaryAsync(tbs => tbs.Ssn, tbs => tbs.Total ?? 0, ct);

        var txnsBySsn = await TotalService
            .GetTransactionsBySsnForProfitYearForOracle(ctx, profitYear)
            .Where(txn => employeeSsns.Contains(txn.Ssn))
            .ToDictionaryAsync(txn => txn.Ssn, txn => txn, ct);

        // Snapshots -------------------------------------------------------------------
        return employeeSsns.Select(ssn => new EmployeeFinancialSnapshot(
            ssn,
            balanceBySsnLastYear.GetValueOrDefault(ssn),
            txnsBySsn.GetValueOrDefault(ssn) ?? new InternalProfitDetailDto(),
            vestingBySsn.GetValueOrDefault(ssn) ?? 0)).ToList();
    }

    private static IQueryable<ActiveMemberDto> ApplyStoreManagementFilter(IQueryable<ActiveMemberDto> q) =>
     q.Where(d =>
         ((d.EmploymentStatusId == EmploymentStatus.Constants.Active &&
           d.PayFrequencyId != PayFrequency.Constants.Monthly) ||
          (d.EmploymentStatusId == EmploymentStatus.Constants.Inactive && d.StoreNumber == 986)) &&
         (
             (d.DepartmentId == Department.Constants.Grocery && new[]
             {
                PayClassification.Constants.Manager,
                PayClassification.Constants.AssistantManager,
                PayClassification.Constants.FrontEndManager,
                PayClassification.Constants.Merchandiser,
                PayClassification.Constants.GroceryManager
             }.Contains(d.PayClassificationId)) ||
             (d.DepartmentId == Department.Constants.Meat && new[]
             {
                PayClassification.Constants.Manager,
                PayClassification.Constants.AssistantManager
             }.Contains(d.PayClassificationId)) ||
             (d.DepartmentId == Department.Constants.Produce && d.PayClassificationId == PayClassification.Constants.Manager) ||
             (d.DepartmentId == Department.Constants.Deli && d.PayClassificationId == PayClassification.Constants.Manager) ||
             (d.DepartmentId == Department.Constants.Dairy && d.PayClassificationId == PayClassification.Constants.Manager) ||
             (d.DepartmentId == Department.Constants.BeerAndWine && d.PayClassificationId == PayClassification.Constants.Manager) ||
             (d.DepartmentId == Department.Constants.Bakery && d.PayClassificationId == PayClassification.Constants.Manager)
         ));

    private static IQueryable<ActiveMemberDto> ApplyNonStoreManagementFilter(IQueryable<ActiveMemberDto> q) =>
        q.Where(d =>
            !(
                ((d.EmploymentStatusId == EmploymentStatus.Constants.Active &&
                  d.PayFrequencyId != PayFrequency.Constants.Monthly) ||
                 (d.EmploymentStatusId == EmploymentStatus.Constants.Inactive && d.StoreNumber == 986)) &&
                (
                    (d.DepartmentId == Department.Constants.Grocery && new[]
                    {
                    PayClassification.Constants.Manager,
                    PayClassification.Constants.AssistantManager,
                    PayClassification.Constants.FrontEndManager,
                    PayClassification.Constants.Merchandiser,
                    PayClassification.Constants.GroceryManager
                    }.Contains(d.PayClassificationId)) ||
                    (d.DepartmentId == Department.Constants.Meat && new[]
                    {
                    PayClassification.Constants.Manager,
                    PayClassification.Constants.AssistantManager
                    }.Contains(d.PayClassificationId)) ||
                    (d.DepartmentId == Department.Constants.Produce && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.Deli && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.Dairy && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.BeerAndWine && d.PayClassificationId == PayClassification.Constants.Manager) ||
                    (d.DepartmentId == Department.Constants.Bakery && d.PayClassificationId == PayClassification.Constants.Manager)
                )
            ));


    private static MemberYearSummaryDto BuildMemberYearSummary(
        ActiveMemberDto member,
        EmployeeFinancialSnapshot? snap)
    {
        // Fallback for missing snapshot (should not happen if dictionaries are aligned)
        snap ??= new EmployeeFinancialSnapshot(member.Ssn, 0, new InternalProfitDetailDto(), 0);

        var endBal = snap.BeginningBalance
                   + snap.Txn.TotalContributions
                   + snap.Txn.TotalEarnings
                   + snap.Txn.TotalForfeitures
                   + snap.Txn.Distribution
                   + snap.Txn.BeneficiaryAllocation;

        return new MemberYearSummaryDto
        {
            BadgeNumber = member.BadgeNumber,
            FullName = member.FullName,
            StoreNumber = member.StoreNumber,
            BeginningBalance = snap.BeginningBalance,
            Earnings = snap.Txn.TotalEarnings,
            Contributions = snap.Txn.TotalContributions,
            Distributions = snap.Txn.Distribution,
            Forfeitures = snap.Txn.TotalForfeitures,
            EndingBalance = endBal,
            VestedAmount = endBal * snap.VestingRatio,
            VestedPercent = (byte)(snap.VestingRatio * 100),
            PayClassificationId = member.PayClassificationId,
            PayClassificationName = member.PayClassificationName
        };
    }

    #endregion
}
