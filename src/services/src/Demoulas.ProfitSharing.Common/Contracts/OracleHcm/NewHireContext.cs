namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public class NewHireContext : DeltaContextBase
{
    public long? PeriodOfServiceId { get; set; }

    public string? WorkEmail { get; set; }

    public DateOnly? EffectiveStartDate { get; set; }
}
