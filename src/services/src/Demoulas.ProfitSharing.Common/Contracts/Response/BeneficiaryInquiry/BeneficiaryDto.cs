using Demoulas.ProfitSharing.Common.Contracts.Request;

namespace Demoulas.ProfitSharing.Common.Contracts.Response.BeneficiaryInquiry;
using Shared;
public partial record BeneficiaryDto : IdRequest, INameParts, IFullNameProperty, IPhoneNumber, IEmailAddress, ICity
{
    public required short PsnSuffix { get; set; } // Suffix for hierarchy (1000, 2000, etc.)

    public required int BadgeNumber { get; set; }
    public required int DemographicId { get; set; }

    public required string Ssn { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public required string Street { get; init; }
    public string? Street2 { get; init; }
    public required string? City { get; init; }
    public required string? State { get; init; }
    public required string? PostalCode { get; init; }
    public required string CountryIso { get; init; }
    public string? FullName { get; init; }
    public required string LastName { get; init; }
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? MobileNumber { get; init; }
    public string? EmailAddress { get; init; }

    public DateOnly CreatedDate { get; set; }
    public string? Relationship { get; set; }
    public char? KindId { get; set; }
    public BeneficiaryKindDto? Kind { get; set; }
    public required decimal Percent { get; set; }
    public decimal? CurrentBalance { get; set; }
}
