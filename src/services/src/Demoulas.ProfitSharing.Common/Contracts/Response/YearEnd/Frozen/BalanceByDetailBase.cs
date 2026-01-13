namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd.Frozen;

public abstract record BalanceByDetailBase
{
    public required int EmployeeCount { get; init; }
    public required decimal CurrentBalance { get; init; }
    public int BeneficiaryCount { get; set; }
    public decimal VestedBalance { get; set; }
    public decimal CurrentBeneficiaryBalance { get; set; }
    public decimal CurrentBeneficiaryVestedBalance { get; set; }
    public int FullTimeCount { get; set; }
    public int PartTimeCount { get; set; }
}
