using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
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

    public async Task<TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>> CreateData(
        DateOnly startDate, DateOnly endDate, decimal profitSharingYear)
    {
        // If the used the AutoSelectYear option
        if (profitSharingYear == ReferenceData.AutoSelectYear)
        {
            // Then if it is 
            profitSharingYear = (_todaysDate.Month < 4) ? _todaysDate.Year - 1 : _todaysDate.Year;
        }

        List<MemberSlice> memberSlices = await RetrieveMemberSlices(startDate, endDate);
        List<Member> members = await MergeMemberSlicesToMembers(memberSlices, profitSharingYear);
        return CreateDataset(members);
    }


    private async Task<List<MemberSlice>> RetrieveMemberSlices(DateOnly startDate, DateOnly endDate)
    {
        // slices of member information (aka employee or beneficiary information)
        List<MemberSlice> memberSlices = new();

        var employeesWithPayProfits = await _ctx.Demographics
            .Where(d => d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                        d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension &&
                        d.TerminationDate >= startDate && d.TerminationDate <= endDate)
            .GroupJoin(
                _ctx.PayProfits,
                demographic => demographic.BadgeNumber,
                payProfit => payProfit.BadgeNumber,
                (demographic, payProfits) => new
                {
                    Demographic = demographic,
                    PayProfits = payProfits
                })
            .ToListAsync();

        foreach (var employeeWithPahProfits in employeesWithPayProfits)
        {
            var employee = employeeWithPahProfits.Demographic;
            IEnumerable<PayProfit> payProfits = employeeWithPahProfits.PayProfits;
            if (payProfits.Count() != 1)
            {
                _logger.LogError("Employee {BadgeNumber} does not have a single pay_profit row.", employee.BadgeNumber);
                continue;
            }
            PayProfit pp = payProfits.First();

            byte zeroCont = (employee.TerminationCodeId == TerminationCode.Constants.Deceased) ?
                ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested // 6
            : pp.ZeroContributionReasonId ?? 0;

            MemberSlice memberSlice = new MemberSlice
            {
                Psn = employee.BadgeNumber,
                Ssn = employee.Ssn,
                BirthDate = employee.DateOfBirth,
                HoursCurrentYear = 0, // hours
                NetBalanceLastYear = pp.NetBalanceLastYear,
                VestedBalanceLastYear = pp.VestedBalanceLastYear,
                EmploymentStatusCode = employee.EmploymentStatusId,
                FullName = employee.FullName!,
                FirstName = employee.FirstName,
                MiddleInitial = employee.MiddleName?.Substring(0, 1) ?? "",
                LastName = employee.LastName,
                YearsInPs = pp.CompanyContributionYears,
                TerminationDate = employee.TerminationDate,
                IncomeRegAndExecCurrentYear = (pp.IncomeCurrentYear ?? 0) + pp.IncomeExecutive,
                TerminationCode = employee.TerminationCodeId,
                ZeroCont = zeroCont,
                Enrolled = pp.EnrollmentId,
                Etva = pp.EarningsEtvaValue,
                BeneficiaryAllocation = 0
            };

            memberSlices.Add(memberSlice);

        }

        foreach (var beneficiary in await _ctx.Beneficiaries.ToListAsync())
        {
            char statusCode = 'T';
            DateOnly? terminationDate = null;
            decimal amount = beneficiary.Amount;
            long psn = beneficiary.Psn;

            // See if beneficiary is also an employee.
            var results = await _ctx.Demographics.Where(demographic => demographic.Ssn == beneficiary.Ssn)
                                .GroupJoin(
                                    _ctx.PayProfits,
                                    demographic => demographic.Ssn,
                                    payProfit => payProfit.Ssn,
                                    (demographic, payProfits) => new { demographic, payProfits })
                                .SelectMany(
                                    x => x.payProfits.DefaultIfEmpty(),
                                    (x, payProfit) => new
                                    {
                                        Demographic = x.demographic,
                                        PayProfit = payProfit
                                    })
                                .ToListAsync();
            if (results.Count == 1)
            {
                var result = results[0];
                terminationDate = result.Demographic.TerminationDate;
                if (terminationDate >= startDate && terminationDate <= endDate)
                {
                    // if the termination date is out of range
                    continue;
                }
                if (amount == 0)
                {
                    amount = result.PayProfit?.NetBalanceLastYear ?? 0;
                }
                psn = result.Demographic.BadgeNumber;
                terminationDate = result.Demographic.TerminationDate;
                statusCode = result.Demographic.EmploymentStatusId;
            }
            MemberSlice memberSlice = new MemberSlice
            {
                Psn = psn,
                Ssn = beneficiary.Ssn,
                BirthDate = beneficiary.DateOfBirth,
                HoursCurrentYear = 0,
                NetBalanceLastYear = amount,
                VestedBalanceLastYear = amount,
                EmploymentStatusCode = statusCode,
                FullName = GetFullName(beneficiary),
                FirstName = beneficiary.FirstName,
                MiddleInitial = beneficiary.MiddleName!,
                LastName = beneficiary.LastName,
                YearsInPs = 10,
                TerminationDate = terminationDate,
                IncomeRegAndExecCurrentYear = 0,
                TerminationCode = null,
                ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                Enrolled = 0,
                Etva = amount,
                BeneficiaryAllocation = 0
            };
            memberSlices.Add(memberSlice);
        }

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

        foreach (var m in memberSlices)
        {
            // if the ssn has changed,
            if (((m.Ssn != ssn && ssn != long.MinValue) ||
                (ssn == long.MaxValue)) && (m != memberSlices[^1]))
            {
                // then merge the slices together

                // Get this year's transactions and merge in those amounts
                ProfitDetailSummary ds = await RetrieveProfitDetail(ssn, profitSharingYearWithIteration);
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

    private TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto> CreateDataset(List<Member> members)
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

        return new TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto>
        {
            ReportDate = DateTimeOffset.Now,
            TotalVested = totalVested,
            TotalForfeit = totalForfeit,
            TotalEndingBalance = totalEndingBalance,
            TotalBeneficiaryAllocation = totalBeneficiaryAllocation,
            Response = new PaginatedResponseDto<TerminatedEmployeeAndBeneficiaryDataResponseDto>()
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


    private async Task<ProfitDetailSummary> RetrieveProfitDetail(long ssn, decimal profitSharingYearWithIteration)
    {
        // This does seem odd, and possibly a bug?   We ask the user for the profit sharing year with a decimal.
        // but then we ignore the decimal part when querying records, but display the full value when printing the report.
        long profitSharingYearOnly = (long)Math.Truncate(profitSharingYearWithIteration);

        // Note that pd.profitYear is a decimal, aka 2021.2 - and we constrain on only the year portion
        List<ProfitDetail> profitDetails = await _ctx.ProfitDetails
                .Where(pd => pd.ProfitYear >= profitSharingYearOnly && pd.ProfitYear < (profitSharingYearOnly + 1) && pd.Ssn == ssn)
                .ToListAsync();

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

    private static string GetFullName(Beneficiary b)
    {
        return $"{b.LastName}, {b.FirstName} {b.MiddleName}";
    }
}
