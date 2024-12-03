namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;
public sealed record BalanceByAgeDetail
{
    public required int Age { get; init; }
    public required int EmployeeCount { get; init; }
    public required decimal CurrentBalance { get; init; }
    public int BeneficiaryCount { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal CurrentBeneficiaryBalance { get; set; }
    public decimal CurrentBeneficiaryVestedBalance { get; set; }
    public int FullTimeCount { get; set; }
    public int PartTimeCount { get; set; }

    public static BalanceByAgeDetail ResponseExample()
    {
        return new BalanceByAgeDetail
        {
            Age = 32,
            CurrentBalance = (decimal)159_451.46,
            EmployeeCount = 7,
            BeneficiaryCount = 3
        };
    }
}
