
namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryRelPercent
{
    public required long PSN { get; set; }
    public required long SSN { get; set; }
    public required char KindId { get; set; }
    public required BeneficiaryKind Kind { get; set; }
    public required decimal Percent { get; set; }
    public string? Relationship { get; set; }
}
