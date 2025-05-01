namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record ForfeitureAdjustmentRequest : ProfitYearRequest
{
    public int? SSN { get; set; }
    public int? Badge { get; set; }
    public int? Client { get; set; }
}
