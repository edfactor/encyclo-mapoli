namespace Demoulas.ProfitSharing.Data.Entities.Base;

public abstract class ModifiedBase
{
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string? UserName { get; set; }
    public DateTimeOffset? ModifiedAtUtc { get; set; }
}
