using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class DistributionRequestStatus : ILookupTable<char>
{
    public static class Constants
    {
        public const char NEW_ENTRY = 'N';
        public const char READY_FOR_REVIEW = 'R';
        public const char IN_COMMITTEE_REVIEW = 'C';
        public const char APPROVED = 'A';
        public const char DECLINED = 'D';
        public const char PROCESSED = 'P';
    }

    public required char Id { get; set; }
    public required string Name { get; set; }
}
