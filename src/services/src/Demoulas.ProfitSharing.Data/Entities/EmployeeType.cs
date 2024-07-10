using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class EmployeeType : LookupTable<byte>
{
    public static class Constants
    {
        public const byte Not_New_LastYear = 0;
        public const byte New_LastYear = 1;
    }
   
    public ICollection<PayProfit>? Profits { get; set; }
}
