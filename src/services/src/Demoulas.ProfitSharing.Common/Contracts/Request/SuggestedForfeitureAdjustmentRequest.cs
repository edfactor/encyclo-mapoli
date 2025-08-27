namespace Demoulas.ProfitSharing.Common.Contracts.Request;

#pragma warning disable DSM001

public sealed record SuggestedForfeitureAdjustmentRequest
{
    public int? Ssn { get; set; }
    public int? Badge { get; set; }
}
