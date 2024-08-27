
namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryRelPercent
{
    public required long Psn { get; set; }
    public required long Ssn { get; set; }
    public required char KindId { get; set; }
    public required BeneficiaryKind Kind { get; set; }
    public required decimal Percent { get; set; }
    public string? Relationship { get; set; }
}
