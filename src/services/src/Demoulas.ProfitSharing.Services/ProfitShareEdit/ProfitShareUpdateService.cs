using Demoulas.Common.Contracts.Contracts.Response;
using Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Entities;
using Demoulas.ProfitSharing.Data.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.Interfaces;
using Demoulas.ProfitSharing.Services.Internal.ProfitShareUpdate;
using Demoulas.ProfitSharing.Services.Internal.ServiceDto;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Services.ProfitShareEdit;

/// <summary>
///     Does the Year And application of Earnings and Contributions to all employees and beneficiaries.
///     Modeled very closely after Pay444
///
///     This class follows the name of the step in the Ready YE flow.    It could instead be named "View effect of YE update on members"
/// </summary>
internal sealed class ProfitShareUpdateService : IInternalProfitShareUpdateService
{
    private readonly ICalendarService _calendarService;
    private readonly IProfitSharingDataContextFactory _dbContextFactory;
    private readonly ITotalService _totalService;

    public ProfitShareUpdateService(IProfitSharingDataContextFactory dbContextFactory, 
        ITotalService totalService, 
        ICalendarService calendarService)
    {
        _dbContextFactory = dbContextFactory;
        _totalService = totalService;
        _calendarService = calendarService;
    }

    public async Task<ProfitShareUpdateResponse> ProfitShareUpdate(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, _, bool employeeExceededMaxContribution) = await ProfitSharingUpdatePaginated(profitShareUpdateRequest, cancellationToken);
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
            ZeroContributionReasonId = m.ZeroContributionReasonId
        }).ToList();

        return new ProfitShareUpdateResponse
        {
            HasExceededMaximumContributions = employeeExceededMaxContribution,
            ReportName = "Profit Sharing Update",
            ReportDate = DateTimeOffset.Now,
            Response = new PaginatedResponseDto<ProfitShareUpdateMemberResponse> { Results = members }
        };
    }

    public async Task<ProfitShareUpdateResult> ProfitShareUpdateInternal(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        (List<MemberFinancials> memberFinancials, _, bool employeeExceededMaxContribution) = await ProfitSharingUpdatePaginated(profitShareUpdateRequest, cancellationToken);
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

        return new ProfitShareUpdateResult()
        {
            HasExceededMaximumContributions = employeeExceededMaxContribution,
            Members = members
        };
    }

    /// <summary>
    ///     Applies updates specified in request and returns members with updated Contributions/Earnings/IncomingForfeitures/SecondaryEarnings
    /// </summary>
    public async Task<ProfitShareUpdateOutcome> ProfitSharingUpdatePaginated(ProfitShareUpdateRequest profitShareUpdateRequest, CancellationToken cancellationToken)
    {
        // Values collected for an "Adjustment Report" that we do not yet generate
        AdjustmentReportData adjustmentReportData = new();

        // Start off with nothing
        List<MemberFinancials> members = [];
        
        // Go fetch some employees
        bool employeeExceededMaxContribution = await EmployeesProcessor.ProcessEmployees(_dbContextFactory, _calendarService, _totalService, members, profitShareUpdateRequest, adjustmentReportData, cancellationToken);
        
        // Go get the Bene's.  May modify some employees if they are both bene and employee (thats why "members" is passed in.)
        await BeneficiariesProcessing.ProcessBeneficiaries(_dbContextFactory, members, profitShareUpdateRequest, cancellationToken);

        // Use the list of members to build up response for client.
        return new(members, adjustmentReportData, employeeExceededMaxContribution);
    }


}
