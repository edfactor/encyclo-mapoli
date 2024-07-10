namespace Demoulas.ProfitSharing.Data.Entities.Base;
public abstract class LookupTable<TType>
{
    public required TType Id { get; set; }
    public required string Name { get; set; }
}
