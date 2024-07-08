namespace Demoulas.ProfitSharing.Data.Entities;

public class PayFrequency
{
    public static class Constants
    {
        public const byte Weekly = 1;
        public const byte Monthly = 2;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Demographic>? Demographics { get; set; }
}
