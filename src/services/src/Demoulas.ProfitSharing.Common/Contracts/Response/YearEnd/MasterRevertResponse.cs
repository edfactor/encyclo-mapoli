namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record MasterRevertResponse
{
    public required long BeneficiariesReverted { get; set; }
    public required int EmployeesReverted { get; set; }
    public required int EtvaReverted { get; set; }

    public static MasterRevertResponse Example()
    {
        return new MasterRevertResponse
        {
            BeneficiariesReverted = 4,
            EmployeesReverted = 721,
            EtvaReverted = 70
        };
    }
}
