namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public record ContactInfoResponseDto
{
    public string? FullName { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }

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
