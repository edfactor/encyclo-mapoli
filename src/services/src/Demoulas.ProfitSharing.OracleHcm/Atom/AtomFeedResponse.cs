namespace Demoulas.ProfitSharing.OracleHcm.Atom;

public sealed class AtomFeedResponse
{
    public IEnumerable<AtomFeedRecord>? Records { get; set; }
    public string? NextPageUrl { get; set; }
}
