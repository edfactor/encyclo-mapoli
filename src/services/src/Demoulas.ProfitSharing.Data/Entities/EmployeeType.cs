using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class EmployeeType : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte NotNewLastYear = 0;
        public const byte NewLastYear = 1;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }
}
