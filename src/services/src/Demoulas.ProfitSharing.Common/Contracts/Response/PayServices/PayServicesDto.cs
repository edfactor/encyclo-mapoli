namespace Demoulas.ProfitSharing.Common.Contracts.Response.PayServices;
public class PayServicesDto
{
    public int Employees { get; set; } = 0;
    public decimal WeeklyPay { get; set; }
    public decimal YearsWages { get; set; }
    public int YearsOfService { get; set; } 
    public string YearsOfServiceLabel { get; set; } = string.Empty;
}
