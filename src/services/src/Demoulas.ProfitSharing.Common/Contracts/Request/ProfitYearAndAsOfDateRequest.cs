namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record ProfitYearAndAsOfDateRequest : ProfitYearRequest
{
    public DateOnly? AsOfDate { get; set; }

    public static new ProfitYearAndAsOfDateRequest RequestExample() => new()
    {
        ProfitYear = 2024,
        AsOfDate = new DateOnly(2024, 12, 31)
    };
}
