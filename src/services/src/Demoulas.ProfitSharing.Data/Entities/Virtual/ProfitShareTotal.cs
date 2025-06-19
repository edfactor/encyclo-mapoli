
namespace Demoulas.ProfitSharing.Data.Entities.Virtual;
public sealed class ProfitShareTotal
{
    public decimal WagesTotal { get; set; }
    public decimal HoursTotal { get; set; }
    public decimal PointsTotal { get; set; }
    public decimal BalanceTotal { get; set; }
    public decimal TerminatedWagesTotal { get; set; }
    public decimal TerminatedHoursTotal { get; set; }
    public decimal TerminatedPointsTotal { get; set; }
    public decimal TerminatedBalanceTotal { get; set; }
    public int NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
}
