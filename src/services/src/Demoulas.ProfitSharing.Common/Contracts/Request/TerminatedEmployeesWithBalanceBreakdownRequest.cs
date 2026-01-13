namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record TerminatedEmployeesWithBalanceBreakdownRequest : BreakdownByStoreRequest, IStartEndDateRequest
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public static new TerminatedEmployeesWithBalanceBreakdownRequest RequestExample()
    {
        return new TerminatedEmployeesWithBalanceBreakdownRequest
        {
            ProfitYear = 2024,
            StoreManagement = true,
            StoreNumber = 22,
            BadgeNumber = 123456,
            EmployeeName = "Smith",
            StartDate = new DateOnly(2024, 1, 1),
            EndDate = new DateOnly(2024, 12, 31)
        };
    }
}
