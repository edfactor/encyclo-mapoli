namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;


public sealed record ProfitMasterResponse
{
    public required int BeneficiariesEffected { get; set; }
    public required int EmployeesEffected { get; set; }
    public required int EtvasEffected { get; set; }

    public static ProfitMasterResponse Example()
    {
        return new ProfitMasterResponse { BeneficiariesEffected = 4, EmployeesEffected = 721, EtvasEffected = 400};
    }
}
