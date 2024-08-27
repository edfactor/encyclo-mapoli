using System.Collections.Immutable;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DistributionRequestReason : ILookupTable<byte>
{
    public static class Constants
    {
        public const string HOME_PURCHASE = "HOME_PURCHASE";
        public const string HOME_REPAIR = "HOME_REPAIR";
        public const string CAR = "CAR";
        public const string EDUCATION_EXP = "EDUCATION_EXP";
        public const string FUNERAL_EXP = "FUNERAL_EXP";
        public const string MEDICAL_DENTAL = "MEDICAL_DENTAL";
        public const string EVICTION_OR_FORECLOSE = "EVICTION_OR_FORECLOSE";
        public const string OTHER = "OTHER";
    }

    public static readonly ImmutableList<string> Reasons = ImmutableList.Create(
        Constants.HOME_PURCHASE,
        Constants.HOME_REPAIR,
        Constants.CAR,
        Constants.EDUCATION_EXP,
        Constants.FUNERAL_EXP,
        Constants.MEDICAL_DENTAL,
        Constants.EVICTION_OR_FORECLOSE,
        Constants.OTHER
    );

    public byte Id { get; set; }
    public required string Name { get; set; }
}
