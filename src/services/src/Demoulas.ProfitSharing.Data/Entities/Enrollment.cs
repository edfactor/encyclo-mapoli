using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Enrollment : LookupTable<byte>
{
    public static class Constants
    {
        public const byte Not_Enrolled = 0;
        public const byte Old_Vesting_Plan_Has_Contributions = 1;
        public const byte New_Vesting_Plan_Has_Contributions = 2;
        public const byte Old_Vesting_Plan_Has_Forfeiture_Records = 3;
        public const byte New_Vesting_Plan_Has_Forfeiture_Records = 4;
    }

    public ICollection<PayProfit>? Profits { get; set; }
}
