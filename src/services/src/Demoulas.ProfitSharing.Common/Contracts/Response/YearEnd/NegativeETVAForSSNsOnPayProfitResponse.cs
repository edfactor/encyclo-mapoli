namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record NegativeETVAForSSNsOnPayProfitResponse
{
    public long EmployeeBadge { get; set; }
    public long EmployeeSSN { get; set; }
    public decimal EtvaValue { get; set; }
    
}
