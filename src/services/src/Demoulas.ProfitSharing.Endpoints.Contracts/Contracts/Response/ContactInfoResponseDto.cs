namespace Demoulas.ProfitSharing.Endpoints.Contracts.Contracts.Response;
public record ContactInfoResponseDto
{
    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }
}
