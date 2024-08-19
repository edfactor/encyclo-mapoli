using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class DistributionRequestReason : ILookupTable<byte>
{
    public static List<string> Reasons = new List<string> {
        "HOME_PURCHASE",
        "HOME_REPAIR",
        "CAR",
        "EDUCATION_EXP",
        "FUNERAL_EXP",
        "MEDICAL_DENTAL",
        "EVICTION_OR_FORECLOSE",
        "OTHER" };

    public byte Id { get; set; }
    public required string Name { get; set; }
}
