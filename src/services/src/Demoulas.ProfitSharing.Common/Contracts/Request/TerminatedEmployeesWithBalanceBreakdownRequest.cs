namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record TerminatedEmployeesWithBalanceBreakdownRequest : BreakdownByStoreRequest, IStartEndDateRequest
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
