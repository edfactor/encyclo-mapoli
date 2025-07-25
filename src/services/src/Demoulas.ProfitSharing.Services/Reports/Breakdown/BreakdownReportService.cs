using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;


namespace Demoulas.ProfitSharing.Services.Reports.Breakdown;

public sealed class BreakdownReportService : IBreakdownService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;

    public BreakdownReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService,
        IDemographicReaderService demographicReaderService)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
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
        public decimal? CurrentBalance { get; init; }
        public decimal? VestedBalance { get; init; }
        public decimal? EtvaBalance { get; init; }
        public decimal? VestedPercent { get; init; }
        public DateOnly HireDate { get; init; }
        public DateOnly? TerminationDate { get; init; }
        public byte EnrollmentId { get; init; }
        public Decimal ProfitShareHours { get; init; }
    }

    private sealed record EmployeeFinancialSnapshot(
        int Ssn,
        decimal BeginningBalance,
        InternalProfitDetailDto Txn,
        decimal VestingRatio);

    private sealed record CombinedTotals(
        short StoreNumber,
        decimal VestingRatio,
        decimal EndBalance);

    #endregion

    public Task<GrandTotalsByStoreResponseDto> GetGrandTotals(
     YearRequest request,
     CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var memberStores =await (await BuildEmployeesBaseQuery(ctx, request.ProfitYear))
                .Select(m => new { m.Ssn, m.StoreNumber })
                .ToListAsync(cancellationToken);

            var ssns = memberStores.Select(x => x.Ssn).ToHashSet();
            ThrowIfInvalidSsns(ssns);

            var snapshots = await GetEmployeeFinancialSnapshotsAsync(
                ctx, request.ProfitYear, ssns, cancellationToken);

            
            var combined = memberStores
                .Join(
                    snapshots,
                    m => m.Ssn,
                    s => s.Ssn,
                    (m, s) => new CombinedTotals(
                        m.StoreNumber,
                        VestingRatio: s.VestingRatio,
                        EndBalance: s.BeginningBalance
                                     + s.Txn.TotalEarnings
                                     + s.Txn.TotalContributions
                                     + s.Txn.TotalForfeitures
                                     + s.Txn.Distribution
                                     + s.Txn.BeneficiaryAllocation))
                .ToList(); // bring into memory once

            HashSet<int> storeKeys = [700, 701, 800, 801, 802, 900];
            var categories = new[]
            {
            ("Grand Total",         (Func<CombinedTotals,bool>)(x => true)),
            ("100% Vested",         x => x.VestingRatio == 1m),
            ("Partially Vested",    x => x.VestingRatio is > 0m and < 1m),
            ("Not Vested",          x => x.VestingRatio == 0m)
        };

            var rows = new List<GrandTotalsByStoreRowDto>();

            foreach (var (label, predicate) in categories)
            {
                var subset = combined.Where(predicate);

                decimal SumFor(short store) =>
                    subset.Where(x => x.StoreNumber == store).Sum(x => x.EndBalance);

                var row = new GrandTotalsByStoreRowDto
                {
                    Category = label,
                    Store700 = SumFor(700),
                    Store701 = SumFor(701),
                    Store800 = SumFor(800),
                    Store801 = SumFor(801),
                    Store802 = SumFor(802),
                    Store900 = SumFor(900),
                    StoreOther = subset
                                    .Where(x => !storeKeys.Contains(x.StoreNumber))
                                    .Sum(x => x.EndBalance)
                };

                // compute the total across all columns
                row = row with
                {
                    RowTotal = row.Store700
                             + row.Store701
                             + row.Store800
                             + row.Store801
                             + row.Store802
                             + row.Store900
                             + row.StoreOther
                };

                rows.Add(row);
            }

            return new GrandTotalsByStoreResponseDto { Rows = rows };
        });
    }


    public Task<BreakdownByStoreTotals> GetTotalsByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ValidateStoreNumber(request);

            // ── Query ------------------------------------------------------------------
            var employeesBase = await BuildEmployeesBaseQuery(ctx, request.ProfitYear);
            employeesBase = employeesBase.Where(q => q.StoreNumber == request.StoreNumber);
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
        return GetMembersByStore(request, inActiveEmployees: false, terminatedEmployees: false, withBalance: false, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, inActiveEmployees: true, terminatedEmployees: false, withBalance: false, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersWithBalanceByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, inActiveEmployees: true, terminatedEmployees: false, withBalance: true, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithBalanceByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, inActiveEmployees: false, terminatedEmployees: true, withBalance: true, cancellationToken);
    }

    #region ── Private: common building blocks ───────────────────────────────────────────

    private Task<ReportResponseBase<MemberYearSummaryDto>> GetMembersByStore(
        BreakdownByStoreRequest request,
        bool inActiveEmployees,
        bool terminatedEmployees,
        bool withBalance,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ValidateStoreNumber(request);

            var employeesBase = await BuildEmployeesBaseQuery(ctx, request.ProfitYear);

            if (inActiveEmployees)
            {
                employeesBase = employeesBase.Where(e => e.EmploymentStatusId == EmploymentStatus.Constants.Inactive && e.TerminationCodeId != TerminationCode.Constants.Transferred);
            }

            if (terminatedEmployees)
            {
                employeesBase = employeesBase.Where(e => e.EmploymentStatusId == EmploymentStatus.Constants.Terminated && e.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension);
                                             
            }
            
            if (request.StoreNumber.HasValue)
            {
                employeesBase = employeesBase.Where(q => q.StoreNumber == request.StoreNumber.Value);
            }

            if (withBalance)
            {
                employeesBase = employeesBase.Where(e => e.VestedBalance.HasValue && e.VestedBalance.Value != 0);
            }


            if (withBalance && inActiveEmployees)
            {
                employeesBase = employeesBase
                    .Where(e => !ctx.ExcludedIds.Any(x=>e.BadgeNumber == x.ExcludedIdValue));
            }

            // Store‑level + management filter
            if (request.StoreManagement.HasValue)
            {
                employeesBase = request.StoreManagement.Value ? ApplyStoreManagementFilter(employeesBase)
                    : ApplyNonStoreManagementFilter(employeesBase);
            }

            if (request.BadgeNumber > 0)
            {
                employeesBase = employeesBase.Where(e => e.BadgeNumber == request.BadgeNumber);
            }

            if (!string.IsNullOrWhiteSpace(request.EmployeeName))
            {
                var pattern = $"%{request.EmployeeName.ToUpperInvariant()}%";
                employeesBase = employeesBase.Where(x => EF.Functions.Like(x.FullName.ToUpper(), pattern));
            }

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

            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            return new ReportResponseBase<MemberYearSummaryDto>
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = calInfo.FiscalBeginDate,
                EndDate = calInfo.FiscalEndDate,
                ReportName = $"Breakdown Report for {request.ProfitYear}",
                Response = new PaginatedResponseDto<MemberYearSummaryDto>
                {
                    Results = members,
                    Total = paginated.Total
                }
            };
        });
    }
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

    /// <summary>
    /// Base query for “active‐members” with ONE round-trip to Oracle.
    /// – Joins year-end Profit-Sharing balances  *and*  ETVA balances  
    /// – Re-creates the legacy COBOL store-bucket logic (700, 701, 800, 801, 802, 900) **inside the SQL**  
    /// – Returns the sequence already filtered to the store requested by the UI
    /// </summary>
    private async Task<IQueryable<ActiveMemberDto>> BuildEmployeesBaseQuery(
        ProfitSharingReadOnlyDbContext ctx, short profitYear)
    {
        /*──────────────────────────── 1️⃣  inline sub-queries – still IQueryable */
        var balances =
            _totalService.TotalVestingBalance(
                ctx, profitYear, DateTime.UtcNow.ToDateOnly());

        var etvaBalances =
            _totalService.GetTotalComputedEtva(ctx, profitYear);

        /* hard-coded list from COBOL
           https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/305004545/QPAY066TA.pco
           https://bitbucket.org/demoulas/hpux/raw/37d043c297e04f3a5a557e0163239177087c2163/iqs-source/QPAY066TA.pco

        11-14-02  R MAISON  #196500 REMNVED SSN#'S ** Redacted **
           *                             AT LINES 2750 THRU 2790           *
           *                             AS PER DON MULLIGAN       

        02/18/04  DPRUGH   P#7790  ADDED ** Redacted ** TO THE 701 SECTION .    *
           *                             ALSO CLEANED UP THE SECTION BY    *
           *                             REMNVING THE COMMENTED OUT SSN'S  *
           *                             SINCE THEY ARE ALREADY NOTED IN   *
           *                             THE COMMENT ABNVE THE SECTION  
         */
        int[] pensionerSsns = await ctx.ExcludedIds.Where(x => x.ExcludedIdTypeId == ExcludedIdType.Constants.QPay066TAExclusions)
            .Select(x => x.ExcludedIdValue)
            .ToArrayAsync();

        /*
       
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
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        var query =
            from d in demographics

            join pp in ctx.PayProfits on d.Id equals pp.DemographicId

            join b in balances on d.Ssn equals b.Ssn into balGrp
            from bal in balGrp.DefaultIfEmpty()

            join e in etvaBalances on d.Ssn equals e.Ssn into etvaGrp
            from etva in etvaGrp.DefaultIfEmpty()
            where pp.ProfitYear == profitYear

            select new ActiveMemberDto
            {
                BadgeNumber = d.BadgeNumber,

                /* ── “virtual” store computed in SQL (flat CASE) ─────────────── */
                StoreNumber =
                    (short)(d.TerminationCodeId == TerminationCode.Constants.Retired
                        ? 700
                        : pensionerSsns.Contains(d.Ssn)
                            ? 701
                            : (d.PayFrequencyId == PayFrequency.Constants.Monthly &&
                               (d.EmploymentStatusId == EmploymentStatus.Constants.Active ||
                                d.EmploymentStatusId == EmploymentStatus.Constants.Inactive))
                                ? 900
                                : ((d.EmploymentStatusId == EmploymentStatus.Constants.Terminated ||
                                    d.EmploymentStatusId == EmploymentStatus.Constants.Inactive) &&
                                   (d.PayFrequencyId == PayFrequency.Constants.Weekly ||
                                    d.PayFrequencyId == PayFrequency.Constants.Monthly) &&
                                   (((bal == null ? 0 : bal.CurrentBalance)
                                     + (etva == null ? 0 : etva.Total)) <= 0))
                                    ? 801
                                    : (d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                                       (d.PayFrequencyId == PayFrequency.Constants.Weekly ||
                                        d.PayFrequencyId == PayFrequency.Constants.Monthly) &&
                                       ((bal == null ? 0 : bal.CurrentBalance) > 0) &&
                                       ((bal == null ? 0 : bal.VestedBalance) == 0) &&
                                       ((etva == null ? 0 : etva.Total) == 0))
                                        ? 802
                                        : ((d.PayFrequencyId == PayFrequency.Constants.Weekly ||
                                            d.PayFrequencyId == PayFrequency.Constants.Monthly) &&
                                           d.EmploymentStatusId != EmploymentStatus.Constants.Active)
                                            ? 800
                                            : d.StoreNumber),

                /* ── plain columns ───────────────────────────────────────────── */
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
                VestedPercent = bal == null ? 0 : bal.VestingPercent,
                EtvaBalance = etva == null ? 0 : etva.Total,
                HireDate = d.HireDate,
                TerminationDate = d.TerminationDate,
                EnrollmentId = pp.EnrollmentId,
                ProfitShareHours = pp.CurrentHoursYear + pp.HoursExecutive
            };

        return query;
    }


    private async Task<List<EmployeeFinancialSnapshot>> GetEmployeeFinancialSnapshotsAsync(
        ProfitSharingReadOnlyDbContext ctx,
        short profitYear,
        HashSet<int> employeeSsns,
        CancellationToken ct)
    {
        // Calendar & prior year --------------------------------------------------------
        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitYear, ct);
        var priorYear = (short)(profitYear - 1);

        // Dictionaries ----------------------------------------------------------------
        var vestingBySsn = await _totalService.GetVestingRatio(ctx, profitYear, calInfo.FiscalEndDate)
            .Where(vr => employeeSsns.Contains(vr.Ssn))
            .ToDictionaryAsync(vr => vr.Ssn, vr => vr.Ratio, ct);

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
            vestingBySsn.GetValueOrDefault(ssn))).ToList();
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
            PayClassificationName = member.PayClassificationName,
            HireDate = member.HireDate,
            TerminationDate = member.TerminationDate,
            DateOfBirth = member.DateOfBirth,
            ProfitShareHours = member.ProfitShareHours,
            EnrollmentId = member.EnrollmentId,
        };
    }
    
    #endregion
}
