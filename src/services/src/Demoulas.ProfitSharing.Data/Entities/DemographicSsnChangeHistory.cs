namespace Demoulas.ProfitSharing.Data.Entities
{
    public sealed class DemographicSsnChangeHistory : SsnChangeHistory
    {
        public int DemographicId { get; set; }
        public Demographic Demographic { get; set; } = null!;
    }
}
