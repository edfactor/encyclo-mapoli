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
        List<byte> validEnrollmentId =
        [
            Enrollment.Constants.NotEnrolled,
            Enrollment.Constants.OldVestingPlanHasContributions,
            Enrollment.Constants.OldVestingPlanHasForfeitureRecords,
            Enrollment.Constants.NewVestingPlanHasContributions,
            Enrollment.Constants.NewVestingPlanHasForfeitureRecords
        ];

        var demographicSliceQuery = ctx.Demographics
            .Include(d => d.PayProfits) // Include related PayProfits
            .Include(d => d.ContactInfo) // Include related ContactInfo
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                        d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension &&
                        d.TerminationDate >= request.StartDate && d.TerminationDate <= request.EndDate)
            .Select(d => new
            {
                Demographic = d,
                PayProfit = d.PayProfits
                    .Where(p => p.ProfitYear == request.ProfitYear)
                    .GroupBy(p => p.ProfitYear)
                    .Select(g => g.First())
                    .First()
            });

        // Now, add the Join with ContributionYears
        var demographicWithContributionQuery = demographicSliceQuery
            .Join(
                ContributionService.GetContributionYearsQuery(ctx, request.ProfitYear, demographicSliceQuery.Select(d => d.Demographic.BadgeNumber)),
                d => d.Demographic.BadgeNumber,
                contribution => contribution.BadgeNumber,
                (d, contribution) => new { d, contribution }
            );


        var demographicSlice = demographicWithContributionQuery
            .Where(x=> validEnrollmentId.Contains(x.d.PayProfit.EnrollmentId))
            .Select(x => new MemberSlice
            {
                Psn = x.d.Demographic.BadgeNumber,
                Ssn = x.d.Demographic.Ssn,
                BirthDate = x.d.Demographic.DateOfBirth,
                HoursCurrentYear = x.d.PayProfit!.CurrentHoursYear ?? 0,
                NetBalanceLastYear = 0m, // TO-DO !!! PayProfit refactor, pp.NetBalanceLastYear
                VestedBalanceLastYear = 0m, // TO-DO !!!! PayProfit refactor pp.VestedBalanceLastYear,
                EmploymentStatusCode = x.d.Demographic.EmploymentStatusId,
                FullName = x.d.Demographic.ContactInfo.FullName,
                FirstName = x.d.Demographic.ContactInfo.FirstName,
                MiddleInitial = x.d.Demographic.ContactInfo.MiddleName != null
                    ? x.d.Demographic.ContactInfo.MiddleName.Substring(0, 1)
                    : string.Empty,
                LastName = x.d.Demographic.ContactInfo.LastName,
                YearsInPs = x.contribution.YearsInPlan, // Now set with the new ContributionYears object
                TerminationDate = x.d.Demographic.TerminationDate,
                IncomeRegAndExecCurrentYear = (x.d.PayProfit!.CurrentIncomeYear ?? 0) + (x.d.PayProfit!.IncomeExecutive),
                TerminationCode = x.d.Demographic.TerminationCodeId,
                ZeroCont = (x.d.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                    ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested
                    : x.d.PayProfit!.ZeroContributionReasonId ?? 0),
                Enrolled = x.d.PayProfit!.EnrollmentId,
                Etva = x.d.PayProfit!.EarningsEtvaValue,
                BeneficiaryAllocation = 0
            });






        IQueryable<MemberSlice> beneficiarySlice = ctx.Beneficiaries
            .Include(b => b.Contact)
            .Include(b => b.Demographic)
            .ThenInclude(d => d!.PayProfits.Where(p => p.ProfitYear == request.ProfitYear))
            .Where(b => b.Demographic != null) // Ensure there is a related Demographic
            .Select(b => new
            {
                Beneficiary = b,
                b.Demographic,
                PayProfit =
                    b.Demographic!.PayProfits[0] // Select the PayProfit for the specified profit year since there's only one record, fetch the first
            })
            .Where(x => x.Demographic != null
                                              && validEnrollmentId.Contains(x.PayProfit.EnrollmentId))
            .Select(x => new MemberSlice
            {
                Psn = x.Beneficiary.PsnSuffix,
                Ssn = x.Beneficiary.Contact!.Ssn,
                BirthDate = x.Beneficiary.Contact!.DateOfBirth,
                HoursCurrentYear = 0, // Replace with actual logic if needed
                NetBalanceLastYear = 0m, // Use PayProfit for the amounts
                VestedBalanceLastYear = 0m,
                EmploymentStatusCode = x.Demographic!.EmploymentStatusId,
                FullName = x.Beneficiary.Contact!.FullName!,
                FirstName = x.Beneficiary.Contact!.FirstName,
                MiddleInitial = x.Beneficiary.Contact.MiddleName != null ? x.Beneficiary.Contact.MiddleName.Substring(0, 1) : string.Empty,
                LastName = x.Beneficiary.Contact.LastName,
                YearsInPs = 0, // Use PayProfit for contribution years
                TerminationDate = x.Demographic.TerminationDate,
                IncomeRegAndExecCurrentYear = (x.PayProfit!.CurrentIncomeYear ?? 0) + (x.PayProfit.IncomeExecutive),
                TerminationCode = x.Demographic.TerminationCodeId,
                ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                Enrolled = x.PayProfit.EnrollmentId,
                Etva = x.PayProfit.EarningsEtvaValue,
                BeneficiaryAllocation = x.Beneficiary.Amount // Adjust if needed
            });

        return demographicSlice
            .Union(beneficiarySlice)
            .OrderBy(m => m.FullName)
            .Skip(request.Skip ?? 0)
            .AsAsyncEnumerable();
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

        // Date for calculating age
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

        // Safely construct the valid date
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
