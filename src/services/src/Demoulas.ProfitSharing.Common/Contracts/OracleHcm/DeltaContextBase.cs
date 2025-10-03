namespace Demoulas.ProfitSharing.Common.Contracts.OracleHcm;

public abstract class DeltaContextBase
{
    public string? FeedType { get; set; }
    public long PersonId { get; set; }
    public string? PersonName { get; set; }
    public int? PersonNumber { get; set; }
    public string? PrimaryPhoneNumber { get; set; }
    public string? WorkerType { get; set; }
    public string? PeriodType { get; set; }
    public string? DMLOperation { get; set; }
    public DateOnly? EffectiveDate { get; set; }
}
