using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public record ContactInfoResponseDto
{
    [MaskSensitive] public string? FullName { get; set; }
    [MaskSensitive] public required string LastName { get; set; }
    [MaskSensitive] public required string FirstName { get; set; }
    [MaskSensitive] public string? MiddleName { get; set; }
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
