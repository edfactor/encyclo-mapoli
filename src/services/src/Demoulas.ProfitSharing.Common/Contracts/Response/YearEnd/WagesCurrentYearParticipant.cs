using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record WagesCurrentYearParticipant : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public decimal IncomeCurrentYear { get; set; }
    public decimal HoursCurrentYear { get; set; }
    public short StoreNumber { get; set; }
    public bool IsExecutive { get; set; }
}
