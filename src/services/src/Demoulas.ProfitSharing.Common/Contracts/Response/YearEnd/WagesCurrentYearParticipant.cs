using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record WagesCurrentYearParticipant : IIsExecutive
{
    public required int BadgeNumber { get; set; }
    public decimal IncomeCurrentYear { get; set; }
    public decimal HoursCurrentYear { get; set; }
    public short StoreNumber { get; set; }
    public bool IsExecutive { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static WagesCurrentYearParticipant ResponseExample()
    {
        return new WagesCurrentYearParticipant
        {
            BadgeNumber = 556677,
            IncomeCurrentYear = 52000.00m,
            HoursCurrentYear = 2080.00m,
            StoreNumber = 32,
            IsExecutive = false
        };
    }
}
