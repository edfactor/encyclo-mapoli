
namespace Demoulas.ProfitSharing.Data.Entities;

public sealed class FrozenState
{
    public int Id { get; set; }
    public short ProfitYear { get; set; }
    public bool IsActive { get; set; }
    public string? FrozenBy { get; set; }
    public DateTime AsOfDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
}
