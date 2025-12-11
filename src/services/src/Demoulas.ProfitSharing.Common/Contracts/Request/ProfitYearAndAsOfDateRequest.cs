namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public record ProfitYearAndAsOfDateRequest : ProfitYearRequest
{
    public DateOnly? AsOfDate { get; set; }
}
