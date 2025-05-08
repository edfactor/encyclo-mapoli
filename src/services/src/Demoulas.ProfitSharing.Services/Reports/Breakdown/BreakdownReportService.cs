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
        public char? TerminationCodeId { get; init; }
        public decimal? CurrentBalance { get; set; }
        public decimal? VestedBalance { get; set; }
        public decimal? EtvaBalance { get; set; }
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

            // Store‑level + management filter
            employeesBase = request.StoreManagement ? ApplyStoreManagementFilter(employeesBase)
                : ApplyNonStoreManagementFilter(employeesBase);

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

        // 1️⃣  Parameterised sub-queries (still IQueryable, so EF composes them)
        var balances = _totalService.TotalVestingBalance(ctx,
            request.ProfitYear, DateTime.UtcNow.ToDateOnly());

        var etvaBalances = _totalService.GetTotalComputedEtva(ctx,
            request.ProfitYear);          // IQueryable<EtvaDto>

        // 2️⃣  One LINQ query ⇒ one SQL statement
        var activeMembers =
            from d in ctx.Demographics

            /* --- profit-sharing totals ------------------------------------ */
            join b in balances on d.Ssn equals b.Ssn into balGrp
            from bal in balGrp.DefaultIfEmpty()              // LEFT JOIN

            /* --- ETVA totals ---------------------------------------------- */
            join e in etvaBalances on d.Ssn equals e.Ssn into etvaGrp
            from etva in etvaGrp.DefaultIfEmpty()            // LEFT JOIN

            select new ActiveMemberDto
            {
                BadgeNumber = d.BadgeNumber,
                StoreNumber = IdentifyVirtualStoreNumber(d),   // must be SQL-translatable
                FullName = d.ContactInfo.FullName!,
                Ssn = d.Ssn,
                DateOfBirth = d.DateOfBirth,
                PayClassificationId = d.PayClassificationId,
                EmploymentStatusId = d.EmploymentStatusId,
                DepartmentId = d.DepartmentId,
                PayFrequencyId = d.PayFrequencyId,
                TerminationCodeId = d.TerminationCodeId,

                DepartmentName = d.Department!.Name,
                PayClassificationName = d.PayClassification!.Name,

                CurrentBalance = bal == null ? 0 : bal.CurrentBalance,
                VestedBalance = bal == null ? 0 : bal.VestedBalance,
                EtvaBalance = etva == null ? 0 : etva.Total
            };



        return activeMembers.Where(e => e.StoreNumber == request.StoreNumber);
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

    private static short IdentifyVirtualStoreNumber(
        Demographic member)
    {
        /*
         LEGACY COBOL - https://bitbucket.org/demoulas/hpux/raw/37d043c297e04f3a5a557e0163239177087c2163/iqs-source/QPAY066TA.pco

         *| Report store                                        | When the COBOL assigns it                                                                                | Key tests in the code                                                                                                                                  |
           | --------------------------------------------------- | -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
           | **700 – “Retired – Drawing Pension”**               | `B-TERM = "W"` (“W” is the retirement term-code).                                                        | `COMPUTE W-ST = WS-STR-VAL-PS-PENSION-RETIRED + 1000`                                                                                                  |
           | **701 – “Active – Drawing Pension”**                | Hard-coded list of SSNs that are still on the active payroll **after** retirement.                       | Later in the same paragraph:<br>`IF B-SSN = 023202688 OR … THEN COMPUTE W-ST = WS-STR-VAL-PS-PENSION-ACTIVE + 1000`                                    |
           | **800 – “Terminated / non-employee beneficiaries”** | Weekly **or** monthly pay-frequency and the employee is no longer active.                                | `IF B-FREQ = 1 OR B-FREQ = 2 THEN COMPUTE W-ST = WS-STR-VAL-PS-TERM-EMPL + 1000`                                                                       |
           | **801 – “Terminated w/ zero balance”**              | Same as 800 **but** profit-sharing balance is zero (or becomes zero after disbursements)                 | Immediately after the 800 test:<br>`IF (B-PS-AMT <= 0 OR ((B-PS-AMT + B-PROF-DISBURSE5) <= 0 …)) … COMPUTE W-ST = WS-STR-VAL-PS-TERM-EMPL-ZERO + 1000` |
           | **802 – “Terminated w/ balance but no vesting”**    | Balance > 0 **and** fully un-vested (`PS-VAMT = 0` and `PS-ETVA = 0`) and the status code is terminated. | `IF (B-PS-AMT > 0 AND (B-PS-VAMT = 0 AND B-PS-ETVA = 0) AND B-ST-CD = "T") … COMPUTE W-ST = WS-STR-VAL-PS-TERM-EMPL-NOVEST + 1000`                     |
           | **900 – “Monthly Payroll”**                         | Active or inactive employees who are still on the **monthly** payroll.                                   | Wherever the code finds `B-FREQ = 2` it sets 900:<br>`COMPUTE W-ST = WS-STR-VAL-PS-MONTHLY-EMPL + 1000`                                                |

        In short
            Retiree vs. active-pensioner is keyed off the termination code (“W”) or an explicit SSN list.
            Terminated buckets (800–802) depend on pay-frequency plus the size/vesting of the participant’s profit-sharing balance.
            Monthly payroll (900) is simply anyone with FREQ = 2.
         */

        /* ‣ 700  – retired (termination code “W”) */
        if (member.TerminationCodeId == TerminationCode.Constants.Retired)
        {
            return 700;
        }

        /* ‣ 701 – still on payroll but already receiving a pension
              (hard-coded SSN list copied from COBOL)                 */
        int[] ssnList =
        [
            023202688, 016201949, 023228733, 025329422, 001301944, 033324971, 020283297, 018260600, 017169396,
            026786919, 029321863, 016269940, 018306437, 126264073, 012242916, 028280107, 031260942, 024243451
        ];

        if (ssnList.Contains(member.Ssn))
        {
            return 701;
        }

        /* ‣ 900 – monthly payroll (frequency 2) that is NOT retired   */
        if (member.PayFrequencyId == PayFrequency.Constants.Monthly &&
            (member.EmploymentStatusId == EmploymentStatus.Constants.Active ||
             member.EmploymentStatusId == EmploymentStatus.Constants.Inactive))
        {
            return 900;
        }

        /* ‣ 801 – terminated/inactive and the net balance is ≤ 0      */
        if ((member.EmploymentStatusId == EmploymentStatus.Constants.Terminated ||
             member.EmploymentStatusId == EmploymentStatus.Constants.Inactive) && member.C)
        {
            return 801;
        }


        /* ‣ 802 – terminated, has a balance, but **no** vesting       */
        if (member.EmploymentStatusId == EmploymentStatus.Constants.Terminated )
        {
            /*
             *  WHEN f.pay_freq IN(1,2)
                     AND f.ps_balance > 0
                     AND NVL(f.vested_balance ,0)         = 0
                     AND NVL(f.etva_balance  ,0)          = 0
                     THEN 802
             */
            return 802;
        }




        /* ‣ 800 – everybody else who is weekly/monthly and not active */
        if ((member.PayFrequencyId == PayFrequency.Constants.Weekly ||
             member.PayFrequencyId == PayFrequency.Constants.Monthly)
            && member.EmploymentStatusId != EmploymentStatus.Constants.Active)
        {
            /*
             *  WHEN f.pay_freq IN(1,2)
                     AND f.ps_balance > 0
                     AND NVL(f.vested_balance ,0)         = 0
                     AND NVL(f.etva_balance  ,0)          = 0
                     THEN 802
             */
            return 802;
        }

        /* Anything else keeps its original store value.*/
        return member.StoreNumber;

    }

    #endregion
}
