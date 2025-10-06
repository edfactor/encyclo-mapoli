
using Demoulas.ProfitSharing.Common.Attributes;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiaryDetailResponse : IIsExecutive
{
    public int BadgeNumber { get; set; }
    public short PsnSuffix { get; set; }
    [MaskSensitive] public string? Name { get; set; }
    public string? Ssn { get; set; }
    [MaskSensitive] public string? Street { get; set; }
    [MaskSensitive] public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    [MaskSensitive] public DateOnly? DateOfBirth { get; set; }
    public decimal? CurrentBalance { get; set; }
    public bool IsExecutive { get; set; }
}
