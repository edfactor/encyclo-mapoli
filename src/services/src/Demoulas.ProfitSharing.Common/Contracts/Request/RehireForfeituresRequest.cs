

namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public sealed record RehireForfeituresRequest : ProfitYearRequest
{
    public DateTime BeginningDate { get; set; }
    public DateTime EndingDate { get; set; }



    public static new RehireForfeituresRequest RequestExample()
    {
        return new RehireForfeituresRequest
        {
            BeginningDate = new DateOnly(2024, 03, 15).ToDateTime(TimeOnly.MinValue),
            EndingDate = new DateOnly(2024, 09, 15).ToDateTime(TimeOnly.MinValue),
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false,
            ProfitYear = 2024
        };
    }
}
