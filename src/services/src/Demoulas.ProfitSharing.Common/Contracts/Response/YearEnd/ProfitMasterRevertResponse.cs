namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record ProfitMasterRevertResponse
{
    public required DateTime UdpatedTime { get; set; }
    public required String UpdatedBy { get; set; }

    public required int BeneficiariesEffected { get; set; }
    public required int EmployeesEffected { get; set; }
    public required int EtvasEffected { get; set; }

    public static ProfitMasterRevertResponse Example()
    {
        return new ProfitMasterRevertResponse
        {
            UdpatedTime = DateTime.Now,
            UpdatedBy = "John Doe",
            BeneficiariesEffected = 4,
            EmployeesEffected = 721,
            EtvasEffected = 400,
        };
    }
}
