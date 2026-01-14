
namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DistributionPayee
{
    public int Id { get; set; }

    public required int Ssn { get; set; }
    public required string Name { get; set; }
    public required Address Address { get; set; }

    public string? Memo { get; set; }

    public List<Distribution> Distributions { get; set; } = [];
}
