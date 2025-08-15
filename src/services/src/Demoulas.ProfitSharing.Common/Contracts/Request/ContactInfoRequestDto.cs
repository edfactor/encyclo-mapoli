namespace Demoulas.ProfitSharing.Common.Contracts.Request;
using Demoulas.ProfitSharing.Common.Contracts.Shared;
public record ContactInfoRequestDto : INameParts, IFullNameProperty, IPhoneNumber, IEmailAddress
{
    public string? FullName { get; set; }
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }

    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }
}
