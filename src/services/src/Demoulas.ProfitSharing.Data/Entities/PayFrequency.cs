using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class PayFrequency : LookupTable<byte>
{
    public static class Constants
    {
        public const byte Weekly = 1;
        public const byte Monthly = 2;
    }

    public ICollection<Demographic>? Demographics { get; set; }
}
