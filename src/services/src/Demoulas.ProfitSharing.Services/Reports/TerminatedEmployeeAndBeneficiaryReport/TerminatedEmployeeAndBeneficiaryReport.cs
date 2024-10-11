using System.Runtime.Intrinsics.X86;
using Demoulas.Common.Contracts.Contracts.Request;
using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.Common.Data.Contexts.Extensions;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Reports on both employees which where terminated in the time range specified (but not retired.), and all beneficiaries.
/// </summary>
internal sealed class TerminatedEmployeeAndBeneficiaryReport
{
    private readonly ProfitSharingReadOnlyDbContext _ctx;

    public TerminatedEmployeeAndBeneficiaryReport(ProfitSharingReadOnlyDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<TerminatedEmployeeAndBeneficiaryResponse> CreateData(TerminatedEmployeeAndBeneficiaryDataRequest req)
    {

        IOrderedQueryable<MemberSlice> memberSliceUnion = RetrieveMemberSlices(req);
        List<Member> members = await MergeMemberSlicesToMembers(req, memberSliceUnion);
        TerminatedEmployeeAndBeneficiaryResponse fullResponse =  CreateDataset(members, req);
        return fullResponse;
    }


    private IOrderedQueryable<MemberSlice> RetrieveMemberSlices(TerminatedEmployeeAndBeneficiaryDataRequest request)
    {
        // slices of member information (aka employee or beneficiary information)

       var demographicSlice =  _ctx.Demographics
            .Include(d => d.PayProfits)
            .Include(demographic => demographic.ContactInfo)
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                        d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension &&
                        d.TerminationDate >= request.StartDate && d.TerminationDate <= request.EndDate)
            .Select(d => new
            {
                Demographic = d,
                PayProfit = d.PayProfits
                    .Where(p => p.ProfitYear == request.ProfitYear)
                    .GroupBy(p => p.ProfitYear)
                    .Select(g => g.First()) // Extracting the first (and only) PayProfit for the year
                    .First()
            })
            .Select(d => new MemberSlice
            {
                Psn = d.Demographic.BadgeNumber,
                Ssn = d.Demographic.Ssn,
                BirthDate = d.Demographic.DateOfBirth,
                HoursCurrentYear = 0, // hours
                NetBalanceLastYear = 0m, // TO-DO !!! PayProfit refactor, pp.NetBalanceLastYear
                VestedBalanceLastYear = 0m, // TO-DO !!!! PayProfit refactor pp.VestedBalanceLastYear,
                EmploymentStatusCode = d.Demographic.EmploymentStatusId,
                FullName = d.Demographic.ContactInfo.FullName,
                FirstName = d.Demographic.ContactInfo.FirstName,
                MiddleInitial = d.Demographic.ContactInfo.MiddleName != null ? d.Demographic.ContactInfo.MiddleName[..1] : string.Empty,
                LastName = d.Demographic.ContactInfo.LastName,
                YearsInPs = 0, // TO-DO !!! PayProfit refactor, pp.CompanyContributionYears,
                TerminationDate = d.Demographic.TerminationDate,
                IncomeRegAndExecCurrentYear = (d.PayProfit.CurrentIncomeYear ?? 0) + d.PayProfit.IncomeExecutive,
                TerminationCode = d.Demographic.TerminationCodeId,
                ZeroCont = d.Demographic.TerminationCodeId == TerminationCode.Constants.Deceased
                    ? ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested // 6
                    : d.PayProfit.ZeroContributionReasonId ?? 0,
                Enrolled = d.PayProfit.EnrollmentId,
                Etva = d.PayProfit.EarningsEtvaValue,
                BeneficiaryAllocation = 0
            });


       var beneficiarySlice = _ctx.Beneficiaries
           .Include(b => b.Demographic)
           .ThenInclude(d => d!.PayProfits)
           .Where(b => b.Demographic != null) // Ensure there is a related Demographic
           .Select(b => new
           {
               Beneficiary = b,
               b.Demographic,
               PayProfit = b.Demographic!.PayProfits
                   .First(p => p.ProfitYear == request.ProfitYear) // Select the PayProfit for the specified profit year since there's only one record, fetch the first
           })
           .Where(x => x.Demographic != null) // Filter out invalid data
           .Select(x => new MemberSlice
           {
               Psn = x.Demographic!.BadgeNumber,
               Ssn = x.Beneficiary.Contact!.Ssn,
               BirthDate = x.Beneficiary.Contact!.DateOfBirth,
               HoursCurrentYear = 0, // Replace with actual logic if needed
               NetBalanceLastYear = 0m, // Use PayProfit for the amounts
               VestedBalanceLastYear = 0m,
               EmploymentStatusCode = x.Demographic.EmploymentStatusId,
               FullName = x.Beneficiary.Contact!.FullName!,
               FirstName = x.Beneficiary.Contact!.FirstName,
               MiddleInitial = x.Beneficiary.Contact.MiddleName != null ? x.Beneficiary.Contact.MiddleName[..1] : string.Empty,
               LastName = x.Beneficiary.Contact.LastName,
               YearsInPs = 0, // Use PayProfit for contribution years
               TerminationDate = x.Demographic.TerminationDate,
               IncomeRegAndExecCurrentYear = (x.PayProfit.CurrentIncomeYear ?? 0) + (x.PayProfit.IncomeExecutive),
               TerminationCode = x.Demographic.TerminationCodeId,
               ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
               Enrolled = x.PayProfit.EnrollmentId,
               Etva = x.PayProfit.EarningsEtvaValue,
               BeneficiaryAllocation = 0 // Adjust if needed
           });

       return demographicSlice
           .Union(beneficiarySlice)
           .OrderBy(m => m.FullName);
    }

    private async Task<List<Member>> MergeMemberSlicesToMembers(TerminatedEmployeeAndBeneficiaryDataRequest req,
        IOrderedQueryable<MemberSlice> memberSliceUnion)
    {
        var combinedCollection =
            from memberSlice in memberSliceUnion
            join profitDetail in _ctx.ProfitDetails
                on new { memberSlice.Ssn, req.ProfitYear } equals new { profitDetail.Ssn, profitDetail.ProfitYear } into profitDetailGroup
            from profitDetail in profitDetailGroup // Left join to include MemberSlices even if there's no matching ProfitDetail
            select new
            {
                memberSlice.Psn,
                memberSlice.Ssn,
                memberSlice.FullName,
                memberSlice.FirstName,
                memberSlice.LastName,
                memberSlice.MiddleInitial,
                memberSlice.BirthDate,
                memberSlice.HoursCurrentYear,
                memberSlice.NetBalanceLastYear,
                memberSlice.VestedBalanceLastYear,
                memberSlice.EmploymentStatusCode,
                memberSlice.TerminationDate,
                memberSlice.IncomeRegAndExecCurrentYear,
                memberSlice.BeneficiaryAllocation,
                memberSlice.TerminationCode,
                memberSlice.YearsInPs,
                memberSlice.ZeroCont,
                memberSlice.Enrolled,
                memberSlice.Etva,
                ProfitDetailForfeiture = profitDetail.Forfeiture,
                ProfitDetailProfitCode = profitDetail.ProfitCodeId,
            };

        var groupedMembers = await combinedCollection
            .GroupBy(m => m.Ssn) // Group by SSN to process all slices for each SSN
            .ToListAsync();

        var members = new List<Member>();

        foreach (var group in groupedMembers)
        {
            // Accumulate financial values for each SSN
            decimal netBalanceLastYear = 0;
            decimal vestedBalanceLastYear = 0;
            decimal beneficiaryAllocation = 0;

            foreach (var m in group)
            {
                netBalanceLastYear += m.NetBalanceLastYear;
                vestedBalanceLastYear += m.VestedBalanceLastYear;
                beneficiaryAllocation += m.BeneficiaryAllocation;
            }

            // Get ProfitDetailSummary for the current SSN
            profitDetailsForAll.Where(pd => pd.Ssn == ssn).ToList()
            ProfitDetailSummary ds = RetrieveProfitDetail(profitDetails, group.Key);
            beneficiaryAllocation += ds.BeneficiaryAllocation;

            // If the member has financial data, create a Member object
            if (netBalanceLastYear != 0 || ds.BeneficiaryAllocation != 0 || ds.Distribution != 0 || ds.Forfeiture != 0)
            {
                var firstSlice = group.First(); // Use the first slice for common member data

                var member = new Member
                {
                    Psn = firstSlice.Psn,
                    FullName = firstSlice.FullName,
                    FirstName = firstSlice.FirstName,
                    LastName = firstSlice.LastName,
                    MiddleInitial = firstSlice.MiddleInitial,
                    Birthday = firstSlice.BirthDate,
                    HoursCurrentYear = firstSlice.HoursCurrentYear,
                    EarningsCurrentYear = firstSlice.IncomeRegAndExecCurrentYear,
                    Ssn = firstSlice.Ssn,
                    TerminationDate = firstSlice.TerminationDate,
                    TerminationCode = firstSlice.TerminationCode,
                    BeginningAmount = netBalanceLastYear,
                    CurrentVestedAmount = vestedBalanceLastYear,
                    YearsInPlan = firstSlice.YearsInPs,
                    ZeroCont = firstSlice.ZeroCont,
                    Enrolled = firstSlice.Enrolled,
                    Evta = firstSlice.Etva,
                    BeneficiaryAllocation = beneficiaryAllocation,
                    DistributionAmount = ds.Distribution,
                    ForfeitAmount = ds.Forfeiture,
                    EndingBalance = netBalanceLastYear + ds.Forfeiture + ds.Distribution + beneficiaryAllocation,
                    VestedBalance = vestedBalanceLastYear + ds.Distribution + beneficiaryAllocation
                };

                members.Add(member);
            }
        }

        return members;
    }

    private TerminatedEmployeeAndBeneficiaryResponse CreateDataset(List<Member> members, TerminatedEmployeeAndBeneficiaryDataRequest req)
    {
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        List<TerminatedEmployeeAndBeneficiaryDataResponseDto> membersSummary = new();

        foreach (var ms in members)
        {
            int vestingPercent = LookupVestingPercent(ms.Enrolled, ms.ZeroCont, ms.YearsInPlan);

            byte enrolled = ms.Enrolled == 2 ? (byte)0 : ms.Enrolled;

            decimal vestedBalance = ms.VestedBalance;
            if (ms.ZeroCont == 6)
            {
                vestedBalance = ms.EndingBalance;
            }
            if (ms.EndingBalance == 0 && vestedBalance == 0)
            {
                vestingPercent = 0;
            }

            int? age = null;
            if (ms.Birthday.HasValue)
            {
                int birthYear = ms.Birthday.Value.Year;
                int birthMonth = ms.Birthday.Value.Month;
                int birthDay = ms.Birthday.Value.Day;
                age = _todaysDate.Year - birthYear;
                if (birthMonth > _todaysDate.Month)
                {
                    age--;
                }
                if (birthMonth == _todaysDate.Month && birthDay > _todaysDate.Day)
                {
                    age--;
                }
            }

            // If they have a contribution the plan and are past the 1st/2nd year for the old/new plan 
            // or have a beneficiary allocation then add them in.
            if (
                ((ms.Enrolled == Enrollment.Constants.NotEnrolled || ms.Enrolled == Enrollment.Constants.OldVestingPlanHasContributions || ms.Enrolled == Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
                 && ms.YearsInPlan > 2 && ms.BeginningAmount != 0)
                || ((ms.Enrolled == Enrollment.Constants.NewVestingPlanHasContributions || ms.Enrolled == Enrollment.Constants.NewVestingPlanHasForfeitureRecords) && ms.YearsInPlan > 1 && ms.BeginningAmount != 0)
                || (ms.BeneficiaryAllocation != 0)
            )
            {
                membersSummary.Add(new TerminatedEmployeeAndBeneficiaryDataResponseDto()
                {
                    BadgePSn = ms.Psn.ToString(),
                    Name = ms.FullName,
                    BeginningBalance = ms.BeginningAmount,
                    BeneficiaryAllocation = ms.BeneficiaryAllocation,
                    DistributionAmount = ms.DistributionAmount,
                    Forfeit = ms.ForfeitAmount,
                    EndingBalance = ms.EndingBalance,
                    VestedBalance = vestedBalance,
                    DateTerm = ms.TerminationDate,
                    YtdPsHours = ms.HoursCurrentYear,
                    VestedPercent = vestingPercent,
                    Age = age,
                    EnrollmentCode = enrolled
                });
                totalVested += vestedBalance;
                totalForfeit += ms.ForfeitAmount;
                totalEndingBalance += ms.EndingBalance;
                totalBeneficiaryAllocation += ms.BeneficiaryAllocation;
            }
        }

        if (req.Skip.HasValue)
        {
            membersSummary = membersSummary.Skip(req.Skip.Value).ToList();
        }

        if (req.Take.HasValue)
        {
            membersSummary = membersSummary.Take(req.Take.Value).ToList();
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

    private static int LookupVestingPercent(byte enrolled, byte? zeroCont, int yearsInPlan)
    {
        if (enrolled > Enrollment.Constants.NewVestingPlanHasContributions || zeroCont == ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested)
        {
            return 100;
        }
        int vestingYearIndex;
        if (enrolled < Enrollment.Constants.NewVestingPlanHasContributions)
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

        foreach (var profitDetail in profitDetails)
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
