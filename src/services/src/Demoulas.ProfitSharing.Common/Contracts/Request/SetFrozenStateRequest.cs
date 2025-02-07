namespace Demoulas.ProfitSharing.Common.Contracts.Request;

public sealed record SetFrozenStateRequest : YearRequest
{
    public DateTime AsOfDateTime { get; set; }
}
