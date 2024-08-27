namespace Demoulas.ProfitSharing.Common.Interfaces;
public interface ILookupTable<TType> where TType : struct
{
    public TType Id { get; set; }
    public string Name { get; set; }
}
