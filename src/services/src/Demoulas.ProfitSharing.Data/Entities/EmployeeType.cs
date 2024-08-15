using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class EmployeeType : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Not_New_LastYear = 0;
        public const byte New_LastYear = 1;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<PayProfit>? Profits { get; set; }
}
