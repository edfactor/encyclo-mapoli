using System.Linq;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Demoulas.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Generates reports for terminated employees and their beneficiaries.
/// </summary>
public sealed class TerminatedEmployeeAndBeneficiaryReport
{
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly ICalendarService _calendarService;
    private readonly TotalService _totalService;

    public TerminatedEmployeeAndBeneficiaryReport(IProfitSharingDataContextFactory factory,
        ICalendarService calendarService,
        TotalService totalService)
    {
        _factory = factory;
        _calendarService = calendarService;
        _totalService = totalService;
    }

    public Task<TerminatedEmployeeAndBeneficiaryResponse> CreateDataAsync(ProfitYearRequest req, CancellationToken cancellationToken)
    {
        return _factory.UseReadOnlyContext(async ctx =>
        {
            List<MemberSlice> memberSliceUnion = await RetrieveMemberSlices(ctx, req, cancellationToken);
            return await MergeAndCreateDataset(ctx, req, memberSliceUnion, cancellationToken);
        });
    }

    #region Get Employees and Beneficiaries

    private async Task<List<MemberSlice>> RetrieveMemberSlices(IProfitSharingDbContext ctx, ProfitYearRequest request,
        CancellationToken cancellationToken)
    {
        CalendarResponseDto startEnd = await _calendarService.GetYearStartAndEndAccountingDatesAsync(request.ProfitYear, cancellationToken);
        var terminatedEmployees = GetTerminatedEmployees(ctx, request, startEnd);
        var terminatedWithContributions = GetEmployeesAsMembers(ctx, request, terminatedEmployees);
        var beneficiaries = GetBeneficiaries(ctx, request);
        return await CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries, cancellationToken);
    }

    private IQueryable<TerminatedEmployeeDto> GetTerminatedEmployees(IProfitSharingDbContext ctx, ProfitYearRequest request,
        CalendarResponseDto startEnd)
    {
        var queryable = ctx.Demographics
            .Include(d => d.PayProfits)
            .Include(d => d.ContactInfo)
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                        && d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension
                        && d.TerminationDate >= startEnd.FiscalBeginDate && d.TerminationDate <= startEnd.FiscalEndDate)
            .Select(d => new TerminatedEmployeeDto
            {
                Demographic = d,
                PayProfit = d.PayProfits
                    .Where(p => p.ProfitYear == request.ProfitYear)
                    .GroupBy(p => p.ProfitYear)
                    .Select(g => g.First())
                    .FirstOrDefault()
            });

        return queryable;
    }

    private IQueryable<MemberSlice> GetEmployeesAsMembers(IProfitSharingDbContext ctx, ProfitYearRequest request,
        IQueryable<TerminatedEmployeeDto> terminatedEmployees)
    {
        var query = from employee in terminatedEmployees
            join payProfit in ctx.PayProfits on employee.Demographic.Id equals payProfit.DemographicId
            join yipTbl in _totalService.GetYearsOfService(ctx, request.ProfitYear) on payProfit.Demographic!.Ssn equals yipTbl.Ssn into yipTmp
            from yip in yipTmp.DefaultIfEmpty()
            where payProfit.ProfitYear == request.ProfitYear
            select new MemberSlice
            {
                PsnSuffix = 0,
                BadgeNumber = employee.Demographic.BadgeNumber,
                Ssn = employee.Demographic.Ssn,
                BirthDate = employee.Demographic.DateOfBirth,
                HoursCurrentYear = payProfit.CurrentHoursYear,
                EmploymentStatusCode = employee.Demographic.EmploymentStatusId,
                FullName = employee.Demographic.ContactInfo.FullName,
                FirstName = employee.Demographic.ContactInfo.FirstName,
                MiddleInitial = employee.Demographic.ContactInfo.MiddleName,
                LastName = employee.Demographic.ContactInfo.LastName,
                YearsInPs = yip != null ? (yip.Years ?? 0): (byte)0,
                TerminationDate = employee.Demographic.TerminationDate,
                IncomeRegAndExecCurrentYear = payProfit.CurrentIncomeYear + payProfit.IncomeExecutive,
                TerminationCode = employee.Demographic.TerminationCodeId,
                ZeroCont = (employee.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                    ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                    : payProfit.ZeroContributionReasonId ?? 0),
                EnrollmentId = payProfit.EnrollmentId,
                Etva = payProfit.EarningsEtvaValue,
                BeneficiaryAllocation = 0
            };

        return query;
    }

#pragma warning disable S1172
    private IQueryable<MemberSlice> GetBeneficiaries(IProfitSharingDbContext ctx, ProfitYearRequest request)
    {
        // This query loads the Beneficiary and then the employee they are related to
        var query = ctx.Beneficiaries
            .Include(b => b.Contact)
            .ThenInclude(c => c!.ContactInfo)
            .Include(b => b.Demographic)
            .ThenInclude(d => d!.PayProfits.Where(p => p.ProfitYear == request.ProfitYear))
            .Where(b => b.Demographic != null)
            .Select(b => new { Beneficiary = b, Demographic = b.Demographic, PayProfit = b.Demographic!.PayProfits.FirstOrDefault(p => p.ProfitYear == request.ProfitYear) })
            .Where(x => x.PayProfit != null)
            .Select(x => new MemberSlice
            {
                PsnSuffix = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? 0 : x.Beneficiary.PsnSuffix,
                BadgeNumber = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.BadgeNumber : x.Beneficiary!.BadgeNumber,
                Ssn = x.Beneficiary.Contact!.Ssn,
                BirthDate = x.Beneficiary.Contact!.DateOfBirth,
                FullName = x.Beneficiary.Contact!.ContactInfo.FullName!,
                FirstName = x.Beneficiary.Contact.ContactInfo.FirstName,
                MiddleInitial = x.Beneficiary.Contact.ContactInfo.MiddleName != null
                    ? x.Beneficiary.Contact.ContactInfo.MiddleName.Substring(0, 1)
                    : string.Empty,
                LastName = x.Beneficiary.Contact.ContactInfo.LastName,
                YearsInPs = 10, // Makes function IsInteresting() always return true for beneficiaries.  This is the same value/convention used in READY.
                BeneficiaryAllocation = x.Beneficiary.Amount,
                TerminationCode = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.TerminationCodeId : null,
                TerminationDate = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? x.Demographic.TerminationDate : null,
                ZeroCont = /*6*/ ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                IsOnlyBeneficiary = true,
#pragma warning disable S1125
                IsBeneficiaryAndEmployee = (x.Beneficiary!.Contact!.Ssn == x.Demographic!.Ssn) ? true : false,
#pragma warning restore S1125
            });

        return query;
    }

    private static async Task<List<MemberSlice>> CombineEmployeeAndBeneficiarySlices(IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries, CancellationToken cancellation)
    {
        // NOTE: the server side union fails
        var benes = await beneficiaries.ToListAsync(cancellation);
        var employees = await terminatedWithContributions.ToListAsync(cancellation);
        return benes.Concat(employees)
            // NOTE: Sort using same character handling that ready uses (ie "Mc" sorts after "ME") aka the Ordinal sort.
            // Failure to use this sort, causes READY and SMART reports to not match.
            .OrderBy(x => x.FullName, StringComparer.Ordinal)
            .ThenBy(x => x.BadgeNumber)
            .ToList();
    }

    #endregion

    private async Task<TerminatedEmployeeAndBeneficiaryResponse> MergeAndCreateDataset(IProfitSharingDbContext ctx, ProfitYearRequest req,
    List<MemberSlice> memberSliceUnion, CancellationToken cancellationToken)
    {
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        short lastYear = (short)(req.ProfitYear - 1);
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Extract SSNs needed in the loop
        var ssns = memberSliceUnion.Select(ms => ms.Ssn).Distinct().ToList();

        // Bulk load profit details for this profit year, grouped by SSN.
        var profitDetailsDict = await ctx.ProfitDetails
            .Where(pd => pd.ProfitYear == req.ProfitYear && ssns.Contains(pd.Ssn))
            .GroupBy(pd => pd.Ssn)
            .Select(g => new InternalProfitDetailDto
            {
                Ssn = g.Key,
                TotalContributions = g.Sum(x => x.Contribution),
                TotalEarnings = g.Sum(x => x.Earnings),
                TotalForfeitures = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id
                                                ? x.Forfeiture
                                                : (x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures.Id ? -x.Forfeiture : 0)),
                TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0),
                Distribution = g.Sum(x => (x.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal.Id ||
                                            x.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments.Id ||
                                            x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment.Id)
                                            ? -x.Forfeiture : 0),
                BeneficiaryAllocation = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
                                                    ? -x.Forfeiture
                                                    : (x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id ? x.Contribution : 0)),
                CurrentAmount = g.Sum(x => x.Contribution + x.Earnings +
                                          (x.ProfitCodeId == ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0) -
                                          (x.ProfitCodeId != ProfitCode.Constants.IncomingContributions.Id ? x.Forfeiture : 0))
            })
            .ToDictionaryAsync(x => x.Ssn, cancellationToken);

        // Bulk load last year balances as a dictionary keyed by SSN.
        var lastYearBalancesDict = await _totalService.GetTotalBalanceSet(ctx, lastYear)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => x.Ssn, cancellationToken);

        // Bulk load current year vesting balances as a dictionary keyed by SSN.
        var thisYearBalancesDict = await _totalService.TotalVestingBalance(ctx, req.ProfitYear, req.ProfitYear, today)
            .Where(x => ssns.Contains(x.Ssn))
            .ToDictionaryAsync(x => x.Ssn, cancellationToken);

        var membersSummary = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto>();
        var unions = memberSliceUnion.Skip(req.Skip ?? 0).ToList();
        var count = memberSliceUnion.Count;

        // Refactored loop using bulk loaded dictionary lookup
        foreach (var memberSlice in unions)
        {
            // Lookup profit details; if missing, use a default instance.
            if (!profitDetailsDict.TryGetValue(memberSlice.Ssn, out InternalProfitDetailDto? transactionsThisYear))
            {
                transactionsThisYear = new InternalProfitDetailDto();
            }

            // Lookup last year balance (BeginningAmount)
            decimal? beginningAmount = lastYearBalancesDict.TryGetValue(memberSlice.Ssn, out var lastYearBalance)
                ? lastYearBalance.Total : 0m;

            // Lookup vesting balance and vesting percent for current year.
            var thisYearBalance = thisYearBalancesDict.GetValueOrDefault(memberSlice.Ssn);

            decimal vestedBalance = thisYearBalance?.VestedBalance ?? 0m;
            var vestingPercent = thisYearBalance?.VestingPercent ?? 0;

            // Construct member record.
            var member = new Member
            {
                Psn = memberSlice.PsnSuffix > 0 ? $"{memberSlice.BadgeNumber}{memberSlice.PsnSuffix}" : memberSlice.BadgeNumber.ToString(),
                FullName = memberSlice.FullName,
                FirstName = memberSlice.FirstName,
                LastName = memberSlice.LastName,
                MiddleInitial = memberSlice.MiddleInitial?.Length > 1 ? memberSlice.MiddleInitial[..1] : memberSlice.MiddleInitial,
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
                Evta = memberSlice.Etva,
                BeneficiaryAllocation = transactionsThisYear.BeneficiaryAllocation,
                DistributionAmount = transactionsThisYear.Distribution,
                ForfeitAmount = transactionsThisYear.TotalForfeitures,
                EndingBalance = (beginningAmount ?? 0)
                                  + transactionsThisYear.TotalForfeitures + transactionsThisYear.Distribution + transactionsThisYear.BeneficiaryAllocation,
                VestedBalance = vestedBalance
            };

            // If not interesting, skip.
            if (!IsInteresting(member))
            {
                count--;
                continue;
            }

            byte enrollmentId = member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                ? Enrollment.Constants.NotEnrolled
                : member.EnrollmentId;

            if (member.ZeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
            {
                vestedBalance = member.EndingBalance;
            }

            if (vestedBalance < 0)
            {
                vestedBalance = 0;
            }

            if (memberSlice.IsOnlyBeneficiary)
            {
                vestingPercent = 100;
            }
            if (member.EndingBalance == 0 && vestedBalance == 0)
            {
                vestingPercent = 0;
            }

            int? age = null;
            if (member.Birthday.HasValue)
            {
                age = member.Birthday.Value.Age();
            }

            membersSummary.Add(new TerminatedEmployeeAndBeneficiaryDataResponseDto
            {
                BadgePSn = member.Psn,
                Name = member.FullName,
                BeginningBalance = member.BeginningAmount,
                BeneficiaryAllocation = member.BeneficiaryAllocation,
                DistributionAmount = member.DistributionAmount,
                Forfeit = member.ForfeitAmount,
                EndingBalance = member.EndingBalance,
                VestedBalance = vestedBalance,
                DateTerm = member.TerminationDate,
                YtdPsHours = member.HoursCurrentYear,
                VestedPercent = vestingPercent * 100,
                Age = age,
                EnrollmentCode = enrollmentId
            });

            totalVested += vestedBalance;
            totalForfeit += member.ForfeitAmount;
            totalEndingBalance += member.EndingBalance;
            totalBeneficiaryAllocation += member.BeneficiaryAllocation;

            if (membersSummary.Count >= req.Take)
            {
                break;
            }
        }

        return new TerminatedEmployeeAndBeneficiaryResponse
        {
            ReportName = "Terminated Employee and Beneficiary Report",
            ReportDate = DateTimeOffset.Now,
            TotalVested = totalVested,
            TotalForfeit = totalForfeit,
            TotalEndingBalance = totalEndingBalance,
            TotalBeneficiaryAllocation = totalBeneficiaryAllocation,
            Response = new PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>(req)
            {
                Results = membersSummary,
                Total = count
            }
        };
    }


    /// <summary>
    /// Do we include the member in the report or not?    They are interesting if they have money (as a bene) or
    /// have been in the plan long enough to have money.
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    private static bool IsInteresting(Member member)
    {
        // Has a bene allocation
        if (member.BeneficiaryAllocation != 0)
        {
            return true;
        }

        //  OldPlan, > 2 years, has beginning amount
        if ((member.EnrollmentId is (Enrollment.Constants.NotEnrolled /*0*/ or Enrollment.Constants.OldVestingPlanHasContributions /*1*/
                 or Enrollment.Constants.OldVestingPlanHasForfeitureRecords /*3*/)
             && member.YearsInPlan > 2
             && member.BeginningAmount != 0))
        {
            return true;
        }

        // NewPlan, > 1 year, has beginning amount
        if (member.EnrollmentId is (Enrollment.Constants.NewVestingPlanHasContributions /*2*/ or Enrollment.Constants.NewVestingPlanHasForfeitureRecords /*4*/ ) &&
            member.YearsInPlan > 1 && member.BeginningAmount != 0)
        {
            return true;
        }

        return false;
    }
}
