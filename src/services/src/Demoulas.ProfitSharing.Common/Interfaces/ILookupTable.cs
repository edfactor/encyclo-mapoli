namespace Demoulas.ProfitSharing.Common.Interfaces;
/// <summary>
/// Generic lookup table contract. (Constraint removed to allow string identifiers.)
/// </summary>
public interface ILookupTable<TType>
{
    public TType Id { get; set; }
    public string Name { get; set; }
}
