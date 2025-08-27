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
    private readonly IDemographicReaderService _demographicReaderService;

    public ProfitShareUpdateService(IProfitSharingDataContextFactory dbContextFactory, TotalService totalService, ICalendarService calendarService, IDemographicReaderService demographicReaderService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
        _demographicReaderService = demographicReaderService;
    }

    public async Task<ProfitShareUpdateResponse> ProfitShareUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        ProfitShareUpdateOutcome result =
            await ProfitSharingUpdate(profitShareUpdateRequest, cancellationToken, false);

        List<ProfitShareUpdateMemberResponse> members = result.MemberFinancials.Select(m => new ProfitShareUpdateMemberResponse
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


        var calInfo = await _calendarService.GetYearStartAndEndAccountingDatesAsync(profitShareUpdateRequest.ProfitYear, cancellationToken);
        return new ProfitShareUpdateResponse
        {
            HasExceededMaximumContributions = result.RerunNeeded,
            AdjustmentsSummary = result.AdjustmentsSummaryData,
            ProfitShareUpdateTotals = result.ProfitShareUpdateTotals,
            ReportName = "Profit Sharing Update",
            ReportDate = DateTimeOffset.UtcNow,
            StartDate = calInfo.FiscalBeginDate,
            EndDate = calInfo.FiscalEndDate,
            Response = new PaginatedResponseDto<ProfitShareUpdateMemberResponse>(profitShareUpdateRequest)
            {
                Total = members.Count, Results = ProfitShareEditService.HandleInMemorySortAndPaging(profitShareUpdateRequest, members)
            }
        };
    }

    /// <summary>
    ///     This is used by other services to access plan members with yearly contributions applied.
    /// </summary>
    public async Task<ProfitShareUpdateResult> ProfitShareUpdateInternal(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        var result = await ProfitSharingUpdate(profitShareUpdateRequest, cancellationToken, true);
        List<ProfitShareUpdateMember> members = result.MemberFinancials.Select(m => new ProfitShareUpdateMember
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

        return new ProfitShareUpdateResult { HasExceededMaximumContributions = result.RerunNeeded, Members = members };
    }

    /// <summary>
    ///     Applies updates specified in the request and returns members with updated
    ///     Contributions/Earnings/IncomingForfeitures/SecondaryEarnings
    ///
    /// The "includeZeroAmounts" is because PAY444 doesnt show members with no financial change, PAY447 does include members with changes to ZeroContributions.
    /// </summary>
    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken, bool includeZeroAmounts)
        {
        // Values collected for an "Adjustment Report" that we do not yet generate (see https://demoulas.atlassian.net/browse/PS-900)
        AdjustmentsSummaryDto adjustmentsSummaryData = new();

        // Start off loading the employees.
        (List<MemberFinancials> members, bool employeeExceededMaxContribution) = await EmployeeProcessorHelper.ProcessEmployees(_dbContextFactory, _calendarService, _totalService,
            _demographicReaderService, profitShareUpdateRequest, adjustmentsSummaryData, cancellationToken);

        // Go get the Bene's.  NOTE: May modify some employees if they are both bene and employee (that's why "members" is passed in - to lookup loaded employees and see if they are also Bene's)
        await BeneficiariesProcessingHelper.ProcessBeneficiaries(_dbContextFactory, _totalService, members, profitShareUpdateRequest, cancellationToken);
        
        members = members.OrderBy(m => m.Name).ToList();
        // The PAY444 Report/Page does not show the zero event individuals, but the PAY447 needs the ZeroContribution records for the PAY447 report/page. 
        if (!includeZeroAmounts)
        {
            members = members.Where(m=>!m.IsAllZeros()).ToList();
        }

        ProfitShareUpdateTotals profitShareUpdateTotals = new();
        foreach (MemberFinancials memberFinancials in members)
        {
            profitShareUpdateTotals.BeginningBalance += memberFinancials.CurrentAmount;
            profitShareUpdateTotals.Distributions += memberFinancials.Distributions;
            profitShareUpdateTotals.TotalContribution += memberFinancials.Contributions;
            profitShareUpdateTotals.Military += memberFinancials.Military;
            profitShareUpdateTotals.Forfeiture += memberFinancials.IncomingForfeitures - memberFinancials.Forfeits;
            profitShareUpdateTotals.Earnings += memberFinancials.AllEarnings;
            profitShareUpdateTotals.Earnings2 += memberFinancials.AllSecondaryEarnings;
            profitShareUpdateTotals.EndingBalance += memberFinancials.EndingBalance;
            profitShareUpdateTotals.Allocations += memberFinancials.Xfer;
            profitShareUpdateTotals.PaidAllocations -= memberFinancials.Pxfer;
            profitShareUpdateTotals.EarningPoints += memberFinancials.EarningPoints;
            profitShareUpdateTotals.ContributionPoints += memberFinancials.ContributionPoints;
            profitShareUpdateTotals.ClassActionFund += memberFinancials.Caf;
            profitShareUpdateTotals.MaxOverTotal += memberFinancials.MaxOver;
            profitShareUpdateTotals.MaxPointsTotal += memberFinancials.MaxPoints;
            
            // members can be both employees and beneficiaries, but I presume that the employee count is the one that matters.
            if (memberFinancials.IsEmployee)
            {
                profitShareUpdateTotals.TotalEmployees++;
            }
            else
            {
                profitShareUpdateTotals.TotalBeneficaries++;
            }
        }

        // Use the list of members to build up response for client.
        return new ProfitShareUpdateOutcome(members, adjustmentsSummaryData, profitShareUpdateTotals, employeeExceededMaxContribution);
    }
}
