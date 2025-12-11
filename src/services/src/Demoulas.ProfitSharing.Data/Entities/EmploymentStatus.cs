using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

public class EmploymentStatus : ILookupTable<char>
{
    public static class Constants
    {
        public const char Active = 'a';
        public const char Inactive = 'i';
        public const char Terminated = 't';
        public const char Delete = 'd';
    }


    public char Id { get; set; }
    public string Name { get; set; } = null!;

    public List<Demographic>? Demographics { get; set; }
}
