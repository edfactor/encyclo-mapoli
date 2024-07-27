namespace Demoulas.ProfitSharing.Common.Contracts.Configuration;
public record OktaSettings
{
    public required string ClientId { get; set; }
    public required string Issuer { get; set; }
    public required string AuthorizationEndpoint { get; set; }
    public required string TokenEndpoint { get; set; }
}

