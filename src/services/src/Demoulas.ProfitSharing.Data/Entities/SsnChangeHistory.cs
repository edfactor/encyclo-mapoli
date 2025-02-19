
namespace Demoulas.ProfitSharing.Data.Entities
{
    public abstract class SsnChangeHistory
    {
        public int Id { get; set; }

        public required int OldSsn { get; set; }

        public required int NewSsn { get; set; }

        public DateTimeOffset ChangeDateUtc { get; set; }
    }
}
