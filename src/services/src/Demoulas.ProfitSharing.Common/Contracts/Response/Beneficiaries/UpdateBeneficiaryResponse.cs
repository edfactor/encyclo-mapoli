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

    public static UpdateBeneficiaryResponse SampleResponse()
    {
        return new UpdateBeneficiaryResponse
        {
            BeneficiaryContactId = 1,
            Relationship = "Spouse",
            Percentage = 100.00m,
            DemographicId = 123,
            BadgeNumber = 1001,
            BeneficiaryId = 456,
            FirstName = "Jane",
            LastName = "Doe",
            MiddleName = "M",
            Ssn = "123456789",
            DateOfBirth = new DateOnly(1985, 5, 15),
            HomePhoneNumber = "555-0100",
            CellPhoneNumber = "555-0101",
            Email = "jane.doe@example.com",
            Address1 = "123 Main St",
            Address2 = "Apt 4B",
            City = "Boston",
            State = "MA",
            ZipCode = "02101"
        };
    }
}
