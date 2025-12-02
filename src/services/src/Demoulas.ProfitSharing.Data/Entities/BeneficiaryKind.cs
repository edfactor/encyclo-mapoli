using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class BeneficiaryKind : ILookupTable<char>
{
    public static class Constants
    {
        public const char Primary = 'P';
        public const char Secondary = 'S';
    }

    public char Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Beneficiary>? Beneficiaries { get; set; }

}
