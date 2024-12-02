namespace Demoulas.ProfitSharing.IntegrationTests.Reports.YearEnd.Update;


public class MemberFinancials
{
    public long EmployeeId { get; set; } 
    public string? Name { get; set; } 
    public int Ssn { get; set; } 
    public decimal CurrentAmount { get; set; } 
    public decimal Distributions { get; set; } 
    public decimal Military { get; set; } 
    public decimal Xfer { get; set; } 
    public decimal Pxfer { get; set; } 
    public long EmployeeTypeId { get; set; } 
    public long ContributionPoints { get; set; } 
    public long EarningPoints { get; set; } 
    public decimal Contributions { get; set; } 
    public decimal IncomingForfeitures { get; set; } 
    public decimal Earnings { get; set; } 
    public decimal SecondaryEarnings { get; set; } 
    public decimal MaxOver { get; set; } 
    public long MaxPoints { get; set; } 
    public long Psn { get; set; } 
    public decimal Caf { get; set; } 
}
