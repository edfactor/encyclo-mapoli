using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Distributions;

[NoMemberDataExposed]
public sealed record CreateDistributionResponse : CreateDistributionRequest
{
    public long Id { get; set; }
    public required string MaskSsn { get; set; }
    public byte PaymentSequence { get; set; }
    public DateTime CreatedAt { get; set; }
}
