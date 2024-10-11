using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Extensions;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Reports on both employees which where terminated in the time range specified (but not retired.), and all beneficiaries.
/// </summary>
public sealed class TerminatedEmployeeAndBeneficiaryReport
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

        var demographicSlice = _ctx.Demographics
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
                MiddleInitial = d.Demographic.ContactInfo.MiddleName != null ? d.Demographic.ContactInfo.MiddleName.Substring(0, 1) : string.Empty,
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
            .Include(b => b.Contact)
            .Include(b => b.Demographic)
            .ThenInclude(d => d!.PayProfits.Where(p=> p.ProfitYear == request.ProfitYear))
            .Where(b => b.Demographic != null) // Ensure there is a related Demographic
            .Select(b => new
            {
                Beneficiary = b,
                b.Demographic,
                PayProfit = b.Demographic!.PayProfits.FirstOrDefault() // Select the PayProfit for the specified profit year since there's only one record, fetch the first
            })
            .Where(x => x.Demographic != null && x.PayProfit != null) // Filter out invalid data
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
                BeneficiaryAllocation = 0 // Adjust if needed
            });

        return demographicSlice.Union(beneficiarySlice)
            .OrderBy(m => m.FullName);
    }

    private async Task<List<Member>> MergeMemberSlicesToMembers(TerminatedEmployeeAndBeneficiaryDataRequest req,
        IQueryable<MemberSlice> memberSliceUnion)
    {
        var combinedCollection =
            from memberSlice in memberSliceUnion
            join profitDetail in _ctx.ProfitDetails
                on new { memberSlice.Ssn, req.ProfitYear } equals new { profitDetail.Ssn, profitDetail.ProfitYear } into profitDetailGroup
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
                ProfitDetails = profitDetailGroup.ToList() // Accumulate the collection of ProfitDetails
            };

        // Group by SSN, now profit details will be in a collection
        var groupedMembers = await combinedCollection.ToListAsync();

        var members = new List<Member>();

        foreach (var group in groupedMembers.AsReadOnly())
        {
            var beneficiaryAllocation = group.BeneficiaryAllocation;

            // Get ProfitDetailSummary for the current SSN
            ProfitDetailSummary ds = RetrieveProfitDetail(group.ProfitDetails, group.Ssn);
            beneficiaryAllocation += ds.BeneficiaryAllocation;

            // If the member has financial data, create a Member object
            if (group.NetBalanceLastYear != 0 || ds.BeneficiaryAllocation != 0 || ds.Distribution != 0 || ds.Forfeiture != 0)
            {
                var member = new Member
                {
                    Psn = group.Psn,
                    FullName = group.FullName,
                    FirstName = group.FirstName,
                    LastName = group.LastName,
                    MiddleInitial = group.MiddleInitial,
                    Birthday = group.BirthDate,
                    HoursCurrentYear = group.HoursCurrentYear,
                    EarningsCurrentYear = group.IncomeRegAndExecCurrentYear,
                    Ssn = group.Ssn,
                    TerminationDate = group.TerminationDate,
                    TerminationCode = group.TerminationCode,
                    BeginningAmount = group.NetBalanceLastYear,
                    CurrentVestedAmount = group.VestedBalanceLastYear,
                    YearsInPlan = group.YearsInPs,
                    ZeroCont = group.ZeroCont,
                    Enrolled = group.Enrolled,
                    Evta = group.Etva,
                    BeneficiaryAllocation = beneficiaryAllocation,
                    DistributionAmount = ds.Distribution,
                    ForfeitAmount = ds.Forfeiture,
                    EndingBalance = group.NetBalanceLastYear + ds.Forfeiture + ds.Distribution + beneficiaryAllocation,
                    VestedBalance = group.VestedBalanceLastYear + ds.Distribution + beneficiaryAllocation
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

#pragma warning disable S6562
        var forBirthDate = new DateTime(req.ProfitYear, DateTime.Now.Month, DateTime.Now.Day);
#pragma warning restore S6562

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
                age = ms.Birthday.Value.ToDateTime(TimeOnly.MinValue).Age(forBirthDate);
            }

            // If they have a contribution the plan and are past the 1st/2nd year for the old/new plan 
            // or have a beneficiary allocation then add them in.
            if (
                (ms.Enrolled is (Enrollment.Constants.NotEnrolled or Enrollment.Constants.OldVestingPlanHasContributions
                     or Enrollment.Constants.OldVestingPlanHasForfeitureRecords)
                 && ms.YearsInPlan > 2 && ms.BeginningAmount != 0)
                || (ms.Enrolled is (Enrollment.Constants.NewVestingPlanHasContributions or Enrollment.Constants.NewVestingPlanHasForfeitureRecords) &&
                    ms.YearsInPlan > 1 && ms.BeginningAmount != 0)
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
