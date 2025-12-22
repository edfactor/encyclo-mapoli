using Demoulas.ProfitSharing.Common.Attributes;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.Beneficiaries;

[NoMemberDataExposed]
public sealed record UpdateBeneficiaryResponse : UpdateBeneficiaryContactResponse
{
    public int BeneficiaryContactId { get; set; }
    public string? Relationship { get; set; }
    public decimal Percentage { get; set; }
    public int DemographicId { get; set; }
    public int BadgeNumber { get; set; }

    public static new UpdateBeneficiaryResponse SampleResponse()
    {
        return new UpdateBeneficiaryResponse
        {
            BeneficiaryContactId = 1,
            Relationship = "Spouse",
            Percentage = 100.00m,
            DemographicId = 123,
            BadgeNumber = 1001,
            // Properties from UpdateBeneficiaryContactResponse base class
            Id = 456,
            Ssn = "XXX-XX-6789",
            DateOfBirth = new DateOnly(1985, 5, 15),
            Street1 = "123 Main St",
            Street2 = "Apt 4B",
            City = "Boston",
            State = "MA",
            PostalCode = "02101",
            FullName = "Doe, Jane M",
            FirstName = "Jane",
            LastName = "Doe",
            MiddleName = "M",
            PhoneNumber = "555-0100",
            MobileNumber = "555-0101",
            EmailAddress = "jane.doe@example.com"
        };
    }
}
