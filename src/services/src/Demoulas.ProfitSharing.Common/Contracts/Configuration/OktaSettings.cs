namespace Demoulas.ProfitSharing.Common.Contracts.Configuration;
public record OktaSettings
{
    public required string ClientId { get; init; }
    public required string Issuer { get; init; }
    public required string AuthorizationEndpoint { get; init; }
    public required string TokenEndpoint { get; init; }
}

