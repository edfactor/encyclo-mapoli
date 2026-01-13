namespace Demoulas.ProfitSharing.Common.Contracts.Request;

using Shared;
public record ContactInfoRequestDto : INameParts, IFullNameProperty, IPhoneNumber, IEmailAddress
{
    public string? FullName { get; set; }
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }

    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }

    public static ContactInfoRequestDto RequestExample() => new()
    {
        FullName = "John Smith",
        LastName = "Smith",
        FirstName = "John",
        MiddleName = "Michael",
        PhoneNumber = "(555) 123-4567",
        MobileNumber = "(555) 987-6543",
        EmailAddress = "john.smith@example.com"
    };
}
