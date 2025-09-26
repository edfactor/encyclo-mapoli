using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities.Navigations;

public class NavigationRole : ILookupTable<byte>
{
    public static class Contants
    {
        public const byte Administrator = 1;
        public const byte FinanceManager = 2;
        public const byte DistributionClerk = 3;
        public const byte HardshipAdministrator = 4;
        public const byte Impersonation = 5;
        public const byte ItDevOps = 6;
        public const byte ItOperations = 7;
        public const byte ExecutiveAdministrator = 8;
        public const byte Auditor = 9;
        public const byte BeneficiaryAdministrator = 10;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }
    public bool IsReadOnly { get; set; }
}
