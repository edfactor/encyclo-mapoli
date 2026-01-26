using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Constants;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Services.Reports.Breakdown;

public sealed class BreakdownReportService : IBreakdownService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dataContextFactory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IPayrollDuplicateSsnReportService _duplicateSsnReportService;
    private readonly ICrossReferenceValidationService _crossReferenceValidationService;
    private readonly TimeProvider _timeProvider;

    public BreakdownReportService(
        IProfitSharingDataContextFactory dataContextFactory,
        ICalendarService calendarService,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        IPayrollDuplicateSsnReportService duplicateSsnReportService,
        ICrossReferenceValidationService crossReferenceValidationService,
        TimeProvider timeProvider)
    {
        _dataContextFactory = dataContextFactory;
        _calendarService = calendarService;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _duplicateSsnReportService = duplicateSsnReportService;
        _crossReferenceValidationService = crossReferenceValidationService;
        _timeProvider = timeProvider;
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
        public string PayClassificationId { get; init; } = string.Empty;
        public DateOnly DateOfBirth { get; init; }
        public char EmploymentStatusId { get; init; }
        public byte DepartmentId { get; init; }
        public byte PayFrequencyId { get; init; }
        public char? TerminationCodeId { get; init; }
        public decimal? CurrentBalance { get; init; }
        public decimal? VestedBalance { get; init; }
        public decimal? EtvaBalance { get; init; }
        public decimal? VestedPercent { get; init; }
        public decimal? BeneficiaryAllocation { get; init; }
        public DateOnly HireDate { get; init; }
        public DateOnly? TerminationDate { get; init; }
        public byte EnrollmentId { get; init; }
        public Decimal ProfitShareHours { get; init; }
        public byte? YearsInPlan { get; internal set; }
        public string Street1 { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        public int CertificateSort { get; internal set; }
        public decimal? BeginningBalance { get; internal set; }
        public decimal Earnings { get; internal set; }
        public decimal Contributions { get; internal set; }
        public decimal Forfeitures { get; internal set; }
        public decimal Distributions { get; internal set; }

    }

    private sealed record EmployeeFinancialSnapshot(
        int Ssn,
        int BadgeNumber,
        decimal BeginningBalance,
        ProfitDetailRollup Txn,
        decimal VestingRatio);

    private enum StatusFilter
    {
        All = 0,
        Active,
        Inactive,
        Terminated,
        Retired,
        Monthly,
    }

    private enum Balance
    {
        BalanceOrNoBalance = 0,
        HasVestedBalance,
        HasBalanceActivity,
        HasCurrentBalanceNotVested,
        HasDistributionForfeitOrContribution,
    }
    #endregion

    public Task<GrandTotalsByStoreResponseDto> GetGrandTotals(
     GrandTotalsByStoreRequest request,
     CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);

            // Get active store IDs for the "Other" category
            // Active stores are retail stores (1-140) that aren't in the specific buckets (700, 701, 800, 801, 802, 900)
            var activeStoreIds = StoreTypes.RetailStores
                .Where(s => s < 700 || s > 900) // Exclude profit sharing report stores
                .Select(s => (short)s)
                .ToHashSet();

            // Aggregate per-store and per-vesting-bucket COUNTS in a SINGLE database query
            var baseQuery = await BuildEmployeesBaseQuery(ctx, request.ProfitYear, calInfo.FiscalEndDate);

            // Apply Under 21 filter if requested (PS-2442: Filter for under 21 participants in grand totals)
            if (request.Under21Participants)
            {
                // Calculate age using the fiscal end date and filter for participants under 21
                // Age is calculated server-side from DateOfBirth relative to the fiscal end date
                DateOnly under21Date = calInfo.FiscalEndDate.AddYears(-21);

                baseQuery = baseQuery.Where(e => e.DateOfBirth > under21Date);
            }

            // Single query that gets all aggregations needed - COUNT participants instead of SUM balances
            var aggregatedData = await baseQuery
                .Select(m => new
                {
                    m.StoreNumber,
                    m.Ssn,
                    // Compute bucket in SQL to enable single-query aggregation
                    VestingBucket = m.VestedPercent == 1m ? "100% Vested"
                                  : (m.VestedPercent > 0m && m.VestedPercent < 1m) ? "Partially Vested"
                                  : (m.VestedPercent == 0m || m.VestedPercent == null) ? "Not Vested"
                                  : "Other"
                })
                .ToListAsync(cancellationToken); // Single DB roundtrip

            // Validate SSNs (using in-memory data already loaded)
            var ssns = aggregatedData.Select(x => x.Ssn).Distinct().ToHashSet();
            ThrowIfInvalidSsns(ssns);

            // Group in memory - COUNT participants instead of SUM balances
            var perStoreTotals = aggregatedData
                .GroupBy(p => p.StoreNumber)
                .ToDictionary(g => g.Key, g => g.Count());

            var perStoreBuckets = aggregatedData
                .GroupBy(p => (p.StoreNumber, p.VestingBucket))
                .ToDictionary(g => g.Key, g => g.Count());

            var storeKeys = new HashSet<short>(new short[] { 700, 701, 800, 801, 802, 900 });
            var categories = new[] { "Grand Total", "100% Vested", "Partially Vested", "Not Vested" };

            var rows = new List<GrandTotalsByStoreRowDto>(categories.Length);

            foreach (var label in categories)
            {
                var row = new GrandTotalsByStoreRowDto
                {
                    Category = label,
                    Store700 = label == "Grand Total"
                        ? (perStoreTotals.TryGetValue(700, out var s700) ? s700 : 0)
                        : (perStoreBuckets.TryGetValue((700, label), out var b700) ? b700 : 0),
                    Store701 = label == "Grand Total"
                        ? (perStoreTotals.TryGetValue(701, out var s701) ? s701 : 0)
                        : (perStoreBuckets.TryGetValue((701, label), out var b701) ? b701 : 0),
                    Store800 = label == "Grand Total"
                        ? (perStoreTotals.TryGetValue(800, out var s800) ? s800 : 0)
                        : (perStoreBuckets.TryGetValue((800, label), out var b800) ? b800 : 0),
                    Store801 = label == "Grand Total"
                        ? (perStoreTotals.TryGetValue(801, out var s801) ? s801 : 0)
                        : (perStoreBuckets.TryGetValue((801, label), out var b801) ? b801 : 0),
                    Store802 = label == "Grand Total"
                        ? (perStoreTotals.TryGetValue(802, out var s802) ? s802 : 0)
                        : (perStoreBuckets.TryGetValue((802, label), out var b802) ? b802 : 0),
                    Store900 = label == "Grand Total"
                        ? (perStoreTotals.TryGetValue(900, out var s900) ? s900 : 0)
                        : (perStoreBuckets.TryGetValue((900, label), out var b900) ? b900 : 0),
                    // Other category: use active store IDs (retail stores 1-140, excluding special buckets)
                    StoreOther = label == "Grand Total"
                        ? perStoreTotals.Where(p => activeStoreIds.Contains(p.Key)).Sum(p => p.Value)
                        : perStoreBuckets.Where(p => activeStoreIds.Contains(p.Key.StoreNumber) && p.Key.VestingBucket == label).Sum(p => p.Value)
                };

                row = row with
                {
                    RowTotal = row.Store700 + row.Store701 + row.Store800 + row.Store801 + row.Store802 + row.Store900 + row.StoreOther
                };

                rows.Add(row);
            }

            return new GrandTotalsByStoreResponseDto { Rows = rows };
        }, cancellationToken);
    }


    public Task<BreakdownByStoreTotals> GetTotalsByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            ValidateStoreNumber(request);
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);

            //  ── Query ------------------------------------------------------------------
            var employeesBase = await BuildEmployeesBaseQuery(ctx, request.ProfitYear, calInfo.FiscalEndDate);

            if (request.StoreNumber != null && request.StoreNumber != -1)
            {
                employeesBase = employeesBase.Where(q => q.StoreNumber == request.StoreNumber);
            }

            // PERFORMANCE OPTIMIZATION: Materialize the full query with ALL financial data in ONE database call
            // instead of getting SSNs first then re-querying for financial data (avoiding multiple WHERE IN queries)
            var employees = await employeesBase
                .TagWith($"GetTotalsByStore-AllData-Store{request.StoreNumber}-{request.ProfitYear}")
                .ToListAsync(cancellationToken);

            ThrowIfInvalidSsns(employees.Select(e => e.Ssn).ToHashSet());

            // Degenerate guard: prevent full table scan
            if (employees.Count == 0)
            {
                return new BreakdownByStoreTotals
                {
                    TotalNumberEmployees = 0,
                    TotalBeginningBalances = 0,
                    TotalEarnings = 0,
                    TotalContributions = 0,
                    TotalForfeitures = 0,
                    TotalDisbursements = 0,
                    TotalEndBalances = 0,
                    TotalVestedBalance = 0
                };
            }

            // ── Aggregate from already-loaded data ────────────────────────────────────
            var totals = new BreakdownByStoreTotals
            {
                TotalNumberEmployees = (ushort)employees.Count,
                TotalBeginningBalances = employees.Sum(e => e.BeginningBalance ?? 0),
                TotalEarnings = employees.Sum(e => e.Earnings),
                TotalContributions = employees.Sum(e => e.Contributions),
                TotalForfeitures = employees.Sum(e => e.Forfeitures),
                TotalDisbursements = employees.Sum(e => e.Distributions)
            };

            totals.TotalEndBalances = totals.TotalBeginningBalances
                                     + totals.TotalEarnings
                                     + totals.TotalContributions
                                     + totals.TotalForfeitures
                                     + totals.TotalDisbursements
                                     + employees.Sum(e => e.BeneficiaryAllocation ?? 0);

            // Use the pre-calculated VestedBalance from TotalVestingBalanceAlt query
            // which uses the correct profit code logic:
            // Contributions (PC 0) + Earnings (PC 0) + EtvaForfeitures (PC 0) +
            // Distributions (PC 1,3,5 * -1) + Forfeitures (PC 2 * -1) +
            // VestedEarnings (PC 6 contrib + PC 8 earnings + PC 9 forfeit * -1)
            // Then applies vesting ratio and ETVA adjustments
            totals.TotalVestedBalance = employees.Sum(e => e.VestedBalance ?? 0);

            totals.CrossReferenceValidation = await _crossReferenceValidationService.ValidateBreakoutReportGrandTotalAsync(
                request.ProfitYear,
                totals.TotalNumberEmployees,
                totals.TotalBeginningBalances,
                totals.TotalEarnings,
                totals.TotalContributions,
                totals.TotalDisbursements,
                totals.TotalEndBalances,
                cancellationToken);

            return totals;
        }, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Active, Balance.BalanceOrNoBalance, applyQPAY066A1Filter: false, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetMembersWithBalanceActivityByStore(BreakdownByStoreRequest request, int[]? Ssns, int[] BadgeNumbers, CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.All, Balance.HasBalanceActivity, applyQPAY066A1Filter: false, Ssns, BadgeNumbers, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Inactive, Balance.BalanceOrNoBalance, applyQPAY066A1Filter: true, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetInactiveMembersWithVestedBalanceByStore(
        BreakdownByStoreRequest request,
        CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Inactive, Balance.HasVestedBalance, applyQPAY066A1Filter: true, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetRetiredEmployessWithBalanceActivity(
       TerminatedEmployeesWithBalanceBreakdownRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Retired, Balance.HasBalanceActivity, applyQPAY066A1Filter: false, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetActiveMembersWithVestedBalanceByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Active, Balance.HasVestedBalance, applyQPAY066A1Filter: false, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithVestedBalanceByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Terminated, Balance.HasVestedBalance, applyQPAY066A1Filter: false, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithBalanceActivityByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Terminated, Balance.HasBalanceActivity, applyQPAY066A1Filter: false, ssns: null, badgeNumbers: null, cancellationToken);
    }

    /// <summary>
    /// Retrieves terminated employees with current balance not vested plus qualifying beneficiaries.
    /// Logic:
    /// 1. Employees terminated during current fiscal year with vesting percentage &lt; 20%
    /// 2. Beneficiaries who are NOT also employees (by SSN match)
    /// 3. Beneficiaries must have a vested amount OR profit detail records with profit_code_id 5 or 6
    /// </summary>
    public Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithCurrentBalanceNotVestedByStore(
       BreakdownByStoreRequest request,
       CancellationToken cancellationToken)
    {
        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
            var asOfDate = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);

            // Beneficiary profit codes: OutgoingXferBeneficiary (5) and IncomingQdroBeneficiary (6)
            var beneficiaryProfitCodes = new[] { ProfitCode.Constants.OutgoingXferBeneficiary.Id, ProfitCode.Constants.IncomingQdroBeneficiary.Id };

            // Build demographic query
            var demographics = await _demographicReaderService.BuildDemographicQueryAsync(ctx);

            // ═══════════════════════════════════════════════════════════════════════════
            // OPTIMIZED: Use database-side subqueries instead of loading large sets
            // ═══════════════════════════════════════════════════════════════════════════

            // 1. Get terminated employees with vesting < 20%
            var terminatedEmployees = await BuildAndExecuteTerminatedEmployeesQueryAsync(
                ctx, demographics, request, calInfo, cancellationToken);

            var terminatedSsns = terminatedEmployees.Select(e => e.Ssn).ToHashSet();
            ThrowIfInvalidSsns(terminatedSsns);

            // Skip beneficiaries if store filter excludes store 800
            // Beneficiaries are always bucketed to virtual store 800 (see line 438 below)
            // If user filters to a specific store other than 800, exclude beneficiaries entirely
            List<ActiveMemberDto> beneficiaries;
            if (request.StoreNumber.HasValue && request.StoreNumber.Value != 800)
            {
                beneficiaries = [];
            }
            else
            {
                // Create subqueries for database-side filtering (NOT materialized)
                // Subquery: SSNs that are employees
                var employeeSsnsSubquery = demographics.Select(d => d.Ssn);

                // Subquery: SSNs with beneficiary profit codes 5 or 6
                var profitCodeSsnsSubquery = ctx.ProfitDetails
                    .Where(pd => pd.ProfitYear == request.ProfitYear && beneficiaryProfitCodes.Contains(pd.ProfitCodeId))
                    .Select(pd => pd.Ssn);

                // Subquery: SSNs with vested balance > 0
                var vestingSsnsSubquery = _totalService
                    .TotalVestingBalance(ctx, request.ProfitYear, asOfDate)
                    .Where(vb => vb.VestedBalance > 0)
                    .Select(vb => vb.Ssn);

                // Query beneficiaries with all filtering done in the database
                var beneficiaryData = await ctx.Beneficiaries
                    .TagWith("GetTerminatedNotVested-Beneficiaries")
                    .Include(b => b.Contact)
                        .ThenInclude(c => c!.ContactInfo)
                    .Include(b => b.Contact)
                        .ThenInclude(c => c!.Address)
                    .Where(b => !b.IsDeleted)
                    .Where(b => !employeeSsnsSubquery.Contains(b.Contact!.Ssn)) // NOT IN employees (subquery)
                    .Where(b => profitCodeSsnsSubquery.Contains(b.Contact!.Ssn) ||
                                vestingSsnsSubquery.Contains(b.Contact!.Ssn)) // Has profit codes 5/6 OR vested balance
                    .Select(b => new
                    {
                        b.BadgeNumber,
                        ContactSsn = b.Contact!.Ssn,
                        ContactDateOfBirth = b.Contact.DateOfBirth,
                        ContactFullName = b.Contact.ContactInfo.FullName,
                        ContactStreet = b.Contact.Address.Street,
                        ContactCity = b.Contact.Address.City,
                        ContactState = b.Contact.Address.State,
                        ContactPostalCode = b.Contact.Address.PostalCode
                    })
                    .ToListAsync(cancellationToken);

                // Convert beneficiaries to ActiveMemberDto in memory
                beneficiaries = beneficiaryData.Select(b => new ActiveMemberDto
                {
                    BadgeNumber = b.BadgeNumber,
                    StoreNumber = 800, // Beneficiaries go to store 800 bucket
                    FullName = b.ContactFullName ?? string.Empty,
                    Ssn = b.ContactSsn,
                    DateOfBirth = b.ContactDateOfBirth,
                    PayClassificationId = string.Empty,
                    EmploymentStatusId = EmploymentStatus.Constants.Terminated,
                    DepartmentId = 0,
                    PayFrequencyId = 0,
                    TerminationCodeId = null,
                    DepartmentName = string.Empty,
                    PayClassificationName = string.Empty,
                    CurrentBalance = 0,
                    VestedBalance = 0,
                    VestedPercent = 0,
                    EtvaBalance = 0,
                    YearsInPlan = 0,
                    HireDate = DateOnly.MinValue,
                    TerminationDate = null,
                    EnrollmentId = EnrollmentConstants.NotEnrolled,
                    ProfitShareHours = 0,
                    Street1 = b.ContactStreet,
                    City = b.ContactCity,
                    State = b.ContactState,
                    PostalCode = b.ContactPostalCode,
                    BeginningBalance = 0,
                    Earnings = 0,
                    Distributions = 0,
                    Contributions = 0,
                    Forfeitures = 0,
                    BeneficiaryAllocation = 0,
                    CertificateSort = 0
                }).ToList();

                if (beneficiaries.Count > 0)
                {
                    var beneficiarySsns = beneficiaries.Select(b => b.Ssn).ToHashSet();
                    ThrowIfInvalidSsns(beneficiarySsns);
                }
            }

            // Combine employees and beneficiaries in memory
            var combined = terminatedEmployees.Concat(beneficiaries).ToList();

            // Apply sorting and pagination in memory (matching GetPaginatedResults logic)
            IEnumerable<ActiveMemberDto> ordered;
            if (ReferenceData.CertificateSort.Equals(request.SortBy, StringComparison.InvariantCultureIgnoreCase))
            {
                ordered = combined
                    .OrderBy(e => e.StoreNumber)
                    .ThenBy(e => e.CertificateSort)
                    .ThenBy(e => e.FullName);
            }
            else
            {
                ordered = combined
                    .OrderBy(e => e.StoreNumber)
                    .ThenBy(e => e.FullName);
            }

            var total = combined.Count;
            var results = ordered
                .Skip(request.Skip ?? 0)
                .Take(request.Take ?? 25)
                .ToList();

            // Get financial snapshots for the paginated results
            var resultSsns = results.Select(r => r.Ssn).ToHashSet();
            var snapshots = await GetEmployeeFinancialSnapshotsAsync(
                ctx, request.ProfitYear, resultSsns, cancellationToken);
            var snapshotByKey = snapshots.ToDictionary(s => (s.Ssn, s.BadgeNumber));

            // Build final response
            var members = results
                .Select(d => BuildMemberYearSummary(d, snapshotByKey.GetValueOrDefault((d.Ssn, d.BadgeNumber))))
                .ToList();

            return new ReportResponseBase<MemberYearSummaryDto>
            {
                ReportDate = DateTimeOffset.UtcNow,
                StartDate = calInfo.FiscalBeginDate,
                EndDate = calInfo.FiscalEndDate,
                ReportName = $"Terminated Members with Current Balance Not Vested for {request.ProfitYear}",
                Response = new PaginatedResponseDto<MemberYearSummaryDto>
                {
                    Results = members,
                    Total = total
                }
            };
        }, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetTerminatedMembersWithBeneficiaryByStore(
       TerminatedEmployeesWithBalanceBreakdownRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Terminated, Balance.BalanceOrNoBalance, applyQPAY066A1Filter: true, ssns: null, badgeNumbers: null, cancellationToken);
    }

    public Task<ReportResponseBase<MemberYearSummaryDto>> GetMonthlyEmployeesWithActivity(
       TerminatedEmployeesWithBalanceBreakdownRequest request,
       CancellationToken cancellationToken)
    {
        return GetMembersByStore(request, StatusFilter.Monthly, Balance.HasDistributionForfeitOrContribution, applyQPAY066A1Filter: false, ssns: null, badgeNumbers: null, cancellationToken);
    }

    #region ── Private: common building blocks ───────────────────────────────────────────

    private Task<ReportResponseBase<MemberYearSummaryDto>> GetMembersByStore(
        BreakdownByStoreRequest request,
        StatusFilter employeeStatusFilter,
        Balance balanceFilter,
        bool applyQPAY066A1Filter,
        int[]? ssns,
        int[]? badgeNumbers,
        CancellationToken cancellationToken)
    {

        return _dataContextFactory.UseReadOnlyContext(async ctx =>
        {
            var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);

            if (request.StoreNumber.HasValue)
            {
                ValidateStoreNumber(request);
            }

            // Get composable query - CRITICAL PERFORMANCE FIX
            var employeesBase = await BuildEmployeesBaseQuery(ctx, request.ProfitYear, calInfo.FiscalEndDate);
            var startEndDateRequest = request as IStartEndDateRequest;

            // Apply status filter BEFORE materialization - PERFORMANCE OPTIMIZATION
            if (employeeStatusFilter == StatusFilter.Active)
            {
                employeesBase = employeesBase.Where(e => e.EmploymentStatusId == EmploymentStatus.Constants.Active);
            }
            else if (employeeStatusFilter == StatusFilter.Inactive)
            {
                employeesBase = employeesBase.Where(e => e.EmploymentStatusId == EmploymentStatus.Constants.Inactive && e.TerminationCodeId != TerminationCode.Constants.Transferred);
            }
            else if (employeeStatusFilter == StatusFilter.Monthly)
            {
                employeesBase = employeesBase.Where(e => e.PayFrequencyId == PayFrequency.Constants.Monthly);
            }

            if (employeeStatusFilter == StatusFilter.Terminated || employeeStatusFilter == StatusFilter.Retired)
            {
                employeesBase = employeesBase.Where(e => e.EmploymentStatusId == EmploymentStatus.Constants.Terminated);
                if (employeeStatusFilter == StatusFilter.Terminated)
                {
                    employeesBase = employeesBase.Where(e => e.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension);
                }
                else
                {
                    employeesBase = employeesBase.Where(e => e.TerminationCodeId == TerminationCode.Constants.RetiredReceivingPension);
                }

                var startEndDates = request as IStartEndDateRequest;
                if (startEndDates != default && (startEndDates.StartDate.HasValue || startEndDates.EndDate.HasValue))
                {
                    if (startEndDates.StartDate.HasValue)
                    {
                        employeesBase = employeesBase.Where(e => e.TerminationDate >= startEndDates.StartDate);
                    }
                    if (startEndDates.EndDate.HasValue)
                    {
                        if (startEndDates.EndDate.Value < startEndDates.StartDate)
                        {
                            throw new InvalidOperationException("End date cannot be earlier than start date.");
                        }
                        employeesBase = employeesBase.Where(e => e.TerminationDate <= startEndDates.EndDate);
                    }
                }

            }

            if (request.StoreNumber.HasValue)
            {
                employeesBase = employeesBase.Where(q => q.StoreNumber == request.StoreNumber.Value);
            }

            employeesBase = balanceFilter switch
            {
                Balance.BalanceOrNoBalance => employeesBase,
                Balance.HasVestedBalance => employeesBase.Where(e => e.VestedBalance.HasValue && e.VestedBalance.Value > 0),
                Balance.HasBalanceActivity => employeesBase.Where(e =>
                    (e.VestedBalance.HasValue && e.VestedBalance.Value > 0)
                    || (e.CurrentBalance.HasValue && e.CurrentBalance.Value > 0)
                    || (e.Earnings != 0)
                    || (e.Distributions != 0)
                    || (e.Forfeitures != 0)
                    || (e.Contributions != 0)),
                Balance.HasCurrentBalanceNotVested => employeesBase.Where(e => e.CurrentBalance.HasValue && e.CurrentBalance.Value > 0 && (e.VestedBalance == null || e.VestedBalance.Value == 0)),
                Balance.HasDistributionForfeitOrContribution => employeesBase.Where(e =>
                    (e.Distributions != 0)
                    || (e.Forfeitures != 0)
                    || (e.Contributions != 0)),
                _ => employeesBase
            };

            if (ssns != null && ssns.Length > 0)
            {
                var ssnSet = ssns.ToHashSet();
                employeesBase = employeesBase.Where(e => ssnSet.Contains(e.Ssn));
            }

            if (badgeNumbers != null && badgeNumbers.Length > 0)
            {
                var badgeNumberSet = badgeNumbers.ToHashSet();
                employeesBase = employeesBase.Where(e => badgeNumberSet.Contains(e.BadgeNumber));
            }

            // QPAY066A-1 filter: Limits to employees with beneficiary allocation OR vested balance,
            // AND restricts to employees with 3 or fewer years in the plan
            if (applyQPAY066A1Filter)
            {
                var profitCodes = new[] { ProfitCode.Constants.IncomingQdroBeneficiary.Id, ProfitCode.Constants.OutgoingXferBeneficiary.Id };

                var ssnsWithBeneficiaryAllocation = ctx.ProfitDetails
                    .Where(ba => ba.ProfitYear == request.ProfitYear && profitCodes.Contains(ba.ProfitCodeId))
                    .GroupBy(x => x.Ssn)
                    // Fixed: Match the calculation used in GetTransactionsBySsnForProfitYearForOracle
                    // OutgoingXferBeneficiary (5): -forfeiture, IncomingQdroBeneficiary (6): +contribution
                    .Where(x => x.Sum(r => r.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
                        ? -r.Forfeiture
                        : (r.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id ? r.Contribution : 0)) > 0)
                    .Select(ba => ba.Key);

                employeesBase = employeesBase.Where(e =>
                    (ssnsWithBeneficiaryAllocation.Contains(e.Ssn) || e.VestedBalance > 0)
                    && e.YearsInPlan <= 3);
            }


            if (balanceFilter == Balance.HasVestedBalance && employeeStatusFilter == StatusFilter.Inactive)
            {
                employeesBase = employeesBase
                    .Where(e => !ctx.ExcludedIds.Any(x => e.BadgeNumber == x.ExcludedIdValue));
                if (startEndDateRequest != null)
                {
                    employeesBase = employeesBase.Where(e => e.TerminationDate >= startEndDateRequest.StartDate &&
                                                             e.TerminationDate <= startEndDateRequest.EndDate);
                }
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

            var paginated = await GetPaginatedResults(request, employeesBase, employeeStatusFilter, cancellationToken);
            var employeeSsns = paginated.Results.Select(r => r.Ssn).ToHashSet();

            ThrowIfInvalidSsns(employeeSsns);

            var snapshots = await GetEmployeeFinancialSnapshotsAsync(
                ctx, request.ProfitYear, employeeSsns, cancellationToken);
            var snapshotByKey = snapshots.ToDictionary(s => (s.Ssn, s.BadgeNumber));

            // Build final response (sorting already done by GetPaginatedResults in SQL)
            var members = paginated.Results
                .Select(d => BuildMemberYearSummary(d, snapshotByKey.GetValueOrDefault((d.Ssn, d.BadgeNumber))))
                .ToList();

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
        }, cancellationToken);
    }

    private static async Task<PaginatedResponseDto<ActiveMemberDto>> GetPaginatedResults(
        BreakdownByStoreRequest request,
        IQueryable<ActiveMemberDto> employeesBase,
        StatusFilter employeeStatusFilter,
        CancellationToken cancellationToken)
    {
        // Apply sorting BEFORE pagination (in SQL) - PERFORMANCE OPTIMIZATION
        IQueryable<ActiveMemberDto> orderedQuery;

        if (ReferenceData.CertificateSort.Equals(request.SortBy, StringComparison.InvariantCultureIgnoreCase))
        {
            orderedQuery = employeesBase
                .OrderBy(e => e.StoreNumber)
                .ThenBy(e => e.CertificateSort)
                .ThenBy(e => e.FullName);
        }
        else
        {
            orderedQuery = employeesBase
                .OrderBy(e => e.StoreNumber)
                .ThenBy(e => employeeStatusFilter == StatusFilter.All ? e.CertificateSort : 0)
                .ThenBy(e => e.FullName);
        }

        var total = await orderedQuery.CountAsync(cancellationToken);
        var results = await orderedQuery
            .Skip(request.Skip ?? 0)
            .Take(request.Take ?? 25)
            .ToListAsync(cancellationToken);

        return new PaginatedResponseDto<ActiveMemberDto>
        {
            Results = results,
            Total = total
        };
    }

    /// <summary>
    /// Builds and executes a lightweight query for terminated employees with vesting &lt; 20%.
    /// This is optimized for the specific filters needed by GetTerminatedMembersWithCurrentBalanceNotVestedByStore.
    /// </summary>
    private Task<List<ActiveMemberDto>> BuildAndExecuteTerminatedEmployeesQueryAsync(
        ProfitSharingReadOnlyDbContext ctx,
        IQueryable<Demographic> demographics,
        BreakdownByStoreRequest request,
        CalendarResponseDto calInfo,
        CancellationToken cancellationToken)
    {
        var asOfDate = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);

        // Build targeted query for terminated employees with vesting < 20%
        var balances = _totalService.TotalVestingBalance(ctx, request.ProfitYear, asOfDate);

        var query =
            from d in demographics.Include(x => x.Address)
            join pp in ctx.PayProfits on d.Id equals pp.DemographicId
            join b in balances on d.Ssn equals b.Ssn into balGrp
            from bal in balGrp.DefaultIfEmpty()
            where pp.ProfitYear == request.ProfitYear
                  && d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                  && d.TerminationDate.HasValue
                  && d.TerminationDate.Value >= calInfo.FiscalBeginDate
                  && d.TerminationDate.Value <= calInfo.FiscalEndDate
                  && (bal == null ? 0 : bal.VestingPercent) < 0.20m
            select new ActiveMemberDto
            {
                BadgeNumber = d.BadgeNumber,
                StoreNumber = d.StoreNumber,
                FullName = d.ContactInfo.FullName ?? string.Empty,
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
                EtvaBalance = 0,
                YearsInPlan = bal == null ? 0 : bal.YearsInPlan,
                HireDate = d.HireDate,
                TerminationDate = d.TerminationDate,
                EnrollmentId = pp.VestingScheduleId == 0
                    ? EnrollmentConstants.NotEnrolled
                    : pp.HasForfeited
                        ? pp.VestingScheduleId == VestingSchedule.Constants.OldPlan
                            ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                            : EnrollmentConstants.NewVestingPlanHasForfeitureRecords
                        : pp.VestingScheduleId == VestingSchedule.Constants.OldPlan
                            ? EnrollmentConstants.OldVestingPlanHasContributions
                            : EnrollmentConstants.NewVestingPlanHasContributions,
                ProfitShareHours = pp.TotalHours,
                Street1 = d.Address.Street,
                City = d.Address.City,
                State = d.Address.State,
                PostalCode = d.Address.PostalCode,
                BeginningBalance = 0,
                Earnings = 0,
                Distributions = 0,
                Contributions = 0,
                Forfeitures = 0,
                BeneficiaryAllocation = 0,
                CertificateSort = 0
            };

        // Apply optional store filter
        if (request.StoreNumber.HasValue)
        {
            query = query.Where(q => q.StoreNumber == request.StoreNumber.Value);
        }

        // Apply optional badge filter
        if (request.BadgeNumber > 0)
        {
            query = query.Where(e => e.BadgeNumber == request.BadgeNumber);
        }

        // Apply optional name filter
        if (!string.IsNullOrWhiteSpace(request.EmployeeName))
        {
            var pattern = $"%{request.EmployeeName.ToUpperInvariant()}%";
            query = query.Where(x => EF.Functions.Like(x.FullName.ToUpper(), pattern));
        }

        // Apply store management filter
        if (request.StoreManagement.HasValue)
        {
            query = request.StoreManagement.Value
                ? ApplyStoreManagementFilter(query)
                : ApplyNonStoreManagementFilter(query);
        }

        return query
            .TagWith("GetTerminatedNotVested-TerminatedEmployees")
            .ToListAsync(cancellationToken);
    }

    private static void ValidateStoreNumber(BreakdownByStoreRequest request)
    {
        if (request.StoreNumber < -1)
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
    /// Base query for "active‐members" with ONE round-trip to Oracle.
    /// – Joins year-end Profit-Sharing balances  *and*  ETVA balances
    /// – Re-creates the legacy COBOL store-bucket logic (700, 701, 800, 801, 802, 900) **inside the SQL**
    /// – Returns the sequence already filtered to the store requested by the UI
    /// </summary>
    private async Task<IQueryable<ActiveMemberDto>> BuildEmployeesBaseQuery(
        ProfitSharingReadOnlyDbContext ctx, short profitYear, DateOnly fiscalEndDate)
    {
        /*──────────────────────────── 1️⃣  inline sub-queries – still IQueryable */
        var asOfDate = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);
        var balances =
            _totalService.TotalVestingBalance(
                ctx, profitYear, asOfDate);

        short priorYear = (short)(profitYear - 1);
        var lastYearBalances = _totalService
            .TotalVestingBalance(ctx, priorYear, asOfDate);

        var etvaBalances =
            _totalService.GetTotalComputedEtva(ctx, profitYear);

        var transactions = _totalService
            .GetTransactionsBySsnForProfitYearForOracle(ctx, profitYear);

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
        // pensionerSsns now passed as parameter instead of queried here

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
        var demographics = await _demographicReaderService.BuildDemographicQueryAsync(ctx);

        var pensionerSsns = from ei in ctx.ExcludedIds
                            where ei.ExcludedIdTypeId == ExcludedIdType.Constants.QPay066TAExclusions
                            select ei.ExcludedIdValue;

        var query =
            from d in demographics.Include(x => x.Address)

            join pp in ctx.PayProfits on d.Id equals pp.DemographicId

            join b in balances on d.Ssn equals b.Ssn into balGrp
            from bal in balGrp.DefaultIfEmpty()

            join e in etvaBalances on d.Ssn equals e.Ssn into etvaGrp
            from etva in etvaGrp.DefaultIfEmpty()

            join lyB in lastYearBalances on d.Ssn equals lyB.Ssn into lyBalGrp
            from lyBal in lyBalGrp.DefaultIfEmpty()

            join t in transactions on d.Ssn equals t.Ssn into txnsGrp
            from txn in txnsGrp.DefaultIfEmpty()

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
                                     + (etva == null ? 0 : etva.TotalAmount)) <= 0))
                                    ? 801
                                    : (d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                                       (d.PayFrequencyId == PayFrequency.Constants.Weekly ||
                                        d.PayFrequencyId == PayFrequency.Constants.Monthly) &&
                                       ((bal == null ? 0 : bal.CurrentBalance) > 0) &&
                                       ((bal == null ? 0 : bal.VestedBalance) == 0) &&
                                       ((etva == null ? 0 : etva.TotalAmount) == 0))
                                        ? 802
                                        : ((d.PayFrequencyId == PayFrequency.Constants.Weekly ||
                                            d.PayFrequencyId == PayFrequency.Constants.Monthly) &&
                                           d.EmploymentStatusId != EmploymentStatus.Constants.Active)
                                            ? 800
                                            : d.StoreNumber),
                /* ── sort column used for certificates ───────────────────────── */
                CertificateSort = d.EmploymentStatusId ==
                        //Active
                        EmploymentStatus.Constants.Active || d.TerminationDate > fiscalEndDate ?
                     (d.DepartmentId == Department.Constants.Grocery && d.PayClassificationId == PayClassification.Constants.Manager ? 10
                      : d.DepartmentId == Department.Constants.Grocery && d.PayClassificationId == PayClassification.Constants.AssistantManager ? 20
                      : d.DepartmentId == Department.Constants.Grocery && d.PayClassificationId == PayClassification.Constants.Merchandiser ? 30
                      : d.DepartmentId == Department.Constants.Grocery && d.PayClassificationId == PayClassification.Constants.FrontEndManager ? 40
                      : d.DepartmentId == Department.Constants.Grocery && d.PayClassificationId == PayClassification.Constants.GroceryManager ? 50
                      : d.DepartmentId == Department.Constants.Meat && d.PayClassificationId == PayClassification.Constants.Manager ? 60
                      : d.DepartmentId == Department.Constants.Meat && d.PayClassificationId == PayClassification.Constants.AssistantManager ? 70
                      : d.DepartmentId == Department.Constants.Deli && d.PayClassificationId == PayClassification.Constants.Manager ? 80
                      : d.DepartmentId == Department.Constants.Produce && d.PayClassificationId == PayClassification.Constants.Manager ? 90
                      : d.DepartmentId == Department.Constants.Dairy && d.PayClassificationId == PayClassification.Constants.Manager ? 100
                      : d.DepartmentId == Department.Constants.Bakery && d.PayClassificationId == PayClassification.Constants.Manager ? 110
                      : d.DepartmentId == Department.Constants.BeerAndWine && d.PayClassificationId == PayClassification.Constants.Manager ? 120
                      : 1999
                     )
                      //Terminated
                      : (d.EmploymentStatusId == EmploymentStatus.Constants.Terminated && d.TerminationDate <= fiscalEndDate) ?
                     (
                        10
                     )
                      //Inactive
                      : d.EmploymentStatusId == EmploymentStatus.Constants.Inactive ?
                     (
                        d.TerminationCodeId == TerminationCode.Constants.WorkmansCompensation ? 1 :
                        d.TerminationCodeId == TerminationCode.Constants.Transferred ? 2 :
                        d.TerminationCodeId == TerminationCode.Constants.FmlaApproved ? 3 :
                        d.TerminationCodeId == TerminationCode.Constants.PersonalOrFamilyReason ? 4 :
                        d.TerminationCodeId == TerminationCode.Constants.HealthReasonsNonFmla ? 5 :
                        d.TerminationCodeId == TerminationCode.Constants.Military ? 6 :
                        d.TerminationCodeId == TerminationCode.Constants.SchoolOrSports ? 7 :
                        d.TerminationCodeId == TerminationCode.Constants.OffForSummer ? 8 :
                        d.TerminationCodeId == TerminationCode.Constants.Injured ? 9 :
                        11
                     ) : 0,
                /* ── plain columns ───────────────────────────────────────────── */
                FullName = d.ContactInfo.FullName ?? string.Empty,
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
                EtvaBalance = etva == null ? 0 : etva.TotalAmount,
                YearsInPlan = bal == null ? 0 : bal.YearsInPlan,
                HireDate = d.HireDate,
                TerminationDate = d.TerminationDate,
                EnrollmentId = pp == null || pp.VestingScheduleId == 0
                    ? EnrollmentConstants.NotEnrolled
                    : pp.HasForfeited
                        ? pp.VestingScheduleId == VestingSchedule.Constants.OldPlan
                            ? EnrollmentConstants.OldVestingPlanHasForfeitureRecords
                            : EnrollmentConstants.NewVestingPlanHasForfeitureRecords
                        : pp.VestingScheduleId == VestingSchedule.Constants.OldPlan
                            ? EnrollmentConstants.OldVestingPlanHasContributions
                            : EnrollmentConstants.NewVestingPlanHasContributions,
                ProfitShareHours = pp.TotalHours,
                Street1 = d.Address.Street,
                City = d.Address.City,
                State = d.Address.State,
                PostalCode = d.Address.PostalCode,
                BeginningBalance = lyBal == null ? 0 : lyBal.CurrentBalance,
                Earnings = txn == null ? 0 : txn.TotalEarnings,
                Distributions = txn == null ? 0 : txn.Distribution,
                Contributions = txn == null ? 0 : txn.TotalContributions,
                Forfeitures = txn == null ? 0 : txn.TotalForfeitures
                ,
                BeneficiaryAllocation = txn == null ? 0 : txn.BeneficiaryAllocation
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

        var validator = new InlineValidator<short>();
        // Inline async rule to prevent running when duplicate SSNs exist.
        validator.RuleFor(r => r)
            .MustAsync(async (_, ct) => !await _duplicateSsnReportService.DuplicateSsnExistsAsync(ct))
            .WithMessage("There are presently duplicate SSN's in the system, which will cause this process to fail.");

        // Dictionaries ----------------------------------------------------------------
        var vestingRatios = await _totalService.GetVestingRatio(ctx, profitYear, calInfo.FiscalEndDate)
            .Where(vr => employeeSsns.Contains(vr.Ssn))
            .ToListAsync(ct);
        var vestingBySsn = vestingRatios.ToLookup(vr => vr.Ssn);

        var priorBalances = await _totalService
            .GetTotalBalanceSet(ctx, priorYear)
            .Where(tbs => employeeSsns.Contains(tbs.Ssn))
            .ToListAsync(ct);
        var balanceBySsnLastYear = priorBalances.ToLookup(tbs => tbs.Ssn);


        var txns = await _totalService
            .GetTransactionsBySsnForProfitYearForOracle(ctx, profitYear)
            .Where(txn => employeeSsns.Contains(txn.Ssn))
            .ToListAsync(ct);
        var txnsBySsn = txns.ToLookup(txn => txn.Ssn);

        // Snapshots -------------------------------------------------------------------
        var demographics = await _demographicReaderService.BuildDemographicQueryAsync(ctx);
        var badgeNumberBySsn = await demographics
            .Where(d => employeeSsns.Contains(d.Ssn))
            .Select(d => new { d.Ssn, d.BadgeNumber })
            .ToListAsync(ct);
        var badgeNumberLookup = badgeNumberBySsn.ToLookup(d => d.Ssn);

        return employeeSsns.Select(ssn => new EmployeeFinancialSnapshot(
            ssn,
            badgeNumberLookup[ssn].FirstOrDefault()?.BadgeNumber ?? 0,
            balanceBySsnLastYear[ssn].FirstOrDefault()?.TotalAmount ?? 0m,
            txnsBySsn[ssn].FirstOrDefault() ?? new ProfitDetailRollup(),
            vestingBySsn[ssn].FirstOrDefault()?.Ratio ?? 0m)).ToList();
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
        snap ??= new EmployeeFinancialSnapshot(member.Ssn, member.BadgeNumber, 0, new ProfitDetailRollup(), 0);

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
            EndingBalance = member.CurrentBalance ?? 0,
            BeneficiaryAllocation = snap.Txn.BeneficiaryAllocation,
            VestedAmount = member.VestedBalance ?? 0,
            VestedPercent = (byte)(member.VestedPercent ?? 0),
            PayClassificationId = member.PayClassificationId,
            PayClassificationName = member.PayClassificationName,
            HireDate = member.HireDate,
            TerminationDate = member.TerminationDate,
            DateOfBirth = member.DateOfBirth,
            ProfitShareHours = member.ProfitShareHours,
            EnrollmentId = member.EnrollmentId,
            Street1 = member.Street1,
            City = member.City,
            State = member.State,
            PostalCode = member.PostalCode,
            CertificateSort = member.CertificateSort,
            IsExecutive = member.PayFrequencyId == PayFrequency.Constants.Monthly
        };
    }

    #endregion
}
