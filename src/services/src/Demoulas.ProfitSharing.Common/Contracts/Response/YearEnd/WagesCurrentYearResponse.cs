using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;
public sealed record WagesCurrentYearResponse : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public decimal IncomeCurrentYear { get; set; }
    public decimal HoursCurrentYear { get; set; }
    public short StoreNumber { get; set; }
    public bool IsExecutive { get; set; }
    public static WagesCurrentYearResponse ResponseExample()
    {
        return new WagesCurrentYearResponse { BadgeNumber = 123456, HoursCurrentYear = 3265, IncomeCurrentYear = (decimal)25_325.18 };
    }
}
