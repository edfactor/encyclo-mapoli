namespace Demoulas.ProfitSharing.Common.Contracts.Request;
public record ContactInfoRequestDto
{
    public string? FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }

    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }
}
