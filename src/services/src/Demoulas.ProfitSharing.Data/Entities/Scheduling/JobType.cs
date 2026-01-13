using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.Scheduling;

public sealed class JobType : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte EmployeeSyncFull = 0;
        public const byte PayrollSyncFull = 1;
        public const byte EmployeeSyncDelta = 2;
    }


    public byte Id { get; set; }
    public required string Name { get; set; }

    public IEnumerable<Job>? Jobs { get; set; }
}
