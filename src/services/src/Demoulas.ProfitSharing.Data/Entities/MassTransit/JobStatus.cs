using Demoulas.ProfitSharing.Common.Contracts.Messaging;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.MassTransit;
public sealed class JobStatus : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Pending = 0;
        public const byte Running = 1;
        public const byte Completed = 2;
        public const byte Failed = 99;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public IEnumerable<Job>? Jobs { get; set; }
}
