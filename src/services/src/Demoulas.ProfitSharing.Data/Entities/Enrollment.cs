using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class Enrollment : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte NotEnrolled = 0;
        public const byte OldVestingPlanHasContributions = 1;
        public const byte NewVestingPlanHasContributions = 2;
        public const byte OldVestingPlanHasForfeitureRecords = 3;
        public const byte NewVestingPlanHasForfeitureRecords = 4;
        public const byte Import_Status_Unknown = 9;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<PayProfit>? Profits { get; set; }
}
