using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Entities.Virtual;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
///     Generates reports for terminated employees and their beneficiaries.
/// </summary>
public sealed class TerminatedEmployeeReportService
{
    private readonly ICalendarService _calendarService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly ILogger<TerminatedEmployeeReportService> _logger;
    private readonly TotalService _totalService;
    private readonly IYearEndService _yearEndService;

    public TerminatedEmployeeReportService(IProfitSharingDataContextFactory factory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        ILogger<TerminatedEmployeeReportService> logger,
        ICalendarService calendarService,
        IYearEndService yearEndService)
    {
        _factory = factory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
        _calendarService = calendarService;
        _yearEndService = yearEndService;
    }

    public async Task<TerminatedEmployeeAndBeneficiaryResponse> CreateDataAsync(FilterableStartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("CreateDataAsync started for date range {BeginningDate} to {EndingDate}", req.BeginningDate, req.EndingDate);

        return await _factory.UseReadOnlyContext(async ctx =>
        {
            var retrieveStartTime = DateTime.UtcNow;
            IQueryable<MemberSlice> memberSliceQuery = await RetrieveMemberSlices(ctx, req);
            var retrieveDuration = (DateTime.UtcNow - retrieveStartTime).TotalMilliseconds;


            var mergeStartTime = DateTime.UtcNow;
            var result = await MergeAndCreateDataset(ctx, req, memberSliceQuery, cancellationToken);
            var mergeDuration = (DateTime.UtcNow - mergeStartTime).TotalMilliseconds;
            int recordCount = result.Response?.Results != null ? result.Response.Results.Count() : 0;
            _logger.LogInformation("MergeAndCreateDataset completed in {DurationMs:F2}ms, processed {RecordCount} records", mergeDuration, recordCount);

            var totalDuration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation("CreateDataAsync completed in {DurationMs:F2}ms (retrieve: {RetrieveDurationMs:F2}ms, merge: {MergeDurationMs:F2}ms)", totalDuration, retrieveDuration, mergeDuration);

            return result;
        }, cancellationToken);
    }

    #region Report Dataset Creation

    /// <summary>
    ///     Merges member slices with transaction and balance data to create the final report dataset.
    ///     Orchestrates the loading of profit details, balances, and the construction of year detail records.
    /// </summary>
    private async Task<TerminatedEmployeeAndBeneficiaryResponse> MergeAndCreateDataset(
        ProfitSharingReadOnlyDbContext ctx,
        FilterableStartAndEndDateRequest req,
        IQueryable<MemberSlice> memberSliceUnion,
        CancellationToken cancellationToken)
    {
        var overallStart = DateTime.UtcNow;


        // Initialize report totals
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        (short beginProfitYear, short endProfitYear) profitYearRange = GetProfitYearRange(req);
        IQueryable<int> ssns = memberSliceUnion.Select(ms => ms.Ssn);

        // COBOL Transaction Year Boundary: Does NOT process transactions after the entered year
        // This is the YDATE in Cobol.  in December it is the current year, in January it is the previous year.
        var yearEnd = await _yearEndService.GetCompletedYearEnd(cancellationToken);
        // We presume we are here working on the not yet complted year end.   Which is the next year end
        // after the last completed one.    So we add 1 year to the completed year end.
        int transactionYearBoundary = yearEnd + 1;

        // Load profit detail transactions
        var profitDetailsStart = DateTime.UtcNow;
        IQueryable<ProfitDetail> profitDetailsRaw = ctx.ProfitDetails
            .Where(pd => pd.ProfitYear >= profitYearRange.beginProfitYear
                         && pd.ProfitYear <= profitYearRange.endProfitYear
                         && pd.ProfitYear <= transactionYearBoundary
                         && ssns.Contains(pd.Ssn));

        var profitDetailsDict = await profitDetailsRaw
            .GroupBy(pd => new { pd.Ssn, pd.ProfitYear })
            .Select(g => new InternalProfitDetailDto
            {
                Ssn = g.Key.Ssn,
                ProfitYear = g.Key.ProfitYear,
                TotalContributions = g.Sum(x => x.Contribution),
                TotalEarnings = g.Sum(x => x.Earnings),
                TotalForfeitures = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                    ? x.Forfeiture
                    : x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id
                        ? -x.Forfeiture
                        : 0),
                TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0),
                Distribution = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id ||
                                          x.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id ||
                                          x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id
                    ? -x.Forfeiture
                    : 0),
                BeneficiaryAllocation = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
                    ? -x.Forfeiture
                    : x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id
                        ? x.Contribution
                        : 0),
                CurrentAmount = g.Sum(x => x.Contribution + x.Earnings +
                                           (x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0) -
                                           (x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0))
            })
            .ToDictionaryAsync(x => new { x.Ssn, x.ProfitYear }, cancellationToken);
        var profitDetailsDuration = (DateTime.UtcNow - profitDetailsStart).TotalMilliseconds;
        _logger.LogInformation("ProfitDetails dictionary loaded in {DurationMs:F2}ms with {RecordCount} entries", profitDetailsDuration, profitDetailsDict.Count);

        // This report is always giving values about today for the members current status.
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        // The "Begin" values always refer to the prior completed year.  "Begin" is the "end" of the last completed YE.
        short lastCompletedYearEnd = await _yearEndService.GetCompletedYearEnd(cancellationToken);
        CalendarResponseDto priorYearDateRange = await _calendarService.GetYearStartAndEndAccountingDatesAsync(lastCompletedYearEnd, cancellationToken);

        var thisYearStart = DateTime.UtcNow;
        Dictionary<(int Ssn, int Id), ParticipantTotalVestingBalance> thisYearBalancesDict = await _totalService
            .TotalVestingBalance(ctx, profitYearRange.beginProfitYear, profitYearRange.endProfitYear, today)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => (x.Ssn, x.Id), x => x, cancellationToken);
        var thisYearDuration = (DateTime.UtcNow - thisYearStart).TotalMilliseconds;
        _logger.LogInformation("ThisYearBalances dictionary loaded in {DurationMs:F2}ms with {RecordCount} entries", thisYearDuration, thisYearBalancesDict.Count);

        var lastYearStart = DateTime.UtcNow;
        Dictionary<int, ParticipantTotal> lastYearBalancesDict = await _totalService.GetTotalBalanceSet(ctx, lastCompletedYearEnd)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => x.Ssn, x => x, cancellationToken);
        var lastYearDuration = (DateTime.UtcNow - lastYearStart).TotalMilliseconds;
        _logger.LogInformation("LastYearBalances dictionary loaded in {DurationMs:F2}ms with {RecordCount} entries", lastYearDuration, lastYearBalancesDict.Count);

        var vestedStart = DateTime.UtcNow;
        Dictionary<(int Ssn, int Id), ParticipantTotalVestingBalance> lastYearVestedBalancesDict = await _totalService
            .TotalVestingBalance(ctx, /*PayProfit Year*/lastCompletedYearEnd, /*Desired Year*/lastCompletedYearEnd, priorYearDateRange.FiscalEndDate)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => (x.Ssn, x.Id), x => x, cancellationToken);
        var vestedDuration = (DateTime.UtcNow - vestedStart).TotalMilliseconds;
        _logger.LogInformation("LastYearVestedBalances dictionary loaded in {DurationMs:F2}ms with {RecordCount} entries", vestedDuration, lastYearVestedBalancesDict.Count);

        // Build year details list for each member
        var processingStart = DateTime.UtcNow;
        List<(int BadgeNumber, short PsnSuffix, string? Name, TerminatedEmployeeAndBeneficiaryYearDetailDto YearDetail)> yearDetailsList = new();

        // Deduplication: Process employees first, then beneficiaries; skip beneficiaries if employee already processed
        // This prevents duplicates when a person appears as both terminated employee and beneficiary
        var seenMembers = new HashSet<(int BadgeNumber, short PsnSuffix)>();

        var memberSliceCollection = memberSliceUnion
            .Where(m => !(m.YearsInPs <= 2 && m.HoursCurrentYear >= ReferenceData.MinimumHoursForContribution()))
            .AsAsyncEnumerable();

        await foreach (MemberSlice memberSlice in memberSliceCollection)
        {
            // Skip duplicate: If same (BadgeNumber, PsnSuffix) already processed, skip this record
            // Since union is employees.Concat(beneficiaries), employees are processed first,
            // so beneficiary records for the same person are automatically skipped
            if (!seenMembers.Add((memberSlice.BadgeNumber, memberSlice.PsnSuffix)))
            {
                continue; // Duplicate - already processed as employee
            }

            // Get transactions for this member
            var key = new { memberSlice.Ssn, memberSlice.ProfitYear };
            if (!profitDetailsDict.TryGetValue(key, out InternalProfitDetailDto? transactionsThisYear))
            {
                transactionsThisYear = new InternalProfitDetailDto();
            }

            // Get beginning balance from last year
            decimal? beginningAmount = lastYearBalancesDict.TryGetValue(memberSlice.Ssn, out ParticipantTotal? lastYearBalance)
                ? lastYearBalance.TotalAmount
                : 0m;

            // Get vesting balance and percentage from this year
            // Use compound key (Ssn, Id) to look up the correct record
            ParticipantTotalVestingBalance? thisYearVestedBalance = thisYearBalancesDict.TryGetValue((memberSlice.Ssn, memberSlice.Id), out var thisYearVested)
                ? thisYearVested
                : null;
            decimal vestedBalance = thisYearVestedBalance?.VestedBalance ?? 0m;

            ParticipantTotalVestingBalance? lastYearVestedBalance = lastYearVestedBalancesDict.TryGetValue((memberSlice.Ssn, memberSlice.Id), out var lastYearVested)
                ? lastYearVested
                : null;

            decimal vestedRatio = lastYearVestedBalance?.VestingPercent ?? 0;

            // Create member record with all values
            Member member = new()
            {
                Id = memberSlice.Id,
                BadgeNumber = memberSlice.BadgeNumber,
                ProfitYear = memberSlice.ProfitYear,
                PsnSuffix = memberSlice.PsnSuffix,
                FullName = memberSlice.FullName,
                FirstName = memberSlice.FirstName,
                LastName = memberSlice.LastName,
                Birthday = memberSlice.BirthDate,
                HoursCurrentYear = memberSlice.HoursCurrentYear,
                EarningsCurrentYear = memberSlice.IncomeRegAndExecCurrentYear,
                Ssn = memberSlice.Ssn,
                TerminationDate = memberSlice.TerminationDate,
                TerminationCode = memberSlice.TerminationCode,
                BeginningAmount = beginningAmount ?? 0,
                YearsInPlan = memberSlice.YearsInPs,
                ZeroCont = memberSlice.ZeroCont,
                EnrollmentId = memberSlice.EnrollmentId,
                IsExecutive = memberSlice.IsExecutive,
                Evta = memberSlice.Etva,
                BeneficiaryAllocation = transactionsThisYear.BeneficiaryAllocation,
                DistributionAmount = transactionsThisYear.Distribution,
                ForfeitAmount = transactionsThisYear.TotalForfeitures,
                EndingBalance = (beginningAmount ?? 0) + transactionsThisYear.TotalForfeitures + transactionsThisYear.Distribution + transactionsThisYear.BeneficiaryAllocation
            };

            // Apply IsInteresting filter
            if (!IsInteresting(member))
            {
                continue;
            }

            // Apply vesting rules and adjustments
            byte enrollmentId = member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                ? Enrollment.Constants.NotEnrolled
                : member.EnrollmentId;

            // If retired, then they get it all.
            if (member.ZeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
            {
                vestedRatio = 1;
            }

            // If bene, they get it all
            if (memberSlice.IsOnlyBeneficiary)
            {
                vestedRatio = 1;
            }

            // If there is no money, then the ratio is meaningless - follow how READY reports it as 0
            if (member.EndingBalance == 0)
            {
                vestedRatio = 0;
            }

            // Calculate age if birthdate available
            int? age = member.Birthday?.Age();

            var hasForfeited = enrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords ||
                               enrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords;
            var suggestedForfeit = hasForfeited ? (decimal?)0 : Math.Round(member.EndingBalance - vestedBalance, 2, MidpointRounding.AwayFromZero);
            if (member.PsnSuffix > 0)
            {
                // You can not forfeit a pure bene
                suggestedForfeit = null;
            }

            // Create year detail record
            TerminatedEmployeeAndBeneficiaryYearDetailDto yearDetail = new()
            {
                ProfitYear = member.ProfitYear,
                BeginningBalance = member.BeginningAmount,
                BeneficiaryAllocation = member.BeneficiaryAllocation,
                DistributionAmount = member.DistributionAmount,
                Forfeit = member.ForfeitAmount,
                EndingBalance = member.EndingBalance,
                VestedBalance = vestedBalance,
                DateTerm = member.TerminationDate,
                YtdPsHours = member.HoursCurrentYear,
                IsExecutive = member.IsExecutive,
                VestedPercent = vestedRatio * 100,
                Age = age,
                HasForfeited = hasForfeited,
                SuggestedForfeit = suggestedForfeit,
                EnrollmentId = member.EnrollmentId
            };

            // Apply user-requested exclusion filters
            // Exclude members with 0 balance OR 100% vested (no forfeiture opportunity)
            if (req.ExcludeZeroAndFullyVested && (member.EndingBalance == 0 || vestedRatio == 1.0m || hasForfeited))
            {
                continue;
            }

            yearDetailsList.Add((member.BadgeNumber, member.PsnSuffix, member.FullName, yearDetail));

            // Accumulate totals
            totalVested += vestedBalance;
            totalForfeit += member.ForfeitAmount;
            totalEndingBalance += member.EndingBalance;
            totalBeneficiaryAllocation += member.BeneficiaryAllocation;
        }
        var processingDuration = (DateTime.UtcNow - processingStart).TotalMilliseconds;

        var groupingStart = DateTime.UtcNow;
        PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto> grouped = await yearDetailsList
            .GroupBy(x => new { x.BadgeNumber, x.PsnSuffix, x.Name })
            .Select(g => new TerminatedEmployeeAndBeneficiaryDataResponseDto
            {
                PSN = g.Key.PsnSuffix == 0 ? (long)g.Key.BadgeNumber : (long)g.Key.BadgeNumber * 10000 + g.Key.PsnSuffix,
                Name = g.Key.Name,
                YearDetails = g.Select(x => x.YearDetail).OrderByDescending(y => y.ProfitYear).ToList()
            }).ToPaginationResultsAsync(req, cancellationToken);

        var groupingDuration = (DateTime.UtcNow - groupingStart).TotalMilliseconds;
        int groupCount = grouped.Results != null ? grouped.Results.Count() : 0;
        _logger.LogInformation("Grouping and pagination completed in {DurationMs:F2}ms, returned {GroupCount} groups", groupingDuration, groupCount);

        var totalProcessingDuration = (DateTime.UtcNow - overallStart).TotalMilliseconds;
        _logger.LogInformation("MergeAndCreateDataset complete: total {TotalDurationMs:F2}ms (queries: {QueryDurationMs:F2}ms, processing: {ProcessingDurationMs:F2}ms, grouping: {GroupingDurationMs:F2}ms)",
            totalProcessingDuration, profitDetailsDuration + thisYearDuration + lastYearDuration + vestedDuration, processingDuration, groupingDuration);

        return new TerminatedEmployeeAndBeneficiaryResponse
        {
            ReportName = "Terminated Employees",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = req.BeginningDate,
            EndDate = req.EndingDate,
            TotalVested = totalVested,
            TotalForfeit = totalForfeit,
            TotalEndingBalance = totalEndingBalance,
            TotalBeneficiaryAllocation = totalBeneficiaryAllocation,
            Response = grouped
        };
    }

    #endregion

    #region IsInteresting Filter

    /// <summary>
    ///     Determines if a member should be included in the report.
    ///     Based on COBOL QPAY066 filtering logic (lines 1060-1090).
    ///     Filters members based on balance and transaction activity.
    /// </summary>
    /// <param name="member">The member to evaluate for inclusion</param>
    /// <returns>True if the member should be included in the report</returns>
    private static bool IsInteresting(Member member)
    {
        // If you are not past your second year, you have no money in profit sharing so lets move along
        if (member.YearsInPlan <= 2 && member.HoursCurrentYear >= /*1000*/ ReferenceData.MinimumHoursForContribution())
        {
            return false;
        }

        // Beginning balance (most important filter)
        if (member.BeginningAmount != 0)
        {
            return true;
        }

        // Distribution amount
        if (member.DistributionAmount != 0)
        {
            return true;
        }

        // Forfeit amount
        if (member.ForfeitAmount != 0)
        {
            return true;
        }

        // Beneficiary allocation (always included)
        if (member.BeneficiaryAllocation != 0)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Member Data Retrieval

    /// <summary>
    ///     Extracts the profit year range from the request date range.
    /// </summary>
    private (short beginProfitYear, short endProfitYear) GetProfitYearRange(FilterableStartAndEndDateRequest request)
    {
        return ((short)request.BeginningDate.Year, (short)request.EndingDate.Year);
    }

    /// <summary>
    ///     Retrieves all member slices (terminated employees + beneficiaries) for the requested date range.
    ///     Coordinates the loading of terminated employees and beneficiaries, then combines them.
    ///     Returns an IQueryable to defer materialization until needed.
    /// </summary>
    private async Task<IQueryable<MemberSlice>> RetrieveMemberSlices(
        IProfitSharingDbContext ctx,
        FilterableStartAndEndDateRequest request)
    {
        _logger.LogInformation(
            "Starting retrieval for date range {BeginningDate} to {EndingDate}",
            request.BeginningDate, request.EndingDate);

        var terminatedEmployees = await GetTerminatedEmployeesSync(ctx, request);
        var terminatedWithContributions = GetEmployeesAsMembers(ctx, request, terminatedEmployees, request.EndingDate);
        var beneficiaries = await GetBeneficiariesSync(ctx, request);

        return CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries);
    }

    /// <summary>
    ///     Queries terminated employees within the specified date range.
    ///     Excludes retirees receiving pension (matching READY COBOL business rules).
    /// </summary>
    private async Task<IQueryable<TerminatedEmployeeDto>> GetTerminatedEmployeesSync(
        IProfitSharingDbContext ctx,
        FilterableStartAndEndDateRequest request)
    {
        IQueryable<Demographic> demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

        // BUSINESS RULE: Get employees who might have profit sharing activity.
        // READY includes employees based on activity rather than just HR termination status.
        // Excludes retirees receiving pension (READY: PY_TERM != 'W').
        IQueryable<TerminatedEmployeeDto> queryable = demographics
            .Include(d => d.ContactInfo)
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                        && (d.TerminationCodeId == null || d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension)
                        && d.TerminationDate != null
                        && d.TerminationDate >= request.BeginningDate
                        && d.TerminationDate <= request.EndingDate)
            .Select(d => new TerminatedEmployeeDto { Demographic = d });

        return queryable;
    }

    /// <summary>
    ///     Queries terminated employees within the specified date range.
    ///     Excludes retirees receiving pension (matching READY COBOL business rules).
    /// </summary>
    private async Task<IQueryable<TerminatedEmployeeDto>> GetTerminatedEmployees(
        IProfitSharingDbContext ctx,
        FilterableStartAndEndDateRequest request)
    {
        var start = DateTime.UtcNow;
        IQueryable<Demographic> demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

        // BUSINESS RULE: Get employees who might have profit sharing activity.
        // READY includes employees based on activity rather than just HR termination status.
        // Excludes retirees receiving pension (READY: PY_TERM != 'W').
        IQueryable<TerminatedEmployeeDto> queryable = demographics
            .Include(d => d.ContactInfo)
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                        && (d.TerminationCodeId == null || d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension)
                        && d.TerminationDate != null
                        && d.TerminationDate >= request.BeginningDate
                        && d.TerminationDate <= request.EndingDate)
            .Select(d => new TerminatedEmployeeDto { Demographic = d });

        var duration = (DateTime.UtcNow - start).TotalMilliseconds;
        _logger.LogDebug("GetTerminatedEmployees query building completed in {DurationMs:F2}ms", duration);

        return queryable;
    }

    /// <summary>
    ///     Transforms terminated employees into MemberSlice records with profit sharing data.
    ///     Uses LEFT JOIN with PayProfit to include employees without current year records.
    /// </summary>
    private IQueryable<MemberSlice> GetEmployeesAsMembers(
        IProfitSharingDbContext ctx,
        FilterableStartAndEndDateRequest request,
        IQueryable<TerminatedEmployeeDto> terminatedEmployees,
        DateOnly asOfDate)
    {
        // CRITICAL: COBOL BUSINESS LOGIC ALIGNMENT (QPAY066 lines 604-634)
        // COBOL queries PAYPROFIT WITHOUT year filter: "WHERE PAYPROF_BADGE = :H-DEM-BADGE"
        // In READY's snapshot model, PayProfit has ONE record per employee.
        // In SMART's temporal model, PayProfit has MULTIPLE records (one per year).
        // Must get PayProfit for REQUESTED YEAR ONLY to match COBOL's "current year" behavior.
        short requestedYear = (short)request.EndingDate.Year;

        IQueryable<MemberSlice> query = from employee in terminatedEmployees
                                        join payProfit in ctx.PayProfits.Where(pp => pp.ProfitYear == requestedYear)
                                            on employee.Demographic.Id equals payProfit.DemographicId into payProfitTmp
                                        from payProfit in payProfitTmp.DefaultIfEmpty()
                                        join yipTbl in _totalService.GetYearsOfService(ctx, requestedYear, asOfDate)
                                            on employee.Demographic.Ssn equals yipTbl.Ssn into yipTmp
                                        from yip in yipTmp.DefaultIfEmpty()
                                        select new MemberSlice
                                        {
                                            Id = employee.Demographic.Id,
                                            PsnSuffix = 0,
                                            BadgeNumber = employee.Demographic.BadgeNumber,
                                            Ssn = employee.Demographic.Ssn,
                                            BirthDate = employee.Demographic.DateOfBirth,
                                            HoursCurrentYear = payProfit != null ? payProfit.CurrentHoursYear : 0,
                                            EmploymentStatusCode = employee.Demographic.EmploymentStatusId,
                                            FullName = employee.Demographic.ContactInfo.FullName,
                                            FirstName = employee.Demographic.ContactInfo.FirstName,
                                            LastName = employee.Demographic.ContactInfo.LastName,
                                            YearsInPs = yip != null ? yip.Years : (byte)0,
                                            TerminationDate = employee.Demographic.TerminationDate,
                                            IncomeRegAndExecCurrentYear = payProfit != null ? payProfit.CurrentIncomeYear + payProfit.IncomeExecutive : 0,
                                            TerminationCode = employee.Demographic.TerminationCodeId,
                                            ZeroCont = employee.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                                                ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                                                : payProfit != null && payProfit.ZeroContributionReasonId != null
                                                    ? payProfit.ZeroContributionReasonId
                                                    : 0,
                                            EnrollmentId = payProfit != null ? payProfit.EnrollmentId : (byte)0,
                                            Etva = payProfit != null ? payProfit.Etva : 0,
                                            ProfitYear = requestedYear,
                                            IsOnlyBeneficiary = false,
                                            IsBeneficiaryAndEmployee = false,
                                            IsExecutive = employee.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly
                                        };

        return query;
    }

    /// <summary>
    ///     Retrieves beneficiary records for deceased employees (synchronous query building).
    ///     Implements COBOL QPAY066 beneficiary logic with PSN suffix handling.
    /// </summary>
    private async Task<IQueryable<MemberSlice>> GetBeneficiariesSync(
        IProfitSharingDbContext ctx,
        FilterableStartAndEndDateRequest request)
    {
        _logger.LogDebug(
            "Loading beneficiaries for date range {BeginningDate} to {EndingDate}",
            request.BeginningDate, request.EndingDate);

        IQueryable<Demographic> demographicsQuery = await _demographicReaderService.BuildDemographicQuery(ctx);

        // Load beneficiaries and their related employee demographics
        IQueryable<MemberSlice> query = ctx.Beneficiaries
            .Include(b => b.Contact)
            .ThenInclude(c => c!.ContactInfo)
            .GroupJoin(
                demographicsQuery,
                b => b.Contact!.Ssn,
                d => d.Ssn,
                (b, ds) => new { b, ds })
            .SelectMany(
                x => x.ds.DefaultIfEmpty(),
                (x, d) =>
                    new MemberSlice
                    {
                        // COBOL Lines 775-782: When beneficiary matches demographics AND termination date is NOT in range,
                        // use badge number with PSN=0 (appears as primary employee), otherwise use PSN suffix
                        Id = d == null ? x.b.Id : d.Id,
                        PsnSuffix = d == null ? x.b.PsnSuffix : (short)0,
                        BadgeNumber = d == null ? x.b.BadgeNumber : d.BadgeNumber,
                        Ssn = x.b.Contact!.Ssn,
                        BirthDate = x.b.Contact!.DateOfBirth,
                        HoursCurrentYear = 0,
                        EmploymentStatusCode = '\0',
                        FullName = x.b.Contact!.ContactInfo.FullName!,
                        FirstName = x.b.Contact.ContactInfo.FirstName,
                        LastName = x.b.Contact.ContactInfo.LastName,
                        YearsInPs = 10, // Convention to make IsInteresting() always return true (matches READY)
                        TerminationDate = d == null ? null : d.TerminationDate,
                        IncomeRegAndExecCurrentYear = 0,
                        TerminationCode = d == null ? null : d.TerminationCodeId,
                        ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                        EnrollmentId = 0,
                        Etva = 0,
                        // CRITICAL: Beneficiaries must use the requested profit year to match transaction lookups
                        ProfitYear = (short)request.EndingDate.Year,
                        IsOnlyBeneficiary = d == null,
                        IsBeneficiaryAndEmployee = d != null,
                        IsExecutive = false
                    }
            );

        return query;
    }

    /// <summary>
    ///     Retrieves beneficiary records for deceased employees.
    ///     Implements COBOL QPAY066 beneficiary logic with PSN suffix handling.
    /// </summary>
    private async Task<IQueryable<MemberSlice>> GetBeneficiaries(
        IProfitSharingDbContext ctx,
        FilterableStartAndEndDateRequest request)
    {
        var start = DateTime.UtcNow;
        _logger.LogDebug(
            "Loading beneficiaries for date range {BeginningDate} to {EndingDate}",
            request.BeginningDate, request.EndingDate);

        IQueryable<Demographic> demographicsQuery = await _demographicReaderService.BuildDemographicQuery(ctx);

        // Load beneficiaries and their related employee demographics
        IQueryable<MemberSlice> query = ctx.Beneficiaries
            .Include(b => b.Contact)
            .ThenInclude(c => c!.ContactInfo)
            .GroupJoin(
                demographicsQuery,
                b => b.Contact!.Ssn,
                d => d.Ssn,
                (b, ds) => new { b, ds })
            .SelectMany(
                x => x.ds.DefaultIfEmpty(),
                (x, d) =>
                    new MemberSlice
                    {
                        // COBOL Lines 775-782: When beneficiary matches demographics AND termination date is NOT in range,
                        // use badge number with PSN=0 (appears as primary employee), otherwise use PSN suffix
                        Id = d == null ? x.b.Id : d.Id,
                        PsnSuffix = d == null ? x.b.PsnSuffix : (short)0,
                        BadgeNumber = d == null ? x.b.BadgeNumber : d.BadgeNumber,
                        Ssn = x.b.Contact!.Ssn,
                        BirthDate = x.b.Contact!.DateOfBirth,
                        HoursCurrentYear = 0,
                        EmploymentStatusCode = '\0',
                        FullName = x.b.Contact!.ContactInfo.FullName!,
                        FirstName = x.b.Contact.ContactInfo.FirstName,
                        LastName = x.b.Contact.ContactInfo.LastName,
                        YearsInPs = 10, // Convention to make IsInteresting() always return true (matches READY)
                        TerminationDate = d == null ? null : d.TerminationDate,
                        IncomeRegAndExecCurrentYear = 0,
                        TerminationCode = d == null ? null : d.TerminationCodeId,
                        ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                        EnrollmentId = 0,
                        Etva = 0,
                        // CRITICAL: Beneficiaries must use the requested profit year to match transaction lookups
                        ProfitYear = (short)request.EndingDate.Year,
                        IsOnlyBeneficiary = d == null,
                        IsBeneficiaryAndEmployee = d != null,
                        IsExecutive = false
                    }
            );

        var duration = (DateTime.UtcNow - start).TotalMilliseconds;
        _logger.LogDebug("GetBeneficiaries query building completed in {DurationMs:F2}ms", duration);

        return query;
    }

    /// <summary>
    ///     Combines employee and beneficiary slices, filtering for eligibility and removing duplicates.
    ///     Prioritizes employee records over beneficiary records for the same person.
    ///     Returns an IQueryable to defer materialization until the data is actually needed.
    /// </summary>
    private IQueryable<MemberSlice> CombineEmployeeAndBeneficiarySlices(
        IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries)
    {
        // Filter employees based on enrollment and years in plan
        IQueryable<MemberSlice> employees = terminatedWithContributions.Where(member =>
            (member.EnrollmentId == Enrollment.Constants.NotEnrolled ||
             member.EnrollmentId == Enrollment.Constants.OldVestingPlanHasContributions ||
             member.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
            && member.YearsInPs > 2
            ||
            (member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions ||
             member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords)
            && member.YearsInPs > 1);

        // Combine: all employees + beneficiaries (without deduplication at query level)
        // Deduplication by SSN happens via GroupBy in the caller (RetrieveMemberSlices)
        // This keeps the query composable and defers materialization
        return employees.Concat(beneficiaries);
    }

    #endregion
}
