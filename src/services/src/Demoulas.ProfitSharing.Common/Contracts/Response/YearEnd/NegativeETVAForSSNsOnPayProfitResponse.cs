using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.YearEnd;

public sealed record NegativeEtvaForSsNsOnPayProfitResponse : IIsExecutive
{
    public long BadgeNumber { get; set; }
    public string Ssn { get; set; } = string.Empty;
    public decimal EtvaValue { get; set; }
    public bool IsExecutive { get; set; }

    /// <summary>
    /// Example data for testing and API documentation.
    /// </summary>
    public static NegativeEtvaForSsNsOnPayProfitResponse ResponseExample()
    {
        return new NegativeEtvaForSsNsOnPayProfitResponse
        {
            BadgeNumber = 654321,
            Ssn = "***-**-3456",
            EtvaValue = -1250.75m,
            IsExecutive = false
        };
    }
}
