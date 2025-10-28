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

    public Task<TerminatedEmployeeAndBeneficiaryResponse> CreateDataAsync(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        return _factory.UseReadOnlyContext(async ctx =>
        {
            List<MemberSlice> memberSliceUnion = await RetrieveMemberSlices(ctx, req, cancellationToken);
            return await MergeAndCreateDataset(ctx, req, memberSliceUnion, cancellationToken);
        }, cancellationToken);
    }

    #region Report Dataset Creation

    /// <summary>
    ///     Merges member slices with transaction and balance data to create the final report dataset.
    ///     Orchestrates the loading of profit details, balances, and the construction of year detail records.
    /// </summary>
    private async Task<TerminatedEmployeeAndBeneficiaryResponse> MergeAndCreateDataset(
        ProfitSharingReadOnlyDbContext ctx,
        StartAndEndDateRequest req,
        List<MemberSlice> memberSliceUnion,
        CancellationToken cancellationToken)
    {
        // Initialize report totals
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        (short beginProfitYear, short endProfitYear) profitYearRange = GetProfitYearRange(req);
        HashSet<int> ssns = memberSliceUnion.Select(ms => ms.Ssn).ToHashSet();

        // COBOL Transaction Year Boundary: Does NOT process transactions after the entered year
        int transactionYearBoundary = req.EndingDate.Year;

        // Load profit detail transactions
        IQueryable<ProfitDetail> profitDetailsRaw = ctx.ProfitDetails
            .Where(pd => pd.ProfitYear >= profitYearRange.beginProfitYear
                         && pd.ProfitYear <= profitYearRange.endProfitYear
                         && pd.ProfitYear <= transactionYearBoundary
                         && ssns.Contains(pd.Ssn));

        var profitDetailsDict = await profitDetailsRaw
            .GroupBy(pd => new { pd.Ssn, pd.ProfitYear })
            .ToDictionaryAsync(g => g.Key, g => new InternalProfitDetailDto
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
            }, cancellationToken);

        // This report is always giving values about today for the members current status.
        DateOnly today = DateOnly.FromDateTime(DateTime.Today); 
        // The "Begin" values always refer to the prior completed year.  "Begin" is the "end" of the last completed YE.
        short lastCompletedYearEnd = await _yearEndService.GetCompletedYearEnd(cancellationToken);
        CalendarResponseDto priorYearDateRange = await _calendarService.GetYearStartAndEndAccountingDatesAsync(lastCompletedYearEnd, cancellationToken);

        Dictionary<int, ParticipantTotalVestingBalance> thisYearBalancesDict = await _totalService
            .TotalVestingBalance(ctx, profitYearRange.beginProfitYear, profitYearRange.endProfitYear, today)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => x.Ssn, x => x, cancellationToken);

        Dictionary<int, ParticipantTotal> lastYearBalancesDict = await _totalService.GetTotalBalanceSet(ctx, lastCompletedYearEnd)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => x.Ssn, x => x, cancellationToken);

        Dictionary<int, ParticipantTotalVestingBalance> lastYearVestedBalancesDict = await _totalService
            .TotalVestingBalance(ctx, /*PayProfit Year*/lastCompletedYearEnd, /*Desired Year*/lastCompletedYearEnd, priorYearDateRange.FiscalEndDate)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => x.Ssn, x => x, cancellationToken);

        // Build year details list for each member
        List<(int BadgeNumber, short PsnSuffix, string? Name, TerminatedEmployeeAndBeneficiaryYearDetailDto YearDetail)> yearDetailsList = new();

        foreach (MemberSlice memberSlice in memberSliceUnion)
        {
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
            ParticipantTotalVestingBalance? thisYearVestedBalance = thisYearBalancesDict.GetValueOrDefault(memberSlice.Ssn);
            decimal vestedBalance = thisYearVestedBalance?.VestedBalance ?? 0m;

            ParticipantTotalVestingBalance? lastYearVestedBalance = lastYearVestedBalancesDict.GetValueOrDefault(memberSlice.Ssn);
            decimal vestedRatio = lastYearVestedBalance?.VestingPercent ?? 0;

            // Create member record with all values
            Member member = new()
            {
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
                HasForfeited = enrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords ||
                               enrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords,
                SuggestedForfeit = member.ProfitYear == req.ProfitYear ? member.EndingBalance - vestedBalance : null,
                EnrollmentId = member.EnrollmentId
            };

            yearDetailsList.Add((member.BadgeNumber, member.PsnSuffix, member.FullName, yearDetail));

            // Accumulate totals
            totalVested += vestedBalance;
            totalForfeit += member.ForfeitAmount;
            totalEndingBalance += member.EndingBalance;
            totalBeneficiaryAllocation += member.BeneficiaryAllocation;
        }

        // Group by BadgeNumber, PsnSuffix, Name and create response
        PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto> grouped = await yearDetailsList
            .GroupBy(x => new { x.BadgeNumber, x.PsnSuffix, x.Name })
            .Select(g => new TerminatedEmployeeAndBeneficiaryDataResponseDto
            {
                BadgeNumber = g.Key.BadgeNumber,
                PsnSuffix = g.Key.PsnSuffix,
                Name = g.Key.Name,
                YearDetails = g.Select(x => x.YearDetail).OrderByDescending(y => y.ProfitYear).ToList()
            }).AsQueryable().ToPaginationResultsAsync(req, cancellationToken);

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
    private (short beginProfitYear, short endProfitYear) GetProfitYearRange(StartAndEndDateRequest request)
    {
        return ((short)request.BeginningDate.Year, (short)request.EndingDate.Year);
    }

    /// <summary>
    ///     Retrieves all member slices (terminated employees + beneficiaries) for the requested date range.
    ///     Coordinates the loading of terminated employees and beneficiaries, then combines them.
    /// </summary>
    private async Task<List<MemberSlice>> RetrieveMemberSlices(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting retrieval for date range {BeginningDate} to {EndingDate}",
            request.BeginningDate, request.EndingDate);

        IQueryable<TerminatedEmployeeDto> terminatedEmployees = await GetTerminatedEmployees(ctx, request);
        IQueryable<MemberSlice> terminatedWithContributions = GetEmployeesAsMembers(ctx, request, terminatedEmployees, request.EndingDate);
        IQueryable<MemberSlice> beneficiaries = await GetBeneficiaries(ctx, request);

        return await CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries, cancellationToken);
    }

    /// <summary>
    ///     Queries terminated employees within the specified date range.
    ///     Excludes retirees receiving pension (matching READY COBOL business rules).
    /// </summary>
    private async Task<IQueryable<TerminatedEmployeeDto>> GetTerminatedEmployees(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request)
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
    ///     Transforms terminated employees into MemberSlice records with profit sharing data.
    ///     Uses LEFT JOIN with PayProfit to include employees without current year records.
    /// </summary>
    private IQueryable<MemberSlice> GetEmployeesAsMembers(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request,
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
    ///     Retrieves beneficiary records for deceased employees.
    ///     Implements COBOL QPAY066 beneficiary logic with PSN suffix handling.
    /// </summary>
    private async Task<IQueryable<MemberSlice>> GetBeneficiaries(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request)
    {
        _logger.LogInformation(
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
    ///     Combines employee and beneficiary slices, filtering for eligibility and removing duplicates.
    ///     Prioritizes employee records over beneficiary records for the same person.
    /// </summary>
    private async Task<List<MemberSlice>> CombineEmployeeAndBeneficiarySlices(
        IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries,
        CancellationToken cancellation)
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

        List<MemberSlice> employeeList = await employees.ToListAsync(cancellation);
        HashSet<int> employeeSsn = employeeList.Select(e => e.Ssn).Distinct().ToHashSet();
        List<MemberSlice> beneficiaryList = await beneficiaries.Where(b => !employeeSsn.Contains(b.Ssn)).ToListAsync(cancellation);

        _logger.LogInformation(
            "Retrieved {EmployeeCount} employees and {BeneficiaryCount} beneficiaries",
            employeeList.Count, beneficiaryList.Count);

        // Combine: all employees + beneficiaries without employee equivalents
        List<MemberSlice> result = employeeList.Concat(beneficiaryList).ToList();

        _logger.LogInformation(
            "Combined result: {TotalCount} members ({EmployeeCount} employees + {BeneficiaryCount} beneficiaries)",
            result.Count, employeeList.Count, beneficiaryList.Count);

        return result;
    }

    #endregion
}
