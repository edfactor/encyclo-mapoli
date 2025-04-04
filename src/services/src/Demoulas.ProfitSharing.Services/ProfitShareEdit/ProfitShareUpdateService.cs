using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

/// <summary>
///     Does the Year And application of Earnings and Contributions to all employees and beneficiaries.
///     Modeled very closely after Pay444
///     This class follows the name of the step in the Ready YE flow.    It could instead be named "View effect of YE
///     update on members"
/// </summary>
internal sealed class ProfitShareUpdateService : IInternalProfitShareUpdateService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly TotalService _totalService;

    public ProfitShareUpdateService(IProfitSharingDataContextFactory dbContextFactory,
        TotalService totalService,
        ICalendarService calendarService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
    }

    public async Task<ProfitShareUpdateResponse> ProfitShareUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, AdjustmentsSummaryDto adjustmentReportData, TotalsDto totalsDto, bool employeeExceededMaxContribution) =
            await ProfitSharingUpdate(profitShareUpdateRequest, cancellationToken);

        List<ProfitShareUpdateMemberResponse> members = memberFinancials.Select(m => new ProfitShareUpdateMemberResponse
        {
            IsEmployee = m.IsEmployee,
            Badge = m.BadgeNumber,
            Psn = m.Psn,
            Name = m.Name,
            BeginningAmount = m.CurrentAmount,
            Distributions = m.Distributions,
            Military = m.Military,
            Xfer = m.Xfer,
            Pxfer = m.Pxfer,
            EmployeeTypeId = m.EmployeeTypeId,
            Contributions = m.Contributions,
            IncomingForfeitures = m.IncomingForfeitures,
            AllEarnings = m.AllEarnings,
            Etva = m.Etva,
            AllSecondaryEarnings = m.AllSecondaryEarnings,
            EtvaEarnings = m.EarningsOnEtva,
            SecondaryEtvaEarnings = m.SecondaryEtvaEarnings,
            EndingBalance = m.EndingBalance,
            ZeroContributionReasonId = m.ZeroContributionReasonId,
            TreatAsBeneficiary = m.TreatAsBeneficiary
        }).ToList();

        return new ProfitShareUpdateResponse
        {
            HasExceededMaximumContributions = employeeExceededMaxContribution,
            AdjustmentsSummary = adjustmentReportData,
            Totals = totalsDto,
            ReportName = "Profit Sharing Update",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<ProfitShareUpdateMemberResponse>(profitShareUpdateRequest)
            {
                Total = members.Count,
                Results =  ProfitShareEditService.HandleInMemorySortAndPaging(profitShareUpdateRequest, members)
            }
        };
    }

    /// <summary>
    ///     This is used by other services to access plan members with yearly contributions applied.
    /// </summary>
    public async Task<ProfitShareUpdateResult> ProfitShareUpdateInternal(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, _, _, bool employeeExceededMaxContribution) = await ProfitSharingUpdate(profitShareUpdateRequest, cancellationToken);
        List<ProfitShareUpdateMember> members = memberFinancials.Select(m => new ProfitShareUpdateMember
        {
            IsEmployee = m.IsEmployee,
            Ssn = m.Ssn,
            BadgeNumber = m.BadgeNumber,
            Psn = m.Psn,
            Name = m.Name,
            BeginningAmount = m.CurrentAmount,
            Distributions = m.Distributions,
            Military = m.Military,
            Xfer = m.Xfer,
            Pxfer = m.Pxfer,
            EmployeeTypeId = m.EmployeeTypeId,
            Contributions = m.Contributions,
            IncomingForfeitures = m.IncomingForfeitures,
            AllEarnings = m.AllEarnings,
            Etva = m.Etva,
            AllSecondaryEarnings = m.AllSecondaryEarnings,
            EtvaEarnings = m.EarningsOnEtva,
            SecondaryEtvaEarnings = m.SecondaryEtvaEarnings,
            EndingBalance = m.EndingBalance,
            ZeroContributionReasonId = m.ZeroContributionReasonId
        }).ToList();

        return new ProfitShareUpdateResult { HasExceededMaximumContributions = employeeExceededMaxContribution, Members = members };
    }

    /// <summary>
    ///     Applies updates specified in request and returns members with updated
    ///     Contributions/Earnings/IncomingForfeitures/SecondaryEarnings
    /// </summary>
    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate (see https://demoulas.atlassian.net/browse/PS-900)
        AdjustmentsSummaryDto adjustmentsSummaryData = new();

        // Start off loading the employees.
        (List<MemberFinancials> members, bool employeeExceededMaxContribution) = await EmployeeProcessorHelper.ProcessEmployees(_dbContextFactory, _calendarService, _totalService,
            profitShareUpdateRequest, adjustmentsSummaryData, cancellationToken);

        // Go get the Bene's.  NOTE: May modify some employees if they are both bene and employee (that's why "members" is passed in - to lookup loaded employees and see if they are also Bene's)
        await BeneficiariesProcessingHelper.ProcessBeneficiaries(_dbContextFactory, members, profitShareUpdateRequest, cancellationToken);

        members = members.OrderBy(m => m.Name).ToList();

        TotalsDto totalsDto = new();
        foreach (MemberFinancials memberFinancials in members)
        {
            totalsDto.BeginningBalance += memberFinancials.CurrentAmount;
            totalsDto.Distributions += memberFinancials.Distributions;
            totalsDto.TotalContribution += memberFinancials.Contributions;
            totalsDto.Military += memberFinancials.Military;
            totalsDto.Forfeiture += memberFinancials.IncomingForfeitures;
            totalsDto.Earnings += memberFinancials.AllEarnings;
            totalsDto.Earnings2 += memberFinancials.AllSecondaryEarnings;
            totalsDto.EndingBalance += memberFinancials.EndingBalance;
            totalsDto.Allocations += memberFinancials.Xfer;
            totalsDto.PaidAllocations -= memberFinancials.Pxfer;
            totalsDto.EarningPoints += memberFinancials.EarningPoints;
            totalsDto.ContributionPoints += memberFinancials.ContributionPoints;
            totalsDto.ClassActionFund += memberFinancials.Caf;
            totalsDto.MaxOverTotal += memberFinancials.MaxOver;
            totalsDto.MaxPointsTotal += memberFinancials.MaxPoints;
            // members can be both employees and beneficiaries, but I presume that the employee count is the one that matters.
            if (memberFinancials.IsEmployee)
            {
                totalsDto.TotalEmployees++;
            }
            else
            {
                totalsDto.TotalBeneficaries++;
            }
        }

        // Use the list of members to build up response for client.
        return new ProfitShareUpdateOutcome(members, adjustmentsSummaryData, totalsDto, employeeExceededMaxContribution);
    }
}
