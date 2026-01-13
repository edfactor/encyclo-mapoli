using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class PayFrequency : ILookupTable<byte>
{
    // Pay Frequency Id Constants
    public static class Constants
    {
        public const byte Weekly = 1;
        public const byte Monthly = 2;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Demographic>? Demographics { get; set; }
}
