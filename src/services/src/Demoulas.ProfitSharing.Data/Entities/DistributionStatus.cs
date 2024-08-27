using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

using Demoulas.ProfitSharing.Data.Interfaces;
public class DistributionStatus: ILookupTable<char>
{
    public static class Constants
    {
        public const char ManualCheck = 'C';
        public const char PurgeRecord = 'D';
        public const char RequestOnHold = 'H';
        public const char Override = 'O';
        public const char PaymentMade = 'P';
        public const char OkayToPay = 'Y';
        public const char PurgeAllRecordsForSSN = 'X';
        public const char PurgeAllRecordsForSSN2 = 'Z';
    }

    public required char Id { get; set; }
    public required string Name { get; set; }
    
}
