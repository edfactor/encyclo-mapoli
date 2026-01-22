namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record CerficatePrintRequest : ProfitYearRequest
{
    public int[]? Ssns { get; set; }
    public int[]? BadgeNumbers { get; set; }
    public bool IsXerox { get; set; }

    public static new CerficatePrintRequest RequestExample()
    {
        return new CerficatePrintRequest
        {
            ProfitYear = 2024,
            Ssns = [123456789, 987654321],
            BadgeNumbers = [1001, 1002],
            IsXerox = false
        };
    }
}
