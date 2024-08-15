using Demoulas.ProfitSharing.Common.Interfaces;
using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.MassTransit;
public sealed class JobType : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Full = 0;
        public const byte Delta = 1;
        public const byte Individual = 2;
    }


    public byte Id { get; set; }
    public required string Name { get; set; }

    public IEnumerable<Job>? Jobs { get; set; }
}
