using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Country : ILookupTable<short>
{
    public static class Constants
    {
        public const string US = "US";
        public const string Canada = "CA";
    }

    public short Id { get; set; }
    public required string Name { get; set; }

    public required string ISO { get; init; }
    public required string TelephoneCode { get; init; }
}
