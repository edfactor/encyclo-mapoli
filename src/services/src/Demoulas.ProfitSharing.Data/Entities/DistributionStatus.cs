using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class DistributionStatus : ILookupTable<char>
{
    public static class Constants
    {
        public const char ManualCheck = 'C';
        public const char PurgeRecord = 'D';
        public const char RequestOnHold = 'H';
        public const char Override = 'O';
        public const char PaymentMade = 'P';
        public const char OkayToPay = 'Y';
        public const char PurgeAllRecordsForSsn = 'X';
        public const char PurgeAllRecordsForSsn2 = 'Z';
    }

    public required char Id { get; set; }
    public required string Name { get; set; }

}
