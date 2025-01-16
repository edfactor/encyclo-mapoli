namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;


public sealed record MasterUpdateResponse
{
    public required int BeneficiariesEffected { get; set; }
    public required int EmployeesEffected { get; set; }
    public required int EtvasUpdated { get; set; }

    public static MasterUpdateResponse Example()
    {
        return new MasterUpdateResponse { BeneficiariesEffected = 4, EmployeesEffected = 721, EtvasUpdated = 400};
    }
}
