namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public sealed record BalanceByYearsDetail : BalanceByDetailBase
{
    public required byte Years { get; init; }

    public static BalanceByYearsDetail ResponseExample()
    {
        return new BalanceByYearsDetail { Years = 12, CurrentBalance = (decimal)159_451.46, EmployeeCount = 7, BeneficiaryCount = 3 };
    }
}
