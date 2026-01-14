using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.Scheduling;

public sealed class StartMethod : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte System = 0;
        public const byte OnDemand = 1;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public IEnumerable<Job>? Jobs { get; set; }
}
