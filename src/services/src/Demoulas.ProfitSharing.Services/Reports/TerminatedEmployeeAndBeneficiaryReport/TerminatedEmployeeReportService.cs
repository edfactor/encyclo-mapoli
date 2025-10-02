using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Generates reports for terminated employees and their beneficiaries.
/// </summary>
public sealed class TerminatedEmployeeReportService
{
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;
    private readonly ILogger<TerminatedEmployeeReportService> _logger;

    public TerminatedEmployeeReportService(IProfitSharingDataContextFactory factory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService,
        ILogger<TerminatedEmployeeReportService> logger)
    {
        _factory = factory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
        _logger = logger;
    }

    public Task<TerminatedEmployeeAndBeneficiaryResponse> CreateDataAsync(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        return _factory.UseReadOnlyContext(async ctx =>
        {
            List<MemberSlice> memberSliceUnion = await RetrieveMemberSlices(ctx, req, cancellationToken);
            return await MergeAndCreateDataset(ctx, req, memberSliceUnion, cancellationToken);
        });
    }

    #region Member Data Retrieval

    /// <summary>
    /// Extracts the profit year range from the request date range.
    /// </summary>
    private (short beginProfitYear, short endProfitYear) GetProfitYearRange(StartAndEndDateRequest request)
    {
        return ((short)request.BeginningDate.Year, (short)request.EndingDate.Year);
    }

    /// <summary>
    /// Retrieves all member slices (terminated employees + beneficiaries) for the requested date range.
    /// Coordinates the loading of terminated employees and beneficiaries, then combines them.
    /// </summary>
    private async Task<List<MemberSlice>> RetrieveMemberSlices(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting retrieval for date range {BeginningDate} to {EndingDate}",
            request.BeginningDate, request.EndingDate);

        var terminatedEmployees = await GetTerminatedEmployees(ctx, request);
        var terminatedWithContributions = GetEmployeesAsMembers(ctx, request, terminatedEmployees, request.EndingDate);
        var beneficiaries = GetBeneficiaries(ctx, request);

        return await CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries, cancellationToken);
    }

    /// <summary>
    /// Queries terminated employees within the specified date range.
    /// Excludes retirees receiving pension (matching READY COBOL business rules).
    /// </summary>
    private async Task<IQueryable<TerminatedEmployeeDto>> GetTerminatedEmployees(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

        // BUSINESS RULE: Get employees who might have profit sharing activity.
        // READY includes employees based on activity rather than just HR termination status.
        // Excludes retirees receiving pension (READY: PY_TERM != 'W').
        var queryable = demographics
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
    /// Transforms terminated employees into MemberSlice records with profit sharing data.
    /// Uses LEFT JOIN with PayProfit to include employees without current year records.
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
        var requestedYear = (short)request.EndingDate.Year;

        var query = from employee in terminatedEmployees
                    join payProfit in ctx.PayProfits.Where(pp => pp.ProfitYear == requestedYear)
                        on employee.Demographic.Id equals payProfit.DemographicId into payProfitTmp
                    from payProfit in payProfitTmp.DefaultIfEmpty()
                    join yipTbl in _totalService.GetYearsOfService(ctx, requestedYear, asOfDate)
                        on employee.Demographic.Ssn equals yipTbl.Ssn into yipTmp
                    from yip in yipTmp.DefaultIfEmpty()
                    select new MemberSlice
                    {
                        PsnSuffix = 0,
                        Id = employee.Demographic.Id,
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
                        IncomeRegAndExecCurrentYear = payProfit != null ? (payProfit.CurrentIncomeYear + payProfit.IncomeExecutive) : 0,
                        TerminationCode = employee.Demographic.TerminationCodeId,
                        ZeroCont = employee.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                            ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                            : (payProfit != null && payProfit.ZeroContributionReasonId != null ? payProfit.ZeroContributionReasonId : 0),
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
    /// Retrieves beneficiary records for deceased employees.
    /// Implements COBOL QPAY066 beneficiary logic with PSN suffix handling.
    /// </summary>
    private IQueryable<MemberSlice> GetBeneficiaries(
        IProfitSharingDbContext ctx,
        StartAndEndDateRequest request)
    {
        _logger.LogInformation(
            "Loading beneficiaries for date range {BeginningDate} to {EndingDate}",
            request.BeginningDate, request.EndingDate);

        // Load beneficiaries and their related employee demographics
        var query = ctx.Beneficiaries
            .Include(b => b.Contact)
            .ThenInclude(c => c!.ContactInfo)
            .Include(b => b.Demographic)
            .Select(b => new { Beneficiary = b, b.Demographic })
            // COBOL QPAY066 (lines 767-769): IF H-TEDAT >= W-FDLY AND H-TEDAT <= W-LDLY THEN skip
            // Exclude beneficiaries whose demographics show termination within the date range.
            // This prevents terminated employees from appearing as both employees and beneficiaries.
            .Where(x => !(x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn &&
                         x.Demographic.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                         x.Demographic.TerminationDate != null &&
                         x.Demographic.TerminationDate >= request.BeginningDate &&
                         x.Demographic.TerminationDate <= request.EndingDate))
            .Select(x => new MemberSlice
            {
                // COBOL Lines 775-782: When beneficiary matches demographics AND termination date is NOT in range,
                // use badge number with PSN=0 (appears as primary employee), otherwise use PSN suffix
                PsnSuffix = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn &&
                           x.Demographic.EmploymentStatusId != EmploymentStatus.Constants.Terminated)
                           ? (short)0
                           : x.Beneficiary.PsnSuffix,
                Id = x.Beneficiary.BeneficiaryContactId,
                BadgeNumber = x.Demographic!.BadgeNumber,
                Ssn = x.Beneficiary.Contact!.Ssn,
                BirthDate = x.Beneficiary.Contact!.DateOfBirth,
                HoursCurrentYear = 0,
                EmploymentStatusCode = '\0',
                FullName = x.Beneficiary.Contact!.ContactInfo.FullName!,
                FirstName = x.Beneficiary.Contact.ContactInfo.FirstName,
                LastName = x.Beneficiary.Contact.ContactInfo.LastName,
                YearsInPs = 10, // Convention to make IsInteresting() always return true (matches READY)
                TerminationDate = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.TerminationDate : null,
                IncomeRegAndExecCurrentYear = 0,
                TerminationCode = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.TerminationCodeId : null,
                ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                EnrollmentId = 0,
                Etva = 0,
                // CRITICAL: Beneficiaries must use the requested profit year to match transaction lookups
                ProfitYear = (short)request.EndingDate.Year,
                IsOnlyBeneficiary = !((x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn &&
                                     x.Demographic.EmploymentStatusId != EmploymentStatus.Constants.Terminated)),
                IsBeneficiaryAndEmployee = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn),
                IsExecutive = false,
            });

        return query;
    }

    /// <summary>
    /// Combines employee and beneficiary slices, filtering for eligibility and removing duplicates.
    /// Prioritizes employee records over beneficiary records for the same person.
    /// </summary>
    private async Task<List<MemberSlice>> CombineEmployeeAndBeneficiarySlices(
        IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries,
        CancellationToken cancellation)
    {
        // Filter employees based on enrollment and years in plan
        var employees = terminatedWithContributions.Where(member =>
            ((member.EnrollmentId == Enrollment.Constants.NotEnrolled ||
              member.EnrollmentId == Enrollment.Constants.OldVestingPlanHasContributions ||
              member.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
             && member.YearsInPs > 2)
            ||
            ((member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions ||
              member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords)
             && member.YearsInPs > 1));

        var employeeList = await employees.ToListAsync(cancellation);
        var beneficiaryList = await beneficiaries.ToListAsync(cancellation);

        _logger.LogInformation(
            "Retrieved {EmployeeCount} employees and {BeneficiaryCount} beneficiaries",
            employeeList.Count, beneficiaryList.Count);

        // BUSINESS RULE: Prioritize employee records over beneficiary records
        // when the same person (by BadgeNumber) appears in both categories.
        var employeeBadgeNumbers = employeeList.Select(e => e.BadgeNumber).ToHashSet();
        var uniqueBeneficiaries = beneficiaryList.Where(b => !employeeBadgeNumbers.Contains(b.BadgeNumber)).ToList();

        _logger.LogInformation(
            "After filtering duplicates: {UniqueBeneficiaryCount} unique beneficiaries (filtered {FilteredCount})",
            uniqueBeneficiaries.Count, beneficiaryList.Count - uniqueBeneficiaries.Count);

        // Combine: all employees + beneficiaries without employee equivalents
        var result = employeeList.Concat(uniqueBeneficiaries).ToList();

        _logger.LogInformation(
            "Combined result: {TotalCount} members ({EmployeeCount} employees + {BeneficiaryCount} beneficiaries)",
            result.Count, employeeList.Count, uniqueBeneficiaries.Count);

        return result;
    }

    #endregion

    #region Report Dataset Creation

    /// <summary>
    /// Merges member slices with transaction and balance data to create the final report dataset.
    /// Orchestrates the loading of profit details, balances, and the construction of year detail records.
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

        var profitYearRange = GetProfitYearRange(req);
        var ssns = memberSliceUnion.Select(ms => ms.Ssn).ToHashSet();

        // COBOL Transaction Year Boundary: Does NOT process transactions after the entered year
        var transactionYearBoundary = req.EndingDate.Year;

        // Load profit detail transactions
        var profitDetailsRaw = ctx.ProfitDetails
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
                    : (x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id ? -x.Forfeiture : 0)),
                TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0),
                Distribution = g.Sum(x => (x.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id ||
                                           x.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id ||
                                           x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id)
                    ? -x.Forfeiture
                    : 0),
                BeneficiaryAllocation = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
                    ? -x.Forfeiture
                    : (x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id ? x.Contribution : 0)),
                CurrentAmount = g.Sum(x => x.Contribution + x.Earnings +
                                           (x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0) -
                                           (x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0))
            }, cancellationToken);

        // Load beginning and ending balances
        var lastYear = (short)(profitYearRange.endProfitYear - 1);
        var lastYearBalancesDict = await _totalService.GetTotalBalanceSet(ctx, lastYear)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => (x.Ssn, lastYear), x => x, cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var thisYearBalancesDict = await _totalService.TotalVestingBalance(ctx, profitYearRange.beginProfitYear, profitYearRange.endProfitYear, today)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => (x.Id, x.Ssn, profitYearRange.endProfitYear), x => x, cancellationToken);

        // Build year details list for each member
        var yearDetailsList = new List<(int BadgeNumber, short PsnSuffix, string? Name, TerminatedEmployeeAndBeneficiaryYearDetailDto YearDetail)>();

        foreach (var memberSlice in memberSliceUnion)
        {
            // Get transactions for this member
            var key = new { memberSlice.Ssn, memberSlice.ProfitYear };
            if (!profitDetailsDict.TryGetValue(key, out InternalProfitDetailDto? transactionsThisYear))
            {
                transactionsThisYear = new InternalProfitDetailDto();
            }

            // Get beginning balance from last year
            decimal? beginningAmount = lastYearBalancesDict.TryGetValue((memberSlice.Ssn, lastYear), out var lastYearBalance)
                ? lastYearBalance.TotalAmount
                : 0m;

            // Get vesting balance and percentage from this year
            var thisYearBalance = thisYearBalancesDict.GetValueOrDefault((memberSlice.Id, memberSlice.Ssn, profitYearRange.endProfitYear));
            decimal vestedBalance = thisYearBalance?.VestedBalance ?? 0m;
            var vestingPercent = thisYearBalance?.VestingPercent ?? 0;

            // Create member record with all values
            var member = new Member
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
                EndingBalance = (beginningAmount ?? 0) + transactionsThisYear.TotalForfeitures + transactionsThisYear.Distribution + transactionsThisYear.BeneficiaryAllocation,
                VestedBalance = vestedBalance
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

            if (member.ZeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
            {
                vestedBalance = member.EndingBalance;
            }
            if (vestedBalance < 0) { vestedBalance = 0; }
            if (memberSlice.IsOnlyBeneficiary) { vestingPercent = 1; }
            if (member.EndingBalance == 0 && vestedBalance == 0) { vestingPercent = 0; }

            // Calculate age if birthdate available
            int? age = null;
            if (member.Birthday.HasValue)
            {
                age = member.ProfitYear > ReferenceData.DsmMinValue.Year
                    ? member.Birthday.Value.Age(new DateTime(member.ProfitYear, 12, 31))
                    : member.Birthday.Value.Age();
            }

            // Create year detail record
            var yearDetail = new TerminatedEmployeeAndBeneficiaryYearDetailDto
            {
                ProfitYear = member.ProfitYear,
                BeginningBalance = member.BeginningAmount,
                BeneficiaryAllocation = member.BeneficiaryAllocation,
                DistributionAmount = Math.Abs(member.DistributionAmount), // Display as positive
                Forfeit = member.ForfeitAmount,
                EndingBalance = member.EndingBalance,
                VestedBalance = vestedBalance,
                DateTerm = member.TerminationDate,
                YtdPsHours = member.HoursCurrentYear,
                IsExecutive = member.IsExecutive,
                VestedPercent = vestingPercent,
                Age = age,
                HasForfeited = enrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords ||
                               enrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords,
                SuggestedForfeit = member.ProfitYear == req.ProfitYear ? member.EndingBalance - vestedBalance : null
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
    /// Determines if a member should be included in the report.
    /// Based on COBOL QPAY066 filtering logic (lines 1060-1090).
    /// Filters members based on balance and transaction activity.
    /// </summary>
    /// <param name="member">The member to evaluate for inclusion</param>
    /// <returns>True if the member should be included in the report</returns>
    private static bool IsInteresting(Member member)
    {
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
}
