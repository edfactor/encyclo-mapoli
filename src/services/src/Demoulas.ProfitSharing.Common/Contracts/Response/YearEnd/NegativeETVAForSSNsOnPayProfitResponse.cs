using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record NegativeEtvaForSsNsOnPayProfitResponse : IIsExecutive
{
    public long BadgeNumber { get; set; }
    public string Ssn { get; set; } = string.Empty;
    public decimal EtvaValue { get; set; }
    public bool IsExecutive { get; set; }
}
