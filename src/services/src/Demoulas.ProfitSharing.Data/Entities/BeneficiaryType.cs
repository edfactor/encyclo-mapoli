namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryType
{
    public static class Constants
    {
        public const byte Employee = 0;
        public const byte Beneficiary = 1;
    }

    public required byte Id { get; set; }
    public required string Name { get; set; }

    public ICollection<PayProfit>? Profits { get; set; }
}
