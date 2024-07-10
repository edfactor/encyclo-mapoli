using Demoulas.ProfitSharing.Data.Entities.Base;

namespace Demoulas.ProfitSharing.Data.Entities;
public sealed class Country : LookupTable<short>
{
    public required string ISO { get; init; }
    public required string TelephoneCode { get; init; }
}
