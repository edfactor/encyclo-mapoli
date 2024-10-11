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
public class TerminatedEmployeeAndBeneficiaryReport
{
    private readonly ILogger _logger;
    private readonly ProfitSharingReadOnlyDbContext _ctx;

    // This is usually Today's date, however when tests are running, a specific date can be used.
    private readonly DateOnly _todaysDate;

    public TerminatedEmployeeAndBeneficiaryReport(ILogger logger, ProfitSharingReadOnlyDbContext ctx, DateOnly todaysDate)
    {
        _logger = logger;
        _ctx = ctx;
        _todaysDate = todaysDate;
    }

    public async Task<TerminatedEmployeeAndBeneficiaryResponse> CreateData(TerminatedEmployeeAndBeneficiaryDataRequest req)
    {
     
        List<MemberSlice> memberSlices = await RetrieveMemberSlices(req, CancellationToken.None);
        List<Member> members = await MergeMemberSlicesToMembers(memberSlices, req.ProfitYear);
        TerminatedEmployeeAndBeneficiaryResponse fullResponse =  CreateDataset(members, req);
        return fullResponse;
    }


    private async Task<PaginatedResponseDto<MemberSlice>> RetrieveMemberSlices(TerminatedEmployeeAndBeneficiaryDataRequest request, CancellationToken cancellationToken)
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
               Demographic = b.Demographic,
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

       var memberSlices = await demographicSlice.Union(beneficiarySlice).ToPaginationResultsAsync(request, cancellationToken);

        return memberSlices;
    }

    private async Task<List<Member>> MergeMemberSlicesToMembers(List<MemberSlice> memberSlices, decimal profitSharingYearWithIteration)
    {
        // Order records by FullName
        memberSlices.Sort((a, b) => StringComparer.Ordinal.Compare(a.FullName, b.FullName));
        long ssn = long.MinValue;

        // These values are merged 
        decimal netBalanceLastYear = 0;
        decimal vestedBalanceLastYear = 0;
        decimal beneficiaryAllocation = 0;

        List<Member> members = [];
        Member? ms = null;

        // This does seem odd, and possibly a bug?   We ask the user for the profit sharing year with a decimal.
        // but then we ignore the decimal part when querying records, but display the full value when printing the report.
        long profitSharingYearOnly = (long)Math.Truncate(profitSharingYearWithIteration);

        var memberSsns = memberSlices.Select(ms => ms.Ssn).Distinct().ToList();
        var profitDetails = await _ctx.ProfitDetails.Where(pd => memberSsns.Contains(pd.Ssn) &&
                                                                 pd.ProfitYear >= profitSharingYearOnly &&
                                                                 pd.ProfitYear < (profitSharingYearOnly + 1)).ToListAsync();

        foreach (var m in memberSlices)
        {
            // if the ssn has changed,
            if (((m.Ssn != ssn && ssn != long.MinValue) ||
                (ssn == long.MaxValue)) && (m != memberSlices[^1]))
            {
                // then merge the slices together

                // Get this year's transactions and merge in those amounts
                ProfitDetailSummary ds = RetrieveProfitDetail(profitDetails, ssn);
                beneficiaryAllocation += ds.BeneficiaryAllocation;

                // If member has money (otherwise we skip them)
                if (netBalanceLastYear != 0 || ds.BeneficiaryAllocation != 0 || ds.Distribution != 0 || ds.Forfeiture != 0)
                {
                    ms = ms! with
                    {
                        EndingBalance = netBalanceLastYear + ds.Forfeiture + ds.Distribution + beneficiaryAllocation,
                        DistributionAmount = ds.Distribution,
                        ForfeitAmount = ds.Forfeiture,
                        VestedBalance = vestedBalanceLastYear + ds.Distribution + beneficiaryAllocation,
                        BeneficiaryAllocation = beneficiaryAllocation
                    };
                    members.Add(ms);
                }
                netBalanceLastYear = 0;
                vestedBalanceLastYear = 0;
                beneficiaryAllocation = 0;
            }
            // This indicates we have processed the last record.
            if (ssn == long.MaxValue)
            {
                break;
            }
            netBalanceLastYear += m.NetBalanceLastYear;
            vestedBalanceLastYear += m.VestedBalanceLastYear;
            beneficiaryAllocation += m.BeneficiaryAllocation;
            ssn = m.Ssn;

            ms = new Member
            {
                Psn = m.Psn,
                FullName = m.FullName,
                FirstName = m.FirstName,
                LastName = m.LastName,
                MiddleInitial = m.MiddleInitial,
                Birthday = m.BirthDate,
                HoursCurrentYear = m.HoursCurrentYear,
                EarningsCurrentYear = m.IncomeRegAndExecCurrentYear,
                Ssn = m.Ssn,
                TerminationDate = m.TerminationDate,
                TerminationCode = m.TerminationCode,
                BeginningAmount = netBalanceLastYear,
                CurrentVestedAmount = vestedBalanceLastYear,
                YearsInPlan = m.YearsInPs,
                ZeroCont = m.ZeroCont,
                Enrolled = m.Enrolled,
                Evta = m.Etva,
                BeneficiaryAllocation = beneficiaryAllocation,
                DistributionAmount = 0,
                ForfeitAmount = 0,
                EndingBalance = 0,
                VestedBalance = 0
            };
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
                distribution = distribution - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures)
            {
                forfeiture = forfeiture - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.IncomingContributions)
            {
                forfeiture = forfeiture + profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment)
            {
                distribution = distribution - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
            {
                beneficiaryAllocation = beneficiaryAllocation - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary)
            {
                beneficiaryAllocation = beneficiaryAllocation + profitDetail.Contribution;
            }
        }
        return new ProfitDetailSummary(distribution, forfeiture, beneficiaryAllocation);
    }
}
