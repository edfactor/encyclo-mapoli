
namespace Demoulas.ProfitSharing.Data.Entities
{
    public abstract class SsnChangeHistory
    {
        public int Id { get; set; }

        public int OldSsn { get; set; }

        public int NewSsn { get; set; }

        public DateTimeOffset ChangeDateUtc { get; set; }
    }
}
