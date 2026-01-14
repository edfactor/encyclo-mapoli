using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class EmploymentType : ILookupTable<char>
{
    public static class Constants
    {
        public const char PartTime = 'P';
        public const char FullTimeStraightSalary = 'H';
        public const char FullTimeAccruedPaidHolidays = 'G';
        public const char FullTimeEightPaidHolidays = 'F';
    }

    public char Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Demographic>? Demographics { get; set; }
}
