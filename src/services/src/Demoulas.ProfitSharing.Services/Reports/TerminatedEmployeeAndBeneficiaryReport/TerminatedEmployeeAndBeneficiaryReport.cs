using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.OracleHcm;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Contexts;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.InternalDto;
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
    private readonly ContributionService _contributionService;

    public TerminatedEmployeeAndBeneficiaryReport(IProfitSharingDataContextFactory factory, ICalendarService calendarService, ContributionService contributionService)
    {
        _factory = factory;
        _calendarService = calendarService;
        _contributionService = contributionService;
    }

    public async Task<TerminatedEmployeeAndBeneficiaryResponse> CreateData(ProfitYearRequest req, CancellationToken cancellationToken)
    {
        return await _factory.UseReadOnlyContext(async ctx =>
        {
            IAsyncEnumerable<MemberSlice> memberSliceUnion = await RetrieveMemberSlices(ctx, req, cancellationToken);
            var fullResponse = await MergeAndCreateDataset(ctx, req, memberSliceUnion, cancellationToken);
            return fullResponse;
        });
    }

    #region Get Employees and Beneficiaries

    private async Task<IAsyncEnumerable<MemberSlice>> RetrieveMemberSlices(ProfitSharingReadOnlyDbContext ctx, ProfitYearRequest request,
        CancellationToken cancellationToken)
    {
        var terminatedEmployees = await GetTerminatedEmployees(ctx, request, cancellationToken);
        var terminatedWithContributions = await GetEmployeesWithContributions(ctx, request, terminatedEmployees, cancellationToken);
        var beneficiaries = GetBeneficiaries(ctx, request);
        return CombineEmployeeAndBeneficiarySlices(terminatedWithContributions, beneficiaries, request.Skip);
    }

    private async Task<IQueryable<TerminatedEmployeeDto>> GetTerminatedEmployees(ProfitSharingReadOnlyDbContext ctx, ProfitYearRequest request,
        CancellationToken cancellationToken)
    {
        var startEnd = await _calendarService.GetYearStartAndEndAccountingDates(request.ProfitYear, cancellationToken);

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

    private async Task<IQueryable<MemberSlice>> GetEmployeesWithContributions(ProfitSharingReadOnlyDbContext ctx, ProfitYearRequest request,
        IQueryable<TerminatedEmployeeDto> terminatedEmployees, CancellationToken cancellationToken)
    {
        var demKeyList = await terminatedEmployees.Select(e => new { e.Demographic.Id, e.Demographic.EmployeeId }).ToListAsync(cancellationToken);
        var idList = demKeyList.Select(e => e.Id).ToHashSet();
        var badgeNumbers = demKeyList.Select(e => e.EmployeeId).ToHashSet();

        var contributionYearsQuery = _contributionService.GetContributionYears(badgeNumbers);

        var validEnrollmentIds = GetValidEnrollmentIds();

        var payProfitsQuery = ctx.PayProfits
            .Where(p => p.ProfitYear == request.ProfitYear
                        && idList.Contains(p.DemographicId)
                        && validEnrollmentIds.Contains(p.EnrollmentId));

        var query = from employee in terminatedEmployees
            join contribution in contributionYearsQuery on employee.Demographic.EmployeeId equals contribution.BadgeNumber
            join payProfit in payProfitsQuery on employee.Demographic.Id equals payProfit.DemographicId
            select new MemberSlice
            {
                PsnSuffix = 0,
                BadgeNumber = employee.Demographic.EmployeeId,
                Ssn = employee.Demographic.Ssn,
                BirthDate = employee.Demographic.DateOfBirth,
                HoursCurrentYear = payProfit.CurrentHoursYear,
                EmploymentStatusCode = employee.Demographic.EmploymentStatusId,
                FullName = employee.Demographic.ContactInfo.FullName,
                FirstName = employee.Demographic.ContactInfo.FirstName,
                MiddleInitial = employee.Demographic.ContactInfo.MiddleName,
                LastName = employee.Demographic.ContactInfo.LastName,
                YearsInPs = contribution.YearsInPlan,
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

    private IQueryable<MemberSlice> GetBeneficiaries(ProfitSharingReadOnlyDbContext ctx, ProfitYearRequest request)
    {
        var validEnrollmentIds = GetValidEnrollmentIds();

        var query = ctx.Beneficiaries
            .Include(b => b.Contact)
            .Include(b => b.Demographic)
            .ThenInclude(d => d!.PayProfits.Where(p => p.ProfitYear == request.ProfitYear))
            .Where(b => b.Demographic != null)
            .Select(b => new { Beneficiary = b, Demographic = b.Demographic, PayProfit = b.Demographic!.PayProfits.FirstOrDefault() })
            .Where(x => x.PayProfit != null
                        && validEnrollmentIds.Contains(x.PayProfit.EnrollmentId))
            .Select(x => new MemberSlice
            {
                PsnSuffix = x.Beneficiary.PsnSuffix,
                BadgeNumber = x.Beneficiary!.EmployeeId,
                Ssn = x.Beneficiary.Contact!.Ssn,
                BirthDate = x.Beneficiary.Contact!.DateOfBirth,
                HoursCurrentYear = 0, // Placeholder logic for hours
                EmploymentStatusCode = x.Demographic!.EmploymentStatusId,
                FullName = x.Beneficiary.Contact!.ContactInfo.FullName!,
                FirstName = x.Beneficiary.Contact.ContactInfo.FirstName,
                MiddleInitial = x.Beneficiary.Contact.ContactInfo.MiddleName != null
                    ? x.Beneficiary.Contact.ContactInfo.MiddleName.Substring(0, 1)
                    : string.Empty,
                LastName = x.Beneficiary.Contact.ContactInfo.LastName,
                YearsInPs = 0,
                TerminationDate = null,
                IncomeRegAndExecCurrentYear = (x.PayProfit!.CurrentIncomeYear ) + x.PayProfit.IncomeExecutive,
                TerminationCode = x.Demographic.TerminationCodeId,
                ZeroCont = ZeroContributionReason.Constants.SixtyFiveAndOverFirstContributionMoreThan5YearsAgo100PercentVested,
                EnrollmentId = Enrollment.Constants.NotEnrolled,
                Etva = x.PayProfit.EarningsEtvaValue,
                BeneficiaryAllocation = x.Beneficiary.Amount
            });

        return query;
    }

    private static IAsyncEnumerable<MemberSlice> CombineEmployeeAndBeneficiarySlices(IQueryable<MemberSlice> terminatedWithContributions,
        IQueryable<MemberSlice> beneficiaries, int? skip)
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

    #endregion

    private async Task<TerminatedEmployeeAndBeneficiaryResponse> MergeAndCreateDataset(IProfitSharingDbContext ctx, ProfitYearRequest req,
        IAsyncEnumerable<MemberSlice> memberSliceUnion, CancellationToken cancellationToken)
    {
        decimal totalVested = 0;
        decimal totalForfeit = 0;
        decimal totalEndingBalance = 0;
        decimal totalBeneficiaryAllocation = 0;

        var membersSummary = new List<TerminatedEmployeeAndBeneficiaryDataResponseDto>();

        await foreach (var memberSlice in memberSliceUnion.WithCancellation(cancellationToken))
        {
            var profitDetails = await ctx.ProfitDetails.Where(pd => pd.ProfitYear <= req.ProfitYear && pd.Ssn == memberSlice.Ssn)
                .ToListAsync(cancellationToken);

            InternalProfitDetailDto profitDetailSummary = RetrieveProfitDetail(profitDetails, req.ProfitYear);

            int vestingPercent = _contributionService.LookupVestingPercent(memberSlice.EnrollmentId, memberSlice.ZeroCont, memberSlice.YearsInPs);

            var currentVestedAmount = _contributionService.CalculateCurrentVested(profitDetails, profitDetailSummary.CurrentAmount, vestingPercent);

            var beneficiaryAllocation = profitDetailSummary.BeneficiaryAllocation;

            if (profitDetailSummary is { CurrentAmount: 0, BeneficiaryAllocation: 0, Distribution: 0, TotalForfeitures: 0 })
            {
                continue;
            }

            var member = new Member
            {
                Psn = memberSlice.PsnSuffix > 0 ? $"{memberSlice.BadgeNumber}{memberSlice.PsnSuffix}" : memberSlice.BadgeNumber.ToString(),
                FullName = memberSlice.FullName,
                FirstName = memberSlice.FirstName,
                LastName = memberSlice.LastName,
                MiddleInitial = memberSlice.MiddleInitial?.Length > 1 ? memberSlice.MiddleInitial?[..1] : memberSlice.MiddleInitial,
                Birthday = memberSlice.BirthDate,
                HoursCurrentYear = memberSlice.HoursCurrentYear,
                EarningsCurrentYear = memberSlice.IncomeRegAndExecCurrentYear,
                Ssn = memberSlice.Ssn,
                TerminationDate = memberSlice.TerminationDate,
                TerminationCode = memberSlice.TerminationCode,
                BeginningAmount = profitDetailSummary.CurrentAmount,
                CurrentVestedAmount = currentVestedAmount,
                YearsInPlan = memberSlice.YearsInPs,
                ZeroCont = memberSlice.ZeroCont,
                EnrollmentId = memberSlice.EnrollmentId,
                Evta = memberSlice.Etva,
                BeneficiaryAllocation = beneficiaryAllocation,
                DistributionAmount = profitDetailSummary.Distribution,
                ForfeitAmount = profitDetailSummary.TotalForfeitures,
                EndingBalance =
                    profitDetailSummary.CurrentAmount + profitDetailSummary.TotalForfeitures + profitDetailSummary.Distribution + beneficiaryAllocation,
                VestedBalance = currentVestedAmount + profitDetailSummary.Distribution + beneficiaryAllocation
            };

            byte enrollmentId = member.EnrollmentId == Enrollment.Constants.NewVestingPlanHasContributions
                ? Enrollment.Constants.NotEnrolled
                : member.EnrollmentId;

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
                age = member.Birthday.Value.Age();
            }

            // If they have a contribution the plan and are past the 1st/2nd year for the old/new plan 
            // or have a beneficiary allocation then add them in.
            if (
                (member.EnrollmentId is (Enrollment.Constants.NotEnrolled or Enrollment.Constants.OldVestingPlanHasContributions or Enrollment.Constants.OldVestingPlanHasForfeitureRecords) && member.YearsInPlan > 2 && member.BeginningAmount != 0) 
                || (member.EnrollmentId is (Enrollment.Constants.NewVestingPlanHasContributions or Enrollment.Constants.NewVestingPlanHasForfeitureRecords) && member.YearsInPlan > 1 && member.BeginningAmount != 0) 
                || (member.BeneficiaryAllocation != 0)
            )
            {
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
                    VestedPercent = vestingPercent,
                    Age = age,
                    EnrollmentCode = enrollmentId
                });

                totalVested += vestedBalance;
                totalForfeit += member.ForfeitAmount;
                totalEndingBalance += member.EndingBalance;
                totalBeneficiaryAllocation += member.BeneficiaryAllocation;
            }

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
                Results = membersSummary, Total = membersSummary.Count
            }
        };
    }

    private static InternalProfitDetailDto RetrieveProfitDetail(List<ProfitDetail> profitDetails, short profitYear)
    {
        if (profitDetails.Count == 0)
        {
            return new InternalProfitDetailDto();
        }

#pragma warning disable S1481
        var currentBalance = profitDetails
            .GroupBy(details => details.Ssn)
            .Select(g => new
            {
                TotalContributions = g.Sum(x => x.Contribution),
                TotalEarnings = g.Sum(x => x.Earnings),
                TotalForfeitures = g.Sum(x => x.ProfitCodeId == ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
                TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
            })
            .Select(r =>  r.TotalContributions + r.TotalEarnings + r.TotalForfeitures - r.TotalPayments).First();


        
#pragma warning disable S3358

        if (!profitDetails.Exists(pd => pd.ProfitYear == profitYear))
        {
           return new InternalProfitDetailDto { CurrentAmount = currentBalance };
        }


        var pdQuery = profitDetails
            .Where(pd => pd.ProfitYear == profitYear)
            .GroupBy(details => details.Ssn)
            .Select(g => new
            {
                TotalContributions = g.Sum(x => x.Contribution),
                TotalEarnings = g.Sum(x => x.Earnings),
                TotalForfeitures = g.Sum(x =>
                    x.ProfitCodeId == ProfitCode.Constants.IncomingContributions ? x.Forfeiture
                        : (x.ProfitCodeId == ProfitCode.Constants.OutgoingForfeitures ? -x.Forfeiture : 0)),
                TotalPayments = g.Sum(x => x.ProfitCodeId != ProfitCode.Constants.IncomingContributions ? x.Forfeiture : 0),
                Distribution = g.Sum(x =>
                    (x.ProfitCodeId == ProfitCode.Constants.OutgoingPaymentsPartialWithdrawal ||
                     x.ProfitCodeId == ProfitCode.Constants.OutgoingDirectPayments || x.ProfitCodeId == ProfitCode.Constants.Outgoing100PercentVestedPayment)
                        ? -x.Forfeiture
                        : 0),
                BeneficiaryAllocation = g.Sum(x => (x.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary)
                    ? -x.Forfeiture
                    : (x.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary)
                        ? x.Contribution
                        : 0)
            })
            .Select(r => new InternalProfitDetailDto
            {
                TotalContributions = r.TotalContributions,
                TotalEarnings = r.TotalEarnings,
                TotalForfeitures = r.TotalForfeitures,
                TotalPayments = r.TotalPayments,
                CurrentAmount = currentBalance,
                Distribution = r.Distribution,
                BeneficiaryAllocation = r.BeneficiaryAllocation
            }).First();
#pragma warning restore S3358
#pragma warning restore S1481


        return pdQuery;
    }
}

