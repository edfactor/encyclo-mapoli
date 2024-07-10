using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public class Department : LookupTable<byte>
{
    public static class Constants
    {
        public const byte Grocery = 1;
        public const byte Meat = 2;
        public const byte Produce = 3;
        public const byte Deli = 4;
        public const byte Dairy = 5;
        public const byte Beer_And_Wine = 6;
        public const byte Bakery = 7;
    }

    public ICollection<Demographic>? Demographics { get; set; }
   
}
