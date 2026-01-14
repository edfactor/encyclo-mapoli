using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Department : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Grocery = 1;
        public const byte Meat = 2;
        public const byte Produce = 3;
        public const byte Deli = 4;
        public const byte Dairy = 5;
        public const byte BeerAndWine = 6;
        public const byte Bakery = 7;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Demographic>? Demographics { get; set; }

}
