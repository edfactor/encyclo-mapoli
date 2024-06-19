namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record ContactInfoRequestDto
{
    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }
}
