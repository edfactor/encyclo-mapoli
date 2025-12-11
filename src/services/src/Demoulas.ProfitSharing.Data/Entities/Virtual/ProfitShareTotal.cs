
namespace Demoulas.ProfitSharing.Data.Entities.Virtual;

public sealed class ProfitShareTotal
{
    public decimal WagesTotal { get; set; }
    public decimal HoursTotal { get; set; }
    public decimal PointsTotal { get; set; }
    public decimal BalanceTotal { get; set; }
    public long NumberOfEmployees { get; set; }
    public int NumberOfNewEmployees { get; set; }
    public int NumberOfEmployeesUnder21 { get; set; }
}
