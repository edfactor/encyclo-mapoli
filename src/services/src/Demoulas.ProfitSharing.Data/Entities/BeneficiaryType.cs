using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryType : LookupTable<byte>
{
    public static class Constants
    {
        public const byte Employee = 0;
        public const byte Beneficiary = 1;
    }

    public ICollection<PayProfit>? Profits { get; set; }
}
