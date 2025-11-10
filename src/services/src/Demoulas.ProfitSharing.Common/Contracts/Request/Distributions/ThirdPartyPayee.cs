namespace Demoulas.ProfitSharing.Common.Contracts.Request.Distributions;

public sealed record ThirdPartyPayee
{
    public string? Payee { get; set; }
    public string? Name { get; set; }
    public string? Account { get; set; }
    public required Address Address { get; set; }
    public string? Memo { get; set; }
}