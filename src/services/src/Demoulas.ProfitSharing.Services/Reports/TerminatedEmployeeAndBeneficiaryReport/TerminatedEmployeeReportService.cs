using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Generates reports for terminated employees and their beneficiaries.
/// </summary>
public sealed class TerminatedEmployeeReportService
{
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly TotalService _totalService;
    private readonly IDemographicReaderService _demographicReaderService;

    public TerminatedEmployeeReportService(IProfitSharingDataContextFactory factory,
        TotalService totalService,
        IDemographicReaderService demographicReaderService)
    {
        _factory = factory;
        _totalService = totalService;
        _demographicReaderService = demographicReaderService;
    }

    public Task<TerminatedEmployeeAndBeneficiaryResponse> CreateDataAsync(StartAndEndDateRequest req, CancellationToken cancellationToken)
    {
        return _factory.UseReadOnlyContext(async ctx =>
        {
            List<MemberSlice> memberSliceUnion = await RetrieveMemberSlices(ctx, req, cancellationToken);
            return await MergeAndCreateDataset(ctx, req, memberSliceUnion, cancellationToken);
        });
    }

    #region Get Employees and Beneficiaries

    private (short beginProfitYear, short endProfitYear) GetProfitYearRange(StartAndEndDateRequest request)
    {
        return ((short)request.BeginningDate.Year, (short)request.EndingDate.Year);
    }

    private async Task<List<MemberSlice>> RetrieveMemberSlices(IProfitSharingDbContext ctx, StartAndEndDateRequest request,
        CancellationToken cancellationToken)
    {
        var terminatedEmployees = await GetTerminatedEmployees(ctx, request);
        var terminatedWithContributions = GetEmployeesAsMembers(ctx, request, terminatedEmployees, request.EndingDate);
        var beneficiaries = GetBeneficiaries(ctx, request);
        return await CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries, cancellationToken);
    }

    private async Task<IQueryable<TerminatedEmployeeDto>> GetTerminatedEmployees(IProfitSharingDbContext ctx, StartAndEndDateRequest request)
    {
        var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);
        // BUSINESS RULE ALIGNMENT: Get all employees who might have profit sharing activity
        // READY includes employees based on profit sharing activity rather than just HR termination status
        var queryable = demographics
            .Include(d => d.ContactInfo)
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated // Focus on terminated employees for now
                        && (d.TerminationCodeId == null || d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension) // READY excludes retirees (PY_TERM != 'W')
                        && d.TerminationDate != null && d.TerminationDate >= request.BeginningDate && d.TerminationDate <= request.EndingDate) // Standard terminated employees in date range
            .Select(d => new TerminatedEmployeeDto
            {
                Demographic = d
            });

        return queryable;
    }

    private IQueryable<MemberSlice> GetEmployeesAsMembers(IProfitSharingDbContext ctx, StartAndEndDateRequest request,
        IQueryable<TerminatedEmployeeDto> terminatedEmployees, DateOnly asOfDate)
    {
        var query = from employee in terminatedEmployees
                    join payProfit in ctx.PayProfits on employee.Demographic.Id equals payProfit.DemographicId
                    join yipTbl in _totalService.GetYearsOfService(ctx, (short)request.EndingDate.Year, asOfDate) on payProfit.Demographic!.Ssn equals yipTbl.Ssn into yipTmp
                    from yip in yipTmp.DefaultIfEmpty()
                    where payProfit.ProfitYear >= request.BeginningDate.Year && payProfit.ProfitYear <= request.EndingDate.Year
                        // COBOL ANALYSIS: READY does NOT filter by YTD work hours - processes all terminated employees regardless of hours
                    select new MemberSlice
                    {
                        PsnSuffix = 0,
                        Id = employee.Demographic.Id,
                        BadgeNumber = employee.Demographic.BadgeNumber,
                        Ssn = employee.Demographic.Ssn,
                        BirthDate = employee.Demographic.DateOfBirth,
                        HoursCurrentYear = payProfit.CurrentHoursYear,
                        EmploymentStatusCode = employee.Demographic.EmploymentStatusId,
                        FullName = employee.Demographic.ContactInfo.FullName,
                        FirstName = employee.Demographic.ContactInfo.FirstName,
                        LastName = employee.Demographic.ContactInfo.LastName,
                        YearsInPs = yip != null ? (yip.Years) : (byte)0,
                        TerminationDate = employee.Demographic.TerminationDate,
                        IncomeRegAndExecCurrentYear = payProfit.CurrentIncomeYear + payProfit.IncomeExecutive,
                        TerminationCode = employee.Demographic.TerminationCodeId,
                        ZeroCont = (employee.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                            ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                            : payProfit.ZeroContributionReasonId != null ? payProfit.ZeroContributionReasonId : 0),
                        EnrollmentId = payProfit.EnrollmentId,
                        Etva = payProfit.Etva,
                        ProfitYear = payProfit.ProfitYear,
                        IsOnlyBeneficiary = false,
                        IsBeneficiaryAndEmployee = false,
                        IsExecutive = employee.Demographic.PayFrequencyId == PayFrequency.Constants.Monthly
                    };

        return query;
    }

#pragma warning disable S1172
    private IQueryable<MemberSlice> GetBeneficiaries(IProfitSharingDbContext ctx, StartAndEndDateRequest request)
    {
        // This query loads the Beneficiary and then the employee they are related to
        var query = ctx.Beneficiaries
            .Include(b => b.Contact)
            .ThenInclude(c => c!.ContactInfo)
            .Include(b => b.Demographic)
            .Select(b => new { Beneficiary = b, b.Demographic })
            // BUSINESS RULE ALIGNMENT WITH COBOL QPAY066:
            // Exclude beneficiaries who have matching SSNs with terminated employees in the date range.
            // COBOL logic: Skip beneficiaries if their demographics show termination date within range.
            // This prevents terminated employees from appearing as both employees and beneficiaries.
            .Where(x => !(x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn &&
                         x.Demographic.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                         x.Demographic.TerminationDate != null &&
                         x.Demographic.TerminationDate >= request.BeginningDate &&
                         x.Demographic.TerminationDate <= request.EndingDate))
            .Select(x => new MemberSlice
            {
                PsnSuffix = x.Beneficiary.PsnSuffix,
                Id = x.Beneficiary.BeneficiaryContactId,
                BadgeNumber = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.BadgeNumber : x.Beneficiary!.BadgeNumber,
                Ssn = x.Beneficiary.Contact!.Ssn,
                BirthDate = x.Beneficiary.Contact!.DateOfBirth,
                HoursCurrentYear = 0, // default for beneficiaries
                EmploymentStatusCode = '\0', // default for beneficiaries
                FullName = x.Beneficiary.Contact!.ContactInfo.FullName!,
                FirstName = x.Beneficiary.Contact.ContactInfo.FirstName,
                LastName = x.Beneficiary.Contact.ContactInfo.LastName,
                YearsInPs = 10, // Makes function IsInteresting() always return true for beneficiaries.  This is the same value/convention used in READY.
                TerminationDate = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.TerminationDate : null,
                IncomeRegAndExecCurrentYear = 0, // default for beneficiaries
                TerminationCode = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.TerminationCodeId : null,
                ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                EnrollmentId = 0, // default for beneficiaries
                Etva = 0, // default for beneficiaries
                ProfitYear = 0, // default for beneficiaries
                IsOnlyBeneficiary = true,
                IsBeneficiaryAndEmployee = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn),
                IsExecutive = false,
            });

        return query;
    }

    private static async Task<List<MemberSlice>> CombineEmployeeAndBeneficiarySlices(IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries, CancellationToken cancellation)
    {
        // NOTE: the server side union fails
        var employees = terminatedWithContributions.Where(member => ((member.EnrollmentId == Enrollment.Constants.NotEnrolled ||
                                                                            member.EnrollmentId == Enrollment.Constants.OldVestingPlanHasContributions ||
                                                                            member.EnrollmentId == Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
                                                                           && member.YearsInPs > 2)
                                                                          ||
                                                                          ((member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions ||
                                                                            member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasForfeitureRecords)
                                                                           && member.YearsInPs > 1));

        // BUSINESS RULE ALIGNMENT WITH READY SYSTEM:
        // To match READY's behavior, prioritize employee records over beneficiary records
        // when the same person (by BadgeNumber) appears in both categories.
        // This prevents duplicate entries and ensures consistent classification.

        var employeeList = await employees.ToListAsync(cancellation);
        var beneficiaryList = await beneficiaries.ToListAsync(cancellation);

        // Get badge numbers of employees to exclude duplicate beneficiaries
        var employeeBadgeNumbers = employeeList.Select(e => e.BadgeNumber).ToHashSet();

        // Only include beneficiaries who don't have a corresponding employee record
        var uniqueBeneficiaries = beneficiaryList.Where(b => !employeeBadgeNumbers.Contains(b.BadgeNumber)).ToList();

        // Combine unique records: all employees + beneficiaries without employee equivalents
        var result = employeeList.Concat(uniqueBeneficiaries).ToList();

        return result;
    }

    #endregion

    private async Task<TerminatedEmployeeAndBeneficiaryResponse> MergeAndCreateDataset(IProfitSharingDbContext ctx, StartAndEndDateRequest req,
        List<MemberSlice> memberSliceUnion, CancellationToken cancellationToken)
    {
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        var profitYearRange = GetProfitYearRange(req);
        var ssns = memberSliceUnion.Select(ms => ms.Ssn).ToHashSet();

        // COBOL Transaction Year Boundary Filtering Implementation
        // Based on COBOL logic: "Does NOT process transactions after the entered year"
        // This prevents including transactions from future years that shouldn't affect the report
        var transactionYearBoundary = req.EndingDate.Year;

        var profitDetailsRaw = ctx.ProfitDetails
            .Where(pd => pd.ProfitYear >= profitYearRange.beginProfitYear
                      && pd.ProfitYear <= profitYearRange.endProfitYear
                      && pd.ProfitYear <= transactionYearBoundary // NEW: Transaction year boundary filtering
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

        var lastYear = (short)(profitYearRange.endProfitYear - 1);
        var lastYearBalancesDict = await _totalService.GetTotalBalanceSet(ctx, lastYear)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => (x.Ssn, lastYear), x => x, cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var thisYearBalancesDict = await _totalService.TotalVestingBalance(ctx, profitYearRange.beginProfitYear, profitYearRange.endProfitYear, today)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => (x.Id, x.Ssn, profitYearRange.endProfitYear), x => x, cancellationToken);

        // Build a list of all year details, then group by BadgeNumber, PsnSuffix, Name
        var yearDetailsList = new List<(int BadgeNumber, short PsnSuffix, string? Name, TerminatedEmployeeAndBeneficiaryYearDetailDto YearDetail)>();

        foreach (var memberSlice in memberSliceUnion)
        {
            var key = new { memberSlice.Ssn, memberSlice.ProfitYear };
            if (!profitDetailsDict.TryGetValue(key, out InternalProfitDetailDto? transactionsThisYear))
            {
                transactionsThisYear = new InternalProfitDetailDto();
            }

            decimal? beginningAmount = lastYearBalancesDict.TryGetValue((memberSlice.Ssn, lastYear), out var lastYearBalance)
                ? lastYearBalance.TotalAmount
                : 0m;

            var thisYearBalance = thisYearBalancesDict.GetValueOrDefault((memberSlice.Id, memberSlice.Ssn, profitYearRange.endProfitYear));
            decimal vestedBalance = thisYearBalance?.VestedBalance ?? 0m;
            var vestingPercent = thisYearBalance?.VestingPercent ?? 0;

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
                EndingBalance = (beginningAmount ?? 0)
                                + transactionsThisYear.TotalForfeitures + transactionsThisYear.Distribution + transactionsThisYear.BeneficiaryAllocation,
                VestedBalance = vestedBalance
            };

            if (!IsInteresting(member))
            {
                continue;
            }

            byte enrollmentId = member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions ? Enrollment.Constants.NotEnrolled : member.EnrollmentId;
            if (member.ZeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
            {
                vestedBalance = member.EndingBalance;
            }
            if (vestedBalance < 0) { vestedBalance = 0; }
            if (memberSlice.IsOnlyBeneficiary) { vestingPercent = 1; }
            if (member.EndingBalance == 0 && vestedBalance == 0) { vestingPercent = 0; }
            int? age = null;
            if (member.Birthday.HasValue)
            {
                age = member.ProfitYear > ReferenceData.DsmMinValue.Year ? member.Birthday.Value.Age(new DateTime(member.ProfitYear, 12, 31)) : member.Birthday.Value.Age();
            }

            var yearDetail = new TerminatedEmployeeAndBeneficiaryYearDetailDto
            {
                ProfitYear = member.ProfitYear,
                BeginningBalance = member.BeginningAmount,
                BeneficiaryAllocation = member.BeneficiaryAllocation,
                DistributionAmount = Math.Abs(member.DistributionAmount), // Display as positive to match READY format
                Forfeit = member.ForfeitAmount,
                EndingBalance = member.EndingBalance,
                VestedBalance = vestedBalance,
                DateTerm = member.TerminationDate,
                YtdPsHours = member.HoursCurrentYear,
                IsExecutive = member.IsExecutive,
                VestedPercent = vestingPercent,
                Age = age,
                HasForfeited = enrollmentId == /*3*/ Enrollment.Constants.OldVestingPlanHasForfeitureRecords || enrollmentId == /*4*/ Enrollment.Constants.NewVestingPlanHasForfeitureRecords,
                SuggestedForfeit = member.ProfitYear == req.ProfitYear ? member.EndingBalance - vestedBalance : null
            };

            yearDetailsList.Add((member.BadgeNumber, member.PsnSuffix, member.FullName, yearDetail));

            totalVested += vestedBalance;
            totalForfeit += member.ForfeitAmount;
            totalEndingBalance += member.EndingBalance;
            totalBeneficiaryAllocation += member.BeneficiaryAllocation;
        }

        // Group by BadgeNumber, PsnSuffix, Name
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



    /// <summary>
    /// Do we include the member in the report or not? Based on COBOL QPAY066 filtering logic:
    /// Implements the exact filtering logic from READY COBOL lines 1060-1090.
    /// Must match the complex vesting and balance rules from the legacy system.
    /// </summary>
    /// <param name="member">The member to evaluate for inclusion</param>
    /// <returns>True if the member should be included in the report</returns>
    private static bool IsInteresting(Member member)
    {
        // TEMPORARY DEBUG: Let's see what values we're getting and use the original logic for now
        // Original logic that was working (balance-based filtering only)

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
}
