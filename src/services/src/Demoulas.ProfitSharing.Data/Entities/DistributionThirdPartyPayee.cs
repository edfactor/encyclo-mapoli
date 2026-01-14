
namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DistributionThirdPartyPayee
{
    public int Id { get; set; }

    public string? Payee { get; set; }
    public string? Name { get; set; }
    public required Address Address { get; set; }

    public string? Memo { get; set; }

    public List<Distribution> Distributions { get; set; } = [];
}
