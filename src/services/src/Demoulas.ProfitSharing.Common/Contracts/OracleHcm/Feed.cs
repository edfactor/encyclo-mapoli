namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class Feed<TContext> where TContext : DeltaContextBase
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Subtitle { get; set; }
    public DateTime Updated { get; set; }
    public List<Author> Authors { get; set; } = new List<Author>();
    public List<Entry<TContext>> Entries { get; set; } = new List<Entry<TContext>>();
    public List<Link> Links { get; set; } = new List<Link>();
}