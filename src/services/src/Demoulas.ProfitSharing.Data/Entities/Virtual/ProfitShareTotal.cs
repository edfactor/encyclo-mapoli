
namespace Demoulas.ProfitSharing.Data.Entities.Virtual;
public sealed record ProfitShareTotal(
    decimal WagesTotal,
    decimal HoursTotal,
    decimal PointsTotal,
    decimal TerminatedWagesTotal,
    decimal TerminatedHoursTotal,
    decimal TerminatedPointsTotal,
    int NumberOfEmployees,
    int NumberOfNewEmployees,
    int NumberOfEmployeesUnder21
);

