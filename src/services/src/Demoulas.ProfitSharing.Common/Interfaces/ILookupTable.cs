namespace Demoulas.ProfitSharing.Common.Interfaces;
/// <summary>
/// Generic lookup table contract. (Constraint removed to allow string identifiers.)
/// </summary>
public interface ILookupTable<TType>
{
    TType Id { get; set; }
    string Name { get; set; }
}
