namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DemographicSsnChangeHistory : SsnChangeHistory
{
    public int DemographicId { get; set; }
    public Demographic Demographic { get; set; } = null!;

    public static DemographicSsnChangeHistory FromDemographic(Demographic source, int newSsn)
    {
        var h = new DemographicSsnChangeHistory()
        {
            DemographicId = source.Id,
            NewSsn = newSsn,
            OldSsn = source.Ssn
        };
        return h;
    }
}
