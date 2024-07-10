using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://www.mass.gov/news/massachusetts-allows-nonbinary-marker-on-licenses-ids
/// https://transequality.org/documents/state/new-hampshire#:~:text=New%20Hampshire%20Drivers%20License%20Policy,documentation%20of%20the%20name%20change.
/// </summary>
public sealed class Gender : LookupTable<char>
{
    public static class Constants
    {
        public const char Male = 'M';
        public const char Female = 'F';
        public const char Other = 'X';
    }

    public ICollection<Demographic>? Demographics { get; set; }
}
