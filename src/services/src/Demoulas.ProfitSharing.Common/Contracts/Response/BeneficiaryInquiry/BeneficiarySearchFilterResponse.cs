

using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiarySearchFilterResponse
{
    public int BadgeNumber { get; set; }
    public short PsnSuffix { get; set; }
    [MaskSensitive] public string? Name { get; set; }
    public string? Ssn { get; set; }
    [MaskSensitive] public string? Street { get; set; }
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public int? Age { get; set; }

}
