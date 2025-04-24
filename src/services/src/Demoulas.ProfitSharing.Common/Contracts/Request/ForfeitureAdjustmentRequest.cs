namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record ForfeitureAdjustmentRequest : ProfitYearRequest
{
    public string? SSN { get; set; }
    public string? Badge { get; set; }
    public int? Client { get; set; }
}
