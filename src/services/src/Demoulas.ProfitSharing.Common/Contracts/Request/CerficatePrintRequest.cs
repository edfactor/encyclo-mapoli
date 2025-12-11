namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record CerficatePrintRequest : ProfitYearRequest
{
    public int[]? Ssns { get; set; }
    public int[]? BadgeNumbers { get; set; }
}
