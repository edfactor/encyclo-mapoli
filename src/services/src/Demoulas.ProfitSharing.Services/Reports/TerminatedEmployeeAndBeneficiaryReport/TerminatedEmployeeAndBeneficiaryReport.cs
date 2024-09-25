using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Endpoints.Endpoints.Reports.YearEnd.TerminatedEmployeeAndBeneficiary;
using Microsoft.Extensions.Logging;

namespace Demoulas.ProfitSharing.Services.Reports.TerminatedEmployeeAndBeneficiaryReport;

/// <summary>
/// Reports on both employees which where terminated in the time range specified (but not retired.), and all beneficiaries.   
/// </summary>
public class TerminatedEmployeeAndBeneficiaryReport(ILogger _logger, ProfitSharingDbContext _ctx, DateOnly? effectiveRunDate = null)
{
    // These should move to a Reference Location.
    // Vesting Schedule Percent by year
    private static readonly List<int> _olderVestingSchedule = [0, 0, 20, 40, 60, 80, 100];
    private static readonly List<int> _newerVestingSchedule = [0, 20, 40, 60, 80, 100];

    // input parameters
    private DateOnly _startDate;
    private DateOnly _endDate;
    private decimal _profitSharingYearWithIteration;

    // This is usually Today's date, however when tests are running, a specific date can be used.
    private DateOnly _effectiveRunDate;


    public string CreateReport(DateOnly startDate, DateOnly endDate, decimal profitSharingYear)
    {
        this._startDate = startDate;
        this._endDate = endDate;
        this._profitSharingYearWithIteration = profitSharingYear;
        this._effectiveRunDate = effectiveRunDate ?? DateOnly.FromDateTime(DateTime.Now);

        // If the user supplies 9999.9, then figure out which profit sharing year based on today's date (_effectiveRunDate)
        if (_profitSharingYearWithIteration == 9999.9m)
        {
            _profitSharingYearWithIteration = (_effectiveRunDate.Month < 4) ? _effectiveRunDate.Year - 1 : _effectiveRunDate.Year;
        }

        List<MemberSlice> memberSlices = RetrieveMemberSlices();
        List<Member> members = MergeMemberSlicesToMembers(memberSlices);
        string report = CreateReport(members);
        return report;
    }

    public TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto> CreateData(DateOnly startDate, DateOnly endDate, decimal profitSharingYear)
    {
        this._startDate = startDate;
        this._endDate = endDate;
        this._profitSharingYearWithIteration = profitSharingYear;
        this._effectiveRunDate = effectiveRunDate ?? DateOnly.FromDateTime(DateTime.Now);

        // If the user supplies 9999.9, then figure out which profit sharing year based on today's date (_effectiveRunDate)
        if (_profitSharingYearWithIteration == 9999.9m)
        {
            _profitSharingYearWithIteration = (_effectiveRunDate.Month < 4) ? _effectiveRunDate.Year - 1 : _effectiveRunDate.Year;
        }

        List<MemberSlice> memberSlices = RetrieveMemberSlices();
        List<Member> members = MergeMemberSlicesToMembers(memberSlices);

        return CreateData(members);
    }

    private List<MemberSlice> RetrieveMemberSlices()
    {
        // slices of member information (aka employee or beneficiary information)
        List<MemberSlice> memberSlices = new();

        var employees = _ctx!.Demographics.Where(d =>
                d.EmploymentStatusId == EmploymentStatus.Constants.Terminated &&
                d.TerminationCodeId != TerminationCode.Constants.RetiredReceivingPension &&
                d.TerminationDate >= _startDate && d.TerminationDate <= _endDate
                )
            .ToList();
        foreach (var employee in employees)
        {
            List<PayProfit> payProfits = _ctx.PayProfits.Where(pp => pp.BadgeNumber == employee.BadgeNumber).ToList();
            if (payProfits.Count != 1)
            {
                _logger.LogError("Employee {BadgeNumber} does not have a single pay_profit row.", employee.BadgeNumber);
                continue;
            }
            PayProfit pp = payProfits[0];

            byte zeroCont = (employee.TerminationCodeId == 'Z') ?
                ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested // 6
            : pp.ZeroContributionReasonId ?? 0;

            MemberSlice memberSlice = new MemberSlice(
                Psn: employee.BadgeNumber,
                Ssn: employee.Ssn,
                BirthDate: employee.DateOfBirth,
                HoursCurrentYear: 0, // hours
                NetBalanceLastYear: pp.NetBalanceLastYear,
                VestedBalanceLastYear: pp.VestedBalanceLastYear,
                EmploymentStatusCode: employee.EmploymentStatusId,
                FullName: employee.FullName!,
                FirstName: employee.FirstName,
                MiddleInitial: employee.MiddleName?.Substring(0, 1) ?? "",
                LastName: employee.LastName,
                YearsInPs: pp.CompanyContributionYears,
                TerminationDate: employee.TerminationDate,
                IncomeRegAndExecCurrentYear: (pp.IncomeCurrentYear ?? 0) + pp.IncomeExecutive,
                TerminationCode: employee.TerminationCodeId,
                ZeroCont: zeroCont,
                Enrolled: pp.EnrollmentId,
                Etva: pp.EarningsEtvaValue,
                BeneficiaryAllocation: 0);

            memberSlices.Add(memberSlice);

        }

        foreach (var beneficiary in _ctx.Beneficiaries.ToList())
        {
            char statusCode = 'T';
            DateOnly? terminationDate = null;
            decimal amount = beneficiary.Amount;
            long psn = beneficiary.Psn;

            // See if beneficiary is also an employee.
            var results = _ctx.Demographics.Where(demographic => demographic.Ssn == beneficiary.Ssn)
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
                                .ToList();
            if (results.Count == 1)
            {
                var result = results[0];
                terminationDate = result.Demographic.TerminationDate;
                if (terminationDate >= _startDate && terminationDate <= _endDate)
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
            MemberSlice memberSlice = new MemberSlice(
                Psn: psn,
                Ssn: beneficiary.Ssn,
                BirthDate: beneficiary.DateOfBirth,
                HoursCurrentYear: 0,
                NetBalanceLastYear: amount,
                VestedBalanceLastYear: amount,
                EmploymentStatusCode: statusCode,
                FullName: GetFullName(beneficiary),
                FirstName: beneficiary.FirstName,
                MiddleInitial: beneficiary.MiddleName!,
                LastName: beneficiary.LastName,
                YearsInPs: 10,
                TerminationDate: terminationDate,
                IncomeRegAndExecCurrentYear: 0,
                TerminationCode: null,
                ZeroCont: 6,
                Enrolled: 0,
                Etva: amount,
                BeneficiaryAllocation: 0);
            memberSlices.Add(memberSlice);
        }

        return memberSlices;
    }

    private List<Member> MergeMemberSlicesToMembers(List<MemberSlice> memberSlices)
    {
        memberSlices.Sort((a, b) => StringComparer.Ordinal.Compare(a.FullName, b.FullName));
        long ssn = long.MinValue;

        decimal netBalanceLastYear = 0;
        decimal vestedBalanceLastYear = 0;
        decimal forfeiture = 0;
        decimal distribution = 0;
        decimal beneficiaryAllocation = 0;

        List<Member> members = [];
        Member? ms = null;

        foreach (var m in memberSlices)
        {
            if (((m.Ssn != ssn && ssn != long.MinValue) ||
                (ssn == long.MaxValue)) && (m != memberSlices[^1]))
            {
                ProfitDetailSummary ds = RetrieveProfitDetail(ssn);
                distribution += ds.Distribution;
                forfeiture += ds.Forfeiture;
                beneficiaryAllocation += ds.BeneficiaryAllocation;

                if (netBalanceLastYear != 0 || ds.BeneficiaryAllocation != 0 || ds.Distribution != 0 || forfeiture != 0)
                {
                    ms = ms! with
                    {
                        EndingBalance = netBalanceLastYear + forfeiture + ds.Distribution + beneficiaryAllocation,
                        DistributionAmount = distribution,
                        ForfeitAmount = forfeiture,
                        VestedBalance = vestedBalanceLastYear + distribution + beneficiaryAllocation,
                        BeneficiaryAllocation = beneficiaryAllocation
                    };
                    members.Add(ms);
                }
                vestedBalanceLastYear = 0;
                forfeiture = 0;
                distribution = 0;
                netBalanceLastYear = 0;
                beneficiaryAllocation = 0;
            }
            if (ssn == long.MaxValue)
            {
                break;
            }
            netBalanceLastYear += m.NetBalanceLastYear;
            vestedBalanceLastYear += m.VestedBalanceLastYear;
            beneficiaryAllocation += m.BeneficiaryAllocation;
            ssn = m.Ssn;

            ms = new Member(
                Psn: m.Psn,
                FullName: m.FullName,
                FirstName: m.FirstName,
                LastName: m.LastName,
                MiddleInitial: m.MiddleInitial,
                Birthday: m.BirthDate,
                HoursCurrentYear: m.HoursCurrentYear,
                EarningsCurrentYear: m.IncomeRegAndExecCurrentYear,
                Ssn: m.Ssn,
                TerminationDate: m.TerminationDate,
                TerminationCode: m.TerminationCode,
                BeginningAmount: netBalanceLastYear,
                CurrentVestedAmount: vestedBalanceLastYear,
                YearsInPlan: m.YearsInPs,
                ZeroCont: m.ZeroCont,
                Enrolled: m.Enrolled,
                Evta: m.Etva,
                BeneficiaryAllocation: beneficiaryAllocation,
                DistributionAmount: 0,
                ForfeitAmount: 0,
                EndingBalance: 0,
                VestedBalance: 0);
        }

        return members;
    }

    private string CreateReport(List<Member> members)
    {
        TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto> reportData = CreateData(members);

        TextReportGenerator textReportGenerator = new TextReportGenerator(_effectiveRunDate, _startDate, _endDate, _profitSharingYearWithIteration);

        foreach (var ms in reportData.Response.Results)
        {

            textReportGenerator.PrintDetails(ms.BadgePSn, ms.Name, ms.BeginningBalance,
                ms.BeneficiaryAllocation, ms.DistributionAmount, ms.Forfeit,
                ms.EndingBalance, ms.VestedBalance, ms.DateTerm, ms.YtdPsHours, ms.VestedPercent, ms.Age,
                ms.EnrollmentCode ?? 0);
        }
        textReportGenerator.PrintTotals(reportData.TotalEndingBalance, reportData.TotalVested, reportData.TotalForfeit, reportData.TotalBeneficiaryAllocation);
        return textReportGenerator.GetReport();

    }

    private TerminatedEmployeeAndBeneficiaryDataResponse<TerminatedEmployeeAndBeneficiaryDataResponseDto> CreateData(List<Member> members)
    {
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        List<TerminatedEmployeeAndBeneficiaryDataResponseDto> membersSummary = new();

        foreach (var ms in members)
        {
            int vestingPercent;
            if (ms.Enrolled > 2 || ms.ZeroCont == 6)
            {
                vestingPercent = 100;
            }
            else
            {
                int vestingYearIndex;
                if (ms.Enrolled < 2)
                {
                    if (ms.YearsInPlan <= 1)
                    {
                        vestingYearIndex = 1;
                    }
                    else
                    {
                        if (ms.YearsInPlan > 6)
                        {
                            vestingYearIndex = 7;
                        }
                        else
                        {
                            vestingYearIndex = (int)ms.YearsInPlan;
                        }
                    }
                    vestingPercent = _olderVestingSchedule[vestingYearIndex - 1];
                }
                else
                {
                    if (ms.YearsInPlan <= 1)
                    {
                        vestingYearIndex = 1;
                    }
                    else
                    {
                        if (ms.YearsInPlan > 5)
                        {
                            vestingYearIndex = 6;
                        }
                        else
                        {
                            vestingYearIndex = (int)ms.YearsInPlan;
                        }
                    }
                    vestingPercent = _newerVestingSchedule[vestingYearIndex - 1];
                }
            }

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
                age = _effectiveRunDate.Year - birthYear;
                if (birthMonth > _effectiveRunDate.Month)
                {
                    age--;
                }
                if (birthMonth == _effectiveRunDate.Month && birthDay > _effectiveRunDate.Day)
                {
                    age--;
                }
            }

            if (
                ((ms.Enrolled == 0 || ms.Enrolled == 1 || ms.Enrolled == 3) && ms.YearsInPlan > 2 && ms.BeginningAmount != 0)
                || ((ms.Enrolled == 2 || ms.Enrolled == 4) && ms.YearsInPlan > 1 && ms.BeginningAmount != 0)
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
            ReportName = "Terminated Employee and Beneficiary Report",
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


    private ProfitDetailSummary RetrieveProfitDetail(long ssn)
    {
        // This does seem odd, and possibly a bug.   We ask the user for the profit sharing year with a decimal.
        // but then we ignore the decimal part when querying records, but display the full value when printing the report.
        long profitSharingYearOnly = (long)Math.Truncate(_profitSharingYearWithIteration);

        // Note that pd.profitYear is a decimal, aka 2021.2 - and we constrain on only the year portion
        List<ProfitDetail> profitDetails = _ctx.ProfitDetails
                .Where(pd => pd.ProfitYear >= profitSharingYearOnly && pd.ProfitYear < (profitSharingYearOnly + 1) && pd.Ssn == ssn)
                .ToList();

        if (profitDetails.Count == 0)
        {
            return new ProfitDetailSummary(0, 0, 0);
        }

        decimal distribution = 0;
        decimal forfeiture = 0;
        decimal beneficiaryAllocation = 0;

        foreach (var profitDetail in profitDetails)
        {
            if (profitDetail.ProfitCodeId == 1 || profitDetail.ProfitCodeId == 3)
            {
                distribution = distribution - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == 2)
            {
                forfeiture = forfeiture - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == 0)
            {
                forfeiture = forfeiture + profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == 9)
            {
                distribution = distribution - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == 5)
            {
                beneficiaryAllocation = beneficiaryAllocation - profitDetail.Forfeiture;
            }

            if (profitDetail.ProfitCodeId == 6)
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
