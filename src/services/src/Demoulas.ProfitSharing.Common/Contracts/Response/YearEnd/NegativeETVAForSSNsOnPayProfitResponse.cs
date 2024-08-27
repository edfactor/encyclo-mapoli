namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record NegativeEtvaForSsNsOnPayProfitResponse
{
    public long EmployeeBadge { get; set; }
    public long EmployeeSsn { get; set; }
    public decimal EtvaValue { get; set; }
    
}
