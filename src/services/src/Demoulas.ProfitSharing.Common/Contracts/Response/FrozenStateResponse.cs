namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record FrozenStateResponse
{
    public int Id { get; set; }
    public short ProfitYear { get; set; }
    public string? FrozenBy { get; set; }
    public DateTime AsOfDateTime { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public bool IsActive { get; set; }
}
