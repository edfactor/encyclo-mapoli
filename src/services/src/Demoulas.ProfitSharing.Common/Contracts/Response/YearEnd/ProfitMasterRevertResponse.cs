using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

[NoMemberDataExposed]
public sealed record ProfitMasterRevertResponse
{
    public required DateTime UpdatedTime { get; set; }
    public required string UpdatedBy { get; set; }
    public required int EtvasEffected { get; set; }
    public required int TransactionsRemoved { get; set; }
    public required int EmployeesEffected { get; set; }
    public required int BeneficiariesEffected { get; set; }

    public static ProfitMasterRevertResponse Example()
    {
        return new ProfitMasterRevertResponse
        {
            UpdatedTime = DateTime.Now,
            UpdatedBy = "John Doe",
            BeneficiariesEffected = 4,
            EmployeesEffected = 721,
            EtvasEffected = 400,
            TransactionsRemoved = 838
        };
    }
}
