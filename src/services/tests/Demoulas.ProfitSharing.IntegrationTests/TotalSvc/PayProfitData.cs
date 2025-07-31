namespace Demoulas.ProfitSharing.IntegrationTests.TotalSvc;

public record PayProfitData
{
    public int BadgeNumber { get; set; }
    public int Ssn { get; set; }
    public int Years { get; set; }
    public decimal Amount { get; set; }
    public decimal Etva { get; set; }
    public decimal VestedAmount { get; set; }
}
