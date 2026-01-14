namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class ExcludedId
{
    public int Id { get; set; }
    public byte ExcludedIdTypeId { get; set; }
    public ExcludedIdType? ExcludedType { get; set; }
    public int ExcludedIdValue { get; set; }
}
