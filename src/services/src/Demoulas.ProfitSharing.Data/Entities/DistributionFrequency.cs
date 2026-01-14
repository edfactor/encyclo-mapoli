using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class DistributionFrequency : ILookupTable<char>
{
    public static class Constants
    {
        public const char Monthly = 'M';
        public const char Annually = 'A';
        public const char Quarterly = 'Q';
        public const char Hardship = 'H';
        public const char PayDirect = 'P';
        public const char RolloverDirect = 'R';
    }

    public required char Id { get; set; }
    public required string Name { get; set; }
}
