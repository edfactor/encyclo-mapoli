using Bogus.DataSets;
using System.Collections.Generic;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalDto;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Generates reports for terminated employees and their beneficiaries.
/// </summary>
public sealed class TerminatedEmployeeAndBeneficiaryReport
{
    private readonly IProfitSharingDataContextFactory _factory;
    private readonly ContributionService _contributionService;

    public TerminatedEmployeeAndBeneficiaryReport(IProfitSharingDataContextFactory factory)
    {
        _factory = factory;
        _contributionService = new ContributionService(factory);
    }

    public async Task<TerminatedEmployeeAndBeneficiaryResponse> CreateData(TerminatedEmployeeAndBeneficiaryDataRequest req, CancellationToken cancellationToken)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            IAsyncEnumerable<MemberSlice> memberSliceUnion = RetrieveMemberSlices(ctx, req);
            var fullResponse = await MergeAndCreateDataset(ctx, req, memberSliceUnion, cancellationToken);
            return fullResponse;
        });
    }


    private IAsyncEnumerable<MemberSlice> RetrieveMemberSlices(ProfitSharingReadOnlyDbContext ctx, TerminatedEmployeeAndBeneficiaryDataRequest request)
    {
        // Step 1: Filter Terminated Employees
        var terminatedEmployees = GetTerminatedEmployees(ctx, request);

        // Step 2: Add Contributions to Terminated Employees
        var terminatedWithContributions = GetEmployeesWithContributions(ctx, request, terminatedEmployees);

        // Step 3: Filter Beneficiaries
        var beneficiaries = GetBeneficiaries(ctx, request);

        // Step 4: Combine and Return Results
        return CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries, request.Skip);
    }

    // Step 1: Get terminated employees with basic demographic and pay profit details
    private IQueryable<TerminatedEmployeeDto> GetTerminatedEmployees(ProfitSharingReadOnlyDbContext ctx, TerminatedEmployeeAndBeneficiaryDataRequest request)
    {
        return ctx.Demographics
            .Include(d => d.PayProfits)
            .Include(d => d.ContactInfo)
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated
                        && d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension
                        && d.TerminationDate >= request.StartDate && d.TerminationDate <= request.EndDate)
            .Select(d => new TerminatedEmployeeDto
            {
                Demographic = d,
                PayProfit = d.PayProfits
                    .Where(p => p.ProfitYear == request.ProfitYear)
                    .GroupBy(p => p.ProfitYear)
                    .Select(g => g.First())
                    .First()
            });
    }

    // Step 2: Join the terminated employees with contribution years
    private IQueryable<MemberSlice> GetEmployeesWithContributions(ProfitSharingReadOnlyDbContext ctx, TerminatedEmployeeAndBeneficiaryDataRequest request, IQueryable<TerminatedEmployeeDto> terminatedEmployees)
    {
        var validEnrollmentIds = GetValidEnrollmentIds();

        return terminatedEmployees
            .Join(
                ContributionService.GetContributionYearsQuery(ctx, request.ProfitYear, terminatedEmployees.Select(e => e.Demographic.BadgeNumber)),
                employee => employee.Demographic.BadgeNumber,
                contribution => contribution.BadgeNumber,
                (employee, contribution) => new { employee, contribution }
            )
            .Where(x => validEnrollmentIds.Contains(x.employee.PayProfit.EnrollmentId))
            .Select(x => new MemberSlice
            {
                Psn = x.employee.Demographic.BadgeNumber,
                Ssn = x.employee.Demographic.Ssn,
                BirthDate = x.employee.Demographic.DateOfBirth,
                HoursCurrentYear = x.employee.PayProfit.CurrentHoursYear ?? 0,
                NetBalanceLastYear = 0m, // Placeholder for actual value
                VestedBalanceLastYear = 0m, // Placeholder for actual value
                EmploymentStatusCode = x.employee.Demographic.EmploymentStatusId,
                FullName = x.employee.Demographic.ContactInfo.FullName,
                FirstName = x.employee.Demographic.ContactInfo.FirstName,
                MiddleInitial = x.employee.Demographic.ContactInfo.MiddleName != null
                    ? x.employee.Demographic.ContactInfo.MiddleName.Substring(0, 1)
                    : string.Empty,
                LastName = x.employee.Demographic.ContactInfo.LastName,
                YearsInPs = x.contribution.YearsInPlan,
                TerminationDate = x.employee.Demographic.TerminationDate,
                IncomeRegAndExecCurrentYear = (x.employee.PayProfit.CurrentIncomeYear ?? 0) + (x.employee.PayProfit.IncomeExecutive),
                TerminationCode = x.employee.Demographic.TerminationCodeId,
                ZeroCont = (x.employee.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                    ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                    : x.employee.PayProfit.ZeroContributionReasonId ?? 0),
                Enrolled = x.employee.PayProfit.EnrollmentId,
                Etva = x.employee.PayProfit.EarningsEtvaValue,
                BeneficiaryAllocation = 0
            });
    }

    // Step 3: Filter beneficiaries with necessary details
    private IQueryable<MemberSlice> GetBeneficiaries(ProfitSharingReadOnlyDbContext ctx, TerminatedEmployeeAndBeneficiaryDataRequest request)
    {
        var validEnrollmentIds = GetValidEnrollmentIds();

        return ctx.Beneficiaries
            .Include(b => b.Contact)
            .Include(b => b.Demographic)
            .ThenInclude(d => d!.PayProfits.Where(p => p.ProfitYear == request.ProfitYear))
            .Where(b => b.Demographic != null)
            .Select(b => new
            {
                Beneficiary = b,
                Demographic = b.Demographic,
                PayProfit = b.Demographic!.PayProfits.FirstOrDefault()
            })
            .Where(x => x.Demographic != null && validEnrollmentIds.Contains(x.PayProfit!.EnrollmentId))
            .Select(x => new MemberSlice
            {
                Psn = x.Beneficiary.PsnSuffix,
                Ssn = x.Beneficiary.Contact!.Ssn,
                BirthDate = x.Beneficiary.Contact!.DateOfBirth,
                HoursCurrentYear = 0, // Placeholder logic for hours
                NetBalanceLastYear = 0m, // Placeholder logic for balance
                VestedBalanceLastYear = 0m,
                EmploymentStatusCode = x.Demographic!.EmploymentStatusId,
                FullName = x.Beneficiary.Contact!.FullName!,
                FirstName = x.Beneficiary.Contact.FirstName,
                MiddleInitial = x.Beneficiary.Contact.MiddleName != null
                    ? x.Beneficiary.Contact.MiddleName.Substring(0, 1)
                    : string.Empty,
                LastName = x.Beneficiary.Contact.LastName,
                YearsInPs = 0, // Placeholder logic for contribution years
                TerminationDate = x.Demographic.TerminationDate,
                IncomeRegAndExecCurrentYear = (x.PayProfit!.CurrentIncomeYear ?? 0) + x.PayProfit.IncomeExecutive,
                TerminationCode = x.Demographic.TerminationCodeId,
                ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                Enrolled = x.PayProfit.EnrollmentId,
                Etva = x.PayProfit.EarningsEtvaValue,
                BeneficiaryAllocation = x.Beneficiary.Amount
            });
    }

    // Step 4: Combine terminated employees and beneficiaries and apply ordering and pagination
    private static IAsyncEnumerable<MemberSlice> CombineEmployeeAndBeneficiarySlices(
        IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries,
        int? skip)
    {
        return terminatedWithContributions
            .Union(beneficiaries)
            .OrderBy(m => m.FullName)
            .Skip(skip ?? 0)
            .AsAsyncEnumerable();
    }

    private static List<byte> GetValidEnrollmentIds()
    {
        return
        [
            Enrollment.Constants.NotEnrolled,
            Enrollment.Constants.OldVestingPlanHasContributions,
            Enrollment.Constants.OldVestingPlanHasForfeitureRecords,
            Enrollment.Constants.NewVestingPlanHasContributions,
            Enrollment.Constants.NewVestingPlanHasForfeitureRecords
        ];
    }


    private async Task<TerminatedEmployeeAndBeneficiaryResponse> MergeAndCreateDataset(
     IProfitSharingDbContext ctx,
     TerminatedEmployeeAndBeneficiaryDataRequest req,
     IAsyncEnumerable<MemberSlice> memberSliceUnion,
     CancellationToken cancellationToken)
    {
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        var membersSummary = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto>();
        var psnSet = new HashSet<int>();

#pragma warning disable S6562
        // Get the current month and day
        int currentMonth = DateTime.Now.Month;
        int currentDay = DateTime.Now.Day;

        /*
         * Validate the date using DateTime.DaysInMonth to ensure we don't create an invalid date.
         * This ensures that if the current day(e.g., 30) exceeds the number of days in the month for the provided ProfitYear,
         * the day is adjusted to the last valid day of the month(e.g., 28 or 29 for February, depending on whether the year is a leap year).
         */
        int validDay = Math.Min(currentDay, DateTime.DaysInMonth(req.ProfitYear, currentMonth));

        // Safely construct the valid date for calculating age
        DateTime forBirthDate = new DateTime(req.ProfitYear, currentMonth, validDay);
#pragma warning restore S6562


        await foreach (var memberSlice in memberSliceUnion.WithCancellation(cancellationToken))
        {
            // Fetch profit details for the current member slice
            var profitDetails = await ctx.ProfitDetails
                .Where(pd => pd.ProfitYear <= req.ProfitYear && pd.Ssn == memberSlice.Ssn)
                .ToListAsync(cancellationToken);

            // Retrieve profit detail summary directly from the list of profit details
            ProfitDetailSummary profitDetailSummary = RetrieveProfitDetail(profitDetails, memberSlice.Ssn);

            // Accumulate beneficiary allocation
            var beneficiaryAllocation = memberSlice.BeneficiaryAllocation + profitDetailSummary.BeneficiaryAllocation;

            // Check if the member has financial data to create a Member object
            if (memberSlice.NetBalanceLastYear != 0 ||
                profitDetailSummary.BeneficiaryAllocation != 0 ||
                profitDetailSummary.Distribution != 0 ||
                profitDetailSummary.Forfeiture != 0)
            {
                // Construct the Member object
                var member = new Member
                {
                    Psn = memberSlice.Psn,
                    FullName = memberSlice.FullName,
                    FirstName = memberSlice.FirstName,
                    LastName = memberSlice.LastName,
                    MiddleInitial = memberSlice.MiddleInitial,
                    Birthday = memberSlice.BirthDate,
                    HoursCurrentYear = memberSlice.HoursCurrentYear,
                    EarningsCurrentYear = memberSlice.IncomeRegAndExecCurrentYear,
                    Ssn = memberSlice.Ssn,
                    TerminationDate = memberSlice.TerminationDate,
                    TerminationCode = memberSlice.TerminationCode,
                    BeginningAmount = memberSlice.NetBalanceLastYear,
                    CurrentVestedAmount = memberSlice.VestedBalanceLastYear,
                    YearsInPlan = memberSlice.YearsInPs,
                    ZeroCont = memberSlice.ZeroCont,
                    EnrollmentId = memberSlice.Enrolled,
                    Evta = memberSlice.Etva,
                    BeneficiaryAllocation = beneficiaryAllocation,
                    DistributionAmount = profitDetailSummary.Distribution,
                    ForfeitAmount = profitDetailSummary.Forfeiture,
                    EndingBalance = memberSlice.NetBalanceLastYear + profitDetailSummary.Forfeiture + profitDetailSummary.Distribution + beneficiaryAllocation,
                    VestedBalance = memberSlice.VestedBalanceLastYear + profitDetailSummary.Distribution + beneficiaryAllocation
                };

                

                // Add PSN to the set for balance lookup later
                psnSet.Add(memberSlice.Psn);

                // Process the member summary for the report
                int vestingPercent = LookupVestingPercent(member.EnrollmentId, member.ZeroCont, member.YearsInPlan);

                byte enrollmentId = member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions ? Enrollment.Constants.NotEnrolled : member.EnrollmentId;

                decimal vestedBalance = member.VestedBalance;
                if (member.ZeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
                {
                    vestedBalance = member.EndingBalance;
                }

                if (member.EndingBalance == 0 && vestedBalance == 0)
                {
                    vestingPercent = 0;
                }

                int? age = null;
                if (member.Birthday.HasValue)
                {
                    age = member.Birthday.Value.Age(forBirthDate);
                }

                if (
                    (member.EnrollmentId is (Enrollment.Constants.NotEnrolled or Enrollment.Constants.OldVestingPlanHasContributions
                         or Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
                     && member.YearsInPlan > 2 && member.BeginningAmount != 0)
                    || (member.EnrollmentId is (Enrollment.Constants.NewVestingPlanHasContributions or Enrollment.Constants.NewVestingPlanHasForfeitureRecords) &&
                        member.YearsInPlan > 1 && member.BeginningAmount != 0)
                    || (member.BeneficiaryAllocation != 0)
                )
                {
                    membersSummary.Add(new TerminatedEmployeeAndBeneficiaryDataResponseDto()
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
                        VestedPercent = vestingPercent,
                        Age = age,
                        EnrollmentCode = enrollmentId
                    });
                    totalVested += vestedBalance;
                    totalForfeit += member.ForfeitAmount;
                    totalEndingBalance += member.EndingBalance;
                    totalBeneficiaryAllocation += member.BeneficiaryAllocation;
                }
            }

            // Stop processing if we've hit the required count
            if (membersSummary.Count >= req.Take)
            {
                break;
            }
        }

        // Query last year's balance in one go using the collected PSNs
        var lastYearsBalance = await _contributionService.GetNetBalance(ctx, (short)(req.ProfitYear - 1), psnSet, cancellationToken);

        // Update beginning amounts using last year's balance
        foreach (var member in membersSummary)
        {
            if (lastYearsBalance.TryGetValue(member.BadgePSn, out InternalProfitDetailDto? balance))
            {
                member.BeginningBalance = balance.TotalEarnings;
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
                Total = membersSummary.Count
            }
        };
    }


    private static int LookupVestingPercent(byte enrollmentId, byte? zeroCont, int yearsInPlan)
    {
        if (enrollmentId > Enrollment.Constants.NewVestingPlanHasContributions || zeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            return 100;
        }
        int vestingYearIndex;
        if (enrollmentId < Enrollment.Constants.NewVestingPlanHasContributions)
        {
            if (yearsInPlan <= 1)
            {
                vestingYearIndex = 1;
            }
            else
            {
                if (yearsInPlan > 6)
                {
                    vestingYearIndex = 7;
                }
                else
                {
                    vestingYearIndex = yearsInPlan;
                }
            }
            return ReferenceData.OlderVestingSchedule[vestingYearIndex - 1];
        }
        if (yearsInPlan <= 1)
        {
            vestingYearIndex = 1;
        }
        else
        {
            if (yearsInPlan > 5)
            {
                vestingYearIndex = 6;
            }
            else
            {
                vestingYearIndex = yearsInPlan;
            }
        }
        return ReferenceData.NewerVestingSchedule[vestingYearIndex - 1];

    }


    private static ProfitDetailSummary RetrieveProfitDetail(List<ProfitDetail> profitDetailsForAll, long ssn)
    {

        // Note that pd.profitYear is a decimal, aka 2021.2 - and we constrain on only the year portion
        List<ProfitDetail> profitDetails = profitDetailsForAll.Where(pd => pd.Ssn == ssn).ToList();

        if (profitDetails.Count == 0)
        {
            return new ProfitDetailSummary(0, 0, 0);
        }

        decimal distribution = 0;
        decimal forfeiture = 0;
        decimal beneficiaryAllocation = 0;

        foreach (ProfitDetail profitDetail in profitDetails)
        {
            if (profitDetail.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal || profitDetail.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments)
            {
                distribution -= profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
            {
                forfeiture -= profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions)
            {
                forfeiture += profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment)
            {
                distribution -= profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
            {
                beneficiaryAllocation -= profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary)
            {
                beneficiaryAllocation += profitDetail.Contribution;
            }
        }
        return new ProfitDetailSummary(distribution, forfeiture, beneficiaryAllocation);
    }
}
