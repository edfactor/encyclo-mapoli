

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record StartAndEndDateRequest : ProfitYearRequest
{
    public DateOnly BeginningDate { get; set; }
    public DateOnly EndingDate { get; set; }
    public bool ExcludeZeroBalance { get; set; } = false;
    public static new StartAndEndDateRequest RequestExample()
    {
        return new StartAndEndDateRequest
        {
            BeginningDate = new DateOnly(2019, 01, 01),
            EndingDate = new DateOnly(2024, 12, 31),
            ProfitYear = 2024,
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false
        };
    }
}
