using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
using Shared;
public record ContactInfoResponseDto : INameParts, IFullNameProperty, IPhoneNumber, IEmailAddress
{
    [MaskSensitive] public string? FullName { get; set; }
    [MaskSensitive] public required string LastName { get; init; }
    [MaskSensitive] public required string FirstName { get; init; }
    [MaskSensitive] public string? MiddleName { get; init; }
    [MaskSensitive] public string? PhoneNumber { get; init; }
    [MaskSensitive] public string? MobileNumber { get; init; }
    [MaskSensitive] public string? EmailAddress { get; init; }

    public static ContactInfoResponseDto ResponseExample()
    {
        return new ContactInfoResponseDto
        {
            FullName = "Doe, John",
            LastName = "John",
            FirstName = "Doe",
            PhoneNumber = "978-654-3210",
            MobileNumber = "555-321-9870",
            EmailAddress = "do-not-use@example.com"
        };
    }
}
