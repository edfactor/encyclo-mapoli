using System.Xml;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Enrollment
{
    public static class Constants
    {
        public const byte Not_Enrolled = 0;
        public const byte Old_Vesting_Plan_Has_Contributions = 1;
        public const byte New_Vesting_Plan_Has_Contributions = 2;
        public const byte Old_Vesting_Plan_Has_Forfeiture_Records = 3;
        public const byte New_Vesting_Plan_Has_Forfeiture_Records = 4;
    }
   

    public required byte Id { get; set; }
    public required string Description { get; set; }

    public ICollection<PayProfit>? Profits { get; set; }
}
