namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record BalanceByAgeDetail
{
    public required int Age { get; init; }
    public required int EmployeeCount { get; init; }
    public required decimal Amount { get; init; }
    public int BeneficiaryCount { get; set; }

    public static BalanceByAgeDetail ResponseExample()
    {
        return new BalanceByAgeDetail
        {
            Age = 32,
            Amount = (decimal)159_451.46,
            EmployeeCount = 7,
            BeneficiaryCount = 3
        };
    }
}
