using System.Collections.Immutable;
using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class DistributionRequestType : ILookupTable<byte>
{

    public static class Constants
    {
        public const string HARDSHIP = "HARDSHIP";
        public const string YEARLY = "YEARY";
        public const string ONE_TIME = "ONE_TIME";
        public const string PAYOUT = "PAYOUT";
        public const string ROLLOVER = "ROLLOVER";
    }

    public static readonly ImmutableList<string> Types = ImmutableList.Create(
        Constants.HARDSHIP,
        Constants.YEARLY,
        Constants.ONE_TIME,
        Constants.PAYOUT,
        Constants.ROLLOVER
    );

    public byte Id { get; set; }
    public required string Name { get; set; }
}
