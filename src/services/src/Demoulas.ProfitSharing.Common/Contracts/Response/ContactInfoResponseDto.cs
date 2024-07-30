namespace Demoulas.ProfitSharing.Common.Contracts.Response;
public record ContactInfoResponseDto
{
    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }

    public static ContactInfoResponseDto ResponseExample()
    {
        return new ContactInfoResponseDto
        {
            PhoneNumber = "978-654-3210",
            MobileNumber = "555-321-9870",
            EmailAddress = "do-not-use@example.com"
        };
    }
}
