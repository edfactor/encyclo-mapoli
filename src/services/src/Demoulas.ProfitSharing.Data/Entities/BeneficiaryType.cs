using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryType : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Employee = 0;
        public const byte Beneficiary = 1;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<PayProfit>? Profits { get; set; }
}
