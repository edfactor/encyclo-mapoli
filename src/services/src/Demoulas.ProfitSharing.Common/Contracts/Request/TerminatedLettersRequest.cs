namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record TerminatedLettersRequest : ProfitYearRequest
{
    public DateOnly? BeginningDate { get; set; }
    public DateOnly? EndingDate { get; set; }
    public bool? ExcludeZeroBalance { get; set; }

    public List<int>? BadgeNumbers { get; set; }

    public static new TerminatedLettersRequest RequestExample()
    {
        return new TerminatedLettersRequest
        {
            BeginningDate = new DateOnly(2019, 01, 01),
            EndingDate = new DateOnly(2024, 12, 31),
            ProfitYear = 2024,
            Skip = 1,
            Take = 10,
            SortBy = "BadgeNumber",
            IsSortDescending = false,
            BadgeNumbers = new List<int> { 1234567, 2345678 }
        };
    }
}
