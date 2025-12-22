using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record WagesCurrentYearResponse
{
    // Totals for the entire dataset
    [YearEndArchiveProperty]
    public decimal TotalHoursCurrentYearWages { get; set; }
    [YearEndArchiveProperty]
    public decimal TotalIncomeCurrentYearWages { get; set; }

    // List of participants for this report container (paginated)
    public required List<WagesCurrentYearParticipant> Participants { get; set; }

    public static WagesCurrentYearResponse ResponseExample()
    {
        return new WagesCurrentYearResponse
        {
            TotalHoursCurrentYearWages = 100000m,
            TotalIncomeCurrentYearWages = 2500000m,
            Participants = new List<WagesCurrentYearParticipant>
            {
                new WagesCurrentYearParticipant { BadgeNumber = 123456, HoursCurrentYear = 3265m, IncomeCurrentYear = 25325.18m, StoreNumber = 1, IsExecutive = false }
            }
        };
    }
}
