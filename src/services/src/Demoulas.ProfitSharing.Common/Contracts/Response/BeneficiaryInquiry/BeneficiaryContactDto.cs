using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
public record BeneficiaryContactDto : IdRequest
{
    public required string Ssn { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public AddressResponseDto? Address { get; set; }
    public ContactInfoResponseDto? ContactInfo { get; set; }

    public DateOnly CreatedDate { get; set; }
}
