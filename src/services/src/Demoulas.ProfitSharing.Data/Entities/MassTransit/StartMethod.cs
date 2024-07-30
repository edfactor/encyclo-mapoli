using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.MassTransit;
public sealed class StartMethod : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte System = 0;
        public const byte OnDemand = 1;
    }


    public enum StartMethodEnum : byte
    {
        System = 0,
        OnDemand = 1
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public IEnumerable<Job>? Jobs { get; set; }
}
