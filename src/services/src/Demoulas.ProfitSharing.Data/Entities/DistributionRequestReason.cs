using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DistributionRequestReason : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte CAR = 1;
        public const byte EDUCATION_EXP = 2;
        public const byte EVICTION_OR_FORECLOSE = 3;
        public const byte FUNERAL_EXP = 4;
        public const byte HOME_PURCHASE = 5;
        public const byte HOME_REPAIR = 6;
        public const byte MEDICAL_DENTAL = 7;
        public const byte OTHER = 8;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }
}
