using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

public class EmploymentType : LookupTable<byte>
{
    public static class Constants
    {
        public const char PartTime = 'P';
        public const char FullTimeStraightSalary = 'H';
        public const char FullTimeAccruedPaidHolidays = 'G';
        public const char FullTimeEightPaidHolidays = 'F';
    }
    public ICollection<Demographic>? Demographics { get; set; }
}
