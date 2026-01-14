namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class Feed<TContext> where TContext : DeltaContextBase
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public DateTime Updated { get; set; }
    public List<Author> Authors { get; set; } = [];
    public List<Entry<TContext>> Entries { get; set; } = [];
    public List<Link> Links { get; set; } = [];
}
