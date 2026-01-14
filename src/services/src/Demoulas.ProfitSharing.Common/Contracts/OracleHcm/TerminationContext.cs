namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class TerminationContext : DeltaContextBase
{
    public DateOnly? EffectiveStartDate { get; set; }
}
