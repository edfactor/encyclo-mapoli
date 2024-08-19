using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class DistributionRequestType : ILookupTable<byte>
{
    public static List<string> Types = new List<string> {
        "HARDSHIP",
        "YEARLY",
        "ONE_TIME",
        "PAYOUT",
        "ROLLOVER",
     };

    public byte Id { get; set; }
    public required string Name { get; set; }
}
