namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class AtomFeedResponse<TContext> where TContext : DeltaContextBase
{
    public required Feed<TContext> Feed { get; set; }
}
